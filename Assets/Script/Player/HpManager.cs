using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HpManager : MonoBehaviour
{
    public UniversalScriptManager usm;
    FirstPersonController fpc;
    GameObject player;
    CameraManager cm;
    HungerManager hungerM;
    SoundManager sm;
    public GameObject deathUI;
    [Header("HP")]
    public int hp;
    public GameObject heartPrefab;
    public Transform hpUI;
    public Sprite full, half, none;
    List<Image> heartImages = new List<Image>();
    public float camShakeTime;
    public float camShakePower;
    public bool camShakeFade;
    public bool died;
    [Header("Auto Regen")]
    public float regenTick;

    [Header("Fall Damage")]
    public bool isGrounded;
    public float lastGroundedHeight;
    public float minFallDamageHeight;
    public float fallDamageMultiplier;

    [Header("Starve")]
    public float starveTick;

    [Header("Lava")]
    public float lavaTick;
    public int lavaDamage;

    void Awake()
    {
        fpc = usm.firstPersonController;
        player = usm.player;
        cm = usm.cameraManager;
        hungerM = usm.hungerManager;
        sm = usm.soundManager;
        hp = 20;
        for (int i = 0; i < 10; i++)
        {
            GameObject h = Instantiate(heartPrefab, hpUI);
            h.transform.localPosition = new Vector2(-80 + i * 17, 0);
            heartImages.Add(h.GetComponent<Image>());
        }
        UpdateHpUI();
        StartCoroutine(AutoRegenHealth());
        StartCoroutine(Starve());
        CheckLavaDamageTick();
    }

    void UpdateHpUI()
    {
        for (int i = 0; i < 10; i++)
        {
            Image h = heartImages[i];
            int type = (hp - (int)i * 2);

            if(type >= 2)
            {
                h.sprite = full;
            }
            else if(type == 1)
            {
                h.sprite = half;
            }
            else
            {
                h.sprite = none;
            }
            
        }

    }

    void Update()
    {
        if (died)
            return;
        if (isGrounded)
        {
            lastGroundedHeight = player.transform.position.y;
        }
        else if(player.transform.position.y > lastGroundedHeight || fpc.inWater)
        {
            lastGroundedHeight = player.transform.position.y;
        }



        bool preGrounded = isGrounded;
        isGrounded = fpc.isGrounded;
        if(!preGrounded && isGrounded)
        {



            float fallHeight = lastGroundedHeight - player.transform.position.y;
            if (fallHeight > minFallDamageHeight)
            {
                if (!fpc.inWater)
                {
                    int damage = Mathf.CeilToInt((fallHeight - minFallDamageHeight) * fallDamageMultiplier);
                    TakeDamage(damage);
                }
            }
        }



        if (hp <= 0)
        {
            Die();
        }
    }
    public void TakeDamage(int damage)
    {
        hp -= damage;
        cm.ShakeCamera(camShakeTime, camShakePower * damage, camShakeFade, 0);

        sm.PlaySound("Hit" + Random.Range(1, 4).ToString(), 1);

        UpdateHpUI();
    }

    public IEnumerator AutoRegenHealth()
    {
        while (true)
        {
            yield return new WaitUntil(() => hungerM.hunger >= 19 && hp <= 19);
            hp++;
            UpdateHpUI();
            yield return new WaitForSeconds(regenTick);
        }
    }
    public IEnumerator Starve()
    {
        while (true)
        {
            yield return new WaitUntil(() => hungerM.hunger <= 0);
            yield return new WaitForSeconds(starveTick);
            TakeDamage(1);
        }
    }

    public void Die()
    {
        died = true;
        deathUI.SetActive(true);
    }

    public void CheckLavaDamageTick()
    {
        if (fpc.inLava)
        {
            TakeDamage(lavaDamage);
        }
        Invoke(nameof(CheckLavaDamageTick), lavaTick);
    }
}
