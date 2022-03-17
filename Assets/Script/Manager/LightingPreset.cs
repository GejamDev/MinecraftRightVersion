using UnityEngine;

[System.Serializable]
[CreateAssetMenu()]
public class LightingPreset : ScriptableObject
{
    public Gradient DirectionalColor;
    public Gradient FogColor;
    public Gradient skyColor;
    public Gradient blackColor;
    public AnimationCurve lightingIntensity;
    public AnimationCurve SunMoonRotation;
}