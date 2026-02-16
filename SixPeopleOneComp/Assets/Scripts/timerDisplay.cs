using UnityEngine;
using TMPro;

public class timerDisplay : MonoBehaviour
{
    public Timer timer;
    public TextMeshProUGUI timerText;

    // Update is called once per frame
    void Update()
    {
        if (timer == null || timerText == null) return;
        timerText.text = "Time: " + timer.GetFormattedTime();
    }
}
