using System;
using System.Collections;
using UnityEngine;

public class PlatformManager : MonoBehaviour
{

    [SerializeField] DroppingPlatform platform;

    [System.Serializable]
    public class DropingPlatArrays
    {
        public DroppingPlatform[] array;
    }
    public DropingPlatArrays[] PlatformWaves;

    bool dropPlatforms;
    float dropTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dropPlatforms = false;
        dropTimer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void startDroppingPlats()
    {
        StartCoroutine(delayDrop());
    }

    IEnumerator delayDrop()
    {
        foreach (DropingPlatArrays wave in PlatformWaves)
        {
            yield return new WaitForSeconds(2f);
            foreach (DroppingPlatform plat in wave.array)
            {
                plat.dropPlatform();
            }
        }
    }

}
