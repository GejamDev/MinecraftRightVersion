using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public UniversalScriptManager usm;
    InventoryManager im;

    void Awake()
    {
        im = usm.inventoryManager;
    }

    public void OpenUI(InventoryMode mode)
    {
        im.OpenInvUI(mode);
    }
}
