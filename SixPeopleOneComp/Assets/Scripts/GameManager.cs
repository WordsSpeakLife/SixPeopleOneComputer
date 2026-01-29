using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public enum GameGoal { DefeatAllEnemies, ReachGoal, Timed, None }

    [Header("---- Game Controls ----")]
    [SerializeField] public GameGoal GameType;
    [SerializeField] float GoalTimerEnd;

    [Header("---- Menus ----")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject startMenu;
    [SerializeField] GameObject menuAudio;
    [SerializeField] public GameObject HealthBar;
    [SerializeField] TMP_Text keyCountText;
    public Sprite weaponIcon;
    [SerializeField] public GameObject CurrentWeapon;

    [Header("---- Credits ----")]
    [SerializeField] TMP_Text creditsText;
    [SerializeField] TMP_Text creditsRequiredText;
    public int credits;

    [Header("---- Tutorial Popup ----")]
    public GameObject tutorialPopup;
    [SerializeField] TMP_Text tutorialText;


    public bool isPaused;
    public GameObject player;
    public PlayerController playerScript;
    public AudioMixer audioMixer;
    public Slider MusicSlider;
    public Slider SfxSlider;
    public Camera playerCamera;
    float timeScaleOrig;
    

    int gameGoalCount;
    float gameGoalTimer;

    private int keyCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        MusicManager.instance.PlayMusic("game");
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
    }

    // Update is called once per frame
    void Update()
    {
        LoadVolume();
        UpdateMusicVolume(MusicSlider.value);
        UpdateSoundVolume(SfxSlider.value);
        SaveVolume();

        if (GameType == GameGoal.Timed)
        {
            gameGoalTimer += Time.deltaTime;
            if (gameGoalTimer >= GoalTimerEnd)
                updateGameGoal(0);
        }

        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
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
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void stateUnpauseMM()
    {
        isPaused = false;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        menuActive.SetActive(false);
        menuActive = null;
        MusicManager.instance.PlayMusic("menus");
    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void updateGameGoal(int amount)
    {
        if (GameType == GameGoal.DefeatAllEnemies)
        {
            gameGoalCount += amount;

            if (gameGoalCount <= 0)
            {
                //you win!!
                statePause();
                menuActive = menuWin;
                menuActive.SetActive(true);
            }
        }
        else if (GameType == GameGoal.ReachGoal)
        {
            gameGoalCount -= amount;

            if (gameGoalCount <= 0)
            {
                //you win!!
                statePause();
                menuActive = menuWin;
                menuActive.SetActive(true);
            }
        }
        else if (GameType == GameGoal.Timed)
        {
            gameGoalCount += amount;

            if (gameGoalTimer >= GoalTimerEnd)
            {
                //you win!!
                statePause();
                menuActive = menuWin;
                menuActive.SetActive(true);
            }
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

    public void SaveVolume()
    {
        audioMixer.GetFloat("MusicVolume", out float musicVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);

        audioMixer.GetFloat("SfxVolume", out float SfxVolume);
        PlayerPrefs.SetFloat("SfxVolume", SfxVolume);
    }

    public void LoadVolume()
    {
        MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        SfxSlider.value = PlayerPrefs.GetFloat("SfxVolume");
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
