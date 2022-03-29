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
    public bool reloaded;
    GameObject player;
    public float placeCoolTime;

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

        player = usm.player;
    }
    void Update()
    {




        if (lm.loading || im.showingInventoryUI || pm.paused)
        {
            return;
        }


        if (Input.GetMouseButton(0))//use item
        {
            if (!reloaded)
                return;



            Item usingItem;
            if (im.currentlyUsingInventorySlot.amount < 1)
            {
                usingItem = im.hand;
            }
            else
            {
                usingItem = im.currentlyUsingInventorySlot.item;
            }


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
        else if (Input.GetMouseButton(1))
        {

            if (im.currentlyUsingInventorySlot.amount < 1)
                return;


            if (!reloaded)
                return;


            Item usingItem = im.currentlyUsingInventorySlot.item;

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
            else if(usingItem.hungerFillAmount >= 1 && hungerM.hunger< 20)
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
        //check if colliding with player
        Vector3 touchingPosition = tm.touchingPosition;
        Vector3 playerPosition = player.transform.position;

        



        GameObject block = Instantiate(usingItem.blockInstance);
        block.transform.position = tm.touchingPosition;
        if (tm.currentlyTouchingChunk != null)
        {
            block.transform.SetParent(tm.currentlyTouchingChunk.objectBundle.transform);
        }
        else
        {
            block.transform.SetParent(tm.currentlyTouchingObject.transform.parent);
        }

        if (usingItem.snapPosition)
        {
            float gridSize = usingItem.snapGridSize;
            if (tm.currentlyTouchingChunk != null)
            {
                block.transform.position = new Vector3(Mathf.RoundToInt(block.transform.position.x / gridSize) * gridSize,
                    Mathf.RoundToInt(block.transform.position.y / gridSize) * gridSize,
                    Mathf.RoundToInt(block.transform.position.z / gridSize) * gridSize);
            }
            else
            {
                block.transform.position = block.transform.position - (block.transform.position - Camera.main.transform.position).normalized * 0.3f;
                block.transform.position = new Vector3(Mathf.RoundToInt(block.transform.position.x / gridSize) * gridSize,
                    Mathf.RoundToInt(block.transform.position.y / gridSize) * gridSize,
                    Mathf.RoundToInt(block.transform.position.z / gridSize) * gridSize);
            }

            //check if collides with player
            Vector3 blockPos = block.transform.position;
            if (Mathf.Abs(playerPosition.x - blockPos.x) <= 0.5f + gridSize * 0.5f && Mathf.Abs(playerPosition.z - blockPos.z) <= 0.5f + gridSize * 0.5f && Mathf.Abs(playerPosition.y - blockPos.y) <= 0.5f + gridSize * 0.5f)
            {
                Destroy(block);
                return;
            }

        }
        if (usingItem.lookAtPlayer)
        {
            if (usingItem.snapPosition)
            {
                Vector2 blockPos = new Vector2(block.transform.position.x, block.transform.position.z);
                Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
                Vector2 dir = playerPos - blockPos;

                float rot = 0;
                if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                {
                    if (dir.x > 0)
                    {
                        rot = 0;
                    }
                    else
                    {
                        rot = 180;
                    }
                }
                else
                {
                    if (dir.y > 0)
                    {
                        rot = 270;
                    }
                    else
                    {
                        rot = 90;
                    }
                }


                block.transform.eulerAngles = new Vector3(0, rot + 90, 0);
            }
            else
            {
                Vector2 blockPos = new Vector2(block.transform.position.x, block.transform.position.z);
                Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
                Vector2 offset = playerPos - blockPos;
                float rot = Mathf.Atan2(offset.x, offset.y) * Mathf.Rad2Deg;


                block.transform.eulerAngles = new Vector3(0, rot, 0);
            }
        }


        if (usingItem.placeSound != "")
        {
            sm.PlaySound(usingItem.placeSound, 1);
        }

        im.inventoryDictionary[usedCell].amount--;
        usedCell.UpdateCell();
        im.UpdateSeletedSlot();

    }
    public void Reload()
    {
        reloaded = true;
    }
}
