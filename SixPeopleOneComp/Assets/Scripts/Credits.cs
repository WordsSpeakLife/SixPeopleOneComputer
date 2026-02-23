using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI creditsText;
    public bool turnMeON = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        
            creditsText.transform.position += Vector3.up * 100f * Time.deltaTime;
        
        if (creditsText.transform.position.y >=6000)
        {
            SceneManager.LoadScene("startMenu");
        }
        //895
    }
}
