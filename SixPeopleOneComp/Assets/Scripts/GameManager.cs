using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
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

    [Header("---- Menus ----")]
    [SerializeField] public GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject menuAudio;
    [SerializeField] public GameObject weaponRadialMenu;
    [SerializeField] public GameObject LevelbuttonSelected;
    [SerializeField] public GameObject HealthBar;
    [SerializeField] public GameObject BossHealthBar;
    [SerializeField] TMP_Text keyCountText;
    public Image weaponIcon;
    public Image weaponIconFill;
    [SerializeField] public GameObject CurrentWeapon;
    [SerializeField] public GameObject EventSystem;

    [Header("---- Credits ----")]
    [SerializeField] TMP_Text creditsText;
    [SerializeField] TMP_Text creditsRequiredText;
    public int credits;

    [Header("---- Tutorial Popup ----")]
    public GameObject tutorialPopup;
    [SerializeField] TMP_Text tutorialText;

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
            Cursor.visible = true;
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
                if (menuActive == null || menuActive == weaponRadialMenu)
                {
                    statePause();
                    menuActive = menuPause;
                    menuActive.SetActive(true);
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
        menuActive = menuLose;
        menuActive.SetActive(true);
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
        menuActive = menuWin;
        menuActive.SetActive(true);

        SaveLevelProgress();
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
        audioMixer.SetFloat("SfxVolume", volume);
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
        credits += amount;
        UpdateCreditsUI();
    }

    void UpdateCreditsUI()
    {
        if (creditsText)
            creditsText.text = "Credits: " + credits;
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
        if (credits < amount) return false;

        credits -= amount;
        UpdateCreditsUI();
        return true;
    }

    public void SetCreditsRequiredUI(int amount)
    {
        if (creditsRequiredText)
            creditsRequiredText.text = "Credits Required: " + amount;
    }

    
}
