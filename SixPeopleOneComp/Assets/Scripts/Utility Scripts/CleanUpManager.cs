using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CleanUpManager : MonoBehaviour
{

    public static CleanUpManager instance;

    void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RemoveClonedObjects()
    {
        List<GameObject> rootObjects = new List<GameObject>();
        UnityEngine.SceneManagement.Scene activeScene = SceneManager.GetActiveScene();
        activeScene.GetRootGameObjects(rootObjects);

        foreach (GameObject obj in rootObjects)
        {
            if (obj.name.Contains("(Clone)"))
            {
                Destroy(obj);
            }
        }
    }

}
