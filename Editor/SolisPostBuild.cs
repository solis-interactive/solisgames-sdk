using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace SolisGames.Editor
{
    /// <summary>
    /// Post-build script that automatically injects the Solis Games SDK script into WebGL builds
    /// Runs automatically after every WebGL build
    /// </summary>
    public class SolisPostBuild
    {
        private const string SDK_SCRIPT_URL = "https://solisgames.com/solis-games-sdk-v1.js";

        /// <summary>
        /// Post-process callback that runs after Unity builds the player
        /// Priority 1 ensures this runs early in the post-build process
        /// </summary>
        [PostProcessBuild(1)]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
        {
            // Only process WebGL builds
            if (target != BuildTarget.WebGL)
            {
                return;
            }

            Debug.Log("[Solis SDK] Post-build: Injecting SDK script into WebGL build...");

            try
            {
                InjectSDKScript(pathToBuiltProject);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[Solis SDK] Post-build failed: {ex.Message}");
                EditorUtility.DisplayDialog(
                    "SDK Injection Failed",
                    $"Failed to inject Solis Games SDK into build:\n\n{ex.Message}\n\nYou may need to manually add the SDK script to your index.html file.",
                    "OK"
                );
            }
        }

        /// <summary>
        /// Inject the Solis Games SDK script into the index.html file
        /// </summary>
        private static void InjectSDKScript(string buildPath)
        {
            // Get API key from SDK settings
            string apiKey = SolisSDKWindow.GetApiKey();

            if (string.IsNullOrEmpty(apiKey))
            {
                Debug.LogWarning("[Solis SDK] ⚠️ API key not configured. SDK script will be injected, but you need to configure the API key in SDK Settings.");
            }

            // Get game ID (optional)
            string gameId = SolisSDKWindow.GetGameId();

            // Find index.html
            string indexPath = Path.Combine(buildPath, "index.html");

            if (!File.Exists(indexPath))
            {
                throw new System.Exception($"index.html not found at: {indexPath}");
            }

            // Read HTML content
            string html = File.ReadAllText(indexPath);

            // Check if SDK is already injected (avoid double injection)
            if (html.Contains(SDK_SCRIPT_URL))
            {
                Debug.Log("[Solis SDK] ℹ️ SDK script already present in build, skipping injection");
                return;
            }

            // Build SDK script tag with data attributes
            string sdkScriptTag = $@"
    <!-- Solis Games SDK - Auto-injected by Unity Plugin -->
    <script src=""{SDK_SCRIPT_URL}""
            data-api-key=""{apiKey}""
            {(string.IsNullOrEmpty(gameId) ? "" : $"data-game-id=\"{gameId}\" ")}data-environment=""production""></script>";

            // Inject before closing </head> tag
            int headCloseIndex = html.IndexOf("</head>");

            if (headCloseIndex == -1)
            {
                throw new System.Exception("</head> tag not found in index.html");
            }

            // Insert SDK script before </head>
            html = html.Insert(headCloseIndex, sdkScriptTag + "\n");

            // Write modified HTML back to file
            File.WriteAllText(indexPath, html);

            Debug.Log("[Solis SDK] ✅ SDK script injected successfully!");

            if (string.IsNullOrEmpty(apiKey))
            {
                Debug.LogWarning("[Solis SDK] ⚠️ Remember to configure your API key in Window > Solis Games > SDK Settings");
            }
            else
            {
                Debug.Log($"[Solis SDK] API Key: {apiKey.Substring(0, System.Math.Min(8, apiKey.Length))}... (configured)");
            }

            if (!string.IsNullOrEmpty(gameId))
            {
                Debug.Log($"[Solis SDK] Game ID: {gameId} (configured)");
            }
        }

        /// <summary>
        /// Menu item to manually test SDK injection on an existing build
        /// Useful for debugging or re-injecting after manual HTML edits
        /// </summary>
        [MenuItem("Window/Solis Games/Re-inject SDK (Manual)", false, 100)]
        public static void ManualReInject()
        {
            string buildPath = EditorUtility.OpenFolderPanel("Select WebGL Build Folder", "Builds/WebGL", "");

            if (string.IsNullOrEmpty(buildPath))
            {
                return;
            }

            string indexPath = Path.Combine(buildPath, "index.html");

            if (!File.Exists(indexPath))
            {
                EditorUtility.DisplayDialog(
                    "Invalid Build Folder",
                    "Selected folder does not contain an index.html file. Please select a valid WebGL build folder.",
                    "OK"
                );
                return;
            }

            try
            {
                // Remove existing SDK script first (if present)
                string html = File.ReadAllText(indexPath);

                // Remove old SDK injection markers
                int startMarker = html.IndexOf("<!-- Solis Games SDK - Auto-injected by Unity Plugin -->");
                if (startMarker != -1)
                {
                    int endMarker = html.IndexOf("</script>", startMarker);
                    if (endMarker != -1)
                    {
                        html = html.Remove(startMarker, endMarker - startMarker + 9); // 9 = "</script>".Length
                        File.WriteAllText(indexPath, html);
                        Debug.Log("[Solis SDK] Removed old SDK injection");
                    }
                }

                // Re-inject
                InjectSDKScript(buildPath);

                EditorUtility.DisplayDialog(
                    "SDK Re-injected",
                    "Solis Games SDK has been successfully re-injected into the build!",
                    "OK"
                );
            }
            catch (System.Exception ex)
            {
                EditorUtility.DisplayDialog(
                    "Re-injection Failed",
                    $"Failed to re-inject SDK:\n\n{ex.Message}",
                    "OK"
                );
                Debug.LogError($"[Solis SDK] Re-injection failed: {ex.Message}");
            }
        }
    }
}
