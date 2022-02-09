using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float delay;
    public float explosionRadius;
    public GameObject explosionEffect;
    public float explosionForce;
    public int explosionDamage;

    float countdown;
    bool hasExploded = false;

    
    void Start()
    {
        countdown = delay;
    }

    void Update()
    {
        countdown -= Time.deltaTime;
        if(countdown <= 0f && !hasExploded)
        {
            Explode();
            hasExploded = true;
        }
    }

    public void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);
        FindObjectOfType<AudioManager>().Play("Grenade Explosion");
        StartCoroutine(FindObjectOfType<CameraShake>().Shake(.25f, .4f));

        Collider[] collidersToMove = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider nearbyObject in collidersToMove)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            Enemy enemy = nearbyObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.GetDamage(explosionDamage);
            }
        }

        Destroy(gameObject, 0.3f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        FindObjectOfType<AudioManager>().Play("Grenade Hit");
    }
}
