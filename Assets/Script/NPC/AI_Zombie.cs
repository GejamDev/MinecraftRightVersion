using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Zombie : MonoBehaviour
{
    [Header("Variables")]
    public UniversalScriptManager usm;
    InventoryManager im;
    CameraManager cm;
    HpManager hm;
    CapsuleCollider cc;
    LightingManager lm;
    WeatherManager wm;
    Rigidbody rb;
    GameObject player;
    public Animator an;


    Vector3 direction;

    [Header("Settings")]
    public float moveSpeed;
    public float movingSynchronizeSpeed;
    public float rotateSpeed;
    public float playerDetectDistance;
    public float minDistance;
    public float attakReloadTime;
    public float attakDelay;
    public float attakDuration;
    public int damage;
    public GameObject attakRange;
    public GameObject hitParticle;
    public GameObject deathParticle;
    public float minRainPowerToSurvive;

    [Header("State")]
    public int hp;
    public bool playerDetected;
    public bool walking;
    public bool knockBacked;
    public Vector3 knockBackDirection;
    public bool attakReloaded = true;
    public bool attaking;

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

        cc = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
        attakRange.GetComponent<IntContainor>().obj = damage;

    }

    public void Update()
    {
        if (!lm.isNight && wm.rainPower < minRainPowerToSurvive)
        {
            cc.isTrigger = true;
            if(transform.position.y < -10)
            {
                Destroy(gameObject);
            }
        }



        direction = (player.transform.position - transform.position).normalized;
        direction = (new Vector3(direction.x, 0, direction.z)).normalized;
        if (knockBacked)
        {
            rb.velocity = new Vector3(knockBackDirection.x, rb.velocity.y, knockBackDirection.z);
        }
        else if (attaking)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            Quaternion targetRot = Quaternion.Euler(new Vector3(0, angle, 0));


            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
        }
        else
        {

            float distance = Vector3.Distance(transform.position + Vector3.up, player.transform.position);


            //player detection
            if (distance <= playerDetectDistance)
            {
                playerDetected = true;
            }


            //walk
            bool walking = playerDetected && distance > minDistance;
            an.SetBool("walking", walking);
            if (walking)
            {
                //check direction
                Vector3 vel = direction * moveSpeed;

                rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(vel.x, rb.velocity.y, vel.z), movingSynchronizeSpeed * Time.deltaTime);//new Vector3(vel.x, rb.velocity.y, vel.z);
                float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
                Quaternion targetRot = Quaternion.Euler(new Vector3(0, angle, 0));


                transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
            }
            else if(playerDetected && distance <= minDistance && attakReloaded)
            {
                Attak();
            }
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
    public void Attak()
    {
        an.ResetTrigger("cancelAttak");
        attakReloaded = false;
        Invoke(nameof(Reload), attakReloadTime);
        Invoke(nameof(TryToDamage), attakDelay);

        attaking = true;
        an.SetTrigger("attak");
    }
    public void TryToDamage()
    {
        attakRange.SetActive(true);
        //hm.TakeDamage(damage);
        Invoke(nameof(DisableAttakRange), attakDuration);
    }
    public void DisableAttakRange()
    {
        attakRange.SetActive(false);
    }
    public void Reload()
    {
        attakReloaded = true ;
        attaking = false;
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


        if (attaking)
        {
            CancelAttak();
        }
    }
    public void CancelAttak()
    {
        attaking = false;
        an.SetTrigger("cancelAttak");
        CancelInvoke(nameof(TryToDamage));
        CancelInvoke(nameof(DisableAttakRange));
        attakRange.SetActive(false);

    }

    public void EndKnockBack()
    {

        knockBacked = false;
    }


    void OnCollisionEnter(Collision collision)
    {
        //change chunk this objt is in
        if(collision.gameObject.name == "Mesh")
        {
            if(collision.gameObject.GetComponent<ObjectContainer>() != null)
            {
                transform.SetParent(collision.gameObject.GetComponent<ObjectContainer>().obj.GetComponent<ChunkScript>().objectBundle.transform);
            }
        }
        else if(collision.gameObject == player)
        {
            hm.TakeDamage(damage);
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
