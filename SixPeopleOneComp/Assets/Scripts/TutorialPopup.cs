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
        Vector2 size1 = GameManager.instance.tutorialPopup.GetComponent<RectTransform>().localScale;

        GameManager.instance.tutorialPopup.GetComponent<RectTransform>().localScale = new Vector2(0, 0);

        GameManager.instance.ShowTutorial(message);
        GameManager.instance.tutorialPopup.GetComponent<RectTransform>().localScale = Vector2.MoveTowards(GameManager.instance.tutorialPopup.GetComponent<RectTransform>().localScale, size1, 100);
        yield return new WaitForSeconds(8);
        GameManager.instance.tutorialPopup.GetComponent<RectTransform>().localScale = Vector2.MoveTowards(GameManager.instance.tutorialPopup.GetComponent<RectTransform>().localScale, new Vector2(0,0), 100);
        GameManager.instance.HideTutorial();
    }
}
