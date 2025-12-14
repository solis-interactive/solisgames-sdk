using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using SolisGames.Models;

namespace SolisGames
{
    /// <summary>
    /// Friends module for social features
    /// Manage friends list, presence status, and online friends
    /// </summary>
    public class SolisFriends : MonoBehaviour
    {
        private TaskCompletionSource<FriendList> _listTcs;
        private TaskCompletionSource<bool> _addTcs;
        private TaskCompletionSource<bool> _removeTcs;
        private TaskCompletionSource<FriendList> _getOnlineTcs;
        private TaskCompletionSource<bool> _updatePresenceTcs;

        #region JavaScript Imports

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void SolisSDK_Friends_List(string gameObjectName, string callbackMethod);

        [DllImport("__Internal")]
        private static extern void SolisSDK_Friends_Add(string userId, Action<int> callback);

        [DllImport("__Internal")]
        private static extern void SolisSDK_Friends_Remove(string userId, Action<int> callback);

        [DllImport("__Internal")]
        private static extern void SolisSDK_Friends_GetOnline(string gameObjectName, string callbackMethod);

        [DllImport("__Internal")]
        private static extern void SolisSDK_Friends_UpdatePresence(string status, string metadataJson, Action<int> callback);
#endif

        #endregion

        #region Public API

        /// <summary>
        /// Get friends list with presence information
        /// </summary>
        /// <returns>List of friends</returns>
        public async Task<List<Friend>> ListAsync()
        {
            if (!SolisSDK.CheckInitialized("Friends"))
                return null;

#if UNITY_WEBGL && !UNITY_EDITOR
            _listTcs = new TaskCompletionSource<FriendList>();

            try
            {
                SolisSDK_Friends_List(gameObject.name, nameof(OnFriendsListReceived));
                var result = await _listTcs.Task;
                return result?.friends;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.Friends] List error: {ex.Message}");
                return null;
            }
#else
            // Editor mode - mock friends
            await Task.Delay(500);
            var mockFriends = new List<Friend>
            {
                new Friend
                {
                    id = "friend_1",
                    username = "BestFriend",
                    avatar_url = "https://via.placeholder.com/150",
                    presence = new FriendPresence
                    {
                        status = "playing",
                        game_title = "Space Raiders",
                        custom_status = "Level 10",
                        joinable = true
                    }
                },
                new Friend
                {
                    id = "friend_2",
                    username = "GamingBuddy",
                    avatar_url = "https://via.placeholder.com/150",
                    presence = new FriendPresence
                    {
                        status = "online",
                        joinable = false
                    }
                }
            };

            Debug.Log($"[SolisSDK.Friends] [Demo] Retrieved {mockFriends.Count} friends");
            return mockFriends;
#endif
        }

        /// <summary>
        /// Send friend request
        /// </summary>
        /// <param name="userId">User ID to add as friend</param>
        /// <returns>True if request was sent successfully</returns>
        public async Task<bool> AddAsync(string userId)
        {
            if (!SolisSDK.CheckInitialized("Friends"))
                return false;

            if (string.IsNullOrEmpty(userId))
            {
                Debug.LogError("[SolisSDK.Friends] User ID is required");
                return false;
            }

#if UNITY_WEBGL && !UNITY_EDITOR
            _addTcs = new TaskCompletionSource<bool>();

            try
            {
                SolisSDK_Friends_Add(userId, OnAddComplete);
                return await _addTcs.Task;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.Friends] Add error: {ex.Message}");
                return false;
            }
#else
            // Editor mode
            await Task.Delay(300);
            Debug.Log($"[SolisSDK.Friends] [Demo] Friend request sent to: {userId}");
            return true;
#endif
        }

        /// <summary>
        /// Remove friend
        /// </summary>
        /// <param name="userId">User ID to remove from friends</param>
        /// <returns>True if removed successfully</returns>
        public async Task<bool> RemoveAsync(string userId)
        {
            if (!SolisSDK.CheckInitialized("Friends"))
                return false;

            if (string.IsNullOrEmpty(userId))
            {
                Debug.LogError("[SolisSDK.Friends] User ID is required");
                return false;
            }

#if UNITY_WEBGL && !UNITY_EDITOR
            _removeTcs = new TaskCompletionSource<bool>();

            try
            {
                SolisSDK_Friends_Remove(userId, OnRemoveComplete);
                return await _removeTcs.Task;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.Friends] Remove error: {ex.Message}");
                return false;
            }
