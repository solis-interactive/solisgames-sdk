using System;
using System.Collections.Generic;

namespace SolisGames.Models
{
    /// <summary>
    /// Result from submitting a score to a leaderboard
    /// </summary>
    [Serializable]
    public class LeaderboardSubmitResult
    {
        public bool success;
        public int rank;
        public float score;
        public string leaderboard_key;
        public bool flagged_for_review;
        public List<string> flags;

        public override string ToString()
        {
            return $"LeaderboardResult(Rank: {rank}, Score: {score}, Flagged: {flagged_for_review})";
        }
    }

    /// <summary>
    /// Leaderboard entry
    /// </summary>
    [Serializable]
    public class LeaderboardEntry
    {
        public int rank;
        public string user_id;
        public string username;
        public string avatar_url;
        public float score;
        public string created_at;
        public Dictionary<string, object> metadata;

        public override string ToString()
        {
            return $"#{rank} {username}: {score}";
        }
    }

    /// <summary>
    /// Leaderboard rankings response
    /// </summary>
    [Serializable]
    public class LeaderboardRankings
    {
        public List<LeaderboardEntry> entries;
        public int total_count;
        public string leaderboard_key;
        public string scope;
        public string error;

        public override string ToString()
        {
            return $"Leaderboard({leaderboard_key}, {entries?.Count ?? 0} entries)";
        }
    }
}
