using UnityEngine;

public class CreditDoorSimple : MonoBehaviour
{
    [SerializeField] Transform door;
    [SerializeField] int cost = 25;
    [SerializeField] Vector3 openOffset;
    [SerializeField] bool consumeCredits = true;

    Vector3 closedPos;
    bool opened;

    private void Start()
    {
        if (!door) door = transform;
        closedPos = door.position;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (opened) return;

        bool canOpen = consumeCredits ? GameManager.instance.SpendCredits(cost) : GameManager.instance.HasCredits(cost);

        if (!canOpen) return;

        door.position += openOffset;
        opened = true;
            
    }
}
