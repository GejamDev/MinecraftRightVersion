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
    public int playerHp;
    public int playerHunger;
    public float currentTime;
    public int curSlot;
    public string[] playerInventory_item;
    public int[] playerInventory_amount;
    public int[,] waterChunkData;
    public int[] waterData;
    public int[,] lavaChunkData;
    public int[] lavaData;
    public WorldData (UniversalScriptManager usm)
    {

        #region player stuff

        //player pos
        lastPlayerPosition_OverWorld = new float[3];
        lastPlayerPosition_OverWorld[0] = usm.player.transform.position.x;
        lastPlayerPosition_OverWorld[1] = usm.player.transform.position.y;
        lastPlayerPosition_OverWorld[2] = usm.player.transform.position.z;
        playerHp = usm.hpManager.hp;
        playerHunger = usm.hungerManager.hunger;

        currentTime = usm.lightingManager.TimeOfDay;





        #endregion

        #region inventory
        curSlot = usm.inventoryManager.curInventorySlot;
        playerInventory_amount = new int[36];
        playerInventory_item = new string[36];
        for (int i = 0; i < 36; i++)
        {
            InventoryCell cell = usm.inventoryManager.inventoryCellList[i];
            InventorySlot slot = usm.inventoryManager.inventoryDictionary[cell];
            if (slot.amount == 0 || slot.item == null)
            {

                playerInventory_amount[i] = 0;
                playerInventory_item[i] = "";
            }
            else
            {

                playerInventory_amount[i] = slot.amount;
                playerInventory_item[i] = slot.item.name;
            }
        }
        #endregion

        #region world stuff


        //seed
        seed = usm.seedManager.seed;

        //terrain
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


        //water
        waterChunkData = new int[usm.chunkLoader.chunkDictionary.Count,3];
        int waterDataTotalCount = 0;
        List<Vector3> waterPoseList = new List<Vector3>();
        int waterChunkIndex = 0;
        foreach(ChunkProperties cp in usm.chunkLoader.chunkDictionary.Values)
        {
            ChunkScript cs = cp.cs;
            waterDataTotalCount += cs.waterData.Count;
            waterChunkData[waterChunkIndex, 0] = (int)cs.position.x;
            waterChunkData[waterChunkIndex, 1] = (int)cs.position.y;
            waterChunkData[waterChunkIndex, 2] = (int)cs.waterData.Count;
            foreach(Vector3 v in cs.waterData)
            {
                waterPoseList.Add(v + new Vector3(cs.position.x, 0, cs.position.y));
            }
            waterChunkIndex++;
        }

        waterData = new int[waterDataTotalCount*3];
        for(int i = 0; i < waterPoseList.Count; i++)
        {
            waterData[i * 3] = (int)waterPoseList[i].x;
            waterData[i * 3 + 1] = (int)waterPoseList[i].y;
            waterData[i * 3 + 2] = (int)waterPoseList[i].z;
        }


        //lava
        lavaChunkData = new int[usm.chunkLoader.chunkDictionary.Count, 3];
        int lavaDataTotalCount = 0;
        List<Vector3> lavaPoseList = new List<Vector3>();
        int lavaChunkIndex = 0;
        foreach (ChunkProperties cp in usm.chunkLoader.chunkDictionary.Values)
        {
            ChunkScript cs = cp.cs;
            lavaDataTotalCount += cs.lavaData.Count;
            lavaChunkData[lavaChunkIndex, 0] = (int)cs.position.x;
            lavaChunkData[lavaChunkIndex, 1] = (int)cs.position.y;
            lavaChunkData[lavaChunkIndex, 2] = (int)cs.lavaData.Count;
            foreach (Vector3 v in cs.lavaData)
            {
                lavaPoseList.Add(v + new Vector3(cs.position.x, 0, cs.position.y));
            }
            lavaChunkIndex++;
        }

        lavaData = new int[lavaDataTotalCount * 3];
        for (int i = 0; i < lavaPoseList.Count; i++)
        {
            lavaData[i * 3] = (int)lavaPoseList[i].x;
            lavaData[i * 3 + 1] = (int)lavaPoseList[i].y;
            lavaData[i * 3 + 2] = (int)lavaPoseList[i].z;
        }

        //block



        #endregion
    }
}
