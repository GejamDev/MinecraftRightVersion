using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassScript : MonoBehaviour
{
    public GameObject grassObject;
    public GameObject grassHolder;
    public GameObject destroyParticle;
    public MeshRenderer mr;
    public Material[] materials;

    public void Awake()
    {
        mr.material = materials[Random.Range(0, materials.Length)];
    }

    public void Scatter()
    {
        GameObject p = Instantiate(destroyParticle);
        p.transform.position = transform.position;
        Destroy(gameObject);
    }
}
