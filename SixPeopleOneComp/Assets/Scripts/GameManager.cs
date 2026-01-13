using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuAudio;

    public bool isPaused;
    public GameObject player;
    public PlayerController playerScript;
    public AudioMixer audioMixer;
    public Slider MusicSlider;
    public Slider SoundSlider;

    float timeScaleOrig;

    int gameGoalCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
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

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void updateGameGoal(int amount)
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

    public void UpdateMusicVolume(float volume)
    {
        audioMixer.SetFloat("Music", volume);
    }

    public void UpdateSoundVolume(float volume)
    {
        audioMixer.SetFloat("Sfx", volume);
    }

    public void SaveVolume()
    {
        audioMixer.GetFloat("Music", out float musicVolume);
        PlayerPrefs.SetFloat("Music", musicVolume);

        audioMixer.GetFloat("Sfx", out float SoundVolume);
        PlayerPrefs.SetFloat("Sfx", SoundVolume);
    }

    public void LoadVolume()
    {
        MusicSlider.value = PlayerPrefs.GetFloat("Music");
        SoundSlider.value = PlayerPrefs.GetFloat("Sfx");
    }
}
