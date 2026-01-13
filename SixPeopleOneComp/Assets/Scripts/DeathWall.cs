using UnityEngine;

public class DeathWall : MonoBehaviour
{

    [SerializeField] Vector3 moveDir;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += moveDir * Time.deltaTime;
    }
}
