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

    public int addedModifiedChunkCount = 0;
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

            //check states
            //bool onWater = false;
            //bool onGround = false;
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
            

            if (modifiedWaterData.Contains(pos + Vector3.down))
            {
                //onWater = true;
            }
            else
            {
                if (pos.y != 0)
                {
                    if (cs.terrainMap[(int)pos.x, (int)pos.y - 1, (int)pos.z] > mg.terrainSuface)
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

}




