using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OnOffButton : MonoBehaviour
{
    public bool value;
    public GameObject toggle;
    public bool valueChanged;

    void Start()
    {
        toggle.SetActive(value);
    }
    public void OnClick()
    {
        Debug.Log(gameObject.name);
        value = !value;
        valueChanged = true;
        toggle.SetActive(value);
    }
}
