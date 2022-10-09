using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrightnessDetector : MonoBehaviour
{
    [Range(0,1)]public float brightness;
    public float raycastDistance;
    public float detectionArea;
    public float detectionThiccness;
    public LayerMask ceilingLayer;
    public float strightness;
    public int totalcount;
    private void Awake()
    {
        for (float x = -detectionArea * 0.5f; x <= detectionArea * 0.5f; x += detectionThiccness)
        {
            for (float y = -detectionArea * 0.5f; y <= detectionArea * 0.5f; y += detectionThiccness)
            {
                totalcount++;
            }
        }
    }

    public void Update()
    {
        CalculateBrightness();
    }
    public void CalculateBrightness()
    {
        int ceilingDetectedCount = 0;
        for(float x = - detectionArea*0.5f; x <= detectionArea*0.5f; x += detectionThiccness)
        {
            for (float y = -detectionArea * 0.5f; y <= detectionArea * 0.5f; y += detectionThiccness)
            {
                Debug.DrawRay(transform.position + new Vector3(x, 0, y), (new Vector3(x, strightness, y)).normalized * raycastDistance, Color.red);
                if (Physics.Raycast(transform.position + new Vector3(x, 0, y), (new Vector3(x, strightness, y)).normalized, raycastDistance, ceilingLayer))
                {
                    ceilingDetectedCount++;
                }
            }
        }
        brightness = 1 - (float)ceilingDetectedCount / (float)totalcount;
    }
}
