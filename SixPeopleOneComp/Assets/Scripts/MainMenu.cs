using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public enum Levels { None, Tutorial_Lvl, Level_1 }

    public void Play()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene("Tutorial_Lvl");
    }

    public void Play2()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene("Level_1");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
