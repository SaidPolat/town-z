using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Generator : MonoBehaviour
{
    public int explosionDamage;
    public float explosionForce;
    public float explosionRadius;
    public int health;
    public bool genoratorAlive;
    public Slider slider;
    public GameObject explosionEffect;
    public GameObject fireEffect;
    public TextMeshProUGUI healthText;
    public Animator animator;

    bool doOnce = true;

    private void Start()
    {
        slider.value = 100;
        healthText.text = "100";
    }

    public void GetDamage(int enemyDamage)
    {
        health -= enemyDamage;

        if(health > 0)
        {
            slider.value = health;
            healthText.text = health.ToString();
        }
        else
        {
            slider.value = 0;
            healthText.text = "0";
        }

        if(animator.isActiveAndEnabled)
            animator.Play(gameObject.name + "Damage");

        //Debug.Log(gameObject.name + " can: " + health);

        if(health <= 0)
        {
            genoratorAlive = false;

            if (doOnce)
            {
                doOnce = false;
                Explode();
            }
                
        }
    }

    public void Explode()
    {
        Instantiate(explosionEffect, transform.position, transform.rotation);
        Instantiate(fireEffect, transform.position, transform.rotation);
        FindObjectOfType<AudioManager>().Play("Explosion");
        StartCoroutine(FindObjectOfType<CameraShake>().Shake(.2f, .5f));

        Collider[] collidersToMove = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach(Collider nearbyObject in collidersToMove)
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            Enemy enemy = nearbyObject.GetComponent<Enemy>();
            if(enemy != null)
            {
                enemy.GetDamage(explosionDamage);
            }
        }

    }
   
}
