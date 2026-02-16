using System.Collections.Generic;
using UnityEngine;

public class MakeTransperentScript : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform player;
    [SerializeField] Transform cameraTransform;

    [SerializeField] List<Iam_InTheWay> currentlyInTheWay;
    [SerializeField] List<Iam_InTheWay> alreadyTransparent;

    private void Start()
    {
        if (!player && GameManager.instance && GameManager.instance.player)
            player = GameManager.instance.player.transform;

                if (!cameraTransform && Camera.main)
            cameraTransform = Camera.main.transform;
    }


    void Awake()
    {
        currentlyInTheWay = new List<Iam_InTheWay>();
        alreadyTransparent = new List<Iam_InTheWay>();
    }
    void Update()
    {
        GetAllObjectsInTheWay();

        MakeObjectsSolid();
        MakeObjectsTransperant();

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void GetAllObjectsInTheWay()
    {
        currentlyInTheWay.Clear();

        float cameraPlayerDistance = Vector3.Magnitude(cameraTransform.position - player.position);

        Ray ray1_forward = new Ray(cameraTransform.position, player.position - cameraTransform.position);
        Ray ray1_Backward = new Ray(player.position, cameraTransform.position - player.position);


        var hits1_Forward = Physics.RaycastAll(ray1_forward, cameraPlayerDistance);
        var hits1_Backward = Physics.RaycastAll(ray1_Backward, cameraPlayerDistance);

        foreach (var hit in hits1_Forward)
        {
            if (hit.collider.gameObject.TryGetComponent(out Iam_InTheWay inTheWay))
            {
                if (!currentlyInTheWay.Contains(inTheWay))
                {
                    currentlyInTheWay.Add(inTheWay);
                }
            }
        }
        foreach (var hit in hits1_Backward)
        {
            if (hit.collider.gameObject.TryGetComponent(out Iam_InTheWay inTheWay))
            {
                if (!currentlyInTheWay.Contains(inTheWay))
                {
                    currentlyInTheWay.Add(inTheWay);
                }
            }
        }


    }

    // Update is called once per frame
    void MakeObjectsTransperant()
    {
        for (int i = 0; i < currentlyInTheWay.Count; i++)
        {
            Iam_InTheWay inTheWay = currentlyInTheWay[i];
            
            if (!alreadyTransparent.Contains(inTheWay))
            {
                inTheWay.ShowTransperant();
                alreadyTransparent.Add(inTheWay);
            }
        }
    }
    void MakeObjectsSolid()
    {
        for (int i = alreadyTransparent.Count-1; i >= 0; i--)
        {
            Iam_InTheWay wasInTheWay = alreadyTransparent[i];

            if (!currentlyInTheWay.Contains(wasInTheWay))
            {
                wasInTheWay.ShowSolid();
                alreadyTransparent.Remove(wasInTheWay);
            }
        }
    }
}
