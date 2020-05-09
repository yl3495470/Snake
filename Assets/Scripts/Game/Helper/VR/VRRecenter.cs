using UnityEngine.VR;
using UnityEngine;
using System.Collections;

public class VRRecenter : MonoBehaviour 
{
    void OnGUI()
    {
		if(GameConfig.instance.IsShowRecenter)
        {
            if (GUI.Button(new Rect(Screen.width * 0.5f - 60, Screen.height - 20f, 120, 20), "Recenter"))
            {
                UnityEngine.XR.InputTracking.Recenter();
            }

            if (GUI.Button(new Rect(Screen.width * 0.5f - 60, Screen.height - 40f, 40, 20), "<<"))
            {
                if (Time.timeScale > 1)
                    Time.timeScale -= 1;
                else if(Time.timeScale > 0.1f)
                    Time.timeScale -= 0.1f;
            }

            GUI.Label(new Rect(Screen.width * 0.5f - 20, Screen.height - 40f, 40, 20), Time.timeScale.ToString("f2"));

            if (GUI.Button(new Rect(Screen.width * 0.5f + 20, Screen.height - 40f, 40, 20), ">>"))
            {
                if(Time.timeScale < 1)
                    Time.timeScale += 0.1f;
                else if(Time.timeScale >= 1)
                    Time.timeScale += 1f;
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            UnityEngine.XR.InputTracking.Recenter();
        }
        if(Input.GetKeyDown(KeyCode.Equals))
        {
            if (Time.timeScale < 1)
                Time.timeScale += 0.1f;
            else if (Time.timeScale >= 1)
                Time.timeScale += 1f;
        }
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            if (Time.timeScale > 1)
                Time.timeScale -= 1;
            else if (Time.timeScale > 0.1f)
                Time.timeScale -= 0.1f;
        }
    }
}
