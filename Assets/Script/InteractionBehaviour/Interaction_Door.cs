using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interaction_Door : MonoBehaviour
{
    public bool opened;

    public Transform doorBundle;
    public float originAngle;
    public Vector3 originPos;
    public float openAngle;
    public Vector3 openPos;
    public float speed;

    public void Interact()
    {
        opened = !opened;
    }

    void Update()
    {
        doorBundle.localPosition = Vector3.Lerp(doorBundle.localPosition, opened ? openPos : originPos, speed * Time.deltaTime);
        doorBundle.localRotation = Quaternion.Lerp(doorBundle.localRotation, Quaternion.Euler(0,opened ? openAngle : originAngle, 0), speed * Time.deltaTime);
    }
}
