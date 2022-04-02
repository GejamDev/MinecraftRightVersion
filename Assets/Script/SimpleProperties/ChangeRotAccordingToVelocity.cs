using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeRotAccordingToVelocity : MonoBehaviour
{
    Rigidbody rb;
    public bool clip;
    public float syncSpeed;
    public bool active;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void LateUpdate()
    {
        if (!active)
            return;


        Vector3 vel = rb.velocity;

        Quaternion targetRot = Quaternion.LookRotation(vel, Vector3.up);
        if (clip)
        {
            transform.rotation = targetRot;
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, syncSpeed * Time.deltaTime);
        }
    }
}
