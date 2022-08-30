using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public UniversalScriptManager usm;
    public Dictionary<Vector3Int, float> modifiedTerrainValue = new Dictionary<Vector3Int, float>();

    private void Awake()
    {
        Load(LoadDimension.OverWorld);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Save();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Load(LoadDimension.OverWorld);
        }
    }
    public void Save()
    {
        Debug.Log("save");
        SaveSystem.SaveWorldData(usm);
    }
    public void Load(LoadDimension dimension)
    {
        WorldData data = SaveSystem.LoadWorldData();
        usm.seedManager.ChangeSeed(data.seed);
        usm.player.transform.position = new Vector3(data.lastPlayerPosition_OverWorld[0], data.lastPlayerPosition_OverWorld[1], data.lastPlayerPosition_OverWorld[2]);


        modifiedTerrainValue.Clear();
        for(int i = 0; i < data.terrainModifiedPoses.GetLength(0); i++)
        {
            Vector3Int pos = new Vector3Int(data.terrainModifiedPoses[i, 0], data.terrainModifiedPoses[i, 1], data.terrainModifiedPoses[i, 2]);
            modifiedTerrainValue.Add(pos, data.terrainModifiedValues[i]);
            Debug.Log(pos);
        }
    }
}

public enum LoadDimension
{
    OverWorld,Nether,Ender
}
