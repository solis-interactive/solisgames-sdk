using UnityEditor;
using UnityEngine;

namespace SolisGames.Editor
{
    /// <summary>
    /// SDK Settings window for configuring Solis Games SDK
    /// Accessible via Window > Solis Games > SDK Settings
    /// </summary>
    public class SolisSDKWindow : EditorWindow
    {
        private const string PREF_API_KEY = "SolisGames_ApiKey";
        private const string PREF_GAME_ID = "SolisGames_GameId";
        private const string PREF_ENABLE_ADS = "SolisGames_EnableAds";
        private const string PREF_ENABLE_ANALYTICS = "SolisGames_EnableAnalytics";
        private const string PREF_ENABLE_CLOUD_SAVE = "SolisGames_EnableCloudSave";
        private const string PREF_ENABLE_LEADERBOARDS = "SolisGames_EnableLeaderboards";
        private const string PREF_ENABLE_TOURNAMENTS = "SolisGames_EnableTournaments";
        private const string PREF_ENABLE_ACHIEVEMENTS = "SolisGames_EnableAchievements";
        private const string PREF_ENABLE_FRIENDS = "SolisGames_EnableFriends";
        private const string PREF_ENABLE_CHAT = "SolisGames_EnableChat";

        private string apiKey = "";
        private string gameId = "";
        private bool enableAds = true;
        private bool enableAnalytics = true;
        private bool enableCloudSave = true;
        private bool enableLeaderboards = true;
        private bool enableTournaments = true;
        private bool enableAchievements = true;
        private bool enableFriends = true;
        private bool enableChat = true;

        private Vector2 scrollPosition;
        private GUIStyle headerStyle;
        private GUIStyle sectionStyle;
        private bool stylesInitialized = false;

        [MenuItem("Window/Solis Games/SDK Settings")]
        public static void ShowWindow()
        {
            var window = GetWindow<SolisSDKWindow>("Solis Games SDK");
            window.minSize = new Vector2(450, 600);
            window.Show();
        }

        private void OnEnable()
        {
            LoadSettings();
        }

        private void InitializeStyles()
        {
            if (stylesInitialized) return;

            headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14,
                margin = new RectOffset(0, 0, 10, 10)
            };

            sectionStyle = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(10, 10, 10, 10),
                margin = new RectOffset(0, 0, 5, 5)
            };

