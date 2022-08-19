using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpecialFeatures 
{
    public static List<Vector3> CoverListWithWalls(List<Vector3> orgList)
    {
        List<Vector3> result = new List<Vector3>();
        foreach (Vector3 v in orgList)
        {
            foreach(Vector3 sv in surroundingTable)
            {
                if (!result.Contains(v + sv))
                {
                    result.Add(v+sv);
                }
            }
        }
        return result;
    }















    static Vector3[] surroundingTable = new Vector3[26]
    {
                new Vector3(0,0,-1),
                new Vector3(0,0,1),
                new Vector3(0,-1,0),
                new Vector3(0,-1,-1),
                new Vector3(0,-1,1),
                new Vector3(0,1,0),
                new Vector3(0,1,-1),
                new Vector3(0,1,1),
                new Vector3(-1,0,0),
                new Vector3(-1,0,-1),
                new Vector3(-1,0,1),
                new Vector3(-1,-1,0),
                new Vector3(-1,-1,-1),
                new Vector3(-1,-1,1),
                new Vector3(-1,1,0),
                new Vector3(-1,1,-1),
                new Vector3(-1,1,1),
                new Vector3(1,0,0),
                new Vector3(1,0,-1),
                new Vector3(1,0,1),
                new Vector3(1,-1,0),
                new Vector3(1,-1,-1),
                new Vector3(1,-1,1),
                new Vector3(1,1,0),
                new Vector3(1,1,-1),
                new Vector3(1,1,1)
    };
}
