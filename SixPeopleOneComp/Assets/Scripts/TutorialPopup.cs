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
    bool isShown = false;
    GameObject curShown;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            max = 10;
            left = max;
            StartCoroutine(ShowMessage());
        }
    }

    private void OnEnable()
    {
        panel = GameManager.instance.tutorialPopup;
        //panel = GameObject;
        anim = panel.GetComponent<Animation>();
    }

    void checkShown()
    {

    }

    void Update()
    {
        if (GameManager.instance.tutorialPopup.Equals(true))
        {
            if (left > 0)
            {
                left -= Time.deltaTime;
                timer.fillAmount = left / max;
            }
            GameManager.instance.HideTutorial();
        }
        else
        {
            left = max;
        }
    }
    IEnumerator ShowMessage()
    {
        GameManager.instance.ShowTutorial(message);
        anim.Play("UIpopout");
        timer = GameManager.instance.tutorialTimer;
        yield return new WaitForSeconds(max);
        anim.Play("UIpopin");
        yield return new WaitForSeconds(.3f);
        //GameManager.instance.HideTutorial();
    }

}
