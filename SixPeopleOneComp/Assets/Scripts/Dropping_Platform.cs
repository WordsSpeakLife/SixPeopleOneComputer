using System.Collections;
using UnityEngine;

public class Dropping_Platform : MonoBehaviour
{

    [SerializeField] GameObject platform;
    private void OnTriggerEnter(Collider other)
    {

        platform.GetComponent<DroppingPlatform>().dropPlatform();


    }
}
