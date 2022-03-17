using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedManager : MonoBehaviour
{
    public int seed;
    void Awake()
    {
        if (PlayerPrefs.GetInt("RandomSeed") == 1)
        {
            seed = Random.Range(int.MinValue, int.MaxValue);
        }
        else
        {
            seed = PlayerPrefs.GetInt("Seed");
        }

        Noise.seed = seed;
    }
}
