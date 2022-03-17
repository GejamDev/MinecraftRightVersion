using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassInteractor : MonoBehaviour
{
    void Update()
    {
        Shader.SetGlobalVector("_PositionMoving", transform.position);
    }
}
