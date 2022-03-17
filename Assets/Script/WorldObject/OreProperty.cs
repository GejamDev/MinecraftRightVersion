using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class OreProperty : ScriptableObject
{
    public string Name;

    public int maxYSpawnLevel;
    public AnimationCurve spawnCountGraph;
    public Vector3 minSize;
    public Vector3 maxSize;

    public string objectName;
}
