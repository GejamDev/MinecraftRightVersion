using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimensionTransportationManager : MonoBehaviour
{
    public UniversalScriptManager usm;
    SaveManager sm;
    GameObject player;

    private void Awake()
    {
        player = usm.player;
        sm = usm.saveManager;
    }

    public void GoToNether()
    {
        Debug.Log("gotonether");
        sm.Save();
    }

    public void GoToOverWorld()
    {
        Debug.Log("gotooverworld");
    }

}
