using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{

    public void Play(string level)
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        GameManager.instance.stateUnpause();
        SceneManager.LoadScene(level); 
    }

    public void resetSave()
    {
        GameManager.instance.levels.levelsUnlocked = 0;
    }

    public void MainMenu()
    {
        GameManager.instance.stateUnpauseMM();
        SceneManager.LoadScene("startMenu");
    }

    public void resume()
    {
        GameManager.instance.stateUnpause();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.instance.stateUnpause();
    }

    public void nextLevel()
    {
        if (GameManager.instance.NextLevelName == "") return;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        GameManager.instance.stateUnpause();
        SceneManager.LoadScene(GameManager.instance.NextLevelName);
    }

    public void quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }

}
