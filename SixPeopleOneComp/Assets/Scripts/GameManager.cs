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

    public bool isPaused;
    public GameObject player;
    public PlayerController playerScript;
    public AudioMixer audioMixer;
    public Slider MusicSlider;
    public Slider SoundSlider;
    public Camera playerCamera;

    float timeScaleOrig;

    int gameGoalCount;
    float gameGoalTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;

        if (GameType != GameGoal.None)
        { 
            player = GameObject.FindWithTag("Player");
            playerScript = player.GetComponent<PlayerController>();
            playerCamera = Camera.main;
        }
    }

    // Update is called once per frame
    void Update()
    {
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
        isPaused = true;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void stateUnpauseMM()
    {
        isPaused = true;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        menuActive.SetActive(false);
        menuActive = null;
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

        audioMixer.GetFloat("SfxVolume", out float SoundVolume);
        PlayerPrefs.SetFloat("SfxVolume", SoundVolume);
    }

    public void LoadVolume()
    {
        MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        SoundSlider.value = PlayerPrefs.GetFloat("SfxVolume");
    }
}
