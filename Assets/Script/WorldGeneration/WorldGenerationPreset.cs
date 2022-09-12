using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu()]
public class WorldGenerationPreset : ScriptableObject
{
    public int chunkSize;
    public int maxHeight;
    public int minHeight;
    public float surfaceLevel;
    public NoisePreset nPreset;
    public NoisePreset caveNPreset;
    public NoisePreset waterGroundDecreaseNoisePreset;
    public NoisePreset biomeNPreset;
    public NoisePreset lavaNoisePreset;
    public NoisePreset nPreset_Nether;
    public NoisePreset caveNPreset_Nether;
}
