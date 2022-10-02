using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObsidianBlock : MonoBehaviour
{
    public ChunkScript cs;
    public List<NetherPortal> connectedPortalList = new List<NetherPortal>();

    private void Destroy()
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
                foreach(ObsidianBlock ob in np.usedObBlock)
                {
                    if (ob.connectedPortalList.Contains(np))
                    {
                        ob.connectedPortalList.Remove(np);
                    }
                }
                np.cs.netherPortalData.Remove(np.posInChunk);
                Destroy(np.gameObject);
            }
        }
        cs.obsidianData.Remove(this);
    }
    public IEnumerator SearchForLinkedPortal(List<Vector3> portalPos, NetherPortalGenerationManager npgm, Dimension dimension)
    {
        switch (dimension)
        {
            case Dimension.OverWorld:
                foreach (Vector3 v in portalPos)
                {
                    yield return new WaitUntil(() => npgm.netherPortalDictionary.ContainsKey(v));
                    connectedPortalList.Add(npgm.netherPortalDictionary[v]);
                    npgm.netherPortalDictionary[v].usedObBlock.Add(this);
                }
                break;
            case Dimension.Nether:
                foreach (Vector3 v in portalPos)
                {
                    yield return new WaitUntil(() => npgm.nether_netherPortalDictionary.ContainsKey(v));
                    connectedPortalList.Add(npgm.nether_netherPortalDictionary[v]);
                    npgm.nether_netherPortalDictionary[v].usedObBlock.Add(this);
                }
                break;
        }
    }
}
