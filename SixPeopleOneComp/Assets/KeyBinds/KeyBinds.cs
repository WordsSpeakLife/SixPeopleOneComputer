using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "KeyBinds", menuName = "Scriptable Objects/KeyBinds")]
public class KeyBinds : ScriptableObject
{
    public KeyCode moveUp;
    public KeyCode moveDown;
    public KeyCode moveLeft;
    public KeyCode moveRight;
    public KeyCode Dash;
    public KeyCode Jump;
    // public KeyCode Fire1;

}
