using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public UniversalScriptManager usm;
    GameObject player;
    FirstPersonController fpc;
    MeshGenerator mg;
    WorldGenerationPreset wgPreset;
    ChunkLoader cl;
    BiomeManager bm;
    WaterManager wm;
    EntitySpawner es;
    LavaManager lm;
    ObjectPool objectPool;

    public GameObject chunkPrefab;
    public Transform worldBundle;
    public float grassGenerationLoadingTime;
    public float grassNoiseSpeed;
    public float minGrassYScale;
    public float maxInclination;
    
    public List<OreProperty> oreList = new List<OreProperty>();

    void Awake()
    {
        player = usm.player;
        fpc = usm.firstPersonController;
        mg = usm.meshGenerator;
        cl = usm.chunkLoader;
        bm = usm.biomeManager;
        wm = usm.waterManager;
        es = usm.entitySpawner;
        lm = usm.lavaManager;
        objectPool = usm.objectPool;
        wgPreset = usm.worldGenerationPreset;
    }

    public ChunkScript MakeChunkAt(Vector2 position, bool loadingTimeExists)
    {
        GameObject chunk = Instantiate(chunkPrefab);
        chunk.transform.position = new Vector3(position.x, 0, position.y);
        chunk.transform.SetParent(worldBundle);
        ChunkScript cs = chunk.GetComponent<ChunkScript>();
        cs.position = position;
        cs.usm = usm;
        cs.GetVariables();
        cs.gameObject.name = "Chunk At " + position.ToString();

        BiomeProperty currentBiome = bm.AssignBiome(cs);
        SetEnviromentToBiome(cs, currentBiome);

        mg.GenerateMesh(cs, loadingTimeExists);


        wm.GenerateWater(cs);
        lm.GenerateLava(cs, 1);


        if (currentBiome.hasTree)
        {
            GenerateTrees(cs);
        }
        if (currentBiome.hasGrass)
        {
            GenerateGrass(cs);
        }
        es.SpawnEntities(cs);

        cs.Activate();
        return cs;
    }
    public void SetEnviromentToBiome(ChunkScript cs, BiomeProperty bp)
    {
        cs.mr.material = bp.terrainMaterial;
    }


    public void GenerateTrees(ChunkScript cs)
    {
        NoisePreset nP = cs.biomeProperty.treeNoisePreset;
        float noise = Noise.Noise2D(cs.position.x, cs.position.y, nP);


        int count = Mathf.RoundToInt(nP.heightMultiplier.Evaluate(noise));
        List<Vector2> generatingPoses = new List<Vector2>();
        for (int i = 0; i < count; i++)
        {
            Vector2 addingPos = new Vector2(Random.Range(0, wgPreset.chunkSize - 0.1f), Random.Range(0, wgPreset.chunkSize - 0.1f));
            generatingPoses.Add(addingPos);
        }
        
        for(int i = 0; i < count; i++)
        {
            float height = cs.heightMap[Mathf.RoundToInt(generatingPoses[i].x), Mathf.RoundToInt(generatingPoses[i].y)];

            if(height >= mg.grassStartHeight && !cs.waterSurfaceData.Contains(new Vector2(Mathf.CeilToInt(generatingPoses[i].x), Mathf.CeilToInt(generatingPoses[i].x))) && !cs.waterSurfaceData.Contains(new Vector2(Mathf.FloorToInt(generatingPoses[i].x), Mathf.FloorToInt(generatingPoses[i].x))))
            {
                GameObject t = Instantiate(cs.biomeProperty.treeObject);
                t.transform.SetParent(cs.objectBundle.transform);
                t.transform.localPosition = new Vector3(generatingPoses[i].x, 0, generatingPoses[i].y) + Vector3.up * height;

                if (t.GetComponent<TreeScript>() != null)
                {
                    TreeScript ts = t.GetComponent<TreeScript>();
                    ts.yPos = cs.heightMap[Mathf.RoundToInt(generatingPoses[i].x), Mathf.RoundToInt(generatingPoses[i].y)];
                    ts.usm = usm;
                    ts.SetTree();
                }
                else if(t.GetComponent<CactusScript>() != null)
                {
                    CactusScript cactusScript = t.GetComponent<CactusScript>();
                    cactusScript.yPos = cs.heightMap[Mathf.RoundToInt(generatingPoses[i].x), Mathf.RoundToInt(generatingPoses[i].y)];
                    cactusScript.SetCactus();
                }
            }
        }
    }
    public void GenerateGrass(ChunkScript cs)
    {
        StartCoroutine(GenerateGrass_Cor(cs));
    }
    public IEnumerator GenerateGrass_Cor(ChunkScript cs)
    {

        NoisePreset np = cs.biomeProperty.grassNoisePreset;
        for(int x = 0; x< wgPreset.chunkSize; x++)
        {
            for (int y = 0; y < wgPreset.chunkSize; y++)
            {
                if(!cs.waterSurfaceData.Contains(new Vector2(x, y)) && !cs.waterSurfaceData.Contains(new Vector2(x + 1, y + 1)))
                {
                    float yScale = np.heightMultiplier.Evaluate(Noise.Noise2D(x + cs.position.x, y + cs.position.y, np));
                    //float yScale = cs.biomeProperty.grassCountMultiplier * Mathf.PerlinNoise((x + cs.position.x) * grassNoiseSpeed, (y + cs.position.y) * grassNoiseSpeed) + Mathf.PerlinNoise((x + cs.position.x + 30) * grassNoiseSpeed, (y + cs.position.y - 30) * grassNoiseSpeed);

                    if (yScale >= cs.biomeProperty.minGrassYScale)
                    {
                        float height = cs.heightMap[x, y];

                        if (height >= 0)
                        {
                            //instantiate
                            GameObject g = Instantiate(cs.biomeProperty.grassObject);
                            g.transform.SetParent(cs.objectBundle.transform);
                            g.transform.localPosition = new Vector3(x, 0, y) + Vector3.up * height;
                            GrassScript gs = g.GetComponent<GrassScript>();
                            cs.grassList.Add(gs);

                            //set scale
                            gs.grassObject.transform.localScale = new Vector3(1, yScale, 1);


                            //rotate

                            float xInclination = 0;

                            xInclination = cs.heightMap[x + 1, y] - height;
                            float xAngle = Mathf.Atan2(xInclination, 1) * Mathf.Rad2Deg;



                            float zInclination = 0;

                            zInclination = cs.heightMap[x, y + 1] - height;
                            float zAngle = Mathf.Atan2(zInclination, 1) * Mathf.Rad2Deg;

                            gs.grassHolder.transform.eulerAngles = new Vector3(-zAngle, 0, xAngle);

                            if (Mathf.Abs(xInclination) > maxInclination || Mathf.Abs(zInclination) > maxInclination)
                            {
                                cs.grassList.Remove(gs);
                                Destroy(g);
                            }







                            //loading time
                            if (grassGenerationLoadingTime != 0)
                                yield return new WaitForSeconds(grassGenerationLoadingTime);
                        }
                    }
                }



            }
        }
    }
    public void GenerateOres(ChunkScript cs)
    {
        foreach(OreProperty op in oreList)
        {
            float time = Random.Range(0, 1.0000f);
            int spawnCount = Mathf.RoundToInt(op.spawnCountGraph.Evaluate(time));
            for(int i =0; i < spawnCount; i++)
            {
                GameObject ore = objectPool.GetObject(op.objectName);
                if(ore == null)
                {
                    break;
                }
                cs.ores.Add(ore);
                ore.transform.SetParent(cs.objectBundle.transform);
                Vector3 pos = new Vector3(Random.Range(0, 8), Random.Range(0, op.maxYSpawnLevel), Random.Range(0, 8));

                if (cs.terrainMap[(int)pos.x, (int)pos.y, (int)pos.z] > mg.terrainSuface)
                {
                    while ((int)pos.y > 0 && cs.terrainMap[(int)pos.x, (int)pos.y, (int)pos.z] > mg.terrainSuface)
                    {
                        pos += Vector3.down;
                    }
                    pos += Vector3.up * 0.5f;
                }


                ore.transform.localPosition = pos + new Vector3(-0.5f, -0.5f, -0.5f);
                ore.transform.localScale = new Vector3(Random.Range(op.minSize.x, op.maxSize.x), Random.Range(op.minSize.y, op.maxSize.y), Random.Range(op.minSize.z, op.maxSize.z));





                ore.transform.eulerAngles = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            }
        }
    }
    
}
