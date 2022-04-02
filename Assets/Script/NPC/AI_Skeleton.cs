using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Skeleton : MonoBehaviour
{
    [Header("Variables")]
    public UniversalScriptManager usm;
    InventoryManager im;
    CameraManager cm;
    HpManager hm;
    CapsuleCollider cc;
    LightingManager lm;
    WeatherManager wm;
    ItemSpawner itemSpawner;
    Rigidbody rb;
    GameObject player;
    public Animator an;
    public LineRenderer bowString;
    public Transform bowMidTransform;


    Vector3 direction;

    [Header("Settings")]
    public float moveSpeed;
    public float movingSynchronizeSpeed;
    public float rotateSpeed;
    public float playerDetectDistance;
    public float minDistance;
    public GameObject hitParticle;
    public GameObject deathParticle;
    public float minRainPowerToSurvive;
    public float attakChargeTime;
    public float reloadtime;
    public float attackAnimationEscapeDelay;
    public GameObject arrowPrefab;
    public float arrowShootPower;
    public Transform arrowShootPos;
    public Item droppingItem;

    [Header("State")]
    public int hp;
    public bool playerDetected;
    public bool walking;
    public bool knockBacked;
    public Vector3 knockBackDirection;
    public bool attacking;
    public bool reloaded;

    [Header("Camera Shake")]
    public float hitCamShakeTime;
    public float hitCamShakePower;

    void Awake()
    {
        
        //get variables
        usm = FindObjectOfType<UniversalScriptManager>();
        player = usm.player;
        im = usm.inventoryManager;
        cm = usm.cameraManager;
        hm = usm.hpManager;
        lm = usm.lightingManager;
        wm = usm.weatherManager;
        itemSpawner = usm.itemSpawner;

        cc = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();


        reloaded = true;

    }
    public void Update()
    {
        //if (!lm.isNight && wm.rainPower < minRainPowerToSurvive)
        //{
        //    cc.isTrigger = true;
        //    if (transform.position.y < -10)
        //    {
        //        Destroy(gameObject);
        //    }
        //}

        bowString.SetPosition(1, bowMidTransform.localPosition);



        direction = (player.transform.position - transform.position).normalized;
        direction = (new Vector3(direction.x, 0, direction.z)).normalized;


        float distance = Vector3.Distance(transform.position + Vector3.up, player.transform.position);


        //player detection
        if (distance <= playerDetectDistance)
        {
            playerDetected = true;
        }


        //walk
        bool walking = playerDetected && distance >= minDistance;
        an.SetBool("walking", walking);
        if (knockBacked)
        {
            rb.velocity = new Vector3(knockBackDirection.x, rb.velocity.y, knockBackDirection.z);
        }
        if (walking)
        {
            //check direction
            Vector3 vel = direction * moveSpeed;

            rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(vel.x, rb.velocity.y, vel.z), movingSynchronizeSpeed * Time.deltaTime);//new Vector3(vel.x, rb.velocity.y, vel.z);
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            Quaternion targetRot = Quaternion.Euler(new Vector3(0, angle, 0));


            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
        }
        else if (playerDetected)
        {
            if(!attacking && reloaded)
            {
                StartAttack();
            }

            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            Quaternion targetRot = Quaternion.Euler(new Vector3(0, angle, 0));


            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
        }
    }
    public void StartAttack()
    {
        attacking = true;
        reloaded = false;
        an.SetTrigger("attack");
        Invoke(nameof(Shoot), attakChargeTime);
        Invoke(nameof(Reload), reloadtime);
    }
    public void Shoot()
    {
        attacking = false;
        Invoke(nameof(EscapeAttackingAnimation), attackAnimationEscapeDelay);


        //shoot
        GameObject arrow = Instantiate(arrowPrefab);
        arrow.transform.position = arrowShootPos.position;
        arrow.transform.rotation = Quaternion.LookRotation(player.transform.position - arrow.transform.position, Vector3.up);

        Rigidbody rb = arrow.GetComponent<Rigidbody>();

        rb.AddForce((player.transform.position-arrow.transform.position).normalized * arrowShootPower, ForceMode.Impulse);
    }
    public void CancelAttak()
    {
        EscapeAttackingAnimation();
        CancelInvoke(nameof(Shoot));
    }
    public void Reload()
    {
        reloaded = true;
    }
    public void EscapeAttackingAnimation()
    {
        an.SetTrigger("cancel");
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


        cm.ShakeCamera(hitCamShakeTime, hitCamShakePower * damage, true, 0);

        if (hp <= 0)
        {
            if (deathParticle != null)
            {
                GameObject p = Instantiate(deathParticle);
                p.transform.position = transform.position + Vector3.up;
            }

            itemSpawner.SpawnItem(new InventorySlot { item = droppingItem, amount = Random.Range(0, 2) }, transform.position + Vector3.up, Quaternion.identity);
            Destroy(gameObject);
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

        CancelAttak();
    }
    public void EndKnockBack()
    {

        knockBacked = false;
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

    public void GetHit_Arrow()
    {
        Item usingItem = usm.arrow;
        int damage = usingItem.damage;
        hp -= damage;


        cm.ShakeCamera(hitCamShakeTime, hitCamShakePower * damage, true, 0);

        if (hp <= 0)
        {
            if (deathParticle != null)
            {
                GameObject p = Instantiate(deathParticle);
                p.transform.position = transform.position + Vector3.up;
            }

            Destroy(gameObject);
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
}
