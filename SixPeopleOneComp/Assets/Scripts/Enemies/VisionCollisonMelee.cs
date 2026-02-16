using UnityEngine;
using UnityEngine.AI;

public class VisionCollisonMelee : MonoBehaviour
{
    [SerializeField] MeleeEnemyAI enemy;
    [SerializeField] NavMeshAgent agent;

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
            agent.stoppingDistance = 0;
        }
    }

}
