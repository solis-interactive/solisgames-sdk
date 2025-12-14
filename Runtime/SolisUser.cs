using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using SolisGames.Models;

namespace SolisGames
{
    /// <summary>
    /// User management module
    /// Get user profile data and authentication status
    /// </summary>
    public class SolisUser : MonoBehaviour
    {
        private TaskCompletionSource<UserData> _getUserTcs;

        #region JavaScript Imports

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void SolisSDK_GetUser(string gameObjectName, string callbackMethod);
#endif

        #endregion

        #region Public API

        /// <summary>
        /// Get current user data
        /// </summary>
        /// <returns>UserData object with user profile information</returns>
        public async Task<UserData> GetAsync()
        {
            if (!SolisSDK.CheckInitialized("User"))
                return null;

#if UNITY_WEBGL && !UNITY_EDITOR
            _getUserTcs = new TaskCompletionSource<UserData>();

            try
            {
                SolisSDK_GetUser(gameObject.name, nameof(OnUserDataReceived));
                return await _getUserTcs.Task;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.User] Get user error: {ex.Message}");
                return null;
            }
#else
            // Editor mode - return mock data
            await Task.Delay(300);
            return new UserData
            {
                id = "demo_user_" + UnityEngine.Random.Range(1000, 9999),
                username = "DemoPlayer",
                email = "demo@example.com",
                avatar_url = "https://via.placeholder.com/150",
                is_authenticated = true,
                total_xp = 2500,
                level = 15,
                created_at = DateTime.Now.AddDays(-30).ToString("o")
            };
#endif
        }

        #endregion

        #region Callbacks

        // Called by JavaScript via SendMessage
        private void OnUserDataReceived(string userJson)
        {
            try
            {
                var userData = JsonUtility.FromJson<UserData>(userJson);

                if (userData != null && string.IsNullOrEmpty(userData.id) == false)
                {
                    _getUserTcs?.SetResult(userData);
                }
                else
                {
                    Debug.LogWarning("[SolisSDK.User] Invalid user data received");
                    _getUserTcs?.SetResult(null);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.User] Parse error: {ex.Message}");
                _getUserTcs?.SetResult(null);
            }
        }

        #endregion
    }
}
