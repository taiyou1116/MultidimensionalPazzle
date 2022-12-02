using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StagePhoto : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) {
            ScreenCapture.CaptureScreenshot(Application.dataPath + "/savedata.PNG");
        }
    }
}
