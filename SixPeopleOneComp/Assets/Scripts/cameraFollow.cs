using UnityEngine;

public class cameraFollow : MonoBehaviour
{
    
    [Header("Follow Settings")]
    [SerializeField] Vector3 offset = new Vector3(0.0f, 14f, -14f);
    [SerializeField] float smoothSpeed = 10f;

    [Header("Rotation Settings")]
    [SerializeField] Vector3 cameraRotation = new Vector3(45f, 0f, 0f);
    [SerializeField] float rotationSmoothSpeed = 10f;


    Transform playerTarget;

    void Awake()
    {
        // Automatically find the player by tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject)
        {
            playerTarget = playerObject.transform;
        }
        else
        {
            Debug.LogError("cameraFollow: No GameObject with tag 'Player' found.");
        }
    }


   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void LateUpdate()
    {

        Vector3 desiredPosition = playerTarget.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime);

        // --- ROTATION ---
        Quaternion desiredRotation = Quaternion.Euler(cameraRotation);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,
            desiredRotation,
            rotationSmoothSpeed * Time.deltaTime);

    }
}
