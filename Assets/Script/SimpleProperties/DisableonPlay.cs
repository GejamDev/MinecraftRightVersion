using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableonPlay : MonoBehaviour
{
    void Awake()
    {
        gameObject.SetActive(false);
    }
}
