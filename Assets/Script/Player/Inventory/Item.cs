using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class Item : ScriptableObject
{
    public Sprite image;

    [Header("variables")]
    [Space(10)]

    [Header("Default Variables")]
    public string usingAnimationName;
    public float coolTime;

    [Header("Block Variables")]
    public GameObject blockInstance;
    public float placeDelay;
    public bool snapPosition;
    public float snapGridSize;
    public bool lookAtPlayer;
    public string placeSound;

    [Header("tool variables")]
    public bool isToolForModifyingTerrain;
    public int maxDiggableYLevel;
    public int minDiggableYLevel;
    public float modifyDelay;
    public bool hasCamShake;
    public bool attainCamShakeByModifyingTerrain;
    public float camShakeDelay;
    public float camShakeTime;
    public float camShakePower;
    public bool camShakeFade;

    [Header("Object Modifying Variables")]
    public ModifiableObjectType modifableObjectType;

    [Header("Furnace Variables")]
    public Item furnaceResult;
    public bool fuel_usable;

    [Header("Weapon Variables")]
    public int damage = 1;
    public float knockBackTime = 0.2f;
    public float knockBackPower = 1;

    [Header("Food")]
    public int hungerFillAmount;

    
}


