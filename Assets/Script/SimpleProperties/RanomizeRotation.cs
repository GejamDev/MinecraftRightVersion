using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RanomizeRotation : MonoBehaviour
{
    private void Awake()
    {
        transform.eulerAngles = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
    }
}
