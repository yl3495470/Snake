using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DownloadPanel : MonoBehaviour
{
    public Text mText;
    public Slider ProcessSlider;

    public void UpdateProcess(string text, float process)
    {
        mText.text = text;
        ProcessSlider.value = process;
    }
}
