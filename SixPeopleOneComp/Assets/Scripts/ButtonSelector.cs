using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using Unity.Mathematics;

public class ButtonSelector : MonoBehaviour
{

    [SerializeField] public Button LevelbuttonSelected;
    [SerializeField] public Button BackButtonSelected;
    [SerializeField] public Button optionsButtonSelected;
    [SerializeField] public Button optionsBackButtonSelected;
    [SerializeField] public EventSystem eventSystem;
    public void selectLevelButton()
    {
        eventSystem.SetSelectedGameObject(LevelbuttonSelected.gameObject);
        eventSystem.firstSelectedGameObject = LevelbuttonSelected.gameObject;
    }

    public void selectBackButton()
    {
        eventSystem.SetSelectedGameObject(BackButtonSelected.gameObject);
        eventSystem.firstSelectedGameObject = BackButtonSelected.gameObject;
    }

    public void selectOptionsButton()
    {
        eventSystem.SetSelectedGameObject(optionsButtonSelected.gameObject);
        eventSystem.firstSelectedGameObject = optionsButtonSelected.gameObject;
    }

    public void selectOptionsBackButton()
    {
        eventSystem.SetSelectedGameObject(optionsBackButtonSelected.gameObject);
        eventSystem.firstSelectedGameObject = optionsBackButtonSelected.gameObject;
    }

 
}
