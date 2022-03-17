using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyLater : MonoBehaviour
{
    public float time;
    void Awake()
    {
        Destroy(gameObject, time);
    }
}
