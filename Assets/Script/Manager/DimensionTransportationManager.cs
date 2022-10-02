using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimensionTransportationManager : MonoBehaviour
{
    public Dimension currentDimesnion;
    public UniversalScriptManager usm;
    SaveManager sm;
    BlockPlacementManager bpm;
    ChunkLoader cl;
    HpManager hm;
    NetherPortalGenerationManager npgm;
    GameObject player;
    public float playerEnsureRadius;

    private void Awake()
    {
        player = usm.player;
        sm = usm.saveManager;
        cl = usm.chunkLoader;
        hm = usm.hpManager;
        bpm = usm.blockPlacementManager;
        npgm = usm.netherPortalGenerationManager;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            GoToNether(null);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            GoToOverWorld(null);
        }
    }

    public void GoToNether(NetherPortal usedPortal)
    {

        sm.Save();
        StartCoroutine(usm.loadingManager.Load());
        cl.hasSpecificSpawnPos = true;
        cl.specificSpawnPos = usm.player.transform.position;//usm.playerPositionRecorder.lastPos_nether;
        usm.player.transform.position = cl.specificSpawnPos;
        cl.firstTimeLoading = true;
        currentDimesnion = Dimension.Nether;
        hm.lastGroundedHeight = 0;
        if (usedPortal == null)
        {
            Debug.Log("nope");
            return;
        }
        npgm.CopyNetherPortal(usedPortal, Dimension.Nether);
    }

    public void GoToOverWorld(NetherPortal usedPortal)
    {

        sm.Save();
        StartCoroutine(usm.loadingManager.Load());
        cl.hasSpecificSpawnPos = true;
        cl.specificSpawnPos = usm.player.transform.position;//usm.playerPositionRecorder.lastPos_overWorld;
        usm.player.transform.position = cl.specificSpawnPos;
        cl.firstTimeLoading = true;
        currentDimesnion = Dimension.OverWorld;
        hm.lastGroundedHeight = 0;
        if (usedPortal == null)
            return;
        npgm.CopyNetherPortal(usedPortal, Dimension.OverWorld);
    }


}
