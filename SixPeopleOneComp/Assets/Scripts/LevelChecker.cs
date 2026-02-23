using UnityEngine;

public class LevelChecker : MonoBehaviour
{
    [SerializeField] GameObject[] allLevels;

    void Start()
    {
        int unlocked = 6;

        for (int i = 0; i < allLevels.Length; i++)
        {
            allLevels[i].SetActive(i < unlocked);
        }
    }
}

