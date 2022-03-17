using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    public List<Setting_OnOff> setting_OnOff = new List<Setting_OnOff>();
    public List<Setting_Slider> setting_Slider = new List<Setting_Slider>();

    void Awake()
    {
        foreach(Setting_OnOff so in setting_OnOff)
        {
            if (PlayerPrefs.HasKey(so.saveFile))
            {
                so.b.value = PlayerPrefs.GetInt(so.saveFile) == 1;
            }
            else
            {
                so.b.value = so.defaultValue;
                PlayerPrefs.SetInt(so.saveFile, so.defaultValue ? 1 : 0);
            }
        }
        foreach(Setting_Slider ss in setting_Slider)
        {
            if (PlayerPrefs.HasKey(ss.saveFile))
            {
                ss.s.s.value = PlayerPrefs.GetFloat(ss.saveFile);
            }
            else
            {
                ss.s.s.value = ss.defaultValue;
                PlayerPrefs.SetFloat(ss.saveFile, ss.defaultValue);
            }
        }
    }
    void Update()
    {
        foreach (Setting_OnOff so in setting_OnOff)
        {
            if (so.b.valueChanged)
            {
                so.b.valueChanged = false;
                PlayerPrefs.SetInt(so.saveFile, so.b.value ? 1 : 0);
            }
        }
        foreach (Setting_Slider ss in setting_Slider)
        {
            if (ss.s.valueChanged)
            {
                ss.s.valueChanged = false;
                PlayerPrefs.SetFloat(ss.saveFile, ss.s.s.value);
            }
        }
    }

}

[System.Serializable]
public class Setting_OnOff
{
    public OnOffButton b;
    public string saveFile;
    public bool defaultValue;
}
[System.Serializable]
public class Setting_Slider
{
    public ValueSlider s;
    public string saveFile;
    public float defaultValue;
}
