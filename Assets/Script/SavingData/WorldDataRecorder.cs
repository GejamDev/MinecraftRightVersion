using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldDataRecorder : MonoBehaviour
{
    public Dictionary<Vector3Int, float> modifiedTerrainDataDictionary = new Dictionary<Vector3Int, float>();
    public List<Vector3Int> modifiedTerrainDataKeys = new List<Vector3Int>();

    public void RecordTerrainData(Vector3Int pos, float value)
    {
        if (modifiedTerrainDataDictionary.ContainsKey(pos))
        {
            modifiedTerrainDataDictionary[pos] = value;
        }
        else
        {
            modifiedTerrainDataDictionary.Add(pos, value);
            modifiedTerrainDataKeys.Add(pos);
        }
    }
}
