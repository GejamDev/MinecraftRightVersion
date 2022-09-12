using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
public class LightingManager : MonoBehaviour
{
    public UniversalScriptManager usm;
    DimensionTransportationManager dtm;


    //Scene References
    public Light DirectionalLight;
    public LightingPreset Preset;
    public LightingPreset Preset_nether;
    public LightingPreset Preset_darken;
    public Material skybox;
    public ColorGrading colorGrading;
    public GameObject SunMoon;
    public Image blackColor;
    public DynamicSky dynamicSky;
    public GameObject cloud;
    WeatherManager wm;
    BrightnessDetector bd;

    //Variables
    [Range(0, 24)] public float TimeOfDay;
    public float timeSpeed;

    //day night
    [Header("Day/Night")]
    public float nightStartTime;
    public float nightEndTime;
    public bool isNight;

    void Awake()
    {
        wm = usm.weatherManager;
        dtm = usm.dimensionTransportationManager;
        bd = usm.brightnessDetector;
    }

    private void Update()
    {
        transform.position = usm.player.transform.position;
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
        LightingPreset curLP = Preset;
        if (dtm.currentDimesnion == Dimension.Nether)
        {
            curLP = Preset_nether;
            cloud.SetActive(false);
        }
        else
        {
            cloud.SetActive(!isNight);
        }

        if (SunMoon != null)
        {
            SunMoon.transform.eulerAngles = new Vector3(0, 0, curLP.SunMoonRotation.Evaluate(timePercent));
        }


        RenderSettings.fogColor = DarkenByBrightness(BlendWithWeather(curLP.FogColor.Evaluate(timePercent), wm.rainLightSetting.FogColor.Evaluate(0)), Preset_darken.FogColor.Evaluate(0));
        RenderSettings.ambientIntensity = Mathf.Lerp(curLP.lightingIntensity.Evaluate(timePercent), Preset_darken.lightingIntensity.Evaluate(0), 1 - bd.brightness);
        if (DirectionalLight != null)
        {
            DirectionalLight.color = DarkenByBrightness(BlendWithWeather(curLP.DirectionalColor.Evaluate(timePercent), wm.rainLightSetting.DirectionalColor.Evaluate(0)), Preset_darken.DirectionalColor.Evaluate(0));
        }
        if (skybox != null)
        {
            skybox.SetColor("_Tint", DarkenByBrightness(BlendWithWeather(curLP.skyColor.Evaluate(timePercent), wm.rainLightSetting.skyColor.Evaluate(0)),Preset_darken.skyColor.Evaluate(0) ));
        }
        if(blackColor != null)
        {
            blackColor.color = curLP.blackColor.Evaluate(timePercent);
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
        if(wm == null)
        {
            wm = usm.weatherManager;
        }
    }

    public Color BlendWithWeather(Color origin, Color weather)
    {
        if (dtm.currentDimesnion != Dimension.OverWorld)
        {
            return origin;
        }
        float rainPower = wm.rainPower * wm.maxLightImpact;

        return origin * (1 - rainPower) + weather * rainPower;


    }
    public Color DarkenByBrightness(Color origin, Color darken)
    {
        return origin * (bd.brightness) + darken * (1 - bd.brightness);
    }
}