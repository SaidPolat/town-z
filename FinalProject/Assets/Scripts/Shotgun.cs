using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Shotgun : MonoBehaviour
{
    public Camera fpsCam;

    public AudioManager audioManager;
    public AudioSource[] sources;

    public static bool isShooting = false;
    bool isShooted = false;
    bool isMovingController = true;
    bool stopController = false;
    bool reloadOnce = false;
    int counter = 0;
    public static bool canFire = true;
    public static bool canReload = true;
    float fireRate;
    Vector3 rot;

    [Header("Weapon Settings")]
    [SerializeField] float spread;
    [SerializeField] int bulletsPerTap;
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
    public ParticleSystem muzzleFlash;
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

        sources = GetComponents<AudioSource>();

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

        if (Input.GetButtonDown("Fire1") && canFire && !PauseMenu.GameIsPaused && Time.time > fireRate && bulletLast != 0 && !GameControl.isMarketOpen)
        {
            StartCoroutine(Shoot());
            fireRate = Time.time + fireRateCustomize;
        }

        if (bulletLast == 0 && bulletLast < magazineAmount && ammoAmount != 0 && !reloadOnce)
        {
            reloadOnce = true;
            StartCoroutine(Reload());
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (bulletLast < magazineAmount && ammoAmount != 0 && canReload)
            {
                if (bulletLast != 0)
                    StartCoroutine(Reload());
                else
                    StartCoroutine(Reload());
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

    IEnumerator Shoot()
    {
        isShooted = true;
        isShooting = true;

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
        muzzleFlash.Play();

        

        for(int i = 0; i < bulletsPerTap; i++)
        {
            SpreadShoot();
        }
       
        if (counter == 4)
        {
            counter = -1;
        }
        Recoil();
        counter++;
        yield return new WaitForSeconds(0.8f);
        isShooted = false;
        isShooting = false;
    }

    void SpreadShoot()
    {
        RaycastHit hit;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0);

        if (Physics.Raycast(fpsCam.transform.position, direction, out hit, range))
        {
            if (hit.transform.gameObject.CompareTag("Shootable"))
            {
                Instantiate(shootableEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Rigidbody rg = hit.transform.gameObject.GetComponent<Rigidbody>();
                rg.AddForce(transform.forward * 100f);
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
        isShooting = false;
    }

    private void Recoil()
    {
        float recX = Random.Range(minX, maxX);  

        fpsCam.transform.localRotation = Quaternion.Euler(rot.x - maxY, rot.y + recX, rot.z);
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(0.2f);

        animator.Play("Reload");

        yield return new WaitForSeconds(0.5f);

        for(int i = 0; i < 8; i++)
        {
            yield return new WaitForSeconds(0.8f);

            //Debug.Log("is shooted " + isShooted);

            if(bulletLast < 8 && ammoAmount > 0)
            {
                ammoAmount--;
                bulletLast++;
            }

            if(bulletLast == 8)
            {
                animator.CrossFade("Idle", 0.15f, 1);
                break;
            }

            if (isShooted)
            {
                break;
            }
           
        }
        
        reloadOnce = false;
    } 

    public void ReloadSound1()
    {
        audioManager.Play("Shotgun Reload1");
    }

    public void ReloadSound2()
    {
        audioManager.Play("Shotgun Reload2");
    }

    public void SwitchSound()
    {
        audioManager.Play("Shotgun Switch");
    }
}
