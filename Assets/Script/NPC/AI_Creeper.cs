using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Creeper : MonoBehaviour
{
    [Header("Variables")]
    public UniversalScriptManager usm;
    InventoryManager im;
    CameraManager cm;
    HpManager hm;
    CapsuleCollider cc;
    LightingManager lm;
    ExplosionManager em;
    SoundManager sm;



    Rigidbody rb;
    GameObject player;
    public Animator an;
    public SkinnedMeshRenderer smr;



    Vector3 direction;

    [Header("Settings")]
    public float moveSpeed;
    public float movingSynchronizeSpeed;
    public float rotateSpeed;
    public float playerDetectDistance;
    public float minDistance;
    public float explodeCancelDistance;
    public GameObject hitParticle;
    public GameObject deathParticle;
    public ExplosionPreset explosionPreset;
    public float originScale;
    public float explodeShakePower;
    public float shakeDampen;
    public Color chargedColor;
    public Color noEmission;
    public float chargeSpeed;
    public float calmSpeed;
    [Header("State")]
    public int hp;
    public bool playerDetected;
    public bool walking;
    public bool knockBacked;
    public Vector3 knockBackDirection;
    public bool exploding;
    public float explodingReadiedTime;
    public float explodeRequireTime;

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
        em = usm.explosionManager;
        sm = usm.soundManager;

        cc = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();

        originScale = transform.localScale.x;

    }

    public void Update()
    {



        direction = (player.transform.position - transform.position).normalized;
        direction = (new Vector3(direction.x, 0, direction.z)).normalized;
        float distance = Vector3.Distance(transform.position + Vector3.up, player.transform.position);
        if (knockBacked)
        {
            rb.velocity = new Vector3(knockBackDirection.x, rb.velocity.y, knockBackDirection.z);
        }
        else if(!exploding)
        {



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
            else if(playerDetected)//gonna explode
            {
                StartExploding();
            }
        }

        if (exploding)
        {

            if (!knockBacked)
            {
                rb.velocity = new Vector3(0, rb.velocity.y, 0);
            }


            //shake
            Vector3 targetScale = new Vector3(originScale + Random.Range(-explodeShakePower, explodeShakePower), originScale + Random.Range(-explodeShakePower, explodeShakePower), originScale + Random.Range(-explodeShakePower, explodeShakePower));
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, shakeDampen * Time.deltaTime);

            //color

            smr.materials[0].SetColor("_EmissionColor", Color.Lerp(smr.materials[0].GetColor("_EmissionColor"), chargedColor, chargeSpeed * Time.deltaTime));
            smr.materials[1].SetColor("_EmissionColor", Color.Lerp(smr.materials[1].GetColor("_EmissionColor"), chargedColor, chargeSpeed * Time.deltaTime));
            
            explodingReadiedTime += Time.deltaTime;

            if (distance >= explodeCancelDistance)
            {
                CancelExploding();
            }
            else if(explodingReadiedTime>= explodeRequireTime)
            {
                Explode();
            }
        }
        else
        {
            transform.localScale = Vector3.one * originScale;

            smr.materials[0].SetColor("_EmissionColor", Color.Lerp(smr.materials[0].GetColor("_EmissionColor"), noEmission, calmSpeed * Time.deltaTime));
            smr.materials[1].SetColor("_EmissionColor", Color.Lerp(smr.materials[1].GetColor("_EmissionColor"), noEmission, calmSpeed * Time.deltaTime));



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

    public void StartExploding()
    {
        sm.PlaySound("creeperHiss", 1);
        an.SetBool("walking", false);
        exploding = true;
    }
    public void CancelExploding()
    {
        explodingReadiedTime = 0;
           exploding = false;
    }
    public void Explode()
    {
        em.CauseExplosion(transform.position, explosionPreset);


        Destroy(gameObject);
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
