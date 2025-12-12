using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class Json : MonoBehaviour
{
    
    [System.Serializable]
    public class SaveDataContainer
    {
        public MapArrayScript.World World;
    }

    public void SaveGameData(MapArrayScript.World world, string fileName)
    {
        try
        {
            SaveDataContainer container = new SaveDataContainer { World = world };
            var settings = new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore };

            // Use persistentDataPath for runtime data saving
            string savePath = Path.Combine(Application.persistentDataPath, fileName + ".json");

            // Ensure the directory exists
            Directory.CreateDirectory(Path.GetDirectoryName(savePath));

            string jsonData = JsonConvert.SerializeObject(container, settings);
            File.WriteAllText(savePath, jsonData);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error saving game data: " + ex.Message);
        }
    }

    public MapArrayScript.World LoadGameData(string fileName)
    {
        try
        {
            // Use persistentDataPath for runtime data loading
            string loadPath = Path.Combine(Application.persistentDataPath, fileName + ".json");

            if (!File.Exists(loadPath))
            {
                Debug.LogError("File not found: " + loadPath);
                return null;
            }

            string jsonData = File.ReadAllText(loadPath);
            SaveDataContainer container = JsonConvert.DeserializeObject<SaveDataContainer>(jsonData);

            return container.World;
        }
        catch (Exception ex)
        {
            Debug.LogError("Error loading game data: " + ex.Message);
            return null;
        }
    }
   

    
}