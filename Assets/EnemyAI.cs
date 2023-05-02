using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FMOD;
using FMODUnity;

public class EnemyAI : MonoBehaviour
{
    private Transform player;
    private NavMeshAgent enemyNavMeshAgent;

    public Transform enemyArm1;
    public Transform enemyArm2;
    private Quaternion armsRotation;

    public float pursuitDistance;
    public float battleDistance;
    
    public float attackInterval;
    public float greetInterval;
    public float loomInterval;
    private float greetTimer;
    private float loomTimer;

    public Transform enemyAttackPoint;
    public float enemyAttackRange = 0.5f;

    public LayerMask playerLayer;

    private EnemyState currentState;

    private Vector3 idlePos;
    public float idleRad;
    public float idleWaitTime;

    public StudioEventEmitter callGreeting;
    public StudioEventEmitter callLooming;
    public StudioEventEmitter callAttacking;
    public StudioEventEmitter breathingLoop;

    private PlayerMovement playerMovement;

    private EnemyDamage enemyDamage;

    private GameObject playerLantern;

    bool shouldBreathe;

    private void Awake()
    {
        greetTimer = Random.Range(5f, 8f);

        shouldLoom = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        enemyNavMeshAgent = GetComponent<NavMeshAgent>();
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        enemyDamage = GetComponent<EnemyDamage>();
        playerLantern = GameObject.FindGameObjectWithTag("Lantern");

        idlePos = this.transform.position;

        breathingLoop.Play();
        shouldBreathe = false;
        playerFound = false;
        searchingForPlayer = false;

        //enemyArm1.transform.rotation = armsRotation;
        //enemyArm2.transform.rotation = armsRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerLantern.GetComponent<Light>().enabled == true && playerFound == false)
        {
            DoTrackLight();
        }
        else
        {
            if (searchingForPlayer == true && playerLantern.GetComponent<Light>().enabled == false)
            {
                enemyNavMeshAgent.ResetPath();
                searchingForPlayer = false;
            }

            switch (currentState)
            {
                case EnemyState.Idle:
                    DoIdle();
                    break;
                case EnemyState.Pursuit:
                    DoPursuit();
                    break;
                case EnemyState.Battle:
                    DoBattle();
                    break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.matrix *= Matrix4x4.Scale(new Vector3(1, 0, 1));

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(this.transform.position, pursuitDistance);
    }

    float idleTimer;
    bool reachedDestinationLastFrame;

    void DoIdle ()
    {
        Greeting();

        playerFound = false;
        searchingForPlayer = false;

        if (shouldBreathe == true)
        {
            breathingLoop.Play();
            shouldBreathe = false;
        }

        bool reachedDestinationCurrentFrame = hasNavMeshAgentReachedDestination();

        if (reachedDestinationCurrentFrame && (reachedDestinationLastFrame == false))
        {
            idleTimer = idleWaitTime;
            shouldLoom = true;
        }
        reachedDestinationLastFrame = reachedDestinationCurrentFrame;

        if (reachedDestinationCurrentFrame)
        {
            idleTimer -= Time.deltaTime;

            if (idleTimer <= 0)
            {
                Vector2 randomizedPos = Random.insideUnitCircle * idleRad;

                enemyNavMeshAgent.destination = idlePos + new Vector3(randomizedPos.x, 0, randomizedPos.y);
            }
        }

        if (Vector3.Distance(player.position, this.transform.position) <= pursuitDistance && playerMovement.isSneaking == false)
        {
            currentState = EnemyState.Pursuit;
        }

        if (Vector3.Distance(player.position, this.transform.position) <= battleDistance && enemyDamage.wasAttacked == true)
        {
            currentState = EnemyState.Battle;
        }
    }

    bool shouldLoom;

    void DoPursuit()
    {
        enemyDamage.wasAttacked = false;
        playerFound = true;
        searchingForPlayer = false;

        if (shouldLoom == true)
        {
            callLooming.Play();
            shouldLoom = false;
        }

        if (shouldBreathe == false)
        {
            breathingLoop.Stop();
            shouldBreathe = true;
        }

        enemyNavMeshAgent.SetDestination(player.position);

        if (Vector3.Distance(player.position, this.transform.position) >= pursuitDistance)
        {
            currentState = EnemyState.Idle;

            idlePos = player.position;
        }

        if (Vector3.Distance(player.position, this.transform.position) <= battleDistance)
        {
            currentState = EnemyState.Battle;
        }
    }

    float attackTimer;

    void DoBattle()
    {
        enemyDamage.wasAttacked = false;
        shouldLoom = false;
        playerFound = true;
        searchingForPlayer = false;

        if (shouldBreathe == false)
        {
            breathingLoop.Stop();
            shouldBreathe = true;
        }

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0)
        {
            EnemyAttack();
            attackTimer = attackInterval;
        }
        Vector3 directionToPlayer = (player.position - this.transform.position);
        directionToPlayer.y = 0;
        directionToPlayer = directionToPlayer.normalized;

        this.transform.rotation = Quaternion.Euler(0, Vector3.SignedAngle(Vector3.forward, directionToPlayer, Vector3.up), 0);

        if (Vector3.Distance(player.position, this.transform.position) >= battleDistance)
        {
            currentState = EnemyState.Pursuit;
        }
    }

    bool playerFound;
    bool searchingForPlayer;

    void DoTrackLight()
    {
        enemyNavMeshAgent.SetDestination(player.position);
        searchingForPlayer = true;

        if (Vector3.Distance(player.position, this.transform.position) <= pursuitDistance)
        {
            playerFound = true;
            searchingForPlayer = false;
            currentState = EnemyState.Pursuit;
        }
    }

    void Greeting()
    {
        greetTimer -= Time.deltaTime;

        if (greetTimer <= 0)
        {
            callGreeting.Play();

            greetTimer = greetInterval;

            greetTimer += Random.Range(1f, 3f);
        }
    }

    void EnemyAttack()
    {
        Collider[] hitPlayer = Physics.OverlapSphere(enemyAttackPoint.position, enemyAttackRange, playerLayer);

        foreach (Collider player in hitPlayer)
        {
            player.GetComponent<PlayerLife>().EnemyAttacked();
            callAttacking.Play();
        }
    }

    bool hasNavMeshAgentReachedDestination ()
    {
        if (enemyNavMeshAgent.pathPending == false)
        {
            if (enemyNavMeshAgent.remainingDistance <= enemyNavMeshAgent.stoppingDistance)
            {
                return true;
            }
        }

        return false;
    }

    /*
    private IEnumerator TurnArms()
    {
        float swingDuration = 0.08f;
        float timer = 0f;

        while (timer <= swingDuration)
        {
            timer += Time.deltaTime;

            float t = timer / swingDuration;

            enemyArm1.transform.localRotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(90f, 0f, 0f), t);

            yield return new WaitForEndOfFrame();
        }

        timer = 0f;
        float swingDurationUp = 0.1f;

        while (timer <= swingDurationUp)
        {
            timer += Time.deltaTime;

            float t = timer / swingDurationUp;

            playerSword.transform.localRotation = Quaternion.Slerp(Quaternion.Euler(90f, 0f, 0f), Quaternion.identity, t);

            yield return new WaitForEndOfFrame();
        }
    }
    */

    enum EnemyState 
    {
        Idle,
        Pursuit,
        Battle,
        //Deathstate?
    }
}
