using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ExplosionPreset : ScriptableObject
{

    [Header("Particle")]
    public GameObject explosionParticle;

    [Header("Destruction")]
    public float DestructionRadius;
    public bool canDestroyObjectsInRadius;

    [Header("Damage")]
    public float maxDamageDistance;
    public AnimationCurve damageByDistance;

    [Header("KnockBack")]
    public AnimationCurve knockBackPower;

    [Header("Cam Shake")]
    public float camShakeTime;
    public float camShakePower;

    [Header("Sound")]
    public float volume;
}
