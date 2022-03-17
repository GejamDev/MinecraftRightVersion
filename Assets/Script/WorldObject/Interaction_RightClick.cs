using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction_RightClick : MonoBehaviour
{
    UniversalScriptManager usm;
    TerrainModifier tm;
    InventoryManager im;
    public MonoBehaviour interactionBehaviour;
    bool selected;

    void Awake()
    {
        usm = FindObjectOfType<UniversalScriptManager>();
        tm = usm.terrainModifier;
        im = usm.inventoryManager;
    }

    void Update()
    {
        if (tm.currentlyTouchingObject == null)
        {
            selected = false;
        }
        else
        {
            selected = tm.currentlyTouchingObject.gameObject == gameObject;
        }

        if (selected && Input.GetMouseButtonDown(1) && !Input.GetKey(KeyCode.LeftShift) && !im.showingInventoryUI)
        {
            //interact
            interactionBehaviour.Invoke("Interact", 0);
        }
    }
}
