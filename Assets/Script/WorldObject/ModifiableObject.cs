using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifiableObject : MonoBehaviour
{
    public ModifiableObjectType type;
    public MonoBehaviour InteractingScript;
    public bool placing_stackable;

    public void Interact()
    {
        InteractingScript.Invoke("Interact", 0);
    }
}
public enum ModifiableObjectType
{
    None,
    Weak,
    Tree,
    Ore,
    NPC
}
