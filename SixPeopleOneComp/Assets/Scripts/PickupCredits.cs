using UnityEngine;

public class PickupCredits : MonoBehaviour
{
    [SerializeField] int creditsAmount = 10;
    [SerializeField] float spinSpeed = 180f;
    [SerializeField] float bobSpeed = 2f;
    [SerializeField] float bobHeight = 0.15f;

    Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.World);

        float bob = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = startPos + Vector3.up * bob;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.instance)
            GameManager.instance.AddCredits(creditsAmount);

        Destroy(gameObject);
    }

    public void SetAmount(int amount)
    {
        creditsAmount = amount;
    }
}
