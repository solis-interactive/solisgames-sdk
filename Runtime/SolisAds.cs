using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;

namespace SolisGames
{
    /// <summary>
    /// Ads module for monetization
    /// Supports rewarded, interstitial, and banner ads
    /// </summary>
    public class SolisAds : MonoBehaviour
    {
        private TaskCompletionSource<bool> _rewardedTcs;
        private TaskCompletionSource<bool> _interstitialTcs;
        private TaskCompletionSource<bool> _bannerTcs;

        #region JavaScript Imports

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void SolisSDK_ShowRewardedAd(string placement, Action<int> callback);

        [DllImport("__Internal")]
        private static extern void SolisSDK_ShowInterstitialAd(string placement, Action<int> callback);

        [DllImport("__Internal")]
        private static extern void SolisSDK_ShowBannerAd(string placement, string position, Action<int> callback);

        [DllImport("__Internal")]
        private static extern void SolisSDK_HideBannerAd();
#endif

        #endregion

        #region Public API

        /// <summary>
        /// Show a rewarded ad
        /// Player must watch the ad to receive reward
        /// </summary>
        /// <param name="placement">Ad placement ID (default: "default")</param>
        /// <returns>True if player watched the ad and should receive reward</returns>
        public async Task<bool> ShowRewardedAsync(string placement = "default")
        {
            if (!SolisSDK.CheckInitialized("Ads"))
                return false;

#if UNITY_WEBGL && !UNITY_EDITOR
            _rewardedTcs = new TaskCompletionSource<bool>();

            try
            {
                SolisSDK_ShowRewardedAd(placement, OnRewardedAdComplete);
                return await _rewardedTcs.Task;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.Ads] Rewarded ad error: {ex.Message}");
                return false;
            }
#else
            // Editor mode - simulate ad
            Debug.Log($"[SolisSDK.Ads] [Demo] Showing rewarded ad: {placement}");
            await Task.Delay(2000); // Simulate ad duration
            Debug.Log("[SolisSDK.Ads] [Demo] Rewarded ad completed - granting reward");
            return true;
#endif
        }

        /// <summary>
        /// Show an interstitial ad
        /// Full-screen ad shown between game actions
        /// </summary>
        /// <param name="placement">Ad placement ID (default: "default")</param>
        /// <returns>True if ad was shown successfully</returns>
        public async Task<bool> ShowInterstitialAsync(string placement = "default")
        {
            if (!SolisSDK.CheckInitialized("Ads"))
                return false;

#if UNITY_WEBGL && !UNITY_EDITOR
            _interstitialTcs = new TaskCompletionSource<bool>();

            try
            {
                SolisSDK_ShowInterstitialAd(placement, OnInterstitialAdComplete);
                return await _interstitialTcs.Task;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.Ads] Interstitial ad error: {ex.Message}");
                return false;
            }
#else
            // Editor mode - simulate ad
            Debug.Log($"[SolisSDK.Ads] [Demo] Showing interstitial ad: {placement}");
            await Task.Delay(1500);
            Debug.Log("[SolisSDK.Ads] [Demo] Interstitial ad completed");
            return true;
#endif
        }

        /// <summary>
        /// Show a banner ad
        /// Small ad displayed at top or bottom of screen
        /// </summary>
        /// <param name="placement">Ad placement ID (default: "default")</param>
        /// <param name="position">Banner position: "top" or "bottom" (default: "bottom")</param>
        /// <returns>True if banner was shown successfully</returns>
        public async Task<bool> ShowBannerAsync(string placement = "default", string position = "bottom")
        {
            if (!SolisSDK.CheckInitialized("Ads"))
                return false;

            if (position != "top" && position != "bottom")
            {
                Debug.LogWarning($"[SolisSDK.Ads] Invalid banner position '{position}', using 'bottom'");
                position = "bottom";
            }

#if UNITY_WEBGL && !UNITY_EDITOR
            _bannerTcs = new TaskCompletionSource<bool>();

            try
            {
                SolisSDK_ShowBannerAd(placement, position, OnBannerAdComplete);
                return await _bannerTcs.Task;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SolisSDK.Ads] Banner ad error: {ex.Message}");
                return false;
            }
#else
            // Editor mode - simulate banner
            Debug.Log($"[SolisSDK.Ads] [Demo] Showing banner ad: {placement} at {position}");
            await Task.Delay(500);
            return true;
#endif
        }

        /// <summary>
        /// Hide the banner ad
        /// </summary>
        public void HideBanner()
        {
            if (!SolisSDK.CheckInitialized("Ads"))
                return;

#if UNITY_WEBGL && !UNITY_EDITOR
            SolisSDK_HideBannerAd();
#else
            Debug.Log("[SolisSDK.Ads] [Demo] Hiding banner ad");
#endif
        }

        #endregion

        #region Callbacks

        [AOT.MonoPInvokeCallback(typeof(Action<int>))]
        private void OnRewardedAdComplete(int rewarded)
        {
            _rewardedTcs?.SetResult(rewarded == 1);
        }

        [AOT.MonoPInvokeCallback(typeof(Action<int>))]
        private void OnInterstitialAdComplete(int shown)
        {
            _interstitialTcs?.SetResult(shown == 1);
        }

        [AOT.MonoPInvokeCallback(typeof(Action<int>))]
        private void OnBannerAdComplete(int shown)
        {
            _bannerTcs?.SetResult(shown == 1);
        }

        #endregion
    }
}
