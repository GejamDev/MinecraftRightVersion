﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WorldData
{
    public float[] lastPlayerPosition_OverWorld;
    public float playerYRot;
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
    public int[,] blockChunkData;
    public string[] blockData_blockType;
    public float[] blockData_transform;
    public int[,] grassChunkData;
    public float[] grassData;
    public int[,] fireChunkData;
    public float[] fireData;
    public WorldData (UniversalScriptManager usm)
    {

        #region player stuff

        //player pos
        lastPlayerPosition_OverWorld = new float[3];
        lastPlayerPosition_OverWorld[0] = usm.player.transform.position.x;
        lastPlayerPosition_OverWorld[1] = usm.player.transform.position.y;
        lastPlayerPosition_OverWorld[2] = usm.player.transform.position.z;
        playerYRot = usm.player.transform.eulerAngles.y;
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
        int unloadedWaterSavedChunk = 0;
        foreach (Vector2 v in usm.saveManager.savedWaterData.Keys)
        {
            if (!usm.chunkLoader.chunkDictionary.ContainsKey(v))
            {
                unloadedWaterSavedChunk++;
            }
        }
        waterChunkData = new int[usm.chunkLoader.chunkDictionary.Count + unloadedWaterSavedChunk,3];
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
        foreach (Vector2 pos in usm.saveManager.savedWaterData.Keys)
        {
            if (!usm.chunkLoader.chunkDictionary.ContainsKey(pos))
            {
                waterChunkData[waterChunkIndex, 0] = (int)pos.x;
                waterChunkData[waterChunkIndex, 1] = (int)pos.y;
                waterChunkData[waterChunkIndex, 2] = usm.saveManager.savedWaterData[pos].Count;
                foreach (Vector3 v in usm.saveManager.savedWaterData[pos])
                {
                    waterDataTotalCount++;
                    waterPoseList.Add(v + new Vector3(pos.x, 0, pos.y));
                }

                waterChunkIndex++;
            }
        }

        waterData = new int[waterDataTotalCount*3];
        for(int i = 0; i < waterPoseList.Count; i++)
        {
            waterData[i * 3] = (int)waterPoseList[i].x;
            waterData[i * 3 + 1] = (int)waterPoseList[i].y;
            waterData[i * 3 + 2] = (int)waterPoseList[i].z;
        }


        //lava
        int unloadedLavaSavedChunk = 0;
        foreach (Vector2 v in usm.saveManager.savedLavaData.Keys)
        {
            if (!usm.chunkLoader.chunkDictionary.ContainsKey(v))
            {
                unloadedLavaSavedChunk++;
            }
        }
        lavaChunkData = new int[usm.chunkLoader.chunkDictionary.Count + unloadedLavaSavedChunk, 3];
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
        foreach (Vector2 pos in usm.saveManager.savedLavaData.Keys)
        {
            if (!usm.chunkLoader.chunkDictionary.ContainsKey(pos))
            {
                lavaChunkData[lavaChunkIndex, 0] = (int)pos.x;
                lavaChunkData[lavaChunkIndex, 1] = (int)pos.y;
                lavaChunkData[lavaChunkIndex, 2] = usm.saveManager.savedLavaData[pos].Count;
                foreach (Vector3 v in usm.saveManager.savedLavaData[pos])
                {
                    lavaDataTotalCount++;
                    lavaPoseList.Add(v + new Vector3(pos.x, 0, pos.y));
                }

                lavaChunkIndex++;
            }
        }

        lavaData = new int[lavaDataTotalCount * 3];
        for (int i = 0; i < lavaPoseList.Count; i++)
        {
            lavaData[i * 3] = (int)lavaPoseList[i].x;
            lavaData[i * 3 + 1] = (int)lavaPoseList[i].y;
            lavaData[i * 3 + 2] = (int)lavaPoseList[i].z;
        }



        //fire
        int unloadedFireSavedChunk = 0;
        foreach (Vector2 v in usm.saveManager.savedFireData.Keys)
        {
            if (!usm.chunkLoader.chunkDictionary.ContainsKey(v))
            {
                unloadedFireSavedChunk++;
            }
        }
        fireChunkData = new int[usm.chunkLoader.chunkDictionary.Count + unloadedFireSavedChunk, 3];
        int fireDataTotalCount = 0;
        List<Vector3> firePosList = new List<Vector3>();
        int fireChunkIndex = 0;
        foreach (ChunkProperties cp in usm.chunkLoader.chunkDictionary.Values)
        {
            ChunkScript cs = cp.cs;
            fireDataTotalCount += cs.fireData.Count;
            fireChunkData[fireChunkIndex, 0] = (int)cs.position.x;
            fireChunkData[fireChunkIndex, 1] = (int)cs.position.y;
            fireChunkData[fireChunkIndex, 2] = (int)cs.fireData.Count;
            foreach (Vector3 v in cs.fireData)
            {
                firePosList.Add(v + new Vector3(cs.position.x, 0, cs.position.y));
            }
            fireChunkIndex++;
        }
        foreach (Vector2 pos in usm.saveManager.savedFireData.Keys)
        {
            if (!usm.chunkLoader.chunkDictionary.ContainsKey(pos))
            {
                fireChunkData[fireChunkIndex, 0] = (int)pos.x;
                fireChunkData[fireChunkIndex, 1] = (int)pos.y;
                fireChunkData[fireChunkIndex, 2] = usm.saveManager.savedFireData[pos].Count;
                foreach (Vector3 v in usm.saveManager.savedFireData[pos])
                {
                    fireDataTotalCount++;
                    firePosList.Add(v + new Vector3(pos.x, 0, pos.y));
                }

                fireChunkIndex++;
            }
        }

        fireData = new float[fireDataTotalCount * 3];
        for (int i = 0; i < firePosList.Count; i++)
        {
            fireData[i * 3] = firePosList[i].x;
            fireData[i * 3 + 1] = firePosList[i].y;
            fireData[i * 3 + 2] = firePosList[i].z;
        }







        //grass
        int unloadedGrassSavedChunk = 0;
        foreach (Vector2 v in usm.saveManager.savedGrassData.Keys)
        {
            if (!usm.chunkLoader.chunkDictionary.ContainsKey(v))
            {
                unloadedGrassSavedChunk++;
            }
        }
        grassChunkData = new int[usm.chunkLoader.chunkDictionary.Count + unloadedGrassSavedChunk, 3];
        int grassDataTotalCount = 0;
        List<Vector3> grassPoseList = new List<Vector3>();
        List<float> grassScaleList = new List<float>();
        List<Vector2> grassRotList = new List<Vector2>();
        int grassChunkIndex = 0;
        foreach (ChunkProperties cp in usm.chunkLoader.chunkDictionary.Values)
        {
            ChunkScript cs = cp.cs;
            grassDataTotalCount += cs.grassList.Count;
            grassChunkData[grassChunkIndex, 0] = (int)cs.position.x;
            grassChunkData[grassChunkIndex, 1] = (int)cs.position.y;
            grassChunkData[grassChunkIndex, 2] = cs.grassList.Count;
            foreach (GrassScript gs in cs.grassList)
            {
                grassPoseList.Add(gs.transform.position);
                grassScaleList.Add(gs.grassObject.transform.localScale.y);
                grassRotList.Add(new Vector2(gs.grassHolder.transform.eulerAngles.x, gs.grassHolder.transform.eulerAngles.z));
            }
            grassChunkIndex++;
        }
        foreach (Vector2 pos in usm.saveManager.savedGrassData.Keys)
        {
            if (!usm.chunkLoader.chunkDictionary.ContainsKey(pos))
            {
                grassChunkData[grassChunkIndex, 0] = (int)pos.x;
                grassChunkData[grassChunkIndex, 1] = (int)pos.y;
                grassChunkData[grassChunkIndex, 2] = usm.saveManager.savedGrassData[pos].Count;
                foreach (GrassProperty gp in usm.saveManager.savedGrassData[pos])
                {
                    grassDataTotalCount++;
                    grassPoseList.Add(gp.pos + new Vector3(pos.x, 0, pos.y));
                    grassScaleList.Add(gp.scale);
                    grassRotList.Add(gp.rot);
                }

                grassChunkIndex++;
            }
        }

        grassData = new float[grassDataTotalCount * 6];
        for (int i = 0; i < grassPoseList.Count; i++)
        {
            grassData[i * 6] = grassPoseList[i].x;
            grassData[i * 6 + 1] = grassPoseList[i].y;
            grassData[i * 6 + 2] = grassPoseList[i].z;
            grassData[i * 6 + 3] = grassScaleList[i];
            grassData[i * 6 + 4] = grassRotList[i].x;
            grassData[i * 6 + 5] = grassRotList[i].y;
        }










        //block
        int unloadedBlockSavedChunk = 0;
        foreach (Vector2 v in usm.saveManager.savedBlockData.Keys)
        {
            if (!usm.chunkLoader.chunkDictionary.ContainsKey(v))
            {
                unloadedBlockSavedChunk++;
            }
        }
        blockChunkData = new int[usm.chunkLoader.chunkDictionary.Count + unloadedBlockSavedChunk, 3];
        List<BlockData> blockDataList = new List<BlockData>();
        List<BlockData_Transform> blockData_TransformList = new List<BlockData_Transform>();
        int blockIndex = 0;
        foreach (ChunkProperties cp in usm.chunkLoader.chunkDictionary.Values)
        {
            ChunkScript cs = cp.cs;
            blockChunkData[blockIndex, 0] = (int)cs.position.x;
            blockChunkData[blockIndex, 1] = (int)cs.position.y;
            blockChunkData[blockIndex, 2] = cs.blockDataList.Count;
            foreach (BlockData bd in cs.blockDataList)
            {
                blockDataList.Add(bd);
            }
            blockIndex++;
        }
        foreach (Vector2 pos in usm.saveManager.savedBlockData.Keys)
        {
            if (!usm.chunkLoader.chunkDictionary.ContainsKey(pos))
            {
                blockChunkData[blockIndex, 0] = (int)pos.x;
                blockChunkData[blockIndex, 1] = (int)pos.y;
                blockChunkData[blockIndex, 2] = usm.saveManager.savedBlockData[pos].Count;
                foreach (BlockData_Transform bd in usm.saveManager.savedBlockData[pos])
                {
                    blockData_TransformList.Add(bd);
                }
                blockIndex++;
            }
        }
        blockData_blockType = new string[blockDataList.Count + blockData_TransformList.Count];
        blockData_transform = new float[(blockDataList.Count + blockData_TransformList.Count) * 6];
        for (int i =0; i < blockDataList.Count; i++)
        {
            BlockData bd = blockDataList[i];
            blockData_blockType[i] = bd.block.name;
            blockData_transform[i * 6] = bd.obj.transform.localPosition.x;
            blockData_transform[i * 6 +1] = bd.obj.transform.localPosition.y;
            blockData_transform[i * 6 + 2] = bd.obj.transform.localPosition.z;
            blockData_transform[i * 6 + 3] = bd.obj.transform.eulerAngles.x;
            blockData_transform[i * 6 + 4] = bd.obj.transform.eulerAngles.y;
            blockData_transform[i * 6 + 5] = bd.obj.transform.eulerAngles.z;
        }
        for (int i = blockDataList.Count; i < blockDataList.Count + blockData_TransformList.Count; i++)
        {
            BlockData_Transform bd = blockData_TransformList[i - blockDataList.Count];
            blockData_blockType[i] = bd.block.name;
            blockData_transform[i * 6] = bd.pos.x;
            blockData_transform[i * 6 + 1] = bd.pos.y;
            blockData_transform[i * 6 + 2] = bd.pos.z;
            blockData_transform[i * 6 + 3] = bd.rot.x;
            blockData_transform[i * 6 + 4] = bd.rot.y;
            blockData_transform[i * 6 + 5] = bd.rot.z;
        }


        #endregion
    }
}
