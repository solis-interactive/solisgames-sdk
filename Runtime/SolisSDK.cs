using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

namespace SolisGames
{
    /// <summary>
    /// Core Solis Games SDK class
    /// Provides access to all SDK modules and handles initialization
    /// </summary>
    public class SolisSDK : MonoBehaviour
    {
        private static SolisSDK _instance;
        private static TaskCompletionSource<bool> _initTcs;
        private static bool _isInitialized = false;

        // Module instances (lazy-loaded)
        private static SolisAds _ads;
        private static SolisAnalytics _analytics;
        private static SolisCloudSave _cloudSave;
        private static SolisLeaderboards _leaderboards;
        private static SolisTournaments _tournaments;
        private static SolisAchievements _achievements;
        private static SolisFriends _friends;
        private static SolisChat _chat;
        private static SolisUser _user;

        #region JavaScript Imports

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void SolisSDK_Init(string apiKey, string gameId, Action<int> callback);
#endif

        #endregion

        #region Singleton Pattern

        /// <summary>
        /// Gets the singleton instance of SolisSDK
        /// </summary>
        public static SolisSDK Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("SolisSDK");
                    _instance = go.AddComponent<SolisSDK>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initialize the Solis Games SDK
        /// Must be called before using any SDK features
        /// </summary>
        /// <param name="apiKey">Your API key from Solis Games Studio Dashboard</param>
        /// <param name="gameId">Optional game ID (auto-detected if not provided)</param>
        /// <returns>True if initialization succeeded, false otherwise</returns>
        public static async Task<bool> InitAsync(string apiKey, string gameId = null)
        {
            if (_isInitialized)
            {
                Debug.LogWarning("[SolisSDK] Already initialized");
                return true;
            }

            if (string.IsNullOrEmpty(apiKey))
            {
                Debug.LogError("[SolisSDK] API key is required");
                return false;
            }

#if UNITY_WEBGL && !UNITY_EDITOR
            _initTcs = new TaskCompletionSource<bool>();

            try
            {
                SolisSDK_Init(apiKey, gameId ?? "", OnInitComplete);
                bool success = await _initTcs.Task;

                if (success)
                {
                    _isInitialized = true;
                    Debug.Log("[SolisSDK] Initialized successfully");
                }
                else
                {
                    Debug.LogError("[SolisSDK] Initialization failed");
                }

                return success;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK] Initialization error: {ex.Message}");
                return false;
            }
#else
            // Editor mode - simulate initialization
            Debug.Log("[SolisSDK] Running in Editor mode - using mock data");
            await Task.Delay(500); // Simulate network delay
            _isInitialized = true;
            return true;
#endif
        }

        [AOT.MonoPInvokeCallback(typeof(Action<int>))]
        private static void OnInitComplete(int success)
        {
            _initTcs?.SetResult(success == 1);
        }

        /// <summary>
        /// Check if SDK is initialized
        /// </summary>
        public static bool IsInitialized => _isInitialized;

        #endregion

        #region Module Access

        /// <summary>
        /// Access to Ads module (rewarded, interstitial, banner)
        /// </summary>
        public static SolisAds Ads
        {
            get
            {
                if (_ads == null)
                {
                    _ads = Instance.gameObject.GetComponent<SolisAds>();
                    if (_ads == null)
                    {
                        _ads = Instance.gameObject.AddComponent<SolisAds>();
                    }
                }
                return _ads;
            }
        }

        /// <summary>
        /// Access to Analytics module (event tracking, sessions)
        /// </summary>
        public static SolisAnalytics Analytics
        {
            get
            {
                if (_analytics == null)
                {
                    _analytics = Instance.gameObject.GetComponent<SolisAnalytics>();
                    if (_analytics == null)
                    {
                        _analytics = Instance.gameObject.AddComponent<SolisAnalytics>();
                    }
                }
                return _analytics;
            }
        }

        /// <summary>
        /// Access to CloudSave module (save/load game data)
        /// </summary>
        public static SolisCloudSave CloudSave
        {
            get
            {
                if (_cloudSave == null)
                {
                    _cloudSave = Instance.gameObject.GetComponent<SolisCloudSave>();
                    if (_cloudSave == null)
                    {
                        _cloudSave = Instance.gameObject.AddComponent<SolisCloudSave>();
                    }
                }
                return _cloudSave;
            }
        }

        /// <summary>
        /// Access to Leaderboards module (rankings, scores)
        /// </summary>
        public static SolisLeaderboards Leaderboards
        {
            get
            {
                if (_leaderboards == null)
                {
                    _leaderboards = Instance.gameObject.GetComponent<SolisLeaderboards>();
                    if (_leaderboards == null)
                    {
                        _leaderboards = Instance.gameObject.AddComponent<SolisLeaderboards>();
                    }
                }
                return _leaderboards;
            }
        }

        /// <summary>
        /// Access to Tournaments module (join, brackets)
        /// </summary>
        public static SolisTournaments Tournaments
        {
            get
            {
                if (_tournaments == null)
                {
                    _tournaments = Instance.gameObject.GetComponent<SolisTournaments>();
                    if (_tournaments == null)
                    {
                        _tournaments = Instance.gameObject.AddComponent<SolisTournaments>();
                    }
                }
                return _tournaments;
            }
        }

        /// <summary>
        /// Access to Achievements module (unlock, progress)
        /// </summary>
        public static SolisAchievements Achievements
        {
            get
            {
                if (_achievements == null)
                {
                    _achievements = Instance.gameObject.GetComponent<SolisAchievements>();
                    if (_achievements == null)
                    {
                        _achievements = Instance.gameObject.AddComponent<SolisAchievements>();
                    }
                }
                return _achievements;
            }
        }

        /// <summary>
        /// Access to Friends module (social, presence)
        /// </summary>
        public static SolisFriends Friends
        {
            get
            {
                if (_friends == null)
                {
                    _friends = Instance.gameObject.GetComponent<SolisFriends>();
                    if (_friends == null)
                    {
                        _friends = Instance.gameObject.AddComponent<SolisFriends>();
                    }
                }
                return _friends;
            }
        }

        /// <summary>
        /// Access to Chat module (messaging)
        /// </summary>
        public static SolisChat Chat
        {
            get
            {
                if (_chat == null)
                {
                    _chat = Instance.gameObject.GetComponent<SolisChat>();
                    if (_chat == null)
                    {
                        _chat = Instance.gameObject.AddComponent<SolisChat>();
                    }
                }
                return _chat;
            }
        }

        /// <summary>
        /// Access to User module (profile, authentication)
        /// </summary>
        public static SolisUser User
        {
            get
            {
                if (_user == null)
                {
                    _user = Instance.gameObject.GetComponent<SolisUser>();
                    if (_user == null)
                    {
                        _user = Instance.gameObject.AddComponent<SolisUser>();
                    }
                }
                return _user;
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Log an error if SDK is not initialized
        /// </summary>
        internal static bool CheckInitialized(string moduleName)
        {
            if (!_isInitialized)
            {
                Debug.LogError($"[SolisSDK.{moduleName}] SDK not initialized. Call SolisSDK.InitAsync() first.");
                return false;
            }
            return true;
        }

        #endregion
    }
}
