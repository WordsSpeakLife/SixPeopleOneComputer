using UnityEngine;

public class cameraFollow : MonoBehaviour
{
    
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
        transform.position = Vector3.Lerp(transform.position, GameManager.instance.player.transform.position, smoothSpeed * Time.deltaTime);

    }
}
