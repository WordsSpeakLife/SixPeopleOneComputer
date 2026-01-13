using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene("Tutorial_Lvl");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
