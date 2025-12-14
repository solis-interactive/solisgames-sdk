using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace SolisGames.Editor
{
    /// <summary>
    /// Build & Deploy window for building WebGL and deploying to Solis Games
    /// Accessible via Window > Solis Games > Build & Deploy
    /// </summary>
    public class SolisBuildWindow : EditorWindow
    {
        private const string PREF_BUILD_PATH = "SolisGames_BuildPath";
        private const string PREF_COMPRESSION = "SolisGames_Compression";
        private const string PREF_DEVELOPMENT_BUILD = "SolisGames_DevelopmentBuild";
        private const string PREF_AUTO_RUN = "SolisGames_AutoRun";

        private string buildPath = "Builds/WebGL";
        private int compressionIndex = 0;
        private bool developmentBuild = false;
        private bool autoRunAfterBuild = false;
        private bool isBuilding = false;

        private readonly string[] compressionOptions = new string[]
        {
            "Gzip (Recommended)",
            "Brotli (Smaller)",
            "Disabled"
        };

        private Vector2 scrollPosition;
        private GUIStyle headerStyle;
        private GUIStyle sectionStyle;
        private bool stylesInitialized = false;

        [MenuItem("Window/Solis Games/Build & Deploy")]
        public static void ShowWindow()
        {
            var window = GetWindow<SolisBuildWindow>("Solis Build");
            window.minSize = new Vector2(450, 500);
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
            GUILayout.Label("Solis Games Build & Deploy", headerStyle);
            EditorGUILayout.HelpBox(
                "Build your game for WebGL with automatic SDK integration. The Solis Games SDK script will be automatically injected into your build.",
                MessageType.Info
            );

            GUILayout.Space(10);

            // API Key Check
            string apiKey = SolisSDKWindow.GetApiKey();
            if (string.IsNullOrEmpty(apiKey))
            {
                EditorGUILayout.HelpBox(
                    "⚠️ API key not configured. Open SDK Settings to configure your API key before building.",
                    MessageType.Warning
                );

                if (GUILayout.Button("Open SDK Settings", GUILayout.Height(30)))
                {
                    SolisSDKWindow.ShowWindow();
                }

                GUILayout.Space(10);
            }
            else
            {
                EditorGUILayout.HelpBox($"✅ API key configured: {apiKey.Substring(0, Math.Min(8, apiKey.Length))}...", MessageType.Info);
                GUILayout.Space(10);
            }

            // Build Settings Section
            EditorGUILayout.BeginVertical(sectionStyle);
            GUILayout.Label("Build Settings", EditorStyles.boldLabel);
            GUILayout.Space(5);

            // Build Path
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Output Path:", GUILayout.Width(100));
            buildPath = EditorGUILayout.TextField(buildPath);
            if (GUILayout.Button("Browse", GUILayout.Width(70)))
            {
                string path = EditorUtility.SaveFolderPanel("Select Build Location", buildPath, "");
                if (!string.IsNullOrEmpty(path))
                {
                    buildPath = path;
                }
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);

            // Compression
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Compression:", GUILayout.Width(100));
            compressionIndex = EditorGUILayout.Popup(compressionIndex, compressionOptions);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);

            // Development Build
            developmentBuild = EditorGUILayout.Toggle("Development Build", developmentBuild);
            EditorGUILayout.LabelField("Enable for debugging and profiling", EditorStyles.miniLabel);

            GUILayout.Space(5);

            // Auto Run
            autoRunAfterBuild = EditorGUILayout.Toggle("Auto-run after build", autoRunAfterBuild);
            EditorGUILayout.LabelField("Open build in default browser after completion", EditorStyles.miniLabel);

            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            // Build Info
            EditorGUILayout.BeginVertical(sectionStyle);
            GUILayout.Label("Build Information", EditorStyles.boldLabel);
            GUILayout.Space(5);

            EditorGUILayout.LabelField("Target Platform:", "WebGL");
            EditorGUILayout.LabelField("SDK Version:", "1.0.0");
            EditorGUILayout.LabelField("Unity Version:", Application.unityVersion);

            int sceneCount = EditorBuildSettings.scenes.Length;
            int enabledScenes = EditorBuildSettings.scenes.Count(s => s.enabled);
            EditorGUILayout.LabelField("Scenes in Build:", $"{enabledScenes} of {sceneCount}");

            if (enabledScenes == 0)
            {
                EditorGUILayout.HelpBox("⚠️ No scenes enabled in build settings. Add scenes in File > Build Settings.", MessageType.Warning);
            }

            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            // Build Actions
            EditorGUILayout.BeginVertical(sectionStyle);

            GUI.enabled = !isBuilding && !string.IsNullOrEmpty(apiKey) && enabledScenes > 0;

            if (GUILayout.Button("🔨 Build WebGL", GUILayout.Height(40)))
            {
                BuildWebGL();
            }

            GUI.enabled = true;

            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Open Build Folder", GUILayout.Height(30)))
            {
                if (Directory.Exists(buildPath))
                {
                    EditorUtility.RevealInFinder(buildPath);
                }
                else
                {
                    EditorUtility.DisplayDialog("Folder Not Found", $"Build folder does not exist: {buildPath}", "OK");
                }
            }

            if (GUILayout.Button("Open Build Settings", GUILayout.Height(30)))
            {
                EditorWindow.GetWindow(Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            // Deploy Section (Coming Soon)
            EditorGUILayout.BeginVertical(sectionStyle);
            GUILayout.Label("Deploy to Solis Games (Coming Soon)", EditorStyles.boldLabel);
            GUILayout.Space(5);

            EditorGUILayout.HelpBox(
                "One-click deployment to Solis Games platform will be available in a future update. For now, manually upload your build to the Studio Dashboard.",
                MessageType.Info
            );

            GUI.enabled = false;
            if (GUILayout.Button("🚀 Deploy Build", GUILayout.Height(30)))
            {
                // Future: Deploy to Solis Games
            }
            GUI.enabled = true;

            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            // Help Section
            EditorGUILayout.BeginVertical(sectionStyle);
            GUILayout.Label("Need Help?", EditorStyles.boldLabel);
            GUILayout.Space(5);

            if (GUILayout.Button("📚 View Build Documentation", GUILayout.Height(25)))
            {
                Application.OpenURL("https://solisgames.com/docs/unity/building");
            }

            if (GUILayout.Button("💬 Get Support", GUILayout.Height(25)))
            {
                Application.OpenURL("https://github.com/solis-interactive/unity-sdk/issues");
            }

            EditorGUILayout.EndVertical();

            GUILayout.Space(10);

            EditorGUILayout.EndScrollView();
        }

        private void LoadSettings()
        {
            buildPath = EditorPrefs.GetString(PREF_BUILD_PATH, "Builds/WebGL");
            compressionIndex = EditorPrefs.GetInt(PREF_COMPRESSION, 0);
            developmentBuild = EditorPrefs.GetBool(PREF_DEVELOPMENT_BUILD, false);
            autoRunAfterBuild = EditorPrefs.GetBool(PREF_AUTO_RUN, false);
        }

        private void SaveSettings()
        {
            EditorPrefs.SetString(PREF_BUILD_PATH, buildPath);
            EditorPrefs.SetInt(PREF_COMPRESSION, compressionIndex);
            EditorPrefs.SetBool(PREF_DEVELOPMENT_BUILD, developmentBuild);
            EditorPrefs.SetBool(PREF_AUTO_RUN, autoRunAfterBuild);
        }

        private void BuildWebGL()
        {
            // Validate settings
            string apiKey = SolisSDKWindow.GetApiKey();
            if (string.IsNullOrEmpty(apiKey))
            {
                EditorUtility.DisplayDialog(
                    "API Key Required",
                    "Please configure your API key in SDK Settings before building.",
                    "OK"
                );
                return;
            }

            if (EditorBuildSettings.scenes.Count(s => s.enabled) == 0)
            {
                EditorUtility.DisplayDialog(
                    "No Scenes in Build",
                    "Please add at least one scene to the build in File > Build Settings.",
                    "OK"
                );
                return;
            }

            // Save settings
            SaveSettings();

            // Configure build options
            BuildPlayerOptions buildOptions = new BuildPlayerOptions
            {
                scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray(),
                locationPathName = buildPath,
                target = BuildTarget.WebGL,
                options = developmentBuild ? BuildOptions.Development : BuildOptions.None
            };

            // Set compression
            PlayerSettings.WebGL.compressionFormat = compressionIndex switch
            {
                0 => WebGLCompressionFormat.Gzip,
                1 => WebGLCompressionFormat.Brotli,
                2 => WebGLCompressionFormat.Disabled,
                _ => WebGLCompressionFormat.Gzip
            };

            // Start build
            isBuilding = true;
            Debug.Log($"[Solis SDK] Starting WebGL build to: {buildPath}");

            BuildReport report = BuildPipeline.BuildPlayer(buildOptions);
            BuildSummary summary = report.summary;

            isBuilding = false;

            // Handle result
            if (summary.result == BuildResult.Succeeded)
            {
                Debug.Log($"[Solis SDK] ✅ Build succeeded! Size: {summary.totalSize / (1024 * 1024)}MB, Time: {summary.totalTime.TotalSeconds:F1}s");

                EditorUtility.DisplayDialog(
                    "Build Successful",
                    $"WebGL build completed successfully!\n\nSize: {summary.totalSize / (1024 * 1024):F1} MB\nTime: {summary.totalTime.TotalSeconds:F1}s\n\nThe Solis Games SDK has been automatically injected into your build.",
                    "OK"
                );

                // Auto-run if enabled
                if (autoRunAfterBuild)
                {
                    string indexPath = Path.Combine(buildPath, "index.html");
                    if (File.Exists(indexPath))
                    {
                        Application.OpenURL("file:///" + indexPath.Replace("\\", "/"));
                    }
                }
            }
            else if (summary.result == BuildResult.Failed)
            {
                Debug.LogError($"[Solis SDK] ❌ Build failed with {summary.totalErrors} error(s)");

                EditorUtility.DisplayDialog(
                    "Build Failed",
                    $"WebGL build failed with {summary.totalErrors} error(s). Check the Console for details.",
                    "OK"
                );
            }
            else if (summary.result == BuildResult.Cancelled)
            {
                Debug.Log("[Solis SDK] Build cancelled by user");
            }

            Repaint();
        }
    }
}
