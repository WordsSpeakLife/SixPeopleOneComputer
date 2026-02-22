using UnityEngine;

public class MenuParallax : MonoBehaviour
{
    public float offsetMultiplier = 1f;
    public float smoothTime = .3f;
    public float loc;

    private Vector2 startPosition;
    private Vector3 velocity;
    private void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        Vector2 offset = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        transform.position = Vector3.SmoothDamp(new Vector3(transform.position.x, transform.position.y, transform.position.z + loc), startPosition + (offset * offsetMultiplier), ref velocity, smoothTime);
    } 
}
