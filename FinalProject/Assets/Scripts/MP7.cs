using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MP7 : MonoBehaviour
{
    public Camera fpsCam;

    public AudioManager audioManager;
    public AudioSource[] sources;

    public static bool isShooting = false;
    bool isMovingController = true;
    bool stopController = false;
    bool reloadOnce = false;
    int counter = 0;
    public static bool canReload = true;
    public static bool canFire = true;
    float fireRate;
    Vector3 rot;

    [Header("Weapon Settings")]
    [SerializeField] float fireRateCustomize;
    [SerializeField] float range;
    [SerializeField] int damage;
    public int ammoAmount;
    [SerializeField] int magazineAmount;
    [SerializeField] int bulletLast;
    public TextMeshProUGUI totalAmmoText;
    public TextMeshProUGUI magazineAmmoText;

    [Header("Recoil Settings")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

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
        bulletLast = magazineAmount;

        totalAmmoText.text = ammoAmount.ToString();
        magazineAmmoText.text = bulletLast.ToString();

        //sources = GetComponents<AudioSource>();

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

        if (Input.GetButton("Fire1") && canFire && !PauseMenu.GameIsPaused && Time.time > fireRate && bulletLast != 0 && !GameControl.isMarketOpen)
        {
            Shoot();
            fireRate = Time.time + fireRateCustomize;
        }

        if (bulletLast == 0 && bulletLast < magazineAmount && ammoAmount != 0 && !reloadOnce)
        {
            reloadOnce = true;
            StartCoroutine(Reload("NoAmmo"));
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (bulletLast < magazineAmount && ammoAmount != 0 && canReload)
            {
                if (bulletLast != 0)
                    StartCoroutine(Reload("StillHasAmmo"));
                else
                    StartCoroutine(Reload("NoAmmo"));
            }

        }

        rot = fpsCam.transform.localRotation.eulerAngles;  //kameranýn açýsal deðerlerini vector 3 e kaydettik

        if (rot.x != 0 || rot.y != 0)
        {
            fpsCam.transform.localRotation = Quaternion.Slerp(fpsCam.transform.localRotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * 2);
        }

        totalAmmoText.text = ammoAmount.ToString();
        magazineAmmoText.text = bulletLast.ToString();
    }

    void Shoot()
    {
        isShooting = true;

        RaycastHit hit;

        if (PlayerMovement.isMoving)
        {
            animator.Play("RunningShoot");
        }
        else
        {
            animator.Play("Shoot");
        }

        bulletLast--;
        magazineAmmoText.text = bulletLast.ToString();

        

        sources[counter].Play();
        //Debug.Log(counter);

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            if (hit.transform.gameObject.CompareTag("Shootable"))
            {
                Instantiate(shootableEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Rigidbody rg = hit.transform.gameObject.GetComponent<Rigidbody>();
                rg.AddForce(transform.forward * 70f);
            }
            else if (hit.transform.gameObject.CompareTag("Enemy"))
            {
                Instantiate(bulletHitOnFlesh, hit.point, Quaternion.LookRotation(hit.normal));
                hit.transform.gameObject.GetComponent<Enemy>().GetDamage(damage);
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
            else if (hit.transform.gameObject.CompareTag("ExplosiveBarrel"))
            {
                Instantiate(bulletHitOnMetal, hit.point, Quaternion.LookRotation(hit.normal));
                hit.transform.gameObject.GetComponent<ExplosiveBarrel>().GetDamage(damage);
            }
            else
            {
                Instantiate(bulletHitOnMetal, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }

        if (counter == 4)
        {
            counter = -1;
        }

        Recoil();

        counter++;

        isShooting = false;
    }

    private void Recoil()
    {
        float recX = Random.Range(minX, maxX);
        float recY = Random.Range(minY, maxY);

        fpsCam.transform.localRotation = Quaternion.Euler(rot.x - recY, rot.y + recX, rot.z);
    }

    IEnumerator Reload(string kind)
    {
        canFire = false;

        animator.Play("Reload");

        yield return new WaitForSeconds(1.5f);

        switch (kind)
        {
            case "StillHasAmmo":
                if (ammoAmount <= magazineAmount)
                {
                    int temp = bulletLast + ammoAmount;

                    if (temp > magazineAmount)
                    {
                        bulletLast = magazineAmount;
                        ammoAmount = temp - magazineAmount;
                    }
                    else
                    {
                        bulletLast += ammoAmount;
                        ammoAmount = 0;
                    }
                }
                else
                {
                    ammoAmount -= magazineAmount - bulletLast;
                    bulletLast = magazineAmount;
                }

                totalAmmoText.text = ammoAmount.ToString();
                magazineAmmoText.text = bulletLast.ToString();
                break;
            case "NoAmmo":
                if (ammoAmount <= magazineAmount)
                {
                    bulletLast = ammoAmount;
                    ammoAmount = 0;
                }
                else
                {
                    ammoAmount -= magazineAmount;
                    bulletLast = magazineAmount;
                }
                totalAmmoText.text = ammoAmount.ToString();
                magazineAmmoText.text = bulletLast.ToString();
                break;
        }

        yield return new WaitForSeconds(2f);

        canFire = true;
        reloadOnce = false;
    }

    public void ReloadSound1()
    {
        audioManager.Play("MP7 Reload1");
    }

    public void ReloadSound2()
    {
        audioManager.Play("MP7 Reload2");
    }

    public void ReloadSound3()
    {
        audioManager.Play("MP7 Reload3");
    }

    public void SwitchSound()
    {
        audioManager.Play("MP7 Switch");
    }
}
