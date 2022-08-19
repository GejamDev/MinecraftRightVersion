using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    public UniversalScriptManager usm;
    InventoryManager im;
    GameObject player;
    public float grabbableDistance;
    public InventorySlot holdingSlot;
    public Transform itemBundle;

    void Awake()
    {
        usm = FindObjectOfType<UniversalScriptManager>();
        player = usm.player;
        im = usm.inventoryManager;

    }
    public void SetItem(InventorySlot slot)
    {
        holdingSlot = slot;

        for (int i = 0; i < itemBundle.childCount; i++)
        {
            itemBundle.GetChild(i).gameObject.SetActive(itemBundle.GetChild(i).name == holdingSlot.item.name);
        }
    }

    void Update()
    {
        if(Vector3.Distance(transform.position, player.transform.position) <= grabbableDistance)
        {
            if (im.HasRoomFor(holdingSlot.item))
            {
                im.ObtainItem(holdingSlot);
                Destroy(gameObject);
            }
        }
    }
}
