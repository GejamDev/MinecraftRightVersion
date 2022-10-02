using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public string currentWorldName;
    public UniversalScriptManager usm;
    public Dimension savedDimension;
    public Dictionary<Vector3Int, float> modifiedTerrainValue = new Dictionary<Vector3Int, float>();
    public Dictionary<Vector3Int, float> nether_modifiedTerrainValue = new Dictionary<Vector3Int, float>();
    public Dictionary<Vector2, List<Vector3Int>> savedWaterData = new Dictionary<Vector2, List<Vector3Int>>();
    public Dictionary<Vector2, List<Vector3Int>> savedLavaData = new Dictionary<Vector2, List<Vector3Int>>();
    public Dictionary<Vector2, List<Vector3Int>> nether_savedLavaData = new Dictionary<Vector2, List<Vector3Int>>();
    public Dictionary<Vector2, List<BlockData_Transform>> savedBlockData = new Dictionary<Vector2, List<BlockData_Transform>>();
    public Dictionary<Vector2, List<BlockData_Transform>> nether_savedBlockData = new Dictionary<Vector2, List<BlockData_Transform>>();
    public Dictionary<Vector2, List<GrassProperty>> savedGrassData = new Dictionary<Vector2, List<GrassProperty>>();
    public Dictionary<Vector2, List<GrassProperty>> nether_savedGrassData = new Dictionary<Vector2, List<GrassProperty>>();
    public Dictionary<Vector2, List<Vector3>> savedFireData = new Dictionary<Vector2, List<Vector3>>(); 
    public Dictionary<Vector2, List<Vector3>> nether_savedFireData = new Dictionary<Vector2, List<Vector3>>();
    public Dictionary<Vector3, List<Vector3>> obsidianNetherPortalData = new Dictionary<Vector3, List<Vector3>>();
    public Dictionary<Vector3, List<Vector3>> nether_obsidianNetherPortalData = new Dictionary<Vector3, List<Vector3>>();

    private void Awake()
    {
        Load(PlayerPrefs.GetString("LoadWorldName"));
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            Save();
        }
    }
    public void Save()
    {

        SaveSystem.SaveWorldData(usm);
    }
    public void Load(string worldName)
    {
        currentWorldName = worldName;
        WorldData data = SaveSystem.LoadWorldData(worldName);
        if (data == null)
        {
            Debug.Log("new world");
            usm.dimensionTransportationManager.currentDimesnion = Dimension.OverWorld;
            usm.seedManager.SetSeed_NewWorld();
            usm.chunkLoader.hasSpecificSpawnPos = false;
            usm.chunkLoader.setted = true;
            usm.inventoryManager.savedInventorySlotList = new List<InventorySlot>();
            usm.inventoryManager.SetInventory();
            usm.player.transform.eulerAngles = new Vector3(0, 0, 0);
            return;
        }

        usm.seedManager.ChangeSeed(data.seed);
        usm.dimensionTransportationManager.currentDimesnion = StringToDimension(data.currentDimesnion);

        usm.player.transform.position = new Vector3(data.lastPlayerPosition_OverWorld[0], data.lastPlayerPosition_OverWorld[1], data.lastPlayerPosition_OverWorld[2]);
        usm.playerPositionRecorder.lastPos_overWorld = new Vector3(data.lastPlayerPosition_OverWorld[0], data.lastPlayerPosition_OverWorld[1], data.lastPlayerPosition_OverWorld[2]);
        usm.player.transform.eulerAngles = new Vector3(0, data.playerYRot, 0);

        //terrain
        modifiedTerrainValue.Clear();
        usm.worldDataRecorder.modifiedTerrainDataKeys.Clear();
        usm.worldDataRecorder.modifiedTerrainDataDictionary.Clear();
        for (int i = 0; i < data.terrainModifiedPoses.GetLength(0); i++)
        {
            Vector3Int pos = new Vector3Int(data.terrainModifiedPoses[i, 0], data.terrainModifiedPoses[i, 1], data.terrainModifiedPoses[i, 2]);
            modifiedTerrainValue.Add(pos, data.terrainModifiedValues[i]);
            usm.worldDataRecorder.modifiedTerrainDataKeys.Add(pos);
            usm.worldDataRecorder.modifiedTerrainDataDictionary.Add(pos, data.terrainModifiedValues[i]);
        }

        //nether terrain
        nether_modifiedTerrainValue.Clear();
        usm.worldDataRecorder.nether_modifiedTerrainDataKeys.Clear();
        usm.worldDataRecorder.nether_modifiedTerrainDataDictionary.Clear();
        if(data.nether_terrainModifiedPoses == null)
        {
            Debug.Log("null");
            data.nether_terrainModifiedPoses = new int[0, 0];
            data.nether_terrainModifiedValues = new float[0];
        }

        for (int i = 0; i < data.nether_terrainModifiedPoses.GetLength(0); i++)
        {
            Vector3Int pos = new Vector3Int(data.nether_terrainModifiedPoses[i, 0], data.nether_terrainModifiedPoses[i, 1], data.nether_terrainModifiedPoses[i, 2]);
            nether_modifiedTerrainValue.Add(pos, data.nether_terrainModifiedValues[i]);
            usm.worldDataRecorder.nether_modifiedTerrainDataKeys.Add(pos);
            usm.worldDataRecorder.nether_modifiedTerrainDataDictionary.Add(pos, data.nether_terrainModifiedValues[i]);
        }

        //water
        savedWaterData.Clear();
        int curWaterIndex = 0;
        for(int i =0; i < data.waterChunkData.GetLength(0); i++)
        {
            Vector2 chunkPos = new Vector2(data.waterChunkData[i, 0], data.waterChunkData[i, 1]);
            savedWaterData.Add(chunkPos, new List<Vector3Int>());
            for(int k = 0; k < data.waterChunkData[i, 2]; k++)
            {
                Vector3Int waterPos_global = new Vector3Int(data.waterData[curWaterIndex * 3], data.waterData[curWaterIndex * 3 + 1], data.waterData[curWaterIndex * 3 + 2]);
                Vector3Int waterPos_local = waterPos_global - new Vector3Int((int)chunkPos.x, 0, (int)chunkPos.y);
                savedWaterData[chunkPos].Add(waterPos_local);

                curWaterIndex++;
            }
        }

        //lava
        savedLavaData.Clear();
        int curLavaIndex = 0;
        for (int i = 0; i < data.lavaChunkData.GetLength(0); i++)
        {
            Vector2 chunkPos = new Vector2(data.lavaChunkData[i, 0], data.lavaChunkData[i, 1]);
            savedLavaData.Add(chunkPos, new List<Vector3Int>());
            for (int k = 0; k < data.lavaChunkData[i, 2]; k++)
            {
                Vector3Int lavaPos_global = new Vector3Int(data.lavaData[curLavaIndex * 3], data.lavaData[curLavaIndex * 3 + 1], data.lavaData[curLavaIndex * 3 + 2]);
                Vector3Int lavaPos_local = lavaPos_global - new Vector3Int((int)chunkPos.x, 0, (int)chunkPos.y);

                savedLavaData[chunkPos].Add(lavaPos_local);

                curLavaIndex++;
            }
        }

        //nether lava
        nether_savedLavaData.Clear();
        int nether_curLavaIndex = 0;
        if (data.nether_lavaChunkData == null)
        {
            data.nether_lavaChunkData = new int[0,0];
            data.nether_lavaData = new int[0];
        }
        for (int i = 0; i < data.nether_lavaChunkData.GetLength(0); i++)
        {
            Vector2 chunkPos = new Vector2(data.nether_lavaChunkData[i, 0], data.nether_lavaChunkData[i, 1]);
            nether_savedLavaData.Add(chunkPos, new List<Vector3Int>());
            for (int k = 0; k < data.nether_lavaChunkData[i, 2]; k++)
            {
                Vector3Int lavaPos_global = new Vector3Int(data.nether_lavaData[nether_curLavaIndex * 3], data.nether_lavaData[nether_curLavaIndex * 3 + 1], data.nether_lavaData[nether_curLavaIndex * 3 + 2]);
                Vector3Int lavaPos_local = lavaPos_global - new Vector3Int((int)chunkPos.x, 0, (int)chunkPos.y);

                nether_savedLavaData[chunkPos].Add(lavaPos_local);

                nether_curLavaIndex++;
            }
        }

        //fire
        savedFireData.Clear();
        int curFireIndex = 0;
        for (int i = 0; i < data.fireChunkData.GetLength(0); i++)
        {
            Vector2 chunkPos = new Vector2(data.fireChunkData[i, 0], data.fireChunkData[i, 1]);
            savedFireData.Add(chunkPos, new List<Vector3>());
            for (int k = 0; k < data.fireChunkData[i, 2]; k++)
            {
                Vector3 firePos_global = new Vector3(data.fireData[curFireIndex * 3], data.fireData[curFireIndex * 3 + 1], data.fireData[curFireIndex * 3 + 2]);
                Vector3 firePos_local = firePos_global - new Vector3((int)chunkPos.x, 0, (int)chunkPos.y);

                savedFireData[chunkPos].Add(firePos_local);

                curFireIndex++;
            }
        }

        //nether fire
        if(data.nether_fireChunkData == null)
        {
            data.nether_fireChunkData = new int[0, 0];
            data.nether_fireData = new float[0];
        }
        nether_savedFireData.Clear();
        int nether_curFireIndex = 0;
        for (int i = 0; i < data.nether_fireChunkData.GetLength(0); i++)
        {
            Vector2 chunkPos = new Vector2(data.nether_fireChunkData[i, 0], data.nether_fireChunkData[i, 1]);
            nether_savedFireData.Add(chunkPos, new List<Vector3>());
            for (int k = 0; k < data.nether_fireChunkData[i, 2]; k++)
            {
                Vector3 firePos_global = new Vector3(data.nether_fireData[nether_curFireIndex * 3], data.nether_fireData[nether_curFireIndex * 3 + 1], data.nether_fireData[nether_curFireIndex * 3 + 2]);
                Vector3 firePos_local = firePos_global - new Vector3((int)chunkPos.x, 0, (int)chunkPos.y);

                nether_savedFireData[chunkPos].Add(firePos_local);

                nether_curFireIndex++;
            }
        }


        //grass
        savedGrassData.Clear();
        int curGrassIndex = 0;
        for (int i = 0; i < data.grassChunkData.GetLength(0); i++)
        {
            Vector2 chunkPos = new Vector2(data.grassChunkData[i, 0], data.grassChunkData[i, 1]);
            savedGrassData.Add(chunkPos, new List<GrassProperty>());
            for (int k = 0; k < data.grassChunkData[i, 2]; k++)
            {
                Vector3 grassPos_global = new Vector3(data.grassData[curGrassIndex * 6], data.grassData[curGrassIndex * 6 + 1], data.grassData[curGrassIndex * 6 + 2]);
                Vector3 grassPos_local = grassPos_global - new Vector3((int)chunkPos.x, 0, (int)chunkPos.y);
                float grassScale = data.grassData[curGrassIndex * 6 + 3];
                Vector2 grassRot = new Vector2(data.grassData[curGrassIndex * 6 + 4], data.grassData[curGrassIndex * 6 + 5]);

                GrassProperty gp = new GrassProperty { pos = grassPos_local, scale = grassScale, rot = grassRot };

                savedGrassData[chunkPos].Add(gp);

                curGrassIndex++;
            }
        }

        //nether grass
        if (data.nether_grassChunkData == null)
        {
            data.nether_grassChunkData = new int[0, 0];
        }
        nether_savedGrassData.Clear();
        int nether_curGrassIndex = 0;
        for (int i = 0; i < data.nether_grassChunkData.GetLength(0); i++)
        {
            Vector2 chunkPos = new Vector2(data.nether_grassChunkData[i, 0], data.nether_grassChunkData[i, 1]);
            nether_savedGrassData.Add(chunkPos, new List<GrassProperty>());
            for (int k = 0; k < data.nether_grassChunkData[i, 2]; k++)
            {
                Vector3 grassPos_global = new Vector3(data.nether_grassData[nether_curGrassIndex * 6], data.nether_grassData[nether_curGrassIndex * 6 + 1], data.nether_grassData[nether_curGrassIndex * 6 + 2]);
                Vector3 grassPos_local = grassPos_global - new Vector3((int)chunkPos.x, 0, (int)chunkPos.y);
                float grassScale = data.nether_grassData[curGrassIndex * 6 + 3];
                Vector2 grassRot = new Vector2(data.nether_grassData[curGrassIndex * 6 + 4], data.nether_grassData[curGrassIndex * 6 + 5]);

                GrassProperty gp = new GrassProperty { pos = grassPos_local, scale = grassScale, rot = grassRot };

                nether_savedGrassData[chunkPos].Add(gp);

                nether_curGrassIndex++;
            }
        }

        //obsidian
        obsidianNetherPortalData.Clear();
        for (int i = 0; i < data.obsidianData.GetLength(0); i++)
        {
            List<Vector3> list = new List<Vector3>();
            for (int k = i; k < i + data.obsidianData[i, 3]; k++)
            {
                list.Add(new Vector3(data.obsidian_linkedData[k * 3], data.obsidian_linkedData[k * 3 + 1], data.obsidian_linkedData[k * 3 + 2]));
            }
            obsidianNetherPortalData.Add(new Vector3(data.obsidianData[i, 0], data.obsidianData[i, 1], data.obsidianData[i, 2]), list);
        }

        //nether obsidian
        if(data.nether_obsidianData == null)
        {
            data.nether_obsidianData = new float[0, 0];
            data.nether_obsidian_linkedData = new float[0];
        }
        nether_obsidianNetherPortalData.Clear();
        for (int i = 0; i < data.nether_obsidianData.GetLength(0); i++)
        {
            List<Vector3> list = new List<Vector3>();
            for (int k = i; k < i + data.nether_obsidianData[i, 3]; k++)
            {
                list.Add(new Vector3(data.nether_obsidian_linkedData[k * 3], data.nether_obsidian_linkedData[k * 3 + 1], data.nether_obsidian_linkedData[k * 3 + 2]));
            }
            nether_obsidianNetherPortalData.Add(new Vector3(data.nether_obsidianData[i, 0], data.nether_obsidianData[i, 1], data.nether_obsidianData[i, 2]), list);
        }

        //block
        savedBlockData.Clear();
        int curBlockIndex = 0;
        for(int i = 0; i < data.blockChunkData.GetLength(0); i++)
        {
            Vector2 chunkPos = new Vector2(data.blockChunkData[i, 0], data.blockChunkData[i, 1]);
            savedBlockData.Add(chunkPos, new List<BlockData_Transform>());
            for (int k = 0; k < data.blockChunkData[i, 2]; k++)
            {
                Vector3 blockPos = new Vector3(data.blockData_transform[curBlockIndex * 6], data.blockData_transform[curBlockIndex * 6 + 1], data.blockData_transform[curBlockIndex * 6 + 2]);
                Vector3 blockRot = new Vector3(data.blockData_transform[curBlockIndex * 6 + 3], data.blockData_transform[curBlockIndex * 6 + 4], data.blockData_transform[curBlockIndex * 6 + 5]);
                BlockData_Transform bdt = new BlockData_Transform { pos = blockPos, rot = blockRot, block = usm.inventoryManager.ItemByName(data.blockData_blockType[curBlockIndex]) };
                savedBlockData[chunkPos].Add(bdt);

                curBlockIndex++;
            }
        }


        //nether block
        if(data.nether_blockChunkData == null)
        {
            data.nether_blockChunkData = new int[0, 0];
        }
        nether_savedBlockData.Clear();
        int nether_curBlockIndex = 0;
        for (int i = 0; i < data.nether_blockChunkData.GetLength(0); i++)
        {
            Vector2 chunkPos = new Vector2(data.nether_blockChunkData[i, 0], data.nether_blockChunkData[i, 1]);
            nether_savedBlockData.Add(chunkPos, new List<BlockData_Transform>());
            for (int k = 0; k < data.nether_blockChunkData[i, 2]; k++)
            {
                Vector3 blockPos = new Vector3(data.nether_blockData_transform[nether_curBlockIndex * 6], data.nether_blockData_transform[nether_curBlockIndex * 6 + 1], data.nether_blockData_transform[nether_curBlockIndex * 6 + 2]);
                Vector3 blockRot = new Vector3(data.nether_blockData_transform[nether_curBlockIndex * 6 + 3], data.nether_blockData_transform[nether_curBlockIndex * 6 + 4], data.nether_blockData_transform[nether_curBlockIndex * 6 + 5]);
                BlockData_Transform bdt = new BlockData_Transform { pos = blockPos, rot = blockRot, block = usm.inventoryManager.ItemByName(data.nether_blockData_blockType[nether_curBlockIndex]) };
                nether_savedBlockData[chunkPos].Add(bdt);

                nether_curBlockIndex++;
            }
        }







        //player status
        usm.chunkLoader.CheckPlayerPosition();
        usm.chunkLoader.hasSpecificSpawnPos = true;
        usm.chunkLoader.specificSpawnPos = new Vector3(data.lastPlayerPosition_OverWorld[0], data.lastPlayerPosition_OverWorld[1], data.lastPlayerPosition_OverWorld[2]);
        usm.chunkLoader.setted = true;



        usm.hpManager.hp = data.playerHp;
        usm.hungerManager.hunger = data.playerHunger;

        usm.inventoryManager.curInventorySlot = data.curSlot;
        usm.inventoryManager.savedInventorySlotList = new List<InventorySlot>();
        for(int i =0; i < 36; i++)
        {
            usm.inventoryManager.savedInventorySlotList.Add(new InventorySlot { amount = data.playerInventory_amount[i], item = usm.inventoryManager.ItemByName(data.playerInventory_item[i]) });
        }
        usm.inventoryManager.SetInventory();

        usm.lightingManager.TimeOfDay = data.currentTime;
    }
    public Dimension StringToDimension(string s)
    {
        switch (s)
        {
            case "OverWorld":
                return Dimension.OverWorld;
            case "Nether":
                return Dimension.Nether;
            case "Ender":
                return Dimension.Ender;
        }
        Debug.LogError("wrong dimension name:" + s);
        return Dimension.OverWorld;
    }
}

public enum Dimension
{
    OverWorld,Nether,Ender
}

public class GrassProperty
{
    public Vector3 pos;
    public float scale;
    public Vector2 rot;
}