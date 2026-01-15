using UnityEngine;

public class Health_PickUp : MonoBehaviour
{
    [SerializeField] int healAmount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        if (other.CompareTag("Player"))
        {
            IDamage dmg = other.GetComponent<IDamage>();

            if ((dmg != null))
            {
                if (dmg.heal(healAmount) == true)
                     Destroy(gameObject);
            }
        }
    }

}
