using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Deagle : MonoBehaviour
{
    public Camera fpsCam;

    public AudioManager audioManager;
    public AudioSource[] sources;

    public static bool isShooting = false;
    bool isMovingController = true;
    bool stopController = false;
    bool reloadOnce = false;
    public static bool canReload = true;
    int counter = 0;
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

    void Start()
    {
        bulletLast = magazineAmount;    //en baþta kalan mermiyi þarjör kapasitesine ayarlýyor

        totalAmmoText.text = ammoAmount.ToString();
        magazineAmmoText.text = bulletLast.ToString();

        sources = GetComponents<AudioSource>();     //sesler için birden fazla ayný audio source kullandýk çünkü ard arda
                                                    //ateþ etme durumlarýnda sesler tamamlanmadan baþtan çalýnca oluþan
        if (PlayerMovement.isMoving)                //çirkin sesleri engellemek için
        {   //oyuncu silah arasý geçiþ yaparsa oluþan bugý engellemek için
            animator.SetTrigger("StopRun");
        }
    }

    void Update()
    {
        if (PlayerMovement.isMoving && isMovingController)  //eðer hareket etmeye baþlarsa animatorden koþma animasyonunu 
        {                                                   //baþlatýyor, ve buglarý önlemek için birkaç bool kullanýuyoruz
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

        //sol týka basýnca bazý durumlarý kontrol edip ateþ etme fonksiyonunu çalýþtýrýyor
        if (Input.GetButton("Fire1") && canFire && !PauseMenu.GameIsPaused && Time.time > fireRate && bulletLast != 0 && !GameControl.isMarketOpen)
        {
            Shoot();
            fireRate = Time.time + fireRateCustomize;   //ateþ etmeler arasýnda zaman olmasý için olan kod
        }

        //mermi kalmayýnca otomatik kendini reloadlamasý için olan kod
        if (bulletLast == 0 && bulletLast < magazineAmount && ammoAmount != 0 && !reloadOnce)
        {
            reloadOnce = true;
            StartCoroutine(Reload("NoAmmo"));
        }

        //r ye basýnca reload yapmasý için olan kod
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

        //kamera recoilden sonra yukarý çýktýysa kendi haline getirmek için olan kod, slerp ile kullanýp smooth bir geçiþ saðladýk
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

        RaycastHit hit;     //raycast oluþturuyoruz
        muzzleFlash.Play();
        
        if (PlayerMovement.isMoving)    //koþma durumuna göre ateþ etme animasyonunu çalýþtýrýyor
        {
            animator.Play("RunningShoot");
        }
        else
        {
            animator.Play("Shoot");
        }

        bulletLast--;       //mermi azalýyor
        magazineAmmoText.text = bulletLast.ToString();

        sources[counter].Play();    //birden fazla olan ateþ etme seslerini sýrayla oynatýyor
        
        //raycastin nereye çarptýðýna göre deðiþen, taglarýna bakarak ne yapýcaðýna karar veren kod
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            if (hit.transform.gameObject.CompareTag("Shootable"))   //kutular variller çömlekleri falan hareket ettiren kod
            {
                Instantiate(shootableEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Rigidbody rg = hit.transform.gameObject.GetComponent<Rigidbody>();
                rg.AddForce(transform.forward * 200f);
            }
            else if (hit.transform.gameObject.CompareTag("Enemy"))  //düþmansa onun efektini oluþturup düþmana hasar veriyor
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
                hit.transform.gameObject.GetComponent<ExplosiveBarrel>().GetDamage(damage); //varile vurduysa varile hasar veren kodu çaðýrýyor
            }
            else    //bu taglardan herhangi biri deðilse metal olma olasýðý yüksek olduðu için metal efekti çaðýrýyoruz
            {
                Instantiate(bulletHitOnMetal, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
        if (counter == 4)   //seslerde sona ulaþtýysa baþa çaðýrýyoruz
        {
            counter = -1;
        }

        Recoil();   //silah sekmesini çaðýrýyoruz

        counter++;      //sonraki ses sourceuna geçmesi için counterý arttýrýyoruz
        isShooting = false;
    }

    private void Recoil()
    {
        float recX = Random.Range(minX, maxX);  //verdiðimiz aralýklardan random deðer belirliyor
        float recY = Random.Range(minY, maxY);

        //kameranýn açýsýna bu aldýðýmýz deðerleri ekliyoruz ve kameranýn tepmesini saðlýyoruz
        fpsCam.transform.localRotation = Quaternion.Euler(rot.x - recY, rot.y + recX, rot.z);
    }


    IEnumerator Reload(string kind)
    {
        canFire = false;    //relaod yaparken ateþ etmesini engelliyoruz

        yield return new WaitForSeconds(0.4f);

        animator.Play("Reload");    //reload animasyonunu baþlatýyor

        yield return new WaitForSeconds(1f);    //animasyondan animasyona deðiþiyor ama yeni þarjörü taktýðý anda 
                                                //mermi yenilemeyi yapýyor, böylece reload ortasýnda silah dðeiþtirince 
        switch (kind)                           //eðer þarjör takýldýysa mermi yenileme yapýlmýþ oluyor
        {
            case "StillHasAmmo":    //eðer þarjörde hala mermi varsa
                if (ammoAmount <= magazineAmount)   //eðer toplam mermi 1 þarjörü doldurucak kadar mermisi yoksa
                {
                    int temp = bulletLast + ammoAmount; //toplam elindeki mermiyi alýyoruz

                    if (temp > magazineAmount)      //toplam mermi þarjörden fazlaysa þarjörü tamamlayýp
                    {                               //kalan mermiyi ayrý yazýyoruz
                        bulletLast = magazineAmount;
                        ammoAmount = temp - magazineAmount;
                    }
                    else                                       //deðilse tüm mermileri þarjöre doldurup kalan mermiyi 0 yapýyoruz
                    {
                        bulletLast += ammoAmount;
                        ammoAmount = 0;
                    }
                }
                else                          //eðer varsa klasik iþlemi yapýyoruz, toplam ammo dan þarjör kadarýný çýkartýyoruz, ve þarjöründe içinde ne kadar varsa
                {
                    ammoAmount -= magazineAmount - bulletLast;
                    bulletLast = magazineAmount;    //þarjörde olan mermi sayýsýný þarjör kapasitesi oalrak eþitliyoruz
                }

                totalAmmoText.text = ammoAmount.ToString();
                magazineAmmoText.text = bulletLast.ToString();
                break;
            case "NoAmmo":      //eðer þarjörde hiç mermi kalmadýysa
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

        yield return new WaitForSeconds(1.45f);     //animasyonun kalan kýsmý süresi

        canFire = true;     //buglarý engellemek için olan boollar
        reloadOnce = false;
    }

    public void ReloadSound1()          //animasyonlarda event þeklinde kullanabilmek için olan fonksyonlar, bunlarý animasyonlarda çaðýrýyoruz
    {
        audioManager.Play("Deagle Reload1");
    }

    public void ReloadSound2()
    {
        audioManager.Play("Deagle Reload2");
    }

    public void ReloadSound3()
    {
        audioManager.Play("Deagle Reload3");
    }
}
