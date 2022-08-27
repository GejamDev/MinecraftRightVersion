using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraManager : MonoBehaviour
{
    public UniversalScriptManager usm;
    FirstPersonController fpc;
    public int fps;
    public Camera playerCam;
    public float currentShakePower;
    public float minShakePower;
    public float maxShakePower;

    public Vector3 targetOffset;
    public float synchronizeSpeed;

    public PostProcessVolume volume;
    public PostProcessVolume volume_underWater;
    public PostProcessVolume volume_underLava;

    Bloom bloom;
    Vignette vignette;
    AmbientOcclusion ambient;
    ColorGrading colorGrading;
    public float underWaterApplySpeed;
    public float underLavaApplySpeed;
    void Awake()
    {
        playerCam = Camera.main;
        fpc = usm.firstPersonController;
        StartCoroutine(CamShakeUpdate());

    }
    public IEnumerator CamShakeUpdate()
    {
        while (true)
        {
            if(currentShakePower >= minShakePower)
            {
                targetOffset = new Vector3(Random.Range(-360, 360), Random.Range(-360, 360), Random.Range(-360, 360)) * Mathf.Clamp(currentShakePower, 0, maxShakePower) / 360;
            }
            else
            {
                currentShakePower = 0;
                targetOffset = Vector3.zero;
            }
            yield return new WaitForSeconds((float)1 / fps);
        }
    }
    void Update()
    {
        fpc.camShakeOffset = Vector3.Lerp(fpc.camShakeOffset, targetOffset, synchronizeSpeed * Time.deltaTime);

        bool postProcessingOn = PlayerPrefs.GetInt("GoodGraphic") == 1;

        volume.isGlobal = postProcessingOn;
        volume_underWater.isGlobal = postProcessingOn;
    }
    private void FixedUpdate()
    {
        UpdatePP();
    }
    public void ShakeCamera(float time, float power, bool fadeness, float delay)
    {
        StartCoroutine(ShakeCamera_Cor(time, power, fadeness, delay));
    }
     IEnumerator ShakeCamera_Cor(float time, float power, bool fade, float delay)
    {
        if (delay != 0)
            yield return new WaitForSeconds(delay);
        currentShakePower += power;
        if (fade)
        {
            int loopCount = Mathf.RoundToInt(time * fps);
            for(int i = 0; i < loopCount; i++)
            {
                currentShakePower -= (power / loopCount);
                yield return new WaitForSeconds((float)1 / fps);
            }

        }
        else
        {
            yield return new WaitForSeconds(time);
            currentShakePower -= power;
        }

    }

    public void UpdatePP()
    {
        if (volume_underWater.weight < 0.1 && !fpc.headInWater)
        {
            volume_underWater.weight = 0;
        }
        volume_underWater.weight = Mathf.Clamp(Mathf.Lerp(volume_underWater.weight, fpc.headInWater ? 1 : 0, underWaterApplySpeed * Time.deltaTime),0,1);

        if (volume_underLava.weight < 0.1 && !fpc.headInLava)
        {
            volume_underLava.weight = 0;
        }
        volume_underLava.weight = Mathf.Clamp(Mathf.Lerp(volume_underLava.weight, fpc.headInLava ? 1 : 0, underLavaApplySpeed * Time.deltaTime), 0, 1);
    }

}
