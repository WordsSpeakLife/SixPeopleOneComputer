using System.Collections;
using UnityEngine;

public class DroppingPlatform : MonoBehaviour
{
    [SerializeField] Transform endPoint;
    [SerializeField] Transform Platform;
    [SerializeField] float speed;
    [SerializeField] float shakeDelay;
    [SerializeField] float destroyDelay;

    float step;
    bool shouldShake;
    float shakeDelayTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shouldShake = false;
        shakeDelayTimer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldShake)
        {
            StartCoroutine(ShakePlatform());
            step = speed * Time.deltaTime;
            shakeDelayTimer += Time.deltaTime;
        }

        if (shakeDelayTimer >= shakeDelay)
            Platform.position = Vector3.MoveTowards(Platform.position, new Vector3(Platform.position.x, endPoint.position.y, Platform.position.z), step);

        if (shakeDelayTimer >= destroyDelay)
            Destroy(this.gameObject);
    }

    public void dropPlatform()
    {
        shouldShake = true;
    }

    IEnumerator ShakePlatform()
    {
        Platform.position = new Vector3(Platform.position.x - 0.05f, Platform.position.y, Platform.position.z);
        yield return new WaitForSeconds(0.1f);
        Platform.position = new Vector3(Platform.position.x + 0.05f, Platform.position.y, Platform.position.z);
    }

}
