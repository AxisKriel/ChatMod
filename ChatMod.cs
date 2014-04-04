using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using ChatMod;

namespace ChatMod
{
    [ApiVersion(1, 15)]
    public class ChatMod : TerrariaPlugin
    {
        #region PluginInfo
        public ChatMod(Main game)
            : base(game)
        {
            base.Order = 0;
        }

        public override Version Version
        {
            get { return new Version(1, 3, 0); }
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
            get { return "Chat modding tools and commands!"; }
        }
        #endregion

        private ChatPlayer[] ChatPlayers = new ChatPlayer[256];

        public override void Initialize()
        {
            ServerApi.Hooks.ServerChat.Register(this, OnChat);
            ServerApi.Hooks.NetGreetPlayer.Register(this, OnJoin);
            ServerApi.Hooks.NetGetData.Register(this, OnGetData);
            ServerApi.Hooks.ServerLeave.Register(this, OnLeave);

            Commands.ChatCommands.Add(new Command("chat.admin", ChatModding, "chat")
            {
                HelpText = "Sets chat filters. Type /chat help for more info."
            });
            Commands.ChatCommands.Add(new Command("chat.ignore", DoIgnore, "ignore")
            {
                HelpText = "Ignores a player, preventing messages from being received. Usage: /ignore +|-<playername>"
            });
            
        }

        protected override void Dispose(bool disposing)
        {
            ServerApi.Hooks.ServerChat.Deregister(this, OnChat);
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnJoin);
            ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
            ServerApi.Hooks.ServerLeave.Deregister(this, OnLeave);
            base.Dispose(disposing);
        }

        // List of triggers
        private bool Moderated = false;
        private bool RawText = false;
        private bool CName = false;
        private string CPrefix = "";

        private void OnJoin(GreetPlayerEventArgs e)
        {
            if (TShock.Players[e.Who] != null)
            {
                UpdateList(e.Who);
            }
        }

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

        private void OnGetData(GetDataEventArgs data)
        {
            foreach (ChatPlayer player in ChatPlayers.Where<ChatPlayer>(ch => ch.IgnoredPlayers.Count > 1))
            {
                foreach (ChatPlayer plr in player.IgnoredPlayers)
                {
                    if (data.Msg.whoAmI == plr.Index)
                    {
                        data.Handled = true;
                        return;
                    }
                }
            }
        }

        private void OnLeave(LeaveEventArgs e)
        {
            if (TShock.Players[e.Who] != null && ChatPlayers[e.Who] != null)
            {
                UpdateList(e.Who, false);
            }
        }

        private void ChatModding(CommandArgs args)
        {
            TSPlayer ply = args.Player;
            if (args.Parameters.Count < 1 || args.Parameters.Count > 2)
            {
                ply.SendErrorMessage("Invalid syntax! Proper syntax: /chat <default|mod|raw|custom> [custom name]");
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
                default:
                    {
                        ply.SendErrorMessage("Invalid syntax! Proper syntax: /chat <efault|mod|raw|custom> [custom prefix]");
                        break;
                    }
            }
        }

        private void DoIgnore(CommandArgs args)
        {
            TSPlayer ply = args.Player;
            if (args.Parameters.Count < 1)
            {
                ply.SendErrorMessage("Invalid syntax! Proper syntax: /ignore +|-<playername>");
            }
            else
            {
                if (args.Parameters[0].ToLower() == "list")
	            {
		            ply.SendInfoMessage("[ChatMod] Ignore List: " +
                        string.Join(", ", ChatPlayers[ply.Index].IgnoredPlayers.Select<ChatPlayer, string>(p => p.TSPlayer.Name)));
                    return;
	            }
                string input = string.Join(" ", args.Parameters);
                var found = new List<TSPlayer>();
                string op = "+";
                if (!args.Parameters[0].StartsWith("+") || !args.Parameters[0].StartsWith("-"))
                {
                    found = TShock.Utils.FindPlayer(input);
                }
                else
                {
                    op = args.Parameters[0].Substring(0, 1);
                    if (op != "+" || op != "-")
                    {
                        ply.SendErrorMessage("[ChatMod] /ignore: Invalid operator! Available operators: +player (add), -player (del)");
                        return;
                    }
                    string subargs = input.Substring(1);
                    found = TShock.Utils.FindPlayer(subargs);
                }
                if (found.Count < 1)
                {
                    ply.SendErrorMessage("[ChatMod] /ignore: No players matched!");
                }
                else if (found.Count > 1)
                {
                    string names = string.Join(", ", found.Select<TSPlayer, string>(p => p.Name));
                    ply.SendErrorMessage("[ChatMod] /ignore: More than one player matched! Matches: " + names);
                }
                else if (found[0] == ply)
                {
                    ply.SendErrorMessage("[ChatMod] /ignore: You cannot ignore yourself!");
                }
                else if (op == "-")
                {
                    if (!ChatPlayers[ply.Index].IgnoredPlayers.Remove(ChatPlayers[found[0].Index]))
                        ply.SendErrorMessage("[ChatMod] /ignore: Player is not in your ignore list!");
                    else
                        ply.SendSuccessMessage("[ChatMod] /ignore: {0} is no longer being ignored!", found[0].Name);
                }
                else
                {
                    ChatPlayers[ply.Index].IgnoredPlayers.Add(ChatPlayers[found[0].Index]);
                    ply.SendSuccessMessage("[ChatMod] /ignore: Added {0} to your ignore list!", found[0].Name);
                }
            }
        }

        #region UTILS
        private void UpdateList(int id, bool add = true)
        {
            if (add)
            {
                ChatPlayers[id] = new ChatPlayer(id);
            }
            else
            {
                ChatPlayers[id] = null;
            }
        }
        #endregion
    }
}
