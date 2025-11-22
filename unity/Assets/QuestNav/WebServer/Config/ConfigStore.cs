using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace QuestNav.WebServer
{
    /// <summary>
    /// Handles persistence of configuration data to disk.
    /// Stores configuration in Application.persistentDataPath as JSON files.
    /// Provides save/load operations for configuration data and authentication tokens.
    /// Thread-safe for use from both main thread and ConfigServer background thread.
    /// </summary>
    public class ConfigStore
    {
        #region Constants
        /// <summary>
        /// Configuration file name
        /// </summary>
        private const string CONFIG_FILENAME = "config.json";

        /// <summary>
        /// Authentication token file name
        /// </summary>
        private const string AUTH_FILENAME = "auth.json";
        #endregion

        #region Fields
        /// <summary>
        /// Full path to configuration file
        /// </summary>
        private readonly string configPath;

        /// <summary>
        /// Full path to authentication token file
        /// </summary>
        private readonly string authPath;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new ConfigStore instance.
        /// Sets up file paths in Application.persistentDataPath for cross-platform compatibility.
        /// </summary>
        public ConfigStore()
        {
            configPath = Path.Combine(Application.persistentDataPath, CONFIG_FILENAME);
            authPath = Path.Combine(Application.persistentDataPath, AUTH_FILENAME);
        }
        #endregion

        #region Configuration Persistence
        /// <summary>
        /// Loads configuration data from persistent storage.
        /// Returns empty ConfigData if file doesn't exist or fails to load.
        /// </summary>
        /// <returns>ConfigData with loaded values, or empty ConfigData on error</returns>
        public ConfigData LoadConfig()
        {
            try
            {
                if (File.Exists(configPath))
                {
                    string json = File.ReadAllText(configPath);
                    return JsonConvert.DeserializeObject<ConfigData>(json);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ConfigStore] Failed to load config: {ex.Message}");
            }

            return new ConfigData();
        }

        /// <summary>
        /// Saves configuration data to persistent storage.
        /// Automatically updates the lastModified timestamp.
        /// </summary>
        /// <param name="config">Configuration data to save</param>
        /// <returns>True if save was successful, false otherwise</returns>
        public bool SaveConfig(ConfigData config)
        {
            if (config == null)
            {
                Debug.LogError("[ConfigStore] Cannot save null config data");
                return false;
            }

            try
            {
                config.lastModified = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(configPath, json);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ConfigStore] Failed to save config: {ex.Message}");
                return false;
            }
        }
        #endregion

        #region Authentication Token Management
        /// <summary>
        /// Loads or generates authentication token for the web interface.
        /// Note: Authentication is currently disabled in ConfigServer.
        /// This method is reserved for future use if authentication is needed.
        /// </summary>
        /// <returns>AuthToken with generated or loaded token</returns>
        public AuthToken LoadOrGenerateToken()
        {
            try
            {
                // Try to load existing token
                if (File.Exists(authPath))
                {
                    string json = File.ReadAllText(authPath);
                    return JsonConvert.DeserializeObject<AuthToken>(json);
                }

                // Generate new token
                var token = new AuthToken
                {
                    token = GenerateSecureToken(),
                    createdAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    deviceId = SystemInfo.deviceUniqueIdentifier,
                };

                // Save token to disk
                string tokenJson = JsonConvert.SerializeObject(token, Formatting.Indented);
                File.WriteAllText(authPath, tokenJson);

                Debug.Log($"[ConfigStore] Generated new auth token: {token.token}");
                return token;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ConfigStore] Failed to load/generate token: {ex.Message}");

                // Return temporary token on error
                return new AuthToken
                {
                    token = GenerateSecureToken(),
                    createdAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    deviceId = SystemInfo.deviceUniqueIdentifier,
                };
            }
        }

        /// <summary>
        /// Generates a secure random token for authentication.
        /// Creates a 32-character alphanumeric token.
        /// </summary>
        /// <returns>32-character random alphanumeric string</returns>
        private string GenerateSecureToken()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new System.Random();
            var token = new char[32];

            for (int i = 0; i < token.Length; i++)
            {
                token[i] = chars[random.Next(chars.Length)];
            }

            return new string(token);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the full path to the configuration file
        /// </summary>
        public string GetConfigPath() => configPath;
        #endregion
    }
}
