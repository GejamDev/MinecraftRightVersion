using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public UniversalScriptManager usm;
    DimensionTransportationManager dtm;
    HpManager hm;
    SoundManager sm;
    PauseManager pm;
    public bool onFire;
    float fireLeftTime;
    public ParticleSystem fireParticle;
    public Light fireLight;
    public float playedTime;

    void Awake()
    {
        hm = usm.hpManager;
        sm = usm.soundManager;
        dtm = usm.dimensionTransportationManager;
        pm = usm.pauseManager;
    }
    private void Update()
    {
        if(Input.GetKey(KeyCode.Alpha1) && Input.GetKey(KeyCode.Alpha2) && Input.GetKey(KeyCode.Alpha9) && Input.GetKey(KeyCode.Alpha7))
        {
            transform.position = new Vector3(15.3f, 28, 39);
        }

        if (onFire)
        {
            fireLeftTime -= Time.deltaTime;
            if (fireLeftTime <= 0)
            {
                onFire = false;
                fireParticle.Stop();
                fireLight.enabled = false;
            }
        }

        if (!pm.paused)
        {
            playedTime += Time.deltaTime;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("DamageZone"))
        {
            hm.TakeDamage(other.gameObject.GetComponent<IntContainor>().obj);
        }
        if (other.gameObject.CompareTag("Water"))
        {
            onFire = false;
            fireParticle.Stop();
            fireLight.enabled = false;
            sm.PlaySound("WaterSplash", 1);
        }
        else if (other.gameObject.CompareTag("Lava"))
        {

        }
        else if (other.gameObject.CompareTag("NetherPortal"))
        {
            dtm.GoToNether();
        }
    }
    public void LitFire(float time)
    {
        if (!onFire)
        {
            fireParticle.Play();
            fireLight.enabled = true;
            onFire = true;
            fireLeftTime = time;
        }
        else if (fireLeftTime < time)
        {
            fireLeftTime = time;
        }
    }
}
