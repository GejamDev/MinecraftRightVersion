using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    UniversalScriptManager usm;
    HpManager hm;
    Rigidbody rb;
    ChangeRotAccordingToVelocity changeRot;
    public bool hitted;
    public Item arrowItem;
    public ModifiableObjectType npcType;
    void Awake()
    {
        usm = FindObjectOfType<UniversalScriptManager>();


        hm = usm.hpManager;


        rb = GetComponent<Rigidbody>();
        changeRot = GetComponent<ChangeRotAccordingToVelocity>();
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (!hitted)
        {
            Hit(collision.gameObject);
        }
    }
    public void Hit(GameObject collision)
    {
        hitted = true;
        changeRot.active = false;
        //damage
        if (collision.GetComponent<ModifiableObject>() != null)
        {
            ModifiableObject mo = collision.GetComponent<ModifiableObject>();
            if (mo.type == npcType)
            {
                mo.InteractingScript.Invoke("GetHit_Arrow", 0);
                Destroy(gameObject);
            }
        }
        else if (collision.name == "Player")
        {
            hm.TakeDamage(arrowItem.damage);
            Destroy(gameObject);
        }


        Destroy(gameObject);


        ItemScript itemScript = gameObject.AddComponent<ItemScript>();
        itemScript.grabbableDistance = 2;
        itemScript.holdingSlot = new InventorySlot();
        itemScript.holdingSlot.amount = 1;
        itemScript.holdingSlot.item = arrowItem;
        itemScript.itemBundle = itemScript.transform;


    }
}
