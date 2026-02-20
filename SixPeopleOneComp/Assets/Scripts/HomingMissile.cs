using System.Collections;
using UnityEngine;

public class HomingMissile : damage
{
    [SerializeField] float turn;
    private Transform Target;
    private Transform rocketTransform;

    private void FindEnemy()
    {

    }

    private void FixedUpdate()
    {
        if (!rb)
        {
            return;
        }

        rb.linearVelocity = transform.forward * speed;

        var rocketTargetRotation = Quaternion.LookRotation(Target.position - transform.position);

        rb.MoveRotation(Quaternion.RotateTowards(transform.rotation, rocketTargetRotation, turn));

        //Target = GameObject.FindGameObjectWithTag("Enemy").transform;

        //rb.angularVelocity = rocketTransform.forward * speed;

        //var targetRot = Quaternion.LookRotation(Target.position - rocketTransform.position);

        //rb.MoveRotation(Quaternion.RotateTowards(rocketTransform.rotation, targetRot, rotateSpeed));
    }
}
