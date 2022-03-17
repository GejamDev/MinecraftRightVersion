using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CraftingManager
{
    public static bool TryToCraft(Item[,] data, CraftingRecipe cr)
    {
        //get width,  height of data
        bool horz_hasLeft = data[0, 0] != null || data[0, 1] != null || data[0, 2] != null;
        bool horz_hasMiddle = data[1, 0] != null || data[1, 1] != null || data[1, 2] != null;
        bool horz_hasRight = data[2, 0] != null || data[2, 1] != null || data[2, 2] != null;
        bool vert_hasTop = data[0, 0] != null || data[1, 0] != null || data[2, 0] != null;
        bool vert_hasMiddle = data[0, 1] != null || data[1, 1] != null || data[2, 1] != null;
        bool vert_hasDown = data[0, 2] != null || data[1, 2] != null || data[2, 2] != null;
        if(!horz_hasLeft && !horz_hasRight && !horz_hasMiddle)
        {
            return false;
        }




        int width = 0;
        int height = 0;
        if(horz_hasLeft && horz_hasRight)
        {
            width = 3;
        }
        else if(horz_hasMiddle && (horz_hasLeft || horz_hasRight))
        {
            width = 2;
        }
        else
        {
            width = 1;
        }

        if (vert_hasTop&& vert_hasDown)
        {
            height = 3;
        }
        else if (vert_hasMiddle && (vert_hasDown || vert_hasTop))
        {
            height = 2;
        }
        else
        {
            height = 1;
        }
        //for(int i = 0; i < 9; i++)
        //{
        //    Debug.Log(i.ToString() + "_______________________________________________");
        //    Debug.Log(cr.inputItem[i]);
        //    Debug.Log(Item2DArrayTo1D(data)[i]);
        //}
        
        if (isArraySame(Item2DArrayTo1D(data), cr.inputItem))
        {
            return true;
        }

        if(width !=3||height!=3)
        {
            List<int> possibleX = new List<int>();
            List<int> possibleY = new List<int>();
            possibleX.Add(0);
            possibleY.Add(0);

            if (!horz_hasLeft)
            {
                possibleX.Add(-1);
                if (!horz_hasMiddle)
                {
                    possibleX.Add(-2);
                }
            }
            if (!horz_hasRight)
            {
                possibleX.Add(1);
                if (!horz_hasMiddle)
                {
                    possibleX.Add(2);
                }
            }

            if (!vert_hasTop)
            {
                possibleY.Add(-1);
                if (!vert_hasMiddle)
                {
                    possibleY.Add(-2);
                }
            }
            if (!vert_hasDown)
            {
                possibleY.Add(1);
                if (!vert_hasMiddle)
                {
                    possibleY.Add(2);
                }
            }


            for (int x = 0; x < possibleX.Count; x++)
            {
                for (int y = 0; y < possibleY.Count; y++)
                {
                    Vector2Int offset = new Vector2Int(possibleX[x], possibleY[y]);

                    Item[,] offsetData = OffsetedItemArray(data, offset);


                    if (isArraySame(Item2DArrayTo1D(offsetData), cr.inputItem))
                    {
                        return true;
                    }
                }
            }
        }


        return false;
    }
    public static Item[] Item2DArrayTo1D(Item[,] data)
    {
        Item[] result = new Item[9];
        int index = 0;
        for(int y = 0; y < 3; y++)
        {
            for(int x = 0; x < 3;x++)
            {
                result[index] = data[x, y];
                index++;
            }
        }
        return result;
    }
    public static bool isArraySame(Item[] a, Item[] b)
    {
        if (a.Length != b.Length)
            return false;
        for(int i = 0; i <a.Length; i++)
        {
            if (a[i] != b[i])
                return false;
        }
        return true;
    }
    public static Item[,] OffsetedItemArray(Item[,] data, Vector2Int offset)
    {
        Item[,] origin = data;
        Item[,] result = new Item[3, 3];

        for(int x= 0; x < 3; x++)
        {
            for(int y = 0; y < 3; y++)
            {
                Vector2Int copyingPosition = new Vector2Int(x - offset.x, y - offset.y);
                if(copyingPosition.x < 0 || copyingPosition.x > 2 || copyingPosition.y < 0 || copyingPosition.y > 2)
                {
                    result[x, y] = null;
                }
                else
                {
                    result[x, y] = origin[copyingPosition.x, copyingPosition.y];
                }
            }
        }

        return result;
    }
}
