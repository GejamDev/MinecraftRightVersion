using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeScript : MonoBehaviour
{
    public UniversalScriptManager usm;
    public float yPos;
    public float treeBundleDefaultYPos;
    public GameObject objectBundle;
    public GameObject treeTrunk;
    public float minHeight;
    public float maxHeight;
    public Mesh[] treeMeshes;
    public Transform leafSpawnPos;
    public GameObject leaf;
    public Mesh[] leafMeshes;
    public Material[] leafMaterails;
    public float minLeafScale;
    public float maxLeafScale;
    MeshFilter TT_mf;
    MeshCollider TT_mc;
    MeshFilter L_mf;
    MeshCollider L_mc;
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
        //get variables
        TT_mf = treeTrunk.GetComponent<MeshFilter>();
        TT_mc = treeTrunk.GetComponent<MeshCollider>();
        L_mf = leaf.GetComponent<MeshFilter>();
        L_mc = leaf.GetComponent<MeshCollider>();


        //randomize tree
        Mesh treeMesh = treeMeshes[Random.Range(0, treeMeshes.Length)];
        TT_mf.mesh = treeMesh;
        TT_mc.sharedMesh = treeMesh;

        float height = Random.Range(minHeight, maxHeight);
        treeTrunk.transform.localScale = new Vector3(1, height, 1);
        treeTrunk.transform.localPosition = new Vector3(0, height * 0.5f, 0);

        treeTrunk.transform.eulerAngles = new Vector3(0, Random.Range(0, 360), 0);


        //randomize leaf
        if(leafMeshes.Length != 0)
        {
            leaf.transform.position = leafSpawnPos.position;
            Mesh leafMesh = leafMeshes[Random.Range(0, leafMeshes.Length)];

            L_mf.mesh = leafMesh;
            L_mc.sharedMesh = leafMesh;

            leaf.transform.eulerAngles = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
            leaf.transform.localScale = Vector3.one * Random.Range(minLeafScale, maxLeafScale);

            leaf.GetComponent<MeshRenderer>().material = leafMaterails[Random.Range(0, leafMaterails.Length)];
        }
    }
}
