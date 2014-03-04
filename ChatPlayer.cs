using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI;
using ChatMod;
using System.Timers;

namespace ChatMod
{
    public class ChatPlayer
    {
        public int Index { get; set; }
        public TSPlayer TSPlayer { get { return TShock.Players[Index]; } }
        //public DateTime LastMessage { get; set; }
        public bool DeathMode { get; set; }
        public int MessagesSent { get; set; }
        public bool Frozen { get; set; }
        public Timer Timer { get; set; }

        public ChatPlayer(int index)
        {
            this.Index = index;
            //this.LastMessage = LMessage
            this.DeathMode = false;
            this.MessagesSent = 0;
            this.Frozen = false;
            this.Timer = new Timer();
            this.Timer.Elapsed += Timer_Elapsed;
        }

        public void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            TSPlayer.All.SendMessage(this.TSPlayer.Name + " is no longer being tickled.", new Color(243, 112, 234));
            this.Timer.Stop();
        }
    }
}
