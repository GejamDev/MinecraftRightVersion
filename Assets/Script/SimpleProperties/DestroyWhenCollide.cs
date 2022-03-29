using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWhenCollide : MonoBehaviour
{
    public bool isTrigger;
    public string interactionTag;

    void OnTriggerEnter(Collider other)
    {
        if (!isTrigger)
            return;
        if (other.gameObject.CompareTag(interactionTag))
        {
        }

    }
    void OnCollisionEnter(Collision collision)
    {
        if (isTrigger)
            return;
        if (collision.gameObject.CompareTag(interactionTag))
        {
            Interact();
        }
    }
    public void Interact()
    {
        Destroy(gameObject);
    }


}
