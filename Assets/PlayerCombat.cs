using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    //anim var

    public Transform attackPoint;
    public float attackRange = 0.5f;
    public Light attackFlash;
    private float waitBetweenAttacks = 1f;

    public LayerMask enemyLayer;

    // Update is called once per frame
    void Update()
    {
        // 
        if (Input.GetKeyDown(KeyCode.E) && waitBetweenAttacks >= 1f)
        {
            Attack();
        }
    }

    void Attack()
    {
        //Anim later

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            Debug.Log("HIT" + enemy.name);
            StartCoroutine("LightFade");
        }
    }



    private IEnumerator LightFade()
    {
        float lightDuration = 1f;
        float interval = 0.01f;
        attackFlash.intensity = 5f;

        while (lightDuration >= 0.0f)
        {
            attackFlash.intensity -= 0.1f;

            lightDuration -= interval;

            waitBetweenAttacks -= 0.1f;

            yield return new WaitForSeconds(interval);
        }
        
        waitBetweenAttacks = 1f;
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
