using UnityEngine;

public class GroundCollisionChecker : MonoBehaviour
{
    [SerializeField] Collider groundCheckCollider;
    PlayerController playerController;
    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    // Update is called once per frame
    void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("ground"))
        {
            playerController.groundCollision = true;
        }
        else
        {
            playerController.groundCollision = false;
        }
    }
    void OnCollisionExit(Collision collision)
    {
        playerController.groundCollision = false;
    }
}
