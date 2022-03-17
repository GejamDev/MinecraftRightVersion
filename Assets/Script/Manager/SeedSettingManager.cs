using UnityEngine;
using UnityEngine.UI;

public class SeedSettingManager : MonoBehaviour
{
    public InputField input;
    void Awake()
    {
        PlayerPrefs.SetInt("RandomSeed", 1);
    }

    void Update()
    {
        int seed = 0;
        if (input.text == "")
        {
            PlayerPrefs.SetInt("RandomSeed", 1);
        }
        else if (int.TryParse(input.text, out seed))
        {
            PlayerPrefs.SetInt("RandomSeed", 0);
            PlayerPrefs.SetInt("Seed", seed);
        }
        else
        {
            PlayerPrefs.SetInt("RandomSeed", 1);
        }
    }
}
