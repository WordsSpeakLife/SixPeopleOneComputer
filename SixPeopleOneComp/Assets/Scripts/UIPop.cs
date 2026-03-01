using UnityEngine;

public class UIPop : MonoBehaviour
{
    GameObject panel;
    Animation anim;
    private void OnEnable()
    {
        panel = GameManager.instance.menuActive;
        anim = panel.GetComponent<Animation>();
    }
    public void animPlay()
    {
        if (GameManager.instance.menuActive)
        {
            popOut();
        }
    }

    public void popIn()
    {
        anim.Play("UIpopin");
    }
    public void popOut()
    {
        anim.Play("UIpopout");
    }
}
