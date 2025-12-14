using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

namespace SolisGames
{
    /// <summary>
    /// Analytics module for event tracking and session management
    /// </summary>
    public class SolisAnalytics : MonoBehaviour
    {
        private TaskCompletionSource<bool> _trackEventTcs;
        private TaskCompletionSource<bool> _startSessionTcs;
        private TaskCompletionSource<bool> _endSessionTcs;

        #region JavaScript Imports

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void SolisSDK_TrackEvent(string eventName, string eventDataJson, Action<int> callback);

        [DllImport("__Internal")]
        private static extern void SolisSDK_StartSession(Action<int> callback);

        [DllImport("__Internal")]
        private static extern void SolisSDK_EndSession(Action<int> callback);
#endif

        #endregion

        #region Public API

        /// <summary>
        /// Track a custom event
        /// </summary>
        /// <param name="eventName">Event name (e.g., "level_complete")</param>
        /// <param name="eventData">Optional event data dictionary</param>
        /// <returns>True if event was tracked successfully</returns>
        public async Task<bool> TrackEventAsync(string eventName, Dictionary<string, object> eventData = null)
        {
            if (!SolisSDK.CheckInitialized("Analytics"))
                return false;

            if (string.IsNullOrEmpty(eventName))
            {
                Debug.LogError("[SolisSDK.Analytics] Event name is required");
                return false;
            }

#if UNITY_WEBGL && !UNITY_EDITOR
            _trackEventTcs = new TaskCompletionSource<bool>();

            try
            {
                string eventDataJson = eventData != null ? JsonUtility.ToJson(eventData) : "{}";
                SolisSDK_TrackEvent(eventName, eventDataJson, OnTrackEventComplete);
                return await _trackEventTcs.Task;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.Analytics] Track event error: {ex.Message}");
                return false;
            }
#else
            // Editor mode - log event
            await Task.Delay(100);
            string dataStr = eventData != null ? JsonUtility.ToJson(eventData) : "{}";
            Debug.Log($"[SolisSDK.Analytics] [Demo] Event tracked: {eventName} {dataStr}");
            return true;
#endif
        }

        /// <summary>
        /// Start an analytics session
        /// Automatically called on SDK init, but can be called manually
        /// </summary>
        /// <returns>True if session was started successfully</returns>
        public async Task<bool> StartSessionAsync()
        {
            if (!SolisSDK.CheckInitialized("Analytics"))
                return false;

#if UNITY_WEBGL && !UNITY_EDITOR
            _startSessionTcs = new TaskCompletionSource<bool>();

            try
            {
                SolisSDK_StartSession(OnStartSessionComplete);
                return await _startSessionTcs.Task;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.Analytics] Start session error: {ex.Message}");
                return false;
            }
#else
            // Editor mode
            await Task.Delay(100);
            Debug.Log("[SolisSDK.Analytics] [Demo] Session started");
            return true;
#endif
        }

        /// <summary>
        /// End an analytics session
        /// Should be called when player quits the game
        /// </summary>
        /// <returns>True if session was ended successfully</returns>
        public async Task<bool> EndSessionAsync()
        {
            if (!SolisSDK.CheckInitialized("Analytics"))
                return false;

#if UNITY_WEBGL && !UNITY_EDITOR
            _endSessionTcs = new TaskCompletionSource<bool>();

            try
            {
                SolisSDK_EndSession(OnEndSessionComplete);
                return await _endSessionTcs.Task;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.Analytics] End session error: {ex.Message}");
                return false;
            }
#else
            // Editor mode
            await Task.Delay(100);
            Debug.Log("[SolisSDK.Analytics] [Demo] Session ended");
            return true;
#endif
        }

        #endregion

        #region Callbacks

        [AOT.MonoPInvokeCallback(typeof(Action<int>))]
        private void OnTrackEventComplete(int success)
        {
            _trackEventTcs?.SetResult(success == 1);
        }

        [AOT.MonoPInvokeCallback(typeof(Action<int>))]
        private void OnStartSessionComplete(int success)
        {
            _startSessionTcs?.SetResult(success == 1);
        }

        [AOT.MonoPInvokeCallback(typeof(Action<int>))]
        private void OnEndSessionComplete(int success)
        {
            _endSessionTcs?.SetResult(success == 1);
        }

        #endregion

        #region Unity Lifecycle

        private void OnApplicationQuit()
        {
            // Auto-end session when game closes
            if (SolisSDK.IsInitialized)
            {
                _ = EndSessionAsync();
            }
        }

        #endregion
    }
}
