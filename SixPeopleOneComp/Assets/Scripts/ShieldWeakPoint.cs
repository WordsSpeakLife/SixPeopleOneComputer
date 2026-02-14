using System.Collections;
using UnityEngine;

public class ShieldWeakPoint : MonoBehaviour, IDamage
{

    [SerializeField] GameObject shield;
    [SerializeField] Renderer model;
    [SerializeField] int HP;

    public static int weakPoints;

    Color colorOrig;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
        weakPoints = 4;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public bool heal(int amount) { return false; }

    public void takeDamage(int amount)
    {
        HP -= amount;
        if (HP <= 0)
        {
            weakPoints--;
            if (weakPoints <= 0)
            {
                Destroy(shield);
            }
            if (true)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            StartCoroutine(flashRed());
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

}
