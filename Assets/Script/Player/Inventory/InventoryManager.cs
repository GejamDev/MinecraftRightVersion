using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public enum InventoryMode
{
    Normal,
    Crafting,
    Furnace
}


public class InventoryManager : MonoBehaviour
{
    public UniversalScriptManager usm;
    public List<InventorySlot> savedInventorySlotList = new List<InventorySlot>();
    UIManager um;
    LoadingManager lm;
    PauseManager pm;
    ItemSpawner itemSpawner;
    SoundManager sm;

    GameObject player;

    public Transform itemThrowTransform;
    public float itemThrowForce;
    public Vector3 additionalThrowForce;

    public bool showingInventoryUI;
    public GameObject inventoryUI;
    public List<InventoryCell> inventoryCellList = new List<InventoryCell>();
    public Dictionary<InventoryCell, InventorySlot> inventoryDictionary = new Dictionary<InventoryCell, InventorySlot>();
    public Transform parent_main;
    public Transform parent_inside;
    public Transform parent_main_outside;
    public Transform parent_craftingInput;
    public Transform parent_craftingOutput;
    public Transform parent_craftingInput_Table;
    public Transform parent_craftingOutput_Table;
    public Transform parent_furnaceInput;
    public Transform parent_furnaceFuel;
    public Transform parent_furnaceOutput;
    public GameObject ui_normal;
    public GameObject ui_crafting;
    public GameObject ui_furnace;
    public GameObject inventoryCellPrefab;
    public int curInventorySlot;
    public bool holding;
    public InventorySlot holdingInventorySlot;
    public GameObject inventoryCursor;
    public Image inventoryCursor_Image;
    public Text inventoryCursor_Text;
    public GameObject selectedSlotOutline;
    public Transform itemBundle;
    public InventorySlot currentlyUsingInventorySlot;
    public List<InventoryCell> craftingCells = new List<InventoryCell>();
    public InventoryCell craftingOutputCell;
    public List<InventoryCell> craftingCells_Table = new List<InventoryCell>();
    public InventoryCell craftingOutputCell_Table;

    public FurnaceScript currentlyTouchingFurnace;
    public InventoryCell furnaceInputCell;
    public InventoryCell furnaceFuelCell;
    public InventoryCell furnaceOutputCell;

    public CraftingRecipe[] craftingRecipes;
    public Item[] itemList;
    
    public Image furnaceProgress;

    public InventoryMode curMode;

    public Item hand;

