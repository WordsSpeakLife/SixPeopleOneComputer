using UnityEngine;

public class Iam_InTheWay : MonoBehaviour
{

    [SerializeField] GameObject solidBody;
    [SerializeField] GameObject transperantBody;

    void Awake()
    {
        ShowSolid();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void ShowTransperant()
    {
        solidBody.SetActive(false);
        transperantBody.SetActive(true);
    }

    // Update is called once per frame
    public void ShowSolid()
    {
        solidBody.SetActive(true);
        transperantBody.SetActive(false);
    }
}
