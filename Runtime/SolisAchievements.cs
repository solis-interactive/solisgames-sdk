using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using SolisGames.Models;

namespace SolisGames
{
    /// <summary>
    /// Achievements module for Xbox Live-style achievements
    /// Unlock achievements, track progress, and earn XP
    /// </summary>
    public class SolisAchievements : MonoBehaviour
    {
        private TaskCompletionSource<bool> _unlockTcs;
        private TaskCompletionSource<bool> _updateProgressTcs;
        private TaskCompletionSource<AchievementList> _listTcs;

        #region JavaScript Imports

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void SolisSDK_Achievement_Unlock(string achievementId, Action<int> callback);

        [DllImport("__Internal")]
        private static extern void SolisSDK_Achievement_UpdateProgress(string achievementId, int current, int target, Action<int> callback);

        [DllImport("__Internal")]
        private static extern void SolisSDK_Achievement_List(string gameObjectName, string callbackMethod);
#endif

        #endregion

        #region Public API

        /// <summary>
        /// Unlock an achievement
        /// </summary>
        /// <param name="achievementId">Achievement identifier</param>
        /// <returns>True if unlocked successfully</returns>
        public async Task<bool> UnlockAsync(string achievementId)
        {
            if (!SolisSDK.CheckInitialized("Achievements"))
                return false;

            if (string.IsNullOrEmpty(achievementId))
            {
                Debug.LogError("[SolisSDK.Achievements] Achievement ID is required");
                return false;
            }

#if UNITY_WEBGL && !UNITY_EDITOR
            _unlockTcs = new TaskCompletionSource<bool>();

            try
            {
                SolisSDK_Achievement_Unlock(achievementId, OnUnlockComplete);
                return await _unlockTcs.Task;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.Achievements] Unlock error: {ex.Message}");
                return false;
            }
#else
            // Editor mode
            await Task.Delay(300);
            Debug.Log($"[SolisSDK.Achievements] [Demo] Achievement unlocked: {achievementId}");
            return true;
#endif
        }

        /// <summary>
        /// Update achievement progress
        /// For achievements that track progress (e.g., "Collect 100 coins")
        /// </summary>
        /// <param name="achievementId">Achievement identifier</param>
        /// <param name="current">Current progress value</param>
        /// <param name="target">Target progress value</param>
        /// <returns>True if progress updated successfully</returns>
        public async Task<bool> UpdateProgressAsync(string achievementId, int current, int target)
        {
            if (!SolisSDK.CheckInitialized("Achievements"))
                return false;

            if (string.IsNullOrEmpty(achievementId))
            {
                Debug.LogError("[SolisSDK.Achievements] Achievement ID is required");
                return false;
            }

#if UNITY_WEBGL && !UNITY_EDITOR
            _updateProgressTcs = new TaskCompletionSource<bool>();

            try
            {
                SolisSDK_Achievement_UpdateProgress(achievementId, current, target, OnUpdateProgressComplete);
                return await _updateProgressTcs.Task;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.Achievements] Update progress error: {ex.Message}");
                return false;
            }
#else
            // Editor mode
            await Task.Delay(300);
            int progress = target > 0 ? (int)((float)current / target * 100) : 100;
            Debug.Log($"[SolisSDK.Achievements] [Demo] Progress updated: {achievementId} ({current}/{target} = {progress}%)");
            return true;
#endif
        }

        /// <summary>
        /// List all achievements for the current game
        /// </summary>
        /// <returns>List of achievements with unlock status</returns>
        public async Task<AchievementList> ListAsync()
        {
            if (!SolisSDK.CheckInitialized("Achievements"))
                return null;

#if UNITY_WEBGL && !UNITY_EDITOR
            _listTcs = new TaskCompletionSource<AchievementList>();

            try
            {
                SolisSDK_Achievement_List(gameObject.name, nameof(OnListReceived));
                return await _listTcs.Task;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.Achievements] List error: {ex.Message}");
                return null;
            }
#else
            // Editor mode - mock achievements
            await Task.Delay(500);
            var mockAchievements = new System.Collections.Generic.List<Achievement>
            {
                new Achievement
                {
                    id = "first_win",
                    name = "First Victory",
                    description = "Win your first game",
                    icon_url = "https://via.placeholder.com/64",
                    rarity = "common",
                    xp_reward = 100,
                    unlocked = true,
                    unlocked_at = DateTime.Now.AddDays(-5).ToString("o"),
                    progress = 100,
                    unlock_rate = 0.85f
                },
                new Achievement
                {
                    id = "speedrunner",
                    name = "Speedrunner",
                    description = "Complete level 5 in under 2 minutes",
                    icon_url = "https://via.placeholder.com/64",
                    rarity = "rare",
                    xp_reward = 500,
                    unlocked = false,
                    progress = 75,
                    unlock_rate = 0.12f
                }
            };

            Debug.Log($"[SolisSDK.Achievements] [Demo] Retrieved {mockAchievements.Count} achievements");

            return new AchievementList { achievements = mockAchievements };
#endif
        }

        #endregion

        #region Callbacks

        [AOT.MonoPInvokeCallback(typeof(Action<int>))]
        private void OnUnlockComplete(int success)
        {
            _unlockTcs?.SetResult(success == 1);
        }

        [AOT.MonoPInvokeCallback(typeof(Action<int>))]
        private void OnUpdateProgressComplete(int success)
        {
            _updateProgressTcs?.SetResult(success == 1);
        }

        private void OnListReceived(string listJson)
        {
            try
            {
                var list = JsonUtility.FromJson<AchievementList>(listJson);
                _listTcs?.SetResult(list);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.Achievements] Parse list error: {ex.Message}");
                _listTcs?.SetResult(null);
            }
        }

        #endregion
    }
}
