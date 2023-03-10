using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    //anim var

    public Transform attackPoint;
    public float attackRange = 0.5f;
    public Light attackFlash;
    private float lightDuration = 20f;

    public LayerMask enemyLayer;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Attack();
        }

        //not super broken, adjust durations & fix hangup
        if (attackFlash.intensity >= 0.1f && lightDuration > 0f)
        {
            attackFlash.intensity -= 0.1f;

            lightDuration -= 0.1f;
        }

    }

    void Attack()
    {
        //Anim later

        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider enemy in hitEnemies)
        {
            attackFlash.intensity = 5f;
            Debug.Log("HIT" + enemy.name);
            //attackFlash.intensity = 0f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
