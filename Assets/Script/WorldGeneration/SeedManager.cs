using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedManager : MonoBehaviour
{
    public int seed;

    public void SetSeed_NewWorld()
    {
        if (PlayerPrefs.GetInt("RandomSeed") == 1)
        {
            seed = Random.Range(int.MinValue, int.MaxValue);
        }
        else
        {
            Debug.Log("got determined seed");
            seed = PlayerPrefs.GetInt("LastSeedInput");
        }
        Noise.seed = seed;
    }
    public void ChangeSeed(int s)
    {
        seed = s;
        Noise.seed = s;
    }
}
