using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPositionRecorder : MonoBehaviour
{
    public UniversalScriptManager usm;
    public Vector3 lastPos_overWorld;
    public Vector3 lastPos_nether;
    private void Update()
    {
        switch (usm.dimensionTransportationManager.currentDimesnion)
        {
            case Dimension.OverWorld:
                lastPos_overWorld = usm.player.transform.position;
                break;
            case Dimension.Nether:
                lastPos_nether = usm.player.transform.position;
                break;
        }
    }
}
