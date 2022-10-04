using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MeshGenerator : MonoBehaviour
{
    public float loadingTime;
    public UniversalScriptManager usm;
    WorldGenerationPreset wgPreset;
    ChunkLoader cl;
    WaterManager wm;
    SaveManager sm;
    public GameObject waterColliderPrefab;
    public GameObject lavaColliderPrefab;


    public float terrainSuface;
    public float caveStartLevel;
    public float caveSurface;
    public float netherCaveSurface;
    int width = 32;
     int height = 8;
    NoisePreset nPreset;
    NoisePreset nPreset_nether;
    NoisePreset caveNPreset;
    NoisePreset caveNPreset_nether;
    NoisePreset waterGroundDecreaseNoise;
    NoisePreset lavaNoisePreset;
    public float waterSurfaceLevel;
    public float maxWaterLevel;
    public float waterDefaultLevel;
    public float minWaterDepth;
    public float lavaSurfaceLevel;
    public float lavaStartHeight;
    public float grassStartHeight;
    public float rockStartHeight;
    public float netherLavaHeight;

    public bool smoothTerrain;
    public bool smoothWater;
    public int maxPossibleHeight;

    public float waterCollisionGenTime;
    public float lavaCollisionGenTime;

    void Awake()
    {
        wgPreset = usm.worldGenerationPreset;
        cl = usm.chunkLoader;
        wm = usm.waterManager;
        sm = usm.saveManager;

        width = wgPreset.chunkSize;
        height = wgPreset.maxHeight;
        nPreset = wgPreset.nPreset;
        nPreset_nether = wgPreset.nPreset_Nether;
        caveNPreset = wgPreset.caveNPreset;
        lavaNoisePreset = wgPreset.lavaNoisePreset;
        caveNPreset_nether = wgPreset.caveNPreset_Nether;
        waterGroundDecreaseNoise = wgPreset.waterGroundDecreaseNoisePreset;
    }

    #region terrain
    public IEnumerator GenerateMesh(ChunkScript cs, bool loadingTimeExists)
    {
        cs.terrainMap = new float[width + 1, height + 1, width + 1];
        cs.terrainMap_pre = new float[width + 1, height + 1, width + 1];
        yield return StartCoroutine(PopulateTerrainMap(cs));
        if (loadingTimeExists)
        {
            StartCoroutine(CreateMeshData_Cor(cs, false));
        }
        else
        {
            CreateMeshData(cs, false);
            BuildMesh(cs);
        }
    }

    public void ReGenerateMesh(ChunkScript cs)
    {

        cs.vertices.Clear();
        cs.triangles.Clear();
        cs.uvs.Clear();
        cs.verticesRangeDictionary.Clear();
        CreateMeshData(cs, false);
        BuildMesh(cs);

    }
    public IEnumerator PopulateTerrainMap_OverWorld(ChunkScript cs)
    {
        float[,] noiseMap2D = Noise.NoiseMap2D(cs.position.x, cs.position.y, width + 1, nPreset);
        yield return new WaitForSeconds(0.01f);
        float[,,] caveNoiseMap = Noise.NoiseMap3D(cs.position.x, cs.position.y, width + 1, maxPossibleHeight + 1, caveNPreset, new Vector3(1, 1.5f, 1));

        yield return new WaitForSeconds(0.01f);
        float[,] waterGroundDecreaseMap2D = Noise.NoiseMap2D(cs.position.x, cs.position.y, width + 1, waterGroundDecreaseNoise);

        yield return new WaitForSeconds(0.01f);

        float[,,] lavaNoiseMap = Noise.NoiseMap3D(cs.position.x, cs.position.y, width + 1, maxPossibleHeight + 1, lavaNoisePreset, new Vector3(1, 2, 1));

        cs.heightMap = new float[noiseMap2D.GetLength(0), noiseMap2D.GetLength(1)];

        bool hasSavedWaterData = sm.savedWaterData.ContainsKey(cs.position);
        bool hasSaveLavaData = sm.savedLavaData.ContainsKey(cs.position);

        for (int x = 0; x < width + 1; x++)
        {

            for (int z = 0; z < width + 1; z++)
            {
                float thisHeight = nPreset.heightMultiplier.Evaluate(noiseMap2D[x, z]) * nPreset.noiseWeight + nPreset.groundLevel;
                float waterGroundDecreaseHeight = waterGroundDecreaseMap2D[x, z];
                bool isWater = waterGroundDecreaseHeight >= waterSurfaceLevel && thisHeight < maxWaterLevel && thisHeight > waterDefaultLevel + minWaterDepth;
                float originalHeight = thisHeight;
                if (isWater) 
                {
                    thisHeight -= waterGroundDecreaseNoise.heightMultiplier.Evaluate(waterGroundDecreaseHeight);
                    cs.waterSurfaceData.Add(new Vector2(x, z));
                }
                cs.heightMap[x, z] = thisHeight;
                for (int y = 0; y < height + 1; y++)
                {
                    float groundNoise;
                    groundNoise = y - thisHeight;
                    if (!hasSavedWaterData)
                    {
                        if (isWater && groundNoise >= 0 && y - 1 < originalHeight && y - 1 >= 0)
                        {
                            cs.waterData.Add(new Vector3(x, y - 1, z));
                        }
                    }





                    //groundNoise = Mathf.Clamp(groundNoise, -2, 2);


                    if (sm.modifiedTerrainValue.ContainsKey(new Vector3Int(x + (int)cs.position.x, y, z + (int)cs.position.y)))
                    {

                        cs.terrainMap[x, y, z] = sm.modifiedTerrainValue[new Vector3Int(x + (int)cs.position.x, y, z + (int)cs.position.y)];
                        cs.terrainMap_pre[x, y, z] = sm.modifiedTerrainValue[new Vector3Int(x + (int)cs.position.x, y, z + (int)cs.position.y)];
                    }
                    else if (groundNoise >= caveStartLevel || y > maxPossibleHeight)// just ground
                    {
                        cs.terrainMap[x, y, z] = groundNoise;
                        cs.terrainMap_pre[x, y, z] = groundNoise;
                    }
                    else//below the surface
                    {
                        float caveNoise = caveNoiseMap[x, y, z];
                        if (caveNoise >= caveSurface + (-2 * 0.06f))
                        {
                            //cs.terrainMap[x, y, z] = 0;
                        }
                        else
                        {
                            //make hole
                            cs.terrainMap[x, y, z] = terrainSuface + 0.1f; //+ Mathf.Clamp(noiseMap2D[x, (z + Mathf.Abs(y)) % 5] * 1, 0.1f, 1);
                            cs.terrainMap_pre[x, y, z] = terrainSuface + 0.1f;//Mathf.Clamp(noiseMap2D[x, (z + Mathf.Abs(y)) % 5] * 1, 0.1f, 1);
                        }
                    }
                }
                yield return new WaitForSeconds(0.01f);
            }

        }


        yield return new WaitForSeconds(0.01f);
        //generate lava
        for (int x = 0; x < lavaNoiseMap.GetLength(0); x++)
        {
            for (int y = 0; y < lavaStartHeight; y++)
            {
                for (int z = 0; z < lavaNoiseMap.GetLength(2); z++)
                {
                    float lavaNoise = lavaNoiseMap[x, y, z];
                    if (lavaNoise < lavaSurfaceLevel && cs.terrainMap[x, y, z] <= terrainSuface)
                    {
                        if (y >= 1)
                        {
                            bool onGround = false;
                            if (y == 1)
                            {
                                onGround = true;
                            }
                            else if (cs.terrainMap[x, y, z] <= terrainSuface)
                            {
                                onGround = true;
                            }
                            else if (cs.lavaData.Contains(new Vector3(x, y - 2, z)))
                            {
                                onGround = true;
                            }
                            if (onGround)
                            {
                                cs.terrainMap[x, y, z] = 1;
                                cs.terrainMap_pre[x, y, z] = 1;
                                if (!hasSaveLavaData)
                                {
                                    cs.lavaData.Add(new Vector3(x, y - 1, z));
                                }
                            }
                        }
                    }
                }
            }
        }

        //load data
        if (hasSavedWaterData)
        {
            foreach (Vector3Int v in sm.savedWaterData[cs.position])
            {
                cs.waterData.Add(v);
            }
        }
        if (hasSaveLavaData)
        {
            foreach (Vector3Int v in sm.savedLavaData[cs.position])
            {
                cs.lavaData.Add(v);
            }
        }

    }
    public IEnumerator PopulateTerrainMap_Nether(ChunkScript cs)
    {
        float[,] noiseMap2D = Noise.NoiseMap2D(cs.position.x, cs.position.y, width + 1, nPreset_nether);
        yield return new WaitForSeconds(0.01f);
        float[,,] caveNoiseMap = Noise.NoiseMap3D(cs.position.x, cs.position.y, width + 1, height + 1, caveNPreset_nether, new Vector3(1, 1.5f, 1));
        //yield return new WaitForSeconds(0.01f);



        //float[,,] lavaNoiseMap = Noise.NoiseMap3D(cs.position.x, cs.position.y, width + 1, maxPossibleHeight + 1, lavaNoisePreset, new Vector3(1, 2, 1));

        cs.heightMap = new float[noiseMap2D.GetLength(0), noiseMap2D.GetLength(1)];

        bool hasSaveLavaData = sm.nether_savedLavaData.ContainsKey(cs.position);

        for (int x = 0; x < width + 1; x++)
        {

            for (int z = 0; z < width + 1; z++)
            {
                float thisHeight = nPreset_nether.heightMultiplier.Evaluate(noiseMap2D[x, z]) * nPreset_nether.noiseWeight + nPreset_nether.groundLevel;
                cs.heightMap[x, z] = thisHeight;
                for (int y = 0; y < height + 1; y++)
                {
                    float groundNoise;
                    groundNoise = y - thisHeight;





                    groundNoise = Mathf.Clamp(groundNoise, -2, 2);


                    float caveNoise = caveNoiseMap[x, y, z];
                    float val = 0;
                    if (sm.nether_modifiedTerrainValue.ContainsKey(new Vector3Int(x + (int)cs.position.x, y, z + (int)cs.position.y)))
                    {

                        cs.terrainMap[x, y, z] = sm.nether_modifiedTerrainValue[new Vector3Int(x + (int)cs.position.x, y, z + (int)cs.position.y)];
                        cs.terrainMap_pre[x, y, z] = sm.nether_modifiedTerrainValue[new Vector3Int(x + (int)cs.position.x, y, z + (int)cs.position.y)];
                    }
                    else
                    {
                        if (caveNoise >= netherCaveSurface + groundNoise * 0.06f)
                        {
                            val = (caveNoise - netherCaveSurface) * 3;
                            if (groundNoise <= terrainSuface)
                            {
                                val = groundNoise;
                            }
                        }
                        else
                        {
                            //make hole
                            val = (caveNoise - netherCaveSurface) * 3;//Mathf.Lerp(groundNoise, 1, (netherCaveSurface + groundNoise * 0.06f - caveNoise) * -8);
                            if (groundNoise > terrainSuface)
                            {
                            }
                            else
                            {
                                val = groundNoise;
                            }
                        }
                    }
                    cs.terrainMap[x, y, z] = val;
                    cs.terrainMap_pre[x, y, z] = val;
                    if (val > terrainSuface && y <=netherLavaHeight)
                    {
                        cs.lavaData.Add(new Vector3(x, y, z));
                    }
                }
            }
            yield return new WaitForSeconds(0.01f);

        }

        yield return new WaitForSeconds(0.01f);
        //currently disabled
        #region lava
        //generate lava
        //for (int x = 0; x < lavaNoiseMap.GetLength(0); x++)
        //{
        //    for (int y = 0; y < lavaStartHeight; y++)
        //    {
        //        for (int z = 0; z < lavaNoiseMap.GetLength(2); z++)
        //        {
        //            float lavaNoise = lavaNoiseMap[x, y, z];
        //            if (lavaNoise < lavaSurfaceLevel && cs.terrainMap[x, y, z] <= terrainSuface)
        //            {
        //                if (y >= 1)
        //                {
        //                    bool onGround = false;
        //                    if (y == 1)
        //                    {
        //                        onGround = true;
        //                    }
        //                    else if (cs.terrainMap[x, y, z] <= terrainSuface)
        //                    {
        //                        onGround = true;
        //                    }
        //                    else if (cs.lavaData.Contains(new Vector3(x, y - 2, z)))
        //                    {
        //                        onGround = true;
        //                    }
        //                    if (onGround)
        //                    {
        //                        cs.terrainMap[x, y, z] = 1;
        //                        cs.terrainMap_pre[x, y, z] = 1;
        //                        if (!hasSaveLavaData)
        //                        {
        //                            cs.lavaData.Add(new Vector3(x, y - 1, z));
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        #endregion
        //load data
        if (hasSaveLavaData)
        {
            foreach (Vector3Int v in sm.nether_savedLavaData[cs.position])
            {
                cs.lavaData.Add(v);
            }
        }

    }

    IEnumerator PopulateTerrainMap(ChunkScript cs)
    {
        switch (cs.dimension)
        {
            case Dimension.OverWorld:
                yield return StartCoroutine(PopulateTerrainMap_OverWorld(cs));
                break;
            case Dimension.Nether:
                yield return StartCoroutine(PopulateTerrainMap_Nether(cs));
                break;
        }
        
    }
    IEnumerator CreateMeshData_Cor(ChunkScript cs, bool onlySurface)
    {
        cs.generatingMesh = true;
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < width; z++)
            {
                int startLevel = 0;
                if (onlySurface)
                {
                    float groundHeight = cs.heightMap[x, z];
                    startLevel = (int)cs.heightMap[x, z] - 2;
                }

                for (int y = startLevel; y < height; y++)
                {
                    MarchCube(cs, new Vector3Int(x, y, z));
                }
                yield return new WaitForEndOfFrame();
            }
        }
        BuildMesh(cs);
        cs.generatingMesh = false;
    }

    void CreateMeshData(ChunkScript cs, bool onlySurface)
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < width; z++)
            {
                int startLevel = 0;
                if (onlySurface)
                {
                    float groundHeight = cs.heightMap[x, z];
                    startLevel = (int)cs.heightMap[x,z] - 2;
                }

                for (int y = startLevel; y < height; y++)
                {
                    MarchCube(cs, new Vector3Int(x, y, z));
                }
            }
        }
    }

    int GetCubeConfiguration(float[] cube)
    {
        int configurationIndex = 0;
        for (int i = 0; i < 8; i++)
        {
            if (cube[i] > terrainSuface)
            {
                configurationIndex |= 1 << i;
            }
        }

        return configurationIndex;
    }

    void MarchCube(ChunkScript cs, Vector3Int position)
    {
        float[] cube = new float[8];
        for (int i = 0; i < 8; i++)
        {
            //Vector3Int pos = position + CornerTable[i];
            cube[i] = cs.terrainMap[position.x + CornerTableX[i], position.y + CornerTableY[i], position.z + CornerTableZ[i]];
        }


        int configIndex = GetCubeConfiguration(cube);
        if (configIndex == 0 || configIndex == 255)
        {
            return;
        }

        int edgeIndex = 0;
        for (int i = 0; i < 5; i++)
        {
            for (int n = 0; n < 3; n++)
            {
                int indice = TriangleTable[configIndex, edgeIndex];

                if (indice == -1)
                {
                    return;
                }

                Vector3 vert1 = position + CornerTable[EdgeIndexes[indice, 0]];
                Vector3 vert2 = position + CornerTable[EdgeIndexes[indice, 1]];


                Vector3 vertPosition;
                if (smoothTerrain)
                {
                    float vert1Sample = cube[EdgeIndexes[indice, 0]];
                    float vert2Sample = cube[EdgeIndexes[indice, 1]];

                    float difference = vert2Sample - vert1Sample;

                    if (difference == 0)
                    {
                        difference = terrainSuface;
                    }
                    else
                    {
                        difference = (terrainSuface - vert1Sample) / difference;
                    }
                    vertPosition = vert1 + ((vert2 - vert1) * difference);
                }
                else
                {
                    vertPosition = (vert1 + vert2) / 2f;
                }

                cs.triangles.Add(VertForIndice(cs, vertPosition));
                edgeIndex++;
            }
        }
    }

    int VertForIndice(ChunkScript cs, Vector3 vert)
    {

        if (cs.verticesRangeDictionary.ContainsKey(vert))
            return cs.verticesRangeDictionary[vert];

        cs.vertices.Add(vert);
        AddVerticesToDictionary(cs, vert);
        return cs.vertices.Count - 1;
    }
    void AddVerticesToDictionary(ChunkScript cs, Vector3 pos)
    {
        cs.verticesRangeDictionary.Add(pos, cs.vertices.Count - 1);
    }

    float SampleTerrain(ChunkScript cs,Vector3Int point)
    {
        return cs.terrainMap[point.x, point.y, point.z];
    }


    void BuildMesh(ChunkScript cs)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = cs.vertices.ToArray();
        mesh.triangles = cs.triangles.ToArray();
        mesh.uv = cs.uvs.ToArray();
        mesh.RecalculateNormals();

        cs.mf.mesh = mesh;
        cs.mc.sharedMesh = mesh;
        cs.mr.material.SetFloat("_GrassStartHeight", grassStartHeight);
        cs.mr.material.SetFloat("_RockStartHeight", rockStartHeight);
        cs.mr.material.SetFloat("_ChunkSize", wgPreset.chunkSize);
        cs.mr.material.SetFloat("_ChunkPosX", cs.position.x);
        cs.mr.material.SetFloat("_ChunkPosY", cs.position.y);

        StartCoroutine(SetTransitionSetting(cs));
    }
    public Dictionary<Vector2, ChunkProperties> getProperChunkDictionaryByDimension(ChunkScript cs)
    {
        switch (cs.dimension)
        {
            case Dimension.OverWorld:
                return cl.chunkDictionary;
            case Dimension.Nether:
                return cl.nether_chunkDictionary;
        }
        return null;
    }
    IEnumerator SetTransitionSetting(ChunkScript cs)
    {


        //wait till able to check info
        yield return new WaitUntil(() =>

        getProperChunkDictionaryByDimension(cs).ContainsKey(cs.position + Vector2.up * wgPreset.chunkSize)
        &&
        getProperChunkDictionaryByDimension(cs).ContainsKey(cs.position + Vector2.right * wgPreset.chunkSize) 
        &&
        getProperChunkDictionaryByDimension(cs).ContainsKey(cs.position + new Vector2(wgPreset.chunkSize, wgPreset.chunkSize))
        
        );

        BiomeProperty bp_r = getProperChunkDictionaryByDimension(cs)[cs.position + Vector2.right * wgPreset.chunkSize].cs.biomeProperty;
        BiomeProperty bp_f = getProperChunkDictionaryByDimension(cs)[cs.position + Vector2.up * wgPreset.chunkSize].cs.biomeProperty;
        BiomeProperty bp_fr = getProperChunkDictionaryByDimension(cs)[cs.position + new Vector2(wgPreset.chunkSize, wgPreset.chunkSize)].cs.biomeProperty;

        cs.mr.material.SetFloat("_RightTransitionNeeded", (bp_r != cs.biomeProperty) ? 1 : 0);
        cs.mr.material.SetFloat("_FrontTransitionNeeded", (bp_f != cs.biomeProperty) ? 1 : 0);
        cs.mr.material.SetFloat("_RightFrontTransitionNeeded", (bp_fr != cs.biomeProperty) ? 1 : 0);


        cs.mr.material.SetTexture("_RightGrassTex", bp_r.terrainMaterial.GetTexture("_GrassTex"));
        cs.mr.material.SetTexture("_RightWallTex", bp_r.terrainMaterial.GetTexture("_WallTex"));
        cs.mr.material.SetTexture("_FrontGrassTex", bp_f.terrainMaterial.GetTexture("_GrassTex"));
        cs.mr.material.SetTexture("_FrontWallTex", bp_f.terrainMaterial.GetTexture("_WallTex"));
        cs.mr.material.SetTexture("_RightFrontGrassTex", bp_fr.terrainMaterial.GetTexture("_GrassTex"));
        cs.mr.material.SetTexture("_RightFrontWallTex", bp_fr.terrainMaterial.GetTexture("_WallTex"));


    }

    #endregion

    #region water

    public void ReGenerateWaterMesh(ChunkScript cs, List<Vector3> modifiedPos)
    {
        if (cs.waterPointDictionary.Count == 0)
            return;
        int preVCount = cs.vertices_water.Count;
        int preTCount = cs.triangles_water.Count;
        cs.vertices_water.Clear();
        cs.triangles_water.Clear();
        cs.verticesRangeDictionary_water.Clear();

        List<Vector3> checkedPosList = new List<Vector3>();
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < width; z++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (!modifiedPos.Contains(new Vector3(x, y, z)))
                    {
                        WaterPointData wpd;
                        if (cs.waterPointDictionary.ContainsKey(new Vector3(x, y, z)))
                        {
                             wpd= cs.waterPointDictionary[new Vector3(x, y, z)];
                        }
                        else
                        {
                            wpd = new WaterPointData();
                            cs.waterPointDictionary.Add(new Vector3(x, y, z), wpd);
                            cs.wpdList.Add(wpd);
                        }
                        MarchWaterCubeWithExistingVertices(cs, wpd);
                    }
                    else
                    {
                        MarchWaterCube(cs, new Vector3(x,y,z), out bool need, checkedPosList, out checkedPosList);
                    }
                }
            }
        }

        BuildWaterMesh(cs, 0);

        //yield return null;
    }


    public void GenerateWaterMesh(ChunkScript cs, bool delay)
    {
        if (cs.waterData.Count == 0)
        {
            return;
        }


        StartCoroutine(GenerateWaterMesh_WaitForFinishedChunk(cs, delay));

    }

    IEnumerator GenerateWaterMesh_WaitForFinishedChunk(ChunkScript cs, bool delay)
    {
        yield return new WaitUntil(() =>

        cl.chunkDictionary.ContainsKey(cs.position + Vector2.right * wgPreset.chunkSize) &&
        cl.chunkDictionary.ContainsKey(cs.position + Vector2.left * wgPreset.chunkSize) &&
        cl.chunkDictionary.ContainsKey(cs.position + Vector2.up * wgPreset.chunkSize) &&
        cl.chunkDictionary.ContainsKey(cs.position + Vector2.down * wgPreset.chunkSize)


        );

        cs.rightChunk = cl.chunkDictionary[cs.position + Vector2.right * wgPreset.chunkSize].cs;
        cs.leftChunk = cl.chunkDictionary[cs.position + Vector2.left * wgPreset.chunkSize].cs;
        cs.frontChunk = cl.chunkDictionary[cs.position + Vector2.up * wgPreset.chunkSize].cs;
        cs.backChunk = cl.chunkDictionary[cs.position + Vector2.down * wgPreset.chunkSize].cs;



        List<Vector3> checkingPosList = new List<Vector3>();
        foreach (Vector3 v in cs.waterData)
        {
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    for (int z = -1; z < 2; z++)
                    {
                        if (!checkingPosList.Contains(v + new Vector3(x, y, z)) && 0 <= v.x + x && v.x + x < 8 && 0 <= v.z + z && v.z + z < 8)
                        {
                            checkingPosList.Add(v + new Vector3(x, y, z));
                        }
                    }
                }
            }
        }

        List<Vector3> checkedPosList = new List<Vector3>();

        foreach (Vector3 v in checkingPosList)
        {
            bool needDelay;
            MarchWaterCube(cs, v, out needDelay,  checkedPosList, out checkedPosList);
            if (delay)
            {
                yield return new WaitForSeconds(0.01f);
            }
        }

        //for (int x = 0; x < width; x++)
        //{
        //    for (int z = 0; z < width; z++)
        //    {
        //        for (int y = 0; y < height; y++)
        //        {
        //            if (!checkingPosList.Contains(new Vector3(x, y, z)))
        //            {
        //                WaterPointData wpd = new WaterPointData();
        //                wpd.vertices = new List<Vector3>();
        //                wpd.triangles = new List<int>();

        //                cs.waterPointDictionary.Add(new Vector3(x, y, z), wpd);
        //                cs.wpdList.Add(wpd);
        //            }
        //        }
        //    }
        //}


        BuildWaterMesh(cs, waterCollisionGenTime);
    }

    int GetCubeConfiguration_Water(bool[] cube)
    {
        int configurationIndex = 0;
        for (int i = 0; i < 8; i++)
        {
            if (!cube[i])
            {
                configurationIndex |= 1 << i;
            }
        }

        return configurationIndex;
    }

    bool SampleTerrainWater(ChunkScript cs, Vector3Int point)
    {


        //return cs.waterData.Contains(point);
        //return Random.Range(0, 10) * 0.1f;

        if (
            cs.waterData.Contains(point)

            ////next to water in chunk
            //|| cs.waterData.Contains(point + Vector3.right)
            //|| cs.waterData.Contains(point + Vector3.left)
            //|| cs.waterData.Contains(point + Vector3.forward)
            //|| cs.waterData.Contains(point + Vector3.back)

            ////next to water in other chunk
            //|| cl.chunkDictionary[cs.position + Vector2.right * wgPreset.chunkSize].cs.waterData.Contains(point + Vector3.left * wgPreset.chunkSize + Vector3.right)
            //|| cl.chunkDictionary[cs.position + Vector2.left * wgPreset.chunkSize].cs.waterData.Contains(point + Vector3.right * wgPreset.chunkSize + Vector3.left)
            //|| cl.chunkDictionary[cs.position + Vector2.up * wgPreset.chunkSize].cs.waterData.Contains(point + Vector3.back * wgPreset.chunkSize + Vector3.forward)
            //|| cl.chunkDictionary[cs.position + Vector2.down * wgPreset.chunkSize].cs.waterData.Contains(point + Vector3.forward * wgPreset.chunkSize + Vector3.back)

        )
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    void MarchWaterCube(ChunkScript cs, Vector3 position, out bool needDelay, List<Vector3> checkedPos, out List<Vector3> checkedPosNew)
    {
        checkedPosNew = checkedPos;
        if (cs.waterPointDictionary.ContainsKey(position))
        {
            cs.waterPointDictionary.Remove(position);
        }

        WaterPointData wpd = new WaterPointData();
        wpd.vertices = new List<Vector3>();
        wpd.triangles = new List<int>();


        bool[] cube = new bool[8];
        for (int i = 0; i < 8; i++)
        {
            Vector3 v = position + CornerTable[i];
            if (checkedPos.Contains(v))
            {
                cube[i] = true;
            }
            else
            {
                if (cs.waterData.Contains(v))
                {
                    cube[i] = true;
                    checkedPosNew.Add(v);
                }
                else
                {
                    cube[i] = false;
                }
            }
        }

        int configIndex = GetCubeConfiguration_Water(cube);
        if (configIndex == 0 || configIndex == 255)
        {
            cs.waterPointDictionary.Add(position, wpd);
            cs.wpdList.Add(wpd);
            needDelay = false;
            return;
        }
        needDelay = true;
        int edgeIndex = 0;
        for (int i = 0; i < 5; i++)
        {
            for (int n = 0; n < 3; n++)
            {
                int indice = TriangleTable[configIndex, edgeIndex];

                if (indice == -1)
                {
                    cs.waterPointDictionary.Add(position, wpd);
                    cs.wpdList.Add(wpd);
                    return;
                }

                Vector3 vert1 = position + CornerTable[EdgeIndexes[indice, 0]];
                Vector3 vert2 = position + CornerTable[EdgeIndexes[indice, 1]];


                Vector3 vertPosition;
                vertPosition = (vert1 + vert2) / 2f;
                bool notCrossed;
                int triangle = VertForIndice_Water(cs, vertPosition, out notCrossed);
                cs.triangles_water.Add(triangle);
                wpd.triangles.Add(triangle);
                wpd.vertices.Add(vertPosition);
                edgeIndex++;
            }
        }
        cs.waterPointDictionary.Add(position, wpd);
        cs.wpdList.Add(wpd);
    }
    void MarchWaterCubeWithExistingVertices(ChunkScript cs, WaterPointData wpd)
    {
        foreach(Vector3 v in wpd.vertices)
        {
            Vector3 vertPosition;
            vertPosition = v;
            bool notCrossed;

            int triangle = VertForIndice_Water(cs, vertPosition, out notCrossed);
            if (notCrossed)
            {
                cs.vertices_water.Add(vertPosition);
            }
            cs.triangles_water.Add(triangle);
        }
        
    }
    int VertForIndice_Water(ChunkScript cs, Vector3 vert, out bool notCrossed)
    {

        if (cs.verticesRangeDictionary_water.ContainsKey(vert))
        {
            notCrossed = false;
            return cs.verticesRangeDictionary_water[vert];
        }

        cs.vertices_water.Add(vert);
        AddWaterVerticesToDictionary(cs, vert);
        notCrossed = true;
        return cs.vertices_water.Count - 1;
    }

    void AddWaterVerticesToDictionary(ChunkScript cs, Vector3 pos)
    {
        cs.verticesRangeDictionary_water.Add(pos, cs.vertices_water.Count - 1);
    }




    public void BuildWaterMesh(ChunkScript cs, float collisionDelay)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = cs.vertices_water.ToArray();
        mesh.triangles = cs.triangles_water.ToArray();
        mesh.RecalculateNormals();
        cs.waterMF.mesh = mesh;
        cs.waterMC.sharedMesh  = mesh;
        if (cs.playerInWater)
        {
            cs.FlipWaterMesh();
        }
        //generate collision
        for (int i = 0; i < cs.waterCollisionParent.childCount; i++)
        {
            Destroy(cs.waterCollisionParent.GetChild(i).gameObject);
        }
        StartCoroutine(BuildWaterCollision(collisionDelay, cs));
    }
    IEnumerator BuildWaterCollision(float time, ChunkScript cs)
    {
        if (time != 0)
            yield return new WaitForSeconds(time);

        foreach (Vector3 v in cs.waterData)
        {
            GameObject b = Instantiate(waterColliderPrefab);
            b.transform.position = v + cs.waterObj.transform.position;
            b.transform.SetParent(cs.waterCollisionParent);
        }

    }





    #endregion



    #region lava

    public void ReGenerateLavaMesh(ChunkScript cs, List<Vector3> modifiedPos)
    {
        if (cs.lavaPointDictionary.Count == 0)
        {
            return;
        }
        int preVCount = cs.vertices_lava.Count;
        int preTCount = cs.triangles_lava.Count;
        cs.vertices_lava.Clear();
        cs.triangles_lava.Clear();
        cs.verticesRangeDictionary_lava.Clear();

        List<Vector3> checkedPosList = new List<Vector3>();//<Vector3, bool>();
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < width; z++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (!modifiedPos.Contains(new Vector3(x, y, z)))
                    {
                        WaterPointData lpd;
                        if (cs.lavaPointDictionary.ContainsKey(new Vector3(x, y, z)))
                        {
                            lpd = cs.lavaPointDictionary[new Vector3(x, y, z)];
                        }
                        else
                        {
                            lpd = new WaterPointData();
                            cs.lavaPointDictionary.Add(new Vector3(x, y, z), lpd);
                            cs.lpdList.Add(lpd);
                        }
                        MarchLavaCubeWithExistingVertices(cs, lpd);
                    }
                    else
                    {
                        MarchLavaCube(cs, new Vector3(x, y, z), out bool needDelay, checkedPosList, out checkedPosList);
                    }
                }
            }
        }

        BuildLavaMesh(cs, 0);

        //yield return null;
    }


    public void GenerateLavaMesh(ChunkScript cs, bool delay, bool colGen)
    {

        StartCoroutine(GenerateLavaMesh_WaitForFinishedChunk(cs, delay, colGen));

    }

    IEnumerator GenerateLavaMesh_WaitForFinishedChunk(ChunkScript cs, bool delay, bool colGen)
    {
        #region wait

        switch (cs.dimension)
        {
            case Dimension.OverWorld:
                yield return new WaitUntil(() =>

                cl.chunkDictionary.ContainsKey(cs.position + Vector2.right * wgPreset.chunkSize) &&
                cl.chunkDictionary.ContainsKey(cs.position + Vector2.left * wgPreset.chunkSize) &&
                cl.chunkDictionary.ContainsKey(cs.position + Vector2.up * wgPreset.chunkSize) &&
                cl.chunkDictionary.ContainsKey(cs.position + Vector2.down * wgPreset.chunkSize)


                );
                cs.rightChunk = cl.chunkDictionary[cs.position + Vector2.right * wgPreset.chunkSize].cs;
                cs.leftChunk = cl.chunkDictionary[cs.position + Vector2.left * wgPreset.chunkSize].cs;
                cs.frontChunk = cl.chunkDictionary[cs.position + Vector2.up * wgPreset.chunkSize].cs;
                cs.backChunk = cl.chunkDictionary[cs.position + Vector2.down * wgPreset.chunkSize].cs;
                break;
            case Dimension.Nether:
                yield return new WaitUntil(() =>

                cl.nether_chunkDictionary.ContainsKey(cs.position + Vector2.right * wgPreset.chunkSize) &&
                cl.nether_chunkDictionary.ContainsKey(cs.position + Vector2.left * wgPreset.chunkSize) &&
                cl.nether_chunkDictionary.ContainsKey(cs.position + Vector2.up * wgPreset.chunkSize) &&
                cl.nether_chunkDictionary.ContainsKey(cs.position + Vector2.down * wgPreset.chunkSize)


                );
                cs.rightChunk = cl.nether_chunkDictionary[cs.position + Vector2.right * wgPreset.chunkSize].cs;
                cs.leftChunk = cl.nether_chunkDictionary[cs.position + Vector2.left * wgPreset.chunkSize].cs;
                cs.frontChunk = cl.nether_chunkDictionary[cs.position + Vector2.up * wgPreset.chunkSize].cs;
                cs.backChunk = cl.nether_chunkDictionary[cs.position + Vector2.down * wgPreset.chunkSize].cs;
                break;
        }



        if (cs.lavaData.Count == 0)
        {
            yield break;
        }

        #endregion
        List<Vector3> checkingPosList = new List<Vector3>();
        foreach(Vector3 v in cs.lavaData)
        {
            for(int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    for (int z = -1; z < 2; z++)
                    {
                        if (!checkingPosList.Contains(v + new Vector3(x,y,z)) && 0 <= v.x + x && v.x+x<8 && 0 <= v.z + z && v.z + z < 8)
                        {
                            checkingPosList.Add(v + new Vector3(x,y,z));
                        }
                    }
                }
            }
        }

        //for (int x = 0; x < width; x++)
        //{
        //    for (int z = 0; z < width; z++)
        //    {
        //        for (int y = 0; y < height; y++)
        //        {
        //            if (!checkingPosList.Contains(new Vector3(x, y, z)))
        //            {

        //                cs.lavaPointDictionary.Add(new Vector3(x, y, z), new WaterPointData());
        //                cs.lpdList.Add(new WaterPointData());
        //                yield return new WaitForSeconds(0.3f);
        //            }
        //        }
        //    }
        //}
        List<Vector3> checkedPosList = new List<Vector3>();

        foreach (Vector3 v in checkingPosList)
        {
            bool needDelay;
            MarchLavaCube(cs, v, out needDelay, checkedPosList, out checkedPosList);
            if (delay && needDelay)
            {
                yield return new WaitForSeconds(0.01f);
            }
        }
        BuildLavaMesh(cs, colGen ? lavaCollisionGenTime : 0);
    }

    int GetCubeConfiguration_Lava(bool[] cube)
    {
        int configurationIndex = 0;
        for (int i = 0; i < 8; i++)
        {
            if (!cube[i])
            {
                configurationIndex |= 1 << i;
            }
        }

        return configurationIndex;
    }

    bool SampleTerrainLava(ChunkScript cs, Vector3Int point)
    {


        //return cs.waterData.Contains(point);
        //return Random.Range(0, 10) * 0.1f;

        if (
            cs.lavaData.Contains(point)

        ////next to water in chunk
        //|| cs.waterData.Contains(point + Vector3.right)
        //|| cs.waterData.Contains(point + Vector3.left)
        //|| cs.waterData.Contains(point + Vector3.forward)
        //|| cs.waterData.Contains(point + Vector3.back)

        ////next to water in other chunk
        //|| cl.chunkDictionary[cs.position + Vector2.right * wgPreset.chunkSize].cs.waterData.Contains(point + Vector3.left * wgPreset.chunkSize + Vector3.right)
        //|| cl.chunkDictionary[cs.position + Vector2.left * wgPreset.chunkSize].cs.waterData.Contains(point + Vector3.right * wgPreset.chunkSize + Vector3.left)
        //|| cl.chunkDictionary[cs.position + Vector2.up * wgPreset.chunkSize].cs.waterData.Contains(point + Vector3.back * wgPreset.chunkSize + Vector3.forward)
        //|| cl.chunkDictionary[cs.position + Vector2.down * wgPreset.chunkSize].cs.waterData.Contains(point + Vector3.forward * wgPreset.chunkSize + Vector3.back)

        )
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    void MarchLavaCube(ChunkScript cs, Vector3 position, out bool needDelay, List<Vector3> checkedPos, out List<Vector3> checkedPosNew)
    {
        checkedPosNew = checkedPos;
        if (cs.lavaPointDictionary.ContainsKey(position))
        {
            cs.lavaPointDictionary.Remove(position);
        }


        WaterPointData lpd = new WaterPointData();
        lpd.vertices = new List<Vector3>();
        lpd.triangles = new List<int>();


        bool[] cube = new bool[8];
        for (int i = 0; i < 8; i++)
        {
            Vector3 v = position + CornerTable[i];
            if (checkedPos.Contains(v))
            {
                cube[i] = true;
            }
            else
            {

                //cube[i] = false;
                if (cs.lavaData.Contains(v))
                {
                    cube[i] = true;
                    checkedPosNew.Add(v);
                }
                else
                {
                    cube[i] = false;
                }
            }
        }

        int configIndex = GetCubeConfiguration_Lava(cube);
        if (configIndex == 0 || configIndex == 255)
        {
            cs.lavaPointDictionary.Add(position, lpd);
            cs.lpdList.Add(lpd);
            needDelay = false;
            return;
        }
        needDelay = true;
        int edgeIndex = 0;
        for (int i = 0; i < 5; i++)
        {
            for (int n = 0; n < 3; n++)
            {
                int indice = TriangleTable[configIndex, edgeIndex];

                if (indice == -1)
                {
                    cs.lavaPointDictionary.Add(position, lpd);
                    cs.lpdList.Add(lpd);
                    return;
                }

                Vector3 vert1 = position + CornerTable[EdgeIndexes[indice, 0]];
                Vector3 vert2 = position + CornerTable[EdgeIndexes[indice, 1]];


                Vector3 vertPosition;
                vertPosition = (vert1 + vert2) / 2f;
                bool notCrossed;
                int triangle = VertForIndice_Lava(cs, vertPosition, out notCrossed);
                cs.triangles_lava.Add(triangle);
                lpd.triangles.Add(triangle);
                lpd.vertices.Add(vertPosition);
                //if (notCrossed)
                //{
                //}
                edgeIndex++;
            }
        }
        cs.lavaPointDictionary.Add(position, lpd);
        cs.lpdList.Add(lpd);

    }

    void MarchLavaCubeWithExistingVertices(ChunkScript cs, WaterPointData lpd)
    {
        foreach (Vector3 v in lpd.vertices)
        {
            Vector3 vertPosition;
            vertPosition = v;
            bool notCrossed;

            int triangle = VertForIndice_Lava(cs, vertPosition, out notCrossed);
            if (notCrossed)
            {
                cs.vertices_lava.Add(vertPosition);
            }
            cs.triangles_lava.Add(triangle);
        }

    }
    int VertForIndice_Lava(ChunkScript cs, Vector3 vert, out bool notCrossed)
    {

        if (cs.verticesRangeDictionary_lava.ContainsKey(vert))
        {
            notCrossed = false;
            return cs.verticesRangeDictionary_lava[vert];
        }

        cs.vertices_lava.Add(vert);
        AddLavaVerticesToDictionary(cs, vert);
        notCrossed = true;
        return cs.vertices_lava.Count - 1;
    }

    void AddLavaVerticesToDictionary(ChunkScript cs, Vector3 pos)
    {
        cs.verticesRangeDictionary_lava.Add(pos, cs.vertices_lava.Count - 1);
    }




    public void BuildLavaMesh(ChunkScript cs, float time)
    {
        Mesh mesh = new Mesh();
        mesh.vertices = cs.vertices_lava.ToArray();
        mesh.triangles = cs.triangles_lava.ToArray();
        mesh.RecalculateNormals();
        cs.lavaMF.mesh = mesh;
        cs.lavaMC.sharedMesh = mesh;


        if (cs.playerInLava)
        {
            cs.FlipLavaMesh();
        }

        //generate collision
        for (int i = 0; i < cs.lavaCollisionParent.childCount; i++)
        {
            Destroy(cs.lavaCollisionParent.GetChild(i).gameObject);
        }
        StartCoroutine(BuildLavaCollision(time, cs));
    }

    IEnumerator BuildLavaCollision(float time, ChunkScript cs)
    {
        if (time != 0)
            yield return new WaitForSeconds(time);

        foreach (Vector3 v in cs.lavaData)
        {
            GameObject b = Instantiate(lavaColliderPrefab);
            b.transform.position = v + cs.lavaObj.transform.position;
            b.transform.SetParent(cs.lavaCollisionParent);
        }
    }





    #endregion





    #region weird stuff

    Vector3Int[] CornerTable = new Vector3Int[8] {

        new Vector3Int(0, 0, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(1, 1, 0),
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, 0, 1),
        new Vector3Int(1, 0, 1),
        new Vector3Int(1, 1, 1),
        new Vector3Int(0, 1, 1)

    };

    int[] CornerTableX = new int[8] {

        0,1,1,0,0,1,1,0

    };

    int[] CornerTableY = new int[8] {

       0,0,1,1,0,0,1,1

    };
    int[] CornerTableZ = new int[8] {
        0,0,0,0,1,1,1,1

    };

    int[,] EdgeIndexes = new int[12, 2] {

        {0, 1}, {1, 2}, {3, 2}, {0, 3}, {4, 5}, {5, 6}, {7, 6}, {4, 7}, {0, 4}, {1, 5}, {2, 6}, {3, 7}

    };


    private int[,] TriangleTable = new int[,] {

        {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 1, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {1, 8, 3, 9, 8, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 8, 3, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {9, 2, 10, 0, 2, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {2, 8, 3, 2, 10, 8, 10, 9, 8, -1, -1, -1, -1, -1, -1, -1},
        {3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 11, 2, 8, 11, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {1, 9, 0, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {1, 11, 2, 1, 9, 11, 9, 8, 11, -1, -1, -1, -1, -1, -1, -1},
        {3, 10, 1, 11, 10, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 10, 1, 0, 8, 10, 8, 11, 10, -1, -1, -1, -1, -1, -1, -1},
        {3, 9, 0, 3, 11, 9, 11, 10, 9, -1, -1, -1, -1, -1, -1, -1},
        {9, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {4, 3, 0, 7, 3, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 1, 9, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {4, 1, 9, 4, 7, 1, 7, 3, 1, -1, -1, -1, -1, -1, -1, -1},
        {1, 2, 10, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {3, 4, 7, 3, 0, 4, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1},
        {9, 2, 10, 9, 0, 2, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1},
        {2, 10, 9, 2, 9, 7, 2, 7, 3, 7, 9, 4, -1, -1, -1, -1},
        {8, 4, 7, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {11, 4, 7, 11, 2, 4, 2, 0, 4, -1, -1, -1, -1, -1, -1, -1},
        {9, 0, 1, 8, 4, 7, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1},
        {4, 7, 11, 9, 4, 11, 9, 11, 2, 9, 2, 1, -1, -1, -1, -1},
        {3, 10, 1, 3, 11, 10, 7, 8, 4, -1, -1, -1, -1, -1, -1, -1},
        {1, 11, 10, 1, 4, 11, 1, 0, 4, 7, 11, 4, -1, -1, -1, -1},
        {4, 7, 8, 9, 0, 11, 9, 11, 10, 11, 0, 3, -1, -1, -1, -1},
        {4, 7, 11, 4, 11, 9, 9, 11, 10, -1, -1, -1, -1, -1, -1, -1},
        {9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {9, 5, 4, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 5, 4, 1, 5, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {8, 5, 4, 8, 3, 5, 3, 1, 5, -1, -1, -1, -1, -1, -1, -1},
        {1, 2, 10, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {3, 0, 8, 1, 2, 10, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1},
        {5, 2, 10, 5, 4, 2, 4, 0, 2, -1, -1, -1, -1, -1, -1, -1},
        {2, 10, 5, 3, 2, 5, 3, 5, 4, 3, 4, 8, -1, -1, -1, -1},
        {9, 5, 4, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 11, 2, 0, 8, 11, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1},
        {0, 5, 4, 0, 1, 5, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1},
        {2, 1, 5, 2, 5, 8, 2, 8, 11, 4, 8, 5, -1, -1, -1, -1},
        {10, 3, 11, 10, 1, 3, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1},
        {4, 9, 5, 0, 8, 1, 8, 10, 1, 8, 11, 10, -1, -1, -1, -1},
        {5, 4, 0, 5, 0, 11, 5, 11, 10, 11, 0, 3, -1, -1, -1, -1},
        {5, 4, 8, 5, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1},
        {9, 7, 8, 5, 7, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {9, 3, 0, 9, 5, 3, 5, 7, 3, -1, -1, -1, -1, -1, -1, -1},
        {0, 7, 8, 0, 1, 7, 1, 5, 7, -1, -1, -1, -1, -1, -1, -1},
        {1, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {9, 7, 8, 9, 5, 7, 10, 1, 2, -1, -1, -1, -1, -1, -1, -1},
        {10, 1, 2, 9, 5, 0, 5, 3, 0, 5, 7, 3, -1, -1, -1, -1},
        {8, 0, 2, 8, 2, 5, 8, 5, 7, 10, 5, 2, -1, -1, -1, -1},
        {2, 10, 5, 2, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1},
        {7, 9, 5, 7, 8, 9, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1},
        {9, 5, 7, 9, 7, 2, 9, 2, 0, 2, 7, 11, -1, -1, -1, -1},
        {2, 3, 11, 0, 1, 8, 1, 7, 8, 1, 5, 7, -1, -1, -1, -1},
        {11, 2, 1, 11, 1, 7, 7, 1, 5, -1, -1, -1, -1, -1, -1, -1},
        {9, 5, 8, 8, 5, 7, 10, 1, 3, 10, 3, 11, -1, -1, -1, -1},
        {5, 7, 0, 5, 0, 9, 7, 11, 0, 1, 0, 10, 11, 10, 0, -1},
        {11, 10, 0, 11, 0, 3, 10, 5, 0, 8, 0, 7, 5, 7, 0, -1},
        {11, 10, 5, 7, 11, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 8, 3, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {9, 0, 1, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {1, 8, 3, 1, 9, 8, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1},
        {1, 6, 5, 2, 6, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {1, 6, 5, 1, 2, 6, 3, 0, 8, -1, -1, -1, -1, -1, -1, -1},
        {9, 6, 5, 9, 0, 6, 0, 2, 6, -1, -1, -1, -1, -1, -1, -1},
        {5, 9, 8, 5, 8, 2, 5, 2, 6, 3, 2, 8, -1, -1, -1, -1},
        {2, 3, 11, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {11, 0, 8, 11, 2, 0, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1},
        {0, 1, 9, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1},
        {5, 10, 6, 1, 9, 2, 9, 11, 2, 9, 8, 11, -1, -1, -1, -1},
        {6, 3, 11, 6, 5, 3, 5, 1, 3, -1, -1, -1, -1, -1, -1, -1},
        {0, 8, 11, 0, 11, 5, 0, 5, 1, 5, 11, 6, -1, -1, -1, -1},
        {3, 11, 6, 0, 3, 6, 0, 6, 5, 0, 5, 9, -1, -1, -1, -1},
        {6, 5, 9, 6, 9, 11, 11, 9, 8, -1, -1, -1, -1, -1, -1, -1},
        {5, 10, 6, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {4, 3, 0, 4, 7, 3, 6, 5, 10, -1, -1, -1, -1, -1, -1, -1},
        {1, 9, 0, 5, 10, 6, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1},
        {10, 6, 5, 1, 9, 7, 1, 7, 3, 7, 9, 4, -1, -1, -1, -1},
        {6, 1, 2, 6, 5, 1, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1},
        {1, 2, 5, 5, 2, 6, 3, 0, 4, 3, 4, 7, -1, -1, -1, -1},
        {8, 4, 7, 9, 0, 5, 0, 6, 5, 0, 2, 6, -1, -1, -1, -1},
        {7, 3, 9, 7, 9, 4, 3, 2, 9, 5, 9, 6, 2, 6, 9, -1},
        {3, 11, 2, 7, 8, 4, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1},
        {5, 10, 6, 4, 7, 2, 4, 2, 0, 2, 7, 11, -1, -1, -1, -1},
        {0, 1, 9, 4, 7, 8, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1},
        {9, 2, 1, 9, 11, 2, 9, 4, 11, 7, 11, 4, 5, 10, 6, -1},
        {8, 4, 7, 3, 11, 5, 3, 5, 1, 5, 11, 6, -1, -1, -1, -1},
        {5, 1, 11, 5, 11, 6, 1, 0, 11, 7, 11, 4, 0, 4, 11, -1},
        {0, 5, 9, 0, 6, 5, 0, 3, 6, 11, 6, 3, 8, 4, 7, -1},
        {6, 5, 9, 6, 9, 11, 4, 7, 9, 7, 11, 9, -1, -1, -1, -1},
        {10, 4, 9, 6, 4, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {4, 10, 6, 4, 9, 10, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1},
        {10, 0, 1, 10, 6, 0, 6, 4, 0, -1, -1, -1, -1, -1, -1, -1},
        {8, 3, 1, 8, 1, 6, 8, 6, 4, 6, 1, 10, -1, -1, -1, -1},
        {1, 4, 9, 1, 2, 4, 2, 6, 4, -1, -1, -1, -1, -1, -1, -1},
        {3, 0, 8, 1, 2, 9, 2, 4, 9, 2, 6, 4, -1, -1, -1, -1},
        {0, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {8, 3, 2, 8, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1},
        {10, 4, 9, 10, 6, 4, 11, 2, 3, -1, -1, -1, -1, -1, -1, -1},
        {0, 8, 2, 2, 8, 11, 4, 9, 10, 4, 10, 6, -1, -1, -1, -1},
        {3, 11, 2, 0, 1, 6, 0, 6, 4, 6, 1, 10, -1, -1, -1, -1},
        {6, 4, 1, 6, 1, 10, 4, 8, 1, 2, 1, 11, 8, 11, 1, -1},
        {9, 6, 4, 9, 3, 6, 9, 1, 3, 11, 6, 3, -1, -1, -1, -1},
        {8, 11, 1, 8, 1, 0, 11, 6, 1, 9, 1, 4, 6, 4, 1, -1},
        {3, 11, 6, 3, 6, 0, 0, 6, 4, -1, -1, -1, -1, -1, -1, -1},
        {6, 4, 8, 11, 6, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {7, 10, 6, 7, 8, 10, 8, 9, 10, -1, -1, -1, -1, -1, -1, -1},
        {0, 7, 3, 0, 10, 7, 0, 9, 10, 6, 7, 10, -1, -1, -1, -1},
        {10, 6, 7, 1, 10, 7, 1, 7, 8, 1, 8, 0, -1, -1, -1, -1},
        {10, 6, 7, 10, 7, 1, 1, 7, 3, -1, -1, -1, -1, -1, -1, -1},
        {1, 2, 6, 1, 6, 8, 1, 8, 9, 8, 6, 7, -1, -1, -1, -1},
        {2, 6, 9, 2, 9, 1, 6, 7, 9, 0, 9, 3, 7, 3, 9, -1},
        {7, 8, 0, 7, 0, 6, 6, 0, 2, -1, -1, -1, -1, -1, -1, -1},
        {7, 3, 2, 6, 7, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {2, 3, 11, 10, 6, 8, 10, 8, 9, 8, 6, 7, -1, -1, -1, -1},
        {2, 0, 7, 2, 7, 11, 0, 9, 7, 6, 7, 10, 9, 10, 7, -1},
        {1, 8, 0, 1, 7, 8, 1, 10, 7, 6, 7, 10, 2, 3, 11, -1},
        {11, 2, 1, 11, 1, 7, 10, 6, 1, 6, 7, 1, -1, -1, -1, -1},
        {8, 9, 6, 8, 6, 7, 9, 1, 6, 11, 6, 3, 1, 3, 6, -1},
        {0, 9, 1, 11, 6, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {7, 8, 0, 7, 0, 6, 3, 11, 0, 11, 6, 0, -1, -1, -1, -1},
        {7, 11, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {3, 0, 8, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 1, 9, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {8, 1, 9, 8, 3, 1, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1},
        {10, 1, 2, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {1, 2, 10, 3, 0, 8, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1},
        {2, 9, 0, 2, 10, 9, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1},
        {6, 11, 7, 2, 10, 3, 10, 8, 3, 10, 9, 8, -1, -1, -1, -1},
        {7, 2, 3, 6, 2, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {7, 0, 8, 7, 6, 0, 6, 2, 0, -1, -1, -1, -1, -1, -1, -1},
        {2, 7, 6, 2, 3, 7, 0, 1, 9, -1, -1, -1, -1, -1, -1, -1},
        {1, 6, 2, 1, 8, 6, 1, 9, 8, 8, 7, 6, -1, -1, -1, -1},
        {10, 7, 6, 10, 1, 7, 1, 3, 7, -1, -1, -1, -1, -1, -1, -1},
        {10, 7, 6, 1, 7, 10, 1, 8, 7, 1, 0, 8, -1, -1, -1, -1},
        {0, 3, 7, 0, 7, 10, 0, 10, 9, 6, 10, 7, -1, -1, -1, -1},
        {7, 6, 10, 7, 10, 8, 8, 10, 9, -1, -1, -1, -1, -1, -1, -1},
        {6, 8, 4, 11, 8, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {3, 6, 11, 3, 0, 6, 0, 4, 6, -1, -1, -1, -1, -1, -1, -1},
        {8, 6, 11, 8, 4, 6, 9, 0, 1, -1, -1, -1, -1, -1, -1, -1},
        {9, 4, 6, 9, 6, 3, 9, 3, 1, 11, 3, 6, -1, -1, -1, -1},
        {6, 8, 4, 6, 11, 8, 2, 10, 1, -1, -1, -1, -1, -1, -1, -1},
        {1, 2, 10, 3, 0, 11, 0, 6, 11, 0, 4, 6, -1, -1, -1, -1},
        {4, 11, 8, 4, 6, 11, 0, 2, 9, 2, 10, 9, -1, -1, -1, -1},
        {10, 9, 3, 10, 3, 2, 9, 4, 3, 11, 3, 6, 4, 6, 3, -1},
        {8, 2, 3, 8, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1},
        {0, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {1, 9, 0, 2, 3, 4, 2, 4, 6, 4, 3, 8, -1, -1, -1, -1},
        {1, 9, 4, 1, 4, 2, 2, 4, 6, -1, -1, -1, -1, -1, -1, -1},
        {8, 1, 3, 8, 6, 1, 8, 4, 6, 6, 10, 1, -1, -1, -1, -1},
        {10, 1, 0, 10, 0, 6, 6, 0, 4, -1, -1, -1, -1, -1, -1, -1},
        {4, 6, 3, 4, 3, 8, 6, 10, 3, 0, 3, 9, 10, 9, 3, -1},
        {10, 9, 4, 6, 10, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {4, 9, 5, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 8, 3, 4, 9, 5, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1},
        {5, 0, 1, 5, 4, 0, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1},
        {11, 7, 6, 8, 3, 4, 3, 5, 4, 3, 1, 5, -1, -1, -1, -1},
        {9, 5, 4, 10, 1, 2, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1},
        {6, 11, 7, 1, 2, 10, 0, 8, 3, 4, 9, 5, -1, -1, -1, -1},
        {7, 6, 11, 5, 4, 10, 4, 2, 10, 4, 0, 2, -1, -1, -1, -1},
        {3, 4, 8, 3, 5, 4, 3, 2, 5, 10, 5, 2, 11, 7, 6, -1},
        {7, 2, 3, 7, 6, 2, 5, 4, 9, -1, -1, -1, -1, -1, -1, -1},
        {9, 5, 4, 0, 8, 6, 0, 6, 2, 6, 8, 7, -1, -1, -1, -1},
        {3, 6, 2, 3, 7, 6, 1, 5, 0, 5, 4, 0, -1, -1, -1, -1},
        {6, 2, 8, 6, 8, 7, 2, 1, 8, 4, 8, 5, 1, 5, 8, -1},
        {9, 5, 4, 10, 1, 6, 1, 7, 6, 1, 3, 7, -1, -1, -1, -1},
        {1, 6, 10, 1, 7, 6, 1, 0, 7, 8, 7, 0, 9, 5, 4, -1},
        {4, 0, 10, 4, 10, 5, 0, 3, 10, 6, 10, 7, 3, 7, 10, -1},
        {7, 6, 10, 7, 10, 8, 5, 4, 10, 4, 8, 10, -1, -1, -1, -1},
        {6, 9, 5, 6, 11, 9, 11, 8, 9, -1, -1, -1, -1, -1, -1, -1},
        {3, 6, 11, 0, 6, 3, 0, 5, 6, 0, 9, 5, -1, -1, -1, -1},
        {0, 11, 8, 0, 5, 11, 0, 1, 5, 5, 6, 11, -1, -1, -1, -1},
        {6, 11, 3, 6, 3, 5, 5, 3, 1, -1, -1, -1, -1, -1, -1, -1},
        {1, 2, 10, 9, 5, 11, 9, 11, 8, 11, 5, 6, -1, -1, -1, -1},
        {0, 11, 3, 0, 6, 11, 0, 9, 6, 5, 6, 9, 1, 2, 10, -1},
        {11, 8, 5, 11, 5, 6, 8, 0, 5, 10, 5, 2, 0, 2, 5, -1},
        {6, 11, 3, 6, 3, 5, 2, 10, 3, 10, 5, 3, -1, -1, -1, -1},
        {5, 8, 9, 5, 2, 8, 5, 6, 2, 3, 8, 2, -1, -1, -1, -1},
        {9, 5, 6, 9, 6, 0, 0, 6, 2, -1, -1, -1, -1, -1, -1, -1},
        {1, 5, 8, 1, 8, 0, 5, 6, 8, 3, 8, 2, 6, 2, 8, -1},
        {1, 5, 6, 2, 1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {1, 3, 6, 1, 6, 10, 3, 8, 6, 5, 6, 9, 8, 9, 6, -1},
        {10, 1, 0, 10, 0, 6, 9, 5, 0, 5, 6, 0, -1, -1, -1, -1},
        {0, 3, 8, 5, 6, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {10, 5, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {11, 5, 10, 7, 5, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {11, 5, 10, 11, 7, 5, 8, 3, 0, -1, -1, -1, -1, -1, -1, -1},
        {5, 11, 7, 5, 10, 11, 1, 9, 0, -1, -1, -1, -1, -1, -1, -1},
        {10, 7, 5, 10, 11, 7, 9, 8, 1, 8, 3, 1, -1, -1, -1, -1},
        {11, 1, 2, 11, 7, 1, 7, 5, 1, -1, -1, -1, -1, -1, -1, -1},
        {0, 8, 3, 1, 2, 7, 1, 7, 5, 7, 2, 11, -1, -1, -1, -1},
        {9, 7, 5, 9, 2, 7, 9, 0, 2, 2, 11, 7, -1, -1, -1, -1},
        {7, 5, 2, 7, 2, 11, 5, 9, 2, 3, 2, 8, 9, 8, 2, -1},
        {2, 5, 10, 2, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1},
        {8, 2, 0, 8, 5, 2, 8, 7, 5, 10, 2, 5, -1, -1, -1, -1},
        {9, 0, 1, 5, 10, 3, 5, 3, 7, 3, 10, 2, -1, -1, -1, -1},
        {9, 8, 2, 9, 2, 1, 8, 7, 2, 10, 2, 5, 7, 5, 2, -1},
        {1, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 8, 7, 0, 7, 1, 1, 7, 5, -1, -1, -1, -1, -1, -1, -1},
        {9, 0, 3, 9, 3, 5, 5, 3, 7, -1, -1, -1, -1, -1, -1, -1},
        {9, 8, 7, 5, 9, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {5, 8, 4, 5, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1},
        {5, 0, 4, 5, 11, 0, 5, 10, 11, 11, 3, 0, -1, -1, -1, -1},
        {0, 1, 9, 8, 4, 10, 8, 10, 11, 10, 4, 5, -1, -1, -1, -1},
        {10, 11, 4, 10, 4, 5, 11, 3, 4, 9, 4, 1, 3, 1, 4, -1},
        {2, 5, 1, 2, 8, 5, 2, 11, 8, 4, 5, 8, -1, -1, -1, -1},
        {0, 4, 11, 0, 11, 3, 4, 5, 11, 2, 11, 1, 5, 1, 11, -1},
        {0, 2, 5, 0, 5, 9, 2, 11, 5, 4, 5, 8, 11, 8, 5, -1},
        {9, 4, 5, 2, 11, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {2, 5, 10, 3, 5, 2, 3, 4, 5, 3, 8, 4, -1, -1, -1, -1},
        {5, 10, 2, 5, 2, 4, 4, 2, 0, -1, -1, -1, -1, -1, -1, -1},
        {3, 10, 2, 3, 5, 10, 3, 8, 5, 4, 5, 8, 0, 1, 9, -1},
        {5, 10, 2, 5, 2, 4, 1, 9, 2, 9, 4, 2, -1, -1, -1, -1},
        {8, 4, 5, 8, 5, 3, 3, 5, 1, -1, -1, -1, -1, -1, -1, -1},
        {0, 4, 5, 1, 0, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {8, 4, 5, 8, 5, 3, 9, 0, 5, 0, 3, 5, -1, -1, -1, -1},
        {9, 4, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {4, 11, 7, 4, 9, 11, 9, 10, 11, -1, -1, -1, -1, -1, -1, -1},
        {0, 8, 3, 4, 9, 7, 9, 11, 7, 9, 10, 11, -1, -1, -1, -1},
        {1, 10, 11, 1, 11, 4, 1, 4, 0, 7, 4, 11, -1, -1, -1, -1},
        {3, 1, 4, 3, 4, 8, 1, 10, 4, 7, 4, 11, 10, 11, 4, -1},
        {4, 11, 7, 9, 11, 4, 9, 2, 11, 9, 1, 2, -1, -1, -1, -1},
        {9, 7, 4, 9, 11, 7, 9, 1, 11, 2, 11, 1, 0, 8, 3, -1},
        {11, 7, 4, 11, 4, 2, 2, 4, 0, -1, -1, -1, -1, -1, -1, -1},
        {11, 7, 4, 11, 4, 2, 8, 3, 4, 3, 2, 4, -1, -1, -1, -1},
        {2, 9, 10, 2, 7, 9, 2, 3, 7, 7, 4, 9, -1, -1, -1, -1},
        {9, 10, 7, 9, 7, 4, 10, 2, 7, 8, 7, 0, 2, 0, 7, -1},
        {3, 7, 10, 3, 10, 2, 7, 4, 10, 1, 10, 0, 4, 0, 10, -1},
        {1, 10, 2, 8, 7, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {4, 9, 1, 4, 1, 7, 7, 1, 3, -1, -1, -1, -1, -1, -1, -1},
        {4, 9, 1, 4, 1, 7, 0, 8, 1, 8, 7, 1, -1, -1, -1, -1},
        {4, 0, 3, 7, 4, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {4, 8, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {9, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {3, 0, 9, 3, 9, 11, 11, 9, 10, -1, -1, -1, -1, -1, -1, -1},
        {0, 1, 10, 0, 10, 8, 8, 10, 11, -1, -1, -1, -1, -1, -1, -1},
        {3, 1, 10, 11, 3, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {1, 2, 11, 1, 11, 9, 9, 11, 8, -1, -1, -1, -1, -1, -1, -1},
        {3, 0, 9, 3, 9, 11, 1, 2, 9, 2, 11, 9, -1, -1, -1, -1},
        {0, 2, 11, 8, 0, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {3, 2, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {2, 3, 8, 2, 8, 10, 10, 8, 9, -1, -1, -1, -1, -1, -1, -1},
        {9, 10, 2, 0, 9, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {2, 3, 8, 2, 8, 10, 0, 1, 8, 1, 10, 8, -1, -1, -1, -1},
        {1, 10, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {1, 3, 8, 9, 1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 9, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {0, 3, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
        {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}

    };

    #endregion
}
