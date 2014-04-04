using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI;

namespace ChatMod
{
    public class ChatPlayer
    {
        public int Index { get; set; }
        public TSPlayer TSPlayer { get { return TShock.Players[Index]; } }
        public DateTime LastMessage { get; set; }
        //public bool DeathMode { get; set; }
        public int MessagesSent { get; set; }
        public List<ChatPlayer> IgnoredPlayers { get; set; }

        public ChatPlayer(int index)
        {
            this.Index = index;
            this.LastMessage = DateTime.UtcNow;
            //this.DeathMode = false;
            this.MessagesSent = 0;
            this.IgnoredPlayers = new List<ChatPlayer>();
        }
    }
}
