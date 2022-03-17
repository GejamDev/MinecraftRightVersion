using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction_Destruction : MonoBehaviour
{
    UniversalScriptManager usm;
    ItemSpawner itemSpawner;
    public bool onlyDisable;

    public GameObject hitParticle;
    public GameObject destroyParticle;
    int hitTime;
    int hp;
    public int maxHp;
    public int minHp;
    public Item droppingItem;
    public int minItemDropCount;
    public int maxItemDropCount;
    public float dropPosRandomness;
    public AnimationCurve smashAnimationCurve;
    public float smashAnimationTime;
    public Transform itemSpawnPos;
    void Awake()
    {

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

            if(hitParticle != null)
            {
                GameObject p = Instantiate(hitParticle);
                TerrainModifier tm = FindObjectOfType<TerrainModifier>();
                p.transform.position = tm.touchingPosition;

            }
        }
        StartCoroutine(SmashAnimation());


    }
    IEnumerator SmashAnimation()
    {
        Vector3 originScale = transform.localScale;
        float playedTime = 0;
        while (playedTime <= smashAnimationTime)
        {
            playedTime += Time.deltaTime;
            transform.localScale = originScale * (smashAnimationCurve.Evaluate(playedTime));

            yield return new WaitForSeconds(Time.deltaTime);
        }
        transform.localScale = originScale;
    }

    public void DestroyObject()
    {
        usm = FindObjectOfType<UniversalScriptManager>();
        itemSpawner = usm.itemSpawner;
        if (destroyParticle != null)
        {

            GameObject p = Instantiate(destroyParticle);
            p.transform.position = transform.position;
            p.transform.rotation = transform.rotation;
        }
        int itemDropCount = Random.Range(minItemDropCount, maxItemDropCount + 1);
        for (int i = 0; i < itemDropCount; i++)
        {
            itemSpawner.SpawnItem(new InventorySlot { item = droppingItem, amount = 1 }, (itemSpawnPos == null ? transform.position : itemSpawnPos.position) + new Vector3(Random.Range(-dropPosRandomness, dropPosRandomness), Random.Range(-dropPosRandomness, dropPosRandomness), Random.Range(-dropPosRandomness, dropPosRandomness)), Quaternion.Euler(new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360))));
        }

        if (onlyDisable)
        {
            gameObject.SetActive(false);
        }
        else
        {

            Destroy(gameObject);
            //haha
        }
    }
}
