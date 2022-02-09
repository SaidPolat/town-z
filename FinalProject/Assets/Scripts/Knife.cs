using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Knife : MonoBehaviour
{
    public Camera fpsCam;

    public AudioManager audioManager;
    public AudioSource[] sources1;

    [HideInInspector]public bool doesHaveFade = false;
    public static bool isShooting = false;
    bool isMovingController = true;
    bool stopController = false;
    int counter = 2;
    int counter2 = 7;
    public static bool canFire = true;
    float leftRate;
    float rightRate;

    [Header("Settings")]
    public GameObject knifeBlack;
    public GameObject knifeFade;
    public float leftClickRateCustomize;
    public float rightClickRateCustomize;
    public float range;
    public int BlackRightClickDamage;
    public int BlackLeftClickDamage;
    public int FadeRightClickDamage;
    public int FadeLeftClickDamage;

    [Header("Texts")]
    public TextMeshProUGUI totalAmmoText;
    public TextMeshProUGUI magazineAmmoText;
    public TextMeshProUGUI slash;

    [Header("Animators")]
    public Animator animator;

    [Header("Effects")]
    public ParticleSystem shootableEffect;
    public ParticleSystem bulletHitOnStone;
    public ParticleSystem bulletHitOnFlesh;
    public ParticleSystem bulletHitOnMetal;
    public ParticleSystem bulletHitOnSand;
    public ParticleSystem bulletHitOnWood;

    void Start()
    {
        totalAmmoText.text = "";
        magazineAmmoText.text = "";
        slash.text = "";

        sources1 = GetComponents<AudioSource>();

        if (PlayerMovement.isMoving)
        {
            animator.SetTrigger("StopRun");
        }
    }

    void Update()
    {
        if (PlayerMovement.isMoving && isMovingController)
        {
            animator.SetTrigger("StartRun");
            isMovingController = false;
            stopController = true;
        }
        if (!PlayerMovement.isMoving && stopController)
        {
            animator.SetTrigger("StopRun");
            stopController = false;
            isMovingController = true;
        }

        if (Input.GetButton("Fire1") && canFire && !PauseMenu.GameIsPaused && Time.time > leftRate)
        {
            Shoot(1);
            leftRate = Time.time + leftClickRateCustomize;
        }

        if (Input.GetKey(KeyCode.Mouse1) && canFire && !PauseMenu.GameIsPaused && Time.time > rightRate)
        {
            Shoot(2);
            rightRate = Time.time + rightClickRateCustomize;
        }
        totalAmmoText.text = "";
        magazineAmmoText.text = "";
    }

    void Shoot(int type)
    {
        isShooting = true;

        RaycastHit hit;
        float forceAmount = 50f;

        if(type == 1)
        {
            animator.Play("LeftClickAttack");
            sources1[0].Play();
            forceAmount = 70f;
        }
        else if(type == 2)
        {
            animator.Play("RightClickAttack");
            sources1[1].Play();
            forceAmount = 100f;
        }

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            if (hit.transform.gameObject.CompareTag("Enemy"))
            {
                if (type == 1) 
                {
                    sources1[counter2].Play();
                    if (!doesHaveFade)
                    {
                        hit.transform.gameObject.GetComponent<Enemy>().GetDamage(BlackLeftClickDamage);
                    }
                    else
                    {
                        hit.transform.gameObject.GetComponent<Enemy>().GetDamage(FadeLeftClickDamage);
                    }
                    
                }
                else if(type == 2)
                {
                    sources1[9].Play();
                    if (!doesHaveFade)
                    {
                        hit.transform.gameObject.GetComponent<Enemy>().GetDamage(BlackRightClickDamage);
                    }
                    else
                    {
                        hit.transform.gameObject.GetComponent<Enemy>().GetDamage(FadeRightClickDamage);
                    }                
                }
            }
            else
            {
                sources1[counter].Play();
            }

            if (hit.transform.gameObject.CompareTag("Shootable"))
            {
                Instantiate(shootableEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Rigidbody rg = hit.transform.gameObject.GetComponent<Rigidbody>();
                rg.AddForce(transform.forward * forceAmount);
            }       
            else if (hit.transform.gameObject.CompareTag("SandMaterial"))
            {
                Instantiate(bulletHitOnSand, hit.point, Quaternion.LookRotation(hit.normal));
            }
            else if (hit.transform.gameObject.CompareTag("WoodMaterial"))
            {
                Instantiate(bulletHitOnWood, hit.point, Quaternion.LookRotation(hit.normal));
            }
            else if (hit.transform.gameObject.CompareTag("MetalMaterial"))
            {
                Instantiate(bulletHitOnMetal, hit.point, Quaternion.LookRotation(hit.normal));
            }
            else if (hit.transform.gameObject.CompareTag("StoneMaterial"))
            {
                Instantiate(bulletHitOnStone, hit.point, Quaternion.LookRotation(hit.normal));
            }
            else
            {
                if(hit.transform.gameObject.CompareTag("Enemy"))
                    Instantiate(bulletHitOnFlesh, hit.point, Quaternion.LookRotation(hit.normal));
                else
                    Instantiate(bulletHitOnMetal, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }

        if (counter == 6)
        {
            counter = 1;
        }
        if (counter2 == 8)
        {
            counter2 = 6;
        }
        counter++;
        counter2++;

        isShooting = false;
    }

}
