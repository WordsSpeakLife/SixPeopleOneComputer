using UnityEngine;

public class Timer : MonoBehaviour
{

    public float elapSeconds {  get; private set; }
    public bool running { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetTimer();
        StartTimer();
    }

    // Update is called once per frame
    void Update()
    {
        if (!running) return;
        elapSeconds += Time.deltaTime;
    }

    public void StartTimer()
    {
        running = true;
    }

    public void StopTimer()
    {
        running = false; 
    }

    public void ResetTimer()
    {
        elapSeconds = 0f;
        running = false;
    }

    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(elapSeconds / 60);
        int seconds = Mathf.FloorToInt(elapSeconds % 60);
        int miliseconds = Mathf.FloorToInt((elapSeconds * 1000f) % 1000f);

        return string.Format("{0:00}:{1:00}.{2:000}", minutes, seconds, miliseconds);
    }
}
