using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    Rigidbody playerBody;

    [SerializeField] float onHitPushBack = 10f;

    void Start()
    {
        playerBody = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("EnemyBody"))
        {
            EnemyHit();
        }
    }

    void EnemyHit()
    {
        playerBody.velocity = new Vector3(0f, 0f, -onHitPushBack);
    }
}
