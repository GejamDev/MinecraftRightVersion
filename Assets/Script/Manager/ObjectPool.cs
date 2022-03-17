using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public UniversalScriptManager usm;
    ChunkLoader cl;
    
    public int maxSpawningObjectsPerFrame;
    

    public List<PoolObject> poolObjectList = new List<PoolObject>();
    void Awake()
    {
        cl = usm.chunkLoader;

        int multiplyingCount = 841;
        //pre generate
        for (int i = 0; i < poolObjectList.Count; i++)
        {
            poolObjectList[i].pool = new GameObject[poolObjectList[i].maxCount * multiplyingCount];

            StartCoroutine(GenerateObjectsInPool(i));
        }
    }
    IEnumerator GenerateObjectsInPool(int index)
    {
        int instantiatedCount = 0;
        for (int i = 0; i < poolObjectList[index].pool.Length; i++)
        {
            GameObject g = Instantiate(poolObjectList[index].originalPrefab);
            g.transform.SetParent(transform);
            poolObjectList[index].pool[i] = g;
            g.SetActive(false);
            instantiatedCount++;
            if (instantiatedCount >= maxSpawningObjectsPerFrame)
            {
                yield return new WaitForEndOfFrame();
                instantiatedCount = 0;
            }
        }
    }

    public GameObject GetObject(string objType)
    {
        for(int i =0; i < poolObjectList.Count; i++)
        {
            if(poolObjectList[i].type == objType)
            {
                foreach(GameObject g in poolObjectList[i].pool)
                {
                    if (!g.activeSelf)
                    {
                        g.SetActive(true);
                        return g;
                    }
                }
                Debug.LogError("Object Pool : ran out of object,  type : " + objType);
                return null;
            }
            else if(i == poolObjectList.Count - 1)
            {
                Debug.LogError("Object Pool : type of " + objType + " doesn't exist");
                return null;
            }
        }
        return null;
    }
}

[System.Serializable]
public class PoolObject
{
    public string type;
    public GameObject originalPrefab;
    public int maxCount;
    public GameObject[] pool;
}
