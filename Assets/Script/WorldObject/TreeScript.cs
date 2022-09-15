using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeScript : MonoBehaviour
{
    public UniversalScriptManager usm;
    public float yPos;
    public float treeBundleDefaultYPos;
    public GameObject treeTrunkBundle;
    public TreeVarient[] treeVarients;


    public GameObject objectBundle;
    public float minHeight;
    public float maxHeight;
    public float minThiccness;
    public float maxThiccness;
    public Mesh[] leafMeshes;
    public float minLeafScale;
    public float maxLeafScale;
    Interaction_Tree it;

    public void SetTree()
    {
        GetComponent<Interaction_Tree>().usm = usm;
        GetComponent<Interaction_Tree>().SetVariables();


        objectBundle.transform.localPosition = Vector3.up * treeBundleDefaultYPos;
        Randomize();
    }
    public void Randomize()
    {
        Material[] usingMat;
        foreach(TreeVarient tv in treeVarients)
        {
            tv.trunk.SetActive(false);
        }
        TreeVarient usingTV = treeVarients[Random.Range(0, treeVarients.Length)];
        usingTV.trunk.SetActive(true);
        usingMat = usingTV.mat;
        foreach(GameObject l in usingTV.leaves)
        {
            l.GetComponent<MeshRenderer>().material = usingMat[Random.Range(0, usingMat.Length)];
            Mesh leafMesh = leafMeshes[Random.Range(0, leafMeshes.Length)];



            l.GetComponent<MeshFilter>().mesh = leafMesh;
            l.GetComponent<MeshCollider>().sharedMesh = leafMesh;

            l.transform.eulerAngles = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            l.transform.localScale *= Random.Range(minLeafScale, maxLeafScale);
        }





        float height = Random.Range(minHeight, maxHeight);
        float radius = Random.Range(minThiccness, maxThiccness);
        treeTrunkBundle.transform.localScale = new Vector3(radius, height, radius);
        treeTrunkBundle.transform.localPosition = new Vector3(0, height * 0.5f, 0);

        treeTrunkBundle.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);


    }
}

[System.Serializable]
public class TreeVarient
{
    public GameObject trunk;
    public GameObject[] leaves;
    public Material[] mat;
}