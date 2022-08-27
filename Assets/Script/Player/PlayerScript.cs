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
    private void Update()
    {
        if(Input.GetKey(KeyCode.Alpha1) && Input.GetKey(KeyCode.Alpha2) && Input.GetKey(KeyCode.Alpha9) && Input.GetKey(KeyCode.Alpha7))
        {
            transform.position = new Vector3(15.3f, 28, 39);
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
            sm.PlaySound("WaterSplash", 1);
        }
        else if (other.gameObject.CompareTag("Lava"))
        {

        }
    }
}
