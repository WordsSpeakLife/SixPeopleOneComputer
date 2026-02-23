using UnityEngine;

public class FlagPole : MonoBehaviour
{
    [SerializeField] int levelIndex;

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        if (other.CompareTag("Player"))
        {
            GameManager.instance.updateGameGoal(1);
        }
    }
}
