using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public UniversalScriptManager usm;
    HpManager hm;
    SoundManager sm;

    void Awake()
    {
        hm = usm.hpManager;
        sm = usm.soundManager;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("DamageZone"))
        {
            hm.TakeDamage(other.gameObject.GetComponent<IntContainor>().obj);
        }
        else if (other.gameObject.CompareTag("Water"))
        {
            sm.PlaySound("WaterSplash", 1);
        }
    }
}
