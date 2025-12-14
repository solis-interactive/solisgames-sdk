/**
 * Solis Games SDK - Unity WebGL JavaScript Bridge
 *
 * This plugin provides the bridge between Unity C# code and the Solis Games JavaScript SDK.
 * All functions use the mergeInto pattern to inject into Unity's JavaScript runtime.
 *
 * Callback Pattern:
 * - Function pointers are passed from C# using Action<int> or Action<string>
 * - Use dynCall('vi', callbackPtr, [intValue]) for int callbacks
 * - Use dynCall('vs', callbackPtr, [stringPtr]) for string callbacks
 * - String parameters from C# are converted using UTF8ToString(ptr)
 * - String returns to C# use allocateUTF8OnStack(string)
 */

mergeInto(LibraryManager.library, {

    /******************************************************************************
     * CORE SDK - Initialization and User Management
     ******************************************************************************/

    /**
     * Initialize the Solis Games SDK
     * @param {number} apiKeyPtr - Pointer to API key string
     * @param {number} gameIdPtr - Pointer to game ID string (optional)
     * @param {number} callbackPtr - Callback function pointer (returns 1 for success, 0 for failure)
     */
    SolisSDK_Init: function(apiKeyPtr, gameIdPtr, callbackPtr) {
        var apiKey = UTF8ToString(apiKeyPtr);
        var gameId = UTF8ToString(gameIdPtr);

        if (!window.SolisGames || !window.SolisGames.SDK) {
            console.error('[Solis Unity SDK] JavaScript SDK not loaded. Ensure solis-games-sdk-v1.js is included in your WebGL template.');
            dynCall('vi', callbackPtr, [0]);
            return;
        }

        var options = gameId ? { gameId: gameId } : {};

        window.SolisGames.SDK.init(apiKey, options)
            .then(function() {
                console.log('[Solis Unity SDK] Initialized successfully');
                dynCall('vi', callbackPtr, [1]);
            })
            .catch(function(error) {
                console.error('[Solis Unity SDK] Initialization failed:', error);
                dynCall('vi', callbackPtr, [0]);
            });
    },

    /**
     * Get current user data as JSON string
     * @param {number} gameObjectNamePtr - GameObject name for SendMessage callback
     * @param {number} callbackMethodPtr - Method name for SendMessage callback
     */
    SolisSDK_GetUser: function(gameObjectNamePtr, callbackMethodPtr) {
        var gameObjectName = UTF8ToString(gameObjectNamePtr);
        var callbackMethod = UTF8ToString(callbackMethodPtr);

        window.SolisGames.SDK.user.getUser()
            .then(function(user) {
                var userJson = JSON.stringify(user || {});
                SendMessage(gameObjectName, callbackMethod, userJson);
            })
            .catch(function(error) {
                var errorJson = JSON.stringify({ error: error.message });
                SendMessage(gameObjectName, callbackMethod, errorJson);
            });
    },

    /******************************************************************************
     * MONETIZATION - Ads Module
     ******************************************************************************/

    /**
     * Show rewarded ad
     * @param {number} placementPtr - Ad placement ID
     * @param {number} callbackPtr - Callback (1 if rewarded, 0 if not)
     */
    SolisSDK_ShowRewardedAd: function(placementPtr, callbackPtr) {
        var placement = UTF8ToString(placementPtr);

        window.SolisGames.SDK.ad.requestRewarded(placement)
            .then(function(result) {
                var rewarded = result && result.reward ? 1 : 0;
                dynCall('vi', callbackPtr, [rewarded]);
            })
            .catch(function(error) {
                console.error('[Solis Unity SDK] Rewarded ad error:', error);
                dynCall('vi', callbackPtr, [0]);
            });
    },

    /**
     * Show interstitial ad
     * @param {number} placementPtr - Ad placement ID
     * @param {number} callbackPtr - Callback (1 if shown, 0 if failed)
     */
    SolisSDK_ShowInterstitialAd: function(placementPtr, callbackPtr) {
        var placement = UTF8ToString(placementPtr);

        window.SolisGames.SDK.ad.requestInterstitial(placement)
            .then(function(result) {
                var shown = result && result.shown ? 1 : 0;
                dynCall('vi', callbackPtr, [shown]);
            })
            .catch(function(error) {
                console.error('[Solis Unity SDK] Interstitial ad error:', error);
                dynCall('vi', callbackPtr, [0]);
            });
    },

    /**
     * Show banner ad
     * @param {number} placementPtr - Ad placement ID
     * @param {number} positionPtr - Banner position (top/bottom)
     * @param {number} callbackPtr - Callback (1 if shown, 0 if failed)
     */
    SolisSDK_ShowBannerAd: function(placementPtr, positionPtr, callbackPtr) {
        var placement = UTF8ToString(placementPtr);
        var position = UTF8ToString(positionPtr);

        window.SolisGames.SDK.ad.requestBanner(placement, { position: position })
            .then(function(result) {
                var shown = result && result.shown ? 1 : 0;
                dynCall('vi', callbackPtr, [shown]);
            })
            .catch(function(error) {
                console.error('[Solis Unity SDK] Banner ad error:', error);
                dynCall('vi', callbackPtr, [0]);
            });
    },

    /**
     * Hide banner ad
     */
    SolisSDK_HideBannerAd: function() {
        if (window.SolisGames && window.SolisGames.SDK && window.SolisGames.SDK.ad.hideBanner) {
            window.SolisGames.SDK.ad.hideBanner();
        }
    },

    /******************************************************************************
     * ANALYTICS - Event Tracking
     ******************************************************************************/

    /**
     * Track custom event
     * @param {number} eventNamePtr - Event name
     * @param {number} eventDataPtr - Event data as JSON string
     * @param {number} callbackPtr - Callback (1 if tracked, 0 if failed)
     */
    SolisSDK_TrackEvent: function(eventNamePtr, eventDataPtr, callbackPtr) {
        var eventName = UTF8ToString(eventNamePtr);
        var eventDataJson = UTF8ToString(eventDataPtr);
        var eventData = eventDataJson ? JSON.parse(eventDataJson) : {};

        window.SolisGames.SDK.analytics.track(eventName, eventData)
            .then(function() {
                dynCall('vi', callbackPtr, [1]);
            })
            .catch(function(error) {
                console.error('[Solis Unity SDK] Analytics error:', error);
                dynCall('vi', callbackPtr, [0]);
            });
    },

    /**
     * Start analytics session
     * @param {number} callbackPtr - Callback (1 if started, 0 if failed)
     */
    SolisSDK_StartSession: function(callbackPtr) {
        window.SolisGames.SDK.analytics.startSession()
            .then(function() {
                dynCall('vi', callbackPtr, [1]);
            })
            .catch(function(error) {
                console.error('[Solis Unity SDK] Start session error:', error);
                dynCall('vi', callbackPtr, [0]);
            });
    },

    /**
     * End analytics session
     * @param {number} callbackPtr - Callback (1 if ended, 0 if failed)
     */
    SolisSDK_EndSession: function(callbackPtr) {
        window.SolisGames.SDK.analytics.endSession()
            .then(function() {
                dynCall('vi', callbackPtr, [1]);
            })
            .catch(function(error) {
                console.error('[Solis Unity SDK] End session error:', error);
                dynCall('vi', callbackPtr, [0]);
            });
    },

    /******************************************************************************
     * CLOUD SAVE - Save/Load Data
     ******************************************************************************/

    /**
     * Save data to cloud
     * @param {number} keyPtr - Save key
     * @param {number} dataPtr - Data as JSON string
     * @param {number} callbackPtr - Callback (1 if saved, 0 if failed)
     */
    SolisSDK_CloudSave_Save: function(keyPtr, dataPtr, callbackPtr) {
        var key = UTF8ToString(keyPtr);
        var dataJson = UTF8ToString(dataPtr);

        try {
            var data = JSON.parse(dataJson);

            window.SolisGames.SDK.cloudSave.save(key, data)
                .then(function() {
                    dynCall('vi', callbackPtr, [1]);
                })
                .catch(function(error) {
                    console.error('[Solis Unity SDK] Cloud save error:', error);
                    dynCall('vi', callbackPtr, [0]);
                });
        } catch (parseError) {
            console.error('[Solis Unity SDK] Cloud save JSON parse error:', parseError);
            dynCall('vi', callbackPtr, [0]);
        }
    },

    /**
     * Load data from cloud
     * @param {number} keyPtr - Save key
     * @param {number} gameObjectNamePtr - GameObject name for SendMessage callback
     * @param {number} callbackMethodPtr - Method name for SendMessage callback
     */
    SolisSDK_CloudSave_Load: function(keyPtr, gameObjectNamePtr, callbackMethodPtr) {
        var key = UTF8ToString(keyPtr);
        var gameObjectName = UTF8ToString(gameObjectNamePtr);
        var callbackMethod = UTF8ToString(callbackMethodPtr);

        window.SolisGames.SDK.cloudSave.load(key)
            .then(function(data) {
                var dataJson = JSON.stringify(data || null);
                SendMessage(gameObjectName, callbackMethod, dataJson);
            })
            .catch(function(error) {
                console.error('[Solis Unity SDK] Cloud load error:', error);
                var errorJson = JSON.stringify({ error: error.message });
                SendMessage(gameObjectName, callbackMethod, errorJson);
            });
    },

    /******************************************************************************
     * LEADERBOARDS - Submit Scores and Get Rankings
     ******************************************************************************/

    /**
     * Submit score to leaderboard
     * @param {number} leaderboardKeyPtr - Leaderboard identifier
     * @param {number} score - Score value
     * @param {number} metadataPtr - Metadata as JSON string (optional)
     * @param {number} gameObjectNamePtr - GameObject name for SendMessage callback
     * @param {number} callbackMethodPtr - Method name for SendMessage callback
     */
    SolisSDK_Leaderboard_Submit: function(leaderboardKeyPtr, score, metadataPtr, gameObjectNamePtr, callbackMethodPtr) {
        var leaderboardKey = UTF8ToString(leaderboardKeyPtr);
        var metadataJson = UTF8ToString(metadataPtr);
        var metadata = metadataJson ? JSON.parse(metadataJson) : {};
        var gameObjectName = UTF8ToString(gameObjectNamePtr);
        var callbackMethod = UTF8ToString(callbackMethodPtr);

        window.SolisGames.SDK.leaderboards.submit(leaderboardKey, { score: score, metadata: metadata })
            .then(function(result) {
                var resultJson = JSON.stringify(result);
                SendMessage(gameObjectName, callbackMethod, resultJson);
            })
            .catch(function(error) {
                console.error('[Solis Unity SDK] Leaderboard submit error:', error);
                var errorJson = JSON.stringify({ error: error.message });
                SendMessage(gameObjectName, callbackMethod, errorJson);
            });
    },

    /**
     * Get leaderboard rankings
     * @param {number} leaderboardKeyPtr - Leaderboard identifier
     * @param {number} scopePtr - Scope (global/daily/weekly)
     * @param {number} limit - Number of entries to fetch
     * @param {number} offset - Offset for pagination
     * @param {number} gameObjectNamePtr - GameObject name for SendMessage callback
     * @param {number} callbackMethodPtr - Method name for SendMessage callback
     */
    SolisSDK_Leaderboard_Get: function(leaderboardKeyPtr, scopePtr, limit, offset, gameObjectNamePtr, callbackMethodPtr) {
        var leaderboardKey = UTF8ToString(leaderboardKeyPtr);
        var scope = UTF8ToString(scopePtr);
        var gameObjectName = UTF8ToString(gameObjectNamePtr);
        var callbackMethod = UTF8ToString(callbackMethodPtr);

        window.SolisGames.SDK.leaderboards.get(leaderboardKey, { scope: scope, limit: limit, offset: offset })
            .then(function(result) {
                var resultJson = JSON.stringify(result);
                SendMessage(gameObjectName, callbackMethod, resultJson);
            })
            .catch(function(error) {
                console.error('[Solis Unity SDK] Leaderboard get error:', error);
                var errorJson = JSON.stringify({ error: error.message });
                SendMessage(gameObjectName, callbackMethod, errorJson);
            });
    },

    /**
     * Get nearby ranks on leaderboard
     * @param {number} leaderboardKeyPtr - Leaderboard identifier
     * @param {number} range - Number of entries above and below player
     * @param {number} gameObjectNamePtr - GameObject name for SendMessage callback
     * @param {number} callbackMethodPtr - Method name for SendMessage callback
     */
    SolisSDK_Leaderboard_GetNearby: function(leaderboardKeyPtr, range, gameObjectNamePtr, callbackMethodPtr) {
        var leaderboardKey = UTF8ToString(leaderboardKeyPtr);
        var gameObjectName = UTF8ToString(gameObjectNamePtr);
        var callbackMethod = UTF8ToString(callbackMethodPtr);

        window.SolisGames.SDK.leaderboards.nearby(leaderboardKey, { range: range })
            .then(function(result) {
                var resultJson = JSON.stringify(result);
                SendMessage(gameObjectName, callbackMethod, resultJson);
            })
            .catch(function(error) {
                console.error('[Solis Unity SDK] Leaderboard nearby error:', error);
                var errorJson = JSON.stringify({ error: error.message });
                SendMessage(gameObjectName, callbackMethod, errorJson);
            });
    },

    /******************************************************************************
     * TOURNAMENTS - Join and Manage Tournaments
     ******************************************************************************/

    /**
     * Join a tournament
     * @param {number} tournamentIdPtr - Tournament UUID
     * @param {number} callbackPtr - Callback (1 if joined, 0 if failed)
     */
    SolisSDK_Tournament_Join: function(tournamentIdPtr, callbackPtr) {
        var tournamentId = UTF8ToString(tournamentIdPtr);

        window.SolisGames.SDK.tournaments.join(tournamentId)
            .then(function() {
                dynCall('vi', callbackPtr, [1]);
            })
            .catch(function(error) {
                console.error('[Solis Unity SDK] Tournament join error:', error);
                dynCall('vi', callbackPtr, [0]);
            });
    },

    /**
     * List tournaments
     * @param {number} statusPtr - Filter by status (active/pending/completed)
     * @param {number} gameObjectNamePtr - GameObject name for SendMessage callback
     * @param {number} callbackMethodPtr - Method name for SendMessage callback
     */
    SolisSDK_Tournament_List: function(statusPtr, gameObjectNamePtr, callbackMethodPtr) {
        var status = UTF8ToString(statusPtr);
        var gameObjectName = UTF8ToString(gameObjectNamePtr);
        var callbackMethod = UTF8ToString(callbackMethodPtr);

        window.SolisGames.SDK.tournaments.list({ status: status })
            .then(function(tournaments) {
                var tournamentsJson = JSON.stringify(tournaments || []);
                SendMessage(gameObjectName, callbackMethod, tournamentsJson);
            })
            .catch(function(error) {
                console.error('[Solis Unity SDK] Tournament list error:', error);
                var errorJson = JSON.stringify({ error: error.message });
                SendMessage(gameObjectName, callbackMethod, errorJson);
            });
    },

    /**
     * Get tournament bracket
     * @param {number} tournamentIdPtr - Tournament UUID
     * @param {number} gameObjectNamePtr - GameObject name for SendMessage callback
     * @param {number} callbackMethodPtr - Method name for SendMessage callback
     */
    SolisSDK_Tournament_GetBracket: function(tournamentIdPtr, gameObjectNamePtr, callbackMethodPtr) {
        var tournamentId = UTF8ToString(tournamentIdPtr);
        var gameObjectName = UTF8ToString(gameObjectNamePtr);
        var callbackMethod = UTF8ToString(callbackMethodPtr);

        window.SolisGames.SDK.tournaments.getBracket(tournamentId)
            .then(function(bracket) {
                var bracketJson = JSON.stringify(bracket || {});
                SendMessage(gameObjectName, callbackMethod, bracketJson);
            })
            .catch(function(error) {
                console.error('[Solis Unity SDK] Tournament bracket error:', error);
                var errorJson = JSON.stringify({ error: error.message });
                SendMessage(gameObjectName, callbackMethod, errorJson);
            });
    },

    /******************************************************************************
     * ACHIEVEMENTS - Unlock and Track Progress
     ******************************************************************************/

    /**
     * Unlock an achievement
     * @param {number} achievementIdPtr - Achievement identifier
     * @param {number} callbackPtr - Callback (1 if unlocked, 0 if failed)
     */
    SolisSDK_Achievement_Unlock: function(achievementIdPtr, callbackPtr) {
        var achievementId = UTF8ToString(achievementIdPtr);

        window.SolisGames.SDK.achievements.unlock(achievementId)
            .then(function() {
                dynCall('vi', callbackPtr, [1]);
            })
            .catch(function(error) {
                console.error('[Solis Unity SDK] Achievement unlock error:', error);
                dynCall('vi', callbackPtr, [0]);
            });
    },

    /**
     * Update achievement progress
     * @param {number} achievementIdPtr - Achievement identifier
     * @param {number} current - Current progress value
     * @param {number} target - Target progress value
     * @param {number} callbackPtr - Callback (1 if updated, 0 if failed)
     */
    SolisSDK_Achievement_UpdateProgress: function(achievementIdPtr, current, target, callbackPtr) {
        var achievementId = UTF8ToString(achievementIdPtr);

        window.SolisGames.SDK.achievements.progress(achievementId, { current: current, target: target })
            .then(function() {
                dynCall('vi', callbackPtr, [1]);
            })
            .catch(function(error) {
                console.error('[Solis Unity SDK] Achievement progress error:', error);
                dynCall('vi', callbackPtr, [0]);
            });
    },

    /**
     * List all achievements
     * @param {number} gameObjectNamePtr - GameObject name for SendMessage callback
     * @param {number} callbackMethodPtr - Method name for SendMessage callback
     */
    SolisSDK_Achievement_List: function(gameObjectNamePtr, callbackMethodPtr) {
        var gameObjectName = UTF8ToString(gameObjectNamePtr);
        var callbackMethod = UTF8ToString(callbackMethodPtr);

        window.SolisGames.SDK.achievements.list()
            .then(function(achievements) {
                var achievementsJson = JSON.stringify(achievements || []);
                SendMessage(gameObjectName, callbackMethod, achievementsJson);
            })
            .catch(function(error) {
                console.error('[Solis Unity SDK] Achievement list error:', error);
                var errorJson = JSON.stringify({ error: error.message });
                SendMessage(gameObjectName, callbackMethod, errorJson);
            });
    },

    /******************************************************************************
     * FRIENDS - Social Features
     ******************************************************************************/

    /**
     * List friends
     * @param {number} gameObjectNamePtr - GameObject name for SendMessage callback
     * @param {number} callbackMethodPtr - Method name for SendMessage callback
     */
    SolisSDK_Friends_List: function(gameObjectNamePtr, callbackMethodPtr) {
        var gameObjectName = UTF8ToString(gameObjectNamePtr);
        var callbackMethod = UTF8ToString(callbackMethodPtr);

        window.SolisGames.SDK.friends.list()
            .then(function(friends) {
                var friendsJson = JSON.stringify(friends || []);
                SendMessage(gameObjectName, callbackMethod, friendsJson);
            })
            .catch(function(error) {
                console.error('[Solis Unity SDK] Friends list error:', error);
                var errorJson = JSON.stringify({ error: error.message });
                SendMessage(gameObjectName, callbackMethod, errorJson);
            });
    },

    /**
     * Add friend
     * @param {number} userIdPtr - User ID to add
     * @param {number} callbackPtr - Callback (1 if added, 0 if failed)
     */
    SolisSDK_Friends_Add: function(userIdPtr, callbackPtr) {
        var userId = UTF8ToString(userIdPtr);

        window.SolisGames.SDK.friends.add(userId)
            .then(function() {
                dynCall('vi', callbackPtr, [1]);
            })
            .catch(function(error) {
                console.error('[Solis Unity SDK] Friends add error:', error);
                dynCall('vi', callbackPtr, [0]);
            });
    },

    /**
     * Remove friend
     * @param {number} userIdPtr - User ID to remove
     * @param {number} callbackPtr - Callback (1 if removed, 0 if failed)
     */
    SolisSDK_Friends_Remove: function(userIdPtr, callbackPtr) {
        var userId = UTF8ToString(userIdPtr);

        window.SolisGames.SDK.friends.remove(userId)
            .then(function() {
                dynCall('vi', callbackPtr, [1]);
            })
            .catch(function(error) {
                console.error('[Solis Unity SDK] Friends remove error:', error);
                dynCall('vi', callbackPtr, [0]);
            });
    },

    /**
     * Get online friends
     * @param {number} gameObjectNamePtr - GameObject name for SendMessage callback
     * @param {number} callbackMethodPtr - Method name for SendMessage callback
     */
    SolisSDK_Friends_GetOnline: function(gameObjectNamePtr, callbackMethodPtr) {
        var gameObjectName = UTF8ToString(gameObjectNamePtr);
        var callbackMethod = UTF8ToString(callbackMethodPtr);

        window.SolisGames.SDK.friends.list()
            .then(function(friends) {
                var onlineFriends = (friends || []).filter(function(f) {
                    return f.presence && f.presence.status === 'online';
                });
                var friendsJson = JSON.stringify(onlineFriends);
                SendMessage(gameObjectName, callbackMethod, friendsJson);
            })
            .catch(function(error) {
                console.error('[Solis Unity SDK] Friends online error:', error);
                var errorJson = JSON.stringify({ error: error.message });
                SendMessage(gameObjectName, callbackMethod, errorJson);
            });
    },

    /**
     * Update presence status
     * @param {number} statusPtr - Status (online/away/offline/playing)
     * @param {number} metadataPtr - Metadata as JSON string (optional)
     * @param {number} callbackPtr - Callback (1 if updated, 0 if failed)
     */
    SolisSDK_Friends_UpdatePresence: function(statusPtr, metadataPtr, callbackPtr) {
        var status = UTF8ToString(statusPtr);
        var metadataJson = UTF8ToString(metadataPtr);
        var metadata = metadataJson ? JSON.parse(metadataJson) : {};

        window.SolisGames.SDK.presence.set(status, metadata)
            .then(function() {
                dynCall('vi', callbackPtr, [1]);
            })
            .catch(function(error) {
                console.error('[Solis Unity SDK] Presence update error:', error);
                dynCall('vi', callbackPtr, [0]);
            });
    },

    /******************************************************************************
     * CHAT - Messaging
     ******************************************************************************/

    /**
     * Send chat message
     * @param {number} channelIdPtr - Channel identifier
     * @param {number} messagePtr - Message text
     * @param {number} callbackPtr - Callback (1 if sent, 0 if failed)
     */
    SolisSDK_Chat_Send: function(channelIdPtr, messagePtr, callbackPtr) {
        var channelId = UTF8ToString(channelIdPtr);
        var message = UTF8ToString(messagePtr);

        window.SolisGames.SDK.chat.send(channelId, message)
            .then(function() {
                dynCall('vi', callbackPtr, [1]);
            })
            .catch(function(error) {
                console.error('[Solis Unity SDK] Chat send error:', error);
                dynCall('vi', callbackPtr, [0]);
            });
    },

    /**
     * Get chat history
     * @param {number} channelIdPtr - Channel identifier
     * @param {number} limit - Number of messages to fetch
     * @param {number} gameObjectNamePtr - GameObject name for SendMessage callback
     * @param {number} callbackMethodPtr - Method name for SendMessage callback
     */
    SolisSDK_Chat_GetHistory: function(channelIdPtr, limit, gameObjectNamePtr, callbackMethodPtr) {
        var channelId = UTF8ToString(channelIdPtr);
        var gameObjectName = UTF8ToString(gameObjectNamePtr);
        var callbackMethod = UTF8ToString(callbackMethodPtr);

        window.SolisGames.SDK.chat.getMessages(channelId, { limit: limit })
            .then(function(messages) {
                var messagesJson = JSON.stringify(messages || []);
                SendMessage(gameObjectName, callbackMethod, messagesJson);
            })
            .catch(function(error) {
                console.error('[Solis Unity SDK] Chat history error:', error);
                var errorJson = JSON.stringify({ error: error.message });
                SendMessage(gameObjectName, callbackMethod, errorJson);
            });
    }
});
