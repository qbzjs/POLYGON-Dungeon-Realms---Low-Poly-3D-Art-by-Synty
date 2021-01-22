using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem_Script
{
    public static void SavePlayer(William_Script william)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.fun";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData_Script data = new PlayerData_Script(william);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData_Script LoadPlayer()
    {
        string path = Application.persistentDataPath + "/player.fun";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData_Script data = formatter.Deserialize(stream) as PlayerData_Script;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Sauvegarde introuvable à " + path);
            return null;
        }
    } 
}
