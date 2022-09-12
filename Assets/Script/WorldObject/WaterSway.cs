using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSway : MonoBehaviour
{
    public PlayerScript ps;
    Vector3 origin;
    public Vector3 amount;
    public float speed;

    private void Awake()
    {
        origin = transform.localPosition;
        //ps = FindObjectOfType<PlayerScript>();
    }
    private void Update()
    {
        if (ps == null)
            return;
        transform.localPosition = origin + amount * Mathf.Sin(ps.playedTime*speed);

    }
}
