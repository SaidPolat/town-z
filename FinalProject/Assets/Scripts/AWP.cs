using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AWP : MonoBehaviour
{
    public Camera fpsCam;
    public GameObject cameraHolder;
  
    public AudioManager audioManager;
    public GameObject crosshair;
    public GameObject scope1;
    public GameObject scope2;
    public GameObject player;

    public static bool isShooting = false;
    bool isMovingController = true;
    bool stopController = false;
    bool reloadOnce = false;
    public static bool canOpenScope = true;
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
    public ParticleSystem muzzleFlash;
    public ParticleSystem shootableEffect;
    public ParticleSystem bulletHitOnStone;
    public ParticleSystem bulletHitOnFlesh;
    public ParticleSystem bulletHitOnMetal;
    public ParticleSystem bulletHitOnSand;
    public ParticleSystem bulletHitOnWood;

    bool isScopeOpen = false;
    float camFOV;
    float zoomPov = 15f;
    PlayerMovement movementScript;
    MouseLook mouseLookScript;

    void Start()
    {
        bulletLast = magazineAmount;

        movementScript = player.GetComponent<PlayerMovement>();
        mouseLookScript = cameraHolder.GetComponent<MouseLook>();

        totalAmmoText.text = ammoAmount.ToString();
        magazineAmmoText.text = bulletLast.ToString();

        camFOV = fpsCam.fieldOfView;

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
            StartCoroutine(Shoot());
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && canOpenScope)
        {
            audioManager.Play("AWP Scope");
            if (!isScopeOpen)
            {         
                OpenScope();
            }
            else
            {    
                CloseScope();
            }
        }

        if (bulletLast == 0 && bulletLast < magazineAmount && ammoAmount != 0 && !reloadOnce && canReload)
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

    IEnumerator Shoot()
    {
        isShooting = true;

        RaycastHit hit;
        canOpenScope = false;
        canReload = false;

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

        CloseScope();

        FindObjectOfType<AudioManager>().Play("AWP Shoot");
        muzzleFlash.Play();

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            if (hit.transform.gameObject.CompareTag("Shootable"))
            {
                Instantiate(shootableEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Rigidbody rg = hit.transform.gameObject.GetComponent<Rigidbody>();
                rg.AddForce(transform.forward * 300f);
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
            fireRate = Time.time + fireRateCustomize;
            isShooting = false;
        }

        Recoil();

        yield return new WaitForSeconds(2f);
        canOpenScope = true;
        canReload = true;
        isShooting = false;
    }

    IEnumerator Reload(string kind)
    {
        canFire = false;
        canOpenScope = false;

        animator.Play("Reload");

        yield return new WaitForSeconds(2f);

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

        yield return new WaitForSeconds(2.3f);

        canFire = true;
        canOpenScope = true;
        reloadOnce = false;
    }

    private void Recoil()
    {
        float recX = Random.Range(minX, maxX);
        

        fpsCam.transform.localRotation = Quaternion.Euler(rot.x - maxY, rot.y + recX, rot.z);
    }

    public void ReloadSound1()
    {
        audioManager.Play("AWP Reload1");
    }

    public void ReloadSound2()
    {
        audioManager.Play("AWP Reload2");
    }

    public void ReloadSound3()
    {
        audioManager.Play("AWP Reload3");
    }

    public void SwitchSound()
    {
        audioManager.Play("AWP Bolt");
    }

    //for open the scope
    public void OpenScope()
    {
        isScopeOpen = true;
        fpsCam.cullingMask = ~(1 << 11);
        scope1.SetActive(true);
        scope2.SetActive(true);
       
        movementScript.speed = 2f;
        fpsCam.fieldOfView = zoomPov;
    }

    public void CloseScope()
    {
        isScopeOpen = false;
        fpsCam.cullingMask = -1;
        scope1.SetActive(false);
        scope2.SetActive(false);
      
        movementScript.speed = 4.3f;
        fpsCam.fieldOfView = camFOV;
    }

}
