using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public enum Levels { None, Tutorial_Lvl, Level_1 }

    public void Play()
    {
        SceneManager.LoadScene("Tutorial_Lvl");
    }

    public void Play2()
    {
        SceneManager.LoadScene("Level_1");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
