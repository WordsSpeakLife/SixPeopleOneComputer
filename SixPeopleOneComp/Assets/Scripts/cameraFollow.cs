using UnityEngine;

public class cameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] Transform target;

    [Header("Follow Settings")]
    [SerializeField] Vector3 offset = new Vector3(0.0f, 14f, -14f);
    [SerializeField] float smoothSpeed = 10f;
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void LateUpdate()
    {
        if (!target) return;

        transform.position = Vector3.Lerp(transform.position, target.position, smoothSpeed * Time.deltaTime);

    }
}
