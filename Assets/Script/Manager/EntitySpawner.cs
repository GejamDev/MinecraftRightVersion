using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySpawner : MonoBehaviour
{
    public UniversalScriptManager usm;
    LightingManager lm;
    ChunkLoader cl;
    GameObject player;

    public float minSpawnRange;
    public float spawnRange;

    public List<EntityProperty> entityList;
    public List<PreSpawningEntityProperty> preSpawnList;

    void Awake()
    {
        lm = usm.lightingManager;
        cl = usm.chunkLoader;
        player = usm.player;


        foreach(EntityProperty ep in entityList)
        {
            StartCoroutine(SpawnCoroutine(ep));
        }
    }
    IEnumerator SpawnCoroutine(EntityProperty ep)
    {

        while (true)
        {
            yield return new WaitUntil(() => lm.isNight || !ep.onlySpawnAtNight);
            yield return new WaitForSeconds(Random.Range(ep.minSpawnDelay, ep.maxSpawnDelay));

            //spawn
            GameObject e = Instantiate(ep.entityObj);
            Vector3 spawnPos = player.transform.position;
            Vector3 randomness = new Vector3(Random.Range(-100, 100), Random.Range(-10, 10), Random.Range(-100, 100)).normalized;
            randomness *= Random.Range(minSpawnRange,spawnRange);

            spawnPos += randomness;
            e.transform.position = spawnPos;



            Vector2 chunkPosition = new Vector2(

                spawnPos.x >= 0 ?
                Mathf.Floor(spawnPos.x / 8) * 8 :
                Mathf.Floor(-spawnPos.x / 8) * -8 - 8,

                spawnPos.z >= 0 ?
                Mathf.Floor(spawnPos.z / 8) * 8 :
                Mathf.Floor(-spawnPos.z / 8) * -8 - 8
                );

            if (cl.chunkDictionary.ContainsKey(chunkPosition))
            {
                ChunkScript cs = cl.chunkDictionary[chunkPosition].cs;
                Vector2Int posInChunk = new Vector2Int(Mathf.RoundToInt(spawnPos.x - cs.position.x), Mathf.RoundToInt(spawnPos.z - cs.position.y));
                float yHeight = cs.heightMap[posInChunk.x, posInChunk.y];
                if(spawnPos.y > yHeight)
                {
                    e.transform.position = new Vector3(spawnPos.x, yHeight, spawnPos.z);
                }
            }

        }
    }

    public void SpawnEntities(ChunkScript cs)
    {
        StartCoroutine(WaitForChunkMeshGeneratingEnd(cs));
    }
    IEnumerator WaitForChunkMeshGeneratingEnd(ChunkScript cs)
    {
        yield return new WaitUntil(() => !cs.generatingMesh);

        foreach (PreSpawningEntityProperty psep in preSpawnList)
        {
            float value = Random.Range(0, 1f);
            int spawnCount = Mathf.RoundToInt(psep.spawnCurve.Evaluate(value));
            if (spawnCount >= 1)
            {
                GameObject e = Instantiate(psep.entityObj);
                Vector3 spawnPos = new Vector3(Random.Range(0, 7.9f), 0, Random.Range(0, 7.9f));
                float y = cs.heightMap[Mathf.RoundToInt(spawnPos.x), Mathf.RoundToInt(spawnPos.z)];
                spawnPos += Vector3.up * y;
                e.transform.SetParent(cs.objectBundle.transform);
                e.transform.localPosition = spawnPos;
                e.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
            }
        }
    }
}

[System.Serializable]
public class EntityProperty
{
    public GameObject entityObj;
    public bool onlySpawnAtNight;
    public float minSpawnDelay;
    public float maxSpawnDelay;
}

[System.Serializable]
public class PreSpawningEntityProperty
{
    public GameObject entityObj;
    public AnimationCurve spawnCurve;
}