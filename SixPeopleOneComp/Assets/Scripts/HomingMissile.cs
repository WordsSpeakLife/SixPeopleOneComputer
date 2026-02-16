using System.Collections;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    private Transform Target;
    private GameObject[] targets;
    float distance;
    float closestTarget = Mathf.Infinity;
    float rotation;

    private void FixedUpdate()
    {
        //Vector3 direction = (Vector3)Target.position - rb.position;
        //direction.Normalize();
        //float rotateAmount = Vector3.Cross(direction, transform.up).z;
        //rb.angularVelocity = -rotation * rotateAmount;
    }
    public void FindEnemy()
    {
        targets = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (var enemy in targets)
        {
            distance = (enemy.transform.position - this.transform.position).sqrMagnitude;

            if (distance < closestTarget)
            {
                closestTarget = distance;
                Target = enemy.transform;
            }
        }

        if (Target != null)
        {
           Vector3 direction = (Vector3)Target.position - this.transform.position;
            direction.Normalize();
            float rotationValue = Vector3.Cross(direction, transform.up).z;
        }
    }


}
