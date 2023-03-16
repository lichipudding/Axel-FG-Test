using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    //anim var

    public Transform attackPoint;
    public float attackRange = 0.5f;
    public Light attackFlash;
    private float waitBetweenAttacks = 40f;

    public LayerMask enemyLayer;

    // Update is called once per frame
    void Update()
    {
        //
        if (Input.GetKeyDown(KeyCode.E) && waitBetweenAttacks >= 40f)
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
            StopCoroutine("LightFade");
            Debug.Log("HIT" + enemy.name);
            StartCoroutine("LightFade");
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
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
