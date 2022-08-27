using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPlacementManager : MonoBehaviour
{
    public UniversalScriptManager usm;
    GameObject player;
    TerrainModifier tm;
    ChunkLoader cl;
    SoundManager sm;
    InventoryManager im;

    private void Awake()
    {
        player = usm.player;
        tm = usm.terrainModifier;
        cl = usm.chunkLoader;
        sm = usm.soundManager;
        im = usm.inventoryManager;
    }
    public void PlaceBlock(Item usingItem, InventoryCell usedCell, bool sound)
    {

        PlaceBlock(usingItem, usedCell, tm.touchingPosition, sound);
    }

    public void PlaceBlock(Item usingItem, InventoryCell usedCell, Vector3 pos, bool sound)
    {

        Vector3 playerPosition = player.transform.position;
        GameObject block = Instantiate(usingItem.blockInstance);
        block.transform.position = pos;//tm.touchingPosition;
        Vector3 blockPos = block.transform.position;
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
            blockPos = block.transform.position;
            if (Mathf.Abs(playerPosition.x - blockPos.x) <= 0.5f + gridSize * 0.5f && Mathf.Abs(playerPosition.z - blockPos.z) <= 0.5f + gridSize * 0.5f && Mathf.Abs(playerPosition.y - blockPos.y) <= 0.5f + gridSize * 0.5f)
            {
                Destroy(block);
                return;
            }

            ChunkScript parentCs = cl.chunkDictionary[new Vector2(Mathf.Floor(blockPos.x / 8) * 8, Mathf.Floor(blockPos.z / 8) * 8)].cs;
            if (parentCs.blockPositionData.Contains(new Vector3Int((int)blockPos.x, (int)blockPos.y, (int)blockPos.z)))
            {
                Destroy(block);
                return;
            }
            parentCs.blockPositionData.Add(new Vector3Int((int)blockPos.x, (int)blockPos.y, (int)blockPos.z) - new Vector3Int((int)parentCs.position.x, 0, (int)parentCs.position.y));
            block.transform.SetParent(parentCs.objectBundle.transform);


        }
        else
        {
            ChunkScript parentCs = cl.chunkDictionary[new Vector2(Mathf.Floor(blockPos.x / 8) * 8, Mathf.Floor(blockPos.z / 8) * 8)].cs;
            block.transform.SetParent(parentCs.objectBundle.transform);
        }
        if (usingItem.lookAtPlayer)
        {
            if (usingItem.snapPosition)
            {
                Vector2 blockPos_2D = new Vector2(block.transform.position.x, block.transform.position.z);
                Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
                Vector2 dir = playerPos - blockPos_2D;

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
                Vector2 blockPos_2D = new Vector2(block.transform.position.x, block.transform.position.z);
                Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);
                Vector2 offset = playerPos - blockPos_2D;
                float rot = Mathf.Atan2(offset.x, offset.y) * Mathf.Rad2Deg;


                block.transform.eulerAngles = new Vector3(0, rot, 0);
            }
        }


        if (usingItem.placeSound != "" && sound)
        {
            sm.PlaySound(usingItem.placeSound, 1);
        }
        if(usedCell != null)
        {
            im.inventoryDictionary[usedCell].amount--;
            usedCell.UpdateCell();
            im.UpdateSeletedSlot();
        }
    }
}
