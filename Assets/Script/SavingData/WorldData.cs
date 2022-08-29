using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldData
{
    public float[] lastPlayerPosition_OverWorld;

    public WorldData (UniversalScriptManager usm)
    {
        lastPlayerPosition_OverWorld[0] = usm.player.transform.position.x;
        lastPlayerPosition_OverWorld[1] = usm.player.transform.position.y;
        lastPlayerPosition_OverWorld[2] = usm.player.transform.position.z;
    }
}
