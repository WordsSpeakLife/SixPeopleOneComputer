using UnityEngine;

public class LightStrobing : MonoBehaviour
{

    [SerializeField] Light ptLight;
    [SerializeField] float minIntensity;
    [SerializeField] float maxIntensity;
    [SerializeField] float strobingSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (ptLight != null)
        {
            float pingPong = Mathf.PingPong(Time.time * strobingSpeed, maxIntensity - minIntensity);
            ptLight.intensity = minIntensity + pingPong;
        }
        
    }
}
