using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public string currentWorldName;
    public UniversalScriptManager usm;
    public Dictionary<Vector3Int, float> modifiedTerrainValue = new Dictionary<Vector3Int, float>();
    public Dictionary<Vector2, List<Vector3Int>> savedWaterData = new Dictionary<Vector2, List<Vector3Int>>();
    public Dictionary<Vector2, List<Vector3Int>> savedLavaData = new Dictionary<Vector2, List<Vector3Int>>();

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
        Debug.Log("save");
        SaveSystem.SaveWorldData(usm);
    }
    public void Load(string worldName)
    {
        currentWorldName = worldName;
        WorldData data = SaveSystem.LoadWorldData(worldName);
        if (data == null)
        {
            Debug.Log("new world");
            usm.seedManager.SetSeed_NewWorld();
            usm.chunkLoader.hasSpecificSpawnPos = false;
            usm.chunkLoader.setted = true;
            usm.inventoryManager.savedInventorySlotList = new List<InventorySlot>();
            usm.inventoryManager.SetInventory();
            return;
        }
        Debug.Log("loaded world");
        usm.seedManager.ChangeSeed(data.seed);
        usm.player.transform.position = new Vector3(data.lastPlayerPosition_OverWorld[0], data.lastPlayerPosition_OverWorld[1], data.lastPlayerPosition_OverWorld[2]);

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



        usm.chunkLoader.CheckPlayerPosition();
        usm.chunkLoader.hasSpecificSpawnPos = true;
        usm.chunkLoader.specificSpawnPos = new Vector3(data.lastPlayerPosition_OverWorld[0], data.lastPlayerPosition_OverWorld[1], data.lastPlayerPosition_OverWorld[2]);
        usm.chunkLoader.setted = true;



        usm.hpManager.hp = data.playerHp;
        usm.hungerManager.hunger = data.playerHunger;

        usm.inventoryManager.curInventorySlot = data.curSlot;
        usm.inventoryManager.savedInventorySlotList = new List<InventorySlot>();
        Debug.Log("inve");
        for(int i =0; i < 36; i++)
        {
            Debug.Log("haha");
            usm.inventoryManager.savedInventorySlotList.Add(new InventorySlot { amount = data.playerInventory_amount[i], item = usm.inventoryManager.ItemByName(data.playerInventory_item[i]) });
        }
        usm.inventoryManager.SetInventory();

        usm.lightingManager.TimeOfDay = data.currentTime;
    }
}

public enum Dimension
{
    OverWorld,Nether,Ender
}
