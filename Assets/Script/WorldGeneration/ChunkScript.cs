using System.Collections;
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
    public float[,] heightMap;
    public List<GrassScript> grassList = new List<GrassScript>();
    public BiomeProperty biomeProperty;



    public UniversalScriptManager usm;
    MeshGenerator mg;
    WaterManager wm;
    LavaManager lm;
    WorldGenerator wg;
    ObjectPool objectPool;

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
    [HideInInspector] public List<Vector3Int> blockPositionData = new List<Vector3Int>();


    private void Update()
    {
        bool preWaterState = playerInWater;
        bool preLavaState = playerInLava;
        if (fpc != null)
        {
            playerInWater = fpc.headInWater;
            playerInLava = fpc.headInLava;
        }
        if((preWaterState && !playerInWater) || (!preWaterState && playerInWater))
        {
            FlipWaterMesh();
        }
        if ((preLavaState && !playerInLava) || (!preLavaState && playerInLava))
        {
            FlipLavaMesh();
        }
        waterObj.SetActive(waterData.Count != 0);
        lavaObj.SetActive(lavaData.Count != 0);
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

}
[System.Serializable]
public class WaterPointData
{
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
}
