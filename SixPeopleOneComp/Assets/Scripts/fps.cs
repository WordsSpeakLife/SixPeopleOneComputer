using UnityEngine;
using TMPro;

public class fps : MonoBehaviour
{
    private TextMeshProUGUI textmesh;

    private void Awake()
    {
        textmesh = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        float fps = 1.0f / Time.unscaledDeltaTime;
        textmesh.text = "FPS: " + (int)fps;
    }
}