#else
            // Editor mode
            await Task.Delay(300);
            Debug.Log($"[SolisSDK.Friends] [Demo] Friend removed: {userId}");
            return true;
#endif
        }

        /// <summary>
        /// Get online friends only
        /// </summary>
        /// <returns>List of online friends</returns>
        public async Task<List<Friend>> GetOnlineAsync()
        {
            if (!SolisSDK.CheckInitialized("Friends"))
                return null;

#if UNITY_WEBGL && !UNITY_EDITOR
            _getOnlineTcs = new TaskCompletionSource<FriendList>();

            try
            {
                SolisSDK_Friends_GetOnline(gameObject.name, nameof(OnOnlineFriendsReceived));
                var result = await _getOnlineTcs.Task;
                return result?.friends;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.Friends] Get online error: {ex.Message}");
                return null;
            }
#else
            // Editor mode - mock online friends
            await Task.Delay(500);
            var mockOnline = new List<Friend>
            {
                new Friend
                {
                    id = "friend_1",
                    username = "BestFriend",
                    avatar_url = "https://via.placeholder.com/150",
                    presence = new FriendPresence
                    {
                        status = "online",
                        joinable = true
                    }
                }
            };

            Debug.Log($"[SolisSDK.Friends] [Demo] {mockOnline.Count} friends online");
            return mockOnline;
#endif
        }

        /// <summary>
        /// Update presence status
        /// </summary>
        /// <param name="status">Status: "online", "away", "offline", "playing"</param>
        /// <param name="gameTitle">Optional game title when playing</param>
        /// <param name="customStatus">Optional custom status message</param>
        /// <param name="joinable">Whether friends can join your session</param>
        /// <returns>True if presence updated successfully</returns>
        public async Task<bool> UpdatePresenceAsync(string status, string gameTitle = null, string customStatus = null, bool joinable = false)
        {
            if (!SolisSDK.CheckInitialized("Friends"))
                return false;

            if (string.IsNullOrEmpty(status))
            {
                Debug.LogError("[SolisSDK.Friends] Status is required");
                return false;
            }

#if UNITY_WEBGL && !UNITY_EDITOR
            _updatePresenceTcs = new TaskCompletionSource<bool>();

            try
            {
                var metadata = new Dictionary<string, object>
                {
                    { "game_title", gameTitle ?? "" },
                    { "custom_status", customStatus ?? "" },
                    { "joinable", joinable }
                };

                string metadataJson = JsonUtility.ToJson(metadata);
                SolisSDK_Friends_UpdatePresence(status, metadataJson, OnUpdatePresenceComplete);
                return await _updatePresenceTcs.Task;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.Friends] Update presence error: {ex.Message}");
                return false;
            }
#else
            // Editor mode
            await Task.Delay(200);
            Debug.Log($"[SolisSDK.Friends] [Demo] Presence updated: {status} - {gameTitle ?? "no game"}");
            return true;
#endif
        }

        #endregion

        #region Callbacks

        private void OnFriendsListReceived(string listJson)
        {
            try
            {
                var list = JsonUtility.FromJson<FriendList>(listJson);
                _listTcs?.SetResult(list);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.Friends] Parse list error: {ex.Message}");
                _listTcs?.SetResult(null);
            }
        }

        [AOT.MonoPInvokeCallback(typeof(Action<int>))]
        private void OnAddComplete(int success)
        {
            _addTcs?.SetResult(success == 1);
        }

        [AOT.MonoPInvokeCallback(typeof(Action<int>))]
        private void OnRemoveComplete(int success)
        {
            _removeTcs?.SetResult(success == 1);
        }

        private void OnOnlineFriendsReceived(string listJson)
        {
            try
            {
                var list = JsonUtility.FromJson<FriendList>(listJson);
                _getOnlineTcs?.SetResult(list);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.Friends] Parse online list error: {ex.Message}");
                _getOnlineTcs?.SetResult(null);
            }
        }

        [AOT.MonoPInvokeCallback(typeof(Action<int>))]
        private void OnUpdatePresenceComplete(int success)
        {
            _updatePresenceTcs?.SetResult(success == 1);
        }

        #endregion
    }
}
