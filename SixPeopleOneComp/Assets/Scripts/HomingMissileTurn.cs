using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissileTurn : MonoBehaviour
{
    public Transform target;

    [SerializeField] float turnSpeed;
    [SerializeField] float snapSpeed;
    Vector3 destination;


    // Update is called once per frame
    void Update()
    {
        if(target)
        {
            destination = target.position;
        }
        Vector3 delta = destination - transform.position;

        Vector3 forward = transform.forward;
        forward = Vector3.Slerp(forward, delta.normalized, snapSpeed * Time.deltaTime);
        transform.forward = forward;

        Vector3 movement = transform.forward;
        movement *= turnSpeed;
        transform.position += movement * Time.deltaTime;
    }
}
