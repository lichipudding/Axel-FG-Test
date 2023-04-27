using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class PlayerCombat : MonoBehaviour
{
    public Transform playerSword;
    private Quaternion playerSwordRotation;
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public Light attackFlash;
    private float waitBetweenAttacks = 40f;

    public GameObject lantern;

    public LayerMask enemyLayer;

    public StudioEventEmitter playerSwing;
    public StudioEventEmitter playerLantern;

    private void Start()
    {
        playerSword.transform.rotation = playerSwordRotation;

        playerSwing = GetComponent<StudioEventEmitter>();

        lantern.GetComponent<Light>().enabled = false;

        playerLantern.Stop();
        playerLantern.SetParameter("LanternState", 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && waitBetweenAttacks >= 40f)
        {
            StopCoroutine("TurnSword");
            StartCoroutine("TurnSword");
            playerSwing.Play();
            Attack();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            //playerLantern.SetParameter("LanternState", 1f);

            if (lantern.GetComponent<Light>().enabled == false)
            {
                lantern.GetComponent<Light>().enabled = true;

                playerLantern.Play();
                playerLantern.SetParameter("LanternState", 1f);
            }
            else if (lantern.GetComponent<Light>().enabled == true)
            {
                lantern.GetComponent<Light>().enabled = false;

                playerLantern.Play();
                playerLantern.SetParameter("LanternState", 0f);
            }


        }
    }

    void Attack()
    {
        //Anim later

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            StopCoroutine("LightFade");
            //playerSwordRotation.x = 90f;
            enemy.GetComponent<EnemyDamage>().Attacked();
            Debug.Log("HIT" + enemy.name);
            StartCoroutine("LightFade");
        }
    }

    private IEnumerator TurnSword()
    {
        float swingDuration = 0.08f;
        float timer = 0f;

        while (timer <= swingDuration)
        {
            timer += Time.deltaTime;

            float t = timer / swingDuration;

            playerSword.transform.localRotation = Quaternion.Slerp(Quaternion.identity, Quaternion.Euler(90f, 0f, 0f), t);

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



    private IEnumerator LightFade()
    {
        float lightDuration = 1f;
        float interval = 0.01f;
        attackFlash.intensity = 8f;

        waitBetweenAttacks = 0f;

        while (lightDuration >= 0.0f)
        {
            attackFlash.intensity -= 0.15f;

            lightDuration -= interval;

            waitBetweenAttacks += 1f;

            yield return new WaitForSeconds(interval);
        }
        //playerSwordRotation.x = 0f;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
