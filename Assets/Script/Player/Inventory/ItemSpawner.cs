using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public ItemScript prefab;

    public void SpawnItem(InventorySlot slot, Vector3 position, Quaternion rotation)
    {
        GameObject i = Instantiate(prefab.gameObject);
        i.transform.position = position;
        i.transform.rotation = rotation;
        i.GetComponent<ItemScript>().SetItem(slot);
    }
    public GameObject GetSpawnItem(InventorySlot slot, Vector3 position, Quaternion rotation)
    {
        GameObject i = Instantiate(prefab.gameObject);
        i.transform.position = position;
        i.transform.rotation = rotation;
        i.GetComponent<ItemScript>().SetItem(slot);

        return i;
    }
}
