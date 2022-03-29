using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionDestructableObject : MonoBehaviour
{
    UniversalScriptManager usm;
    ExplosionManager em;
    void Awake()
    {
        usm = FindObjectOfType<UniversalScriptManager>();
        em = usm.explosionManager;
    }

    void FixedUpdate()
    {
        if (!em.hasExplosion)
            return;
        if(Vector3.Distance(transform.position, em.explosionPos) <= em.explosionRadius)
        {
            Destroy(gameObject);
        }
    }
}
