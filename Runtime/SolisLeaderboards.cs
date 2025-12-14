using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using SolisGames.Models;

namespace SolisGames
{
    /// <summary>
    /// Leaderboards module for competitive rankings
    /// Submit scores, get rankings, and view nearby players
    /// </summary>
    public class SolisLeaderboards : MonoBehaviour
    {
        private TaskCompletionSource<LeaderboardSubmitResult> _submitTcs;
        private TaskCompletionSource<LeaderboardRankings> _getRankingsTcs;
        private TaskCompletionSource<LeaderboardRankings> _getNearbyTcs;

        #region JavaScript Imports

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void SolisSDK_Leaderboard_Submit(string leaderboardKey, float score, string metadataJson, string gameObjectName, string callbackMethod);

        [DllImport("__Internal")]
        private static extern void SolisSDK_Leaderboard_Get(string leaderboardKey, string scope, int limit, int offset, string gameObjectName, string callbackMethod);

        [DllImport("__Internal")]
        private static extern void SolisSDK_Leaderboard_GetNearby(string leaderboardKey, int range, string gameObjectName, string callbackMethod);
#endif

        #endregion

        #region Public API

        /// <summary>
        /// Submit a score to a leaderboard
        /// </summary>
        /// <param name="leaderboardKey">Leaderboard identifier (e.g., "high_scores")</param>
        /// <param name="score">Score value</param>
        /// <param name="metadata">Optional metadata (e.g., level, time)</param>
        /// <returns>Submit result with rank information</returns>
        public async Task<LeaderboardSubmitResult> SubmitAsync(string leaderboardKey, float score, Dictionary<string, object> metadata = null)
        {
            if (!SolisSDK.CheckInitialized("Leaderboards"))
                return null;

            if (string.IsNullOrEmpty(leaderboardKey))
            {
                Debug.LogError("[SolisSDK.Leaderboards] Leaderboard key is required");
                return null;
            }

#if UNITY_WEBGL && !UNITY_EDITOR
            _submitTcs = new TaskCompletionSource<LeaderboardSubmitResult>();

            try
            {
                string metadataJson = metadata != null ? JsonUtility.ToJson(metadata) : "{}";
                SolisSDK_Leaderboard_Submit(leaderboardKey, score, metadataJson, gameObject.name, nameof(OnSubmitResultReceived));
                return await _submitTcs.Task;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.Leaderboards] Submit error: {ex.Message}");
                return null;
            }
#else
            // Editor mode - simulate submit
            await Task.Delay(500);
            int mockRank = UnityEngine.Random.Range(5, 100);
            Debug.Log($"[SolisSDK.Leaderboards] [Demo] Score submitted: {score} - Rank: {mockRank}");

            return new LeaderboardSubmitResult
            {
                success = true,
                rank = mockRank,
                score = score,
                leaderboard_key = leaderboardKey,
                flagged_for_review = false
            };
#endif
        }

        /// <summary>
        /// Get leaderboard rankings
        /// </summary>
        /// <param name="leaderboardKey">Leaderboard identifier</param>
        /// <param name="scope">Scope: "global", "daily", "weekly" (default: "global")</param>
        /// <param name="limit">Number of entries to fetch (default: 100)</param>
        /// <param name="offset">Offset for pagination (default: 0)</param>
        /// <returns>Leaderboard rankings with entries</returns>
        public async Task<LeaderboardRankings> GetAsync(string leaderboardKey, string scope = "global", int limit = 100, int offset = 0)
        {
            if (!SolisSDK.CheckInitialized("Leaderboards"))
                return null;

            if (string.IsNullOrEmpty(leaderboardKey))
            {
                Debug.LogError("[SolisSDK.Leaderboards] Leaderboard key is required");
                return null;
            }

#if UNITY_WEBGL && !UNITY_EDITOR
            _getRankingsTcs = new TaskCompletionSource<LeaderboardRankings>();

            try
            {
                SolisSDK_Leaderboard_Get(leaderboardKey, scope, limit, offset, gameObject.name, nameof(OnRankingsReceived));
                return await _getRankingsTcs.Task;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.Leaderboards] Get rankings error: {ex.Message}");
                return null;
            }
#else
            // Editor mode - generate mock rankings
            await Task.Delay(500);
            var mockEntries = new List<LeaderboardEntry>();

            for (int i = 0; i < Mathf.Min(limit, 10); i++)
            {
                mockEntries.Add(new LeaderboardEntry
                {
                    rank = offset + i + 1,
                    user_id = "user_" + (i + 1),
                    username = "Player" + (i + 1),
                    avatar_url = "https://via.placeholder.com/150",
                    score = 10000 - (i * 500),
                    created_at = DateTime.Now.AddHours(-i).ToString("o")
                });
            }

