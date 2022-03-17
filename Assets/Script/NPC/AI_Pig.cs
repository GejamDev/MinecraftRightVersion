using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Pig : MonoBehaviour
{
    [Header("Variables")]
    public UniversalScriptManager usm;
    InventoryManager im;
    ItemSpawner itemSpawner;
    CameraManager cm;
    HpManager hm;
    Rigidbody rb;
    GameObject player;
    public Item droppingItem;
    public int minItemDropCount;
    public int maxItemDropCount;
    public GameObject hitParticle;
    public GameObject deathParticle;

    [Header("State")]
    public int hp;
    public bool knockBacked;
    public Vector3 knockBackDirection;

    [Header("Camera Shake")]
    public float camShakeTime;
    public float camShakePower;
    public bool camShakeFade;

    void Awake()
    {
        //get variables
        usm = FindObjectOfType<UniversalScriptManager>();
        player = usm.player;
        im = usm.inventoryManager;
        cm = usm.cameraManager;
        hm = usm.hpManager;
        itemSpawner = usm.itemSpawner;

        rb = GetComponent<Rigidbody>();

    }

    public void Update()
    {
        if (knockBacked)
        {
            rb.velocity = new Vector3(knockBackDirection.x, rb.velocity.y, knockBackDirection.z);
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    public void Interact()
    {
        GetHit();
    }
    public void GetHit()
    {
        Item usingItem;
        if (im.currentlyUsingInventorySlot.amount < 1)
        {
            usingItem = im.hand;
        }
        else
        {
            usingItem = im.currentlyUsingInventorySlot.item;
        }
        int damage = usingItem.damage;
        hp -= damage;


        cm.ShakeCamera(camShakeTime, camShakePower * damage, camShakeFade, 0);

        if (hp <= 0)
        {
            Die();
        }
        else
        {
            //knock back
            KnockBack(usingItem);
        }
        if (hitParticle != null)
        {
            GameObject p = Instantiate(hitParticle);
            p.transform.position = transform.position + Vector3.up;
        }
    }

    public void KnockBack(Item item)
    {
        knockBacked = true;
        CancelInvoke(nameof(EndKnockBack));
        Invoke(nameof(EndKnockBack), item.knockBackTime);
        Vector3 dir = transform.position - player.transform.position;
        dir = new Vector3(dir.x, 0, dir.z);
        dir = dir.normalized * item.knockBackPower;
        knockBackDirection = dir;
    }
    public void EndKnockBack()
    {

        knockBacked = false;
    }

    public void Die()
    {



        itemSpawner.SpawnItem(new InventorySlot { item = droppingItem, amount = Random.Range(minItemDropCount, maxItemDropCount) }, transform.position + Vector3.up, Quaternion.identity);

        if(deathParticle != null)
        {
            GameObject p = Instantiate(deathParticle);
            p.transform.position = transform.position + Vector3.up;
        }

        Destroy(gameObject);
    }


    void OnCollisionEnter(Collision collision)
    {

        //change chunk this objt is in
        if (collision.gameObject.name == "Mesh")
        {
            if (collision.gameObject.GetComponent<ObjectContainer>() != null)
            {
                transform.SetParent(collision.gameObject.GetComponent<ObjectContainer>().obj.GetComponent<ChunkScript>().objectBundle.transform);
            }
        }
    }
}
