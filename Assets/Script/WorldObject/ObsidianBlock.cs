using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObsidianBlock : MonoBehaviour
{
    public ChunkScript cs;
    public List<NetherPortal> connectedPortalList = new List<NetherPortal>();

    private void OnDestroy()
    {
        if (cs == null)
            return;
        for(int i =0; i <connectedPortalList.Count; i++)
        {
            NetherPortal np = connectedPortalList[i];
            if (np == null)
            {
                connectedPortalList.RemoveAt(i);
                i--;
            }
            else
            {
                np.cs.netherPortalData.Remove(np.posInChunk);
                Destroy(np.gameObject);
            }
        }
        cs.obsidianData.Remove(this);
    }
}
