using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BiomeManager : MonoBehaviour
{
    public UniversalScriptManager usm;
    public BiomeProperty[] biomes;
    WorldGenerationPreset wgPreset;
    NoisePreset np;

    void Awake()
    {
        wgPreset = usm.worldGenerationPreset;
        np = wgPreset.biomeNPreset;
    }

    public BiomeProperty AssignBiome(ChunkScript cs)
    {
        //float noiseValue = Mathf.PerlinNoise((cs.position.x - 50) * 0.001f, cs.position.y * 0.001f);
        float noiseValue = Noise.Noise2D(cs.position.x, cs.position.y, np);

        BiomeProperty b = biomes[0];
        for(int i = 0; i < biomes.Length; i++)
        {
            if(noiseValue > biomes[i].noiseValue)
            {
                b = biomes[i];
                break;
            }
        }
        cs.biomeProperty = b;

        return b;
    }
}

[System.Serializable]
public class BiomeProperty
{
    public string name;
    public float noiseValue;
    public Material terrainMaterial;
    public bool hasTree;
    public bool hasGrass;
    public GameObject grassObject;
    public NoisePreset grassNoisePreset;
    public float minGrassYScale;
    public GameObject treeObject;
    public NoisePreset treeNoisePreset;
    public GameObject groundDestroyParticle;
}