using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFog : MonoBehaviour
{
    GameObject player;
    public UniversalScriptManager usm;
    public Vector3 org;

    private void Awake()
    {
        player = usm.player;
        org = transform.position;
    }
    private void Update()
    {
        transform.position = new Vector3(player.transform.position.x, 0, player.transform.position.z) + org;
    }
}
