using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimensionTransportationManager : MonoBehaviour
{
    public Dimension currentDimesnion;
    public UniversalScriptManager usm;
    SaveManager sm;
    ChunkLoader cl;
    HpManager hm;
    GameObject player;

    private void Awake()
    {
        player = usm.player;
        sm = usm.saveManager;
        cl = usm.chunkLoader;
        hm = usm.hpManager;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            GoToNether();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            GoToOverWorld();
        }
    }

    public void GoToNether()
    {
        Debug.Log("gotonether");
        sm.Save();
        StartCoroutine(usm.loadingManager.Load());
        cl.hasSpecificSpawnPos = true;
        cl.specificSpawnPos = usm.player.transform.position;//usm.playerPositionRecorder.lastPos_nether;
        usm.player.transform.position = cl.specificSpawnPos;
        cl.firstTimeLoading = true;
        currentDimesnion = Dimension.Nether;
        hm.lastGroundedHeight = 0;
    }

    public void GoToOverWorld()
    {
        Debug.Log("gotooverworld");
        sm.Save();
        StartCoroutine(usm.loadingManager.Load());
        cl.hasSpecificSpawnPos = true;
        cl.specificSpawnPos = usm.playerPositionRecorder.lastPos_overWorld;
        usm.player.transform.position = cl.specificSpawnPos;
        cl.firstTimeLoading = true;
        currentDimesnion = Dimension.OverWorld;
        hm.lastGroundedHeight = 0;
    }


}
