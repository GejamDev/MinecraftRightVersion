using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactusScript : MonoBehaviour
{
    public float yPos;
    public GameObject[] cactuses;

    public void SetCactus()
    {
        foreach(GameObject g in cactuses)
        {
            g.SetActive(false);
        }
        cactuses[Random.Range(0, cactuses.Length)].SetActive(true);
        transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
        transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);
    }
}
