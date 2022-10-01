using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionManager : MonoBehaviour
{
    public UniversalScriptManager usm;

    GameObject player;
    TerrainModifier tm;
    CameraManager cm;
    HpManager hm;
    FirstPersonController fpc;
    SoundManager sm;
    DimensionTransportationManager dtm;

    public bool hasExplosion;
    public Vector3 explosionPos;
    public float explosionRadius;

    void Awake()
    {
        tm = usm.terrainModifier;
        cm = usm.cameraManager;
        player = usm.player;
        hm = usm.hpManager;
        fpc = usm.firstPersonController;
        sm = usm.soundManager;
        dtm = usm.dimensionTransportationManager;
    }
    void Update()
    {
        hasExplosion = false;
    }




    public void CauseExplosion(Vector3 position, ExplosionPreset expPreset)
    {
        //particle
        GameObject particle = Instantiate(expPreset.explosionParticle, position, Quaternion.identity);


        //ground destruction
        tm.Destruct_Custom(position, expPreset.DestructionRadius, dtm.currentDimesnion);


        //object destruction
        hasExplosion = true;
        explosionPos = position;
        explosionRadius = expPreset.DestructionRadius;


        //damage
        float playerDistance = Vector3.Distance(position, player.transform.position);
        if (playerDistance <= expPreset.maxDamageDistance)
        {
            int damage = Mathf.CeilToInt(expPreset.damageByDistance.Evaluate(playerDistance));
            hm.TakeDamage(damage);
        }

        //knock back
        fpc.KnockBack((player.transform.position - position + Vector3.up).normalized * expPreset.knockBackPower.Evaluate(playerDistance));


        //cam shake
        cm.ShakeCamera(expPreset.camShakeTime, expPreset.camShakePower, true, 0);

        //sound
        sm.PlaySound("explosion", expPreset.volume);
    }
}
