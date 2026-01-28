using UnityEngine;

public class FallResetTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerRespawn respawn = other.GetComponent<PlayerRespawn>();
        if (!respawn) return;

        respawn.TeleportToCheckpoint();
    }
}
