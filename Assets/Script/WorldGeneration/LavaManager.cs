using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaManager : MonoBehaviour
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
        UpdateLava_Tick();
    }

    public void GenerateLava(ChunkScript cs)
    {
        mg.GenerateLavaMesh(cs, true);
    }
    public void GenerateLava(ChunkScript cs, float delay)
    {
        StartCoroutine(GenerateLava_Delay(cs, delay));
    }
    IEnumerator GenerateLava_Delay(ChunkScript cs, float delay)
    {
        yield return new WaitForSeconds(delay);
        GenerateLava(cs);

    }


    public void UpdateLava_Tick()
    {
        for (int i = 0; i < modifiedChunkDataKeys.Count - addedModifiedChunkCount; i++)
        {
            ChunkScript key = modifiedChunkDataKeys[i];
            UpdatedChunkData ucd = modifiedChunksDataDictionary[key];


            modifiedChunkDataKeys.RemoveAt(i);
            modifiedChunksDataDictionary.Remove(key);
            i--;


            UpdateLava(ucd.cs, new List<Vector3>());
            mg.ReGenerateLavaMesh(ucd.cs, ucd.modifiedPoses);
        }
        addedModifiedChunkCount = 0;

        Invoke(nameof(UpdateLava_Tick), updateTick);
    }

    public void UpdateLava(ChunkScript cs, List<Vector3> previouslyModifiedPosition)
    {
        //disabled or not ready
        if (!cs.activated || cs.rightChunk == null)
        {
            cs.lavaBeingModified = false;
            return;
        }

        cs.lavaBeingModified = true;


        bool modified = false;
        List<Vector3> modifiedLavaData = cs.lavaData;
        List<Vector3> modifiedPos = new List<Vector3>();
        foreach (Vector3 v in previouslyModifiedPosition)
        {
            modifiedPos.Add(v);
        }

        int count = modifiedLavaData.Count;

        Dictionary<ChunkScript, UpdatedChunkData> effectedChunkDataDictionary = new Dictionary<ChunkScript, UpdatedChunkData>();

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = modifiedLavaData[i];

            //check states
            //bool onWater = false;
            //bool onGround = false;
            bool goDown = false;

            bool hasLava_R = false;
            bool hasLavaBottom_R = false;
            bool hasGround_R = false;
            bool hasGroundBottom_R = false;
            bool hasLava_L = false;
            bool hasLavaBottom_L = false;
            bool hasGround_L = false;
            bool hasGroundBottom_L = false;
            bool hasLava_F = false;
            bool hasLavaBottom_F = false;
            bool hasGround_F = false;
            bool hasGroundBottom_F = false;
            bool hasLava_B = false;
            bool hasLavaBottom_B = false;
            bool hasGround_B = false;
            bool hasGroundBottom_B = false;


            if (modifiedLavaData.Contains(pos + Vector3.down))
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

            if (pos.x == 0)
            {
                hasLava_R = modifiedLavaData.Contains(pos + Vector3.right);
                hasLavaBottom_R = modifiedLavaData.Contains(pos + Vector3.right + Vector3.down);

                hasGround_R = cs.terrainMap[(int)pos.x + 1, (int)pos.y, (int)pos.z] <= mg.terrainSuface;
                hasGroundBottom_R = pos.y == 0 ? true : cs.terrainMap[(int)pos.x + 1, (int)pos.y - 1, (int)pos.z] <= mg.terrainSuface;


                hasLava_L = cs.leftChunk.lavaData.Contains(new Vector3(wgPreset.chunkSize - 1, pos.y, pos.z));
                hasLavaBottom_L = cs.leftChunk.lavaData.Contains(new Vector3(wgPreset.chunkSize - 1, pos.y - 1, pos.z));

                hasGround_L = cs.leftChunk.terrainMap[wgPreset.chunkSize - 1, (int)pos.y, (int)pos.z] <= mg.terrainSuface;
                hasGroundBottom_L = pos.y == 0 ? true : cs.leftChunk.terrainMap[wgPreset.chunkSize - 1, (int)pos.y - 1, (int)pos.z] <= mg.terrainSuface;
            }
            else if (pos.x == wgPreset.chunkSize)
            {
                hasLava_R = cs.rightChunk.lavaData.Contains(new Vector3(1, pos.y, pos.z));
                hasLavaBottom_R = cs.rightChunk.lavaData.Contains(new Vector3(1, pos.y - 1, pos.z));

                hasGround_R = cs.rightChunk.terrainMap[1, (int)pos.y, (int)pos.z] <= mg.terrainSuface;
                hasGroundBottom_R = pos.y == 0 ? true : cs.rightChunk.terrainMap[1, (int)pos.y - 1, (int)pos.z] <= mg.terrainSuface;


                hasLava_L = modifiedLavaData.Contains(pos + Vector3.left);
                hasLava_L = modifiedLavaData.Contains(pos + Vector3.left + Vector3.down);

                hasGround_L = cs.terrainMap[(int)pos.x - 1, (int)pos.y, (int)pos.z] <= mg.terrainSuface;
                hasGroundBottom_L = pos.y == 0 ? true : cs.terrainMap[(int)pos.x - 1, (int)pos.y - 1, (int)pos.z] <= mg.terrainSuface;
            }
            else
            {
                hasLava_R = modifiedLavaData.Contains(pos + Vector3.right);
                hasLavaBottom_R = modifiedLavaData.Contains(pos + Vector3.right + Vector3.down);

                hasGround_R = cs.terrainMap[(int)pos.x + 1, (int)pos.y, (int)pos.z] <= mg.terrainSuface;
                hasGroundBottom_R = pos.y == 0 ? true : cs.terrainMap[(int)pos.x + 1, (int)pos.y - 1, (int)pos.z] <= mg.terrainSuface;


                hasLava_L = modifiedLavaData.Contains(pos + Vector3.left);
                hasLavaBottom_L = modifiedLavaData.Contains(pos + Vector3.left + Vector3.down);

                hasGround_L = cs.terrainMap[(int)pos.x - 1, (int)pos.y, (int)pos.z] <= mg.terrainSuface;
                hasGroundBottom_L = pos.y == 0 ? true : cs.terrainMap[(int)pos.x - 1, (int)pos.y - 1, (int)pos.z] <= mg.terrainSuface;
            }


            if (pos.z == 0)
            {
                hasLava_F = modifiedLavaData.Contains(pos + Vector3.forward);
                hasLavaBottom_F = modifiedLavaData.Contains(pos + Vector3.forward + Vector3.down);

                hasGround_F = cs.terrainMap[(int)pos.x, (int)pos.y, (int)pos.z + 1] <= mg.terrainSuface;
                hasGroundBottom_F = pos.y == 0 ? true : cs.terrainMap[(int)pos.x, (int)pos.y - 1, (int)pos.z + 1] <= mg.terrainSuface;


                hasLava_B = cs.backChunk.lavaData.Contains(new Vector3(pos.x, pos.y, wgPreset.chunkSize - 1));
                hasLavaBottom_B = cs.backChunk.lavaData.Contains(new Vector3(pos.x, pos.y - 1, wgPreset.chunkSize - 1));


                hasGround_B = cs.backChunk.terrainMap[(int)pos.x, (int)pos.y, wgPreset.chunkSize - 1] <= mg.terrainSuface;
                hasGroundBottom_B = pos.y == 0 ? true : cs.backChunk.terrainMap[(int)pos.x, (int)pos.y - 1, wgPreset.chunkSize - 1] <= mg.terrainSuface;
            }
            else if (pos.z == wgPreset.chunkSize)
            {
                hasLava_F = cs.frontChunk.lavaData.Contains(new Vector3(pos.x, pos.y, 1));
                hasLavaBottom_F = cs.frontChunk.lavaData.Contains(new Vector3(pos.x, pos.y - 1, 1));

                hasGround_F = cs.frontChunk.terrainMap[(int)pos.x, (int)pos.y, 1] <= mg.terrainSuface;
                hasGroundBottom_F = pos.y == 0 ? true : cs.frontChunk.terrainMap[(int)pos.x, (int)pos.y - 1, 1] <= mg.terrainSuface;


                hasLava_B = modifiedLavaData.Contains(pos + Vector3.back);
                hasLavaBottom_B = modifiedLavaData.Contains(pos + Vector3.back + Vector3.down);

                hasGround_B = cs.terrainMap[(int)pos.x, (int)pos.y, (int)pos.z - 1] <= mg.terrainSuface;
                hasGroundBottom_B = pos.y == 0 ? true : cs.terrainMap[(int)pos.x, (int)pos.y - 1, (int)pos.z - 1] <= mg.terrainSuface;
            }
            else
            {
                hasLava_F = modifiedLavaData.Contains(pos + Vector3.forward);
                hasLavaBottom_F = modifiedLavaData.Contains(pos + Vector3.forward + Vector3.down);

                hasGround_F = cs.terrainMap[(int)pos.x, (int)pos.y, (int)pos.z + 1] <= mg.terrainSuface;
                hasGroundBottom_F = pos.y == 0 ? true : cs.terrainMap[(int)pos.x, (int)pos.y - 1, (int)pos.z + 1] <= mg.terrainSuface;


                hasLava_B = modifiedLavaData.Contains(pos + Vector3.back);
                hasLavaBottom_B = modifiedLavaData.Contains(pos + Vector3.back + Vector3.down);

                hasGround_B = cs.terrainMap[(int)pos.x, (int)pos.y, (int)pos.z - 1] <= mg.terrainSuface;
                hasGroundBottom_B = pos.y == 0 ? true : cs.terrainMap[(int)pos.x, (int)pos.y - 1, (int)pos.z - 1] <= mg.terrainSuface;
            }

            //go down
            if (goDown)
            {


                modifiedLavaData.Add(pos + Vector3.down);
                modifiedLavaData.RemoveAt(i);
                i--;
                count--;
                modified = true;

                modifiedPos.Add(pos + Vector3.down);
                modifiedPos.Add(pos);
            }
        }


        int modifiedPosCount = modifiedPos.Count;
        for (int i = 0; i < modifiedPosCount; i++)
        {
            Vector3 pos = modifiedPos[i];

            foreach (Vector3 v in surroundingTable)
            {
                Vector3 t = pos + v;
                if (!modifiedPos.Contains(t) && t.x <= wgPreset.chunkSize && t.x >= 0 && t.z <= wgPreset.chunkSize && t.z >= 0 && t.y >= 0 && t.y <= wgPreset.maxHeight)
                {
                    modifiedPos.Add(t);
                }
            }
        }

        if (modified)
        {
            cs.lavaData = modifiedLavaData;
            modifiedChunksDataDictionary.Add(cs, new UpdatedChunkData { cs = cs, modifiedPoses = modifiedPos });
            modifiedChunkDataKeys.Add(cs);
            addedModifiedChunkCount++;
        }
        else
        {
            cs.lavaBeingModified = false;
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
[System.Serializable]
public class UpdatedChunkData
{
    public ChunkScript cs;
    public List<Vector3> modifiedPoses = new List<Vector3>();
}




