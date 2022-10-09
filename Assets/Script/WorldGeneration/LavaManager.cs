using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaManager : MonoBehaviour
{
    public UniversalScriptManager usm;
    MeshGenerator mg;
    ChunkLoader cl;
    WaterManager wm;
    DimensionTransportationManager dtm;
    WorldGenerationPreset wgPreset;
    int chunkSize;
    public float updateTick;
    public float drainSpeed;
    public AnimationCurve drainSpeedByDistanceCurve;
    public float pourSpeed;
    public AnimationCurve pourSpeedByDistanceCurve;

    int addedModifiedChunkCount = 0;
    
    public List<ChunkScript> modifiedChunkList = new List<ChunkScript>();
    public List<ChunkScript> chunkToUpdateList = new List<ChunkScript>();
    public int lavaFireTime;

    [Header("Lava Update")]
    public float fallSpeed;
    public float spreadSpeed;
    public float drySpeed;


    void Awake()
    {
        mg = usm.meshGenerator;
        cl = usm.chunkLoader;
        wm = usm.waterManager;
        dtm = usm.dimensionTransportationManager;

        wgPreset = usm.worldGenerationPreset;
        chunkSize = wgPreset.chunkSize;
        //StartCoroutine(UpdateLava_Tick());
    }

    public void GenerateLava(ChunkScript cs, bool delay, bool colGen)
    {

        mg.GenerateLavaMesh(cs, delay, colGen);
    }
    public void GenerateLava(ChunkScript cs, float delay)
    {

        StartCoroutine(GenerateLava_Delay(cs, delay));
    }
    IEnumerator GenerateLava_Delay(ChunkScript cs, float delay)
    {

        yield return new WaitForSeconds(delay);
        GenerateLava(cs, true, true);

    }


    //public IEnumerator UpdateLava_Tick()
    //{
    //    while (true)
    //    {
    //        for (int i = 0; i < modifiedChunkDataKeys.Count - addedModifiedChunkCount; i++)
    //        {
    //            ChunkScript key = modifiedChunkDataKeys[i];
    //            UpdatedChunkData ucd = modifiedChunksDataDictionary[key];


    //            modifiedChunkDataKeys.RemoveAt(i);
    //            modifiedChunksDataDictionary.Remove(key);
    //            i--;


    //            UpdateLava(ucd.cs, new List<Vector3>(), i);
    //            mg.ReGenerateLavaMesh(ucd.cs, ucd.modifiedPoses);
    //        }
    //        addedModifiedChunkCount = 0;
    //        yield return new WaitForSeconds(updateTick);
    //        yield return new WaitUntil(() => modifiedChunkDataKeys.Count > 0);
    //    }
    //}

    //public void UpdateLava(ChunkScript cs, List<Vector3> previouslyModifiedPosition, int chunkIndex)
    //{
    //    //updating lava is currently disabled due to big update


    //    cs.lavaBeingModified = false;
    //    return;

    //    ////disabled or not ready
    //    //if (!cs.activated || cs.rightChunk == null)
    //    //{
    //    //    cs.lavaBeingModified = false;
    //    //    return;
    //    //}

    //    //cs.lavaBeingModified = true;


    //    //bool modified = false;
    //    //List<Vector3> modifiedLavaData = cs.lavaData;
    //    //List<Vector3> modifiedPos = new List<Vector3>();
    //    //List<Vector3> addedPos = new List<Vector3>();
    //    //foreach (Vector3 v in previouslyModifiedPosition)
    //    //{
    //    //    modifiedPos.Add(v);
    //    //}

    //    //int count = modifiedLavaData.Count;


    //    //List<ChunkScript> effectedChunks = new List<ChunkScript>();

    //    //void CheckEffectingOtherChunks(Vector3 v)
    //    //{
    //    //    bool crossedRight = false;
    //    //    bool crossedLeft = false;
    //    //    bool crossedFront = false;
    //    //    bool crossedBack = false;

    //    //    if (v.x >= 8)
    //    //    {
    //    //        crossedRight = true;
    //    //    }
    //    //    else if (v.x <= 0)
    //    //    {
    //    //        crossedLeft = true;
    //    //    }
    //    //    if (v.z >= 8)
    //    //    {
    //    //        crossedFront = true;
    //    //    }
    //    //    else if (v.z <= 0)
    //    //    {
    //    //        crossedBack = true;
    //    //    }
    //    //    if (crossedRight && crossedFront && !effectedChunks.Contains(cs.rightChunk.frontChunk))
    //    //    {
    //    //        effectedChunks.Add(cs.rightChunk.frontChunk);
    //    //    }
    //    //    else if (crossedRight && crossedBack && !effectedChunks.Contains(cs.rightChunk.backChunk))
    //    //    {
    //    //        effectedChunks.Add(cs.rightChunk.backChunk);
    //    //    }
    //    //    else if (crossedLeft && crossedFront && !effectedChunks.Contains(cs.leftChunk.frontChunk))
    //    //    {
    //    //        effectedChunks.Add(cs.leftChunk.frontChunk);
    //    //    }
    //    //    else if (crossedLeft && crossedBack && !effectedChunks.Contains(cs.leftChunk.backChunk))
    //    //    {
    //    //        effectedChunks.Add(cs.leftChunk.backChunk);
    //    //    }
    //    //    if (crossedRight && !effectedChunks.Contains(cs.rightChunk))
    //    //    {
    //    //        effectedChunks.Add(cs.rightChunk);
    //    //    }
    //    //    else if (crossedLeft && !effectedChunks.Contains(cs.leftChunk))
    //    //    {
    //    //        effectedChunks.Add(cs.leftChunk);
    //    //    }
    //    //    if (crossedFront && !effectedChunks.Contains(cs.frontChunk))
    //    //    {
    //    //        effectedChunks.Add(cs.frontChunk);
    //    //    }
    //    //    else if (crossedBack && !effectedChunks.Contains(cs.backChunk))
    //    //    {
    //    //        effectedChunks.Add(cs.backChunk);
    //    //    }
    //    //}

    //    //for (int i = 0; i < count; i++)
    //    //{
    //    //    Vector3 pos = modifiedLavaData[i];
    //    //    bool goDown = false;

    //    //    bool hasWater_R = false;
    //    //    bool hasWaterBottom_R = false;
    //    //    bool hasGround_R = false;
    //    //    bool hasGroundBottom_R = false;
    //    //    bool hasWater_L = false;
    //    //    bool hasWaterBottom_L = false;
    //    //    bool hasGround_L = false;
    //    //    bool hasGroundBottom_L = false;
    //    //    bool hasWater_F = false;
    //    //    bool hasWaterBottom_F = false;
    //    //    bool hasGround_F = false;
    //    //    bool hasGroundBottom_F = false;
    //    //    bool hasWater_B = false;
    //    //    bool hasWaterBottom_B = false;
    //    //    bool hasGround_B = false;
    //    //    bool hasGroundBottom_B = false;
    //    //    bool hasLavaDown = modifiedLavaData.Contains(pos + Vector3.down);
    //    //    bool separated = false;

    //    //    bool onGround(Vector3 checkPos)
    //    //    {
    //    //        if (cs.terrainMap[(int)checkPos.x, (int)checkPos.y - 1, (int)checkPos.z] <= mg.terrainSuface)
    //    //        {
    //    //            return true;
    //    //        }
    //    //        if (cs.HasBlockAt(new Vector3Int((int)checkPos.x, (int)checkPos.y - 1, (int)checkPos.z)))
    //    //        {
    //    //            return true;
    //    //        }



    //    //        if (cs.rightChunk.HasBlockAt((new Vector3Int((int)checkPos.x - 8, (int)checkPos.y - 1, (int)checkPos.z))))
    //    //        {
    //    //            return true;
    //    //        }
    //    //        if (cs.leftChunk.HasBlockAt((new Vector3Int((int)checkPos.x + 8, (int)checkPos.y - 1, (int)checkPos.z))))
    //    //        {
    //    //            return true;
    //    //        }
    //    //        if (cs.frontChunk.HasBlockAt((new Vector3Int((int)checkPos.x, (int)checkPos.y - 1, (int)checkPos.z - 8))))
    //    //        {
    //    //            return true;
    //    //        }
    //    //        if (cs.backChunk.HasBlockAt((new Vector3Int((int)checkPos.x, (int)checkPos.y - 1, (int)checkPos.z + 8))))
    //    //        {
    //    //            return true;
    //    //        }
    //    //        if (cs.rightChunk.frontChunk.HasBlockAt((new Vector3Int((int)checkPos.x - 8, (int)checkPos.y - 1, (int)checkPos.z - 8))))
    //    //        {
    //    //            return true;
    //    //        }
    //    //        if (cs.rightChunk.backChunk.HasBlockAt((new Vector3Int((int)checkPos.x - 8, (int)checkPos.y - 1, (int)checkPos.z + 8))))
    //    //        {
    //    //            return true;
    //    //        }
    //    //        if (cs.leftChunk.frontChunk.HasBlockAt((new Vector3Int((int)checkPos.x + 8, (int)checkPos.y - 1, (int)checkPos.z - 8))))
    //    //        {
    //    //            return true;
    //    //        }
    //    //        if (cs.leftChunk.backChunk.HasBlockAt((new Vector3Int((int)checkPos.x + 8, (int)checkPos.y - 1, (int)checkPos.z + 8))))
    //    //        {
    //    //            return true;
    //    //        }
    //    //        return false;
    //    //    }

    //    //    if (hasLavaDown)
    //    //    {
    //    //        //onWater = true;
    //    //    }
    //    //    else
    //    //    {
    //    //        if (pos.y > 1)
    //    //        {
    //    //            if (!onGround(pos))
    //    //            {
    //    //                goDown = true;
    //    //            }
    //    //            else
    //    //            {
    //    //                //onGround = true;
    //    //            }
    //    //        }
    //    //        else
    //    //        {
    //    //            //onGround = true;
    //    //        }
    //    //    }

    //    //    if (pos.x == 0)
    //    //    {
    //    //        hasWater_R = modifiedLavaData.Contains(pos + Vector3.right);
    //    //        hasWaterBottom_R = modifiedLavaData.Contains(pos + Vector3.right + Vector3.down);

    //    //        hasGround_R = cs.terrainMap[(int)pos.x + 1, (int)pos.y, (int)pos.z] <= mg.terrainSuface;
    //    //        hasGroundBottom_R = pos.y == 0 ? true : cs.terrainMap[(int)pos.x + 1, (int)pos.y - 1, (int)pos.z] <= mg.terrainSuface;


    //    //        hasWater_L = cs.leftChunk.lavaData.Contains(new Vector3(wgPreset.chunkSize - 1, pos.y, pos.z));
    //    //        hasWaterBottom_L = cs.leftChunk.lavaData.Contains(new Vector3(wgPreset.chunkSize - 1, pos.y - 1, pos.z));

    //    //        hasGround_L = cs.leftChunk.terrainMap[wgPreset.chunkSize - 1, (int)pos.y, (int)pos.z] <= mg.terrainSuface;
    //    //        hasGroundBottom_L = pos.y == 0 ? true : cs.leftChunk.terrainMap[wgPreset.chunkSize - 1, (int)pos.y - 1, (int)pos.z] <= mg.terrainSuface;
    //    //    }
    //    //    else if (pos.x == wgPreset.chunkSize)
    //    //    {
    //    //        hasWater_R = cs.rightChunk.lavaData.Contains(new Vector3(1, pos.y, pos.z));
    //    //        hasWaterBottom_R = cs.rightChunk.lavaData.Contains(new Vector3(1, pos.y - 1, pos.z));

    //    //        hasGround_R = cs.rightChunk.terrainMap[1, (int)pos.y, (int)pos.z] <= mg.terrainSuface;
    //    //        hasGroundBottom_R = pos.y == 0 ? true : cs.rightChunk.terrainMap[1, (int)pos.y - 1, (int)pos.z] <= mg.terrainSuface;


    //    //        hasWater_L = modifiedLavaData.Contains(pos + Vector3.left);
    //    //        hasWater_L = modifiedLavaData.Contains(pos + Vector3.left + Vector3.down);

    //    //        hasGround_L = cs.terrainMap[(int)pos.x - 1, (int)pos.y, (int)pos.z] <= mg.terrainSuface;
    //    //        hasGroundBottom_L = pos.y == 0 ? true : cs.terrainMap[(int)pos.x - 1, (int)pos.y - 1, (int)pos.z] <= mg.terrainSuface;
    //    //    }
    //    //    else
    //    //    {
    //    //        hasWater_R = modifiedLavaData.Contains(pos + Vector3.right);
    //    //        hasWaterBottom_R = modifiedLavaData.Contains(pos + Vector3.right + Vector3.down);

    //    //        hasGround_R = cs.terrainMap[(int)pos.x + 1, (int)pos.y, (int)pos.z] <= mg.terrainSuface;
    //    //        hasGroundBottom_R = pos.y == 0 ? true : cs.terrainMap[(int)pos.x + 1, (int)pos.y - 1, (int)pos.z] <= mg.terrainSuface;


    //    //        hasWater_L = modifiedLavaData.Contains(pos + Vector3.left);
    //    //        hasWaterBottom_L = modifiedLavaData.Contains(pos + Vector3.left + Vector3.down);

    //    //        hasGround_L = cs.terrainMap[(int)pos.x - 1, (int)pos.y, (int)pos.z] <= mg.terrainSuface;
    //    //        hasGroundBottom_L = pos.y == 0 ? true : cs.terrainMap[(int)pos.x - 1, (int)pos.y - 1, (int)pos.z] <= mg.terrainSuface;
    //    //    }


    //    //    if (pos.z == 0)
    //    //    {
    //    //        hasWater_F = modifiedLavaData.Contains(pos + Vector3.forward);
    //    //        hasWaterBottom_F = modifiedLavaData.Contains(pos + Vector3.forward + Vector3.down);

    //    //        hasGround_F = cs.terrainMap[(int)pos.x, (int)pos.y, (int)pos.z + 1] <= mg.terrainSuface;
    //    //        hasGroundBottom_F = pos.y == 0 ? true : cs.terrainMap[(int)pos.x, (int)pos.y - 1, (int)pos.z + 1] <= mg.terrainSuface;


    //    //        hasWater_B = cs.backChunk.lavaData.Contains(new Vector3(pos.x, pos.y, wgPreset.chunkSize - 1));
    //    //        hasWaterBottom_B = cs.backChunk.lavaData.Contains(new Vector3(pos.x, pos.y - 1, wgPreset.chunkSize - 1));


    //    //        hasGround_B = cs.backChunk.terrainMap[(int)pos.x, (int)pos.y, wgPreset.chunkSize - 1] <= mg.terrainSuface;
    //    //        hasGroundBottom_B = pos.y == 0 ? true : cs.backChunk.terrainMap[(int)pos.x, (int)pos.y - 1, wgPreset.chunkSize - 1] <= mg.terrainSuface;
    //    //    }
    //    //    else if (pos.z == wgPreset.chunkSize)
    //    //    {
    //    //        hasWater_F = cs.frontChunk.lavaData.Contains(new Vector3(pos.x, pos.y, 1));
    //    //        hasWaterBottom_F = cs.frontChunk.lavaData.Contains(new Vector3(pos.x, pos.y - 1, 1));

    //    //        hasGround_F = cs.frontChunk.terrainMap[(int)pos.x, (int)pos.y, 1] <= mg.terrainSuface;
    //    //        hasGroundBottom_F = pos.y == 0 ? true : cs.frontChunk.terrainMap[(int)pos.x, (int)pos.y - 1, 1] <= mg.terrainSuface;


    //    //        hasWater_B = modifiedLavaData.Contains(pos + Vector3.back);
    //    //        hasWaterBottom_B = modifiedLavaData.Contains(pos + Vector3.back + Vector3.down);

    //    //        hasGround_B = cs.terrainMap[(int)pos.x, (int)pos.y, (int)pos.z - 1] <= mg.terrainSuface;
    //    //        hasGroundBottom_B = pos.y == 0 ? true : cs.terrainMap[(int)pos.x, (int)pos.y - 1, (int)pos.z - 1] <= mg.terrainSuface;
    //    //    }
    //    //    else
    //    //    {
    //    //        hasWater_F = modifiedLavaData.Contains(pos + Vector3.forward);
    //    //        hasWaterBottom_F = modifiedLavaData.Contains(pos + Vector3.forward + Vector3.down);

    //    //        hasGround_F = cs.terrainMap[(int)pos.x, (int)pos.y, (int)pos.z + 1] <= mg.terrainSuface;
    //    //        hasGroundBottom_F = pos.y == 0 ? true : cs.terrainMap[(int)pos.x, (int)pos.y - 1, (int)pos.z + 1] <= mg.terrainSuface;


    //    //        hasWater_B = modifiedLavaData.Contains(pos + Vector3.back);
    //    //        hasWaterBottom_B = modifiedLavaData.Contains(pos + Vector3.back + Vector3.down);

    //    //        hasGround_B = cs.terrainMap[(int)pos.x, (int)pos.y, (int)pos.z - 1] <= mg.terrainSuface;
    //    //        hasGroundBottom_B = pos.y == 0 ? true : cs.terrainMap[(int)pos.x, (int)pos.y - 1, (int)pos.z - 1] <= mg.terrainSuface;
    //    //    }


    //    //    if (hasLavaDown && !hasWater_R && !hasWater_L && !hasWater_F && !hasWater_B)
    //    //    {
    //    //        separated = true;
    //    //    }

    //    //    //go down
    //    //    if (goDown)
    //    //    {

    //    //        modifiedLavaData.Add(pos + Vector3.down);
    //    //        modifiedLavaData.RemoveAt(i);
    //    //        addedPos.Add(pos + Vector3.down);
    //    //        i--;
    //    //        count--;
    //    //        modified = true;

    //    //        modifiedPos.Add(pos + Vector3.down);
    //    //        modifiedPos.Add(pos);


    //    //        CheckEffectingOtherChunks(pos);
    //    //    }


    //    //    #region spread stuff(currently disabled)
    //    //    //else //spread
    //    //    //{
    //    //    //    //check directions able to spread
    //    //    //    bool canSpread_R = !hasWater_R && !hasGround_R && !hasGroundBottom_R && !hasWaterBottom_R;
    //    //    //    bool canSpread_L = !hasWater_L && !hasGround_L && !hasGroundBottom_L && !hasWaterBottom_L;
    //    //    //    bool canSpread_F = !hasWater_F && !hasGround_F && !hasGroundBottom_F && !hasWaterBottom_F;
    //    //    //    bool canSpread_B = !hasWater_B && !hasGround_B && !hasGroundBottom_B && !hasWaterBottom_B;

    //    //    //    List<Vector3> movableDirections = new List<Vector3>();
    //    //    //    if (canSpread_R && pos.x > 0 && pos.x < wgPreset.chunkSize - 1)
    //    //    //    {
    //    //    //        movableDirections.Add(Vector3.right);
    //    //    //    }
    //    //    //    if (canSpread_L && pos.x > 1 && pos.x < wgPreset.chunkSize)
    //    //    //    {
    //    //    //        movableDirections.Add(Vector3.left);
    //    //    //    }
    //    //    //    if (canSpread_F && pos.z > 0 && pos.z < wgPreset.chunkSize - 1)
    //    //    //    {
    //    //    //        movableDirections.Add(Vector3.forward);
    //    //    //    }
    //    //    //    if (canSpread_B && pos.z > 1 && pos.z < wgPreset.chunkSize)
    //    //    //    {
    //    //    //        movableDirections.Add(Vector3.back);
    //    //    //    }
    //    //    //    if (movableDirections.Count != 0)
    //    //    //    {
    //    //    //        modified = true;



    //    //    //        Vector3 dir = movableDirections[Random.Range(0, movableDirections.Count)];
    //    //    //        Vector3 target = pos + dir;

    //    //    //        modifiedWaterData.RemoveAt(i);
    //    //    //        i--;
    //    //    //        count--;
    //    //    //        modifiedPos.Add(pos);
    //    //    //        modifiedWaterData.Add(target);

    //    //    //        modifiedPos.Add(target);
    //    //    //    }
    //    //    //}
    //    //    #endregion
    //    //}

    //    //int modifiedPosCount = modifiedPos.Count;
    //    //for (int i = 0; i < modifiedPosCount; i++)
    //    //{
    //    //    Vector3 pos = modifiedPos[i];

    //    //    foreach (Vector3 v in surroundingTable)
    //    //    {
    //    //        Vector3 t = pos + v;
    //    //        if (!modifiedPos.Contains(t) && t.x <= wgPreset.chunkSize && t.x >= 0 && t.z <= wgPreset.chunkSize && t.z >= 0 && t.y >= 0 && t.y <= wgPreset.maxHeight)
    //    //        {
    //    //            modifiedPos.Add(t);
    //    //        }
    //    //    }
    //    //}

    //    //if (modified)
    //    //{
    //    //    if (cs.dimension != Dimension.Nether)
    //    //    {
    //    //        bool touchWater = ConflictionWithWater(cs, modifiedLavaData).Count != 0;
    //    //        if (touchWater)
    //    //        {
    //    //            List<Vector3> conf = ConflictionWithWater(cs, modifiedLavaData);
    //    //            foreach (Vector3 v in conf)
    //    //            {
    //    //                modifiedLavaData.Remove(v);
    //    //                cs.waterData.Remove(v);
    //    //                wm.PlaceObsidian(cs, v);
    //    //            }
    //    //            if (wm.modifiedChunkDataKeys.Contains(cs))
    //    //            {
    //    //                foreach (Vector3 v in SpecialFeatures.CoverListWithWalls(conf))
    //    //                {
    //    //                    if (!wm.modifiedChunksDataDictionary[cs].modifiedPoses.Contains(v))
    //    //                        wm.modifiedChunksDataDictionary[cs].modifiedPoses.Add(v);
    //    //                }
    //    //            }
    //    //            else
    //    //            {
    //    //                wm.modifiedChunksDataDictionary.Add(cs, new UpdatedChunkData { cs = cs, modifiedPoses = SpecialFeatures.CoverListWithWalls(conf) });
    //    //                wm.modifiedChunkDataKeys.Add(cs);
    //    //            }
    //    //        }
    //    //    }
    //    //    cs.lavaData = modifiedLavaData;
    //    //    modifiedChunksDataDictionary.Add(cs, new UpdatedChunkData { cs = cs, modifiedPoses = modifiedPos });
    //    //    modifiedChunkDataKeys.Add(cs);
    //    //    addedModifiedChunkCount++;
    //    //    foreach (ChunkScript other in effectedChunks)
    //    //    {
    //    //        if (!modifiedChunksDataDictionary.ContainsKey(other))
    //    //        {
    //    //            modifiedChunksDataDictionary.Add(other, new UpdatedChunkData { cs = other, modifiedPoses = new List<Vector3>() });
    //    //            modifiedChunkDataKeys.Add(other);
    //    //            addedModifiedChunkCount++;
    //    //        }
    //    //        else
    //    //        {
    //    //        }
    //    //    }
    //    //}
    //    //else
    //    //{
    //    //    cs.lavaBeingModified = false;
    //    //}

    //}


    void LateUpdate()
    {
        UpdateModifiedChunks();
    }
    void UpdateModifiedChunks()
    {
        for(int i =0; i < chunkToUpdateList.Count; i++)
        {
            ChunkScript cs = chunkToUpdateList[i];
            //update lava data
            bool updated;
            UpdateLavaData(cs, out updated);
            if (updated)
            {
                AddChunkToModifiedList(cs);
            }
        }
        chunkToUpdateList.Clear();
        foreach (ChunkScript cs in modifiedChunkList)
        {
            Debug.Log(cs.name);
            cs.RegenerateLavaMesh();
            chunkToUpdateList.Add(cs);
        }
        modifiedChunkList.Clear();
    }

    public void UpdateLavaData(ChunkScript cs, out bool updated)
    {
        updated = false;
        for(int x = 0; x < cs.lavaData.GetLength(0); x++)
        {
            for (int y = 0; y < cs.lavaData.GetLength(1); y++)
            {
                for (int z = 0; z < cs.lavaData.GetLength(2); z++)
                {
                    if (cs.lavaData[x, y, z] <= mg.terrainSuface)
                    {
                        //Debug.Log("checked");
                        Vector3Int pos = new Vector3Int(x, y, z);
                        Vector3Int pos_global = new Vector3Int(x, y, z) + new Vector3Int((int)cs.position.x, 0, (int)cs.position.y);
                        if (CanPlaceLavaAt(cs, pos + Vector3Int.down))
                        {
                            //fall
                            updated = true;
                            ModifyWorldLavaData(pos_global, fallSpeed * Time.deltaTime);
                            ModifyWorldLavaData(pos_global + Vector3Int.down, -fallSpeed * Time.deltaTime * 2);
                        }
                        else if (HasGroundAt(cs, pos))
                        {
                            //dry
                            int point = 0;
                            if (HasLavaAt(cs, pos + Vector3Int.right))
                                point++;
                            if (HasLavaAt(cs, pos + Vector3Int.left))
                                point++;
                            if (HasLavaAt(cs, pos + new Vector3Int(0, 0, 1)))
                                point++;
                            if (HasLavaAt(cs, pos + new Vector3Int(0, 0, -1)))
                                point++;
                            if (point <= 2)
                            {

                                updated = true;
                                ModifyWorldLavaData(pos_global, drySpeed * Time.deltaTime);

                            }



                            #region spread(disabled)
                            //spread
                            //List<Vector3Int> spreadingPoses = new List<Vector3Int>();
                            //if (CanPlaceLavaAt(cs, pos + Vector3Int.right))
                            //{
                            //    spreadingPoses.Add(pos + Vector3Int.right);
                            //}
                            //if (CanPlaceLavaAt(cs, pos + Vector3Int.left))
                            //{
                            //    spreadingPoses.Add(pos + Vector3Int.left);
                            //}
                            //if (CanPlaceLavaAt(cs, pos + new Vector3Int(0, 0, 1)))
                            //{
                            //    spreadingPoses.Add(pos + new Vector3Int(0, 0, 1));
                            //}
                            //if (CanPlaceLavaAt(cs, pos + new Vector3Int(0, 0, -1)))
                            //{
                            //    spreadingPoses.Add(pos + new Vector3Int(0, 0, -1));
                            //}
                            //if (CanPlaceLavaAt(cs, pos + new Vector3Int(1, 0, 1)))
                            //{
                            //    spreadingPoses.Add(pos + new Vector3Int(1, 0, 1));
                            //}
                            //if (CanPlaceLavaAt(cs, pos + new Vector3Int(1, 0, -1)))
                            //{
                            //    spreadingPoses.Add(pos + new Vector3Int(1, 0, -1));
                            //}
                            //if (CanPlaceLavaAt(cs, pos + new Vector3Int(-1, 0, 1)))
                            //{
                            //    spreadingPoses.Add(pos + new Vector3Int(-1, 0, 1));
                            //}
                            //if (CanPlaceLavaAt(cs, pos + new Vector3Int(-1, 0, -1)))
                            //{
                            //    spreadingPoses.Add(pos + new Vector3Int(-1, 0, -1));
                            //}
                            //float speed = -spreadSpeed / spreadingPoses.Count * Time.deltaTime;
                            //if (spreadingPoses.Count != 0)
                            //{
                            //    ModifyWorldLavaData(pos + new Vector3Int((int)cs.position.x, 0, (int)cs.position.y), -speed * 3);
                            //}
                            //foreach (Vector3Int v in spreadingPoses)
                            //{
                            //    ModifyWorldLavaData(v + new Vector3Int((int)cs.position.x, 0, (int)cs.position.y), speed);
                            //}
                            #endregion
                        }




                    }
                    else
                    {
                        //Debug.Log("didn't checked");
                    }
                    cs.lavaData[x, y, z] = Mathf.Clamp(cs.lavaData[x, y, z], -1, 1);

                }
            }
        }
    }
    bool CanPlaceLavaAt(ChunkScript cs, Vector3Int localPos)
    {
        while(localPos.x<0 || localPos.x>=chunkSize || localPos.z <0 || localPos.z >= chunkSize)
        {
            if (localPos.x < 0)
            {
                cs = cs.leftChunk;
                localPos += new Vector3Int(chunkSize, 0, 0);
            }
            else if (localPos.x >= chunkSize)
            {
                cs = cs.rightChunk;
                localPos += new Vector3Int(-chunkSize, 0, 0);
            }
            if (localPos.z < 0)
            {
                cs = cs.backChunk;
                localPos += new Vector3Int(0, 0, chunkSize);
            }
            else if (localPos.z >= chunkSize)
            {
                cs = cs.frontChunk;
                localPos += new Vector3Int(0, 0, -chunkSize);
            }

        }




        if (localPos.y <= 0)
            return false;
        if (cs.terrainMap[localPos.x, localPos.y + 1, localPos.z] <= mg.terrainSuface)
            return false;
        if (cs.lavaData[localPos.x, localPos.y, localPos.z] <= mg.terrainSuface - 0.2f)
            return false;
        if (cs.HasBlockAt(localPos))
            return false;




        return true;

    }
    bool HasLavaAt(ChunkScript cs, Vector3Int localPos)
    {

        while (localPos.x < 0 || localPos.x >= chunkSize || localPos.z < 0 || localPos.z >= chunkSize)
        {
            if (localPos.x < 0)
            {
                cs = cs.leftChunk;
                localPos += new Vector3Int(chunkSize, 0, 0);
            }
            else if (localPos.x >= chunkSize)
            {
                cs = cs.rightChunk;
                localPos += new Vector3Int(-chunkSize, 0, 0);
            }
            if (localPos.z < 0)
            {
                cs = cs.backChunk;
                localPos += new Vector3Int(0, 0, chunkSize);
            }
            else if (localPos.z >= chunkSize)
            {
                cs = cs.frontChunk;
                localPos += new Vector3Int(0, 0, -chunkSize);
            }

        }
        return cs.lavaData[localPos.x, localPos.y, localPos.z] <= mg.terrainSuface;


    }
    bool HasGroundAt(ChunkScript cs, Vector3Int localPos)
    {
        while (localPos.x < 0 || localPos.x >= chunkSize || localPos.z < 0 || localPos.z >= chunkSize)
        {
            if (localPos.x < 0)
            {
                cs = cs.leftChunk;
                localPos += new Vector3Int(chunkSize, 0, 0);
            }
            else if (localPos.x >= chunkSize)
            {
                cs = cs.rightChunk;
                localPos += new Vector3Int(-chunkSize, 0, 0);
            }
            if (localPos.z < 0)
            {
                cs = cs.backChunk;
                localPos += new Vector3Int(0, 0, chunkSize);
            }
            else if (localPos.z >= chunkSize)
            {
                cs = cs.frontChunk;
                localPos += new Vector3Int(0, 0, -chunkSize);
            }

        }
        return cs.terrainMap[localPos.x, localPos.y, localPos.z] <= mg.terrainSuface;

    }
    void ModifyWorldLavaData(Vector3Int pos, float addingValue)
    {
        Dictionary<Vector2, ChunkProperties> modifyingDictionary = dtm.currentDimesnion == Dimension.OverWorld ? cl.chunkDictionary : cl.nether_chunkDictionary;

        Vector3Int modifyingPos = pos;
        Vector2 chunkPos = new Vector2(modifyingPos.x >= 0 ? Mathf.Floor(modifyingPos.x / chunkSize) * chunkSize : -Mathf.Floor((-modifyingPos.x) / chunkSize) * chunkSize - chunkSize,
            modifyingPos.z >= 0 ? Mathf.Floor(modifyingPos.z / chunkSize) * chunkSize : -Mathf.Floor((-modifyingPos.z) / chunkSize) * chunkSize - chunkSize);

        ChunkScript modifyingCs = modifyingDictionary[chunkPos].cs;
        Vector3Int mp_loc = modifyingPos - Vector3Int.FloorToInt(new Vector3(chunkPos.x, 0, chunkPos.y));

        //check if it collides with terrain
        if (modifyingCs.terrainMap[mp_loc.x, mp_loc.y + 1, mp_loc.z] <= mg.terrainSuface)
        {
            Debug.Log("coll");
            //collided with terrain
        }
        else if (modifyingCs.HasBlockAt(new Vector3Int(mp_loc.x, mp_loc.y + 1, mp_loc.z)))
        {
            Debug.Log("coll");
            //collided with block
        }
        else
        {
            modifyingCs.lavaData[mp_loc.x, mp_loc.y, mp_loc.z] += addingValue;
            AddChunkToModifiedList(modifyingCs);
            if (mp_loc.x == 0)
            {
                modifyingCs.leftChunk.lavaData[chunkSize, mp_loc.y, mp_loc.z] += addingValue;
                AddChunkToModifiedList(modifyingCs.leftChunk);
                if (mp_loc.z == 0)
                {
                    modifyingCs.leftChunk.backChunk.lavaData[chunkSize, mp_loc.y, chunkSize] += addingValue;
                    AddChunkToModifiedList(modifyingCs.leftChunk.backChunk);
                }
                else if (mp_loc.z == chunkSize)
                {
                    modifyingCs.leftChunk.frontChunk.lavaData[chunkSize, mp_loc.y, 0] += addingValue;
                    AddChunkToModifiedList(modifyingCs.leftChunk.frontChunk);
                }
            }
            else if (mp_loc.x == chunkSize)
            {
                modifyingCs.rightChunk.lavaData[0, mp_loc.y, mp_loc.z] += addingValue;
                AddChunkToModifiedList(modifyingCs.rightChunk);
                if (mp_loc.z == 0)
                {
                    modifyingCs.rightChunk.backChunk.lavaData[0, mp_loc.y, chunkSize] += addingValue;
                    AddChunkToModifiedList(modifyingCs.rightChunk.backChunk);
                }
                else if (mp_loc.z == chunkSize)
                {
                    modifyingCs.rightChunk.frontChunk.lavaData[0, mp_loc.y, 0] += addingValue;
                    AddChunkToModifiedList(modifyingCs.rightChunk.frontChunk);
                }
            }
            if (mp_loc.z == 0)
            {
                modifyingCs.backChunk.lavaData[mp_loc.x, mp_loc.y, chunkSize] += addingValue;
                AddChunkToModifiedList(modifyingCs.backChunk);
            }
            else if (mp_loc.z == chunkSize)
            {
                modifyingCs.frontChunk.lavaData[mp_loc.x, mp_loc.y, 0] += addingValue;
                AddChunkToModifiedList(modifyingCs.frontChunk);
            }
        }
    }
    void ModifyLavaDataInChunk(ChunkScript cs, Vector3Int pos, float addingValue)
    {

        cs.lavaData[pos.x, pos.y, pos.z] += addingValue;
        if (pos.x == 0)
        {
            cs.leftChunk.lavaData[chunkSize, pos.y, pos.z] += addingValue;
            if (pos.z == 0)
            {
                cs.leftChunk.backChunk.lavaData[chunkSize, pos.y, chunkSize] += addingValue;
            }
            else if (pos.z == chunkSize)
            {
                cs.leftChunk.frontChunk.lavaData[chunkSize, pos.y, 0] += addingValue;
            }
        }
        else if (pos.x == chunkSize)
        {
            cs.rightChunk.lavaData[0, pos.y, pos.z] += addingValue;
            if (pos.z == 0)
            {
                cs.rightChunk.backChunk.lavaData[0, pos.y, chunkSize] += addingValue;
            }
            else if (pos.z == chunkSize)
            {
                cs.rightChunk.frontChunk.lavaData[0, pos.y, 0] += addingValue;
            }
        }
        if (pos.z == 0)
        {
            cs.backChunk.lavaData[pos.x, pos.y, chunkSize] += addingValue;
        }
        else if (pos.z == chunkSize)
        {
            cs.frontChunk.lavaData[pos.x, pos.y, 0] += addingValue;
        }

        if (!chunkToUpdateList.Contains(cs))
        {
            chunkToUpdateList.Add(cs);
        }
    }


    void AddChunkToModifiedList(ChunkScript cs)
    {
        if (!modifiedChunkList.Contains(cs))
            modifiedChunkList.Add(cs);
    }

    public IEnumerator DrainLava(ChunkScript cs, Vector3 drainPos, float drainDistance, float delay)
    {
        #region modify terrain

        int chunkSize = wgPreset.chunkSize;
        List<ChunkScript> modifiedChunks = new List<ChunkScript>();
        Vector3 curDestroyingPosition = drainPos;
        Dictionary<Vector2, ChunkProperties> modifyingDictionary = dtm.currentDimesnion == Dimension.OverWorld ? cl.chunkDictionary : cl.nether_chunkDictionary;

        for (float x = -drainDistance; x <= drainDistance; x++)
        {
            for (float z = -drainDistance; z <= drainDistance; z++)
            {
                for (float y = -drainDistance; y <= drainDistance; y++)
                {
                    if (y + curDestroyingPosition.y >= 0)
                    {
                        float dis = Mathf.Sqrt(x * x + y * y + z * z);
                        if (dis <= drainDistance)
                        {

                            float power = drainDistance * drainSpeedByDistanceCurve.Evaluate(dis / drainDistance);

                            Vector3Int modifyingPos = Vector3Int.FloorToInt(curDestroyingPosition) + new Vector3Int((int)x, (int)y, (int)z);
                            Vector2 chunkPos = new Vector2(modifyingPos.x >= 0 ? Mathf.Floor(modifyingPos.x / chunkSize) * chunkSize : -Mathf.Floor((-modifyingPos.x) / chunkSize) * chunkSize - chunkSize,
                                modifyingPos.z >= 0 ? Mathf.Floor(modifyingPos.z / chunkSize) * chunkSize : -Mathf.Floor((-modifyingPos.z) / chunkSize) * chunkSize - chunkSize);
                            ChunkScript modifyingCs = modifyingDictionary[chunkPos].cs;
                            Vector3Int mp_loc = modifyingPos - Vector3Int.FloorToInt(new Vector3(chunkPos.x, 0, chunkPos.y));
                            modifyingCs.lavaData[mp_loc.x, mp_loc.y, mp_loc.z] += power;
                            AddChunkToModifiedList(modifyingCs);
                            if (mp_loc.x == 0)
                            {
                                modifyingCs.leftChunk.lavaData[chunkSize, mp_loc.y, mp_loc.z] += power;
                                AddChunkToModifiedList(modifyingCs.leftChunk);
                                if (mp_loc.z == 0)
                                {
                                    modifyingCs.leftChunk.backChunk.lavaData[chunkSize, mp_loc.y, chunkSize] += power;
                                    AddChunkToModifiedList(modifyingCs.leftChunk.backChunk);
                                }
                                else if (mp_loc.z == chunkSize)
                                {
                                    modifyingCs.leftChunk.frontChunk.lavaData[chunkSize, mp_loc.y, 0] += power;
                                    AddChunkToModifiedList(modifyingCs.leftChunk.frontChunk);
                                }
                            }
                            else if (mp_loc.x == chunkSize)
                            {
                                modifyingCs.rightChunk.lavaData[0, mp_loc.y, mp_loc.z] += power;
                                AddChunkToModifiedList(modifyingCs.rightChunk);
                                if (mp_loc.z == 0)
                                {
                                    modifyingCs.rightChunk.backChunk.lavaData[0, mp_loc.y, chunkSize] += power;
                                    AddChunkToModifiedList(modifyingCs.rightChunk.backChunk);
                                }
                                else if (mp_loc.z == chunkSize)
                                {
                                    modifyingCs.rightChunk.frontChunk.lavaData[0, mp_loc.y, 0] += power;
                                    AddChunkToModifiedList(modifyingCs.rightChunk.frontChunk);
                                }
                            }
                            if (mp_loc.z == 0)
                            {
                                modifyingCs.backChunk.lavaData[mp_loc.x, mp_loc.y, chunkSize] += power;
                                AddChunkToModifiedList(modifyingCs.backChunk);
                            }
                            else if (mp_loc.z == chunkSize)
                            {
                                modifyingCs.frontChunk.lavaData[mp_loc.x, mp_loc.y, 0] += power;
                                AddChunkToModifiedList(modifyingCs.frontChunk);
                            }
                            if (!modifiedChunks.Contains(modifyingCs))
                            {
                                modifiedChunks.Add(modifyingCs);
                            }
                        }
                    }
                }
            }
        }



        #endregion
        yield break;

        #region old code;


        //draining lava is currently disabled due to big update

        ////set variables
        //int removedCount = 0;
        //List<Vector3> preLavaList = new List<Vector3>(cs.lavaData);
        //drainPos -= new Vector3(cs.position.x, 0, cs.position.y);
        //List<Vector3> removedPoses = new List<Vector3>();

        ////remove water from data
        //foreach (Vector3 v in preLavaList)
        //{
        //    if (Vector3.Distance(v, drainPos) <= drainDistance)
        //    {
        //        cs.lavaData.Remove(v);
        //        removedCount++;
        //        removedPoses.Add(v);
        //    }
        //}
        //drainPos += new Vector3(cs.position.x, 0, cs.position.y);

        ////apply removal
        //if (removedCount != 0)
        //{

        //    //get modified poses
        //    List<Vector3> modifiedPoses = SpecialFeatures.CoverListWithWalls(removedPoses);


        //    //check crossing parts
        //    bool crossRight = false;
        //    bool crossLeft = false;
        //    bool crossFront = false;
        //    bool crossBack = false;

        //    foreach (Vector3 v in removedPoses)
        //    {
        //        if (v.x >= 8)
        //        {
        //            crossRight = true;
        //        }
        //        else if (v.x <= 0)
        //        {
        //            crossLeft = true;
        //        }
        //        if (v.z >= 8)
        //        {
        //            crossFront = true;
        //        }
        //        else if (v.z <= 0)
        //        {
        //            crossBack = true;
        //        }
        //    }


        //    yield return new WaitForSeconds(delay);

        //    //effect neighborhood chunks
        //    #region effect neighborhood chunks
        //    if (crossRight && crossFront)
        //    {
        //        StartCoroutine(DrainLava(cs.rightChunk.frontChunk, drainPos, drainDistance, delay));
        //    }
        //    if (crossRight && crossBack)
        //    {
        //        StartCoroutine(DrainLava(cs.rightChunk.backChunk, drainPos, drainDistance, delay));
        //    }
        //    if (crossLeft && crossFront)
        //    {
        //        StartCoroutine(DrainLava(cs.leftChunk.frontChunk, drainPos, drainDistance, delay));
        //    }
        //    if (crossLeft && crossBack)
        //    {
        //        StartCoroutine(DrainLava(cs.leftChunk.backChunk, drainPos, drainDistance, delay));
        //    }

        //    if (crossRight)
        //    {
        //        StartCoroutine(DrainLava(cs.rightChunk, drainPos, drainDistance, delay));
        //    }
        //    if (crossLeft)
        //    {
        //        StartCoroutine(DrainLava(cs.leftChunk, drainPos, drainDistance, delay));
        //    }

        //    if (crossFront)
        //    {
        //        StartCoroutine(DrainLava(cs.frontChunk, drainPos, drainDistance, delay));
        //    }
        //    if (crossBack)
        //    {
        //        StartCoroutine(DrainLava(cs.backChunk, drainPos, drainDistance, delay));
        //    }
        //    #endregion






        //    //apply
        //    if (modifiedChunksDataDictionary.ContainsKey(cs))
        //    {
        //        foreach (Vector3 m in modifiedPoses)
        //        {
        //            if (!modifiedChunksDataDictionary[cs].modifiedPoses.Contains(m))
        //            {
        //                modifiedChunksDataDictionary[cs].modifiedPoses.Add(m);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        modifiedChunkDataKeys.Add(cs);
        //        modifiedChunksDataDictionary.Add(cs, new UpdatedChunkData { cs = cs, modifiedPoses = modifiedPoses });
        //    }
        //}
#endregion
    }
    List<Vector3> ConflictionWithWater(ChunkScript cs, List<Vector3> input)
    {
        List<Vector3> result = new List<Vector3>();
        foreach (Vector3 v in input)
        {
            if (cs.waterData.Contains(v))
            {
                result.Add(v);
            }
        }
        return result;
    }


    public IEnumerator PourLava(Vector3 pourPos, float pourDistance, float delay)
    {
        #region modify terrain

        Vector3 curPouringPosition = pourPos;
        Dictionary<Vector2, ChunkProperties> modifyingDictionary = dtm.currentDimesnion == Dimension.OverWorld ? cl.chunkDictionary : cl.nether_chunkDictionary;

        for (float x = -pourDistance; x <= pourDistance; x++)
        {
            for (float z = -pourDistance; z <= pourDistance; z++)
            {
                for (float y = -pourDistance; y <= pourDistance; y++)
                {
                    if (y + curPouringPosition.y >= 0)
                    {
                        float dis = Mathf.Sqrt(x * x + y * y + z * z);
                        if (dis <= pourDistance)
                        {
                            float power = pourSpeed * pourSpeedByDistanceCurve.Evaluate(dis / pourDistance);

                            Vector3Int modifyingPos = Vector3Int.FloorToInt(curPouringPosition) + new Vector3Int((int)x, (int)y, (int)z);
                            Vector2 chunkPos = new Vector2(modifyingPos.x >= 0 ? Mathf.Floor(modifyingPos.x / chunkSize) * chunkSize : -Mathf.Floor((-modifyingPos.x) / chunkSize) * chunkSize - chunkSize,
                                modifyingPos.z >= 0 ? Mathf.Floor(modifyingPos.z / chunkSize) * chunkSize : -Mathf.Floor((-modifyingPos.z) / chunkSize) * chunkSize - chunkSize);
                            ChunkScript cs = modifyingDictionary[chunkPos].cs;
                            Vector3Int mp_loc = modifyingPos - Vector3Int.FloorToInt(new Vector3(chunkPos.x, 0, chunkPos.y));


                            //check if it collides with terrain
                            if (cs.terrainMap[mp_loc.x, mp_loc.y, mp_loc.z] <= mg.terrainSuface)
                            {
                                //collided with terrain
                            }
                            else if(cs.HasBlockAt(new Vector3Int(mp_loc.x, mp_loc.y, mp_loc.z)))
                            {
                                //collided with block
                            }
                            else
                            {
                                cs.lavaData[mp_loc.x, mp_loc.y, mp_loc.z] -= power;
                                AddChunkToModifiedList(cs);
                                if (mp_loc.x == 0)
                                {
                                    cs.leftChunk.lavaData[chunkSize, mp_loc.y, mp_loc.z] -= power;
                                    AddChunkToModifiedList(cs.leftChunk);
                                    if (mp_loc.z == 0)
                                    {
                                        cs.leftChunk.backChunk.lavaData[chunkSize, mp_loc.y, chunkSize] -= power;
                                        AddChunkToModifiedList(cs.leftChunk.backChunk);
                                    }
                                    else if (mp_loc.z == chunkSize)
                                    {
                                        cs.leftChunk.frontChunk.lavaData[chunkSize, mp_loc.y, 0] -= power;
                                        AddChunkToModifiedList(cs.leftChunk.frontChunk);
                                    }
                                }
                                else if (mp_loc.x == chunkSize)
                                {
                                    cs.rightChunk.lavaData[0, mp_loc.y, mp_loc.z] -= power;
                                    AddChunkToModifiedList(cs.rightChunk);
                                    if (mp_loc.z == 0)
                                    {
                                        cs.rightChunk.backChunk.lavaData[0, mp_loc.y, chunkSize] -= power;
                                        AddChunkToModifiedList(cs.rightChunk.backChunk);
                                    }
                                    else if (mp_loc.z == chunkSize)
                                    {
                                        cs.rightChunk.frontChunk.lavaData[0, mp_loc.y, 0] -= power;
                                        AddChunkToModifiedList(cs.rightChunk.frontChunk);
                                    }
                                }
                                if (mp_loc.z == 0)
                                {
                                    cs.backChunk.lavaData[mp_loc.x, mp_loc.y, chunkSize] -= power;
                                    AddChunkToModifiedList(cs.backChunk);
                                }
                                else if (mp_loc.z == chunkSize)
                                {
                                    cs.frontChunk.lavaData[mp_loc.x, mp_loc.y, 0] -= power;
                                    AddChunkToModifiedList(cs.frontChunk);
                                }
                            }
                        }
                    }
                }
            }
        }




        #endregion
        yield break;




        #region old code

        //pouring lava is currently disabled due to big update

        yield break;

        //#region set settings
        //pourPos = new Vector3(Mathf.Round(pourPos.x), Mathf.Round(pourPos.y), Mathf.Round(pourPos.z));
        //pourPos -= new Vector3(cs.position.x, 0, cs.position.y);
        //int pourDist = (int)pourDistance_float;
        //#endregion



        //List<Vector3> addedPoses_org = new List<Vector3>();




        //List<Vector3> addedPoses_right = new List<Vector3>();
        //List<Vector3> addedPoses_left = new List<Vector3>();
        //List<Vector3> addedPoses_front = new List<Vector3>();
        //List<Vector3> addedPoses_back = new List<Vector3>();
        //List<Vector3> addedPoses_rightfront = new List<Vector3>();
        //List<Vector3> addedPoses_rightback = new List<Vector3>();
        //List<Vector3> addedPoses_leftfront = new List<Vector3>();
        //List<Vector3> addedPoses_leftback = new List<Vector3>();

        //void TryAddLavaData(ChunkScript addingChunk, Vector3 v, List<Vector3> orgData, out List<Vector3> newData)
        //{
        //    newData = new List<Vector3>(orgData);
        //    if (v.x > 8 || v.x < 0 || v.z < 0 || v.z > 8)
        //    {
        //        return;
        //    }
        //    if (!addingChunk.lavaData.Contains(v))
        //    {
        //        if (addingChunk.terrainMap[(int)v.x, (int)v.y, (int)v.z] > mg.terrainSuface)
        //        {
        //            addingChunk.lavaData.Add(v);
        //            newData.Add(v);
        //        }
        //    }
        //}

        //void AddLavaData(Vector3 v)
        //{
        //    if (!(v.y >= 0 && v.y <= 256))
        //    {
        //        Debug.Log("low");
        //        return;
        //    }

        //    if (v.x <= 8 && v.x >= 0 && v.z <= 8 && v.z >= 0)
        //    {
        //        TryAddLavaData(cs, v, addedPoses_org, out addedPoses_org);
        //    }


        //    bool crossRight = false;
        //    bool crossLeft = false;
        //    bool crossFront = false;
        //    bool crossBack = false;
        //    if (v.x >= 8)
        //    {
        //        crossRight = true;
        //    }
        //    else if (v.x <= 0)
        //    {
        //        crossLeft = true;
        //    }
        //    if (v.z >= 8)
        //    {
        //        crossFront = true;
        //    }
        //    else if (v.z <= 0)
        //    {
        //        crossBack = true;
        //    }
        //    if (crossRight && crossFront)
        //    {
        //        TryAddLavaData(cs.rightChunk.frontChunk, v - new Vector3(1, 0, 1) * 8, addedPoses_rightfront, out addedPoses_rightfront);
        //    }
        //    if (crossRight && crossBack)
        //    {
        //        TryAddLavaData(cs.rightChunk.backChunk, v - new Vector3(1, 0, -1) * 8, addedPoses_rightback, out addedPoses_rightback);
        //    }
        //    if (crossLeft && crossFront)
        //    {
        //        TryAddLavaData(cs.leftChunk.frontChunk, v - new Vector3(-1, 0, 1) * 8, addedPoses_leftfront, out addedPoses_leftfront);
        //    }
        //    if (crossLeft && crossBack)
        //    {
        //        TryAddLavaData(cs.leftChunk.backChunk, v - new Vector3(-1, 0, -1) * 8, addedPoses_leftback, out addedPoses_leftback);
        //    }

        //    if (crossRight)
        //    {
        //        TryAddLavaData(cs.rightChunk, v - new Vector3(1, 0, 0) * 8, addedPoses_right, out addedPoses_right);
        //    }
        //    if (crossLeft)
        //    {
        //        TryAddLavaData(cs.leftChunk, v - new Vector3(-1, 0, 0) * 8, addedPoses_left, out addedPoses_left);
        //    }

        //    if (crossFront)
        //    {
        //        TryAddLavaData(cs.frontChunk, v - new Vector3(0, 0, 1) * 8, addedPoses_front, out addedPoses_front);
        //    }
        //    if (crossBack)
        //    {
        //        TryAddLavaData(cs.backChunk, v - new Vector3(0, 0, -1) * 8, addedPoses_back, out addedPoses_back);
        //    }
        //}


        //for (int x = -pourDist; x <= pourDist; x++)
        //{
        //    int width = pourDist - Mathf.Abs(x);
        //    for (int y = -0; y <= 1; y++)
        //    {
        //        for (int z = -width; z <= width; z++)
        //        {
        //            Vector3 v = pourPos + new Vector3(x, y, z);
        //            AddLavaData(v);
        //        }
        //    }
        //}
        //StartCoroutine(PourLava_ApplyData(cs, addedPoses_org, delay));

        //StartCoroutine(PourLava_ApplyData(cs.rightChunk, addedPoses_right, delay));
        //StartCoroutine(PourLava_ApplyData(cs.leftChunk, addedPoses_left, delay));
        //StartCoroutine(PourLava_ApplyData(cs.frontChunk, addedPoses_front, delay));
        //StartCoroutine(PourLava_ApplyData(cs.backChunk, addedPoses_back, delay));
        //StartCoroutine(PourLava_ApplyData(cs.rightChunk.frontChunk, addedPoses_rightfront, delay));
        //StartCoroutine(PourLava_ApplyData(cs.rightChunk.backChunk, addedPoses_rightback, delay));
        //StartCoroutine(PourLava_ApplyData(cs.leftChunk.frontChunk, addedPoses_leftfront, delay));
        //StartCoroutine(PourLava_ApplyData(cs.leftChunk.backChunk, addedPoses_leftback, delay));



        //yield return null;
        #endregion
    }
    IEnumerator PourLava_ApplyData(ChunkScript cs, List<Vector3> addedPoses, float delay)
    {









































        //currently disabled due to big update
        yield break;
        
        //if (addedPoses.Count == 0)
        //    yield break;

        //bool touchLava = ConflictionWithWater(cs, addedPoses).Count != 0;
        //if (touchLava)
        //{
        //    List<Vector3> conf = ConflictionWithWater(cs, addedPoses);
        //    foreach (Vector3 v in conf)
        //    {
        //        wm.PlaceObsidian(cs, v);
        //        cs.waterData.Remove(v);
        //        cs.lavaData.Remove(v);
        //    }
        //    if (wm.modifiedChunkDataKeys.Contains(cs))
        //    {
        //        foreach (Vector3 v in SpecialFeatures.CoverListWithWalls(conf))
        //        {
        //            if (!wm.modifiedChunksDataDictionary[cs].modifiedPoses.Contains(v))
        //                wm.modifiedChunksDataDictionary[cs].modifiedPoses.Add(v);
        //        }
        //    }
        //    else
        //    {
        //        wm.modifiedChunksDataDictionary.Add(cs, new UpdatedChunkData { cs = cs, modifiedPoses = SpecialFeatures.CoverListWithWalls(conf) });
        //        wm.modifiedChunkDataKeys.Add(cs);
        //    }
        //}


        //List<Vector3> modifiedPoses = new List<Vector3>(addedPoses);

        //foreach (Vector3 v in modifiedPoses)
        //{
        //    if (!cs.lavaPointDictionary.ContainsKey(v))
        //    {
        //        WaterPointData lpd = new WaterPointData();
        //        lpd.vertices = new List<Vector3>();
        //        lpd.triangles = new List<int>();

        //        cs.lavaPointDictionary.Add(v, lpd);
        //        cs.lpdList.Add(lpd);
        //    }
        //}

        //if (delay != 0)
        //    yield return new WaitForSeconds(delay);

        //cs.vertices_lava.Clear();
        //cs.triangles_lava.Clear();
        //cs.verticesRangeDictionary_lava.Clear();
        //cs.lavaPointDictionary.Clear();
        //cs.lpdList.Clear();
        //GenerateLava(cs, false, false);

        ////apply
        //if (modifiedChunksDataDictionary.ContainsKey(cs))
        //{
        //    foreach (Vector3 m in addedPoses)
        //    {
        //        if (!modifiedChunksDataDictionary[cs].modifiedPoses.Contains(m))
        //        {
        //            modifiedChunksDataDictionary[cs].modifiedPoses.Add(m);
        //        }
        //    }
        //}
        //else
        //{
        //    modifiedChunkDataKeys.Add(cs);
        //    modifiedChunksDataDictionary.Add(cs, new UpdatedChunkData { cs = cs, modifiedPoses = addedPoses });
        //}
    }




    #region wierd stuff

    Vector3[] surroundingTable = new Vector3[26]
    {
                new Vector3(0,0,-1),
                new Vector3(0,0,1),
                new Vector3(0,-1,0),
                new Vector3(0,-1,-1),
                new Vector3(0,-1,1),
                new Vector3(0,1,0),
                new Vector3(0,1,-1),
                new Vector3(0,1,1),
                new Vector3(-1,0,0),
                new Vector3(-1,0,-1),
                new Vector3(-1,0,1),
                new Vector3(-1,-1,0),
                new Vector3(-1,-1,-1),
                new Vector3(-1,-1,1),
                new Vector3(-1,1,0),
                new Vector3(-1,1,-1),
                new Vector3(-1,1,1),
                new Vector3(1,0,0),
                new Vector3(1,0,-1),
                new Vector3(1,0,1),
                new Vector3(1,-1,0),
                new Vector3(1,-1,-1),
                new Vector3(1,-1,1),
                new Vector3(1,1,0),
                new Vector3(1,1,-1),
                new Vector3(1,1,1)
    };
    Vector3[] surroundingTable_simplified = new Vector3[4]
    {
        Vector3.up, Vector3.right, Vector3.down, Vector3.left
    };

    #endregion
}

[System.Serializable]
public class UpdatedChunkData
{
    public ChunkScript cs;
    public List<Vector3> modifiedPoses = new List<Vector3>();
}


