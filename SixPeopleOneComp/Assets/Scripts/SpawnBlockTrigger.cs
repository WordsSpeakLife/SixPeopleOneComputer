using UnityEngine;

public class SpawnBlockTrigger : MonoBehaviour
{
    [SerializeField] ObstacleBlockSpawner spawner;
    [SerializeField] bool destroyAfterUse = true;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!spawner) return;

        spawner.SpawnBlock();

        if (destroyAfterUse)
            Destroy(gameObject);
    }
}

