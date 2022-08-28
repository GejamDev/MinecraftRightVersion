using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockConsideredGround : MonoBehaviour
{
    private void OnDestroy()
    {
        if (transform.parent == null)
            return;
        ChunkScript cs =transform.parent.parent.GetComponent<ChunkScript>();
        if (cs == null)
            return;
        cs.blockPositionData.Remove(new Vector3Int((int)transform.localPosition.x, (int)transform.localPosition.y, (int)transform.localPosition.z));
        cs.ReGenerateLiquidMesh();
    }
}
