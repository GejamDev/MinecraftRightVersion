using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterManager : MonoBehaviour
{
    public UniversalScriptManager usm;
    MeshGenerator mg;
    ChunkLoader cl;
    WorldGenerationPreset wgPreset;
    public float updateTick;

     int addedModifiedChunkCount = 0;
    public List<ChunkScript> modifiedChunkDataKeys = new List<ChunkScript>();
    public Dictionary<ChunkScript, UpdatedChunkData> modifiedChunksDataDictionary = new Dictionary<ChunkScript, UpdatedChunkData>();


    void Awake()
    {
        mg = usm.meshGenerator;
        cl = usm.chunkLoader;

        wgPreset = usm.worldGenerationPreset;
        UpdateWater_Tick();
    }

    public void GenerateWater(ChunkScript cs)
    {
        mg.GenerateWaterMesh(cs, true);
    }
    public void GenerateWater(ChunkScript cs, float delay)
    {
        StartCoroutine(GenerateWater_Delay(cs, delay));
    }
    IEnumerator GenerateWater_Delay(ChunkScript cs, float delay)
    {
        yield return new WaitForSeconds(delay);
        GenerateWater(cs);

    }


    public void UpdateWater_Tick()
    {
        for (int i = 0; i < modifiedChunkDataKeys.Count - addedModifiedChunkCount; i++)
        {
            ChunkScript key = modifiedChunkDataKeys[i];
            UpdatedChunkData ucd = modifiedChunksDataDictionary[key];


            modifiedChunkDataKeys.RemoveAt(i);
            modifiedChunksDataDictionary.Remove(key);
            i--;


            UpdateWater(ucd.cs, new List<Vector3>());
            mg.ReGenerateWaterMesh(ucd.cs, ucd.modifiedPoses);
        }
        addedModifiedChunkCount = 0;

        Invoke(nameof(UpdateWater_Tick), updateTick);
    }

    public void UpdateWater(ChunkScript cs, List<Vector3> previouslyModifiedPosition)
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
        foreach(Vector3 v in previouslyModifiedPosition)
        {
            modifiedPos.Add(v);
        }

        int count = modifiedWaterData.Count;

        Dictionary<ChunkScript, UpdatedChunkData> effectedChunkDataDictionary = new Dictionary<ChunkScript, UpdatedChunkData>();
        
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
            bool hasGroundDown = modifiedWaterData.Contains(pos + Vector3.down);
            bool separated = false;



            if (hasGroundDown)
            {
                //onWater = true;
            }
            else
            {
                if (pos.y > 1)
                {
                    if (cs.terrainMap[(int)pos.x, (int)pos.y-1, (int)pos.z] > mg.terrainSuface)
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


            if(hasGroundDown && !hasWater_R && !hasWater_L && !hasWater_F && !hasWater_B)
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
            }
            //else if (separated)
            //{
            //    modifiedWaterData.RemoveAt(i);
            //    i--;

            //    modified = true;
            //    modifiedPos.Add(pos);
            //}



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
        for(int i = 0; i < modifiedPosCount; i++)
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
            cs.waterData = modifiedWaterData;
            modifiedChunksDataDictionary.Add(cs, new UpdatedChunkData { cs = cs, modifiedPoses= modifiedPos });
            modifiedChunkDataKeys.Add(cs);
            addedModifiedChunkCount++;
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

    public IEnumerator Pourwater(ChunkScript cs, Vector3 pourPos, float pourDistance_float, float delay, List<Vector2> alreadyDoneChunkPoses_old)
    {
        if (alreadyDoneChunkPoses_old.Contains(cs.position))
        {
            Debug.Log(0);
            yield break;
        }

        pourPos = new Vector3(Mathf.Round(pourPos.x), Mathf.Round(pourPos.y), Mathf.Round(pourPos.z));


        int addedCount = 0;
        List<Vector3> addedPoses = new List<Vector3>();
        List<Vector2> alreadyDoneChunkList = new List<Vector2>(alreadyDoneChunkPoses_old);
        alreadyDoneChunkList.Add(cs.position);
        
        pourPos -= new Vector3(cs.position.x, 0, cs.position.y);

        int pourDist = (int)pourDistance_float;

        for(int x = -pourDist; x<=pourDist; x++)
        {
            int width = pourDist - Mathf.Abs(x);
            Debug.Log(width);
            for (int y = -0; y <= 0; y++)
            {
                for (int z = -width; z <= width; z++)
                {
                    Vector3 v = pourPos + new Vector3(x, y, z);
                    if (v.x <= 8 && v.x >= 0 && v.z <= 8 && v.z >= 0 && v.y >= 0 && v.y <= 256)
                    {
                        if (!cs.waterData.Contains(v))
                        {
                            if (cs.terrainMap[(int)v.x, (int)v.y - 1, (int)v.z] > mg.terrainSuface)
                            {
                                cs.waterData.Add(v);
                                addedCount++;
                                addedPoses.Add(v);
                            }
                            else if (!cs.waterData.Contains(v + Vector3.up))
                            {
                                if (cs.terrainMap[(int)v.x, (int)v.y, (int)v.z] > mg.terrainSuface)
                                {
                                    cs.waterData.Add(v + Vector3.up);
                                    addedCount++;
                                    addedPoses.Add(v + Vector3.up);
                                }
                                else if (!cs.waterData.Contains(v + Vector3.up * 2))
                                {
                                    if (cs.terrainMap[(int)v.x, (int)v.y + 1, (int)v.z] > mg.terrainSuface)
                                    {
                                        cs.waterData.Add(v + Vector3.up * 2);
                                        addedCount++;
                                        addedPoses.Add(v + Vector3.up * 2);
                                    }
                                }
                            }
                            else if (!cs.waterData.Contains(v + Vector3.up * 2))
                            {
                                if (cs.terrainMap[(int)v.x, (int)v.y + 1, (int)v.z] > mg.terrainSuface)
                                {
                                    cs.waterData.Add(v + Vector3.up * 2);
                                    addedCount++;
                                    addedPoses.Add(v + Vector3.up * 2);
                                }
                            }
                        }
                    }
                    else
                    {

                    }
                }
            }
        }
        pourPos += new Vector3(cs.position.x, 0, cs.position.y);



        if (addedCount != 0)
        {

            //get modified poses
            List<Vector3> modifiedPoses = SpecialFeatures.CoverListWithWalls(addedPoses);

            foreach(Vector3 v in modifiedPoses)
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


            //check crossing parts
            bool crossRight = false;
            bool crossLeft = false;
            bool crossFront = false;
            bool crossBack = false;

            foreach (Vector3 v in addedPoses)
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





            #region effect neighborhood chunks
            if (crossRight && crossFront)
            {
                StartCoroutine(Pourwater(cs.rightChunk.frontChunk, pourPos, pourDist, delay, alreadyDoneChunkList));
            }
            if (crossRight && crossBack)
            {
                StartCoroutine(Pourwater(cs.rightChunk.backChunk, pourPos, pourDist, delay, alreadyDoneChunkList));
            }
            if (crossLeft && crossFront)
            {
                StartCoroutine(Pourwater(cs.leftChunk.frontChunk, pourPos, pourDist, delay, alreadyDoneChunkList));
            }
            if (crossLeft && crossBack)
            {
                StartCoroutine(Pourwater(cs.leftChunk.backChunk, pourPos, pourDist, delay, alreadyDoneChunkList));
            }

            if (crossRight)
            {
                StartCoroutine(Pourwater(cs.rightChunk, pourPos, pourDist, delay, alreadyDoneChunkList));
            }
            if (crossLeft)
            {
                StartCoroutine(Pourwater(cs.leftChunk, pourPos, pourDist, delay, alreadyDoneChunkList));
            }

            if (crossFront)
            {
                StartCoroutine(Pourwater(cs.frontChunk, pourPos, pourDist, delay, alreadyDoneChunkList));
            }
            if (crossBack)
            {
                StartCoroutine(Pourwater(cs.backChunk, pourPos, pourDist, delay, alreadyDoneChunkList));
            }
            #endregion


            if(delay!=0)
                yield return new WaitForSeconds(delay);



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




