using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireScript : MonoBehaviour
{
    [HideInInspector] public UniversalScriptManager usm;
    public ChunkScript parentChunk;
    PlayerScript ps;
    public float litTime;
    public void GetVariables()
    {
        ps = usm.player.GetComponent<PlayerScript>();
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == usm.player)
        {
            ps.LitFire(litTime);
        }
        if (other.gameObject.CompareTag("Water") || other.gameObject.CompareTag("Lava"))
        {
            DestroyFire();
        }
    }
    public void DestroyFire()
    {
        parentChunk.fireData.Remove(transform.localPosition);
        Destroy(gameObject);
    }
}
