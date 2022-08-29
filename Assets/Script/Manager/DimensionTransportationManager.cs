using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimensionTransportationManager : MonoBehaviour
{
    public UniversalScriptManager usm;
    GameObject player;

    private void Awake()
    {
        player = usm.player;
    }

    public void GoToNether()
    {
        Debug.Log("gotonether");

        SaveSystem.SaveWorldData(usm);
    }

    public void GoToOverWorld()
    {
        Debug.Log("gotooverworld");

        WorldData data = SaveSystem.LoadWorldData();
        player.transform.position = new Vector3(data.lastPlayerPosition_OverWorld[0], data.lastPlayerPosition_OverWorld[1], data.lastPlayerPosition_OverWorld[2]);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GoToNether();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            GoToOverWorld();
        }
    }
}
