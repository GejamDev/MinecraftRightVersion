using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    public bool setToSetting;




    public UniversalScriptManager usm;
    GameObject player;
    WorldGenerationPreset wgPreset;
    WorldGenerator wg;
    LoadingManager lm;

    [Header("Variables")]
    public bool loadingTerrain;
    public bool firstTimeLoading;
    public float firstTimeLoadingTime;
    public bool playerPositionChanged;
    public Vector2 roundedPlayerPosition;

    [Header("Settings")]
    public float loadingTime;
    public int firstTimeLoadingFrequency;
    public int loadingFrequency;
    [Range(1, 30)] public int viewDistance;


    [Header("Chunk")]
    public List<Vector2> enabledChunkPositionList = new List<Vector2>();
    public List<ChunkProperties> activatedChunkList = new List<ChunkProperties>();
    public Dictionary<Vector2, ChunkProperties> chunkDictionary = new Dictionary<Vector2, ChunkProperties>();
    

    void Awake()
    {
        firstTimeLoading = true;
        loadingTerrain = false;
        player = usm.player;
        wgPreset = usm.worldGenerationPreset;
        wg = usm.worldGenerator;
        lm = usm.loadingManager;
        if(setToSetting)
            viewDistance = Mathf.RoundToInt(PlayerPrefs.GetFloat("ViewDistance") * 9) + 6;
    }

    void Update()
    {

        CheckPlayerPosition();

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
            viewDistance = Mathf.RoundToInt(PlayerPrefs.GetFloat("ViewDistance") * 9) + 6;

        if (viewDistance != preViewDistance)
        {
            StartCoroutine(UpdateEnabledChunk_Cor());
        }
    }
    void CheckPlayerPosition()
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
        //update viewable chunk positions
        enabledChunkPositionList.Clear();
        for(int x = 1 - viewDistance; x < viewDistance; x++)
        {
            for (int y = 1 - viewDistance; y < viewDistance; y++)
            {
                enabledChunkPositionList.Add(new Vector2(x, y) * wgPreset.chunkSize + roundedPlayerPosition);
            }
        }

        //remove chunks from list that is not in the view distance
        for(int i = 0; i < activatedChunkList.Count; i++)
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
            foreach(ChunkProperties cp in activatedChunkList)
            {
                if(cp.position == addingPosition)
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
                    ChunkScript cs = wg.MakeChunkAt(addingPosition, !spawnPlayerAtEnd);//main problem
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
        
        loadingTerrain = false;
        if (spawnPlayerAtEnd)
        {
            SpawnPlayer();
        }
        yield return null;

    }
    public void SpawnPlayer()
    {

        Vector3 spawnPos = Vector3.zero;
        spawnPos += Vector3.up * chunkDictionary[Vector2.zero].cs.heightMap[0, 0];
        spawnPos += Vector3.up * 2;
        player.transform.position = spawnPos;
        lm.loading = false;
    }
}