            stylesInitialized = true;
        }

        private void OnGUI()
        {
            InitializeStyles();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // Header
            GUILayout.Space(10);
            GUILayout.Label("Solis Games SDK Configuration", headerStyle);
            EditorGUILayout.HelpBox(
                "Configure your Solis Games SDK settings. Get your API key from the Studio Dashboard at solisgames.com/studio",
                MessageType.Info
            );

            GUILayout.Space(10);

            // API Configuration Section
            EditorGUILayout.BeginVertical(sectionStyle);
            GUILayout.Label("API Configuration", EditorStyles.boldLabel);
            GUILayout.Space(5);

            EditorGUILayout.LabelField("API Key", EditorStyles.miniBoldLabel);
            apiKey = EditorGUILayout.TextField(apiKey);

            if (string.IsNullOrEmpty(apiKey))
            {
                EditorGUILayout.HelpBox("API key is required. Get yours from the Studio Dashboard.", MessageType.Warning);
            }

            GUILayout.Space(5);

            EditorGUILayout.LabelField("Game ID (Optional)", EditorStyles.miniBoldLabel);
            gameId = EditorGUILayout.TextField(gameId);
            EditorGUILayout.LabelField("Leave empty to auto-detect", EditorStyles.miniLabel);

            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            // Features Section
            EditorGUILayout.BeginVertical(sectionStyle);
            GUILayout.Label("Enabled Features", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Enable/disable SDK features. Disabled features will not be included in your WebGL build.",
                MessageType.Info
            );
            GUILayout.Space(5);

            // Monetization
            EditorGUILayout.LabelField("💰 Monetization", EditorStyles.boldLabel);
            enableAds = EditorGUILayout.Toggle("Ads (Rewarded, Interstitial, Banner)", enableAds);

            GUILayout.Space(5);

            // Core Features
            EditorGUILayout.LabelField("🎮 Core Features", EditorStyles.boldLabel);
            enableAnalytics = EditorGUILayout.Toggle("Analytics & Event Tracking", enableAnalytics);
            enableCloudSave = EditorGUILayout.Toggle("Cloud Saves (Cross-device)", enableCloudSave);

            GUILayout.Space(5);

            // Competitive Gaming
            EditorGUILayout.LabelField("🏆 Competitive Gaming", EditorStyles.boldLabel);
            enableLeaderboards = EditorGUILayout.Toggle("Leaderboards & Rankings", enableLeaderboards);
            enableTournaments = EditorGUILayout.Toggle("Tournaments & Brackets", enableTournaments);
            enableAchievements = EditorGUILayout.Toggle("Achievements & XP", enableAchievements);

            GUILayout.Space(5);

            // Social Features
            EditorGUILayout.LabelField("👥 Social Features", EditorStyles.boldLabel);
            enableFriends = EditorGUILayout.Toggle("Friends & Presence", enableFriends);
            enableChat = EditorGUILayout.Toggle("In-Game Chat", enableChat);

            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            // Action Buttons
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Save Settings", GUILayout.Height(30)))
            {
                SaveSettings();
            }

            if (GUILayout.Button("Reset to Defaults", GUILayout.Height(30)))
            {
                ResetToDefaults();
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            // Quick Links Section
            EditorGUILayout.BeginVertical(sectionStyle);
            GUILayout.Label("Quick Links", EditorStyles.boldLabel);
            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("📊 Studio Dashboard", GUILayout.Height(25)))
            {
                Application.OpenURL("https://solisgames.com/studio");
            }

            if (GUILayout.Button("📚 Documentation", GUILayout.Height(25)))
            {
                Application.OpenURL("https://solisgames.com/docs/unity");
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("🚀 Build & Deploy", GUILayout.Height(25)))
            {
                SolisBuildWindow.ShowWindow();
            }

            if (GUILayout.Button("❓ Get Help", GUILayout.Height(25)))
            {
                Application.OpenURL("https://github.com/solis-interactive/unity-sdk/issues");
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            // SDK Info
            EditorGUILayout.BeginVertical(sectionStyle);
            GUILayout.Label("SDK Information", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Version:", "1.0.0");
            EditorGUILayout.LabelField("Min Unity Version:", "2022.3 LTS");
            EditorGUILayout.LabelField("Platform:", "WebGL");
            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            EditorGUILayout.EndScrollView();
        }

        private void LoadSettings()
        {
            apiKey = EditorPrefs.GetString(PREF_API_KEY, "");
            gameId = EditorPrefs.GetString(PREF_GAME_ID, "");
            enableAds = EditorPrefs.GetBool(PREF_ENABLE_ADS, true);
            enableAnalytics = EditorPrefs.GetBool(PREF_ENABLE_ANALYTICS, true);
            enableCloudSave = EditorPrefs.GetBool(PREF_ENABLE_CLOUD_SAVE, true);
            enableLeaderboards = EditorPrefs.GetBool(PREF_ENABLE_LEADERBOARDS, true);
            enableTournaments = EditorPrefs.GetBool(PREF_ENABLE_TOURNAMENTS, true);
            enableAchievements = EditorPrefs.GetBool(PREF_ENABLE_ACHIEVEMENTS, true);
            enableFriends = EditorPrefs.GetBool(PREF_ENABLE_FRIENDS, true);
            enableChat = EditorPrefs.GetBool(PREF_ENABLE_CHAT, true);
        }

        private void SaveSettings()
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                EditorUtility.DisplayDialog(
                    "Validation Error",
                    "API key is required. Get your API key from the Studio Dashboard at solisgames.com/studio",
                    "OK"
                );
                return;
            }

            EditorPrefs.SetString(PREF_API_KEY, apiKey);
            EditorPrefs.SetString(PREF_GAME_ID, gameId);
            EditorPrefs.SetBool(PREF_ENABLE_ADS, enableAds);
            EditorPrefs.SetBool(PREF_ENABLE_ANALYTICS, enableAnalytics);
            EditorPrefs.SetBool(PREF_ENABLE_CLOUD_SAVE, enableCloudSave);
            EditorPrefs.SetBool(PREF_ENABLE_LEADERBOARDS, enableLeaderboards);
            EditorPrefs.SetBool(PREF_ENABLE_TOURNAMENTS, enableTournaments);
            EditorPrefs.SetBool(PREF_ENABLE_ACHIEVEMENTS, enableAchievements);
            EditorPrefs.SetBool(PREF_ENABLE_FRIENDS, enableFriends);
            EditorPrefs.SetBool(PREF_ENABLE_CHAT, enableChat);

            EditorUtility.DisplayDialog(
                "Settings Saved",
                "Solis Games SDK settings have been saved successfully!",
                "OK"
            );

            Debug.Log("[Solis SDK] Settings saved successfully");
        }

        private void ResetToDefaults()
        {
            if (EditorUtility.DisplayDialog(
                "Reset to Defaults",
                "Are you sure you want to reset all settings to defaults? This will not affect your API key.",
                "Reset",
                "Cancel"
            ))
            {
                enableAds = true;
                enableAnalytics = true;
                enableCloudSave = true;
                enableLeaderboards = true;
                enableTournaments = true;
                enableAchievements = true;
                enableFriends = true;
                enableChat = true;

                Debug.Log("[Solis SDK] Settings reset to defaults");
            }
        }

        /// <summary>
        /// Get the API key from EditorPrefs
        /// Used by the post-build script
        /// </summary>
        public static string GetApiKey()
        {
            return EditorPrefs.GetString(PREF_API_KEY, "");
        }

        /// <summary>
        /// Get the game ID from EditorPrefs
        /// Used by the post-build script
        /// </summary>
        public static string GetGameId()
        {
            return EditorPrefs.GetString(PREF_GAME_ID, "");
        }
    }
}
