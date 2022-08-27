using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUseManager : MonoBehaviour
{
    public UniversalScriptManager usm;
    public Animator handAnim;
    LoadingManager lm;
    InventoryManager im;
    TerrainModifier tm;
    CameraManager cm;
    UIManager um;
    PauseManager pm;
    HungerManager hungerM;
    SoundManager sm;
    ChunkLoader cl;
    BlockPlacementManager bpm;
    public bool reloaded;
    GameObject player;
    public float placeCoolTime;
    public List<CustomItemUseArray> customItemUseArray_L = new List<CustomItemUseArray>();
    Dictionary<Item, MonoBehaviour> customItemUseDictionary_L = new Dictionary<Item, MonoBehaviour>();
    public List<CustomItemUseArray> customItemUseArray_R = new List<CustomItemUseArray>();
    Dictionary<Item, MonoBehaviour> customItemUseDictionary_R = new Dictionary<Item, MonoBehaviour>();

    void Awake()
    {
        lm = usm.loadingManager;
        im = usm.inventoryManager;
        tm = usm.terrainModifier;
        cm = usm.cameraManager;
        um = usm.uiManager;
        pm = usm.pauseManager;
        hungerM = usm.hungerManager;
        sm = usm.soundManager;
        cl = usm.chunkLoader;
        bpm = usm.blockPlacementManager;

        player = usm.player;


        foreach(CustomItemUseArray ci in customItemUseArray_L)
        {
            customItemUseDictionary_L.Add(ci.item, ci.behaviour);
        }
        foreach (CustomItemUseArray ci in customItemUseArray_R)
        {
            customItemUseDictionary_R.Add(ci.item, ci.behaviour);
        }
    }
    void Update()
    {




        if (lm.loading || im.showingInventoryUI || pm.paused)
        {
            return;
        }


        Item usingItem;
        if (im.currentlyUsingInventorySlot.amount < 1)
        {
            usingItem = im.hand;
        }
        else
        {
            usingItem = im.currentlyUsingInventorySlot.item;
        }

        bool leftInput = usingItem.continuousCustomUse ? Input.GetMouseButton(0) : (usingItem.canUseByKeepingMouse ? (Input.GetMouseButton(0) && reloaded) :Input.GetMouseButtonDown(0));
        bool rightInput = usingItem.continuousCustomUse ? Input.GetMouseButton(1) : (usingItem.canUseByKeepingMouse ? (Input.GetMouseButton(1) && reloaded) : Input.GetMouseButtonDown(1));
        if (leftInput)//use item
        {

            //custom
            if (customItemUseDictionary_L.ContainsKey(usingItem))
            {
                customItemUseDictionary_L[usingItem].Invoke("Use", 0);



                return;
            }



            if (!reloaded)
                return;





            //reload
            reloaded = false;
            Invoke(nameof(Reload), usingItem.coolTime);

            //animate
            handAnim.SetTrigger(usingItem.usingAnimationName);

            //sound
            sm.PlaySound("Swing", 1);

            //cam shake
            if (usingItem.hasCamShake)
            {
                if (usingItem.attainCamShakeByModifyingTerrain)
                {
                    tm.usedItem_CameraShake.Add(usingItem);
                }
                else
                {
                    cm.ShakeCamera(usingItem.camShakeTime, usingItem.camShakePower, usingItem.camShakeFade, usingItem.camShakeDelay);
                }
            }

            //modify terrain
            tm.Invoke(nameof(tm.DestroyInChunk), usingItem.modifyDelay);

        }
        else if (rightInput)
        {
            //custom
            if (customItemUseDictionary_R.ContainsKey(usingItem))
            {
                customItemUseDictionary_R[usingItem].Invoke("Use", 0);



                return;
            }

            if (im.currentlyUsingInventorySlot.amount < 1)
                return;


            if (!reloaded)
                return;

            //reload
            reloaded = false;
            Invoke(nameof(Reload), placeCoolTime);

            //animate
            handAnim.SetTrigger(usingItem.usingAnimationName);

            //sound
            sm.PlaySound("Swing", 1);

            //place
            if (usingItem.blockInstance != null)
            {
                //check if touching ground
                InventoryCell usedCell = im.inventoryCellList[im.curInventorySlot];
                StartCoroutine(PlaceDelay(usingItem, usingItem.placeDelay, usedCell));
            }
            else if (usingItem.hungerFillAmount >= 1 && hungerM.hunger < 20)
            {
                hungerM.hunger += usingItem.hungerFillAmount;

                hungerM.UpdateHungerUI();
                InventoryCell usedCell = im.inventoryCellList[im.curInventorySlot];

                im.inventoryDictionary[usedCell].amount--;
                usedCell.UpdateCell();
                im.UpdateSeletedSlot();

                sm.PlaySound("eat" + Random.Range(1, 4).ToString(), 1);
            }
        }
    }
    public IEnumerator PlaceDelay(Item usingItem, float delay, InventoryCell usedCell)
    {
        yield return new WaitForSeconds(delay);
        bool placable = false;
        if(tm.currentlyTouchingChunk != null)
        {
            placable = true;
        }
        else if(tm.currentlyTouchingObject != null)
        {
            if (tm.currentlyTouchingObject.placing_stackable)
            {
                placable = true;
            }
        }


        if (placable)
        {

            PlaceBlock(usingItem, usedCell);
        }
    }

    public void PlaceBlock(Item usingItem, InventoryCell usedCell)
    {
        bpm.PlaceBlock(usingItem, usedCell, true);



    }
    public void Reload()
    {
        reloaded = true;
    }
}

[System.Serializable]
public class CustomItemUseArray
{
    public Item item;
    public MonoBehaviour behaviour;
}
