using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ValueSlider : MonoBehaviour
{
    public Slider s;
    public bool valueChanged;

    void Awake()
    {
        s.onValueChanged.AddListener(delegate { OnValueChanged(); });
    }
    public void OnValueChanged()
    {
        valueChanged = true;
    }
}