    void Awake()
    {
        um = usm.uiManager;
        lm = usm.loadingManager;
        pm = usm.pauseManager;
        sm = usm.soundManager;
        itemSpawner = usm.itemSpawner;

        player = usm.player;
    }
    public void SetInventory()
    {
        for (int i = 1; i <= 54; i++)
        {
            InventoryCell ic = Instantiate(inventoryCellPrefab).GetComponent<InventoryCell>();
            ic.im = this;
            ic.insideInventory = true;


            bool isMain = i <= 9;
            bool isInside = 9 < i && i <= 36;
            bool isCraftingInput = 36 < i && i <= 40;
            bool isCraftingOuput = i == 41;
            bool isCraftingInput_Table = 41 < i && i <= 50;
            bool isCraftingOutput_Table = i == 51;
            bool isFurnaceInput = i == 52;
            bool isFurnaceFuel = i == 53;
            bool isFurnaceOutput = i == 54;

            if (isMain)
            {
                ic.gameObject.name = "Main_" + i.ToString();
                ic.transform.SetParent(parent_main);
                ic.transform.localPosition = new Vector2((i - 5) * 40, 0);


                InventoryCell ic_syn = Instantiate(inventoryCellPrefab).GetComponent<InventoryCell>();
                ic_syn.im = this;
                ic_syn.gameObject.name = ("Main_Outside_" + (i).ToString());
                ic_syn.transform.SetParent(parent_main_outside);
                ic_syn.transform.localPosition = new Vector2((i - 5) * 40, 0);
                ic_syn.insideInventory = false;
                ic_syn.UpdateCell();

                ic.synchronizedCell = ic_syn;
            }
            else if (isInside)
            {
                ic.gameObject.name = "Inside" + (i - 9).ToString();
                ic.transform.SetParent(parent_inside);
                ic.transform.localPosition = new Vector2((((i - 1) % 9) - 4) * 40, i <= 18 ? 40 : (i <= 27 ? 0 : -40));

            }
            else if (isCraftingInput)
            {
                ic.gameObject.name = "CraftingInput_" + (i - 36).ToString();
                ic.transform.SetParent(parent_craftingInput);
                ic.transform.localPosition = new Vector2(((i - 1) % 2) * 40 - 20, i <= 38 ? 20 : -20);
                craftingCells.Add(ic);
            }
            else if (isCraftingOuput)
            {

                ic.gameObject.name = "CraftingOutput";
                ic.transform.SetParent(parent_craftingOutput);
                ic.transform.localPosition = Vector2.zero;
                ic.isCraftingOutput = true;
                craftingOutputCell = ic;

            }
            else if (isCraftingInput_Table)
            {
                int index = i - 42;
                ic.gameObject.name = "CraftingInput_" + (index + 1).ToString();
                ic.transform.SetParent(parent_craftingInput_Table);
                ic.transform.localPosition = new Vector2(((index % 3) - 1) * 40, (index <= 2 ? 40 : (index <= 5 ? 0 : -40)));
                craftingCells_Table.Add(ic);
            }
            else if (isCraftingOutput_Table)
            {

                ic.gameObject.name = "CraftingOutput_Table";
                ic.transform.SetParent(parent_craftingOutput_Table);
                ic.transform.localPosition = Vector2.zero;
                ic.isCraftingOutput = true;
                craftingOutputCell_Table = ic;

            }
            else if (isFurnaceInput)
            {

                ic.gameObject.name = "FurnaceInput";
                ic.transform.SetParent(parent_furnaceInput);
                ic.transform.localPosition = Vector2.zero;
                furnaceInputCell = ic;

            }
            else if (isFurnaceFuel)
            {

                ic.gameObject.name = "FurnaceFuel";
                ic.transform.SetParent(parent_furnaceFuel);
                ic.transform.localPosition = Vector2.zero;
                furnaceFuelCell = ic;

            }
            else if (isFurnaceOutput)
            {
                ic.gameObject.name = "FurnaceOutput";
                ic.transform.SetParent(parent_furnaceOutput);
                ic.transform.localPosition = Vector2.zero;
                ic.isCraftingOutput = true;
                furnaceOutputCell = ic;
            }
            else
            {
                throw new System.Exception("inventory cell out of range");
            }





            inventoryCellList.Add(ic);
            Debug.Log(savedInventorySlotList.Count);
            if ((isMain || isInside) && savedInventorySlotList.Count!=0)
            {
                //inventoryDictionary.Add(ic, new InventorySlot() { amount = 0, item = null });
                inventoryDictionary.Add(ic, new InventorySlot() { amount = savedInventorySlotList[i - 1].amount, item = savedInventorySlotList[i - 1].item });
            }
            else
            {
                inventoryDictionary.Add(ic, new InventorySlot() { amount = 0, item = null });
            }
            ic.UpdateCell();
        }
        inventoryUI.SetActive(showingInventoryUI);
        UpdateSeletedSlot();
    }

