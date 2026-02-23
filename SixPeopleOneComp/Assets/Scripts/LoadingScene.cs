using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] GameObject LoadingScreen;
    [SerializeField] Slider LoadingSlider;

    [SerializeField] float loadSpeed = 0.6f;

    public GameManager GameManager;

    bool isLoadingDone = false;
    float timeScaleOrig;

    void Start()
    {
        GameManager.statePause();
        LoadingScreen.SetActive(true);
        Application.targetFrameRate = 999;
        LoadingSlider.value = 0f;

        StartCoroutine(routine: FakeLoading());
    }

    IEnumerator FakeLoading()
    {
        while(LoadingSlider.value < 1f)
        {
            LoadingSlider.value += loadSpeed * Time.unscaledDeltaTime;
            yield return null;
        }

        isLoadingDone = true;
        LoadingScreen.SetActive(false);
        GameManager.stateUnpause();
    }
}
