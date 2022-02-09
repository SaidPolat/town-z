using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveBarrel : MonoBehaviour
{
    public int health;
    public int explosionDamage;
    public float explosionForce;
    public float explosionRadius;

    public GameObject explosionEffect;
    public GameObject fireEffect;
    public GameObject destroyedVersion;

    public void GetDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Explode();
        }
    }

    public void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);
        Instantiate(fireEffect, transform.position, transform.rotation);
        Instantiate(destroyedVersion, transform.position, transform.rotation);
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
}
