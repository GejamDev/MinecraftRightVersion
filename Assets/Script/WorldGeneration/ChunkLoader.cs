using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    public bool setToSetting;
    public bool setted;
    public bool hasSpecificSpawnPos;
    public Vector3 specificSpawnPos;
    public float fogPower;




    public UniversalScriptManager usm;
    GameObject player;
    WorldGenerationPreset wgPreset;
    WorldGenerator wg;
    LoadingManager lm;
    TerrainModifier tm;
    DimensionTransportationManager dtm;

    [Header("Variables")]
    public bool loadingTerrain;
    public bool firstTimeLoading;
    public float firstTimeLoadingTime;
    public bool playerPositionChanged;
    public Vector2 roundedPlayerPosition;
    public GameObject overWorldTransform;
    public GameObject netherTransform;

    [Header("Settings")]
    public float loadingTime;
    public int firstTimeLoadingFrequency;
    public int loadingFrequency;
    [Range(1, 50)] public int viewDistance;


    [Header("Chunk")]
    public List<Vector2> enabledChunkPositionList = new List<Vector2>();
    public List<ChunkProperties> activatedChunkList = new List<ChunkProperties>();
    public List<ChunkProperties> netherActivatedChunkList = new List<ChunkProperties>();
    public Dictionary<Vector2, ChunkProperties> chunkDictionary = new Dictionary<Vector2, ChunkProperties>();
    public Dictionary<Vector2, ChunkProperties> nether_chunkDictionary = new Dictionary<Vector2, ChunkProperties>();


    void Awake()
    {
        firstTimeLoading = true;
        loadingTerrain = false;
        player = usm.player;
        wgPreset = usm.worldGenerationPreset;
        wg = usm.worldGenerator;
        lm = usm.loadingManager;
        tm = usm.terrainModifier;
        dtm = usm.dimensionTransportationManager;
        if (setToSetting)
        {
            float dist = PlayerPrefs.GetFloat("ViewDistance");
            viewDistance = Mathf.RoundToInt(dist * 12) + 6;
            RenderSettings.fogDensity = Mathf.Pow(10, Mathf.Lerp(-1.3f, -2, Mathf.Pow(dist, fogPower)));
        }
    }

    void Update()
    {
        if (setted)
        {
            CheckPlayerPosition();
        }

        if (firstTimeLoading)
        {
            StartCoroutine(UpdateEnabledChunk_Cor());
            firstTimeLoading = false;
        }
        else if(playerPositionChanged && !loadingTerrain)
        {
            StartCoroutine(UpdateEnabledChunk_Cor());
        }

        int preViewDistance = viewDistance;

        if (setToSetting)
        {
            float dist = PlayerPrefs.GetFloat("ViewDistance");
            viewDistance = Mathf.RoundToInt(dist * 12) + 6;
            RenderSettings.fogDensity = Mathf.Pow(10, Mathf.Lerp(-1.3f, -2, Mathf.Pow(dist, fogPower)));
        }

        if (viewDistance != preViewDistance)
        {
            StartCoroutine(UpdateEnabledChunk_Cor());
        }
    }
    public void CheckPlayerPosition()
    {
        Vector2 prePlayerPosition = roundedPlayerPosition;
        float playerPosX = player.transform.position.x;
        float playerPosZ = player.transform.position.z;
        roundedPlayerPosition = new Vector2(


            playerPosX >= 0 ?
            (playerPosX - (playerPosX % wgPreset.chunkSize)) :
            (playerPosX - (wgPreset.chunkSize - (Mathf.Abs(playerPosX) % wgPreset.chunkSize)))



            ,


            playerPosZ >= 0 ?
            (playerPosZ - (playerPosZ % wgPreset.chunkSize)) :
            (playerPosZ - (wgPreset.chunkSize - (Mathf.Abs(playerPosZ) % wgPreset.chunkSize)))

            );


        playerPositionChanged = prePlayerPosition != roundedPlayerPosition;
    }
    IEnumerator UpdateEnabledChunk_Cor()
    {
        loadingTerrain = true;


        bool spawnPlayerAtEnd = firstTimeLoading;

        if (spawnPlayerAtEnd)
        {
            lm.loading = true;
        }

        //wait if fisrt
        if (firstTimeLoading)
        {
            yield return new WaitUntil(() => setted);
        }



        switch (dtm.currentDimesnion)
        {
            case Dimension.OverWorld:
                yield return StartCoroutine(UpdateEnabledChunk_Cor_OverWorld(spawnPlayerAtEnd));
                break;
            case Dimension.Nether:
                yield return StartCoroutine(UpdateEnabledChunk_Cor_Nether(spawnPlayerAtEnd));
                break;
        }


    }
    IEnumerator UpdateEnabledChunk_Cor_OverWorld(bool spawnPlayerAtEnd)
    {

        overWorldTransform.SetActive(true);
        netherTransform.SetActive(false);
        //update viewable chunk positions
        enabledChunkPositionList.Clear();
        for (int x = 1 - viewDistance; x < viewDistance; x++)
        {
            for (int y = 1 - viewDistance; y < viewDistance; y++)
            {
                enabledChunkPositionList.Add(new Vector2(x, y) * wgPreset.chunkSize + roundedPlayerPosition);
            }
        }

        //remove chunks from list that is not in the view distance
        for (int i = 0; i < activatedChunkList.Count; i++)
        {
            if (!enabledChunkPositionList.Contains(activatedChunkList[i].position))
            {
                //actually disable chunk 
                activatedChunkList[i].cs.Deactivate();


                activatedChunkList.RemoveAt(i);




                i--;
            }
        }

        //add chunks to list that is in the view distance
        lm.goal = enabledChunkPositionList.Count;
        float lt = firstTimeLoading ? firstTimeLoadingTime : loadingTime;
        float lf = firstTimeLoading ? firstTimeLoadingFrequency : loadingFrequency;

        for (int i = 0; i < enabledChunkPositionList.Count; i++)
        {
            lm.curProgress = i + 1;
            Vector2 addingPosition = enabledChunkPositionList[i];


            //check if it has chunk
            bool containCurrentEnabledPosition = false;
            foreach (ChunkProperties cp in activatedChunkList)
            {
                if (cp.position == addingPosition)
                {
                    containCurrentEnabledPosition = true;
                    break;
                }
            }


            if (!containCurrentEnabledPosition)
            {
                if (chunkDictionary.ContainsKey(addingPosition))
                {
                    //add chunk to activated list
                    activatedChunkList.Add(chunkDictionary[addingPosition]);

                    //actually enable the chunk
                    chunkDictionary[addingPosition].cs.Activate();
                    //yield return new WaitForSeconds(loadingTime);
                }
                else
                {
                    //make chunk in world generator
                    ChunkScript cs = wg.MakeChunkAt(addingPosition, !spawnPlayerAtEnd, Dimension.OverWorld );
                    //StartCoroutine(wg.MakeChunkAt(addingPosition, !spawnPlayerAtEnd, Dimension.OverWorld, out cs));
                    ChunkProperties cp = new ChunkProperties();
                    cp.position = addingPosition;
                    cp.cs = cs;
                    chunkDictionary.Add(addingPosition, cp);
                    activatedChunkList.Add(cp);
                    if (i % lf == 0)
                    {
                        yield return new WaitForSeconds(lt);
                    }
                }



                i--;
            }
        }

        if (spawnPlayerAtEnd)
        {

            yield return new WaitForSeconds(1);
            SpawnPlayer_OverWorld();
        }
        loadingTerrain = false;
        yield return null;
    }
    IEnumerator UpdateEnabledChunk_Cor_Nether(bool spawnPlayerAtEnd)
    {
        overWorldTransform.SetActive(false);
        netherTransform.SetActive(true);
        loadingTerrain = false;


        #region generation

        enabledChunkPositionList.Clear();
        for (int x = 1 - viewDistance; x < viewDistance; x++)
        {
            for (int y = 1 - viewDistance; y < viewDistance; y++)
            {
                enabledChunkPositionList.Add(new Vector2(x, y) * wgPreset.chunkSize + roundedPlayerPosition);
            }
        }

        //remove chunks from list that is not in the view distance
        for (int i = 0; i < netherActivatedChunkList.Count; i++)
        {
            if (!enabledChunkPositionList.Contains(netherActivatedChunkList[i].position))
            {
                //actually disable chunk 
                netherActivatedChunkList[i].cs.Deactivate();


                netherActivatedChunkList.RemoveAt(i);




                i--;
            }
        }

        //add chunks to list that is in the view distance
        lm.goal = enabledChunkPositionList.Count;
        float lt = firstTimeLoading ? firstTimeLoadingTime : loadingTime;
        float lf = firstTimeLoading ? firstTimeLoadingFrequency : loadingFrequency;

        for (int i = 0; i < enabledChunkPositionList.Count; i++)
        {
            lm.curProgress = i + 1;
            Vector2 addingPosition = enabledChunkPositionList[i];


            //check if it has chunk
            bool containCurrentEnabledPosition = false;
            foreach (ChunkProperties cp in netherActivatedChunkList)
            {
                if (cp.position == addingPosition)
                {
                    containCurrentEnabledPosition = true;
                    break;
                }
            }


            if (!containCurrentEnabledPosition)
            {
                if (nether_chunkDictionary.ContainsKey(addingPosition))
                {
                    //add chunk to activated list
                    netherActivatedChunkList.Add(nether_chunkDictionary[addingPosition]);

                    //actually enable the chunk
                    nether_chunkDictionary[addingPosition].cs.Activate();
                    //yield return new WaitForSeconds(loadingTime);
                }
                else
                {
                    //make chunk in world generator
                    ChunkScript cs = wg.MakeChunkAt(addingPosition, !spawnPlayerAtEnd, Dimension.Nether);//main problem
                    ChunkProperties cp = new ChunkProperties();
                    cp.position = addingPosition;
                    cp.cs = cs;
                    nether_chunkDictionary.Add(addingPosition, cp);
                    netherActivatedChunkList.Add(cp);
                    if (i % lf == 0)
                    {
                        yield return new WaitForSeconds(lt);
                    }
                }



                i--;
            }
        }

#endregion


        if (spawnPlayerAtEnd)
        {
            usm.hpManager.lastGroundedHeight = 0;
            player.transform.position = specificSpawnPos;

            //ensure ground

            tm.Destruct_Custom(specificSpawnPos, dtm.playerEnsureRadius, Dimension.Nether);


            lm.loading = false;


        }
        yield return null;
    }
    public void SpawnPlayer_OverWorld()
    {

        if (hasSpecificSpawnPos)
        {
            player.transform.position = specificSpawnPos;
            lm.loading = false;
            tm.Destruct_Custom(specificSpawnPos, dtm.playerEnsureRadius, Dimension.OverWorld);
            return;
        }
        usm.hpManager.lastGroundedHeight = 0;
        Vector3 spawnPos = Vector3.zero;
        spawnPos += Vector3.up * chunkDictionary[Vector2.zero].cs.heightMap[0, 0];
        spawnPos += Vector3.up * 6;
        player.transform.position = spawnPos;
        lm.loading = false;
    }
}
