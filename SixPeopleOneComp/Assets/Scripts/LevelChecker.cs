using UnityEngine;

public class LevelChecker : MonoBehaviour
{
    [SerializeField] GameObject[] allLevels;

    void Start()
    {
        int unlocked = PlayerPrefs.GetInt("LevelsUnlocked", 1);

        for (int i = 0; i < allLevels.Length; i++)
        {
            allLevels[i].SetActive(i < unlocked);
        }
    }
}

