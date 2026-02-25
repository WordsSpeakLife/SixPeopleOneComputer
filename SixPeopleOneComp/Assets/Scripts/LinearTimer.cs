using UnityEngine;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;

public class LinearTimer : MonoBehaviour
{
    private bool isActive = false;
    public Image timer;
    public float max;
    public float left;

    void Start()
    {
            left = max;
            timer = GameManager.instance.tutorialTimer;
    }

    //private void Update()
    //{
    //    if(isActive)
    //    {
    //        left -= Time.deltaTime;
    //        timer.fillAmount = left/max;

    //        if(left <= 0)
    //        {
    //            countdownStop();
    //        }
    //    }
    //}

    // Update is called once per frame
    void Update()
    {
        if (timer.IsActive())
        {
            if (left > 0)
            {
                left -= Time.deltaTime;
                timer.fillAmount = left / max;
            }
        }
    }

    //public void ActivateCountdown(float time)
    //{
    //    isActive = true;
    //    max = time;
    //    left = max;
    //}

    //public void countdownStop()
    //{
    //    isActive = false;
    //}
}
