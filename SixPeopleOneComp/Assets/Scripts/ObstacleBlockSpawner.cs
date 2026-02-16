using UnityEngine;

public class ObstacleBlockSpawner : MonoBehaviour
{
    [Header("----- Spawn Points -----")]
    [SerializeField] Transform spawnPoint;
    [SerializeField] Transform cleanupPoint;        

    [Header("----- Block Settings -----")]
    [SerializeField] GameObject[] obstaclePrefabs;  
    [SerializeField] int obstaclesPerBlock = 3;
    [SerializeField] float spacingZ = 12f;          

    [Header("----- Runtime -----")]
    [SerializeField] bool spawnOnStart = true;

    private int blockIndex = 0;

    private void Start()
    {
        if (!spawnPoint) spawnPoint = transform;

        if (spawnOnStart)
            SpawnBlock();
    }

    public void SpawnBlock()
    {
        if (obstaclePrefabs == null || obstaclePrefabs.Length == 0) return;

        Vector3 basePos = spawnPoint.position;

        for (int i = 0; i < obstaclesPerBlock; i++)
        {
            GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];

            Vector3 pos = basePos + new Vector3(0f, 0f, (blockIndex * obstaclesPerBlock + i) * spacingZ);

            Instantiate(prefab, pos, prefab.transform.rotation);
        }

        blockIndex++;
    }
}

