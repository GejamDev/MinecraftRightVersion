using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryCell : MonoBehaviour, IPointerClickHandler
{
    public InventoryManager im;
    public Image iconImage;
    public Text amountText;
    public InventoryCell synchronizedCell;

    public bool insideInventory;
    public bool isCraftingOutput;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            im.OnButtonClicked_Left(this);
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            im.OnButtonClicked_Right(this);
        }
    }
    public void UpdateCell()
    {
        if (insideInventory)
        {
            InventorySlot slot = im.inventoryDictionary[this];
            if (slot.amount == 0)
            {
                iconImage.color = new Color(1, 1, 1, 0);
                amountText.text = null;
            }
            else
            {
                iconImage.color = Color.white;
                amountText.text = slot.amount == 1 ? null : slot.amount.ToString();

                iconImage.sprite = slot.item.image;
            }
            if(synchronizedCell != null)
            {
                synchronizedCell.iconImage.color = iconImage.color;
                synchronizedCell.iconImage.sprite = iconImage.sprite;
                synchronizedCell.amountText.text = amountText.text;
            }
        }
        else
        {
            iconImage.raycastTarget = false;
        }
    }

}
