using System.Collections;
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
    LavaManager lm;
    public float maxDistance;
    public float drainRadius;
    public float delay;
    public LayerMask waterLayer;
    public LayerMask lavaLayer;
    public Item waterBucket;
    public Item lavaBucket;

    private void Awake()
    {
        cam = Camera.main.transform;

        wm = usm.waterManager;
        im = usm.inventoryManager;
        lm = usm.lavaManager;
    }
    public void Use()
    {
        InventoryCell usedCell = im.inventoryCellList[im.curInventorySlot];
        handAnim.SetTrigger(item.usingAnimationName);


        RaycastHit hit_water;
        RaycastHit hit_lava;
        bool waterDetected = Physics.Raycast(cam.position, cam.forward, out hit_water, maxDistance, waterLayer);
        bool lavaDetected = Physics.Raycast(cam.position, cam.forward, out hit_lava, maxDistance, lavaLayer);
        void DoWater()
        {
            Vector3 dryingPos = hit_water.point;

            ChunkScript chunk = hit_water.collider.transform.parent.parent.parent.GetComponent<ChunkScript>();
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
        void DoLava()
        {
            Debug.Log(1);
            Vector3 dryingPos = hit_lava.point;

            ChunkScript chunk = hit_lava.collider.transform.parent.parent.parent.GetComponent<ChunkScript>();
            StartCoroutine(lm.DrainLava(chunk, dryingPos, drainRadius, delay));


            //swap item 
            im.inventoryDictionary[usedCell].amount--;
            if (im.inventoryDictionary[usedCell].amount <= 0)
            {
                im.inventoryDictionary[usedCell].item = waterBucket;
                im.inventoryDictionary[usedCell].amount = 1;
            }
            else
            {
                im.ObtainItem(new InventorySlot { item = lavaBucket, amount = 1 });
            }
            usedCell.UpdateCell();
            im.UpdateSeletedSlot();

        }
        if (waterDetected&&!lavaDetected)
        {
            DoWater();
        }
        else if (lavaDetected && !waterDetected)
        {
            DoLava();
        }
        else if(lavaDetected && waterDetected)
        {
            if(Vector3.Distance(cam.transform.position, hit_water.point) > Vector3.Distance(cam.transform.position, hit_lava.point))
            {
                DoWater();
            }
            else
            {
                DoLava();
            }
        }
    }
}
