using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class EnemyDamage : MonoBehaviour
{
    public StudioEventEmitter impactBlade;
    private Vector3 currentPos;
    public GameObject droppedLight;
    public StudioEventEmitter deathSound;

    public bool wasAttacked = false;

    public int enemyHealth;

    private void Start()
    {
        impactBlade = GetComponent<StudioEventEmitter>();
    }

    private void Update()
    {
        currentPos = this.transform.position;

        if (enemyHealth <= 0)
        {
            Instantiate(droppedLight, currentPos, Quaternion.identity);

            deathSound.Play();

            Destroy(gameObject);
        }
    }

    public void Attacked()
    {
        impactBlade.Play();

        enemyHealth--;

        wasAttacked = true;
    }
}
