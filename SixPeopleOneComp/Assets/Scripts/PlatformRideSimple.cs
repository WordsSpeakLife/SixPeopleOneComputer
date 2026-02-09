using UnityEngine;

public class PlatformRideSimple : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        other.transform.SetParent(transform, true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        other.transform.SetParent(null, true);
    }
}

