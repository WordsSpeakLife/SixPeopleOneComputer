using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelChecker : MonoBehaviour
{

    [SerializeField] GameObject[] allLevels;
    [SerializeField] SaveData levels;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void checkLevels()
    {
        for (int i = 0; i < levels.levelsUnlocked; i++)
        {
            if(i <= allLevels.Length)
                allLevels[i].SetActive(true);
        }
    }

    public void resetLevels()
    {
        foreach (GameObject level in allLevels)
        {
                level.SetActive(false);
        }
    }

}