            Debug.Log($"[SolisSDK.Leaderboards] [Demo] Retrieved {mockEntries.Count} rankings");

            return new LeaderboardRankings
            {
                entries = mockEntries,
                total_count = 1000,
                leaderboard_key = leaderboardKey,
                scope = scope
            };
#endif
        }

        /// <summary>
        /// Get nearby ranks around the current player
        /// </summary>
        /// <param name="leaderboardKey">Leaderboard identifier</param>
        /// <param name="range">Number of ranks above and below player (default: 5)</param>
        /// <returns>Leaderboard rankings centered on player</returns>
        public async Task<LeaderboardRankings> GetNearbyAsync(string leaderboardKey, int range = 5)
        {
            if (!SolisSDK.CheckInitialized("Leaderboards"))
                return null;

            if (string.IsNullOrEmpty(leaderboardKey))
            {
                Debug.LogError("[SolisSDK.Leaderboards] Leaderboard key is required");
                return null;
            }

#if UNITY_WEBGL && !UNITY_EDITOR
            _getNearbyTcs = new TaskCompletionSource<LeaderboardRankings>();

            try
            {
                SolisSDK_Leaderboard_GetNearby(leaderboardKey, range, gameObject.name, nameof(OnNearbyRankingsReceived));
                return await _getNearbyTcs.Task;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.Leaderboards] Get nearby error: {ex.Message}");
                return null;
            }
#else
            // Editor mode - generate mock nearby rankings
            await Task.Delay(500);
            var mockEntries = new List<LeaderboardEntry>();
            int playerRank = 50;

            for (int i = -range; i <= range; i++)
            {
                int rank = playerRank + i;
                if (rank > 0)
                {
                    mockEntries.Add(new LeaderboardEntry
                    {
                        rank = rank,
                        user_id = i == 0 ? "current_user" : "user_" + rank,
                        username = i == 0 ? "You" : "Player" + rank,
                        avatar_url = "https://via.placeholder.com/150",
                        score = 10000 - (rank * 100),
                        created_at = DateTime.Now.AddHours(-Math.Abs(i)).ToString("o")
                    });
                }
            }

            Debug.Log($"[SolisSDK.Leaderboards] [Demo] Retrieved {mockEntries.Count} nearby rankings");

            return new LeaderboardRankings
            {
                entries = mockEntries,
                total_count = 1000,
                leaderboard_key = leaderboardKey,
                scope = "global"
            };
#endif
        }

        #endregion

        #region Callbacks

        // Called by JavaScript via SendMessage
        private void OnSubmitResultReceived(string resultJson)
        {
            try
            {
                var result = JsonUtility.FromJson<LeaderboardSubmitResult>(resultJson);

                if (result != null && result.success)
                {
                    _submitTcs?.SetResult(result);
                }
                else
                {
                    Debug.LogWarning("[SolisSDK.Leaderboards] Submit failed");
                    _submitTcs?.SetResult(null);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.Leaderboards] Parse submit result error: {ex.Message}");
                _submitTcs?.SetResult(null);
            }
        }

        // Called by JavaScript via SendMessage
        private void OnRankingsReceived(string rankingsJson)
        {
            try
            {
                var rankings = JsonUtility.FromJson<LeaderboardRankings>(rankingsJson);

                if (rankings != null && string.IsNullOrEmpty(rankings.error))
                {
                    _getRankingsTcs?.SetResult(rankings);
                }
                else
                {
                    Debug.LogWarning($"[SolisSDK.Leaderboards] Get rankings failed: {rankings?.error}");
                    _getRankingsTcs?.SetResult(null);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.Leaderboards] Parse rankings error: {ex.Message}");
                _getRankingsTcs?.SetResult(null);
            }
        }

        // Called by JavaScript via SendMessage
        private void OnNearbyRankingsReceived(string rankingsJson)
        {
            try
            {
                var rankings = JsonUtility.FromJson<LeaderboardRankings>(rankingsJson);

                if (rankings != null && string.IsNullOrEmpty(rankings.error))
                {
                    _getNearbyTcs?.SetResult(rankings);
                }
                else
                {
                    Debug.LogWarning($"[SolisSDK.Leaderboards] Get nearby failed: {rankings?.error}");
                    _getNearbyTcs?.SetResult(null);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.Leaderboards] Parse nearby error: {ex.Message}");
                _getNearbyTcs?.SetResult(null);
            }
        }

        #endregion
    }
}
