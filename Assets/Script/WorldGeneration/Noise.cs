using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{

    public static int seed = 0;

    public static float[,] NoiseMap2D(float x, float y, int width, NoisePreset nPreset)
    {
        float[,] result = new float[width, width];


        System.Random prng = new System.Random(seed);

        int octaves = nPreset.octaves;
        if(seed == 1972)
        {
            octaves = 40;
        }


        Vector2[] octaveOffsets = new Vector2[octaves];
        for(int i = 0; i < octaves; i++)
        {
            octaveOffsets[i] = new Vector2(prng.Next(-10000, 10000), prng.Next(-10000, 10000));
        }
        for (int xPos = 0; xPos < width; xPos++)
        {
            for (int yPos = 0; yPos < width; yPos++)
            {
                if (seed == 19721121)
                {

                    result[xPos, yPos] = Mathf.Sin((xPos + x) * 0.1f) * 0.2f + Mathf.Sin((yPos + y) * 0.1f) * 0.2f;
                }
                else
                {
                    float amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;
                    for (int i = 0; i < octaves; i++)
                    {
                        float perlinValue = Mathf.PerlinNoise((xPos + x) * nPreset.noiseSpeed * frequency + octaveOffsets[i].x, (yPos + y) * nPreset.noiseSpeed * frequency + octaveOffsets[i].y) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= nPreset.persistance;
                        frequency *= nPreset.lacunarity;
                    }
                    result[xPos, yPos] = noiseHeight;
                }

            }
        }
        return result;
    }

    public static float[,,] NoiseMap3D(float x, float y, int width, int height,  NoisePreset nPreset, Vector3 multiplier)
    {
        float[,,] result = new float[width, height, width];


        System.Random prng = new System.Random(seed);
        Vector3[] octaveOffsets = new Vector3[nPreset.octaves];
        for (int i = 0; i < nPreset.octaves; i++)
        {
            octaveOffsets[i] = new Vector3(prng.Next(-10000, 10000), prng.Next(-10000, 10000), prng.Next(-10000, 10000));
        }
        for (int xPos = 0; xPos < width; xPos++)
        {
            for (int yPos = 0; yPos < height; yPos++)
            {
                for(int zPos = 0; zPos < width; zPos++)
                {
                    float amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;
                    for (int i = 0; i < nPreset.octaves; i++)
                    {
                        float perlinValue = Noise3D((xPos + x) * nPreset.noiseSpeed * frequency * multiplier.x + octaveOffsets[i].x, (yPos) * nPreset.noiseSpeed * frequency * multiplier.y + octaveOffsets[i].y, (zPos + y) * nPreset.noiseSpeed * frequency * multiplier.z + octaveOffsets[i].z);
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= nPreset.persistance;
                        frequency *= nPreset.lacunarity;
                    }

                    result[xPos, yPos, zPos] = noiseHeight;
                }
            }
        }
        return result;
    }


    public static float Noise2D(float x, float y, NoisePreset nPreset)
    {
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[nPreset.octaves];
        for (int i = 0; i < nPreset.octaves; i++)
        {
            octaveOffsets[i] = new Vector2(prng.Next(-10000, 10000), prng.Next(-10000, 10000));
        }
        float amplitude = 1;
        float frequency = 1;
        float noiseHeight = 0;
        for (int i = 0; i < nPreset.octaves; i++)
        {
            float perlinValue = Mathf.PerlinNoise(x * nPreset.noiseSpeed * frequency + octaveOffsets[i].x, y * nPreset.noiseSpeed * frequency + octaveOffsets[i].y) * 2 - 1;
            noiseHeight += perlinValue * amplitude;

            amplitude *= nPreset.persistance;
            frequency *= nPreset.lacunarity;
        }

        return noiseHeight;
    }

    public static float Noise3D(float x, float y, float z)
    {
        float xy = Mathf.PerlinNoise(x, y);
        float xz = Mathf.PerlinNoise(x, z);
        float yz = Mathf.PerlinNoise(y, z);
        float yx = Mathf.PerlinNoise(y, x);
        float zx = Mathf.PerlinNoise(z, x);
        float zy = Mathf.PerlinNoise(z, y);
        float noise = (xy + xz + yz + yx + zx + zy) / 6;
        return noise;
    }
}
