using UnityEngine;
using System.Collections;

public class damage : MonoBehaviour
{
    enum damageType { moving, stationary, DOT }
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;
    [SerializeField] Collider col;
   [SerializeField]  Renderer rend;

    [SerializeField] int damageAmount;
    [SerializeField] float damageRate;
    [SerializeField] int speed;
    [SerializeField] float destroyTime;
    [SerializeField] GameObject hitEffect;
    [SerializeField]  Color startColor = Color.black;
    [SerializeField]  Color endColor = Color.red;
    public float duration = 2.0f;

    bool isDamaging;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (type == damageType.moving)
        {
            rb.linearVelocity = transform.forward * speed;

            Destroy(gameObject, destroyTime);


        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        if (type == damageType.stationary)
        {
            StartCoroutine(ChangeColor(startColor, endColor, destroyTime));
            Destroy(gameObject, destroyTime);
        }

        IDamage dmg = other.GetComponent<IDamage>();
        if (other.CompareTag("Shoot_Obj"))
        {
            Destroy(other.gameObject);
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }

        if ((dmg != null) && type != damageType.DOT)
        {
            dmg.takeDamage(damageAmount);
            Instantiate(hitEffect, transform.position, Quaternion.identity);
            SoundManager.instance.PlaySound3D("Damage", transform.position);
        }
        if (type == damageType.moving)
        {
            Destroy(gameObject);
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }



    }

    private void OnTriggerStay(Collider other)
    {
        //  Debug.Log("hit player");
        if (other.isTrigger) return;

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null && type == damageType.DOT && !isDamaging)
        {
            StartCoroutine(damageOther(dmg));
        }
    }

    IEnumerator damageOther(IDamage d)
    {
        isDamaging = true;
        d.takeDamage(damageAmount);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("hit player");
        IDamage dmg = collision.gameObject.GetComponent<IDamage>();

        StartCoroutine(damageOther(dmg));

    }
    private IEnumerator ChangeColor(Color currentStartColor, Color currentEndColor, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            Color lerpColor = Color.Lerp(currentStartColor, currentEndColor, t);
            rend.material.color = lerpColor;
            yield return null;
        }
        rend.material.color = currentEndColor;
    }

}
