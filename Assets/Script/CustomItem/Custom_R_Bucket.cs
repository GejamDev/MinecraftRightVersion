﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Custom_R_Bucket : MonoBehaviour
{
    public UniversalScriptManager usm;
    public Item item;
    public Animator handAnim;
    WaterManager wm;
    InventoryManager im;
    Transform cam;
    public float maxDistance;
    public float drainRadius;
    public float delay;
    public LayerMask waterLayer;
    public Item waterBucket;
    
    private void Awake()
    {
        cam = Camera.main.transform;

        wm = usm.waterManager;
        im = usm.inventoryManager;
    }
    public void Use()
    {
        InventoryCell usedCell = im.inventoryCellList[im.curInventorySlot];
        handAnim.SetTrigger(item.usingAnimationName);


        RaycastHit hit_water;
        if (Physics.Raycast(cam.position, cam.forward, out hit_water, maxDistance, waterLayer))
        {
            Vector3 dryingPos = hit_water.point;

            ChunkScript chunk = hit_water.collider.transform.parent.parent.GetComponent<ChunkScript>();
            StartCoroutine(wm.DrainWater(chunk, dryingPos, drainRadius, delay));


            //swap item
            im.inventoryDictionary[usedCell].amount--;
            if (im.inventoryDictionary[usedCell].amount <= 0)
            {
                im.inventoryDictionary[usedCell].item = waterBucket;
                im.inventoryDictionary[usedCell].amount = 1;
            }
            else
            {
                im.ObtainItem(new InventorySlot { item = waterBucket, amount = 1 });
            }
            usedCell.UpdateCell();
            im.UpdateSeletedSlot();
        }
    }
}
