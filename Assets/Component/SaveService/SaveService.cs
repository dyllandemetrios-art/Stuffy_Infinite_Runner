using System;
using System.IO;
using UnityEngine;

namespace Components.SaveService
{
    /// <summary>Static service handling JSON serialization and deserialization of SaveData to disk.</summary>
    public static class SaveService
    {
        private const string FILE_NAME = "InfiniteDiscountSave.json"; // Save file name stored in persistentDataPath.
        private static string FilePath => Path.Combine(Application.persistentDataPath, FILE_NAME); // Full path to the save file.

        /// <summary>Serializes SaveData to JSON and writes it to disk.</summary>
        public static void Save(SaveData data)
        {
            string json = JsonUtility.ToJson(data);
            File.WriteAllText(FilePath, json);

            Debug.Log("Data successfully saved at: " + FilePath);
        }
        
        /// <summary>Reads and deserializes SaveData from disk, returns a fresh instance if no file exists.</summary>
        public static SaveData Load()
        {
            try
            {
                string json = File.ReadAllText(FilePath);
                return JsonUtility.FromJson<SaveData>(json);
            }
            catch (Exception exception)
            {
                Debug.LogWarning("No data found, creating a new one... Details: " + exception);
                return new SaveData();
            }
        }
    }
}