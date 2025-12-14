using System;
using System.Collections.Generic;

namespace SolisGames.Models
{
    /// <summary>
    /// Achievement information
    /// </summary>
    [Serializable]
    public class Achievement
    {
        public string id;
        public string name;
        public string description;
        public string icon_url;
        public string rarity; // common, uncommon, rare, epic, legendary
        public int xp_reward;
        public bool unlocked;
        public string unlocked_at;
        public int progress; // 0-100
        public float unlock_rate; // Percentage of players who unlocked

        public override string ToString()
        {
            return $"Achievement({name}, {rarity}, {(unlocked ? "Unlocked" : $"{progress}%")})";
        }
    }

    /// <summary>
    /// List of achievements response
    /// </summary>
    [Serializable]
    public class AchievementList
    {
        public List<Achievement> achievements;
        public string error;
    }
}
