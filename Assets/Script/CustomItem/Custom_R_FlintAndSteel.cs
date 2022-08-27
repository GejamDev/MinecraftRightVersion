using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Custom_R_FlintAndSteel : MonoBehaviour
{
    public UniversalScriptManager usm;
    public Item item;
    public Animator handAnim;
    Transform cam;
    public float maxDistance;
    public LayerMask groundLayer;

    private void Awake()
    {
        cam = Camera.main.transform;

    }
    public void Use()
    {
        handAnim.SetTrigger(item.usingAnimationName);


        RaycastHit hit_water;
        if(Physics.Raycast(cam.position, cam.forward, out hit_water, maxDistance, groundLayer))
        {
            //iit fire
        }
    }
}
