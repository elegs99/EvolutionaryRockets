using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameRateScript : MonoBehaviour
{
    public static int target = 60;
    public Slider slider;
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        slider.value = 60;
    }
    void Update()
    {
        if(target != Application.targetFrameRate)
        {
            Application.targetFrameRate = target;
        }
    }
    public void ChangeFramerate() {
        target = (int)slider.value;
    }
}
