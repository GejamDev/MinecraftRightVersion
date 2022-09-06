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

    public IEnumerator SearchForLinkedPortal(List<Vector3> portalPos, NetherPortalGenerationManager npgm)
    {

        foreach(Vector3 v in portalPos)
        {
            yield return new WaitUntil(() => npgm.netherPortalDictionary.ContainsKey(v));
            connectedPortalList.Add(npgm.netherPortalDictionary[v]);
        }
        Debug.Log("Ang");
    }
}
