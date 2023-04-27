using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    Rigidbody playerBody;
    public int playerHealth;

    [SerializeField] float onHitPushBack = 10f;

    void Start()
    {
        playerBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (playerHealth <= 0)
        {
            Destroy(gameObject);
        }
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
        Vector3 hit = new Vector3(0f, 0f, -onHitPushBack);

        playerBody.AddForce(hit);
    }

    public void EnemyAttacked()
    {
        playerHealth--;
    }
}
