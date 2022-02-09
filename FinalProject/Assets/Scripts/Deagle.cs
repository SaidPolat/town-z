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
        bulletLast = magazineAmount;    //en ba�ta kalan mermiyi �arj�r kapasitesine ayarl�yor

        totalAmmoText.text = ammoAmount.ToString();
        magazineAmmoText.text = bulletLast.ToString();

        sources = GetComponents<AudioSource>();     //sesler i�in birden fazla ayn� audio source kulland�k ��nk� ard arda
                                                    //ate� etme durumlar�nda sesler tamamlanmadan ba�tan �al�nca olu�an
        if (PlayerMovement.isMoving)                //�irkin sesleri engellemek i�in
        {   //oyuncu silah aras� ge�i� yaparsa olu�an bug� engellemek i�in
            animator.SetTrigger("StopRun");
        }
    }

    void Update()
    {
        if (PlayerMovement.isMoving && isMovingController)  //e�er hareket etmeye ba�larsa animatorden ko�ma animasyonunu 
        {                                                   //ba�lat�yor, ve buglar� �nlemek i�in birka� bool kullan�uyoruz
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

        //sol t�ka bas�nca baz� durumlar� kontrol edip ate� etme fonksiyonunu �al��t�r�yor
        if (Input.GetButton("Fire1") && canFire && !PauseMenu.GameIsPaused && Time.time > fireRate && bulletLast != 0 && !GameControl.isMarketOpen)
        {
            Shoot();
            fireRate = Time.time + fireRateCustomize;   //ate� etmeler aras�nda zaman olmas� i�in olan kod
        }

        //mermi kalmay�nca otomatik kendini reloadlamas� i�in olan kod
        if (bulletLast == 0 && bulletLast < magazineAmount && ammoAmount != 0 && !reloadOnce)
        {
            reloadOnce = true;
            StartCoroutine(Reload("NoAmmo"));
        }

        //r ye bas�nca reload yapmas� i�in olan kod
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

        rot = fpsCam.transform.localRotation.eulerAngles;  //kameran�n a��sal de�erlerini vector 3 e kaydettik

        //kamera recoilden sonra yukar� ��kt�ysa kendi haline getirmek i�in olan kod, slerp ile kullan�p smooth bir ge�i� sa�lad�k
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

        RaycastHit hit;     //raycast olu�turuyoruz
        muzzleFlash.Play();
        
        if (PlayerMovement.isMoving)    //ko�ma durumuna g�re ate� etme animasyonunu �al��t�r�yor
        {
            animator.Play("RunningShoot");
        }
        else
        {
            animator.Play("Shoot");
        }

        bulletLast--;       //mermi azal�yor
        magazineAmmoText.text = bulletLast.ToString();

        sources[counter].Play();    //birden fazla olan ate� etme seslerini s�rayla oynat�yor
        
        //raycastin nereye �arpt���na g�re de�i�en, taglar�na bakarak ne yap�ca��na karar veren kod
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            if (hit.transform.gameObject.CompareTag("Shootable"))   //kutular variller ��mlekleri falan hareket ettiren kod
            {
                Instantiate(shootableEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Rigidbody rg = hit.transform.gameObject.GetComponent<Rigidbody>();
                rg.AddForce(transform.forward * 200f);
            }
            else if (hit.transform.gameObject.CompareTag("Enemy"))  //d��mansa onun efektini olu�turup d��mana hasar veriyor
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
                hit.transform.gameObject.GetComponent<ExplosiveBarrel>().GetDamage(damage); //varile vurduysa varile hasar veren kodu �a��r�yor
            }
            else    //bu taglardan herhangi biri de�ilse metal olma olas��� y�ksek oldu�u i�in metal efekti �a��r�yoruz
            {
                Instantiate(bulletHitOnMetal, hit.point, Quaternion.LookRotation(hit.normal));
            }
        }
        if (counter == 4)   //seslerde sona ula�t�ysa ba�a �a��r�yoruz
        {
            counter = -1;
        }

        Recoil();   //silah sekmesini �a��r�yoruz

        counter++;      //sonraki ses sourceuna ge�mesi i�in counter� artt�r�yoruz
        isShooting = false;
    }

    private void Recoil()
    {
        float recX = Random.Range(minX, maxX);  //verdi�imiz aral�klardan random de�er belirliyor
        float recY = Random.Range(minY, maxY);

        //kameran�n a��s�na bu ald���m�z de�erleri ekliyoruz ve kameran�n tepmesini sa�l�yoruz
        fpsCam.transform.localRotation = Quaternion.Euler(rot.x - recY, rot.y + recX, rot.z);
    }


    IEnumerator Reload(string kind)
    {
        canFire = false;    //relaod yaparken ate� etmesini engelliyoruz

        yield return new WaitForSeconds(0.4f);

        animator.Play("Reload");    //reload animasyonunu ba�lat�yor

        yield return new WaitForSeconds(1f);    //animasyondan animasyona de�i�iyor ama yeni �arj�r� takt��� anda 
                                                //mermi yenilemeyi yap�yor, b�ylece reload ortas�nda silah d�ei�tirince 
        switch (kind)                           //e�er �arj�r tak�ld�ysa mermi yenileme yap�lm�� oluyor
        {
            case "StillHasAmmo":    //e�er �arj�rde hala mermi varsa
                if (ammoAmount <= magazineAmount)   //e�er toplam mermi 1 �arj�r� doldurucak kadar mermisi yoksa
                {
                    int temp = bulletLast + ammoAmount; //toplam elindeki mermiyi al�yoruz

                    if (temp > magazineAmount)      //toplam mermi �arj�rden fazlaysa �arj�r� tamamlay�p
                    {                               //kalan mermiyi ayr� yaz�yoruz
                        bulletLast = magazineAmount;
                        ammoAmount = temp - magazineAmount;
                    }
                    else                                       //de�ilse t�m mermileri �arj�re doldurup kalan mermiyi 0 yap�yoruz
                    {
                        bulletLast += ammoAmount;
                        ammoAmount = 0;
                    }
                }
                else                          //e�er varsa klasik i�lemi yap�yoruz, toplam ammo dan �arj�r kadar�n� ��kart�yoruz, ve �arj�r�nde i�inde ne kadar varsa
                {
                    ammoAmount -= magazineAmount - bulletLast;
                    bulletLast = magazineAmount;    //�arj�rde olan mermi say�s�n� �arj�r kapasitesi oalrak e�itliyoruz
                }

                totalAmmoText.text = ammoAmount.ToString();
                magazineAmmoText.text = bulletLast.ToString();
                break;
            case "NoAmmo":      //e�er �arj�rde hi� mermi kalmad�ysa
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

        yield return new WaitForSeconds(1.45f);     //animasyonun kalan k�sm� s�resi

        canFire = true;     //buglar� engellemek i�in olan boollar
        reloadOnce = false;
    }

    public void ReloadSound1()          //animasyonlarda event �eklinde kullanabilmek i�in olan fonksyonlar, bunlar� animasyonlarda �a��r�yoruz
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
