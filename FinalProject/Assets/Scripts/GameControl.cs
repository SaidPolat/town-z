using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameControl : MonoBehaviour
{
    public GameObject[] weapons;

    Generator g1Script;
    Generator g2Script;
    Generator g3Script;
    Generator g4Script;
    Animator canvasAnimator;
    Animator cameraGameOverAnimator;

    //bool weaponDoesNotExist = true;
    bool throwingGrenade = false;
    private bool grenadeInProggress = false;
    bool animationFinished = false;
    private float timeTillStart = 0;
    private bool startTime = false;
    private bool stopTimer;
    private bool restartTime = false;
    private bool doOnce = true;
    public static bool isMarketOpen = false;

    public AudioManager audioManager;
    public TextMeshProUGUI slash;
    public GameObject crosshair;
    public Camera fpsCam;

    //int currentWeaponForScroll = 3;
    int currentRoundNumber;
    int currentWeapon = 3;
    int lastWeapon = 3;
    bool isRoundOver = false;
    int zombieLeftInFunction;
    PlayerMovement movement;
    public static List<int> scoreList = new List<int>();

    [HideInInspector] public int zombieLeft;
    [HideInInspector] public bool doesHaveAk = false;
    [HideInInspector] public bool doesHaveM4 = false;
    [HideInInspector] public bool doesHaveDeagle = false;
    [HideInInspector] public bool doesHaveKnife = true;
    [HideInInspector] public bool doesHaveAwp = false;
    [HideInInspector] public bool doesHaveMp7 = false;
    [HideInInspector] public bool doesHaveFiveSeven = true;
    [HideInInspector] public bool doesHaveShotgun = false;
    [HideInInspector] public bool doesHaveKarambitFade = false;
    [HideInInspector] public int killCount = 0;

    [Header("ForGameOver")]
    public GameObject cameraForGameOver;
    public GameObject actualCamera;
    public GameObject canvas;
    public GameObject canvasGameOver;
    public GameObject canvasGameplay;
    public TextMeshProUGUI gameOverScoreText;
    public TextMeshProUGUI winText;

    [Header("GunScripts")]
    public Ak akScript;
    public M4A1 m4Script;
    public AWP awpScript;
    public Deagle deagleScript;
    public Knife knifeScript;
    public MP7 mp7Script;
    public FiveSeven fiveSevenScript;
    public Shotgun shotgunScript;

    [Header("RoundSettings")]
    public TextMeshProUGUI zombieCountText;
    public TextMeshProUGUI roundTimerText;
    public GameObject marketAvailableBorder;
    public GameObject timeCountBorder;
    public int roundCount;
    public float spawnTimer;
    public float timeBetweenRounds;
    public int firstRoundZombieCount;
    public bool tutorialOn;
    public TextMeshProUGUI tutorialText;
    public TextMeshProUGUI roundCountText;

    [Header("MarketSettings")]
    public GameObject marketPanel;
    public GameObject menu1;
    public GameObject weaponsMenu;
    public GameObject utilitiesMenu;
    public int deagleMoney;
    public int mp7Money;
    public int shotgunMoney;
    public int akMoney;
    public int m4Money;
    public int awpMoney;
    public int knifeMoney;
    public int money762;
    public int money556;
    public int money9mm;
    public int money40m;
    public int moneyShell;
    public int moneyGrenade;
    public int moneyRepairGenorator;
    public int moneyHealth;

    [Header("Grenade")]
    public GameObject grenadeCreatePoint;
    public GameObject Grenade;
    public float grenadeExpDelay;
    public float grenadeThrowForceLong;
    public float grenadeThrowForceShort;
    public int grenadeNumber;
    public TextMeshProUGUI grenadeNumberText;

    [Header("Player")]
    public GameObject player;
    public Slider playerHealthSlider;
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI playerMoneyText;
    public TextMeshProUGUI playerKillText;
    public TextMeshProUGUI playerNameText;
    public int playerHealth;
    public int playerMoney;

    [Header("EnemySystem")]
    public GameObject[] enemies;
    public GameObject[] spawnPoints;
    public GameObject[] targetPoints;

    [Header("IndicatorUI")]
    public GameObject marketIndicator;
    public GameObject ammo762Indicator;
    public GameObject ammo556Indicator;
    public GameObject ammo9mmIndicator;
    public GameObject ammo40mIndicator;
    public GameObject ammoSlugIndicator;
    public GameObject medKitIndicator;

    [Header("IconsForBullets")]
    public GameObject icon762;
    public GameObject icon556;
    public GameObject icon9mm;
    public GameObject icon40m;
    public GameObject iconShell;
    public GameObject iconAmmoKnife;

    [Header("Animators")]
    public Animator playerHealthUI;
    public Animator akAnimator;
    public Animator akAnimator2;
    public Animator m4Animator;
    public Animator m4Animator2;
    public Animator deagleAnimator;
    public Animator fiveSevenAnimator;
    public Animator shotgunAnimator;
    public Animator awpAnimator;
    public Animator knifeAnimator;
    public Animator mp7Animator;
    public Animator grenadeAnimator;

    void Start()
    {
        movement = player.GetComponent<PlayerMovement>();
        canvasAnimator = canvas.GetComponent<Animator>();
        cameraGameOverAnimator = cameraForGameOver.GetComponent<Animator>();
        g1Script = targetPoints[0].GetComponent<Generator>();
        g2Script = targetPoints[1].GetComponent<Generator>();
        g3Script = targetPoints[2].GetComponent<Generator>();
        g4Script = targetPoints[3].GetComponent<Generator>();

        zombieLeft = firstRoundZombieCount;                                 //ui elementlerini yazdýrýyoruz
        zombieCountText.text = firstRoundZombieCount.ToString();
        grenadeNumberText.text = grenadeNumber.ToString();
        playerKillText.text = "0 Kills";
        playerNameText.text = MainMenuScript.myName + " :";
        roundCountText.text = roundCount.ToString();

        StartCoroutine(StartRound());       //roundun baþlamasýný saðlayan coroutine
        StartCoroutine(GrenadeLoop());          //grenade atarken oluþan bir bugý engellemek için yapýlmýþ coroutine
    }
    
    
    void Update()
    {
        if(zombieLeft == 0)         
        {
            isRoundOver = true;
        }

        if(currentRoundNumber == roundCount)        //son roundu bitirdiðinde win ekraný gelmesi için
        {
            StartCoroutine(PlayerDead("win"));
        }

        if(playerHealth <= 0 && doOnce)
        {
            StartCoroutine(PlayerDead("lose"));
            doOnce = false;   
        }

        if(!g1Script.genoratorAlive && !g2Script.genoratorAlive && !g3Script.genoratorAlive && !g4Script.genoratorAlive)
        {
            StartCoroutine(PlayerDead("lose"));         //generatorlerin hepsi patlarsa oyunun bitmesi
        }

        //ammo boxlarýn veya markete baktýðýnýzda çýkan indicatorlarýn kodu, ifin böyle olmasýnýn sebebi, sürekli bir raycast göndermemizden dolayý
        if (!Ak.isShooting && !Deagle.isShooting && !Knife.isShooting && !M4A1.isShooting && !AWP.isShooting && !MP7.isShooting && !FiveSeven.isShooting && !Shotgun.isShooting)
        {
            RaycastHit hit;             //silahlar için bunu kontrol etmezsek daha sonra ateþ eterken bug ile karþýlaþýyoruz

            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, 3))
            {
                if (hit.transform.gameObject.CompareTag("MarketTrigger"))   //çarptýðý þey neyse tagýna göre onun indicatorýný çýkartýyor
                {
                    if(!isMarketOpen)
                        marketIndicator.SetActive(true);
                }
                else
                {
                    if (marketIndicator.activeSelf)
                        marketIndicator.SetActive(false);
                }

                if (hit.transform.gameObject.CompareTag("AmmoBox9mm"))
                {
                    ammo9mmIndicator.SetActive(true);
                }
                else
                {
                    if (ammo9mmIndicator.activeSelf)
                        ammo9mmIndicator.SetActive(false);
                }

                if (hit.transform.gameObject.CompareTag("AmmoBox7.62"))
                {
                    ammo762Indicator.SetActive(true);
                }
                else
                {
                    if (ammo762Indicator.activeSelf)
                        ammo762Indicator.SetActive(false);
                }

                if (hit.transform.gameObject.CompareTag("AmmoBox5.56"))
                {
                    ammo556Indicator.SetActive(true);
                }
                else
                {
                    if (ammo556Indicator.activeSelf)
                        ammo556Indicator.SetActive(false);
                }

                if (hit.transform.gameObject.CompareTag("AmmoBox40"))
                {
                    ammo40mIndicator.SetActive(true);
                }
                else
                {
                    if (ammo40mIndicator.activeSelf)
                        ammo40mIndicator.SetActive(false);
                }

                if (hit.transform.gameObject.CompareTag("AmmoBoxShotgun"))
                {
                    ammoSlugIndicator.SetActive(true);
                }
                else
                {
                    if (ammoSlugIndicator.activeSelf)
                        ammoSlugIndicator.SetActive(false);
                }

                if (hit.transform.gameObject.CompareTag("MedKit"))
                {
                    medKitIndicator.SetActive(true);
                }
                else
                {
                    if (medKitIndicator.activeSelf)
                        medKitIndicator.SetActive(false);
                }
            }
        }

        //tagýna bakarak ona göre ammoboxlar veya diðer þeylerle etkileþime girmesini saðlýyor
        if (Input.GetKeyDown(KeyCode.E)) 
        {
            RaycastHit hit;

            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, 3))
            {
                if (hit.transform.gameObject.CompareTag("MarketTrigger"))
                {
                    if (isRoundOver)
                    {
                        marketIndicator.SetActive(false);

                        OpenMarket();
                    }
                }
                if (hit.transform.gameObject.CompareTag("MedKit"))
                {
                    if(playerHealth < 100)
                    {
                        playerHealth += 10;
                        if (playerHealth > 100)
                            playerHealth = 100;
                        playerHealthText.text = playerHealth.ToString();
                        playerHealthSlider.value = playerHealth;

                        Destroy(hit.transform.gameObject);
                    }                   
                }
                if (hit.transform.gameObject.CompareTag("AmmoBox5.56"))
                {
                    m4Script.ammoAmount += 40;
                    Destroy(hit.transform.gameObject);
                }
                if (hit.transform.gameObject.CompareTag("AmmoBox7.62"))
                {
                    akScript.ammoAmount += 30;
                    awpScript.ammoAmount += 10;
                    Destroy(hit.transform.gameObject);
                }
                if (hit.transform.gameObject.CompareTag("AmmoBox40"))
                {
                    deagleScript.ammoAmount += 14;
                    Destroy(hit.transform.gameObject);
                }
                if (hit.transform.gameObject.CompareTag("AmmoBox9mm"))
                {
                    fiveSevenScript.ammoAmount += 40;
                    mp7Script.ammoAmount += 120;
                    Destroy(hit.transform.gameObject);
                }
                if (hit.transform.gameObject.CompareTag("AmmoBoxShotgun"))
                {
                    shotgunScript.ammoAmount += 16;
                    Destroy(hit.transform.gameObject);
                }
            }
        }

        //pause menüsü kapatma
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isMarketOpen)
            {
                CloseMarket();
            }
        }

        //silahlar arasý geçiþ yapmak için klavyedeki sayý olan tuþlar. Açýklama sadece ilk ifte var(Alpha1 de) diðerleri ayný zaten
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (currentWeapon != 1 && !isMarketOpen && !throwingGrenade && doesHaveAk) //buglarý önlemek için bazý þeyleri kontrol ediyoruz
            {
                ResetAnimationAndDeactivateWeapon(currentWeapon); //deðiþtirmeden önceki silahý güvenli þekilde kapatan fonksiyon
                lastWeapon = currentWeapon;  //son silahý þu an elimizdeki silah yapýyor
                currentWeapon = 1;  //current weaponý yeni alýcaðýmýz silah yapýyor
                StartCoroutine(ChangeWeapon(0));   //alýcaðýmýz silahý çýkartma animasyonunu ve aktifleþtirmeleri yapýyor
            }     
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {     
            if (currentWeapon != 2 && !isMarketOpen && !throwingGrenade && doesHaveDeagle)
            {
                ResetAnimationAndDeactivateWeapon(currentWeapon);
                lastWeapon = currentWeapon;
                currentWeapon = 2;
                StartCoroutine(ChangeWeapon(1));
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if(currentWeapon != 3 && !isMarketOpen && !throwingGrenade)
            {
                ResetAnimationAndDeactivateWeapon(currentWeapon);
                lastWeapon = currentWeapon;
                currentWeapon = 3;
                StartCoroutine(ChangeWeapon(2));
            }
            
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            if (currentWeapon != 4 && !isMarketOpen && !throwingGrenade && doesHaveM4)
            {
                ResetAnimationAndDeactivateWeapon(currentWeapon);
                lastWeapon = currentWeapon;
                currentWeapon = 4;
                StartCoroutine(ChangeWeapon(3));
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if(currentWeapon != 5 && !isMarketOpen && !throwingGrenade && doesHaveAwp)
            {
                ResetAnimationAndDeactivateWeapon(currentWeapon);
                lastWeapon = currentWeapon;
                currentWeapon = 5;           
                StartCoroutine(ChangeWeapon(4));
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            if (currentWeapon != 6 && !isMarketOpen && !throwingGrenade && doesHaveMp7)
            {
                ResetAnimationAndDeactivateWeapon(currentWeapon);
                lastWeapon = currentWeapon;
                currentWeapon = 6;
                StartCoroutine(ChangeWeapon(5));
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            if (currentWeapon != 7 && !isMarketOpen && !throwingGrenade)
            {
                ResetAnimationAndDeactivateWeapon(currentWeapon);
                lastWeapon = currentWeapon;
                currentWeapon = 7;
                StartCoroutine(ChangeWeapon(6));
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            if (currentWeapon != 8 && !isMarketOpen && !throwingGrenade && doesHaveShotgun)
            {
                ResetAnimationAndDeactivateWeapon(currentWeapon);
                lastWeapon = currentWeapon;
                currentWeapon = 8;
                StartCoroutine(ChangeWeapon(7));
            }
        }

        //elimizdekinden önceki silahý almak için yapýlan iþlemler
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!isMarketOpen && !throwingGrenade)
            {
                ResetAnimationAndDeactivateWeapon(currentWeapon);   //elimizde olan silahý kapatýyor
                int temp = lastWeapon;
                lastWeapon = currentWeapon;         //elimizdeki silahý önceki silaha çeviriyoruz
                currentWeapon = temp;
                StartCoroutine(ChangeWeapon(currentWeapon - 1));    //önceki silah neyse onu açýyoruz
            }     
        }

        //grenade atma kodu ama sadece ilk basma için, asýl atma olayý parmaðýmýzý tuþtan kaldýrdýðýmýzda
        if (Input.GetKeyDown(KeyCode.G) || Input.GetMouseButtonDown(2))
        {
            if (!isMarketOpen && !grenadeInProggress && grenadeNumber > 0)
            {
                throwingGrenade = true;
                ResetAnimationAndDeactivateWeapon(currentWeapon);   //elimizdeki silahý kapatýyoruz ekranda gözükmesin diye
                StartCoroutine(ChangeWeapon(8));    //grenade i açýyoruz ekranda gözükmesi için
            }
        }

        //bu ikisi bombayý fýrlatmaya yarayan kod
        if (Input.GetKeyUp(KeyCode.G))
        {
            if (!isMarketOpen && !grenadeInProggress)
            {
                grenadeInProggress = true;
                StartCoroutine(CreateGrenade(0));     //bu coroutine ile bomba oluþturup fýrlatýyoruz,           
            }                                           // indexi uzaða veya yakýna atmayý deðiþtiriyor
        }
        if (Input.GetMouseButtonUp(2))
        {
            if (!isMarketOpen && !grenadeInProggress)
            {
                grenadeInProggress = true;
                StartCoroutine(CreateGrenade(1));
            }
        }

        //silahlarý ayný zamanda mouse tekerliði ile de deðiþtirebilicektik ve bunu sistem otomatik olarak açýk olanlarý bularak yapýcaktý
        //fakat silah satýn alma sistemini tamamladýktan sonra
        //bu sistemde baya deðiþiklik yapmak zorunda kaldýk, tam hallettik dediðimizde Q tuþu sisteminde hatalarla da karþýlaþtýk
        //bir de hala 8. silahtan 1. silaha giderken sýkýntý çýkartýyordu, o yüzden tüm silahlarý tamamladýðýnda açýlan sisteme çevirdik
        //silahlar tam olmadan çalýþan kodu yorum içinde býraktýk.
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if (!isMarketOpen && !throwingGrenade)
            {
                if(doesHaveAk && doesHaveAwp && doesHaveDeagle && doesHaveFiveSeven && doesHaveKnife && doesHaveM4 && doesHaveMp7 && doesHaveShotgun)
                {
                    ResetAnimationAndDeactivateWeapon(currentWeapon);
                    lastWeapon = currentWeapon;
                    if (currentWeapon == 8)
                    {
                        currentWeapon = 0;      //þu an ki silah 8. silah deðilse 1 arttýrýyor, 8 ise 0 a getiriyor
                                                //daha sonra sonraki silahý açýyoruz
                    }
                    currentWeapon++;
                    StartCoroutine(ChangeWeapon(currentWeapon - 1));

                    //Normalde her silah yokken de bu fonksiyon çalýþýcaktý, otomatik açýk olan silahý bulucaktý 
                    //ama maalesef çok fazla bugla karþýlaþtýðýmýz için erteledik

                    /*while (weaponDoesNotExist)
                    {
                        if (currentWeapon == 0 && !doesHaveAk)
                        {
                            currentWeapon++;
                        }
                        else
                            weaponDoesNotExist = false;

                        if (currentWeapon == 1 && !doesHaveDeagle)
                        {
                            currentWeapon++;
                        }
                        else
                            weaponDoesNotExist = false;

                        if (currentWeapon == 2 && !doesHaveKnife)
                        {
                            currentWeapon++;
                        }
                        else
                            weaponDoesNotExist = false;

                        if (currentWeapon == 3 && !doesHaveM4)
                        {
                            currentWeapon++;
                        }
                        else
                            weaponDoesNotExist = false;

                        if (currentWeapon == 4 && !doesHaveAwp)
                        {
                            currentWeapon++;
                        }
                        else
                            weaponDoesNotExist = false;

                        if (currentWeapon == 5 && !doesHaveMp7)
                        {
                            currentWeapon++;
                        }
                        else
                            weaponDoesNotExist = false;

                        if (currentWeapon == 6 && !doesHaveFiveSeven)
                        {
                            currentWeapon++;
                        }
                        else
                            weaponDoesNotExist = false;

                        if (currentWeapon == 7 && !doesHaveShotgun)
                        {
                            currentWeapon++;
                        }
                        else
                            weaponDoesNotExist = false;

                        if (currentWeapon == 8)
                            currentWeapon = 0;
                    }*/
                }



            }          
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            if (!isMarketOpen && !throwingGrenade)
            {
                if (doesHaveAk && doesHaveAwp && doesHaveDeagle && doesHaveFiveSeven && doesHaveKnife && doesHaveM4 && doesHaveMp7 && doesHaveShotgun)
                {
                    ResetAnimationAndDeactivateWeapon(currentWeapon);
                    lastWeapon = currentWeapon;
                    if (currentWeapon == 1)
                    {
                        currentWeapon = 9;
                    }
                    currentWeapon--;
                    StartCoroutine(ChangeWeapon(currentWeapon - 1));
              
                }

            }
        }

        //round arasýndaki saniyeyi gösteren kod
        if (startTime == true)
        {
            timeTillStart += Time.deltaTime;
            double time = timeBetweenRounds - timeTillStart;   //süreyi geriye doðru saydýrýyor
            if (time <= 0)
            {
                stopTimer = true;
                CloseMarket();
            }
            if (!stopTimer)
            {
                roundTimerText.text = "Time left for the next round: " + time.ToString("F2");
            }
        }
        if (restartTime)
        {
            timeTillStart = 0f;
        }

        //býçak elimizdeyse mermi yerindeki slashý kaldýrýyor
        if (currentWeapon != 3)
            slash.text = "/";
        else
            slash.text = "";

        //ui da belli baþlý þeyleri yazdýrýyoruz
        zombieCountText.text = zombieLeft.ToString();
        playerMoneyText.text = "$" + playerMoney.ToString();

    }
    
    //marketi kapatmak için gereken kod, market açýkken kýsýtladýðýmýz þeyleri açýyoruz
    public void CloseMarket()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        PlayerMovement.canMove = true;
        MouseLook.canLook = true;
        MP7.canFire = true;
        MP7.canReload = true;
        AWP.canFire = true;
        AWP.canOpenScope = true;
        AWP.canReload = true;
        M4A1.canFire = true;
        M4A1.canReload = true;
        Deagle.canFire = true;
        Deagle.canReload = true;
        Ak.canReload = true;
        Ak.canFire = true;
        Knife.canFire = true;
        FiveSeven.canFire = true;
        FiveSeven.canReload = true;
        Shotgun.canReload = true;
        Shotgun.canFire = true;

        FindObjectOfType<LevelCanvasScript>().ResetBuyButtons();  //buy buttonlarý resetliyoruz
        isMarketOpen = false;
        audioManager.Play("Menu5");
        marketPanel.SetActive(false);
        menu1.SetActive(true);
        weaponsMenu.SetActive(false);
        utilitiesMenu.SetActive(false);
    }

    //marketi açmak için kod, bazý þeyleri kýsýtlýyoruz ki market açýkken oyuncu her þeyi yapamasýn
    public void OpenMarket()
    {
        isMarketOpen = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        PlayerMovement.canMove = false;
        MouseLook.canLook = false;
        MP7.canFire = false;
        MP7.canReload = false;
        AWP.canFire = false;
        AWP.canOpenScope = false;
        AWP.canReload = false;
        M4A1.canFire = false;
        M4A1.canReload = false;
        Deagle.canFire = false;
        Deagle.canReload = false;
        Ak.canReload = false;
        Ak.canFire = false;
        Knife.canFire = false;
        FiveSeven.canFire = false;
        FiveSeven.canReload = false;
        Shotgun.canReload = false;
        Shotgun.canFire = false;

        audioManager.Play("Menu2");
        FindObjectOfType<LevelCanvasScript>().CheckBuyAvailable();  //buy tuþlarýný kontrol ettiriyoruz
        marketPanel.SetActive(true);
    }

    //yeni silahý açma ve switch animasyonu ve sesini çaldýrmak için olan kod, indexe göre çalýþýyor
    IEnumerator ChangeWeapon(int index)
    {
        weapons[index].SetActive(true); //arrayden indexi alýp en baþta aktif ediyor

        switch (index)
        {
            case 0:
                movement.speed = 4f;    //silaha özel hareket hýzý veriyor
                icon762.SetActive(true);    //silahýn mermi ikonunu gösteriyor
                Ak.canFire = false;     //switch animasyonu oynarken ateþ edip reload yapmasýn diye
                Ak.canReload = false;
                akAnimator2.Play("SwitchAk");
                akAnimator.Play("Switch");
                yield return new WaitForSeconds(1.3f);
                Ak.canFire = true;      //switch animasyonu bittikten sonra özellikleri tekrar devrede
                Ak.canReload = true;
                if (PlayerMovement.isMoving)   //oyuncu silah deðiþtirirken hareket ediyorsa ona göre trigger uyguluyoruz
                {
                    akAnimator.SetTrigger("StartRun");
                }
                break;
            case 1:
                movement.speed = 4.4f;
                icon40m.SetActive(true);
                Deagle.canFire = false;
                Deagle.canReload = false;
                audioManager.Play("Deagle Switch");
                deagleAnimator.Play("Switch");
                yield return new WaitForSeconds(0.85f);
                Deagle.canFire = true;
                Deagle.canReload = true;
                if (PlayerMovement.isMoving)
                {
                    deagleAnimator.SetTrigger("StartRun");
                }               
                break;
            case 2:
                movement.speed = 5f;
                iconAmmoKnife.SetActive(true);
                Knife.canFire = false;
                audioManager.Play("Knife Switch");
                knifeAnimator.Play("SwitchAlt");
                yield return new WaitForSeconds(0.8f);
                Knife.canFire = true;
                if (PlayerMovement.isMoving)
                {
                    knifeAnimator.SetTrigger("StartRun");
                }
                break;
            case 3:
                movement.speed = 4.1f;
                icon556.SetActive(true);
                M4A1.canFire = false;
                M4A1.canReload = false;
                m4Animator.Play("Switch");
                m4Animator2.Play("SwitchM4");
                yield return new WaitForSeconds(1.4f);
                M4A1.canFire = true;
                M4A1.canReload = true;
                if (PlayerMovement.isMoving)
                {
                    m4Animator.SetTrigger("StartRun");
                }
                break;
            case 4:
                crosshair.SetActive(false);  //awpye geçince crosshairi kapatýyoruz
                movement.speed = 3.5f;
                icon762.SetActive(true);
                AWP.canFire = false;
                AWP.canOpenScope = false;
                AWP.canReload = false;
                awpAnimator.Play("Switch");
                yield return new WaitForSeconds(1.1f);
                AWP.canFire = true;
                AWP.canOpenScope = true;
                AWP.canReload = true;
                if (PlayerMovement.isMoving)
                {
                    awpAnimator.SetTrigger("StartRun");
                }
                break;
            case 5:
                movement.speed = 4.3f;
                icon9mm.SetActive(true);
                MP7.canReload = false;
                MP7.canFire = false;
                audioManager.Play("MP7 Switch");
                mp7Animator.Play("Switch");
                yield return new WaitForSeconds(1.4f);
                MP7.canReload = true;
                MP7.canFire = true;
                if (PlayerMovement.isMoving)
                {
                    mp7Animator.SetTrigger("StartRun");
                }
                break;
            case 6:
                movement.speed = 4.4f;
                icon9mm.SetActive(true);
                FiveSeven.canReload = false;
                FiveSeven.canFire = false;
                audioManager.Play("FiveSeven Switch");
                fiveSevenAnimator.Play("Switch");
                yield return new WaitForSeconds(0.85f);
                FiveSeven.canReload = true;
                FiveSeven.canFire = true;
                if (PlayerMovement.isMoving)
                {
                    fiveSevenAnimator.SetTrigger("StartRun");
                }
                break;
            case 7:
                movement.speed = 3.9f;
                iconShell.SetActive(true);
                Shotgun.canReload = false;
                Shotgun.canFire = false;
                audioManager.Play("Shotgun Switch");
                shotgunAnimator.Play("Switch");
                yield return new WaitForSeconds(0.95f);
                Shotgun.canReload = true;
                Shotgun.canFire = true;
                if (PlayerMovement.isMoving)
                {
                    shotgunAnimator.SetTrigger("StartRun");
                }
                break;
            case 8:    
                grenadeAnimator.SetTrigger("LeftClick");
                yield return new WaitForSeconds(0.25f);
                audioManager.Play("Grenade Ring");
                yield return new WaitForSeconds(0.45f);
                animationFinished = true;
                break;
        }
            

    }

    //silah deðiþtirirken elimizde hazýr bulunan silahý kapatma ve eðer animasyondaysa onu sýfýrlama kodu
    public void ResetAnimationAndDeactivateWeapon(int currentWeapon)
    {
        switch (currentWeapon)
        {
            case 1:
                akAnimator.CrossFade("Idle", 0f, 0);    //animasyonun ortasýndaysa transform deðerleri öyle kalmasýn diye
                akAnimator.CrossFade("Idle", 0f, 1);    //animasyonu normal haline çeviriyoruz, böylece tekrar o silaha
                akAnimator2.CrossFade("Idle", 0f, 0);   //geçilince silah normal þekilde çýkýyor
                akAnimator.Update(0f);
                akAnimator.Update(0f);
                akAnimator2.Update(0f);
                akAnimator2.Update(0f);
                icon762.SetActive(false);
                weapons[0].gameObject.SetActive(false);         //silahýn aktifliðini kapatýyoruz
                break;
            case 2:
                deagleAnimator.CrossFade("Idle", 0f, 0);
                deagleAnimator.CrossFade("Idle", 0f, 1);
                deagleAnimator.Update(0f);
                deagleAnimator.Update(0f);
                icon40m.SetActive(false);
                weapons[1].gameObject.SetActive(false);
                break;
            case 3:
                knifeAnimator.CrossFade("Idle", 0f, 0);
                knifeAnimator.CrossFade("Idle", 0f, 1);
                knifeAnimator.Update(0f);
                knifeAnimator.Update(0f);
                iconAmmoKnife.SetActive(false);
                weapons[2].gameObject.SetActive(false);
                break;
            case 4:
                m4Animator.CrossFade("Idle", 0f, 0);
                m4Animator.CrossFade("Idle", 0f, 1);
                m4Animator2.CrossFade("Idle", 0f, 0);
                m4Animator.Update(0f);
                m4Animator.Update(0f);
                m4Animator2.Update(0f);
                m4Animator2.Update(0f);
                icon556.SetActive(false);
                weapons[3].gameObject.SetActive(false);
                break;
            case 5:
                awpAnimator.CrossFade("Idle", 0f, 0);
                awpAnimator.CrossFade("Idle", 0f, 1);
                awpAnimator.Update(0f);
                awpAnimator.Update(0f);
                icon762.SetActive(false);   
                awpScript.CloseScope();
                crosshair.SetActive(true);  //awp olunca crosshairi geri açýyoruz
                weapons[4].gameObject.SetActive(false);
                break;
            case 6:
                mp7Animator.CrossFade("Idle", 0f, 0);
                mp7Animator.CrossFade("Idle", 0f, 1);
                mp7Animator.Update(0f);
                mp7Animator.Update(0f);
                icon9mm.SetActive(false);
                weapons[5].gameObject.SetActive(false);
                break;
            case 7:
                fiveSevenAnimator.CrossFade("Idle", 0f, 0);
                fiveSevenAnimator.CrossFade("Idle", 0f, 1);
                fiveSevenAnimator.Update(0f);
                fiveSevenAnimator.Update(0f);
                icon9mm.SetActive(false);
                weapons[6].gameObject.SetActive(false);
                break;
            case 8:
                shotgunAnimator.CrossFade("Idle", 0f, 0);
                shotgunAnimator.CrossFade("Idle", 0f, 1);
                shotgunAnimator.Update(0f);
                shotgunAnimator.Update(0f);
                iconShell.SetActive(false);
                weapons[7].gameObject.SetActive(false);
                break;
            case 9:
                grenadeAnimator.CrossFade("Idle", 0f, 0);
                grenadeAnimator.Update(0f);
                grenadeAnimator.Update(0f);
                weapons[8].gameObject.SetActive(false);
                break;
        }
    }

    //düþmanlarý spawnlayan kod, ilk parametre kaç zombi çýkacaðýný alýyor, ikinci parametre spawnlarýn arasýnda kaç saniye bekleyeceðini
    IEnumerator SpawnEnemy(int firstRoundZombieCount, float spawnTimer)
    {
        zombieLeftInFunction = firstRoundZombieCount;    //deðeri deðiþtirebiliceðimiz bir deðiþkene atýyoruz

        Debug.Log("spawn enemy while öncesi");

        while (zombieLeftInFunction > 0)   //düþman sayýsý bitene kadar iþleme devam ediyor
        {
            Debug.Log("spawn while içi");
            yield return new WaitForSeconds(spawnTimer);   //spawnlar arasý beklenen süre

            int enemyType = Random.Range(0, 3);
            int spawnPoint = Random.Range(0, 6);    //burda deðerlerden random alýyoruz, böylece enemy typlarý, nerden doðacaklarý
            int targetPoint = Random.Range(0, 12);  //nereye gidicekleri random olarak belirleniyor, bunlarý inspectordan atýyoruz

            //random olan düþmaný spawnlama kodu
            GameObject obj = Instantiate(enemies[enemyType], spawnPoints[spawnPoint].transform.position, Quaternion.identity);

            //random sonucu düþman, target olarak oyuncuyu mu seçti diye kontrol ediyoruz
            if (targetPoint > 3) //oyuncuya saldýrýcaksa olan kod
            {
                //burda düþmanýn scripti içindeki kodu çaðýrýyoruz, target pointini burdan ayarlýyoruz, ilk parametre oyuncuya
                //ulaþamazsa yani ai pathi partial olursa gidicek yedek hedef noktasý, 4 generatorden birini random seçiyor
                //ikinci parametre burda belirlenen hedef noktasý yani oyuncu oluyor bu durumda, 3. parametre ise bool olan
                //zombi oyuncuyu mu hedefliyor sorusu, eðer oyuncuya gidiyorsa true veriyoruz
                obj.GetComponent<Enemy>().SetTargetPoint(targetPoints[Random.Range(0, 4)], targetPoints[targetPoint], true);
            }
            //random sonucu target 4 generatorden biri ise olan kod
            else if(targetPoint >= 0 && targetPoint < 4)
            {
                if(targetPoint == 0)   //eðer ilk generatore gidiyorusa dynamic þekilde olan generatorlerin yaþama durumuna göre
                {                       //hedef belirleyen fonksiyonu çaðýrýyoruz
                    DynamicTargetSet(obj);
                }
                else if (targetPoint == 1)  //2. jenerator ise ilk baþta generator hala patladý mý diye kontrol ediyoruz
                {
                    if (g2Script.genoratorAlive) //eðer hala yaþýyorsa target noktasýný ona götürüyor, oyuncuyu kovalamadýðý için 
                    {                               //bool u false olarak döndürüyoruz, ikinci parametre ise boþ kalmamasý için oyuncuyu gösteriyoruz
                        obj.GetComponent<Enemy>().SetTargetPoint(targetPoints[targetPoint], targetPoints[5], false);
                    }
                    else    //eðer hedef generator patlamýþsa dinamik hedef belirleme fonksiyonuna yönlendiriyoruz orda yeni 
                    {       //generator hedefi belirliyor
                        DynamicTargetSet(obj);
                    }
                }
                else if (targetPoint == 2)
                {
                    if (g3Script.genoratorAlive)
                    {
                        obj.GetComponent<Enemy>().SetTargetPoint(targetPoints[targetPoint], targetPoints[5], false);
                    }
                    else
                    {
                        DynamicTargetSet(obj);
                    }
                }
                else if (targetPoint == 3)
                {
                    if (g4Script.genoratorAlive)
                    {
                        obj.GetComponent<Enemy>().SetTargetPoint(targetPoints[targetPoint], targetPoints[5], false);
                    }
                    else
                    {
                        DynamicTargetSet(obj);
                    }
                }       
            }

            //her zombi spawlandýðýnda, spawnlanýcak zombi sayýsýný azaltýyoruz
            zombieLeftInFunction--;
        }

    }

    //start fonksiyonunda çaðýrdýðýmýz, round sistemini baþlatan kod
    IEnumerator StartRound()
    {
        yield return new WaitForSeconds(2.9f);
        cameraForGameOver.SetActive(false);     //açýlýþ sahnesindeki geçiþi saðlayan kamerayý kapatýp oyuncuyu açýyoruz
        player.SetActive(true);
        StartCoroutine(ChangeWeapon(2));

        if (tutorialOn)     //tutorial açýksa ekranda tutorial diyaloglarý çýkýyor, hepsi bitince otomatik kapanýyor,
        {                   //restart game dendiðinde tekrar tutorial çýkmýyor
            yield return new WaitForSeconds(.5f);
            canvasAnimator.SetTrigger("TutorialOpen");
            yield return new WaitForSeconds(8f);
            StartCoroutine(TutorialDialogs());
        }
        
        //round sayýsýna göre kaç kere yapýlcaðýný saðlayan for
        for (currentRoundNumber = 0; currentRoundNumber < roundCount; currentRoundNumber++)
        {
            isRoundOver = false;   //eðer burdaysa round içindeyiz demektir, buradaki kodlar her round için sadece 1 kere çalýþtýrýlýr
            zombieLeft = firstRoundZombieCount;         //kaç zombi kaldýðýna bakýyoruz
            StartCoroutine(SpawnEnemy(firstRoundZombieCount, spawnTimer));      //düþmanlarý spawnlayan kodu çalýþtýrýyoruz
            Debug.Log("start round içi, if öncesi, is round over: " + isRoundOver + " round count: " + currentRoundNumber);
            yield return new WaitUntil(() => isRoundOver == true);  //round bitene kadar beklettiriyoruz, böylece kafasýna göre kod devam edemiyor
            if (isRoundOver)                                //isRoundOver sahnede zombi kalmayýnca true oluyor
            {
                Debug.Log("isRoundOverýn içine girdi");

                int roundLeft = roundCount - currentRoundNumber;    //round sayýsýný azaltýyoruz
                roundCountText.text = roundLeft.ToString();
                roundTimerText.text = timeBetweenRounds.ToString();
                timeCountBorder.SetActive(true);        //round arasýndaki sayacý çýkarýyoruz
                marketAvailableBorder.SetActive(true);  
                firstRoundZombieCount *= 2;         //sonraki round için çýkýcak zombi sayýsýný hesaplýyoruz, önceki zombi sayýsný 2 ile çarpýyoruz
                spawnTimer -= 0.7f;  //zombilerin her round daha hýzlý çýkmasýný saðlýyoruz

                restartTime = false;
                stopTimer = false;
                startTime = true;       //sayacý baþlatmayý saðlayan bool
                yield return new WaitForSeconds(timeBetweenRounds);     //round arasý süre kadar bekliyoruz

                timeCountBorder.SetActive(false);
                marketAvailableBorder.SetActive(false);
                startTime = false;
                restartTime = true;
                //süreyi sýfýrlayýp öbür rounda geçiyoruz yani tekrar for un baþýna
            }
        }

    }

    //tutorial diyaloglarýnda çýkan yazýlarý ayarlayan ve animasyonu yöneten kod
    IEnumerator TutorialDialogs()
    {
        yield return new WaitForSeconds(1f);
        canvasAnimator.SetTrigger("TutorialClose");
        yield return new WaitForSeconds(2f);
        tutorialText.text = "At first you have only Five-Seven pistol which is Number 7. You can press numbers for switching weapons, after you buy every weapon, you can switch with mouse scroll";
        canvasAnimator.SetTrigger("TutorialOpen");
        yield return new WaitForSeconds(10f);
        canvasAnimator.SetTrigger("TutorialClose");
        yield return new WaitForSeconds(1.5f);
        tutorialText.text = "Zombies will come to you wave by wave, try to survive. Some zombies will attack you while other ones will atack generators";
        canvasAnimator.SetTrigger("TutorialOpen");
        yield return new WaitForSeconds(10f);
        canvasAnimator.SetTrigger("TutorialClose");
        yield return new WaitForSeconds(1.5f);
        tutorialText.text = "There is a market that you can buy new weapons, ammos, grenade, repair generators or med kit. But you can only buy between rounds";
        canvasAnimator.SetTrigger("TutorialOpen");
        yield return new WaitForSeconds(10f);
        canvasAnimator.SetTrigger("TutorialClose");
        yield return new WaitForSeconds(1.5f);
        tutorialText.text = "For interact with ammo boxes or market, press E when you are close to them";
        canvasAnimator.SetTrigger("TutorialOpen");
        yield return new WaitForSeconds(10f);
        canvasAnimator.SetTrigger("TutorialClose");
        yield return new WaitForSeconds(1.5f);
        tutorialText.text = "You can throw grenade with G or Middle Mouse Button, G throws further while Middle Mouse Button throws close distance.";
        canvasAnimator.SetTrigger("TutorialOpen");
        yield return new WaitForSeconds(10f);
        canvasAnimator.SetTrigger("TutorialClose");
        tutorialOn = false;
    }

    //zombinin patlayan generatorlere gitmesini engelleyen ve yeni target ayarlayan kod
    public void DynamicTargetSet(GameObject obj)
    {
        //ilk generator patlamamýþsa ilk ona yönlendiriyor
        if (g1Script.genoratorAlive)
            obj.GetComponent<Enemy>().SetTargetPoint(targetPoints[0], targetPoints[5], false);
        else
        {
            if (g2Script.genoratorAlive) //ilk generator patlamýþsa ikinciyi kontrol ediyoruz
            {
                obj.GetComponent<Enemy>().SetTargetPoint(targetPoints[1], targetPoints[5], false);
            }
            else
            {
                if (g3Script.genoratorAlive)   //2. generator patladýysa 3 ü kontrol ediyoruz
                {
                    obj.GetComponent<Enemy>().SetTargetPoint(targetPoints[2], targetPoints[5], false);
                }
                else
                {
                    if (g4Script.genoratorAlive)  //3 de patladýysa artýk 4 ü kontrol ediyoruz, zaten 4 de patlayýnca oyun bitiyor
                    {
                        obj.GetComponent<Enemy>().SetTargetPoint(targetPoints[3], targetPoints[5], false);
                    }
                    else
                    {
                        obj.GetComponent<Enemy>().SetTargetPoint(targetPoints[1], targetPoints[5], true);
                    }
                }
            }

        }
    }

    //oyuncuya hasar verdiren kod
    public void GetDamageAsPlayer(int enemyDamage)
    {
        playerHealth -= enemyDamage;    //canýný azaltýp ui animasyonunu oynatýyoruz

        if(playerHealthUI.isActiveAndEnabled)
            playerHealthUI.Play("PlayerHealthDamage");

        if (playerHealth > 0)
        {
            playerHealthSlider.value = playerHealth;
            playerHealthText.text = playerHealth.ToString();
        }
        else
        {
            playerHealthSlider.value = 0;
            playerHealthText.text = "0";
        }
    }
    
    //oyuncu ölünce veya oyun bitince çalýþan kod
    public IEnumerator PlayerDead(string situation)
    {
        audioManager.Play("GameOver Theme");
        canvasAnimator.SetTrigger("GameOverCanvasTransition");    //ui da game over ile ilgili olan þeyleri baþlatýyoruz
        yield return new WaitForSeconds(.95f);
        canvasGameplay.SetActive(false);
        player.SetActive(false);
        cameraForGameOver.SetActive(true);    //kamera animasyonu için playerý kapatýp baþka kamerayý açýyoruz
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        cameraGameOverAnimator.SetTrigger("Game Over");
        yield return new WaitForSeconds(2.35f);
        if(situation == "lose")             //kazanýp kaybetmemize göre yazý deðiþiyor
        {
            winText.text = "Game Over";
        }
        else if(situation == "win")
        {
            winText.text = "You did it!!\nYou won the game";
        }
        yield return new WaitForSeconds(.15f);
        canvasGameOver.SetActive(true);
        gameOverScoreText.text = "Highest Kill: " + killCount.ToString();
        canvasAnimator.SetTrigger("GameOverUIStart");           //yazýlarýn çýkmasýný saðlayan animasyon
    }

    public void GameOverButtons(int index)
    {
        StartCoroutine(GameOver(index));
    }

    //game over ekranýndaki butonlarýn kodu
    public IEnumerator GameOver(int index)
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        player.SetActive(false);
        cameraForGameOver.SetActive(true);
        switch (index)
        {
            case 1:     //restart game dendiðinde kod
                killCount = 0;
                cameraGameOverAnimator.SetTrigger("RestartGame");
                canvasAnimator.SetTrigger("RestartGame");
                yield return new WaitForSeconds(2f);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                SceneManager.LoadScene(1);
                break;
            case 2:     //return to menu dendiðinde kod
                scoreList.Add(killCount);       //scoreboard için listeye skorumuzu ekliyor
                killCount = 0;
                cameraGameOverAnimator.SetTrigger("ReturnToMenu");
                canvasAnimator.SetTrigger("RestartGame");
                yield return new WaitForSeconds(2f);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                SceneManager.LoadScene(0);
                break;
        }
    }

    //bomba atmamýzý saðlayan kod
    IEnumerator CreateGrenade(int index)
    {
        yield return new WaitUntil(() => animationFinished == true);    //eðer hazýrda bomba atma animasyonu varsa onu bekliyor
        grenadeAnimator.SetTrigger("Release");      //atma animasyonunu oynatýyor
        yield return new WaitForSeconds(0.37f);
        //grenade objesi oluþturuyoruz, elini býraktýðý noktada oluþturup rigidbodysine kuvvet uyguluyoruz
        GameObject grenadeObj = Instantiate(Grenade, grenadeCreatePoint.transform.position, grenadeCreatePoint.transform.rotation);
        Rigidbody rb = grenadeObj.GetComponent<Rigidbody>();
        Vector3 throwAngle = Quaternion.AngleAxis(110, fpsCam.transform.forward) * fpsCam.transform.forward;
        audioManager.Play("Grenade Swing");

        if(index == 0)  //eðer index 0 ise G ye basarak atmýþ demektir yani uzaða, else kýsmýnda ise mouse orta tuþu ile atmýþtýr
            rb.AddForce(throwAngle * grenadeThrowForceLong, ForceMode.VelocityChange);
        else
            rb.AddForce(throwAngle * grenadeThrowForceShort, ForceMode.VelocityChange);

        yield return new WaitForSeconds(0.5f);
        ResetAnimationAndDeactivateWeapon(9);  //grende atýldýktan sonra eli kapatýyoruz eski silahýmýz neyse onu açýyoruz
        StartCoroutine(ChangeWeapon(currentWeapon - 1));
        animationFinished = false;
        grenadeInProggress = false;     //buglarý engellemek için olan kodlar
        throwingGrenade = false;

        if(grenadeNumber > 0)       //atýldýðý için 1 azaltýyoruz
            grenadeNumber--;

        grenadeNumberText.text = grenadeNumber.ToString();  //ui ý yeniliyoruz
    }

    //grenade atma animasyonu bitmeden grenade atma tuþlarýný spamlayýnca oluþan bir bugý engellemek için fonksiyon 
    IEnumerator GrenadeLoop()  
    {   
        while (true)
        {
            if (grenadeInProggress)
            {
                grenadeInProggress = false;
                throwingGrenade = false;
            }
            yield return new WaitForSeconds(6f);
        }
        
    }
}
