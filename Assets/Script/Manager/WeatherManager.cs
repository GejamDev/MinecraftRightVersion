using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weather
{
    Sunny,
    Raining
}
public class WeatherManager : MonoBehaviour
{
    public UniversalScriptManager usm;
    DimensionTransportationManager dtm;

    [Header("Settings")]
    public Weather[] weatherArray;

    public float WeatherChangeSpeed_Min;
    public float WeatherChangeSpeed_Max;

    public float rainDecreaseSpeed;
    public float rainIncreaseSpeed;

    public LightingPreset rainLightSetting;
    public float maxLightImpact;


    [Header("Status")]
    public Weather currentWeather;
    [Range(0, 1)] public float rainPower;
    public AnimationCurve rainAmountCurve;
    public AnimationCurve rainSpeedCurve;


    [Header("Variables")]
    public ParticleSystem rainParticle;
    public Transform rainPosition;
    GameObject player;

    void Awake()
    {
        dtm = usm.dimensionTransportationManager;
        player = usm.player;
        ChangeWeatherRandomly();
    }

    public void Update()
    {
        var rainParticleEmission = rainParticle.emission;
        var rainParticleMain = rainParticle.main;
        if (dtm.currentDimesnion != Dimension.OverWorld)
        {
            rainParticleEmission.rateOverTime = 0;
            rainParticleMain.simulationSpeed = 0;
            rainParticle.gameObject.SetActive(false);
            return;
        }
        rainParticle.gameObject.SetActive(true);



        if (currentWeather == Weather.Sunny)
        {
            if (rainPower <=0)
            {
                rainPower = 0;
            }
            else
            {
                rainPower -= rainDecreaseSpeed * Time.deltaTime;
            }
        }
        else if(currentWeather == Weather.Raining)
        {
            if (rainPower >= 1)
            {
                rainPower = 1;
            }
            else
            {
                rainPower += rainIncreaseSpeed * Time.deltaTime;
            }
        }

        rainParticle.transform.position = rainPosition.position;

        float rainAmount = rainAmountCurve.Evaluate(rainPower);
        rainParticleEmission.rateOverTime = rainAmount;


        float rainSpeed = rainSpeedCurve.Evaluate(rainPower);
        rainParticleMain.simulationSpeed = rainSpeed;

    }

    public void ChangeWeatherRandomly()
    {
        currentWeather = weatherArray[Random.Range(0, weatherArray.Length)];
        Invoke(nameof(ChangeWeatherRandomly), Random.Range(WeatherChangeSpeed_Min, WeatherChangeSpeed_Max));
    }
}
