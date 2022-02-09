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

        zombieLeft = firstRoundZombieCount;                                 //ui elementlerini yazd�r�yoruz
        zombieCountText.text = firstRoundZombieCount.ToString();
        grenadeNumberText.text = grenadeNumber.ToString();
        playerKillText.text = "0 Kills";
        playerNameText.text = MainMenuScript.myName + " :";
        roundCountText.text = roundCount.ToString();

        StartCoroutine(StartRound());       //roundun ba�lamas�n� sa�layan coroutine
        StartCoroutine(GrenadeLoop());          //grenade atarken olu�an bir bug� engellemek i�in yap�lm�� coroutine
    }
    
    
    void Update()
    {
        if(zombieLeft == 0)         
        {
            isRoundOver = true;
        }

        if(currentRoundNumber == roundCount)        //son roundu bitirdi�inde win ekran� gelmesi i�in
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

        //ammo boxlar�n veya markete bakt���n�zda ��kan indicatorlar�n kodu, ifin b�yle olmas�n�n sebebi, s�rekli bir raycast g�ndermemizden dolay�
        if (!Ak.isShooting && !Deagle.isShooting && !Knife.isShooting && !M4A1.isShooting && !AWP.isShooting && !MP7.isShooting && !FiveSeven.isShooting && !Shotgun.isShooting)
        {
            RaycastHit hit;             //silahlar i�in bunu kontrol etmezsek daha sonra ate� eterken bug ile kar��la��yoruz

            if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, 3))
            {
                if (hit.transform.gameObject.CompareTag("MarketTrigger"))   //�arpt��� �ey neyse tag�na g�re onun indicator�n� ��kart�yor
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

        //tag�na bakarak ona g�re ammoboxlar veya di�er �eylerle etkile�ime girmesini sa�l�yor
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

        //pause men�s� kapatma
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isMarketOpen)
            {
                CloseMarket();
            }
        }

        //silahlar aras� ge�i� yapmak i�in klavyedeki say� olan tu�lar. A��klama sadece ilk ifte var(Alpha1 de) di�erleri ayn� zaten
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (currentWeapon != 1 && !isMarketOpen && !throwingGrenade && doesHaveAk) //buglar� �nlemek i�in baz� �eyleri kontrol ediyoruz
            {
                ResetAnimationAndDeactivateWeapon(currentWeapon); //de�i�tirmeden �nceki silah� g�venli �ekilde kapatan fonksiyon
                lastWeapon = currentWeapon;  //son silah� �u an elimizdeki silah yap�yor
                currentWeapon = 1;  //current weapon� yeni al�ca��m�z silah yap�yor
                StartCoroutine(ChangeWeapon(0));   //al�ca��m�z silah� ��kartma animasyonunu ve aktifle�tirmeleri yap�yor
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

        //elimizdekinden �nceki silah� almak i�in yap�lan i�lemler
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!isMarketOpen && !throwingGrenade)
            {
                ResetAnimationAndDeactivateWeapon(currentWeapon);   //elimizde olan silah� kapat�yor
                int temp = lastWeapon;
                lastWeapon = currentWeapon;         //elimizdeki silah� �nceki silaha �eviriyoruz
                currentWeapon = temp;
                StartCoroutine(ChangeWeapon(currentWeapon - 1));    //�nceki silah neyse onu a��yoruz
            }     
        }

        //grenade atma kodu ama sadece ilk basma i�in, as�l atma olay� parma��m�z� tu�tan kald�rd���m�zda
        if (Input.GetKeyDown(KeyCode.G) || Input.GetMouseButtonDown(2))
        {
            if (!isMarketOpen && !grenadeInProggress && grenadeNumber > 0)
            {
                throwingGrenade = true;
                ResetAnimationAndDeactivateWeapon(currentWeapon);   //elimizdeki silah� kapat�yoruz ekranda g�z�kmesin diye
                StartCoroutine(ChangeWeapon(8));    //grenade i a��yoruz ekranda g�z�kmesi i�in
            }
        }

        //bu ikisi bombay� f�rlatmaya yarayan kod
        if (Input.GetKeyUp(KeyCode.G))
        {
            if (!isMarketOpen && !grenadeInProggress)
            {
                grenadeInProggress = true;
                StartCoroutine(CreateGrenade(0));     //bu coroutine ile bomba olu�turup f�rlat�yoruz,           
            }                                           // indexi uza�a veya yak�na atmay� de�i�tiriyor
        }
        if (Input.GetMouseButtonUp(2))
        {
            if (!isMarketOpen && !grenadeInProggress)
            {
                grenadeInProggress = true;
                StartCoroutine(CreateGrenade(1));
            }
        }

        //silahlar� ayn� zamanda mouse tekerli�i ile de de�i�tirebilicektik ve bunu sistem otomatik olarak a��k olanlar� bularak yap�cakt�
        //fakat silah sat�n alma sistemini tamamlad�ktan sonra
        //bu sistemde baya de�i�iklik yapmak zorunda kald�k, tam hallettik dedi�imizde Q tu�u sisteminde hatalarla da kar��la�t�k
        //bir de hala 8. silahtan 1. silaha giderken s�k�nt� ��kart�yordu, o y�zden t�m silahlar� tamamlad���nda a��lan sisteme �evirdik
        //silahlar tam olmadan �al��an kodu yorum i�inde b�rakt�k.
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
                        currentWeapon = 0;      //�u an ki silah 8. silah de�ilse 1 artt�r�yor, 8 ise 0 a getiriyor
                                                //daha sonra sonraki silah� a��yoruz
                    }
                    currentWeapon++;
                    StartCoroutine(ChangeWeapon(currentWeapon - 1));

                    //Normalde her silah yokken de bu fonksiyon �al���cakt�, otomatik a��k olan silah� bulucakt� 
                    //ama maalesef �ok fazla bugla kar��la�t���m�z i�in erteledik

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

        //round aras�ndaki saniyeyi g�steren kod
        if (startTime == true)
        {
            timeTillStart += Time.deltaTime;
            double time = timeBetweenRounds - timeTillStart;   //s�reyi geriye do�ru sayd�r�yor
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

        //b��ak elimizdeyse mermi yerindeki slash� kald�r�yor
        if (currentWeapon != 3)
            slash.text = "/";
        else
            slash.text = "";

        //ui da belli ba�l� �eyleri yazd�r�yoruz
        zombieCountText.text = zombieLeft.ToString();
        playerMoneyText.text = "$" + playerMoney.ToString();

    }
    
    //marketi kapatmak i�in gereken kod, market a��kken k�s�tlad���m�z �eyleri a��yoruz
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

        FindObjectOfType<LevelCanvasScript>().ResetBuyButtons();  //buy buttonlar� resetliyoruz
        isMarketOpen = false;
        audioManager.Play("Menu5");
        marketPanel.SetActive(false);
        menu1.SetActive(true);
        weaponsMenu.SetActive(false);
        utilitiesMenu.SetActive(false);
    }

    //marketi a�mak i�in kod, baz� �eyleri k�s�tl�yoruz ki market a��kken oyuncu her �eyi yapamas�n
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
        FindObjectOfType<LevelCanvasScript>().CheckBuyAvailable();  //buy tu�lar�n� kontrol ettiriyoruz
        marketPanel.SetActive(true);
    }

    //yeni silah� a�ma ve switch animasyonu ve sesini �ald�rmak i�in olan kod, indexe g�re �al���yor
    IEnumerator ChangeWeapon(int index)
    {
        weapons[index].SetActive(true); //arrayden indexi al�p en ba�ta aktif ediyor

        switch (index)
        {
            case 0:
                movement.speed = 4f;    //silaha �zel hareket h�z� veriyor
                icon762.SetActive(true);    //silah�n mermi ikonunu g�steriyor
                Ak.canFire = false;     //switch animasyonu oynarken ate� edip reload yapmas�n diye
                Ak.canReload = false;
                akAnimator2.Play("SwitchAk");
                akAnimator.Play("Switch");
                yield return new WaitForSeconds(1.3f);
                Ak.canFire = true;      //switch animasyonu bittikten sonra �zellikleri tekrar devrede
                Ak.canReload = true;
                if (PlayerMovement.isMoving)   //oyuncu silah de�i�tirirken hareket ediyorsa ona g�re trigger uyguluyoruz
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
                crosshair.SetActive(false);  //awpye ge�ince crosshairi kapat�yoruz
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

    //silah de�i�tirirken elimizde haz�r bulunan silah� kapatma ve e�er animasyondaysa onu s�f�rlama kodu
    public void ResetAnimationAndDeactivateWeapon(int currentWeapon)
    {
        switch (currentWeapon)
        {
            case 1:
                akAnimator.CrossFade("Idle", 0f, 0);    //animasyonun ortas�ndaysa transform de�erleri �yle kalmas�n diye
                akAnimator.CrossFade("Idle", 0f, 1);    //animasyonu normal haline �eviriyoruz, b�ylece tekrar o silaha
                akAnimator2.CrossFade("Idle", 0f, 0);   //ge�ilince silah normal �ekilde ��k�yor
                akAnimator.Update(0f);
                akAnimator.Update(0f);
                akAnimator2.Update(0f);
                akAnimator2.Update(0f);
                icon762.SetActive(false);
                weapons[0].gameObject.SetActive(false);         //silah�n aktifli�ini kapat�yoruz
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
                crosshair.SetActive(true);  //awp olunca crosshairi geri a��yoruz
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

    //d��manlar� spawnlayan kod, ilk parametre ka� zombi ��kaca��n� al�yor, ikinci parametre spawnlar�n aras�nda ka� saniye bekleyece�ini
    IEnumerator SpawnEnemy(int firstRoundZombieCount, float spawnTimer)
    {
        zombieLeftInFunction = firstRoundZombieCount;    //de�eri de�i�tirebilice�imiz bir de�i�kene at�yoruz

        Debug.Log("spawn enemy while �ncesi");

        while (zombieLeftInFunction > 0)   //d��man say�s� bitene kadar i�leme devam ediyor
        {
            Debug.Log("spawn while i�i");
            yield return new WaitForSeconds(spawnTimer);   //spawnlar aras� beklenen s�re

            int enemyType = Random.Range(0, 3);
            int spawnPoint = Random.Range(0, 6);    //burda de�erlerden random al�yoruz, b�ylece enemy typlar�, nerden do�acaklar�
            int targetPoint = Random.Range(0, 12);  //nereye gidicekleri random olarak belirleniyor, bunlar� inspectordan at�yoruz

            //random olan d��man� spawnlama kodu
            GameObject obj = Instantiate(enemies[enemyType], spawnPoints[spawnPoint].transform.position, Quaternion.identity);

            //random sonucu d��man, target olarak oyuncuyu mu se�ti diye kontrol ediyoruz
            if (targetPoint > 3) //oyuncuya sald�r�caksa olan kod
            {
                //burda d��man�n scripti i�indeki kodu �a��r�yoruz, target pointini burdan ayarl�yoruz, ilk parametre oyuncuya
                //ula�amazsa yani ai pathi partial olursa gidicek yedek hedef noktas�, 4 generatorden birini random se�iyor
                //ikinci parametre burda belirlenen hedef noktas� yani oyuncu oluyor bu durumda, 3. parametre ise bool olan
                //zombi oyuncuyu mu hedefliyor sorusu, e�er oyuncuya gidiyorsa true veriyoruz
                obj.GetComponent<Enemy>().SetTargetPoint(targetPoints[Random.Range(0, 4)], targetPoints[targetPoint], true);
            }
            //random sonucu target 4 generatorden biri ise olan kod
            else if(targetPoint >= 0 && targetPoint < 4)
            {
                if(targetPoint == 0)   //e�er ilk generatore gidiyorusa dynamic �ekilde olan generatorlerin ya�ama durumuna g�re
                {                       //hedef belirleyen fonksiyonu �a��r�yoruz
                    DynamicTargetSet(obj);
                }
                else if (targetPoint == 1)  //2. jenerator ise ilk ba�ta generator hala patlad� m� diye kontrol ediyoruz
                {
                    if (g2Script.genoratorAlive) //e�er hala ya��yorsa target noktas�n� ona g�t�r�yor, oyuncuyu kovalamad��� i�in 
                    {                               //bool u false olarak d�nd�r�yoruz, ikinci parametre ise bo� kalmamas� i�in oyuncuyu g�steriyoruz
                        obj.GetComponent<Enemy>().SetTargetPoint(targetPoints[targetPoint], targetPoints[5], false);
                    }
                    else    //e�er hedef generator patlam��sa dinamik hedef belirleme fonksiyonuna y�nlendiriyoruz orda yeni 
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

            //her zombi spawland���nda, spawnlan�cak zombi say�s�n� azalt�yoruz
            zombieLeftInFunction--;
        }

    }

    //start fonksiyonunda �a��rd���m�z, round sistemini ba�latan kod
    IEnumerator StartRound()
    {
        yield return new WaitForSeconds(2.9f);
        cameraForGameOver.SetActive(false);     //a��l�� sahnesindeki ge�i�i sa�layan kameray� kapat�p oyuncuyu a��yoruz
        player.SetActive(true);
        StartCoroutine(ChangeWeapon(2));

        if (tutorialOn)     //tutorial a��ksa ekranda tutorial diyaloglar� ��k�yor, hepsi bitince otomatik kapan�yor,
        {                   //restart game dendi�inde tekrar tutorial ��km�yor
            yield return new WaitForSeconds(.5f);
            canvasAnimator.SetTrigger("TutorialOpen");
            yield return new WaitForSeconds(8f);
            StartCoroutine(TutorialDialogs());
        }
        
        //round say�s�na g�re ka� kere yap�lca��n� sa�layan for
        for (currentRoundNumber = 0; currentRoundNumber < roundCount; currentRoundNumber++)
        {
            isRoundOver = false;   //e�er burdaysa round i�indeyiz demektir, buradaki kodlar her round i�in sadece 1 kere �al��t�r�l�r
            zombieLeft = firstRoundZombieCount;         //ka� zombi kald���na bak�yoruz
            StartCoroutine(SpawnEnemy(firstRoundZombieCount, spawnTimer));      //d��manlar� spawnlayan kodu �al��t�r�yoruz
            Debug.Log("start round i�i, if �ncesi, is round over: " + isRoundOver + " round count: " + currentRoundNumber);
            yield return new WaitUntil(() => isRoundOver == true);  //round bitene kadar beklettiriyoruz, b�ylece kafas�na g�re kod devam edemiyor
            if (isRoundOver)                                //isRoundOver sahnede zombi kalmay�nca true oluyor
            {
                Debug.Log("isRoundOver�n i�ine girdi");

                int roundLeft = roundCount - currentRoundNumber;    //round say�s�n� azalt�yoruz
                roundCountText.text = roundLeft.ToString();
                roundTimerText.text = timeBetweenRounds.ToString();
                timeCountBorder.SetActive(true);        //round aras�ndaki sayac� ��kar�yoruz
                marketAvailableBorder.SetActive(true);  
                firstRoundZombieCount *= 2;         //sonraki round i�in ��k�cak zombi say�s�n� hesapl�yoruz, �nceki zombi say�sn� 2 ile �arp�yoruz
                spawnTimer -= 0.7f;  //zombilerin her round daha h�zl� ��kmas�n� sa�l�yoruz

                restartTime = false;
                stopTimer = false;
                startTime = true;       //sayac� ba�latmay� sa�layan bool
                yield return new WaitForSeconds(timeBetweenRounds);     //round aras� s�re kadar bekliyoruz

                timeCountBorder.SetActive(false);
                marketAvailableBorder.SetActive(false);
                startTime = false;
                restartTime = true;
                //s�reyi s�f�rlay�p �b�r rounda ge�iyoruz yani tekrar for un ba��na
            }
        }

    }

    //tutorial diyaloglar�nda ��kan yaz�lar� ayarlayan ve animasyonu y�neten kod
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
        //ilk generator patlamam��sa ilk ona y�nlendiriyor
        if (g1Script.genoratorAlive)
            obj.GetComponent<Enemy>().SetTargetPoint(targetPoints[0], targetPoints[5], false);
        else
        {
            if (g2Script.genoratorAlive) //ilk generator patlam��sa ikinciyi kontrol ediyoruz
            {
                obj.GetComponent<Enemy>().SetTargetPoint(targetPoints[1], targetPoints[5], false);
            }
            else
            {
                if (g3Script.genoratorAlive)   //2. generator patlad�ysa 3 � kontrol ediyoruz
                {
                    obj.GetComponent<Enemy>().SetTargetPoint(targetPoints[2], targetPoints[5], false);
                }
                else
                {
                    if (g4Script.genoratorAlive)  //3 de patlad�ysa art�k 4 � kontrol ediyoruz, zaten 4 de patlay�nca oyun bitiyor
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
        playerHealth -= enemyDamage;    //can�n� azalt�p ui animasyonunu oynat�yoruz

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
    
    //oyuncu �l�nce veya oyun bitince �al��an kod
    public IEnumerator PlayerDead(string situation)
    {
        audioManager.Play("GameOver Theme");
        canvasAnimator.SetTrigger("GameOverCanvasTransition");    //ui da game over ile ilgili olan �eyleri ba�lat�yoruz
        yield return new WaitForSeconds(.95f);
        canvasGameplay.SetActive(false);
        player.SetActive(false);
        cameraForGameOver.SetActive(true);    //kamera animasyonu i�in player� kapat�p ba�ka kameray� a��yoruz
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        cameraGameOverAnimator.SetTrigger("Game Over");
        yield return new WaitForSeconds(2.35f);
        if(situation == "lose")             //kazan�p kaybetmemize g�re yaz� de�i�iyor
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
        canvasAnimator.SetTrigger("GameOverUIStart");           //yaz�lar�n ��kmas�n� sa�layan animasyon
    }

    public void GameOverButtons(int index)
    {
        StartCoroutine(GameOver(index));
    }

    //game over ekran�ndaki butonlar�n kodu
    public IEnumerator GameOver(int index)
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        player.SetActive(false);
        cameraForGameOver.SetActive(true);
        switch (index)
        {
            case 1:     //restart game dendi�inde kod
                killCount = 0;
                cameraGameOverAnimator.SetTrigger("RestartGame");
                canvasAnimator.SetTrigger("RestartGame");
                yield return new WaitForSeconds(2f);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                SceneManager.LoadScene(1);
                break;
            case 2:     //return to menu dendi�inde kod
                scoreList.Add(killCount);       //scoreboard i�in listeye skorumuzu ekliyor
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

    //bomba atmam�z� sa�layan kod
    IEnumerator CreateGrenade(int index)
    {
        yield return new WaitUntil(() => animationFinished == true);    //e�er haz�rda bomba atma animasyonu varsa onu bekliyor
        grenadeAnimator.SetTrigger("Release");      //atma animasyonunu oynat�yor
        yield return new WaitForSeconds(0.37f);
        //grenade objesi olu�turuyoruz, elini b�rakt��� noktada olu�turup rigidbodysine kuvvet uyguluyoruz
        GameObject grenadeObj = Instantiate(Grenade, grenadeCreatePoint.transform.position, grenadeCreatePoint.transform.rotation);
        Rigidbody rb = grenadeObj.GetComponent<Rigidbody>();
        Vector3 throwAngle = Quaternion.AngleAxis(110, fpsCam.transform.forward) * fpsCam.transform.forward;
        audioManager.Play("Grenade Swing");

        if(index == 0)  //e�er index 0 ise G ye basarak atm�� demektir yani uza�a, else k�sm�nda ise mouse orta tu�u ile atm��t�r
            rb.AddForce(throwAngle * grenadeThrowForceLong, ForceMode.VelocityChange);
        else
            rb.AddForce(throwAngle * grenadeThrowForceShort, ForceMode.VelocityChange);

        yield return new WaitForSeconds(0.5f);
        ResetAnimationAndDeactivateWeapon(9);  //grende at�ld�ktan sonra eli kapat�yoruz eski silah�m�z neyse onu a��yoruz
        StartCoroutine(ChangeWeapon(currentWeapon - 1));
        animationFinished = false;
        grenadeInProggress = false;     //buglar� engellemek i�in olan kodlar
        throwingGrenade = false;

        if(grenadeNumber > 0)       //at�ld��� i�in 1 azalt�yoruz
            grenadeNumber--;

        grenadeNumberText.text = grenadeNumber.ToString();  //ui � yeniliyoruz
    }

    //grenade atma animasyonu bitmeden grenade atma tu�lar�n� spamlay�nca olu�an bir bug� engellemek i�in fonksiyon 
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
