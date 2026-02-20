using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public float duration = 1f;
    [SerializeField] AnimationCurve curve;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartCameraShake (float duration)
    {
        this.duration = duration;
        StartCoroutine(Shake());
    }

    public void StartCameraShake()
    {
        StartCoroutine(Shake());
    }

    IEnumerator Shake()
    {
        Vector3 startPos = transform.localPosition;
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime / duration);
            transform.localPosition = startPos + Random.insideUnitSphere * strength;
            yield return null;
        }

        transform.localPosition = startPos;
        duration = 1f;
    }

}
