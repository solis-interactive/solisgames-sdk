using System;
using System.Collections.Generic;

namespace SolisGames.Models
{
    /// <summary>
    /// Tournament information
    /// </summary>
    [Serializable]
    public class Tournament
    {
        public string id;
        public string name;
        public string description;
        public string status; // pending, active, completed
        public string bracket_type; // single_elimination, double_elimination, swiss, round_robin
        public string start_time;
        public string end_time;
        public int max_participants;
        public int participant_count;
        public PrizePool prize_pool;
        public bool is_participant;
        public string participant_status;

        public override string ToString()
        {
            return $"Tournament({name}, {status}, {participant_count}/{max_participants})";
        }
    }

    /// <summary>
    /// Prize pool data
    /// </summary>
    [Serializable]
    public class PrizePool
    {
        public int coins;
        public Dictionary<string, object> items;
    }

    /// <summary>
    /// Tournament match data
    /// </summary>
    [Serializable]
    public class TournamentMatch
    {
        public string id;
        public string tournament_id;
        public int round;
        public string player1_id;
        public string player2_id;
        public string player1_username;
        public string player2_username;
        public string winner_id;
        public string status; // pending, in_progress, completed
        public float player1_score;
        public float player2_score;

        public override string ToString()
        {
            return $"Match(Round {round}: {player1_username} vs {player2_username})";
        }
    }

    /// <summary>
    /// Tournament bracket with all matches
    /// </summary>
    [Serializable]
    public class TournamentBracket
    {
        public string tournament_id;
        public List<TournamentMatch> matches;
        public string error;

        public override string ToString()
        {
            return $"Bracket({matches?.Count ?? 0} matches)";
        }
    }

    /// <summary>
    /// List of tournaments response
    /// </summary>
    [Serializable]
    public class TournamentList
    {
        public List<Tournament> tournaments;
        public string error;
    }
}
