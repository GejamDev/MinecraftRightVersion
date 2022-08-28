using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Custom_R_FlintAndSteel : MonoBehaviour
{
    public UniversalScriptManager usm;
    public Item item;
    public Animator handAnim;
    Transform cam;
    FireManager fm;
    NetherPortalGenerationManager npgm;
    public float maxDistance;
    public LayerMask groundLayer;

    private void Awake()
    {
        cam = Camera.main.transform;
        fm = usm.fireManager;
        npgm = usm.netherPortalGenerationManager;

    }
    public void Use()
    {
        handAnim.SetTrigger(item.usingAnimationName);


        RaycastHit hit;
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxDistance, groundLayer))
        {
            //iit fire
            ObsidianBlock ob;
            if(hit.collider.gameObject.TryGetComponent<ObsidianBlock>(out ob))
            {
                Debug.Log("Try Making Nether Portal");
                npgm.TryMakeNetherPortal(ob, out bool s);
            }
            else
            {
                ChunkScript chunk = hit.collider.transform.parent.parent.GetComponent<ChunkScript>();

                fm.LitFire(hit.point, chunk);
            }
        }
    }
}