    public void OpenInvUI(InventoryMode mode)
    {

        showingInventoryUI = true;
        inventoryUI.SetActive(true);

        //display inv ui
        foreach (InventoryCell ic in inventoryCellList)
        {
            ic.UpdateCell();
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;


        ui_normal.SetActive(mode == InventoryMode.Normal);
        ui_crafting.SetActive(mode == InventoryMode.Crafting);
        ui_furnace.SetActive(mode == InventoryMode.Furnace);

        curMode = mode;

        if(curMode == InventoryMode.Furnace)
        {
            inventoryDictionary[furnaceInputCell] = currentlyTouchingFurnace.inputSlot;
            inventoryDictionary[furnaceFuelCell] = currentlyTouchingFurnace.fuelSlot;
            inventoryDictionary[furnaceOutputCell] = currentlyTouchingFurnace.outputSlot;
        }
    }

    public void LateUpdate()
    {

        if (lm.loading || pm.paused)
        {
            return;
        }

        //throw
        if(Input.GetKeyDown(KeyCode.Q))
        {
            if(inventoryDictionary[inventoryCellList[curInventorySlot]].amount >= 1)
            {
                Item item = inventoryDictionary[inventoryCellList[curInventorySlot]].item;

                inventoryDictionary[inventoryCellList[curInventorySlot]].amount--;
                inventoryCellList[curInventorySlot].UpdateCell();
                UpdateSeletedSlot();
                GameObject g = itemSpawner.GetSpawnItem(new InventorySlot { item = item, amount = 1 }, itemThrowTransform.transform.position, Quaternion.identity);

                g.GetComponent<Rigidbody>().AddForce(itemThrowTransform.transform.forward * itemThrowForce + additionalThrowForce, ForceMode.Impulse);
            }



            return;
        }


        //show/hide ui
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (showingInventoryUI)
            {
                showingInventoryUI = false;
                inventoryUI.SetActive(false);

                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                OpenInvUI(InventoryMode.Normal);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (showingInventoryUI)
            {
                showingInventoryUI = false;
                inventoryUI.SetActive(false);

                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        //manage cursor
        if (showingInventoryUI && holdingInventorySlot.amount != 0)
        {
            inventoryCursor.SetActive(true);
            inventoryCursor.transform.position = Input.mousePosition;
            inventoryCursor_Image.sprite = holdingInventorySlot.item.image;
            inventoryCursor_Text.text = holdingInventorySlot.amount == 1 ? null : holdingInventorySlot.amount.ToString();
        }
        else
        {
            inventoryCursor.SetActive(false);
        }

        //manage current slot index
        int preIndex = curInventorySlot;


        int scrollWheelInput = (int)Input.GetAxisRaw("Mouse ScrollWheel");
        if (scrollWheelInput == -1)
        {
            if (curInventorySlot < 8)
            {
                curInventorySlot++;
            }
            else
            {
                curInventorySlot = 0;
            }

        }
        else if (scrollWheelInput == 1)
        {
            if (curInventorySlot > 0)
            {
                curInventorySlot--;
            }
            else
            {
                curInventorySlot = 8;
            }
        }


        if (Input.GetKeyDown(KeyCode.Alpha1))
            curInventorySlot = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2))
            curInventorySlot = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3))
            curInventorySlot = 2;
        if (Input.GetKeyDown(KeyCode.Alpha4))
            curInventorySlot = 3;
        if (Input.GetKeyDown(KeyCode.Alpha5))
            curInventorySlot = 4;
        if (Input.GetKeyDown(KeyCode.Alpha6))
            curInventorySlot = 5;
        if (Input.GetKeyDown(KeyCode.Alpha7))
            curInventorySlot = 6;
        if (Input.GetKeyDown(KeyCode.Alpha8))
            curInventorySlot = 7;
        if (Input.GetKeyDown(KeyCode.Alpha9))
            curInventorySlot = 8;

        if (preIndex != curInventorySlot)
        {
            UpdateSeletedSlot();
        }

