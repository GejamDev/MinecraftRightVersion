using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectUIInteraction : MonoBehaviour
{
    UniversalScriptManager usm;
    TerrainModifier tm;
    InventoryManager im;
    UIManager um;
    bool selected;

    public InventoryMode mode;

    void Awake()
    {
        usm = FindObjectOfType<UniversalScriptManager>();
        tm = usm.terrainModifier;
        im = usm.inventoryManager;
        um = usm.uiManager;
    }

    void Update()
    {
        if(tm.currentlyTouchingObject == null)
        {
            selected = false;
        }
        else
        {
            selected = tm.currentlyTouchingObject.gameObject == gameObject;
        }

        if (selected && Input.GetMouseButtonDown(1) && !Input.GetKey(KeyCode.LeftShift) && !im.showingInventoryUI)
        {
            //open ui
            um.OpenUI(mode);
        }
    }

}
