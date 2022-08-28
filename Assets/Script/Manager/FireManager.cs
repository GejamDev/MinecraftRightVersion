using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireManager : MonoBehaviour
{
    public UniversalScriptManager usm;
    public float minFireDistance;
    public GameObject firePrefab;

    private void Awake()
    {
    }
    public void LitFire(Vector3 pos, ChunkScript cs)
    {

        Vector3 addingPosInChunk = pos - new Vector3(cs.position.x, 0, cs.position.y);
        foreach(Vector3 v in cs.fireData)
        {
            if(Vector3.Distance(addingPosInChunk, v) <= minFireDistance)
            {
                return;
            }
        }

        cs.fireData.Add(addingPosInChunk);

        GameObject f = Instantiate(firePrefab);
        f.transform.position = pos;
        f.transform.SetParent(cs.objectBundle.transform);

        FireScript fs = f.GetComponent<FireScript>();
        fs.usm = usm;
        fs.parentChunk = cs;
        fs.GetVariables();


        cs.fireDictionary.Add(addingPosInChunk, fs);
    }
}