        UpdateFuelData();


    }

    public void UpdateFuelData()
    {
        if (currentlyTouchingFurnace == null || curMode != InventoryMode.Furnace || !showingInventoryUI)
            return;



        furnaceProgress.fillAmount = currentlyTouchingFurnace.curFuelProgress;


        furnaceInputCell.UpdateCell();
        furnaceFuelCell.UpdateCell();
        furnaceOutputCell.UpdateCell();




    }

    public void UpdateSeletedSlot()
    {
        currentlyUsingInventorySlot = inventoryDictionary[inventoryCellList[curInventorySlot]];
        selectedSlotOutline.transform.localPosition = new Vector2((curInventorySlot - 4) * 40, 0);
        bool hasItem = currentlyUsingInventorySlot.amount >= 1;
        if (hasItem)
        {
            for (int i = 0; i < itemBundle.childCount; i++)
            {
                itemBundle.GetChild(i).gameObject.SetActive(itemBundle.GetChild(i).name == currentlyUsingInventorySlot.item.name);
            }
        }
        else
        {
            for (int i = 0; i < itemBundle.childCount; i++)
            {
                itemBundle.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void CheckCraftingInput()
    {
        if(curMode == InventoryMode.Normal)
        {
            Item[,] data = new Item[3, 3];
            data[0, 0] = inventoryDictionary[craftingCells[0]].amount == 0 ? null : inventoryDictionary[craftingCells[0]].item;
            data[1, 0] = inventoryDictionary[craftingCells[1]].amount == 0 ? null : inventoryDictionary[craftingCells[1]].item;
            data[0, 1] = inventoryDictionary[craftingCells[2]].amount == 0 ? null : inventoryDictionary[craftingCells[2]].item;
            data[1, 1] = inventoryDictionary[craftingCells[3]].amount == 0 ? null : inventoryDictionary[craftingCells[3]].item;
            foreach (CraftingRecipe cr in craftingRecipes)
            {
                if (CraftingManager.TryToCraft(data, cr))
                {

                    inventoryDictionary[craftingOutputCell] = new InventorySlot { item = cr.outputItem, amount = cr.outPutCount };
                    craftingOutputCell.UpdateCell();


                    return;
                }
            }
            inventoryDictionary[craftingOutputCell] = new InventorySlot { item = null, amount = 0 };
            craftingOutputCell.UpdateCell();
        }
        else if (curMode == InventoryMode.Crafting)
        {
            Item[,] data = new Item[3, 3];
            data[0, 0] = inventoryDictionary[craftingCells_Table[0]].amount == 0 ? null : inventoryDictionary[craftingCells_Table[0]].item;
            data[1, 0] = inventoryDictionary[craftingCells_Table[1]].amount == 0 ? null : inventoryDictionary[craftingCells_Table[1]].item;
            data[2, 0] = inventoryDictionary[craftingCells_Table[2]].amount == 0 ? null : inventoryDictionary[craftingCells_Table[2]].item;
            data[0, 1] = inventoryDictionary[craftingCells_Table[3]].amount == 0 ? null : inventoryDictionary[craftingCells_Table[3]].item;
            data[1, 1] = inventoryDictionary[craftingCells_Table[4]].amount == 0 ? null : inventoryDictionary[craftingCells_Table[4]].item;
            data[2, 1] = inventoryDictionary[craftingCells_Table[5]].amount == 0 ? null : inventoryDictionary[craftingCells_Table[5]].item;
            data[0, 2] = inventoryDictionary[craftingCells_Table[6]].amount == 0 ? null : inventoryDictionary[craftingCells_Table[6]].item;
            data[1, 2] = inventoryDictionary[craftingCells_Table[7]].amount == 0 ? null : inventoryDictionary[craftingCells_Table[7]].item;
            data[2, 2] = inventoryDictionary[craftingCells_Table[8]].amount == 0 ? null : inventoryDictionary[craftingCells_Table[8]].item;
            foreach (CraftingRecipe cr in craftingRecipes)
            {
                if (CraftingManager.TryToCraft(data, cr))
                {

                    inventoryDictionary[craftingOutputCell_Table] = new InventorySlot { item = cr.outputItem, amount = cr.outPutCount };
                    craftingOutputCell_Table.UpdateCell();


                    return;
                }
            }
            inventoryDictionary[craftingOutputCell_Table] = new InventorySlot { item = null, amount = 0 };
            craftingOutputCell_Table.UpdateCell();
        }

    }
    
    public void OnButtonClicked_Left(InventoryCell touchedCell)
    {
        InventorySlot touchedSlot = inventoryDictionary[touchedCell];
        if (!holding)// not holding anything
        {
            if (HasItemIn(touchedCell))//has item
            {

                if (touchedCell.isCraftingOutput)
                {
                    //hold the thing in cell
                    if (Input.GetKey(KeyCode.LeftShift) && (curMode == InventoryMode.Crafting || curMode == InventoryMode.Normal))
                    {
                        holding = true;
                        holdingInventorySlot.item = touchedSlot.item;
                        while (true)
                        {
                            holdingInventorySlot.amount += touchedSlot.amount;
                            UseItemsInCraftingSlot();
                            CheckCraftingInput();
                            if (inventoryDictionary[touchedCell].amount < 1 || inventoryDictionary[touchedCell].item != holdingInventorySlot.item)
                                break;
                        }
                    }
                    else
                    {
                        holding = true;
                        holdingInventorySlot.item = touchedSlot.item;
                        holdingInventorySlot.amount = touchedSlot.amount;
                        inventoryDictionary[touchedCell].amount = 0;
                        touchedCell.UpdateCell();
                        if (curMode == InventoryMode.Crafting || curMode == InventoryMode.Normal)
                        {
                            UseItemsInCraftingSlot();
                        }
                    }
                }
                else
                {

                    //hold the thing in cell
                    holding = true;
                    holdingInventorySlot.item = touchedSlot.item;
                    holdingInventorySlot.amount = touchedSlot.amount;

                    inventoryDictionary[touchedCell].amount = 0;
                    touchedCell.UpdateCell();

                }
            }
        }
        else//holding something
        {
            if (!HasItemIn(touchedCell) && !touchedCell.isCraftingOutput)//if it is empty place, just add the thing you were holding to that cell
            {
                inventoryDictionary[touchedCell].item = holdingInventorySlot.item;
                inventoryDictionary[touchedCell].amount = holdingInventorySlot.amount;
                touchedCell.UpdateCell();

                holding = false;
                holdingInventorySlot.amount = 0;
            }
            else if(touchedSlot.item == holdingInventorySlot.item)//if it is same as the thing you're holding add count of cell to holding and remove the cell
            {
                if (!touchedCell.isCraftingOutput)
                {
                    inventoryDictionary[touchedCell].amount += holdingInventorySlot.amount;
                    holdingInventorySlot.amount = 0;
                    touchedCell.UpdateCell();
                    holding = false;
                }
                else
                {
                    if (Input.GetKey(KeyCode.LeftShift) && (curMode == InventoryMode.Crafting || curMode == InventoryMode.Normal))
                    {
                        while (true)
                        {
                            holdingInventorySlot.amount += touchedSlot.amount;
                            UseItemsInCraftingSlot();
                            CheckCraftingInput();
                            if (inventoryDictionary[touchedCell].amount < 1)
                                break;
                        }
                    }
                    else
                    {
                        holdingInventorySlot.amount += inventoryDictionary[touchedCell].amount;
                        inventoryDictionary[touchedCell].amount = 0;
                        touchedCell.UpdateCell();
                        if(curMode == InventoryMode.Crafting || curMode == InventoryMode.Normal)
                        {
                            UseItemsInCraftingSlot();
                        }
                    }
                }
            }
            else if (!touchedCell.isCraftingOutput)//if it is different, hold that cell and add the thing you were holding to that cell
            {
                Item copiedItem = touchedSlot.item;
                int copiedAmount = touchedSlot.amount;


                inventoryDictionary[touchedCell].item = holdingInventorySlot.item;
                inventoryDictionary[touchedCell].amount = holdingInventorySlot.amount;
                touchedCell.UpdateCell();


                holdingInventorySlot.item = copiedItem;
                holdingInventorySlot.amount = copiedAmount;
            }
        }
        UpdateSeletedSlot();
        CheckCraftingInput();
    }

    public void OnButtonClicked_Right(InventoryCell touchedCell)
    {
        InventorySlot touchedSlot = inventoryDictionary[touchedCell];
        if (!holding)// not holding anything
        {
            if (HasItemIn(touchedCell))//has item
            {
                //hold the thing in cell

                if (touchedCell.isCraftingOutput)
                {
                    holding = true;
                    holdingInventorySlot.item = touchedSlot.item;
                    holdingInventorySlot.amount = touchedSlot.amount;

                    inventoryDictionary[touchedCell].amount = 0;
                    touchedCell.UpdateCell();

                    UseItemsInCraftingSlot();
                }
                else
                {
                    int gettingAmount = Mathf.Clamp(Mathf.CeilToInt(touchedSlot.amount * 0.5f),0, inventoryDictionary[touchedCell].amount);
                    holding = true;
                    holdingInventorySlot.item = touchedSlot.item;
                    holdingInventorySlot.amount = gettingAmount;

                    inventoryDictionary[touchedCell].amount -= gettingAmount;
                    touchedCell.UpdateCell();

                }
            }
        }
        else//holding something
        {
            if (!HasItemIn(touchedCell) && !touchedCell.isCraftingOutput)//if it is empty place, just add the thing you were holding to that cell
            {
                inventoryDictionary[touchedCell].item = holdingInventorySlot.item;
                inventoryDictionary[touchedCell].amount += 1; //holdingInventorySlot.amount;
                touchedCell.UpdateCell();

                holdingInventorySlot.amount -= 1;
                if(holdingInventorySlot.amount <= 0)
                {
                    holding = false;
                }
            }
            else if (touchedSlot.item == holdingInventorySlot.item)//if it is same as the thing you're holding add count of cell to holding and remove the cell
            {
                if (!touchedCell.isCraftingOutput)
                {
                    inventoryDictionary[touchedCell].amount++;
                    holdingInventorySlot.amount--;
                    touchedCell.UpdateCell();
                }
                else
                {
                    holdingInventorySlot.amount += inventoryDictionary[touchedCell].amount;
                    UseItemsInCraftingSlot();
                }

            }
            else if(!touchedCell.isCraftingOutput)//if it is different, hold that cell and add the thing you were holding to that cell
            {
                Item copiedItem = touchedSlot.item;
                int copiedAmount = touchedSlot.amount;


                inventoryDictionary[touchedCell].item = holdingInventorySlot.item;
                inventoryDictionary[touchedCell].amount = holdingInventorySlot.amount;
                touchedCell.UpdateCell();


                holdingInventorySlot.item = copiedItem;
                holdingInventorySlot.amount = copiedAmount;
            }
        }
        UpdateSeletedSlot();
        CheckCraftingInput();
    }

    public void UseItemsInCraftingSlot()
    {
        //trade
        if(curMode == InventoryMode.Normal)
        {
            foreach (InventoryCell ic in craftingCells)
            {
                if (inventoryDictionary[ic].amount > 0)
                {
                    inventoryDictionary[ic].amount--;
                    ic.UpdateCell();
                }
            }
        }
        else if (curMode == InventoryMode.Crafting)
        {
            foreach (InventoryCell ic in craftingCells_Table)
            {
                if (inventoryDictionary[ic].amount > 0)
                {
                    inventoryDictionary[ic].amount--;
                    ic.UpdateCell();
                }
            }
        }
    }

    public bool HasItemIn(InventoryCell ic)
    {
        return inventoryDictionary[ic].amount > 0;
    }

    public bool HasRoomFor(Item item)
    {
        foreach(InventorySlot slot in inventoryDictionary.Values)
        {
            if(slot.item == item || slot.amount <=0)
            {
                return true;
            }
        }
        return false;
    }

    public void ObtainItem(InventorySlot inputSlot, int a, int b)
    {
        for (int i = a; i < b; i++)
        {
            InventoryCell ic = inventoryCellList[i];

            InventorySlot checkingSlot = inventoryDictionary[ic];
            if (checkingSlot.item == inputSlot.item || checkingSlot.amount <= 0)
            {
                inventoryDictionary[ic].item = inputSlot.item;
                inventoryDictionary[ic].amount += inputSlot.amount;

                ic.UpdateCell();
                UpdateSeletedSlot();
                sm.PlaySound("ItemObtain", 1);
                return;
            }
        }
        itemSpawner.SpawnItem(inputSlot, player.transform.position, Quaternion.identity);
    }
    public void ObtainItem(InventorySlot inputSlot)
    {
        ObtainItem(inputSlot, 0, 36);
    }

    public bool HasItem(Item item, out InventoryCell havingCell)
    {
        for(int i =0; i < 36; i++)
        {
            if (inventoryDictionary[inventoryCellList[i]].item == item && inventoryDictionary[inventoryCellList[i]].amount > 0)
            {
                havingCell = inventoryCellList[i];
                return true;
            }
        }
        havingCell = null;
        return false;
    }

    public Item ItemByName(string itemName)
    {
        foreach(Item i in itemList)
        {
            if (i.name == itemName)
                return i;
        }
        Debug.LogWarning("item, " + itemName + " not found");
        return null;
    }
}
