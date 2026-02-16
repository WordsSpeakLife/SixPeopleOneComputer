using UnityEngine;

public class BreakableObstacle : MonoBehaviour, IDamage
{
    [SerializeField] int HP = 10;

    [Header("Optional")]
    [SerializeField] GameObject destroyRoot;   

    private void Awake()
    {
        if (!destroyRoot) destroyRoot = gameObject;
    }

    public void takeDamage(int _amount)
    {
        HP -= _amount;

        if (HP <= 0)
        {
            Destroy(destroyRoot);
        }
    }

    public bool heal(int _amount)
    {
        HP += _amount;
        return true;
    }

    
}


