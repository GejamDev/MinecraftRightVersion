using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeAnimator : MonoBehaviour
{
    public AnimationCurve curve;
    public bool loop;
    public float maxTime;
    float passedTime;
    Vector3 originScale;

    void Awake()
    {
        originScale = transform.localScale;
    }

    void Update()
    {
        transform.localScale = originScale * curve.Evaluate(passedTime);
        if (passedTime > maxTime)
        {
            if (loop)
            {
                passedTime = 0;
            }
            else
            {
                transform.localScale = originScale;
                Destroy(this);
            }
        }
        passedTime += Time.deltaTime;
    }
}
