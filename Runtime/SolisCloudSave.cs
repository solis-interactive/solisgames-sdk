using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

namespace SolisGames
{
    /// <summary>
    /// Cloud Save module for cross-device progress storage
    /// Supports saving and loading any serializable data type
    /// </summary>
    public class SolisCloudSave : MonoBehaviour
    {
        private TaskCompletionSource<bool> _saveTcs;
        private TaskCompletionSource<string> _loadTcs;

        #region JavaScript Imports

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void SolisSDK_CloudSave_Save(string key, string dataJson, Action<int> callback);

        [DllImport("__Internal")]
        private static extern void SolisSDK_CloudSave_Load(string key, string gameObjectName, string callbackMethod);
#endif

        #endregion

        #region Public API

        /// <summary>
        /// Save data to cloud
        /// Data is automatically serialized to JSON
        /// </summary>
        /// <typeparam name="T">Data type (must be serializable)</typeparam>
        /// <param name="key">Save key identifier</param>
        /// <param name="data">Data to save</param>
        /// <returns>True if save was successful</returns>
        public async Task<bool> SaveAsync<T>(string key, T data)
        {
            if (!SolisSDK.CheckInitialized("CloudSave"))
                return false;

            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("[SolisSDK.CloudSave] Save key is required");
                return false;
            }

            if (data == null)
            {
                Debug.LogError("[SolisSDK.CloudSave] Data cannot be null");
                return false;
            }

#if UNITY_WEBGL && !UNITY_EDITOR
            _saveTcs = new TaskCompletionSource<bool>();

            try
            {
                string dataJson = JsonUtility.ToJson(data);
                SolisSDK_CloudSave_Save(key, dataJson, OnSaveComplete);
                return await _saveTcs.Task;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.CloudSave] Save error: {ex.Message}");
                return false;
            }
#else
            // Editor mode - simulate save
            await Task.Delay(500);
            Debug.Log($"[SolisSDK.CloudSave] [Demo] Saved '{key}': {JsonUtility.ToJson(data)}");
            return true;
#endif
        }

        /// <summary>
        /// Load data from cloud
        /// Data is automatically deserialized from JSON
        /// </summary>
        /// <typeparam name="T">Data type (must be serializable)</typeparam>
        /// <param name="key">Save key identifier</param>
        /// <returns>Loaded data, or default(T) if not found or error occurred</returns>
        public async Task<T> LoadAsync<T>(string key)
        {
            if (!SolisSDK.CheckInitialized("CloudSave"))
                return default(T);

            if (string.IsNullOrEmpty(key))
            {
                Debug.LogError("[SolisSDK.CloudSave] Save key is required");
                return default(T);
            }

#if UNITY_WEBGL && !UNITY_EDITOR
            _loadTcs = new TaskCompletionSource<string>();

            try
            {
                SolisSDK_CloudSave_Load(key, gameObject.name, nameof(OnLoadDataReceived));
                string dataJson = await _loadTcs.Task;

                if (string.IsNullOrEmpty(dataJson) || dataJson == "null")
                {
                    Debug.LogWarning($"[SolisSDK.CloudSave] No data found for key '{key}'");
                    return default(T);
                }

                T data = JsonUtility.FromJson<T>(dataJson);
                return data;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.CloudSave] Load error: {ex.Message}");
                return default(T);
            }
#else
            // Editor mode - return mock data
            await Task.Delay(500);
            Debug.Log($"[SolisSDK.CloudSave] [Demo] Loading '{key}' (returning default mock data)");

            // Try to return a reasonable default for demo
            if (typeof(T).IsValueType)
            {
                return default(T);
            }
            else
            {
                try
                {
                    return Activator.CreateInstance<T>();
                }
                catch
                {
                    return default(T);
                }
            }
#endif
        }

        #endregion

        #region Callbacks

        [AOT.MonoPInvokeCallback(typeof(Action<int>))]
        private void OnSaveComplete(int success)
        {
            _saveTcs?.SetResult(success == 1);
        }

        // Called by JavaScript via SendMessage
        private void OnLoadDataReceived(string dataJson)
        {
            _loadTcs?.SetResult(dataJson);
        }

        #endregion
    }

    #region Example Save Data Classes

    /// <summary>
    /// Example save data structure
    /// Create your own custom save data classes like this
    /// </summary>
    [Serializable]
    public class PlayerSaveData
    {
        public int level = 1;
        public int coins = 0;
        public int experience = 0;
        public string[] unlockedItems = new string[0];
        public bool tutorialCompleted = false;

        public PlayerSaveData()
        {
            // Default constructor for deserialization
        }
    }

    #endregion
}
