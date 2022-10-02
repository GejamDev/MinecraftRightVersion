using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetherPortalGenerationManager : MonoBehaviour
{
    public UniversalScriptManager usm;
    CameraManager cm;
    ChunkLoader cl;
    DimensionTransportationManager dtm;
    BlockPlacementManager bpm;
     public List<NetherPortalRecipe> netherPortalRecipes = new List<NetherPortalRecipe>();
    public GameObject netherPortalPrefab;
    public float generated_camShakeTime;
    public float generated_camShakeIntensity;
    public Item netherPortalItem;
    public Item obsidianBlockItem;
    public Dictionary<Vector3, NetherPortal> netherPortalDictionary = new Dictionary<Vector3, NetherPortal>();
    public Dictionary<Vector3, NetherPortal> nether_netherPortalDictionary = new Dictionary<Vector3, NetherPortal>();

    private void Awake()
    {
        cm = usm.cameraManager;
        dtm = usm.dimensionTransportationManager;
        cl = usm.chunkLoader;
        bpm = usm.blockPlacementManager;
    }
    public void TryMakeNetherPortal(ObsidianBlock ob, out bool succeded)
    {
        List<ObsidianBlock> obsidianDatas = new List<ObsidianBlock>();
        Vector3 pos = ob.transform.position;
        #region get nearby obsidian datas 
        ChunkScript cs = ob.transform.parent.parent.gameObject.GetComponent<ChunkScript>();
        foreach (ObsidianBlock o in cs.obsidianData)
        {
            obsidianDatas.Add(o);
        }
        foreach (ObsidianBlock o in cs.rightChunk.obsidianData)
        {
            obsidianDatas.Add(o);
        }
        foreach (ObsidianBlock o in cs.leftChunk.obsidianData)
        {
            obsidianDatas.Add(o);
        }
        foreach (ObsidianBlock o in cs.frontChunk.obsidianData)
        {
            obsidianDatas.Add(o);

        }
        foreach (ObsidianBlock o in cs.backChunk.obsidianData)
        {
            obsidianDatas.Add(o);
        }
        foreach (ObsidianBlock o in cs.rightChunk.frontChunk.obsidianData)
        {
            obsidianDatas.Add(o);
        }
        foreach (ObsidianBlock o in cs.rightChunk.backChunk.obsidianData)
        {
            obsidianDatas.Add(o);
        }
        foreach (ObsidianBlock o in cs.leftChunk.frontChunk.obsidianData)
        {
            obsidianDatas.Add(o);

        }
        foreach (ObsidianBlock o in cs.leftChunk.backChunk.obsidianData)
        {
            obsidianDatas.Add(o);
        }

        #endregion


        List<ObsidianBlock> usedObsidians = new List<ObsidianBlock>();
        foreach (NetherPortalRecipe recipe in netherPortalRecipes)
        {
            foreach(Vector3 startpos in recipe.vectorList)
            {
                if(qualify(recipe.vectorList, pos, startpos, obsidianDatas, out usedObsidians))
                {
                    succeded = true;

                    Debug.Log("success");
                    cm.ShakeCamera(generated_camShakeTime, generated_camShakeIntensity, true, 0);
                    cm.ShakeCamera(generated_camShakeTime*15, generated_camShakeIntensity/30, true, 0);
                    if (cs.netherPortalData.Contains(recipe.t_pos + pos - startpos - new Vector3(cs.position.x, 0, cs.position.y)))
                    {
                        succeded = false;
                        return;
                    }
                    //make portal
                    GameObject p = Instantiate(netherPortalPrefab);
                    p.transform.position = recipe.t_pos + pos-startpos;
                    p.transform.eulerAngles = recipe.t_rot;
                    NetherPortal np = p.GetComponent<NetherPortal>();
                    np.cs = cs;
                    np.posInChunk = p.transform.position - new Vector3(cs.position.x, 0, cs.position.y) + new Vector3(0.1f, 0.1f, 0.1f);
                    p.transform.SetParent(cs.objectBundle.transform);
                    cs.netherPortalData.Add(p.transform.position - new Vector3(cs.position.x, 0, cs.position.y));
                    cs.blockDataList.Add(new BlockData { block = netherPortalItem, obj = p });

                    foreach (ObsidianBlock usedob in usedObsidians)
                    {
                        usedob.connectedPortalList.Add(np);
                        np.usedObBlock.Add(usedob);
                    }
                    switch (dtm.currentDimesnion)
                    {
                        case Dimension.OverWorld:
                            netherPortalDictionary.Add(p.transform.position, np);
                            break;
                        case Dimension.Nether:
                            nether_netherPortalDictionary.Add(p.transform.position, np);
                            break;
                    }


                    return;
                }
            }
        }

        succeded = false;
    }
    public void CopyNetherPortal(NetherPortal source, Dimension targetDimension)
    {
        StartCoroutine(CopyNetherPortal_Cor(source, targetDimension));
    }
    IEnumerator CopyNetherPortal_Cor(NetherPortal source, Dimension targetDimension)
    {
        Debug.Log("copying");
        switch (targetDimension)
        {
            case Dimension.OverWorld:
                if (netherPortalDictionary.ContainsKey(source.transform.position))
                    yield break;
                break;
            case Dimension.Nether:
                if (nether_netherPortalDictionary.ContainsKey(source.transform.position))
                    yield break;
                break;
        }
        GameObject p = Instantiate(netherPortalPrefab);
        p.transform.position = source.transform.position;
        p.transform.eulerAngles = source.transform.eulerAngles;
        NetherPortal np = p.GetComponent<NetherPortal>();
        Dictionary<Vector2, ChunkProperties> sourceChunkDic = targetDimension == Dimension.OverWorld ? cl.chunkDictionary : cl.nether_chunkDictionary;
        Debug.Log(source.cs.position);
        yield return new WaitUntil(() => sourceChunkDic.ContainsKey(source.cs.position));
        ChunkScript cs = sourceChunkDic[source.cs.position].cs;
        np.cs = cs;
        np.posInChunk = p.transform.position - new Vector3(cs.position.x, 0, cs.position.y) + new Vector3(0.1f, 0.1f, 0.1f);
        p.transform.SetParent(cs.objectBundle.transform);
        cs.netherPortalData.Add(p.transform.position - new Vector3(cs.position.x, 0, cs.position.y));
        cs.blockDataList.Add(new BlockData { block = netherPortalItem, obj = p });
        Debug.Log(source.usedObBlock.Count);
        foreach (ObsidianBlock usedob in source.usedObBlock)
        {
            GameObject block = bpm.PlaceBlock_Obj(obsidianBlockItem, null, usedob.transform.position, false, targetDimension, false);
            Debug.Log(block);
            if (block != null)
            {
                ObsidianBlock ob = block.GetComponent<ObsidianBlock>();
                np.usedObBlock.Add(ob);
            }
        }
        switch (targetDimension)
        {
            case Dimension.OverWorld:
                netherPortalDictionary.Add(p.transform.position, np);
                break;
            case Dimension.Nether:
                nether_netherPortalDictionary.Add(p.transform.position, np);
                break;
        }
    }
    bool qualify(List<Vector3> recipe, Vector3 orgPos, Vector3 startPos, List<ObsidianBlock> obsidianDatas, out List<ObsidianBlock> usedObsidians)
    {
        Vector3 offset = orgPos - startPos;
        List<ObsidianBlock> obsidianBlockList = new List<ObsidianBlock>();
        foreach (Vector3 checkingPos in recipe)
        {
            ObsidianBlock resultOb;
            if (obsidianDataContain(obsidianDatas, checkingPos + offset, out resultOb))
            {
                obsidianBlockList.Add(resultOb);
            }
            else
            {
                usedObsidians = null;
                return false;
            }

        }
        usedObsidians = obsidianBlockList;
        return true;
    }
    bool obsidianDataContain(List<ObsidianBlock> obData, Vector3 pos, out ObsidianBlock ob)
    {
        foreach(ObsidianBlock o in obData)
        {
            if(o.transform.position == pos)
            {
                ob = o;
                return true;
            }
        }
        ob = null;
        return false;
    }
}
[System.Serializable]
public class NetherPortalRecipe
{
    public List<Vector3> vectorList = new List<Vector3>();
    public Vector3 t_pos;
    public Vector3 t_rot;
}


