using System;
using System.Collections.Generic;

namespace SolisGames.Models
{
    /// <summary>
    /// Friend information with presence
    /// </summary>
    [Serializable]
    public class Friend
    {
        public string id;
        public string username;
        public string avatar_url;
        public FriendPresence presence;

        public override string ToString()
        {
            return $"Friend({username}, {presence?.status ?? "offline"})";
        }
    }

    /// <summary>
    /// Friend presence status
    /// </summary>
    [Serializable]
    public class FriendPresence
    {
        public string status; // online, away, offline, playing
        public string game_title;
        public string custom_status;
        public bool joinable;

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(game_title))
                return $"{status} - Playing {game_title}";
            return status;
        }
    }

    /// <summary>
    /// List of friends response
    /// </summary>
    [Serializable]
    public class FriendList
    {
        public List<Friend> friends;
        public string error;
    }
}
