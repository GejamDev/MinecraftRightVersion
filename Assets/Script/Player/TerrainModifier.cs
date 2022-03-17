using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainModifier : MonoBehaviour
{
    public UniversalScriptManager usm;
    LoadingManager lm;
    InventoryManager im;
    CameraManager cm;
    MeshGenerator mg;
    UIManager um;
    SoundManager sm;

    public List<ChunkScript> modifiedChunkList = new List<ChunkScript>();
    public int terrainModifyFPS;
    public GameObject placeCursor;
    bool destroying;
    bool placing;
    public float maxDistance;
    Transform cam;
    LayerMask groundLayer;
    LayerMask objectLayer;
    public bool reachable;
    public ChunkScript currentlyTouchingChunk;
    public ModifiableObject currentlyTouchingObject;
    public Vector3 touchingPosition;
    WorldGenerationPreset wgPreset;
    ChunkLoader cl;
    public float destroySpeed;
    public float destroyMaxDistance;
    public AnimationCurve destroySpeedByDistanceCurve;
    public float placeSpeed;
    public float placeMaxDistance;
    public AnimationCurve placeSpeedByDistanceCurve;
    public List<Item> usedItem_CameraShake = new List<Item>();
    public GameObject rockDestroyParticle;
    public float rockObtainMultiplier;
    public Item rockItem;
    void Awake()
    {
        cam = usm.cam.transform;
        groundLayer = usm.groundLayer;
        objectLayer = usm.objectLayer;
        wgPreset = usm.worldGenerationPreset;
        cl = usm.chunkLoader;
        lm = usm.loadingManager;
        im = usm.inventoryManager;
        cm = usm.cameraManager;
        mg = usm.meshGenerator;
        um = usm.uiManager;
        sm = usm.soundManager;
    }
    void CheckInput()
    {
        destroying = Input.GetMouseButtonDown(0);
        placing = Input.GetMouseButton(1);
    }

    void Update()
    {
        if (lm.loading||im.showingInventoryUI)
        {
            return;
        }
        CheckRachable();
        if (reachable && currentlyTouchingChunk != null)
        {
            placeCursor.SetActive(true);
            placeCursor.transform.position = touchingPosition;
            placeCursor.transform.localScale = Vector3.one * (destroyMaxDistance + 0.5f);
        }
        else
        {
            placeCursor.SetActive(false);
        }
    }
    void CheckRachable()
    {
        RaycastHit hit_ground;
        bool checkedGround;
        currentlyTouchingObject = null;
        if (Physics.Raycast(cam.position, cam.forward, out hit_ground, maxDistance, groundLayer))
        {
            currentlyTouchingChunk = hit_ground.collider.gameObject.GetComponent<ObjectContainer>().obj.GetComponent<ChunkScript>();
            touchingPosition = hit_ground.point;
            reachable = true;
            checkedGround = true;
        }
        else
        {
            currentlyTouchingChunk = null;
            reachable = false;
            checkedGround = false;
        }

        if (checkedGround)
        {
            RaycastHit hit_object;
            if (Physics.Raycast(cam.position, cam.forward, out hit_object, Vector3.Distance(cam.position, touchingPosition) - 0.1f, objectLayer))
            {
                currentlyTouchingChunk = null;
                currentlyTouchingObject = hit_object.collider.gameObject.GetComponent<ObjectContainer>().obj.GetComponent<ModifiableObject>();
                touchingPosition = hit_object.point;
            }
        }
        else
        {
            RaycastHit hit_object;
            if (Physics.Raycast(cam.position, cam.forward, out hit_object, maxDistance, objectLayer))
            {
                touchingPosition = hit_object.point;

                currentlyTouchingObject = hit_object.collider.gameObject.GetComponent<ObjectContainer>().obj.GetComponent<ModifiableObject>();
                reachable = true;
            }
        }

    }
    public void DestroyInChunk()
    {

        Item usingItem;
        if (im.currentlyUsingInventorySlot.amount < 1)
        {
            usingItem = im.hand;
        }
        else
        {
            usingItem = im.currentlyUsingInventorySlot.item;
        }


        if (currentlyTouchingChunk == null)
        {
            if (currentlyTouchingObject == null)
            {
                usedItem_CameraShake.Clear();
                return;
            }
            else
            {
                sm.PlaySound("Punch", 1);

                if(currentlyTouchingObject.type == ModifiableObjectType.Weak)
                {
                    //modify object
                    currentlyTouchingObject.Interact();
                    Item i = im.hand;
                    cm.ShakeCamera(i.camShakeTime, i.camShakePower, i.camShakeFade, 0);
                    usedItem_CameraShake.Clear();
                    return;

                }
                else if(currentlyTouchingObject.type == ModifiableObjectType.NPC)
                {
                    currentlyTouchingObject.Interact();
                    usedItem_CameraShake.Clear();
                    return;
                }
                else if(usingItem.modifableObjectType == currentlyTouchingObject.type)
                {
                    //modify object
                    currentlyTouchingObject.Interact();
                    foreach (Item i in usedItem_CameraShake)
                    {
                        cm.ShakeCamera(i.camShakeTime, i.camShakePower, i.camShakeFade, 0);
                    }
                    usedItem_CameraShake.Clear();
                    return;
                }
                else
                {
                    usedItem_CameraShake.Clear();
                    return;
                }
            }

        }


        int chunkSize = wgPreset.chunkSize;

        ChunkScript cs = currentlyTouchingChunk;
        Vector3 curDestroyingPosition = touchingPosition - new Vector3(cs.position.x, 0, cs.position.y);


        int yStart = Mathf.Clamp(Mathf.RoundToInt(curDestroyingPosition.y - destroyMaxDistance), 0, cs.terrainMap.GetLength(1));
        int yEnd = Mathf.Clamp(Mathf.RoundToInt(curDestroyingPosition.y + destroyMaxDistance), 0, cs.terrainMap.GetLength(1));

        int gettingRockCount = 0;

        AddChunkToModifiedList(cs);

        int destroyedPointCount = 0;
        int maxDestroyPointCount = Mathf.RoundToInt((yEnd - yStart) * 4 * destroyMaxDistance * destroyMaxDistance);

        for (int y = yStart; y < yEnd; y++)
        {
            if(y >= usingItem.minDiggableYLevel && y <= usingItem.maxDiggableYLevel)
            {
                bool isRock = y <= mg.rockStartHeight;

                for (int x = Mathf.RoundToInt(curDestroyingPosition.x - destroyMaxDistance); x < Mathf.RoundToInt(curDestroyingPosition.x + destroyMaxDistance); x++)
                {
                    for (int z = Mathf.RoundToInt(curDestroyingPosition.z - destroyMaxDistance); z < Mathf.RoundToInt(curDestroyingPosition.z + destroyMaxDistance); z++)
                    {
                        float dis = Vector3.Distance(new Vector3(x, y, z), curDestroyingPosition);
                        if (dis <= destroyMaxDistance)
                        {
                            if (isRock)
                                gettingRockCount++;
                            destroyedPointCount++;

                            //calculate value
                            float power = destroySpeed * destroySpeedByDistanceCurve.Evaluate(dis / destroyMaxDistance);
                            if (x == chunkSize && z == chunkSize)//between four chunks
                            {
                                cs.terrainMap[x, y, z] += power;
                                cl.chunkDictionary[cs.position + new Vector2(chunkSize, 0)].cs.terrainMap[0, y, z] += power;
                                cl.chunkDictionary[cs.position + new Vector2(0, chunkSize)].cs.terrainMap[x, y, 0] += power;
                                cl.chunkDictionary[cs.position + new Vector2(chunkSize, chunkSize)].cs.terrainMap[0, y, 0] += power;


                                AddChunkToModifiedList(cl.chunkDictionary[cs.position + new Vector2(chunkSize, 0)].cs);
                                AddChunkToModifiedList(cl.chunkDictionary[cs.position + new Vector2(0, chunkSize)].cs);
                                AddChunkToModifiedList(cl.chunkDictionary[cs.position + new Vector2(chunkSize, chunkSize)].cs);
                            }
                            else if (x == chunkSize && z == 0)//between four chunks
                            {
                                cs.terrainMap[x, y, z] += power;
                                cl.chunkDictionary[cs.position + new Vector2(chunkSize, 0)].cs.terrainMap[0, y, z] += power;
                                cl.chunkDictionary[cs.position + new Vector2(0, -chunkSize)].cs.terrainMap[x, y, chunkSize] += power;
                                cl.chunkDictionary[cs.position + new Vector2(chunkSize, -chunkSize)].cs.terrainMap[0, y, chunkSize] += power;


                                AddChunkToModifiedList(cl.chunkDictionary[cs.position + new Vector2(chunkSize, 0)].cs);
                                AddChunkToModifiedList(cl.chunkDictionary[cs.position + new Vector2(0, -chunkSize)].cs);
                                AddChunkToModifiedList(cl.chunkDictionary[cs.position + new Vector2(chunkSize, -chunkSize)].cs);
                            }
                            else if (x == 0 && z == chunkSize)//between four chunks
                            {
                                cs.terrainMap[x, y, z] += power;
                                cl.chunkDictionary[cs.position + new Vector2(-chunkSize, 0)].cs.terrainMap[chunkSize, y, z] += power;
                                cl.chunkDictionary[cs.position + new Vector2(0, chunkSize)].cs.terrainMap[x, y, 0] += power;
                                cl.chunkDictionary[cs.position + new Vector2(-chunkSize, chunkSize)].cs.terrainMap[chunkSize, y, 0] += power;


                                AddChunkToModifiedList(cl.chunkDictionary[cs.position + new Vector2(-chunkSize, 0)].cs);
                                AddChunkToModifiedList(cl.chunkDictionary[cs.position + new Vector2(0, chunkSize)].cs);
                                AddChunkToModifiedList(cl.chunkDictionary[cs.position + new Vector2(-chunkSize, chunkSize)].cs);
                            }
                            else if (x == 0 && z == 0)//between four chunks
                            {
                                cs.terrainMap[x, y, z] += power;
                                cl.chunkDictionary[cs.position + new Vector2(-chunkSize, 0)].cs.terrainMap[chunkSize, y, z] += power;
                                cl.chunkDictionary[cs.position + new Vector2(0, -chunkSize)].cs.terrainMap[x, y, chunkSize] += power;
                                cl.chunkDictionary[cs.position + new Vector2(-chunkSize, -chunkSize)].cs.terrainMap[chunkSize, y, chunkSize] += power;


                                AddChunkToModifiedList(cl.chunkDictionary[cs.position + new Vector2(-chunkSize, 0)].cs);
                                AddChunkToModifiedList(cl.chunkDictionary[cs.position + new Vector2(0, -chunkSize)].cs);
                                AddChunkToModifiedList(cl.chunkDictionary[cs.position + new Vector2(-chunkSize, -chunkSize)].cs);
                            }
                            else//just in chunk
                            {
                                Vector3Int modifyingPositionInChunk = new Vector3Int(x, y, z);
                                ChunkScript modifyingChunk = cs;


                                bool crossedRight = x >= chunkSize;
                                bool crossedLeft = x <= 0;
                                bool crossedFront = z >= chunkSize;
                                bool crossedBack = z <= 0;

                                if (crossedLeft)
                                {
                                    modifyingChunk = cl.chunkDictionary[modifyingChunk.position + new Vector2(-chunkSize, 0)].cs;
                                    modifyingPositionInChunk += new Vector3Int(chunkSize, 0, 0);
                                }
                                else if (crossedRight)
                                {
                                    modifyingChunk = cl.chunkDictionary[modifyingChunk.position + new Vector2(chunkSize, 0)].cs;
                                    modifyingPositionInChunk += new Vector3Int(-chunkSize, 0, 0);
                                }

                                if (crossedBack)
                                {
                                    modifyingChunk = cl.chunkDictionary[modifyingChunk.position + new Vector2(0, -chunkSize)].cs;
                                    modifyingPositionInChunk += new Vector3Int(0, 0, chunkSize);
                                }
                                else if (crossedFront)
                                {
                                    modifyingChunk = cl.chunkDictionary[modifyingChunk.position + new Vector2(0, chunkSize)].cs;
                                    modifyingPositionInChunk += new Vector3Int(0, 0, -chunkSize);
                                }

                                if (!modifiedChunkList.Contains(modifyingChunk))
                                {
                                    modifiedChunkList.Add(modifyingChunk);
                                }

                                modifyingChunk.terrainMap[modifyingPositionInChunk.x, modifyingPositionInChunk.y, modifyingPositionInChunk.z] += power;

                                AddChunkToModifiedList(modifyingChunk);

                                //collides with another chunk
                                if (modifyingPositionInChunk.x == chunkSize)
                                {
                                    cl.chunkDictionary[modifyingChunk.position + new Vector2(chunkSize, 0)].cs.terrainMap[0, y, modifyingPositionInChunk.z] += power;

                                    AddChunkToModifiedList(cl.chunkDictionary[modifyingChunk.position + new Vector2(chunkSize, 0)].cs);
                                }
                                else if (modifyingPositionInChunk.x == 0)
                                {
                                    cl.chunkDictionary[modifyingChunk.position + new Vector2(-chunkSize, 0)].cs.terrainMap[chunkSize, y, modifyingPositionInChunk.z] += power;

                                    AddChunkToModifiedList(cl.chunkDictionary[modifyingChunk.position + new Vector2(-chunkSize, 0)].cs);
                                }
                                else if ((modifyingPositionInChunk.z == chunkSize))
                                {
                                    cl.chunkDictionary[modifyingChunk.position + new Vector2(0, chunkSize)].cs.terrainMap[modifyingPositionInChunk.x, y, 0] += power;

                                    AddChunkToModifiedList(cl.chunkDictionary[modifyingChunk.position + new Vector2(0, chunkSize)].cs);
                                }
                                else if (modifyingPositionInChunk.z == 0)
                                {
                                    cl.chunkDictionary[modifyingChunk.position + new Vector2(0, -chunkSize)].cs.terrainMap[modifyingPositionInChunk.x, y, chunkSize] += power;

                                    AddChunkToModifiedList(cl.chunkDictionary[modifyingChunk.position + new Vector2(0, -chunkSize)].cs);
                                }
                            }


                        }
                    }
                }
            }

        }



        //sound
        if (curDestroyingPosition.y <= 66)
        {
            string str = Random.Range(1, 4).ToString();
            for(int i = 0; i < 4; i++)
            {
                sm.PlaySound("StoneBreak" + str, 1);
            }
        }
        else
        {
            switch (cs.biomeProperty.name)
            {
                case "Plain":
                    sm.PlaySound("DestroyDirt" + Random.Range(1, 4).ToString(), ((float)destroyedPointCount / maxDestroyPointCount) * 1.5f + 0.5f);
                    break;
                case "SnowTundra":
                    sm.PlaySound("DestroySand" + Random.Range(1, 5).ToString(), ((float)destroyedPointCount / maxDestroyPointCount) * 0.7f + 0.2f);
                    break;
                case "Desert":
                    sm.PlaySound("DestroySand" + Random.Range(1, 5).ToString(), ((float)destroyedPointCount / maxDestroyPointCount) * 0.7f + 0.2f);
                    break;
                default:
                    break;
            }
        }




        //destroy grass
        bool destroyedGrass = false;
        for (int i = 0; i < cs.grassList.Count; i++)
        {
            GrassScript gs = cs.grassList[i];
            if (Vector3.Distance(gs.transform.position, touchingPosition) <= destroyMaxDistance + 0.5f)
            {
                gs.Scatter();
                cs.grassList.RemoveAt(i);
                i--;
                destroyedGrass = true;
            }
        }
        if (destroyedGrass)
        {
            sm.PlaySound("DestroyGrass" + Random.Range(1, 5).ToString(), 1);
        }



        //camshake
        foreach (Item i in usedItem_CameraShake)
        {
            cm.ShakeCamera(i.camShakeTime, i.camShakePower, i.camShakeFade, 0);
        }
        usedItem_CameraShake.Clear();

        //particle
        BiomeProperty bp = currentlyTouchingChunk.biomeProperty;
        if(touchingPosition.y >= mg.grassStartHeight)
        {
            if (bp.groundDestroyParticle != null)
            {
                GameObject p = Instantiate(bp.groundDestroyParticle);
                p.transform.position = touchingPosition;
            }
        }
        else
        {
            GameObject p = Instantiate(rockDestroyParticle);
            p.transform.position = touchingPosition;
        }


        //get rock
        if(gettingRockCount != 0 && im.HasRoomFor(rockItem))
        {
            gettingRockCount = Mathf.RoundToInt(gettingRockCount * rockObtainMultiplier);
            im.ObtainItem(new InventorySlot { item = rockItem, amount = gettingRockCount }, 0, 27);
        }

    }
    void Place()
    {
        if (currentlyTouchingChunk == null)
            return;

        int chunkSize = wgPreset.chunkSize;

        ChunkScript cs = currentlyTouchingChunk;
        Vector3 curPlacingPosition = touchingPosition - new Vector3(cs.position.x, 0, cs.position.y);


        int yStart = Mathf.Clamp(Mathf.RoundToInt(curPlacingPosition.y - destroyMaxDistance), 0, cs.terrainMap.GetLength(1));
        int yEnd = Mathf.Clamp(Mathf.RoundToInt(curPlacingPosition.y + destroyMaxDistance), 0, cs.terrainMap.GetLength(1));


        AddChunkToModifiedList(cs);

        for (int x = Mathf.RoundToInt(curPlacingPosition.x - placeMaxDistance); x < Mathf.RoundToInt(curPlacingPosition.x + placeMaxDistance); x++)
        {
            for (int y = yStart; y < yEnd; y++)
            {
                for (int z = Mathf.RoundToInt(curPlacingPosition.z - placeMaxDistance); z < Mathf.RoundToInt(curPlacingPosition.z + placeMaxDistance); z++)
                {
                    float dis = Vector3.Distance(new Vector3(x, y, z), curPlacingPosition);
                    if (dis <= placeMaxDistance)
                    {
                        //calculate value
                        float power = -placeSpeed * placeSpeedByDistanceCurve.Evaluate(dis / placeMaxDistance) * Time.deltaTime;

                        if (x == chunkSize && z == chunkSize)//between four chunks
                        {
                            cs.terrainMap[x, y, z] += power;
                            cl.chunkDictionary[cs.position + new Vector2(chunkSize, 0)].cs.terrainMap[0, y, z] += power;
                            cl.chunkDictionary[cs.position + new Vector2(0, chunkSize)].cs.terrainMap[x, y, 0] += power;
                            cl.chunkDictionary[cs.position + new Vector2(chunkSize, chunkSize)].cs.terrainMap[0, y, 0] += power;


                            AddChunkToModifiedList(cl.chunkDictionary[cs.position + new Vector2(chunkSize, 0)].cs);
                            AddChunkToModifiedList(cl.chunkDictionary[cs.position + new Vector2(0, chunkSize)].cs);
                            AddChunkToModifiedList(cl.chunkDictionary[cs.position + new Vector2(chunkSize, chunkSize)].cs);
                        }
                        else if (x == chunkSize && z == 0)//between four chunks
                        {
                            cs.terrainMap[x, y, z] += power;
                            cl.chunkDictionary[cs.position + new Vector2(chunkSize, 0)].cs.terrainMap[0, y, z] += power;
                            cl.chunkDictionary[cs.position + new Vector2(0, -chunkSize)].cs.terrainMap[x, y, chunkSize] += power;
                            cl.chunkDictionary[cs.position + new Vector2(chunkSize, -chunkSize)].cs.terrainMap[0, y, chunkSize] += power;


                            AddChunkToModifiedList(cl.chunkDictionary[cs.position + new Vector2(chunkSize, 0)].cs);
                            AddChunkToModifiedList(cl.chunkDictionary[cs.position + new Vector2(0, -chunkSize)].cs);
                            AddChunkToModifiedList(cl.chunkDictionary[cs.position + new Vector2(chunkSize, -chunkSize)].cs);
                        }
                        else if (x == 0 && z == chunkSize)//between four chunks
                        {
                            cs.terrainMap[x, y, z] += power;
                            cl.chunkDictionary[cs.position + new Vector2(-chunkSize, 0)].cs.terrainMap[chunkSize, y, z] += power;
                            cl.chunkDictionary[cs.position + new Vector2(0, chunkSize)].cs.terrainMap[x, y, 0] += power;
                            cl.chunkDictionary[cs.position + new Vector2(-chunkSize, chunkSize)].cs.terrainMap[chunkSize, y, 0] += power;


                            AddChunkToModifiedList(cl.chunkDictionary[cs.position + new Vector2(-chunkSize, 0)].cs);
                            AddChunkToModifiedList(cl.chunkDictionary[cs.position + new Vector2(0, chunkSize)].cs);
                            AddChunkToModifiedList(cl.chunkDictionary[cs.position + new Vector2(-chunkSize, chunkSize)].cs);
                        }
                        else if (x == 0 && z == 0)//between four chunks
                        {
                            cs.terrainMap[x, y, z] += power;
                            cl.chunkDictionary[cs.position + new Vector2(-chunkSize, 0)].cs.terrainMap[chunkSize, y, z] += power;
                            cl.chunkDictionary[cs.position + new Vector2(0, -chunkSize)].cs.terrainMap[x, y, chunkSize] += power;
                            cl.chunkDictionary[cs.position + new Vector2(-chunkSize, -chunkSize)].cs.terrainMap[chunkSize, y, chunkSize] += power;


                            AddChunkToModifiedList(cl.chunkDictionary[cs.position + new Vector2(-chunkSize, 0)].cs);
                            AddChunkToModifiedList(cl.chunkDictionary[cs.position + new Vector2(0, -chunkSize)].cs);
                            AddChunkToModifiedList(cl.chunkDictionary[cs.position + new Vector2(-chunkSize, -chunkSize)].cs);
                        }
                        else//just in chunk
                        {
                            Vector3Int modifyingPositionInChunk = new Vector3Int(x, y, z);
                            ChunkScript modifyingChunk = cs;


                            bool crossedRight = x >= chunkSize;
                            bool crossedLeft = x <= 0;
                            bool crossedFront = z >= chunkSize;
                            bool crossedBack = z <= 0;

                            if (crossedLeft)
                            {
                                modifyingChunk = cl.chunkDictionary[modifyingChunk.position + new Vector2(-chunkSize, 0)].cs;
                                modifyingPositionInChunk += new Vector3Int(chunkSize, 0, 0);
                            }
                            else if (crossedRight)
                            {
                                modifyingChunk = cl.chunkDictionary[modifyingChunk.position + new Vector2(chunkSize, 0)].cs;
                                modifyingPositionInChunk += new Vector3Int(-chunkSize, 0, 0);
                            }

                            if (crossedBack)
                            {
                                modifyingChunk = cl.chunkDictionary[modifyingChunk.position + new Vector2(0, -chunkSize)].cs;
                                modifyingPositionInChunk += new Vector3Int(0, 0, chunkSize);
                            }
                            else if (crossedFront)
                            {
                                modifyingChunk = cl.chunkDictionary[modifyingChunk.position + new Vector2(0, chunkSize)].cs;
                                modifyingPositionInChunk += new Vector3Int(0, 0, -chunkSize);
                            }

                            if (!modifiedChunkList.Contains(modifyingChunk))
                            {
                                modifiedChunkList.Add(modifyingChunk);
                            }

                            modifyingChunk.terrainMap[modifyingPositionInChunk.x, modifyingPositionInChunk.y, modifyingPositionInChunk.z] += power;

                            AddChunkToModifiedList(modifyingChunk);

                            //collides with another chunk
                            if (modifyingPositionInChunk.x == chunkSize)
                            {
                                cl.chunkDictionary[modifyingChunk.position + new Vector2(chunkSize, 0)].cs.terrainMap[0, y, modifyingPositionInChunk.z] += power;

                                AddChunkToModifiedList(cl.chunkDictionary[modifyingChunk.position + new Vector2(chunkSize, 0)].cs);
                            }
                            else if (modifyingPositionInChunk.x == 0)
                            {
                                cl.chunkDictionary[modifyingChunk.position + new Vector2(-chunkSize, 0)].cs.terrainMap[chunkSize, y, modifyingPositionInChunk.z] += power;

                                AddChunkToModifiedList(cl.chunkDictionary[modifyingChunk.position + new Vector2(-chunkSize, 0)].cs);
                            }
                            else if ((modifyingPositionInChunk.z == chunkSize))
                            {
                                cl.chunkDictionary[modifyingChunk.position + new Vector2(0, chunkSize)].cs.terrainMap[modifyingPositionInChunk.x, y, 0] += power;

                                AddChunkToModifiedList(cl.chunkDictionary[modifyingChunk.position + new Vector2(0, chunkSize)].cs);
                            }
                            else if (modifyingPositionInChunk.z == 0)
                            {
                                cl.chunkDictionary[modifyingChunk.position + new Vector2(0, -chunkSize)].cs.terrainMap[modifyingPositionInChunk.x, y, chunkSize] += power;

                                AddChunkToModifiedList(cl.chunkDictionary[modifyingChunk.position + new Vector2(0, -chunkSize)].cs);
                            }
                        }
                    }
                }
            }
        }
    }
    void AddChunkToModifiedList(ChunkScript cs)
    {
        if (!modifiedChunkList.Contains(cs))
            modifiedChunkList.Add(cs);
    }

    void LateUpdate()
    {
        UpdateModifiedChunks();
    }
    void UpdateModifiedChunks()
    {
        foreach (ChunkScript cs in modifiedChunkList)
        {
            cs.ReGenerateMesh();
        }
        modifiedChunkList.Clear();
    }
}
