using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace ChatMod
{
    [ApiVersion(1, 15)]
    public class ChatMod : TerrariaPlugin
    {
        #region PluginInfo
        public ChatMod(Main game)
            : base(game)
        {
            base.Order = int.MaxValue;
        }

        public override Version Version
        {
            get { return new Version(1, 0, 0); }
        }

        public override string Name
        {
            get { return "Chat Mod"; }
        }

        public override string Author
        {
            get { return "Enerdy"; }
        }

        public override string Description
        {
            get { return "Chat modding tools!"; }
        }
        #endregion

        public override void Initialize()
        {
            ServerApi.Hooks.ServerChat.Register(this, OnChat);

            Commands.ChatCommands.Add(new Command("chat.admin", ChatModding, "chat"));
        }

        protected override void Dispose(bool disposing)
        {
            ServerApi.Hooks.ServerChat.Deregister(this, OnChat);
            base.Dispose(disposing);
        }

        // List of triggers
        private bool Moderated = false;
        private bool RawText = false;
        private bool CName = false;
        private string CPrefix = "";


        private void OnChat(ServerChatEventArgs e)
        {
            TSPlayer ply = TShock.Players[e.Who];
            if (e.Text.StartsWith("/"))
            {
                e.Handled = false;
            }
            else if (Moderated)
            {
                if (ply.Group.HasPermission("chat.mod") || ply.Group.HasPermission("chat.admin"))
                {
                    e.Handled = false;
                }
                else
                {
                    e.Handled = true;
                    ply.SendErrorMessage("You do not have the permission to talk!");
                }
            }
            else if (RawText)
            {
                e.Handled = true;
                TSPlayer.All.SendMessageFromPlayer(e.Text, 255, 255, 255, e.Who);
            }
            else if (CName)
            {
                e.Handled = true;
                TSPlayer.All.SendInfoMessage(CPrefix + ": " + e.Text);
            }
        }

        private void ChatModding(CommandArgs args)
        {
            TSPlayer ply = args.Player;
            if (args.Parameters.Count < 1 || args.Parameters.Count > 2)
            {
                ply.SendErrorMessage("Invalid syntax! Proper syntax: /chat <help or default|mod|raw|custom> [custom name]");
                return;
            }
            var trigger = args.Parameters[0].ToLower();
            switch (trigger)
            {
                case ("mod"):
                    {
                        Moderated = true;
                        RawText = false;
                        CName = false;
                        TSPlayer.All.SendInfoMessage("Chat mode has been set to Moderated! (Only Spirit+ can talk)");
                        break;
                    }
                case ("raw"):
                    {
                        RawText = true;
                        Moderated = false;
                        CName = false;
                        TSPlayer.All.SendInfoMessage("Chat mode has been set to Raw! (Vanilla Chat)");
                        break;
                    }
                case ("custom"):
                    {
                        if (args.Parameters.Count != 2)
                        {
                            ply.SendErrorMessage("Invalid syntax! Proper syntax: /chat custom <custom prefix>");
                            break;
                        }
                        else
                        {
                            CName = true;
                            Moderated = false;
                            RawText = false;
                            CPrefix = args.Parameters[1];
                            TSPlayer.All.SendInfoMessage("Chat mode has been set to Custom! (Anonymity)");
                            break;
                        }
                    }
                case ("default"):
                    {
                        Moderated = false;
                        RawText = false;
                        CName = false;
                        TSPlayer.All.SendInfoMessage("Chat mode has been set back to Default!");
                        break;
                    }
                case ("help"):
                    {
                        ply.SendInfoMessage("Chat Mod Help File");
                        ply.SendInfoMessage("default - Removes the chat filter");
                        ply.SendInfoMessage("mod - Sets the chat filter to Moderated (only Spirit+ can talk)");
                        ply.SendInfoMessage("raw - Sets the chat filter to Vanilla's chat system");
                        ply.SendInfoMessage("custom - Activates anonymity and sets everyone to speak under a custom prefix");
                        break;
                    }
                default:
                    {
                        ply.SendErrorMessage("Invalid syntax! Proper syntax: /chat <help or default|mod|raw|custom> [custom prefix]");
                        break;
                    }
            }
        }
    }
}
