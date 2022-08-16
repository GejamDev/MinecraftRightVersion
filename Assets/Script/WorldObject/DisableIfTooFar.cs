using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableIfTooFar : MonoBehaviour
{
    public MeshRenderer mr;
    public MeshCollider mc;
    public GameObject player;
    public float minDistance;
    public float rate;

    public void Awake()
    {
        Invoke(nameof(CheckDisable), rate);
    }

    void CheckDisable()
    {
        if (player != null)
        {

            bool disableing = Vector3.Distance(transform.position, player.transform.position) > minDistance;
            mc.enabled = !disableing;
            mr.enabled = !disableing;
        }
        Invoke(nameof(CheckDisable), rate);
    }
}
