using UnityEngine;
using UnityEngine.AI;

public class VisionCollisonTurrent : MonoBehaviour
{
    [SerializeField] TurretEnemyAI enemy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            enemy.playerInTrigger = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemy.playerInTrigger = false;
        }
    }

}
