using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using SolisGames.Models;

namespace SolisGames
{
    /// <summary>
    /// Chat module for real-time messaging
    /// Send messages, get chat history, and subscribe to channels
    /// </summary>
    public class SolisChat : MonoBehaviour
    {
        private TaskCompletionSource<bool> _sendTcs;
        private TaskCompletionSource<ChatHistory> _getHistoryTcs;

        #region JavaScript Imports

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void SolisSDK_Chat_Send(string channelId, string message, Action<int> callback);

        [DllImport("__Internal")]
        private static extern void SolisSDK_Chat_GetHistory(string channelId, int limit, string gameObjectName, string callbackMethod);
#endif

        #endregion

        #region Public API

        /// <summary>
        /// Send a chat message
        /// </summary>
        /// <param name="channelId">Channel identifier</param>
        /// <param name="message">Message text</param>
        /// <returns>True if message was sent successfully</returns>
        public async Task<bool> SendAsync(string channelId, string message)
        {
            if (!SolisSDK.CheckInitialized("Chat"))
                return false;

            if (string.IsNullOrEmpty(channelId))
            {
                Debug.LogError("[SolisSDK.Chat] Channel ID is required");
                return false;
            }

            if (string.IsNullOrEmpty(message))
            {
                Debug.LogError("[SolisSDK.Chat] Message is required");
                return false;
            }

#if UNITY_WEBGL && !UNITY_EDITOR
            _sendTcs = new TaskCompletionSource<bool>();

            try
            {
                SolisSDK_Chat_Send(channelId, message, OnSendComplete);
                return await _sendTcs.Task;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.Chat] Send error: {ex.Message}");
                return false;
            }
#else
            // Editor mode
            await Task.Delay(200);
            Debug.Log($"[SolisSDK.Chat] [Demo] Message sent to {channelId}: {message}");
            return true;
#endif
        }

        /// <summary>
        /// Get chat history for a channel
        /// </summary>
        /// <param name="channelId">Channel identifier</param>
        /// <param name="limit">Number of messages to fetch (default: 50)</param>
        /// <returns>Chat history with messages</returns>
        public async Task<List<ChatMessage>> GetHistoryAsync(string channelId, int limit = 50)
        {
            if (!SolisSDK.CheckInitialized("Chat"))
                return null;

            if (string.IsNullOrEmpty(channelId))
            {
                Debug.LogError("[SolisSDK.Chat] Channel ID is required");
                return null;
            }

#if UNITY_WEBGL && !UNITY_EDITOR
            _getHistoryTcs = new TaskCompletionSource<ChatHistory>();

            try
            {
                SolisSDK_Chat_GetHistory(channelId, limit, gameObject.name, nameof(OnHistoryReceived));
                var result = await _getHistoryTcs.Task;
                return result?.messages;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.Chat] Get history error: {ex.Message}");
                return null;
            }
#else
            // Editor mode - mock messages
            await Task.Delay(500);
            var mockMessages = new List<ChatMessage>
            {
                new ChatMessage
                {
                    id = "msg_1",
                    channel_id = channelId,
                    user_id = "user_1",
                    username = "Player1",
                    avatar_url = "https://via.placeholder.com/150",
                    message = "GG everyone!",
                    created_at = DateTime.Now.AddMinutes(-5).ToString("o"),
                    flagged = false
                },
                new ChatMessage
                {
                    id = "msg_2",
                    channel_id = channelId,
                    user_id = "user_2",
                    username = "Player2",
                    avatar_url = "https://via.placeholder.com/150",
                    message = "That was intense!",
                    created_at = DateTime.Now.AddMinutes(-2).ToString("o"),
                    flagged = false
                }
            };

            Debug.Log($"[SolisSDK.Chat] [Demo] Retrieved {mockMessages.Count} messages");
            return mockMessages;
#endif
        }

        #endregion

        #region Callbacks

        [AOT.MonoPInvokeCallback(typeof(Action<int>))]
        private void OnSendComplete(int success)
        {
            _sendTcs?.SetResult(success == 1);
        }

        private void OnHistoryReceived(string historyJson)
        {
            try
            {
                var history = JsonUtility.FromJson<ChatHistory>(historyJson);
                _getHistoryTcs?.SetResult(history);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.Chat] Parse history error: {ex.Message}");
                _getHistoryTcs?.SetResult(null);
            }
        }

        #endregion
    }
}
