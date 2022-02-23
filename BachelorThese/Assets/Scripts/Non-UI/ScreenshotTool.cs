using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenshotTool : MonoBehaviour
{
    ReferenceManager refM;
    InputMap controls;
    string nextFilename;

    void Start()
    {
        SetValues();
        EnableControls();

        if (refM.enableScreenshots)
            controls.Other.Screenshot.performed += cxt => TakeScreenshot();
    }

    void SetValues()
    {
        refM = ReferenceManager.instance;
        controls = new InputMap();
    }

    void TakeScreenshot()
    {
        GenerateNextFileName();
        ScreenCapture.CaptureScreenshot(Application.dataPath + "/Data/Screenshots/" + nextFilename, 8);
    }
    void GenerateNextFileName()
    {
        System.DateTime currentTime = System.DateTime.Now;
        nextFilename = "Screenshot_" + currentTime.Year + "_" + currentTime.Month + "_" +
            currentTime.Day + "_" + currentTime.Hour + "_" + currentTime.Minute + "_"
            + currentTime.Second + ".png";
    }
    private void OnEnable()
    {
        EnableControls();
    }
    void EnableControls()
    {
        if (controls != null)
            controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }
}
