using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    //Scene References
    public Light DirectionalLight;
    public LightingPreset Preset;
    public Material skybox;
    public PostProcessVolume volume;
    public ColorGrading colorGrading;
    public GameObject SunMoon;
    public Image blackColor;

    //Variables
    [Range(0, 24)] public float TimeOfDay;
    public float timeSpeed;

    //day night
    [Header("Day/Night")]
    public float nightStartTime;
    public float nightEndTime;
    public bool isNight;


    private void Update()
    {
        if (Preset == null)
            return;

        if (Application.isPlaying)
        {
            //(Replace with a reference to the game time)
            if (Time.timeScale != 0)
            {
                TimeOfDay += Time.deltaTime * timeSpeed;
            }
            TimeOfDay %= 24; //Modulus to ensure always between 0-24
            UpdateLighting(TimeOfDay / 24f);
            isNight = TimeOfDay <= nightEndTime && TimeOfDay >= nightStartTime;
        }
        else
        {
            UpdateLighting(TimeOfDay / 24f);
        }
    }


    private void UpdateLighting(float timePercent)
    {
        if (SunMoon != null)
        {
            SunMoon.transform.eulerAngles = new Vector3(0, 0, Preset.SunMoonRotation.Evaluate(timePercent));
        }


        RenderSettings.fogColor = Preset.FogColor.Evaluate(timePercent);
        RenderSettings.ambientIntensity = Preset.lightingIntensity.Evaluate(timePercent);
        if (DirectionalLight != null)
        {
            DirectionalLight.color = Preset.DirectionalColor.Evaluate(timePercent);
        }
        if (skybox != null)
        {
            skybox.SetColor("_Tint", Preset.skyColor.Evaluate(timePercent));
        }
        if(blackColor != null)
        {
            blackColor.color = Preset.blackColor.Evaluate(timePercent);
        }
    }

    //Try to find a directional light to use if we haven't set one
    private void OnValidate()
    {
        if (DirectionalLight == null)
        {

            //Search for lighting tab sun
            if (RenderSettings.sun != null)
            {
                DirectionalLight = RenderSettings.sun;
            }
            //Search scene for light that fits criteria (directional)
            else
            {
                Light[] lights = GameObject.FindObjectsOfType<Light>();
                foreach (Light light in lights)
                {
                    if (light.type == LightType.Directional)
                    {
                        DirectionalLight = light;
                        return;
                    }
                }
            }
        }
    }
}