using System.Collections;
using UnityEngine;

public class WallClimb : MonoBehaviour
{

    [SerializeField] float duration;

    [SerializeField] float maxClimbHeight;
    [SerializeField] CharacterController playercontroller;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playercontroller = GameManager.instance.player.GetComponent<CharacterController>();
    }
    void Update()
    {

    }
    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !playercontroller.isGrounded)
        {
            if (Physics.Raycast(playercontroller.transform.position, playercontroller.transform.forward, out RaycastHit climbHit, 0.6f))
            {
                playercontroller.GetComponent<PlayerController>().PlayerVelo.y = 0f;
                StartCoroutine(WallClimbCoroutine());
            }
        }

    }
    IEnumerator WallClimbCoroutine()
    {
        Vector3 startPos = playercontroller.transform.position;
        Vector3 endPos = startPos + Vector3.up * maxClimbHeight;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            playercontroller.Move(Vector3.Lerp(startPos, endPos, t) - playercontroller.transform.position);
            elapsed += Time.deltaTime;
            yield return null;
        }
        playercontroller.transform.position = endPos;
        playercontroller.Move(Vector3.forward * 0.3f);


    }
}
