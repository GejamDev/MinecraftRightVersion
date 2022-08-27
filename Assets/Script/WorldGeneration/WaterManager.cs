using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterManager : MonoBehaviour
{
    public UniversalScriptManager usm;
    MeshGenerator mg;
    ChunkLoader cl;
    LavaManager lm;
    BlockPlacementManager bpm;
    WorldGenerationPreset wgPreset;
    public float updateTick;

     int addedModifiedChunkCount = 0;
    public List<ChunkScript> modifiedChunkDataKeys = new List<ChunkScript>();
    public Dictionary<ChunkScript, UpdatedChunkData> modifiedChunksDataDictionary = new Dictionary<ChunkScript, UpdatedChunkData>();

    public Item obsidian;


    void Awake()
    {
        mg = usm.meshGenerator;
        cl = usm.chunkLoader;
        lm = usm.lavaManager;
        bpm = usm.blockPlacementManager;

        wgPreset = usm.worldGenerationPreset;
        StartCoroutine(UpdateWater_Tick());
    }

    public void GenerateWater(ChunkScript cs, bool delay)
    {
        mg.GenerateWaterMesh(cs, delay);
    }
    public void GenerateWater(ChunkScript cs, float delay)
    {
        StartCoroutine(GenerateWater_Delay(cs, delay));
    }
    IEnumerator GenerateWater_Delay(ChunkScript cs, float delay)
    {
        yield return new WaitForSeconds(delay);
        GenerateWater(cs, true);

    }


    public IEnumerator UpdateWater_Tick()
    {
        yield return new WaitForSeconds(0.05f);
        while (true)
        {
            for (int i = 0; i < modifiedChunkDataKeys.Count - addedModifiedChunkCount; i++)
            {
                ChunkScript key = modifiedChunkDataKeys[i];
                UpdatedChunkData ucd = modifiedChunksDataDictionary[key];


                modifiedChunkDataKeys.RemoveAt(i);
                modifiedChunksDataDictionary.Remove(key);
                i--;


                UpdateWater(ucd.cs, new List<Vector3>(), i);
                mg.ReGenerateWaterMesh(ucd.cs, ucd.modifiedPoses);
            }
            addedModifiedChunkCount = 0;
            yield return new WaitForSeconds(updateTick);
            yield return new WaitUntil(() => modifiedChunkDataKeys.Count > 0);
        }
    }

    public void UpdateWater(ChunkScript cs, List<Vector3> previouslyModifiedPosition, int chunkIndex)
    {
        //disabled or not ready
        if (!cs.activated || cs.rightChunk == null)
        {
            cs.waterBeingModified = false;
            return;
        }

        cs.waterBeingModified = true;


        bool modified = false;
        List<Vector3> modifiedWaterData = cs.waterData;
        List<Vector3> modifiedPos = new List<Vector3>();
        List<Vector3> addedPos = new List<Vector3>();
        foreach (Vector3 v in previouslyModifiedPosition)
        {
            modifiedPos.Add(v);
        }

        int count = modifiedWaterData.Count;


        List<ChunkScript> effectedChunks = new List<ChunkScript>();

        void CheckEffectingOtherChunks(Vector3 v)
        {
            bool crossedRight = false;
            bool crossedLeft = false;
            bool crossedFront = false;
            bool crossedBack = false;

            if (v.x >= 8)
            {
                crossedRight = true;
            }
            else if (v.x <= 0)
            {
                crossedLeft = true;
            }
            if (v.z >= 8)
            {
                crossedFront = true;
            }
            else if (v.z <= 0)
            {
                crossedBack = true;
            }
            if (crossedRight && crossedFront && !effectedChunks.Contains(cs.rightChunk.frontChunk))
            {
                effectedChunks.Add(cs.rightChunk.frontChunk);
            }
            else if (crossedRight && crossedBack && !effectedChunks.Contains(cs.rightChunk.backChunk))
            {
                effectedChunks.Add(cs.rightChunk.backChunk);
            }
            else if (crossedLeft && crossedFront && !effectedChunks.Contains(cs.leftChunk.frontChunk))
            {
                effectedChunks.Add(cs.leftChunk.frontChunk);
            }
            else if (crossedLeft && crossedBack && !effectedChunks.Contains(cs.leftChunk.backChunk))
            {
                effectedChunks.Add(cs.leftChunk.backChunk);
            }
            if(crossedRight && !effectedChunks.Contains(cs.rightChunk))
            {
                effectedChunks.Add(cs.rightChunk);
            }
            else if (crossedLeft && !effectedChunks.Contains(cs.leftChunk))
            {
                effectedChunks.Add(cs.leftChunk);
            }
            if (crossedFront && !effectedChunks.Contains(cs.frontChunk))
            {
                effectedChunks.Add(cs.frontChunk);
            }
            else if (crossedBack && !effectedChunks.Contains(cs.backChunk))
            {
                effectedChunks.Add(cs.backChunk);
            }
        }
        
        for(int i = 0; i <count; i++)
        {
            Vector3 pos = modifiedWaterData[i];
            bool goDown = false;

            bool hasWater_R = false;
            bool hasWaterBottom_R = false;
            bool hasGround_R = false;
            bool hasGroundBottom_R = false;
            bool hasWater_L = false;
            bool hasWaterBottom_L = false;
            bool hasGround_L = false;
            bool hasGroundBottom_L = false;
            bool hasWater_F = false;
            bool hasWaterBottom_F = false;
            bool hasGround_F = false;
            bool hasGroundBottom_F = false;
            bool hasWater_B = false;
            bool hasWaterBottom_B = false;
            bool hasGround_B = false;
            bool hasGroundBottom_B = false;
            bool hasWaterDown = modifiedWaterData.Contains(pos + Vector3.down);
            bool separated = false;

            bool onGround(Vector3 checkPos)
            {
                if(cs.terrainMap[(int)checkPos.x, (int)checkPos.y - 1, (int)checkPos.z] <= mg.terrainSuface)
                {
                    return true;
                }
                if(cs.blockPositionData.Contains(new Vector3Int((int)checkPos.x, (int)checkPos.y - 1, (int)checkPos.z)))
                {
                    return true;
                }



                if (cs.rightChunk.blockPositionData.Contains((new Vector3Int((int)checkPos.x - 8, (int)checkPos.y - 1, (int)checkPos.z))))
                {
                    return true;
                }
                if (cs.leftChunk.blockPositionData.Contains((new Vector3Int((int)checkPos.x + 8, (int)checkPos.y - 1, (int)checkPos.z))))
                {
                    return true;
                }
                if (cs.frontChunk.blockPositionData.Contains((new Vector3Int((int)checkPos.x, (int)checkPos.y - 1, (int)checkPos.z - 8))))
                {
                    return true;
                }
                if (cs.backChunk.blockPositionData.Contains((new Vector3Int((int)checkPos.x, (int)checkPos.y - 1, (int)checkPos.z + 8))))
                {
                    return true;
                }
                if (cs.rightChunk.frontChunk.blockPositionData.Contains((new Vector3Int((int)checkPos.x - 8, (int)checkPos.y - 1, (int)checkPos.z - 8))))
                {
                    return true;
                }
                if (cs.rightChunk.backChunk.blockPositionData.Contains((new Vector3Int((int)checkPos.x - 8, (int)checkPos.y - 1, (int)checkPos.z + 8))))
                {
                    return true;
                }
                if (cs.leftChunk.frontChunk.blockPositionData.Contains((new Vector3Int((int)checkPos.x + 8, (int)checkPos.y - 1, (int)checkPos.z - 8))))
                {
                    return true;
                }
                if (cs.leftChunk.backChunk.blockPositionData.Contains((new Vector3Int((int)checkPos.x + 8, (int)checkPos.y - 1, (int)checkPos.z + 8))))
                {
                    return true;
                }
                return false;
            }

            if (hasWaterDown)
            {
                //onWater = true;
            }
            else
            {
                if (pos.y > 1)
                {
                    if (!onGround(pos))
                    {
                        goDown = true;
                    }
                    else
                    {
                        //onGround = true;
                    }
                }
                else
                {
                    //onGround = true;
                }
            }

            if(pos.x == 0)
            {
                hasWater_R = modifiedWaterData.Contains(pos + Vector3.right);
                hasWaterBottom_R = modifiedWaterData.Contains(pos + Vector3.right + Vector3.down);

                hasGround_R = cs.terrainMap[(int)pos.x + 1, (int)pos.y, (int)pos.z] <= mg.terrainSuface;
                hasGroundBottom_R = pos.y == 0 ? true : cs.terrainMap[(int)pos.x + 1, (int)pos.y - 1, (int)pos.z] <= mg.terrainSuface;


                hasWater_L = cs.leftChunk.waterData.Contains(new Vector3(wgPreset.chunkSize - 1, pos.y, pos.z));
                hasWaterBottom_L = cs.leftChunk.waterData.Contains(new Vector3(wgPreset.chunkSize - 1, pos.y - 1, pos.z));

                hasGround_L = cs.leftChunk.terrainMap[wgPreset.chunkSize - 1, (int)pos.y, (int)pos.z] <= mg.terrainSuface;
                hasGroundBottom_L = pos.y == 0 ? true : cs.leftChunk.terrainMap[wgPreset.chunkSize - 1, (int)pos.y - 1, (int)pos.z] <= mg.terrainSuface;
            }
            else if(pos.x == wgPreset.chunkSize)
            {
                hasWater_R = cs.rightChunk.waterData.Contains(new Vector3(1, pos.y, pos.z));
                hasWaterBottom_R = cs.rightChunk.waterData.Contains(new Vector3(1, pos.y - 1, pos.z));

                hasGround_R = cs.rightChunk.terrainMap[1, (int)pos.y, (int)pos.z] <= mg.terrainSuface;
                hasGroundBottom_R = pos.y == 0 ? true : cs.rightChunk.terrainMap[1, (int)pos.y - 1, (int)pos.z] <= mg.terrainSuface;


                hasWater_L = modifiedWaterData.Contains(pos + Vector3.left);
                hasWater_L = modifiedWaterData.Contains(pos + Vector3.left + Vector3.down);

                hasGround_L = cs.terrainMap[(int)pos.x - 1, (int)pos.y, (int)pos.z] <= mg.terrainSuface;
                hasGroundBottom_L = pos.y == 0 ? true : cs.terrainMap[(int)pos.x - 1, (int)pos.y - 1, (int)pos.z] <= mg.terrainSuface;
            }
            else
            {
                hasWater_R = modifiedWaterData.Contains(pos + Vector3.right);
                hasWaterBottom_R = modifiedWaterData.Contains(pos + Vector3.right + Vector3.down);

                hasGround_R = cs.terrainMap[(int)pos.x + 1, (int)pos.y, (int)pos.z] <= mg.terrainSuface;
                hasGroundBottom_R = pos.y == 0 ? true :  cs.terrainMap[(int)pos.x + 1, (int)pos.y - 1, (int)pos.z] <= mg.terrainSuface;


                hasWater_L = modifiedWaterData.Contains(pos + Vector3.left);
                hasWaterBottom_L = modifiedWaterData.Contains(pos + Vector3.left + Vector3.down);

                hasGround_L = cs.terrainMap[(int)pos.x - 1, (int)pos.y, (int)pos.z] <= mg.terrainSuface;
                hasGroundBottom_L = pos.y == 0 ? true : cs.terrainMap[(int)pos.x - 1, (int)pos.y - 1, (int)pos.z] <= mg.terrainSuface;
            }


            if (pos.z == 0)
            {
                hasWater_F =modifiedWaterData.Contains(pos + Vector3.forward);
                hasWaterBottom_F = modifiedWaterData.Contains(pos + Vector3.forward + Vector3.down);

                hasGround_F = cs.terrainMap[(int)pos.x, (int)pos.y, (int)pos.z + 1] <= mg.terrainSuface;
                hasGroundBottom_F = pos.y == 0 ? true : cs.terrainMap[(int)pos.x, (int)pos.y - 1, (int)pos.z + 1] <= mg.terrainSuface;


                hasWater_B = cs.backChunk.waterData.Contains(new Vector3(pos.x, pos.y, wgPreset.chunkSize - 1));
                hasWaterBottom_B = cs.backChunk.waterData.Contains(new Vector3(pos.x, pos.y - 1, wgPreset.chunkSize - 1));


                hasGround_B = cs.backChunk.terrainMap[(int)pos.x, (int)pos.y, wgPreset.chunkSize - 1] <= mg.terrainSuface;
                hasGroundBottom_B = pos.y == 0 ? true : cs.backChunk.terrainMap[(int)pos.x, (int)pos.y - 1, wgPreset.chunkSize - 1] <= mg.terrainSuface;
            }
            else if (pos.z == wgPreset.chunkSize)
            {
                hasWater_F = cs.frontChunk.waterData.Contains(new Vector3(pos.x, pos.y, 1));
                hasWaterBottom_F = cs.frontChunk.waterData.Contains(new Vector3(pos.x, pos.y - 1, 1));

                hasGround_F = cs.frontChunk.terrainMap[(int)pos.x, (int)pos.y, 1] <= mg.terrainSuface;
                hasGroundBottom_F = pos.y == 0 ? true : cs.frontChunk.terrainMap[(int)pos.x, (int)pos.y - 1, 1] <= mg.terrainSuface;


                hasWater_B = modifiedWaterData.Contains(pos + Vector3.back);
                hasWaterBottom_B = modifiedWaterData.Contains(pos + Vector3.back + Vector3.down);

                hasGround_B = cs.terrainMap[(int)pos.x, (int)pos.y, (int)pos.z - 1] <= mg.terrainSuface;
                hasGroundBottom_B = pos.y == 0 ? true : cs.terrainMap[(int)pos.x, (int)pos.y - 1, (int)pos.z - 1] <= mg.terrainSuface;
            }
            else
            {
                hasWater_F = modifiedWaterData.Contains(pos + Vector3.forward);
                hasWaterBottom_F = modifiedWaterData.Contains(pos + Vector3.forward + Vector3.down);

                hasGround_F = cs.terrainMap[(int)pos.x, (int)pos.y, (int)pos.z + 1] <= mg.terrainSuface;
                hasGroundBottom_F = pos.y == 0 ? true : cs.terrainMap[(int)pos.x, (int)pos.y - 1, (int)pos.z + 1] <= mg.terrainSuface;


                hasWater_B = modifiedWaterData.Contains(pos + Vector3.back);
                hasWaterBottom_B = modifiedWaterData.Contains(pos + Vector3.back + Vector3.down);

                hasGround_B = cs.terrainMap[(int)pos.x, (int)pos.y, (int)pos.z - 1] <= mg.terrainSuface;
                hasGroundBottom_B = pos.y == 0 ? true : cs.terrainMap[(int)pos.x, (int)pos.y - 1, (int)pos.z - 1] <= mg.terrainSuface;
            }


            if(hasWaterDown && !hasWater_R && !hasWater_L && !hasWater_F && !hasWater_B)
            {
                separated = true;
            }

            //go down
            if (goDown)
            {

                modifiedWaterData.Add(pos + Vector3.down);
                modifiedWaterData.RemoveAt(i);
                i--;
                count--;
                modified = true;

                modifiedPos.Add(pos + Vector3.down);
                modifiedPos.Add(pos);


                CheckEffectingOtherChunks(pos);
            }


            #region spread stuff(currently disabled)
            //else //spread
            //{
            //    //check directions able to spread
            //    bool canSpread_R = !hasWater_R && !hasGround_R && !hasGroundBottom_R && !hasWaterBottom_R;
            //    bool canSpread_L = !hasWater_L && !hasGround_L && !hasGroundBottom_L && !hasWaterBottom_L;
            //    bool canSpread_F = !hasWater_F && !hasGround_F && !hasGroundBottom_F && !hasWaterBottom_F;
            //    bool canSpread_B = !hasWater_B && !hasGround_B && !hasGroundBottom_B && !hasWaterBottom_B;

            //    List<Vector3> movableDirections = new List<Vector3>();
            //    if (canSpread_R && pos.x > 0 && pos.x < wgPreset.chunkSize - 1)
            //    {
            //        movableDirections.Add(Vector3.right);
            //    }
            //    if (canSpread_L && pos.x > 1 && pos.x < wgPreset.chunkSize)
            //    {
            //        movableDirections.Add(Vector3.left);
            //    }
            //    if (canSpread_F && pos.z > 0 && pos.z < wgPreset.chunkSize - 1)
            //    {
            //        movableDirections.Add(Vector3.forward);
            //    }
            //    if (canSpread_B && pos.z > 1 && pos.z < wgPreset.chunkSize)
            //    {
            //        movableDirections.Add(Vector3.back);
            //    }
            //    if (movableDirections.Count != 0)
            //    {
            //        modified = true;



            //        Vector3 dir = movableDirections[Random.Range(0, movableDirections.Count)];
            //        Vector3 target = pos + dir;

            //        modifiedWaterData.RemoveAt(i);
            //        i--;
            //        count--;
            //        modifiedPos.Add(pos);
            //        modifiedWaterData.Add(target);

            //        modifiedPos.Add(target);
            //    }
            //}
            #endregion
        }


        int modifiedPosCount = modifiedPos.Count;
        for (int i = 0; i < modifiedPosCount; i++)
        {
            Vector3 pos = modifiedPos[i];

            foreach(Vector3 v in surroundingTable)
            {
                Vector3 t = pos + v;
                if(!modifiedPos.Contains(t) && t.x <= wgPreset.chunkSize && t.x >=0 && t.z <=wgPreset.chunkSize && t.z >=0 && t.y >=0 && t.y <= wgPreset.maxHeight)
                {
                    modifiedPos.Add(t);
                }
            }
        }

        if (modified)
        {
            bool touchLava = ConflictionWithLava(cs, modifiedWaterData).Count != 0;
            if (touchLava)
            {
                List<Vector3> conf = ConflictionWithLava(cs, modifiedWaterData);
                foreach (Vector3 v in conf)
                {
                    PlaceObsidian(cs, v);
                    modifiedWaterData.Remove(v);
                    cs.lavaData.Remove(v);
                }
                if (lm.modifiedChunkDataKeys.Contains(cs))
                {
                    foreach(Vector3 v  in SpecialFeatures.CoverListWithWalls(conf))
                    {
                        if(!lm.modifiedChunksDataDictionary[cs].modifiedPoses.Contains(v))
                            lm.modifiedChunksDataDictionary[cs].modifiedPoses.Add(v);
                    }
                }
                else
                {
                    lm.modifiedChunksDataDictionary.Add(cs, new UpdatedChunkData { cs = cs, modifiedPoses = SpecialFeatures.CoverListWithWalls(conf) });
                    lm.modifiedChunkDataKeys.Add(cs);
                }
            }

            cs.waterData = modifiedWaterData;
            modifiedChunksDataDictionary.Add(cs, new UpdatedChunkData { cs = cs, modifiedPoses= modifiedPos });
            modifiedChunkDataKeys.Add(cs);
            addedModifiedChunkCount++;
            foreach (ChunkScript other in effectedChunks)
            {
                if (!modifiedChunksDataDictionary.ContainsKey(other))
                {
                    modifiedChunksDataDictionary.Add(other, new UpdatedChunkData { cs = other, modifiedPoses = new List<Vector3>() });
                    modifiedChunkDataKeys.Insert(chunkIndex + 1, other);
                }
                else
                {
                }
            }
        }
        else
        {
            cs.waterBeingModified = false;
        }

    }

    public IEnumerator DrainWater(ChunkScript cs, Vector3 drainPos, float drainDistance, float delay)
    {





        //set variables
        int removedCount = 0;
        List<Vector3> preWaterList = new List<Vector3>(cs.waterData);
        drainPos -= new Vector3(cs.position.x, 0, cs.position.y);
        List<Vector3> removedPoses = new List<Vector3>();

        //remove water from data
        foreach(Vector3 v in preWaterList)
        {
            if(Vector3.Distance(v, drainPos) <= drainDistance)
            {
                cs.waterData.Remove(v);
                removedCount++;
                removedPoses.Add(v);
            }
        }
        drainPos += new Vector3(cs.position.x, 0, cs.position.y);

        //apply removal
        if (removedCount != 0)
        {

            //get modified poses
            List<Vector3> modifiedPoses = SpecialFeatures.CoverListWithWalls(removedPoses);


            //check crossing parts
            bool crossRight = false;
            bool crossLeft = false;
            bool crossFront = false;
            bool crossBack = false;

            foreach (Vector3 v in removedPoses)
            {
                if (v.x >= 8)
                {
                    crossRight = true;
                }
                else if (v.x <= 0)
                {
                    crossLeft = true;
                }
                if (v.z >= 8)
                {
                    crossFront = true;
                }
                else if (v.z <= 0)
                {
                    crossBack = true;
                }
            }


            yield return new WaitForSeconds(delay);

            //effect neighborhood chunks
            #region effect neighborhood chunks
            if (crossRight && crossFront)
            {
                StartCoroutine(DrainWater(cs.rightChunk.frontChunk, drainPos, drainDistance, delay));
            }
            if (crossRight && crossBack)
            {
                StartCoroutine(DrainWater(cs.rightChunk.backChunk, drainPos, drainDistance, delay));
            }
            if (crossLeft && crossFront)
            {
                StartCoroutine(DrainWater(cs.leftChunk.frontChunk, drainPos, drainDistance, delay));
            }
            if (crossLeft && crossBack)
            {
                StartCoroutine(DrainWater(cs.leftChunk.backChunk, drainPos, drainDistance, delay));
            }

            if (crossRight)
            {
                StartCoroutine(DrainWater(cs.rightChunk, drainPos, drainDistance, delay));
            }
            if (crossLeft)
            {
                StartCoroutine(DrainWater(cs.leftChunk, drainPos, drainDistance, delay));
            }

            if (crossFront)
            {
                StartCoroutine(DrainWater(cs.frontChunk, drainPos, drainDistance, delay));
            }
            if (crossBack)
            {
                StartCoroutine(DrainWater(cs.backChunk, drainPos, drainDistance, delay));
            }
            #endregion






            //apply
            if (modifiedChunksDataDictionary.ContainsKey(cs))
            {
                foreach (Vector3 m in modifiedPoses)
                {
                    if (!modifiedChunksDataDictionary[cs].modifiedPoses.Contains(m))
                    {
                        modifiedChunksDataDictionary[cs].modifiedPoses.Add(m);
                    }
                }
            }
            else
            {
                modifiedChunkDataKeys.Add(cs);
                modifiedChunksDataDictionary.Add(cs, new UpdatedChunkData { cs = cs, modifiedPoses = modifiedPoses });
            }
        }

    }

    
    public IEnumerator Pourwater(ChunkScript cs, Vector3 pourPos, float pourDistance_float, float delay)
    {
        #region set settings
        pourPos = new Vector3(Mathf.Round(pourPos.x), Mathf.Round(pourPos.y), Mathf.Round(pourPos.z));
        pourPos -= new Vector3(cs.position.x, 0, cs.position.y);
        int pourDist = (int)pourDistance_float;
        #endregion



        List<Vector3> addedPoses_org = new List<Vector3>();




        List<Vector3> addedPoses_right = new List<Vector3>();
        List<Vector3> addedPoses_left = new List<Vector3>();
        List<Vector3> addedPoses_front = new List<Vector3>();
        List<Vector3> addedPoses_back = new List<Vector3>();
        List<Vector3> addedPoses_rightfront = new List<Vector3>();
        List<Vector3> addedPoses_rightback = new List<Vector3>();
        List<Vector3> addedPoses_leftfront = new List<Vector3>();
        List<Vector3> addedPoses_leftback = new List<Vector3>();

        void TryAddWaterData(ChunkScript addingChunk, Vector3 v, List<Vector3> orgData, out List<Vector3> newData)
        {
            newData = new List<Vector3>(orgData);
            if (v.x>8 || v.x<0 || v.z < 0 || v.z > 8)
            {
                Debug.Log("out");
                return;
            }
            if (!addingChunk.waterData.Contains(v))
            {
                if (addingChunk.terrainMap[(int)v.x, (int)v.y, (int)v.z] > mg.terrainSuface)
                {
                    addingChunk.waterData.Add(v);
                    newData.Add(v);
                }
                //else if (!addingChunk.waterData.Contains(v + Vector3.up))
                //{
                //    if (addingChunk.terrainMap[(int)v.x, (int)v.y + 1, (int)v.z] > mg.terrainSuface)
                //    {
                //        addingChunk.waterData.Add(v + Vector3.up);
                //        newData.Add(v + Vector3.up);
                //    }
                //    else if (!addingChunk.waterData.Contains(v + Vector3.up * 2))
                //    {
                //        if (addingChunk.terrainMap[(int)v.x, (int)v.y + 2, (int)v.z] > mg.terrainSuface)
                //        {
                //            addingChunk.waterData.Add(v + Vector3.up * 2);
                //            newData.Add(v + Vector3.up * 2);
                //        }
                //    }
                //}
                //else if (!addingChunk.waterData.Contains(v + Vector3.up * 2))
                //{
                //    if (addingChunk.terrainMap[(int)v.x, (int)v.y + 2, (int)v.z] > mg.terrainSuface)
                //    {
                //        addingChunk.waterData.Add(v + Vector3.up * 2);
                //        addedPoses_org.Add(v + Vector3.up * 2);
                //    }
                //}
            }
        }

        void AddWaterData(Vector3 v)
        {
            if (!(v.y >= 0 && v.y <= 256))
            {
                Debug.Log("low");
                return;
            }

            if (v.x <= 8 && v.x >= 0 && v.z <= 8 && v.z >= 0)
            {
                TryAddWaterData(cs, v, addedPoses_org, out addedPoses_org);
            }


            bool crossRight = false;
            bool crossLeft = false;
            bool crossFront = false;
            bool crossBack = false;
            if (v.x >= 8)
            {
                crossRight = true;
            }
            else if (v.x <= 0)
            {
                crossLeft = true;
            }
            if (v.z >= 8)
            {
                crossFront = true;
            }
            else if (v.z <= 0)
            {
                crossBack = true;
            }
            if (crossRight && crossFront)
            {
                TryAddWaterData(cs.rightChunk.frontChunk, v - new Vector3(1, 0, 1) * 8, addedPoses_rightfront, out addedPoses_rightfront);
            }
            if (crossRight && crossBack)
            {
                TryAddWaterData(cs.rightChunk.backChunk, v - new Vector3(1, 0, -1) * 8, addedPoses_rightback, out addedPoses_rightback);
            }
            if (crossLeft && crossFront)
            {
                TryAddWaterData(cs.leftChunk.frontChunk, v - new Vector3(-1, 0, 1) * 8, addedPoses_leftfront, out addedPoses_leftfront);
            }
            if (crossLeft && crossBack)
            {
                TryAddWaterData(cs.leftChunk.backChunk, v - new Vector3(-1, 0, -1) * 8, addedPoses_leftback, out addedPoses_leftback);
            }

            if (crossRight)
            {
                TryAddWaterData(cs.rightChunk, v - new Vector3(1, 0, 0) * 8, addedPoses_right, out addedPoses_right);
            }
            if (crossLeft)
            {
                TryAddWaterData(cs.leftChunk, v - new Vector3(-1, 0, 0) * 8, addedPoses_left, out addedPoses_left);
            }

            if (crossFront)
            {
                TryAddWaterData(cs.frontChunk, v - new Vector3(0, 0, 1) * 8, addedPoses_front, out addedPoses_front);
            }
            if (crossBack)
            {
                TryAddWaterData(cs.backChunk, v - new Vector3(0, 0, -1) * 8, addedPoses_back, out addedPoses_back);
            }
        }


        for (int x = -pourDist; x <= pourDist; x++)
        {
            int width = pourDist - Mathf.Abs(x);
            for (int y = -0; y <= 1; y++)
            {
                for (int z = -width; z <= width; z++)
                {
                    Vector3 v = pourPos + new Vector3(x, y, z);
                    AddWaterData(v);
                }
            }
        }
        StartCoroutine(PourWater_ApplyData(cs, addedPoses_org, delay));

        StartCoroutine(PourWater_ApplyData(cs.rightChunk, addedPoses_right, delay));
        StartCoroutine(PourWater_ApplyData(cs.leftChunk, addedPoses_left, delay));
        StartCoroutine(PourWater_ApplyData(cs.frontChunk, addedPoses_front, delay));
        StartCoroutine(PourWater_ApplyData(cs.backChunk, addedPoses_back, delay));
        StartCoroutine(PourWater_ApplyData(cs.rightChunk.frontChunk, addedPoses_rightfront, delay));
        StartCoroutine(PourWater_ApplyData(cs.rightChunk.backChunk, addedPoses_rightback, delay));
        StartCoroutine(PourWater_ApplyData(cs.leftChunk.frontChunk, addedPoses_leftfront, delay));
        StartCoroutine(PourWater_ApplyData(cs.leftChunk.backChunk, addedPoses_leftback, delay));



        yield return null;
    }
    List<Vector3> ConflictionWithLava(ChunkScript cs, List<Vector3> input)
    {
        List<Vector3> result = new List<Vector3>();
        foreach(Vector3 v in input)
        {
            if (cs.lavaData.Contains(v))
            {
                result.Add(v);
            }
        }
        return result;
    }
    IEnumerator PourWater_ApplyData(ChunkScript cs, List<Vector3> addedPoses, float delay)
    {
        if (addedPoses.Count == 0)
            yield break;
        bool touchLava = ConflictionWithLava(cs, addedPoses).Count != 0;
        if (touchLava)
        {
            List<Vector3> conf = ConflictionWithLava(cs, addedPoses);
            foreach (Vector3 v in conf)
            {
                cs.waterData.Remove(v);
                cs.lavaData.Remove(v);
                PlaceObsidian(cs, v);
            }
            if (lm.modifiedChunkDataKeys.Contains(cs))
            {
                foreach (Vector3 v in SpecialFeatures.CoverListWithWalls(conf))
                {
                    if (!lm.modifiedChunksDataDictionary[cs].modifiedPoses.Contains(v))
                        lm.modifiedChunksDataDictionary[cs].modifiedPoses.Add(v);
                }
            }
            else
            {
                lm.modifiedChunksDataDictionary.Add(cs, new UpdatedChunkData { cs = cs, modifiedPoses = SpecialFeatures.CoverListWithWalls(conf) });
                lm.modifiedChunkDataKeys.Add(cs);
            }
        }

        List<Vector3> modifiedPoses = new List<Vector3>(addedPoses); 
        
        foreach (Vector3 v in modifiedPoses)
        {
            if (!cs.waterPointDictionary.ContainsKey(v))
            {
                WaterPointData wpd = new WaterPointData();
                wpd.vertices = new List<Vector3>();
                wpd.triangles = new List<int>();

                cs.waterPointDictionary.Add(v, wpd);
                cs.wpdList.Add(wpd); 
            }

        }

        if (delay != 0)
            yield return new WaitForSeconds(delay);

        cs.vertices_water.Clear();
        cs.triangles_water.Clear();
        cs.verticesRangeDictionary_water.Clear();
        cs.waterPointDictionary.Clear();
        cs.wpdList.Clear();
        GenerateWater(cs, false);

        //apply
        if (modifiedChunksDataDictionary.ContainsKey(cs))
        {
            foreach (Vector3 m in addedPoses)
            {
                if (!modifiedChunksDataDictionary[cs].modifiedPoses.Contains(m))
                {
                    modifiedChunksDataDictionary[cs].modifiedPoses.Add(m);
                }
            }
        }
        else
        {
            modifiedChunkDataKeys.Add(cs);
            modifiedChunksDataDictionary.Add(cs, new UpdatedChunkData { cs = cs, modifiedPoses = addedPoses });
        }
    }

    public void PlaceObsidian(ChunkScript cs, Vector3 pos)
    {
        bpm.PlaceBlock(obsidian, null, pos+new Vector3(cs.position.x, 0, cs.position.y), false);
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




