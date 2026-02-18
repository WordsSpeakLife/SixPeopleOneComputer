using System.Collections;
using UnityEngine;

public class ShieldWeakPoint : MonoBehaviour, IDamage
{

    [SerializeField] GameObject shield;
    [SerializeField] GameObject WeakpointEffect;
    [SerializeField] GameObject shieldEffect;
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
                SoundManager.instance.PlaySound3D("ShieldDown", transform.position);
                Instantiate(shieldEffect, shield.transform.position, Quaternion.identity);
                Destroy(shield);
                Camera.main.GetComponent<CameraShake>().StartCameraShake();
            }
            if (true)
            {
                SoundManager.instance.PlaySound3D("Explosions", transform.position);
                Renderer rend = shield.GetComponent<Renderer>();
                if (rend != null)
                {
                    Color color = rend.material.color;
                    color.a = weakPoints * 0.25f;
                    shield.GetComponent<Renderer>().material.color = color;
                }
                else
                {
                    Debug.LogError("No Renderer found on " + shield.name);
                }
                Destroy(gameObject);
                Instantiate(WeakpointEffect, transform.position, Quaternion.identity);
                Camera.main.GetComponent<CameraShake>().StartCameraShake();
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
