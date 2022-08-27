using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockConsideredGround : MonoBehaviour
{
    private void OnDestroy()
    {
        ChunkScript cs =transform.parent.parent.GetComponent<ChunkScript>();
        cs.blockPositionData.Remove(new Vector3Int((int)transform.localPosition.x, (int)transform.localPosition.y, (int)transform.localPosition.z));
        cs.ReGenerateLiquidMesh();
    }
}
