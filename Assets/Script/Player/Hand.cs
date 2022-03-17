using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public Transform handTransform;
    public Vector3 targetPosition;
    public Quaternion targetRotation;

    public float posFollowSpeed;
    public float rotFollowSpeed;

    void Update()
    {
        targetPosition = handTransform.position;
        targetRotation = handTransform.rotation;
        transform.position = targetPosition;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotFollowSpeed * Time.deltaTime);
    }
}
