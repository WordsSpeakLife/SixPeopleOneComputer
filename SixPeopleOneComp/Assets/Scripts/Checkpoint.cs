using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] string checkpointID;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;


        Debug.Log("Checkpoint Saved " + checkpointID);
    }
}
