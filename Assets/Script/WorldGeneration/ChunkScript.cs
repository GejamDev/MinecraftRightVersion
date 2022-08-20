using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ChunkScript : MonoBehaviour
{
    [Header("Objects")]
    public GameObject objectBundle;
    public GameObject meshObject;
    public GameObject bedrock;
    public GameObject waterObj;
    public GameObject lavaObj;
    public MeshFilter waterMF;
    public MeshFilter lavaMF;
    public MeshCollider waterMC;
    public MeshCollider lavaMC;

    [Header("Properties")]
    public bool activated;
    public Vector2 position;
    public float[,,] terrainMap;
    public float[,] heightMap;
    public List<GrassScript> grassList = new List<GrassScript>();
    public BiomeProperty biomeProperty;



    public UniversalScriptManager usm;
    MeshGenerator mg;
    WaterManager wm;
    LavaManager lm;
    WorldGenerator wg;
    ObjectPool objectPool;


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
    [HideInInspector] public List<Vector3Int> blockPositionData = new List<Vector3Int>();



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
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (!wm.modifiedChunkDataKeys.Contains(this))
            {
                wm.modifiedChunkDataKeys.Add(this);
                wm.modifiedChunksDataDictionary.Add(this, new UpdatedChunkData { cs = this, modifiedPoses = new List<Vector3>() });
            }
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            vertices_water.Clear();
            triangles_water.Clear();
            verticesRangeDictionary_water.Clear();
            waterPointDictionary.Clear();
            wpdList.Clear();
            wm.GenerateWater(this, true);
        }
    }


}
[System.Serializable]
public class WaterPointData
{
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
}
