using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction_Tree : MonoBehaviour
{
    [HideInInspector]public UniversalScriptManager usm;
    ItemSpawner itemSpawner;
    public bool interacted;
    Rigidbody rb;
    public Vector3 cutOffOffset;
    GameObject player;
    public float pushForce;
    public float torqueForce;
    public GameObject hitParticle;
    public GameObject destroyParticle;
    public int hitTime;
    public int hp;
    public int maxHp;
    public int minHp;
    public Item droppingItem;
    public int minItemDropCount;
    public int maxItemDropCount;
    public float dropPosRandomness;
    public float spawningArea;
    public void SetVariables()
    {
        player = usm.player;
        itemSpawner = usm.itemSpawner;

        hp = Random.Range(maxHp, minHp + 1);
    }
    public void Interact()
    {
        hitTime++;

        if (hitTime >= hp)
        {
            DestroyObject();
            return;
        }
        else
        {

            GameObject p = Instantiate(hitParticle);
            TerrainModifier tm = FindObjectOfType<TerrainModifier>();
            p.transform.position = tm.touchingPosition;

        }



        if (!interacted)
        {
            transform.Translate(cutOffOffset);
        }
        interacted = true;

        rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddForce((transform.position - player.transform.position).normalized * pushForce, ForceMode.Impulse);
        rb.AddTorque((transform.position - player.transform.position).normalized * pushForce, ForceMode.Impulse);
    }

    public void DestroyObject()
    {
        GameObject p = Instantiate(destroyParticle);
        p.transform.position = transform.position;
        p.transform.rotation = transform.rotation;
        int itemDropCount = Random.Range(minItemDropCount, maxItemDropCount + 1);
        for (int i = 0; i < itemDropCount; i++)
        {
            itemSpawner.SpawnItem(new InventorySlot { item = droppingItem, amount = 1 }, transform.position + transform.up * ((float)i / itemDropCount) * spawningArea + new Vector3(Random.Range(-dropPosRandomness, dropPosRandomness), Random.Range(-dropPosRandomness, dropPosRandomness), Random.Range(-dropPosRandomness, dropPosRandomness)), transform.rotation);
        }


        Destroy(gameObject);
    }
}
