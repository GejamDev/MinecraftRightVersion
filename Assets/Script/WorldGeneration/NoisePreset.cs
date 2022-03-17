using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class NoisePreset : ScriptableObject
{
    public float noiseSpeed;
    public float noiseWeight;
    public float groundLevel;
    public int octaves;
    public float persistance;
    public float lacunarity;
    public AnimationCurve heightMultiplier;
}
