using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using SolisGames.Models;

namespace SolisGames
{
    /// <summary>
    /// Tournaments module for competitive tournaments
    /// Join tournaments, view brackets, and track status
    /// </summary>
    public class SolisTournaments : MonoBehaviour
    {
        private TaskCompletionSource<bool> _joinTcs;
        private TaskCompletionSource<TournamentList> _listTcs;
        private TaskCompletionSource<TournamentBracket> _getBracketTcs;

        #region JavaScript Imports

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void SolisSDK_Tournament_Join(string tournamentId, Action<int> callback);

        [DllImport("__Internal")]
        private static extern void SolisSDK_Tournament_List(string status, string gameObjectName, string callbackMethod);

        [DllImport("__Internal")]
        private static extern void SolisSDK_Tournament_GetBracket(string tournamentId, string gameObjectName, string callbackMethod);
#endif

        #endregion

        #region Public API

        /// <summary>
        /// Join a tournament
        /// </summary>
        /// <param name="tournamentId">Tournament UUID</param>
        /// <returns>True if successfully joined</returns>
        public async Task<bool> JoinAsync(string tournamentId)
        {
            if (!SolisSDK.CheckInitialized("Tournaments"))
                return false;

            if (string.IsNullOrEmpty(tournamentId))
            {
                Debug.LogError("[SolisSDK.Tournaments] Tournament ID is required");
                return false;
            }

#if UNITY_WEBGL && !UNITY_EDITOR
            _joinTcs = new TaskCompletionSource<bool>();

            try
            {
                SolisSDK_Tournament_Join(tournamentId, OnJoinComplete);
                return await _joinTcs.Task;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.Tournaments] Join error: {ex.Message}");
                return false;
            }
#else
            // Editor mode
            await Task.Delay(500);
            Debug.Log($"[SolisSDK.Tournaments] [Demo] Joined tournament: {tournamentId}");
            return true;
#endif
        }

        /// <summary>
        /// List tournaments
        /// </summary>
        /// <param name="status">Filter by status: "active", "pending", "completed", or "all" (default: "all")</param>
        /// <returns>List of tournaments</returns>
        public async Task<TournamentList> ListAsync(string status = "all")
        {
            if (!SolisSDK.CheckInitialized("Tournaments"))
                return null;

#if UNITY_WEBGL && !UNITY_EDITOR
            _listTcs = new TaskCompletionSource<TournamentList>();

            try
            {
                SolisSDK_Tournament_List(status, gameObject.name, nameof(OnListReceived));
                return await _listTcs.Task;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.Tournaments] List error: {ex.Message}");
                return null;
            }
#else
            // Editor mode - mock data
            await Task.Delay(500);
            var mockTournaments = new System.Collections.Generic.List<Tournament>
            {
                new Tournament
                {
                    id = "tournament_1",
                    name = "Weekend Blitz",
                    description = "Compete for glory!",
                    status = "active",
                    bracket_type = "single_elimination",
                    start_time = DateTime.Now.ToString("o"),
                    end_time = DateTime.Now.AddHours(48).ToString("o"),
                    max_participants = 64,
                    participant_count = 42,
                    prize_pool = new PrizePool { coins = 10000 },
                    is_participant = false
                }
            };

            Debug.Log($"[SolisSDK.Tournaments] [Demo] Retrieved {mockTournaments.Count} tournaments");

            return new TournamentList { tournaments = mockTournaments };
#endif
        }

        /// <summary>
        /// Get tournament bracket with all matches
        /// </summary>
        /// <param name="tournamentId">Tournament UUID</param>
        /// <returns>Tournament bracket data</returns>
        public async Task<TournamentBracket> GetBracketAsync(string tournamentId)
        {
            if (!SolisSDK.CheckInitialized("Tournaments"))
                return null;

            if (string.IsNullOrEmpty(tournamentId))
            {
                Debug.LogError("[SolisSDK.Tournaments] Tournament ID is required");
                return null;
            }

#if UNITY_WEBGL && !UNITY_EDITOR
            _getBracketTcs = new TaskCompletionSource<TournamentBracket>();

            try
            {
                SolisSDK_Tournament_GetBracket(tournamentId, gameObject.name, nameof(OnBracketReceived));
                return await _getBracketTcs.Task;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.Tournaments] Get bracket error: {ex.Message}");
                return null;
            }
#else
            // Editor mode - mock bracket
            await Task.Delay(500);
            var mockMatches = new System.Collections.Generic.List<TournamentMatch>
            {
                new TournamentMatch
                {
                    id = "match_1",
                    tournament_id = tournamentId,
                    round = 1,
                    player1_id = "player_1",
                    player2_id = "player_2",
                    player1_username = "Player1",
                    player2_username = "Player2",
                    winner_id = "player_1",
                    status = "completed",
                    player1_score = 100,
                    player2_score = 85
                }
            };

            Debug.Log($"[SolisSDK.Tournaments] [Demo] Retrieved bracket with {mockMatches.Count} matches");

            return new TournamentBracket
            {
                tournament_id = tournamentId,
                matches = mockMatches
            };
#endif
        }

        #endregion

        #region Callbacks

        [AOT.MonoPInvokeCallback(typeof(Action<int>))]
        private void OnJoinComplete(int success)
        {
            _joinTcs?.SetResult(success == 1);
        }

        private void OnListReceived(string listJson)
        {
            try
            {
                var list = JsonUtility.FromJson<TournamentList>(listJson);
                _listTcs?.SetResult(list);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.Tournaments] Parse list error: {ex.Message}");
                _listTcs?.SetResult(null);
            }
        }

        private void OnBracketReceived(string bracketJson)
        {
            try
            {
                var bracket = JsonUtility.FromJson<TournamentBracket>(bracketJson);
                _getBracketTcs?.SetResult(bracket);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.Tournaments] Parse bracket error: {ex.Message}");
                _getBracketTcs?.SetResult(null);
            }
        }

        #endregion
    }
}
