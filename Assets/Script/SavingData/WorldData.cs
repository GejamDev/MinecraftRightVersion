using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldData
{
    public float[] lastPlayerPosition_OverWorld;
    public int seed;
    public int[,] terrainModifiedPoses;
    public float[] terrainModifiedValues;
    public WorldData (UniversalScriptManager usm)
    {
        seed = usm.seedManager.seed;



        lastPlayerPosition_OverWorld = new float[3];
        lastPlayerPosition_OverWorld[0] = usm.player.transform.position.x;
        lastPlayerPosition_OverWorld[1] = usm.player.transform.position.y;
        lastPlayerPosition_OverWorld[2] = usm.player.transform.position.z;

        WorldDataRecorder wdr = usm.worldDataRecorder;
        int modifiedTerrainCount = wdr.modifiedTerrainDataDictionary.Count;
        terrainModifiedPoses = new int[modifiedTerrainCount, 3];
        terrainModifiedValues = new float[modifiedTerrainCount];

        List<Vector3Int> modifiedTerrainKeys = wdr.modifiedTerrainDataKeys;
        for (int i = 0; i < modifiedTerrainCount; i++)
        {
            terrainModifiedPoses[i, 0] = modifiedTerrainKeys[i].x;
            terrainModifiedPoses[i, 1] = modifiedTerrainKeys[i].y;
            terrainModifiedPoses[i, 2] = modifiedTerrainKeys[i].z;
            terrainModifiedValues[i] = wdr.modifiedTerrainDataDictionary[modifiedTerrainKeys[i]];
        }



    }
}
