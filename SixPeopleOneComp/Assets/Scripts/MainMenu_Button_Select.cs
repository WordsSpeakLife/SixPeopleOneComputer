using UnityEngine;
using UnityEngine.EventSystems;
public class MainMenu_Button_Select : MonoBehaviour
{
    [SerializeField] public GameObject LevelbuttonSelected;

    void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(LevelbuttonSelected);
    }
}
