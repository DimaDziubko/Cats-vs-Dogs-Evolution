using System;
using System.IO;
using UnityEngine;

namespace _Game.Utils._LocalConfigSaver
{
    public static class LocalConfigSaver
    {
        private static readonly string ConfigFolderPath = Path.Combine(Application.persistentDataPath, "Config");
        private static readonly string ConfigFilePath = Path.Combine(ConfigFolderPath, "gameConfig.json");

        public static void SaveConfig(string config)
        {
            try
            {
                if (!Directory.Exists(ConfigFolderPath))
                {
                    Directory.CreateDirectory(ConfigFolderPath);
                }

                File.WriteAllText(ConfigFilePath, config);
                Debug.Log("Configuration saved locally");
            }
            catch (Exception e)
            {
                Debug.LogError($"Error saving configuration: {e.Message}");
            }
        }

        public static string GetConfig()
        {
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    string configString = File.ReadAllText(ConfigFilePath);
                    return configString;
                }
                else
                {
                    Debug.LogWarning("Local configuration file not found");
                    return null;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error loading local configuration: {e.Message}");
                return null;
            }
        }
    }
}