using UnityEngine;

public class UniversalScriptManager : MonoBehaviour
{
    //just manager for containing other objects



    [Header("Player")]
    public GameObject player;
    public FirstPersonController firstPersonController;
    public Camera cam;

    [Header("Scripts")]
    public WorldGenerationPreset worldGenerationPreset;
    public WorldGenerator worldGenerator;
    public MeshGenerator meshGenerator;
    public ChunkLoader chunkLoader;
    public TerrainModifier terrainModifier;
    public BiomeManager biomeManager;
    public WaterManager waterManager;
    public LoadingManager loadingManager;
    public InventoryManager inventoryManager;
    public CameraManager cameraManager;
    public ItemSpawner itemSpawner;
    public UIManager uiManager;
    public HpManager hpManager;
    public LightingManager lightingManager;
    public HungerManager hungerManager;
    public EntitySpawner entitySpawner;
    public ObjectPool objectPool;
    public PauseManager pauseManager;
    public SoundManager soundManager;
    public WeatherManager weatherManager;
    public ExplosionManager explosionManager;
    public LavaManager lavaManager;
    public BlockPlacementManager blockPlacementManager;
    public FireManager fireManager;
    public NetherPortalGenerationManager netherPortalGenerationManager;
    public DimensionTransportationManager dimensionTransportationManager;
    public WorldDataRecorder worldDataRecorder;
    public SeedManager seedManager;
    public SaveManager saveManager;

    [Header("Layer")]
    public LayerMask groundLayer;
    public LayerMask objectLayer;

    [Header("Item")]
    public Item arrow;
}
