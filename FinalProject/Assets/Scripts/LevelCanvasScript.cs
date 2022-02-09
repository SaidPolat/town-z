using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class LevelCanvasScript : MonoBehaviour 
{
    GameControl gameControl;
    public AudioManager audioManager;

    [Header("Generators")]
    public Generator generator1;
    public Generator generator2;
    public Generator generator3;
    public Generator generator4;

    [Header("GunScripts")]
    public Ak akScript;
    public M4A1 m4Script;
    public AWP awpScript;
    public Deagle deagleScript;
    public Knife knifeScript;
    public MP7 mp7Script;
    public FiveSeven fiveSevenScript;
    public Shotgun shotgunScript;

    [Header("MenuPanels")]
    public GameObject menu1;
    public GameObject weaponsMenu;
    public GameObject utilitiesMenu;

    [Header("IconsForGuns")]
    public Image iconAk;
    public Image iconM4;
    public Image iconDeagle;
    public Image iconFiveSeven;
    public Image iconKnife;
    public Image iconAwp;
    public Image iconMp7;
    public Image iconShotgun;

    [Header("Buttons")]
    public Button deagleButton;
    public Button shotgunButton;
    public Button mp7Button;
    public Button akButton;
    public Button m4Button;
    public Button awpButton;
    public Button karambitButton;
    public Button button762;
    public Button button556;
    public Button buttonShell;
    public Button button9mm;
    public Button button40m;
    public Button buttonGrenade;
    public Button buttonGenerator;
    public Button buttonMedKit;

    [Header("MenuTexts")]
    public TextMeshProUGUI menu1Text;
    public TextMeshProUGUI weaponsMenuText;
    public TextMeshProUGUI utilitiesMenuText;

    [Header("WeaponsMenuImages")]
    public Sprite deagleImage;
    public Sprite shotgunImage;
    public Sprite mp7Image;
    public Sprite akImage;
    public Sprite m4Image;
    public Sprite awpImage;
    public Sprite karambitImage;
    public Image weaponsMenuImage;

    [Header("UtilitiesMenuImages")]
    public Sprite image762;
    public Sprite image556;
    public Sprite imageShell;
    public Sprite image9mm;
    public Sprite image40m;
    public Sprite imageGrenade;
    public Sprite imageGenerator;
    public Sprite imageMedkit;
    public Image utilitiesMenuImage;

    private void Start()
    {
        gameControl = FindObjectOfType<GameControl>();

    }

    //market menüleri arasý geçiþ butonlarý kodu
    public void MenuSwitchButtons(int index)
    {
        audioManager.Play("Menu3");
        switch (index)
        {
            case 1:
                menu1.SetActive(false);
                weaponsMenu.SetActive(true);
                CheckBuyAvailable();
                break;
            case 2:
                weaponsMenu.SetActive(false);
                menu1.SetActive(true);
                ResetBuyButtons();
                break;
            case 3:
                menu1.SetActive(false);
                utilitiesMenu.SetActive(true);
                CheckBuyAvailable();
                break;
            case 4:
                utilitiesMenu.SetActive(false);
                menu1.SetActive(true);
                ResetBuyButtons();
                break;
        }
    }

    //markette satýn alma buttonlarýnýn kodu
    public void BuyButtons(int index)
    {
        audioManager.Play("Menu4");
        switch (index)
        {
            case 1:
                if (!gameControl.doesHaveDeagle && gameControl.playerMoney >= gameControl.deagleMoney)
                { //eðer o silahý yoksa ve parasý o silahý almaya yetiyorsa satýn alma iþlemi yapýlýyor
                    gameControl.doesHaveDeagle = true;      //silahý aldýðýný gösteren bool trueya dönüyor
                    gameControl.playerMoney -= gameControl.deagleMoney;     //parasý o silahýn parasý kadar azalýyor
                    deagleButton.enabled = false;       //tuþa bir daha basamýyor
                    deagleButton.GetComponent<Image>().color = Color.grey;  //tuþun rengini griye çeviriyoruz
                    iconDeagle.color = Color.white;     //saðdaki silahlarý gösteren ui da silaha artýk sahip olduðumuzu
                }                                           //gösterecek þekilde silah logosu kýrmýzýdan beyaza dönüyor
                break;
            case 2:
                if (!gameControl.doesHaveShotgun && gameControl.playerMoney >= gameControl.shotgunMoney)
                {
                    gameControl.doesHaveShotgun = true;
                    gameControl.playerMoney -= gameControl.shotgunMoney;
                    shotgunButton.enabled = false;
                    shotgunButton.GetComponent<Image>().color = Color.grey;
                    iconShotgun.color = Color.white;
                }            
                break;
            case 3:
                if (!gameControl.doesHaveMp7 && gameControl.playerMoney >= gameControl.mp7Money)
                {
                    gameControl.doesHaveMp7 = true;
                    gameControl.playerMoney -= gameControl.mp7Money;
                    mp7Button.enabled = false;
                    mp7Button.GetComponent<Image>().color = Color.grey;
                    iconMp7.color = Color.white;
                }
                break;
            case 4:
                if (!gameControl.doesHaveAk && gameControl.playerMoney >= gameControl.akMoney)
                {
                    gameControl.doesHaveAk = true;
                    gameControl.playerMoney -= gameControl.akMoney;
                    akButton.enabled = false;
                    akButton.GetComponent<Image>().color = Color.grey;
                    iconAk.color = Color.white;
                }
                break;
            case 5:
                if (!gameControl.doesHaveM4 && gameControl.playerMoney >= gameControl.m4Money)
                {
                    gameControl.doesHaveM4 = true;
                    gameControl.playerMoney -= gameControl.m4Money;
                    m4Button.enabled = false;
                    m4Button.GetComponent<Image>().color = Color.grey;
                    iconM4.color = Color.white;
                }
                break;
            case 6:
                if (!gameControl.doesHaveAwp && gameControl.playerMoney >= gameControl.awpMoney)
                {
                    gameControl.doesHaveAwp = true;
                    gameControl.playerMoney -= gameControl.awpMoney;
                    awpButton.enabled = false;
                    awpButton.GetComponent<Image>().color = Color.grey;
                    iconAwp.color = Color.white;
                }
                break;
            case 7:
                if (!gameControl.doesHaveKarambitFade && gameControl.playerMoney >= gameControl.knifeMoney)
                {
                    knifeScript.doesHaveFade = true;
                    knifeScript.knifeBlack.SetActive(false);
                    knifeScript.knifeFade.SetActive(true);      //býçak için görünüþünü deðiþtirdiði için öncekini kapatýp yenisini açýyoruz
                    gameControl.doesHaveKarambitFade = true;
                    gameControl.playerMoney -= gameControl.knifeMoney;
                    karambitButton.enabled = false;
                    karambitButton.GetComponent<Image>().color = Color.grey;    
                }
                break;
            case 8:
                if(gameControl.playerMoney >= gameControl.money762)
                {
                    akScript.ammoAmount += 30;          //o mermi türünü kullanan silahlara özel mühimmat ekliyor
                    awpScript.ammoAmount += 30;
                    gameControl.playerMoney -= gameControl.money762;
                } 
                break;
            case 9:
                if (gameControl.playerMoney >= gameControl.money556)
                {
                    m4Script.ammoAmount += 40;
                    gameControl.playerMoney -= gameControl.money556;
                }  
                break;
            case 10:
                if (gameControl.playerMoney >= gameControl.moneyShell)
                {
                    shotgunScript.ammoAmount += 20;
                    gameControl.playerMoney -= gameControl.moneyShell;
                }   
                break;
            case 11:
                if (gameControl.playerMoney >= gameControl.money9mm)
                {
                    mp7Script.ammoAmount += 60;
                    fiveSevenScript.ammoAmount += 60;
                    gameControl.playerMoney -= gameControl.money9mm;
                }  
                break;
            case 12:
                if (gameControl.playerMoney >= gameControl.money40m)
                {
                    deagleScript.ammoAmount += 21;
                    gameControl.playerMoney -= gameControl.money40m;
                }   
                break;
            case 13:
                if (gameControl.playerMoney >= gameControl.moneyGrenade)
                {
                    gameControl.grenadeNumber++;
                    gameControl.playerMoney -= gameControl.moneyGrenade;
                    gameControl.grenadeNumberText.text = gameControl.grenadeNumber.ToString();
                }   
                break;
            case 14:
                if (gameControl.playerMoney >= gameControl.moneyRepairGenorator)
                {   //generatorlarýn canlarý 100 den küçükse sadece yapabiliyor
                    if (generator1.health < 100 || generator2.health < 100 || generator3.health < 100 || generator4.health < 100)
                    {       //patlayan generator bir daha yenilenemiyor
                        if (generator1.health > 0)
                        {
                            generator1.health += 30;
                            if (generator1.health > 100)
                                generator1.health = 100;
                            generator1.healthText.text = generator1.health.ToString();
                            generator1.slider.value = generator1.health;
                        }
                        if (generator2.health > 0)
                        {
                            generator2.health += 30;
                            if (generator2.health > 100)
                                generator2.health = 100;
                            generator2.healthText.text = generator2.health.ToString();
                            generator2.slider.value = generator2.health;
                        }
                        if (generator3.health > 0)
                        {
                            generator3.health += 30;
                            if (generator3.health > 100)
                                generator3.health = 100;
                            generator3.healthText.text = generator3.health.ToString();
                            generator3.slider.value = generator3.health;
                        }
                        if (generator4.health > 0)
                        {
                            generator4.health += 30;
                            if (generator4.health > 100)
                                generator4.health = 100;
                            generator4.healthText.text = generator4.health.ToString();
                            generator4.slider.value = generator4.health;
                        }
                        gameControl.playerMoney -= gameControl.moneyRepairGenorator;
                    }
                }                 
                break;
            case 15:
                if (gameControl.playerMoney >= gameControl.moneyHealth)
                {
                    if (gameControl.playerHealth < 100)
                    {
                        gameControl.playerHealth += 40;
                        if (gameControl.playerHealth > 100)
                            gameControl.playerHealth = 100;
                        gameControl.playerHealthText.text = gameControl.playerHealth.ToString();
                        gameControl.playerHealthSlider.value = gameControl.playerHealth;
                        gameControl.playerMoney -= gameControl.moneyHealth;
                    }
                }               
                break;
        }
        CheckBuyAvailable();        //her satýn almadan sonra paranýn neye yeticeðine göre butonlarý kapatýyor
    }

    //markette weapons kýsmý için mouse imleci üzerine geldiðinde onun kendi fotoðrafý ve bilgilerini yazdýran kod
    public void OnPointerButtonsForWeapons(int index)
    {
        audioManager.Play("OnPointerSFX");
        switch (index)
        {
            case 1:
                menu1Text.text = "Weapons menu that you can buy new and more powerful weapons.";
                break;
            case 2:
                menu1Text.text = "Utilities menu that you can buy ammos for your weapons, grenades, health, etc..";
                break;
            case 3:
                weaponsMenuImage.sprite = deagleImage;
                weaponsMenuText.text = "Desert Eagle\n----------------------------------------------------------------------\n" +
                    "Damage: 50\nRange: 50\nFire Rate: 0.45 Second\nMagazine Ammo: 7\nStarting Ammo: 35\nAmmo Type: .40 Magnum";
                break;    
            case 5:
                weaponsMenuImage.sprite = shotgunImage;
                weaponsMenuText.text = "SPAS-12 Shotgun\n----------------------------------------------------------------------\n" +
                    "Damage: 70\nRange: 50\nFire Rate: 0.45 Second\nMagazine Ammo: 6\nStarting Ammo: 30\nAmmo Type: Shotgun Shell";
                break;
            case 6:
                weaponsMenuImage.sprite = mp7Image;
                weaponsMenuText.text = "MP7\n----------------------------------------------------------------------\n" +
                    "Damage: 20\nRange: 37\nFire Rate: 0.09 Second\nMagazine Ammo: 60\nStarting Ammo: 240\nAmmo Type: 9mm";
                break;
            case 7:
                weaponsMenuImage.sprite = akImage;
                weaponsMenuText.text = "AK-47\n----------------------------------------------------------------------\n" +
                    "Damage: 40\nRange: 60\nFire Rate: 0.13 Second\nMagazine Ammo: 30\nStarting Ammo: 120\nAmmo Type: 7.62";
                break;
            case 8:
                weaponsMenuImage.sprite = m4Image;
                weaponsMenuText.text = "M4A4-S\n----------------------------------------------------------------------\n" +
                    "Damage: 30\nRange: 65\nFire Rate: 0.11 Second\nMagazine Ammo: 40\nStarting Ammo: 160\nAmmo Type: 5.56";
                break;
            case 9:
                weaponsMenuImage.sprite = awpImage;
                weaponsMenuText.text = "AWP\n----------------------------------------------------------------------\n" +
                    "Damage: 200\nRange: 400\nFire Rate: 2 Second\nMagazine Ammo: 10\nStarting Ammo: 50\nAmmo Type: 7.62";
                break;
            case 10:
                weaponsMenuImage.sprite = karambitImage;
                weaponsMenuText.text = "KarambitFade\n----------------------------------------------------------------------\n" +
                    "Upgraded Stats than Black One\nDamage Low: 60\nDamage High: 120\nWhile the Black ones stats are:\nDamage Low: 45\nDamage High: 60";
                break;
        }
    }

    public void OnPointerButtonsForUtilities(int index)
    {
        audioManager.Play("OnPointerSFX");
        switch (index)
        {
            case 1:
                utilitiesMenuText.text = "7.62 Ammo\n----------------------------------------------------------------------\n" +
                    "Amount: 30\n\nUsed By:\n-AK-47\n-AWP";
                utilitiesMenuImage.sprite = image762;
                break;
            case 2:
                utilitiesMenuText.text = "5.56 Ammo\n----------------------------------------------------------------------\n" +
                    "Amount: 40\n\nUsed By:\n-M4A4-S";
                utilitiesMenuImage.sprite = image556;
                break;
            case 3:
                utilitiesMenuText.text = "Shotgun Shell\n----------------------------------------------------------------------\n" +
                    "Amount: 20\n\nUsed By:\n-SPAS-12 Shotgun";
                utilitiesMenuImage.sprite = imageShell;
                break;
            case 4:
                utilitiesMenuText.text = "9mm Ammo\n----------------------------------------------------------------------\n" +
                    "Amount: 60\n\nUsed By:\n-Five Seven\n-MP7";
                utilitiesMenuImage.sprite = image9mm;
                break;
            case 5:
                utilitiesMenuText.text = ".40 Magnum Ammo\n----------------------------------------------------------------------\n" +
                    "Amount: 21\n\nUsed By:\n-Desert Eagle";
                utilitiesMenuImage.sprite = image40m;
                break;
            case 6:
                utilitiesMenuText.text = "Grenade\n----------------------------------------------------------------------\n" +
                    "Amount: 1\n\nSo powerful that when you throw it to zombie, zombie goes brrrrrrr";
                utilitiesMenuImage.sprite = imageGrenade;
                break;
            case 7:
                utilitiesMenuText.text = "Repair Generator\n----------------------------------------------------------------------\n" +
                    "\nRepairs all generators health by 30%\nExploded ones can not be repaired!!";
                utilitiesMenuImage.sprite = imageGenerator;
                break;
            case 8:
                utilitiesMenuText.text = "Medicine Kit\n----------------------------------------------------------------------\n" +
                    "\nHeals you by 40%";
                utilitiesMenuImage.sprite = imageMedkit;
                break;           
        }
    }

    //o silaha sahip olma durumuna göre veya paranýn yetme durumuna göre butonlarý kapatan kod
    public void CheckBuyAvailable()
    {
        if (gameControl.playerMoney < gameControl.deagleMoney || gameControl.doesHaveDeagle)
        {
            deagleButton.enabled = false;
            deagleButton.GetComponent<Image>().color = Color.grey;
        }
        if (gameControl.playerMoney < gameControl.shotgunMoney || gameControl.doesHaveShotgun)
        {
            shotgunButton.enabled = false;
            shotgunButton.GetComponent<Image>().color = Color.grey;
        }
        if (gameControl.playerMoney < gameControl.mp7Money || gameControl.doesHaveMp7)
        {
            mp7Button.enabled = false;
            mp7Button.GetComponent<Image>().color = Color.grey;
        }
        if (gameControl.playerMoney < gameControl.akMoney || gameControl.doesHaveAk)
        {
            akButton.enabled = false;
            akButton.GetComponent<Image>().color = Color.grey;
        }
        if (gameControl.playerMoney < gameControl.m4Money || gameControl.doesHaveM4)
        {
            m4Button.enabled = false;
            m4Button.GetComponent<Image>().color = Color.grey;
        }
        if (gameControl.playerMoney < gameControl.awpMoney || gameControl.doesHaveAwp)
        {
            awpButton.enabled = false;
            awpButton.GetComponent<Image>().color = Color.grey;
        }
        if (gameControl.playerMoney < gameControl.knifeMoney || gameControl.doesHaveKarambitFade)
        {
            karambitButton.enabled = false;
            karambitButton.GetComponent<Image>().color = Color.grey;
        }
        if (gameControl.playerMoney < gameControl.money762)
        {
            button762.enabled = false;
            button762.GetComponent<Image>().color = Color.grey;
        }
        if (gameControl.playerMoney < gameControl.money556)
        {
            button556.enabled = false;
            button556.GetComponent<Image>().color = Color.grey;
        }
        if (gameControl.playerMoney < gameControl.moneyShell)
        {
            buttonShell.enabled = false;
            buttonShell.GetComponent<Image>().color = Color.grey;
        }
        if (gameControl.playerMoney < gameControl.money9mm)
        {
            button9mm.enabled = false;
            button9mm.GetComponent<Image>().color = Color.grey;
        }
        if (gameControl.playerMoney < gameControl.money40m)
        {
            button40m.enabled = false;
            button40m.GetComponent<Image>().color = Color.grey;
        }
        if (gameControl.playerMoney < gameControl.moneyGrenade)
        {
            buttonGrenade.enabled = false;
            buttonGrenade.GetComponent<Image>().color = Color.grey;
        }
        if (gameControl.playerMoney < gameControl.moneyRepairGenorator)
        {
            buttonGenerator.enabled = false;
            buttonGenerator.GetComponent<Image>().color = Color.grey;
        }
        if (gameControl.playerMoney < gameControl.moneyHealth)
        {
            buttonMedKit.enabled = false;
            buttonMedKit.GetComponent<Image>().color = Color.grey;
        }
    }

    //kapatýlan butonlarý para kazandýktan sonra tekrar kullanabilmek için tüm hepsini eski haline resetleyen fonksiyon
    public void ResetBuyButtons()
    {
        deagleButton.enabled = true;
        deagleButton.GetComponent<Image>().color = new Color32(255, 162, 0, 255);
        shotgunButton.enabled = true;
        shotgunButton.GetComponent<Image>().color = new Color32(255, 162, 0, 255);
        mp7Button.enabled = true;
        mp7Button.GetComponent<Image>().color = new Color32(255, 162, 0, 255);
        akButton.enabled = true;
        akButton.GetComponent<Image>().color = new Color32(255, 162, 0, 255);
        m4Button.enabled = true;
        m4Button.GetComponent<Image>().color = new Color32(255, 162, 0, 255);
        awpButton.enabled = true;
        awpButton.GetComponent<Image>().color = new Color32(255, 162, 0, 255);
        karambitButton.enabled = true;
        karambitButton.GetComponent<Image>().color = new Color32(255, 162, 0, 255);
        button762.enabled = true;
        button762.GetComponent<Image>().color = new Color32(255, 162, 0, 255);
        button556.enabled = true;
        button556.GetComponent<Image>().color = new Color32(255, 162, 0, 255);
        buttonShell.enabled = true;
        buttonShell.GetComponent<Image>().color = new Color32(255, 162, 0, 255);
        button9mm.enabled = true;
        button9mm.GetComponent<Image>().color = new Color32(255, 162, 0, 255);
        button40m.enabled = true;
        button40m.GetComponent<Image>().color = new Color32(255, 162, 0, 255);
        buttonGrenade.enabled = true;
        buttonGrenade.GetComponent<Image>().color = new Color32(255, 162, 0, 255);
        buttonGenerator.enabled = true;
        buttonGenerator.GetComponent<Image>().color = new Color32(255, 162, 0, 255);
        buttonMedKit.enabled = true;
        buttonMedKit.GetComponent<Image>().color = new Color32(255, 162, 0, 255);
    }

}
