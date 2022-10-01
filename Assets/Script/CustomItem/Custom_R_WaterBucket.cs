using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Custom_R_WaterBucket : MonoBehaviour
{
    public UniversalScriptManager usm;
    public Item item;
    public Animator handAnim;
    WaterManager wm;
    InventoryManager im;
    DimensionTransportationManager dtm;
    Transform cam;
    public float maxDistance;
    public float pourRadius;
    public float delay;
    public LayerMask groundLayer;
    public Item bucket;
    public bool reloaded;
    public float reloadTime;

    private void Awake()
    {
        cam = Camera.main.transform;

        wm = usm.waterManager;
        im = usm.inventoryManager;
        dtm = usm.dimensionTransportationManager;

        reloaded = true;
    }
    public void Use()
    {
        if (!reloaded)
            return;
        if (dtm.currentDimesnion == Dimension.Nether)
            return;


        reloaded = false;
        InventoryCell usedCell = im.inventoryCellList[im.curInventorySlot];
        handAnim.SetTrigger(item.usingAnimationName);

        RaycastHit hit_ground;
        if (Physics.Raycast(cam.position, cam.forward, out hit_ground, maxDistance, groundLayer))
        {
            Vector3 pouringPos = hit_ground.point - (hit_ground.point - cam.position).normalized * 0.3f;// + Vector3.down;

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


            Invoke(nameof(Reload), reloadTime);
        }
        else
        {
            reloaded = true;
        }

    }

    public void Reload()
    {
        reloaded = true;
    }
}
