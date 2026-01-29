using UnityEngine;

public class CreditDoorSimple : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] int creditsRequired = 50;
    [SerializeField] bool spendCredits = true;

    [Header("Door Move")]
    [SerializeField] Transform door;          
    [SerializeField] Vector3 openOffset = new Vector3(3f, 0f, 0f);
    [SerializeField] float moveSpeed = 12f;

    [Header("Blocking")]
    [SerializeField] Collider doorBlockCollider;

    Vector3 closedPos;
    Vector3 openPos;

    bool isOpening;
    bool isOpen;

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

        if (Vector3.Distance(door.position, openPos) <= 0.001f)
        {
            isOpening = false;
            isOpen = true;

            if (doorBlockCollider) doorBlockCollider.enabled = false;

            GameManager.instance.SetCreditsRequiredUI(0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameManager.instance.SetCreditsRequiredUI(creditsRequired);

        if (isOpen) return;

        if (GameManager.instance.HasCredits(creditsRequired))
        {
            if (spendCredits)
                GameManager.instance.SpendCredits(creditsRequired);

            isOpening = true;
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (!isOpening && !isOpen)
        GameManager.instance.SetCreditsRequiredUI(0);
    }
}   
