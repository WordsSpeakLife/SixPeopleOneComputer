using UnityEngine;
using System.Collections;

public class TutorialPopup : MonoBehaviour
{
    [TextArea(2, 6)]
    [SerializeField] string message;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ShowMessage());
        }
    }

    IEnumerator ShowMessage()
    {
        GameManager.instance.ShowTutorial(message);
        yield return new WaitForSeconds(8);
        GameManager.instance.HideTutorial();
    }
}
