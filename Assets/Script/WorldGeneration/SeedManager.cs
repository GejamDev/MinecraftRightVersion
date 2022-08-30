using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedManager : MonoBehaviour
{
    public int seed;
    public bool setted;
    void Awake()
    {
        if (setted)
            return;
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
    public void ChangeSeed(int s)
    {
        setted = true;
        seed = s;
        Noise.seed = s;
    }
}
