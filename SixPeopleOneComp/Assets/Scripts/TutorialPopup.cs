using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;

public class TutorialPopup : MonoBehaviour
{
    [TextArea(2, 6)]
    [SerializeField] string message;
    GameObject panel;
    Animation anim;
    public Image timer;
    public float max;
    public float left;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            max = 10;
            StartCoroutine(ShowMessage());
        }
    }

    private void OnEnable()
    {
        panel = GameManager.instance.tutorialPopup;
        //panel = GameObject;
        anim = panel.GetComponent<Animation>();
    }

    void Update()
    {
            if (left > 0)
            {
                left -= Time.deltaTime;
                timer.fillAmount = left / max;
            }
    }
    IEnumerator ShowMessage()
    {

        GameManager.instance.ShowTutorial(message);
        anim.Play("UIpopout");
        left = max;
        timer = GameManager.instance.tutorialTimer;
        yield return new WaitForSeconds(10);
        anim.Play("UIpopin");
        yield return new WaitForSeconds(.3f);
        GameManager.instance.HideTutorial();
    }

}
