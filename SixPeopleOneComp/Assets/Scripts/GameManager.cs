using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public enum GameGoal { DefeatAllEnemies, ReachGoal, Timed, None }

    [Header("---- Game Controls ----")]
    [SerializeField] public GameGoal GameType;
    [SerializeField] float GoalTimerEnd;
    [SerializeField] bool isMainMenu;
    [SerializeField] CreditDoorSimple door;

    [Header("---- Menus ----")]
    [SerializeField] public GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject menuAudio;
    [SerializeField] public GameObject weaponRadialMenu;
    [SerializeField] public GameObject LevelbuttonSelected;
    [SerializeField] public Image HealthBar;
    Animation anim;

    [SerializeField] public GameObject BossHealthBar;
    [SerializeField] TMP_Text keyCountText;
    public Image weaponIcon;
    public Image weaponIconFill;
    [SerializeField] public GameObject CurrentWeapon;
    [SerializeField] public GameObject EventSystem;

    [Header("---- Credits ----")]
    [SerializeField] public TMP_Text creditsText;
    [SerializeField] TMP_Text creditsRequiredText;
    public int credits;
    int creditsRequired;
    bool isCounting = false;
    bool bitChangeType;

    [Header("---- Tutorial Popup ----")]
    public GameObject tutorialPopup;
    [SerializeField] TMP_Text tutorialText;
    public Image tutorialTimer;

    [Header("---- Level Data ----")]
    [Tooltip("Starts at 1, add 1 per level (ex: this is level 3 so it would be 1+1+1+1+1 so 5")]
    [SerializeField] int LevelNumber = 1;

    [Tooltip("Leave blank if not in use")]
    [SerializeField] public string NextLevelName;


    //[Header("---- Save Data ----")]
    //[SerializeField] public SaveData levels;


    [Header("---- Other ----")]
    public bool isPaused;
    public GameObject player;
    public KeyBinds keyBinds;  
    public PlayerController playerScript;
    public AudioMixer audioMixer;
    public Slider MusicSlider;
    public Slider SfxSlider;
    public Slider VolumeSlider;
    public Camera playerCamera;
    float timeScaleOrig;
    public GameObject DamageFlash;
    public Image dmgIndLeft;
    public RectTransform leftPos;
    public Image dmgIndRight;
    public RectTransform rightPos;

    public float damageReceived = 0;
    public float damageDone = 0;
    public float enemysKilled = 0;

    GameObject killCount,
    dmgDoneCount, dmgRecCount;

    [SerializeField]
    GameObject winKill, winDone, winRec;
    [SerializeField] GameObject loseKill, loseDone, loseRec;

    int gameGoalCount;
    float gameGoalTimer;


    private int keyCount;
    const string LEVELS_UNLOCKED_KEY = "LevelsUnlocked";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        HideTutorial();
        UpdateCreditsUI();
        SetCreditsRequiredUI(0);
        timeScaleOrig = Time.timeScale;
        if (GameType != GameGoal.None)
        {
            player = GameObject.FindWithTag("Player");
            playerScript = player.GetComponent<PlayerController>();
            playerCamera = Camera.main;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
        }

        if (!PlayerPrefs.HasKey(LEVELS_UNLOCKED_KEY))
        {
            PlayerPrefs.SetInt(LEVELS_UNLOCKED_KEY, 1);
            PlayerPrefs.Save();
        }
    }

    private void Start()
    {
        //LoadGame();
        creditsRequired = door.creditsRequired;
        SetCreditsRequiredUI(creditsRequired);

    }

    // Update is called once per frame
    void Update()
    {

        // LoadVolume();
        // UpdateMusicVolume(MusicSlider.value);
        // UpdateSoundVolume(SfxSlider.value);
        // SaveVolume();

        if (GameType == GameGoal.Timed)
        {
            gameGoalTimer += Time.deltaTime;
            if (gameGoalTimer >= GoalTimerEnd)
                updateGameGoal(0);
        }
        if (!isMainMenu)
        {
            if (Input.GetButtonDown("Cancel"))
            {
                if (menuActive == null)
                {
                    statePause();
                    menuActive = menuPause;
                    anim = menuActive.GetComponent<Animation>();
                    menuActive.SetActive(true);
                    //popOut();
                }
                else if (menuActive == menuPause)
                {
                    stateUnpause();
                }
            }
            //if (Input.GetButtonDown("Weapon Menu"))
            //{
            //    if (menuActive == null)
            //    {
            //        weaponRadial();
            //    }
            //}
            //else if(Input.GetButtonUp("Weapon Menu") && menuActive == weaponRadialMenu)
            //{
            //    stateUnpause();
            //}
        }
    }

    public void statePause()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;  
    }

    public void stateUnpause()
    {
        isPaused = false;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        if (menuActive != null)
        {
            anim = menuActive.GetComponent<Animation>();
            //popIn();
            menuActive.SetActive(false);
            menuActive = null;
        }
    }

    public void stateUnpauseMM()
    {
        isPaused = false;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (menuActive != null)
        {
            menuActive.SetActive(false);
            menuActive = null;
        }
    }

    public void youLose()
    {
        statePause();
        dmgRecCount = loseRec;
        dmgDoneCount = loseDone;
        killCount = loseKill;
        setReceived();
        setDone();
        setKilled();
        menuActive = menuLose;
        menuActive.SetActive(true);
        anim = menuActive.GetComponent<Animation>();
        //popOut();
    }

    //public void weaponRadial()
    //{
    //    isPaused = false;
    //    Time.timeScale = timeScaleOrig / 2;
    //    Cursor.visible = true;
    //    Cursor.lockState = CursorLockMode.Confined;
    //    menuActive = weaponRadialMenu;
    //    menuActive.SetActive(true);
    //    weaponRadialMenu.GetComponent<RadialMenu>().Open();
    //}

    public void updateGameGoal(int amount)
    {
        if (GameType == GameGoal.DefeatAllEnemies)
        {
            gameGoalCount += amount;

            if (gameGoalCount <= 0) OnWin();
            
        }
        else if (GameType == GameGoal.ReachGoal)
        {
            gameGoalCount -= amount;

            if (gameGoalCount <= 0) OnWin();

        }
        else if (GameType == GameGoal.Timed)
        {
            gameGoalCount -= amount;

            if (gameGoalTimer >= GoalTimerEnd) OnWin();
        }
    }

    void OnWin()
    {
        statePause();
        dmgRecCount = winRec;
        dmgDoneCount = winDone;
        killCount = winKill;
        setReceived();
        setDone();
        setKilled();
        menuActive = menuWin;
        menuActive.SetActive(true);
        //anim = menuActive.GetComponent<Animation>();
        //popOut();
        SaveLevelProgress();
    }

    void setReceived()
    {
        dmgRecCount.GetComponent<TMP_Text>().text = "Damage Received: " + damageReceived;
    }

    void setDone()
    {
        dmgDoneCount.GetComponent<TMP_Text>().text = "Damage Done: " + damageDone;
    }

    void setKilled()
    {
        killCount.GetComponent<TMP_Text>().text = "Enemies deleted: " + enemysKilled;
    }

    void SaveLevelProgress()
    {
        int currentlyUnlocked = PlayerPrefs.GetInt(LEVELS_UNLOCKED_KEY, 1);
        int shouldBeUnlocked = Mathf.Max(currentlyUnlocked, LevelNumber + 1);

        PlayerPrefs.SetInt(LEVELS_UNLOCKED_KEY, shouldBeUnlocked);
        PlayerPrefs.Save();

        Debug.Log("Saved LevelsUnlocked = " + shouldBeUnlocked);
    }

    public void CompleteLevel(int levelIndex)
    {
        int unlocked = PlayerPrefs.GetInt(LEVELS_UNLOCKED_KEY, 1);

        if (levelIndex + 1 > unlocked)
        {
            PlayerPrefs.SetInt(LEVELS_UNLOCKED_KEY, levelIndex + 1);
            PlayerPrefs.Save();

            Debug.Log("Unlocked level " + (levelIndex + 1));
        }

        LoadNextLevel();
    }
    public void LoadNextLevel()
    {
        stateUnpauseMM();

        if (!string.IsNullOrEmpty(NextLevelName))
        {
            SceneManager.LoadScene(NextLevelName);
                return;
        }
    }


    public void UpdateMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
    }

    public void UpdateSoundVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
    }
    public void UpdateMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", volume);
    }

    public void SaveVolume()
    {
        audioMixer.GetFloat("MusicVolume", out float musicVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);

        audioMixer.GetFloat("MasterVolume", out float MasterVolume);
        PlayerPrefs.SetFloat("MasterVolume", MasterVolume);

        audioMixer.GetFloat("SfxVolume", out float SfxVolume);
        PlayerPrefs.SetFloat("SfxVolume", SfxVolume);
    }

    public void LoadVolume()
    {
        MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        SfxSlider.value = PlayerPrefs.GetFloat("SfxVolume");
        VolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume");
    }

    public void AddCredits(int amount)
    {
        bitChangeType = true;
        int beforeAmount = credits;
        credits += amount;
        if(isCounting == false)
        StartCoroutine(CountToBits(beforeAmount, credits));
    }

    IEnumerator CountToBits(int current, int target)
    {
        Vector2 sizeOrig = creditsText.rectTransform.localScale;
        Color origColor = creditsText.color;
        float frac=0;
        float t = 0;
        while (frac < 1)
        {
            t += Time.deltaTime;
            frac = t / 10;
        }

        isCounting = true;
        if(bitChangeType == true)
        {
            while (current < target)
            {
                Vector2 sizeChange1 = new Vector2(creditsText.rectTransform.localScale.x + .05f, creditsText.rectTransform.localScale.y + .05f);
                Vector2 sizeChange2 = new Vector2(creditsText.rectTransform.localScale.x + .1f, creditsText.rectTransform.localScale.y + .1f);
                current++;

                creditsText.text = "x" + current;
                creditsText.rectTransform.localScale = sizeChange2;
                creditsText.color = Color.Lerp(creditsText.color, Color.yellow, 0.1f);
                yield return new WaitForSeconds(.05f);
                creditsText.rectTransform.localScale = sizeChange1;
                sizeChange1 = new Vector2(creditsText.rectTransform.localScale.x + .05f, creditsText.rectTransform.localScale.y + .05f);
                sizeChange2 = new Vector2(creditsText.rectTransform.localScale.x + .1f, creditsText.rectTransform.localScale.y + .1f);
                yield return new WaitForSeconds(.1f);
            }
        }
        else if (bitChangeType == false)
        {
            while (current > target)
            {
                Vector2 sizeChange1 = new Vector2(creditsText.rectTransform.localScale.x - .025f, creditsText.rectTransform.localScale.y - .025f);
                Vector2 sizeChange2 = new Vector2(creditsText.rectTransform.localScale.x - .0125f, creditsText.rectTransform.localScale.y - .0125f);
                current--;

                creditsText.text = "x" + current;
                creditsText.rectTransform.localScale = sizeChange2;
                creditsText.color = Color.Lerp(creditsText.color, Color.magenta, 0.1f);
                yield return new WaitForSeconds(.05f);
                creditsText.rectTransform.localScale = sizeChange1;
                yield return new WaitForSeconds(.1f);
            }
        }
        creditsText.rectTransform.localScale = Vector2.Lerp(creditsText.rectTransform.localScale, sizeOrig, frac);
        creditsText.color = origColor;
        isCounting = false;

        UpdateCreditsUI();

        yield return null;
    }

    void UpdateCreditsUI()
    {
        if (creditsText)
        {
            creditsText.text = "x" + credits;
        }
    }


    public void ShowTutorial(string message)
    {
        if (!tutorialPopup || !tutorialText) return;

        

        tutorialText.text = message;
        tutorialPopup.SetActive(true);
    }

    public void HideTutorial()
    {
        if (!tutorialPopup) return;
        tutorialPopup.SetActive(false);
    }

    public bool HasCredits(int amount)
    {
        return credits >= amount;
    }

    public bool SpendCredits(int amount)
    {
        bitChangeType = false;
        int beforeAmount = credits;

        if (credits < amount) return false;

        credits -= amount;

        if (!isCounting)
        StartCoroutine(CountToBits(beforeAmount, credits));

        return true;
    }

    public void SetCreditsRequiredUI(int amount)
    {
        if (creditsRequiredText)
        {
            if (door)
            {
                if (GameType == GameGoal.ReachGoal)
                {
                    if (door.isOpen != true)
                    {
                        if (amount > 0)
                        {
                            creditsRequiredText.text = "Collect " + amount + " more bits to progress!";
                        }
                        else if (amount <= 0)
                        {
                            creditsRequiredText.text = "Get to the door!";
                        }
                    }
                    else
                    {
                        creditsRequiredText.text = "Progress!";
                    }
                }
            }
            else if (!door && GameType == GameGoal.DefeatAllEnemies)
            {
                creditsRequiredText.text = "Defeat the enemies!";
            }
            else
            {
                creditsRequiredText.text = "Progress!";
            }
        }
    }
    
    public void SaveCredits()
    {
        PlayerPrefs.SetInt("Credits", credits);
        PlayerPrefs.Save();
    }

    public void LoadCredits()
    {
        if (PlayerPrefs.HasKey("Credits"))
        {
            credits = PlayerPrefs.GetInt("Credits");
            UpdateCreditsUI();
        }
    }

    public void creditCheck()
    {
        if (credits <= door.creditsRequired)
        {
            creditsRequired = door.creditsRequired - credits;
            SetCreditsRequiredUI(creditsRequired);
        }
        else if (credits > door.creditsRequired)
        {
            creditsRequired = door.creditsRequired - credits;
            SetCreditsRequiredUI(creditsRequired);
        }
    }

    public void SaveGame()
    {
        Vector3 pos = player.transform.position;

        PlayerPrefs.SetFloat("PlayerX", pos.x);
        PlayerPrefs.SetFloat("PlayerY", pos.y);
        PlayerPrefs.SetFloat("PlayerZ", pos.z);

        PlayerPrefs.SetInt("PlayerHP", playerScript.GetHP());
        PlayerPrefs.SetInt("Credits", credits);

        PlayerPrefs.Save();

        Debug.Log("Game Saved");
    }

    public void LoadGame()
    {
        if (!PlayerPrefs.HasKey("PlayerX"))
        {
            Debug.Log("No Save Found");
            return;
        }
        Vector3 pos = new Vector3
        (PlayerPrefs.GetFloat("PlayerX"),
        PlayerPrefs.GetFloat("PlayerY"),
        PlayerPrefs.GetFloat("PlayerZ"));

        playerScript.TeleportTo(pos);

        playerScript.SetHP(PlayerPrefs.GetInt("PlayerHP", playerScript.GetHP()));
        credits = PlayerPrefs.GetInt("Credits", credits);
        UpdateCreditsUI();


        Debug.Log("Game Loaded");
        stateUnpause();
    }

    public void popIn()
    {
        anim.Play("UIpopin");
    }
    public void popOut()
    {
        anim.Play("UIpopout");
    }
}
