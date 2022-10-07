using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class ChunkScript : MonoBehaviour
{
    [Header("Objects")]
    GameObject player;
    public GameObject objectBundle;
    public GameObject meshObject;
    public GameObject bedrock;
    public GameObject bedrock_nether;
    public Transform waterCollisionParent;
    public Transform lavaCollisionParent;

    [Header("Properties")]
    public Dimension dimension;
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
    public WaterSway waterSway;
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
    public Material netherLavaMat;
    public Material overWorldLavaMat;
    public Material defaultLavaMaterial;
    public Material lavaMaterialInWater;
    public bool playerInLava;
    public float lavaSurface;
    public GameObject lavaColliderPrefab;
    public GameObject[,,] lavaCollisions;
    public bool lavaCollisionSetuped;



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



    [HideInInspector] public float[,,] lavaData;

    [HideInInspector] public List<Vector3> vertices_lava = new List<Vector3>();
    [HideInInspector] public List<int> triangles_lava = new List<int>();
    [HideInInspector] public Dictionary<Vector3, int> verticesRangeDictionary_lava = new Dictionary<Vector3, int>();

    [HideInInspector] public bool lavaBeingModified;



    [HideInInspector] public List<GameObject> ores = new List<GameObject>();
    [HideInInspector] public List<BlockData> blockDataList = new List<BlockData>();
    [HideInInspector] public List<Vector3> fireData = new List<Vector3>();
    [HideInInspector] public Dictionary<Vector3, FireScript> fireDictionary = new Dictionary<Vector3, FireScript>();

    private IEnumerator Start()
    {
        //lavaCollisionSetuped = false;
        //lavaCollisions = new GameObject[lavaData.GetLength(0), lavaData.GetLength(1), lavaData.GetLength(2)];
        //for (int y = 0; y < lavaData.GetLength(1); y++)
        //{
        //    for (int x = 0; x < lavaData.GetLength(0); x++)
        //    {
        //        for (int z = 0; z < lavaData.GetLength(2); z++)
        //        {
        //            if (lavaData[x, y, z] <= mg.terrainSuface)
        //            {
        //                GameObject b = Instantiate(lavaColliderPrefab);
        //                b.transform.position = new Vector3(x, y, z) + lavaObj.transform.position;
        //                b.transform.SetParent(lavaCollisionParent);
        //                lavaCollisions[x, y, z] = b;
        //            }
        //        }
        //    }
        //    yield return new WaitForSeconds(0.1f);
        //}
        //Debug.Log("lava col gen finished");
        //lavaCollisionSetuped = true;
        yield return new WaitForSeconds(0.1f);
    }



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
        //waterObj.SetActive(waterData.Count != 0);
        //lavaObj.SetActive(lavaData.Count != 0);

        if (Input.GetKeyDown(KeyCode.R))
        {
            //Debug.Log("1:" + terrainMap.GetLength(0) + "2:" + terrainMap.GetLength(1) + "3:" + terrainMap.GetLength(2));
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
        player = usm.player;
        objectPool = usm.objectPool;
        bedrock.transform.localScale = new Vector3(usm.worldGenerationPreset.chunkSize, 1, usm.worldGenerationPreset.chunkSize);
        bedrock.transform.localPosition = new Vector3(usm.worldGenerationPreset.chunkSize * 0.5f, bedrock.transform.localPosition.y, usm.worldGenerationPreset.chunkSize);
        if(dimension == Dimension.Nether)
        {
            defaultLavaMaterial = netherLavaMat;
            bedrock_nether.SetActive(true);
            bedrock_nether.transform.localScale = new Vector3(usm.worldGenerationPreset.chunkSize, 1, usm.worldGenerationPreset.chunkSize);
            bedrock_nether.transform.localPosition = new Vector3(usm.worldGenerationPreset.chunkSize * 0.5f, bedrock_nether.transform.localPosition.y, usm.worldGenerationPreset.chunkSize);
        }
        else
        {
            defaultLavaMaterial = overWorldLavaMat;
            bedrock_nether.SetActive(false);
        }
        lavaMR.material = defaultLavaMaterial;

        waterSway = waterObj.GetComponent<WaterSway>();
        waterSway.ps = usm.player.GetComponent<PlayerScript>();
    }
    public void Activate()
    {
        objectBundle.SetActive(true);
        if (dimension != Dimension.Nether)
        {
            wg.StartCoroutine(wg.GenerateOres(this));
        }
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

        if (dimension != Dimension.Nether)
        {
            if (!wm.modifiedChunkDataKeys.Contains(this))
            {
                wm.modifiedChunkDataKeys.Add(this);
                wm.modifiedChunksDataDictionary.Add(this, new UpdatedChunkData { cs = this, modifiedPoses = new List<Vector3>() });
            }
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
                        switch (dimension)
                        {
                            case Dimension.OverWorld:
                                wdr.RecordTerrainData(new Vector3Int(x, y, z) + new Vector3Int((int)position.x, 0, (int)position.y), terrainMap[x, y, z]);
                                break;
                            case Dimension.Nether:
                                wdr.RecordNetherTerrainData(new Vector3Int(x, y, z) + new Vector3Int((int)position.x, 0, (int)position.y), terrainMap[x, y, z]);
                                break;
                        }
                        terrainMap_pre[x, y, z] = terrainMap[x, y, z];
                    }
                }
            }
        }
        if (!lm.chunkToUpdateList.Contains(this))
        {
            lm.chunkToUpdateList.Add(this);
        }
    }
    public void RegenerateLavaMesh()
    {

        mg.ReGenerateLavaMesh(this);
    }

    public void ReGenerateLiquidMesh()
    {
        if (!wm.modifiedChunkDataKeys.Contains(this))
        {
            wm.modifiedChunkDataKeys.Add(this);
            wm.modifiedChunksDataDictionary.Add(this, new UpdatedChunkData { cs = this, modifiedPoses = new List<Vector3>() });
        }
    }

    public bool HasBlockAt(Vector3Int pos)
    {
        foreach(BlockData bd in blockDataList)
        {
            if (bd.obj != null)
            {

                if (bd.obj.transform.position == pos + new Vector3(position.x, 0, position.y))
                    return true;
            }
        }
        return false;
    }
    public void RemoveBlockAt(Vector3Int pos)
    {
        foreach (BlockData bd in blockDataList)
        {
            if (bd.obj.transform.position == pos + new Vector3(position.x, 0, position.y))
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
    public GameObject obj;
    public Item block;
}

[System.Serializable]
public class BlockData_Transform
{
    public Vector3 pos;
    public Vector3 rot;
    public Item block;
}