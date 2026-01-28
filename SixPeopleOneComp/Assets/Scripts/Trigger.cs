using Unity.VisualScripting;
using UnityEngine;

public class Trigger : MonoBehaviour
{

    [SerializeField] MeleeEnemyAI enemyScript;
    [SerializeField] ParticleSystem explosionEffect;
    [SerializeField] LayerMask damageableLayers;
    [SerializeField] float explosionDelay;
    [SerializeField] float explosionRadius;
    [SerializeField] int damage;

    bool triggered;
    float explosionTimer;

    private void Update()
    {
        if (triggered)
            explosionTimer += Time.deltaTime;
        if (explosionTimer >= explosionDelay)
        {
            explosion();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        triggered = true;
        enemyScript.flash();
    }

    public void explosion()
    {
        triggered = false;
        explosionTimer = 0;
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, damageableLayers);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject == enemyScript.gameObject) continue;

            IDamage dmg = collider.GetComponent<IDamage>();
            if (dmg != null)
            {
                dmg.takeDamage(damage);
            }
        }
        Instantiate(explosionEffect, transform.position, transform.rotation);
        enemyScript.die();
    }
}
