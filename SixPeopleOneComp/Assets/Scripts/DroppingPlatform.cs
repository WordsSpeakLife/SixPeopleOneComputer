using System.Collections;
using UnityEngine;

public class DroppingPlatform : MonoBehaviour
{
    [SerializeField] Renderer model;
    [SerializeField] Transform endPoint;
    [SerializeField] Transform Platform;
    [SerializeField] float speed;
    [SerializeField] float shakeDelay;
    [SerializeField] float destroyDelay;

    Color colorOrig;

    float step;
    bool shouldShake;
    float shakeDelayTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
        shouldShake = false;
        shakeDelayTimer = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (shouldShake)
        {
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
        StartCoroutine(ShakePlatform());
    }

    IEnumerator ShakePlatform()
    {
        //Platform.position = new Vector3(Platform.position.x - 0.05f, Platform.position.y, Platform.position.z);
        //yield return new WaitForSeconds(0.1f);
        //Platform.position = new Vector3(Platform.position.x + 0.05f, Platform.position.y, Platform.position.z);
        while (true)
        {

            yield return new WaitForSeconds(0.4f);
            model.material.color = Color.red;
            yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
            model.material.color = colorOrig;
        }
    }

}
