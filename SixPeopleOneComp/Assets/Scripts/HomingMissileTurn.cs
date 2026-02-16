using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissileTurn : MonoBehaviour
{
    public Transform target;
    [SerializeField] float turnSpeed;
    [SerializeField] float homedSpeed;
    Vector3 destination;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(target)
        {
            destination = target.position;
        }
        Vector3 delta = destination - transform.position;

        Vector3 forward = transform.forward;
        forward = Vector3.Slerp(forward, delta.normalized, turnSpeed * Time.deltaTime);
        transform.forward = forward;

        Vector3 movement = transform.forward;

        transform.position += movement * Time.deltaTime;
    }
}
