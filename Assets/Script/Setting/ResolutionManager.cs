using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolutionManager : MonoBehaviour
{
    void Update()
    {
        bool fullScreen = PlayerPrefs.GetInt("FullScreen") == 1;
        Screen.SetResolution(fullScreen ? 1920 : 1600, fullScreen ? 1080 : 900, fullScreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed);
    }
}
