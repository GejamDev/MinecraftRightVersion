using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public UniversalScriptManager usm;
    InventoryManager im;
    PauseManager pm;


    public List<Sound> soundList = new List<Sound>();

    Dictionary<string, Sound> soundDictionary = new Dictionary<string, Sound>();

    public float masterVolume;
    public float sfxVolume;

    void Awake()
    {
        im = usm.inventoryManager;
        pm = usm.pauseManager;

        foreach (Sound s in soundList)
        {
            soundDictionary.Add(s.audio.name, s);
        }
    }

    public void Update()
    {
        UpdateSoundVolume();
        ClickSound();
    }
    public void UpdateSoundVolume()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume");
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume");
    }
    public void ClickSound()
    {
        if (!im.showingInventoryUI && !pm.paused)
            return;

        if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            PlaySound("Click", 1);
        }
    }

    public void PlaySound(string soundName, float vol)
    {
        if (!soundDictionary.ContainsKey(soundName))
        {
            Debug.Log(soundName + ", sound doesn't exist");
            return;
        }
        Sound playingSound = soundDictionary[soundName];

        float volume = masterVolume;
        volume *= playingSound.defaultVolume;
        volume *= vol;
        switch (playingSound.type)
        {
            case SoundType.sfx:
                volume *= sfxVolume;
                break;
            default:
                break;
        }
        GameObject prefab = new GameObject("Sound : " + soundName);
        AudioSource source = prefab.AddComponent<AudioSource>();
        source.volume = volume;

        source.clip = playingSound.audio;
        source.Play();

        Destroy(prefab, playingSound.audio.length);

    }
}

public enum SoundType
{
    sfx
}

[System.Serializable]
public class Sound
{
    public AudioClip audio;
    [Range(0, 1)]
    public float defaultVolume = 1;
    public SoundType type;
}

[System.Serializable]
public class BGM
{
    public AudioClip audio;
    [Range(0, 1)]
    public float defaultVolume = 1;
}
