﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class ChunkScript : MonoBehaviour
{
    [Header("Objects")]
    public GameObject objectBundle;
    public GameObject meshObject;
    public GameObject bedrock;
    public Transform waterCollisionParent;
    public Transform lavaCollisionParent;

    [Header("Properties")]
    public bool activated;
    public Vector2 position;
    public float[,,] terrainMap;
    public float[,,] terrainMap_pre;
    public float[,] heightMap;
    public List<GrassScript> grassList = new List<GrassScript>();
    public List<ObsidianBlock> obsidianData = new List<ObsidianBlock>();
    public List<Vector3> netherPortalData = new List<Vector3>();
    public BiomeProperty biomeProperty;



    public UniversalScriptManager usm;
    MeshGenerator mg;
    WaterManager wm;
    LavaManager lm;
    WorldGenerator wg;
    ObjectPool objectPool;
    WorldDataRecorder wdr;

    [Header("Water Stuff")]
    public GameObject waterObj;
    public MeshFilter waterMF;
    public MeshCollider waterMC;
    public MeshRenderer waterMR;
    public Material defaultWaterMaterial;
    public Material waterMaterialInWater;
    public bool playerInWater;

    [Header("Lava Stuff")]
    public GameObject lavaObj;
    public MeshFilter lavaMF;
    public MeshCollider lavaMC;
    public MeshRenderer lavaMR;
    public Material defaultLavaMaterial;
    public Material lavaMaterialInWater;
    public bool playerInLava;



    [HideInInspector] public FirstPersonController fpc;

    [HideInInspector] public MeshRenderer mr;
    [HideInInspector] public MeshCollider mc;
    [HideInInspector] public MeshFilter mf;


    [HideInInspector]public List<Vector3> vertices = new List<Vector3>();
    [HideInInspector]public List<int> triangles = new List<int>();
    [HideInInspector] public List<Vector2> uvs = new List<Vector2>();
    [HideInInspector] public Dictionary<Vector3, int> verticesRangeDictionary = new Dictionary<Vector3, int>();


    [HideInInspector] public ChunkScript rightChunk;
    [HideInInspector] public ChunkScript leftChunk;
    [HideInInspector] public ChunkScript frontChunk;
    [HideInInspector] public ChunkScript backChunk;



    [HideInInspector] public List<Vector2> waterSurfaceData = new List<Vector2>();
    [HideInInspector] public List<Vector3> waterData = new List<Vector3>();

    [HideInInspector] public List<Vector3> vertices_water = new List<Vector3>();
    [HideInInspector] public List<int> triangles_water = new List<int>();
    [HideInInspector] public Dictionary<Vector3, int> verticesRangeDictionary_water = new Dictionary<Vector3, int>();

    [HideInInspector] public Dictionary<Vector3, WaterPointData> waterPointDictionary = new Dictionary<Vector3, WaterPointData>();
    [HideInInspector] public List<WaterPointData> wpdList = new List<WaterPointData>();

    [HideInInspector] public bool waterBeingModified;

    [HideInInspector] public bool generatingMesh;



    [HideInInspector] public List<Vector3> lavaData = new List<Vector3>();

    [HideInInspector] public List<Vector3> vertices_lava = new List<Vector3>();
    [HideInInspector] public List<int> triangles_lava = new List<int>();
    [HideInInspector] public Dictionary<Vector3, int> verticesRangeDictionary_lava = new Dictionary<Vector3, int>();

    [HideInInspector] public Dictionary<Vector3, WaterPointData> lavaPointDictionary = new Dictionary<Vector3, WaterPointData>();
    [HideInInspector] public List<WaterPointData> lpdList = new List<WaterPointData>();

    [HideInInspector] public bool lavaBeingModified;



    [HideInInspector] public List<GameObject> ores = new List<GameObject>();
    [HideInInspector] public List<BlockData> blockDataList = new List<BlockData>();
    [HideInInspector] public List<Vector3> fireData = new List<Vector3>();
    [HideInInspector] public Dictionary<Vector3, FireScript> fireDictionary = new Dictionary<Vector3, FireScript>();


    private void Update()
    {
        bool preWaterState = playerInWater;
        bool preLavaState = playerInLava;
        if (fpc != null)
        {
            playerInWater = fpc.headInWater;
            playerInLava = fpc.headInLava;
        }
        if ((preWaterState && !playerInWater) || (!preWaterState && playerInWater))
        {
            FlipWaterMesh();
        }
        if ((preLavaState && !playerInLava) || (!preLavaState && playerInLava))
        {
            FlipLavaMesh();
        }
        waterObj.SetActive(waterData.Count != 0);
        lavaObj.SetActive(lavaData.Count != 0);

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("1:" + terrainMap.GetLength(0) + "2:" + terrainMap.GetLength(1) + "3:" + terrainMap.GetLength(2));
        }
    }
    public void FlipWaterMesh()
    {
        waterMF.mesh.triangles = waterMF.mesh.triangles.Reverse().ToArray();
        waterMR.material = fpc.headInWater ? waterMaterialInWater : defaultWaterMaterial;
    }
    public void FlipLavaMesh()
    {
        lavaMF.mesh.triangles = lavaMF.mesh.triangles.Reverse().ToArray();
        lavaMR.material = fpc.headInWater ? lavaMaterialInWater : defaultLavaMaterial;
    }

    public void GetVariables()
    {

        mg = usm.meshGenerator;
        wm = usm.waterManager;
        lm = usm.lavaManager;
        wg = usm.worldGenerator;
        objectPool = usm.objectPool;
        bedrock.transform.localScale = new Vector3(usm.worldGenerationPreset.chunkSize, 1, usm.worldGenerationPreset.chunkSize);
        bedrock.transform.localPosition = new Vector3(usm.worldGenerationPreset.chunkSize * 0.5f, bedrock.transform.localPosition.y, usm.worldGenerationPreset.chunkSize);
    }
    public void Activate()
    {
        objectBundle.SetActive(true);
        wg.StartCoroutine(wg.GenerateOres(this));
        activated = true;
    }
    public void Deactivate()
    {
        activated = false;
        objectBundle.SetActive(false);
        foreach(GameObject g in ores)
        {
            g.SetActive(false);
            g.transform.SetParent(objectPool.transform);
        }
        ores.Clear();
    }
    public void ReGenerateMesh()
    {
        mg.ReGenerateMesh(this);

        if (!wm.modifiedChunkDataKeys.Contains(this))
        {
            wm.modifiedChunkDataKeys.Add(this);
            wm.modifiedChunksDataDictionary.Add(this, new UpdatedChunkData { cs = this,modifiedPoses = new List<Vector3>( )});
        }
        if (!lm.modifiedChunkDataKeys.Contains(this))
        {
            lm.modifiedChunkDataKeys.Add(this);
            lm.modifiedChunksDataDictionary.Add(this, new UpdatedChunkData { cs = this, modifiedPoses = new List<Vector3>() });
        }

        if (wdr == null)
        {
            wdr = usm.worldDataRecorder;
        }
        for(int x = 0; x < terrainMap.GetLength(0); x++)
        {
            for (int y = 0; y < terrainMap.GetLength(1); y++)
            {
                for (int z = 0; z < terrainMap.GetLength(2); z++)
                {
                    if (terrainMap_pre[x, y, z] != terrainMap[x, y, z])
                    {
                        wdr.RecordTerrainData(new Vector3Int(x, y, z) + new Vector3Int((int)position.x, 0, (int)position.y), terrainMap[x, y, z]);
                        terrainMap_pre[x, y, z] = terrainMap[x, y, z];
                    }
                }
            }
        }
    }

    public void ReGenerateLiquidMesh()
    {
        if (!wm.modifiedChunkDataKeys.Contains(this))
        {
            wm.modifiedChunkDataKeys.Add(this);
            wm.modifiedChunksDataDictionary.Add(this, new UpdatedChunkData { cs = this, modifiedPoses = new List<Vector3>() });
        }
        if (!lm.modifiedChunkDataKeys.Contains(this))
        {
            lm.modifiedChunkDataKeys.Add(this);
            lm.modifiedChunksDataDictionary.Add(this, new UpdatedChunkData { cs = this, modifiedPoses = new List<Vector3>() });
        }
    }

    public bool HasBlockAt(Vector3Int pos)
    {
        foreach(BlockData bd in blockDataList)
        {
            if (bd.position == pos)
                return true;
        }
        return false;
    }
    public void RemoveBlockAt(Vector3Int pos)
    {
        foreach (BlockData bd in blockDataList)
        {
            if (bd.position == pos)
            {
                blockDataList.Remove(bd);
                return;
            }
        }
    }
}
[System.Serializable]
public class WaterPointData
{
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
}

[System.Serializable]
public class BlockData
{
    public Vector3Int position;
    public Item block;
}
