using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveWorldData(UniversalScriptManager usm)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + usm.saveManager.currentWorldName+".gejam";
        FileStream stream = new FileStream(path, FileMode.Create);


        WorldData data = new WorldData(usm);

        formatter.Serialize(stream, data);
        stream.Close();

    }

    public static WorldData LoadWorldData(string worldName)
    {

        string path = Application.persistentDataPath + "/" + worldName + ".gejam";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            WorldData data = formatter.Deserialize(stream) as WorldData;
            stream.Close();
            return data;

        }
        else
        {
            Debug.LogWarning("Save File Not found, path:" + path);
            return null;
        }
    }
}
