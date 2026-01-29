using UnityEngine;

public class CreditDoorSimple : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] int creditsRequired = 50;

    [Header("Door Move")]
    [SerializeField] Transform door;          
    [SerializeField] Vector3 openOffset = new Vector3(3f, 0f, 0f);
    [SerializeField] float moveSpeed = 12f;



    Vector3 closedPos;
    Vector3 openPos;
    bool isOpening;

    private void Start()
    {
        if (!door) door = transform;
        closedPos = door.position;
        openPos = closedPos + openOffset;

    }

    private void Update()
    {
        if (!isOpening) return;

        door.position = Vector3.MoveTowards(door.position, openPos, moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager.instance.SetCreditsRequiredUI(creditsRequired);

        if (GameManager.instance.credits >= creditsRequired)
        {
            isOpening = true;
        }
            
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager.instance.SetCreditsRequiredUI(0);
    }
}
