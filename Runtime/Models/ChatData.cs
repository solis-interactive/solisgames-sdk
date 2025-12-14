using System;
using System.Collections.Generic;

namespace SolisGames.Models
{
    /// <summary>
    /// Chat message
    /// </summary>
    [Serializable]
    public class ChatMessage
    {
        public string id;
        public string channel_id;
        public string user_id;
        public string username;
        public string avatar_url;
        public string message;
        public string created_at;
        public bool flagged;
        public string flag_reason;

        public override string ToString()
        {
            return $"{username}: {message}";
        }
    }

    /// <summary>
    /// Chat history response
    /// </summary>
    [Serializable]
    public class ChatHistory
    {
        public List<ChatMessage> messages;
        public string error;
    }
}
