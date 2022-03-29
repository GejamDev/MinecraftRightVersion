using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FurnaceScript : MonoBehaviour
{
    public InventorySlot inputSlot;
    public InventorySlot fuelSlot;
    public InventorySlot outputSlot;

    public float curFuelProgress;
    public bool hasFuelPowerLeft;
    public float furnaceSpeed;

    public void Update()
    {
        if (inputSlot.amount == 0)
        {
            curFuelProgress = 0;
            return;
        }
        else if (inputSlot.item.furnaceResult == null)
        {
            curFuelProgress = 0;
            return;
        }
        else if (fuelSlot.amount == 0 && !hasFuelPowerLeft)
        {
            curFuelProgress = 0;
            return;
        }
        else if (!fuelSlot.item.fuel_usable)
        {
            curFuelProgress = 0;
            return;
        }
        else if (outputSlot.amount > 0)
        {
            if (outputSlot.item != inputSlot.item.furnaceResult)
            {
                curFuelProgress = 0;
                return;
            }
        }


        
        if (curFuelProgress >= 1)
        {
            //get one
            curFuelProgress = 0;
            outputSlot.item = inputSlot.item.furnaceResult;
            outputSlot.amount++;
            inputSlot.amount--;
            fuelSlot.amount--;
            if (fuelSlot.amount <= 0)
            {
                hasFuelPowerLeft = false;
            }
            else
            {
                hasFuelPowerLeft = true;
            }
        }
        else
        {
            curFuelProgress += Time.deltaTime * furnaceSpeed;
        }

    }
}
