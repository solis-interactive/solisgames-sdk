using System;

namespace SolisGames.Models
{
    /// <summary>
    /// Represents user data from Solis Games platform
    /// </summary>
    [Serializable]
    public class UserData
    {
        public string id;
        public string username;
        public string email;
        public string avatar_url;
        public bool is_authenticated;
        public int total_xp;
        public int level;
        public string created_at;

        public override string ToString()
        {
            return $"User({username}, Level {level}, XP {total_xp})";
        }
    }
}
