using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Custom_R_Bow : MonoBehaviour
{
    public UniversalScriptManager usm;
    InventoryManager im;



    Camera cam;

    public Item bowItem;
    public Item arrowItem;
    public GameObject arrowPrefab;
    public InventoryCell usingCell;




    public bool inUse;
    public float maxChargeTime;
    public float minChargeTime;
    public float shootOffset;
    public AnimationCurve shootPowerCurve;



    bool preUseState;
    float chargedTime;


    void Awake()
    {
        im = usm.inventoryManager;

        cam = Camera.main;
    }


    void Update()
    {
        inUse = false;
    }
    public void Use()
    {
        if(im.HasItem(arrowItem, out usingCell))
        {
            inUse = true;
        }

    }
    void LateUpdate()
    {
        if (inUse)
        {
            if (chargedTime >= maxChargeTime)
            {
                chargedTime = maxChargeTime;
            }
            else
            {
                chargedTime += Time.deltaTime;
            }
        }
        else
        {
            if (preUseState && !inUse && chargedTime >= minChargeTime)
            {
                Shoot();
            }


            chargedTime = 0;
        }
        preUseState = inUse;
    }
    public void Shoot()
    {
        if(im.currentlyUsingInventorySlot.item == bowItem)
        {
            GameObject arrow = Instantiate(arrowPrefab);
            arrow.transform.position = cam.transform.position + (cam.transform.forward).normalized * shootOffset;
            arrow.transform.rotation = cam.transform.rotation;

            Rigidbody rb = arrow.GetComponent<Rigidbody>();
            rb.AddForce(arrow.transform.forward * shootPowerCurve.Evaluate(chargedTime), ForceMode.Impulse);

            im.inventoryDictionary[usingCell].amount--;
            usingCell.UpdateCell();
        }
    }
}
