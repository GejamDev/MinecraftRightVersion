﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Custom_R_WaterBucket : MonoBehaviour
{
    public UniversalScriptManager usm;
    public Item item;
    public Animator handAnim;
    WaterManager wm;
    InventoryManager im;
    Transform cam;
    public float maxDistance;
    public float pourRadius;
    public float delay;
    public LayerMask groundLayer;
    public Item bucket;

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

        RaycastHit hit_ground;
        if (Physics.Raycast(cam.position, cam.forward, out hit_ground, maxDistance, groundLayer))
        {
            Vector3 pouringPos = hit_ground.point;// + Vector3.down;

            ChunkScript chunk = hit_ground.collider.transform.parent.parent.GetComponent<ChunkScript>();


            //pour water
            StartCoroutine(wm.Pourwater(chunk, pouringPos, pourRadius, delay));


            //swap item
            im.inventoryDictionary[usedCell].amount--;
            if (im.inventoryDictionary[usedCell].amount <= 0)
            {
                im.inventoryDictionary[usedCell].item = bucket;
                im.inventoryDictionary[usedCell].amount = 1;
            }
            else
            {
                im.ObtainItem(new InventorySlot { item = bucket, amount = 1 });
            }
            usedCell.UpdateCell();
            im.UpdateSeletedSlot();
        }
    }
}
