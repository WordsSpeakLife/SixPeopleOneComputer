using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{

    [Header("References")]
    [SerializeField] CharacterController controller;

    [Header("Respawn Settings")]
    [SerializeField] float respawnYOffset = 1f;

    Vector3 lastCheckpoint;

    private void Awake()
    {
        if (!controller)
            controller = GetComponent<CharacterController>();

        lastCheckpoint = transform.position;

    }

    public void SetCheckpoint(Vector3 position)
    {
        lastCheckpoint = position;
    }

    public void TeleportToCheckpoint()
    {
        controller.enabled = false;

        transform.position = lastCheckpoint + Vector3.up * respawnYOffset;

        controller.enabled = true;
    }

}
