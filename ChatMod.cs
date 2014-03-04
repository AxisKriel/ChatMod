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
            base.Order = int.MaxValue;
        }

        public override Version Version
        {
            get { return new Version(1, 2, 6); }
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

        private List<Item> BannedItems { get; set; }
        private List<ChatPlayer> ChatPlayer = new List<ChatPlayer>();

        public override void Initialize()
        {
            ServerApi.Hooks.ServerChat.Register(this, OnChat);
            ServerApi.Hooks.NetGreetPlayer.Register(this, OnJoin);
            //ServerApi.Hooks.ClientChatReceived.Register(this, OnChatReceive);
            //ServerApi.Hooks.ClientChat.Register(this, OnClientChat);

            Commands.ChatCommands.Add(new Command("chat.admin", ChatModding, "chat")
            {
                HelpText= "default - Removes the chat filter \nmod - Sets the chat filter to Moderated (only Spirit+ can talk) \nraw - Sets the chat filter to Vanilla's chat system \ncustom - Activates anonymity and sets everyone to speak under a custom prefix"
            });
            //Commands.ChatCommands.Add(new Command("chat.invmod", InvMod, "invmod"));

            //Chat Commands
            Commands.ChatCommands.Add(new Command("chat.stab", DoStab, "stab") { HelpText = "[ChatMod] Help: /stab <player> - Damages [player] for 250 damage" });
            Commands.ChatCommands.Add(new Command("chat.dedge", DoEdge, "dedge") { HelpText = "[ChatMod] Help: /dedge <player> - Damages [player] for 300 damage, and the user for 200 damage" });
            Commands.ChatCommands.Add(new Command("chat.tickle", DoTickle, "tickle") { HelpText = "[ChatMod] Help: /tickle <player> - Freezes [player] for 15 seconds" });
            //Commands.ChatCommands.Add(new Command("chat.debug", Debug, "debug"));
            //Commands.ChatCommands.Add(new Command("chat.raptor", Raptor, "raptor") { HelpText = "[ChatMod] Raptor: Functionality for Raptor Clients:" });
            
            
        }

        protected override void Dispose(bool disposing)
        {
            ServerApi.Hooks.ServerChat.Deregister(this, OnChat);
            ServerApi.Hooks.NetGreetPlayer.Deregister(this, OnJoin);
            //ServerApi.Hooks.ClientChatReceived.Deregister(this, OnChatReceive);
            //ServerApi.Hooks.ClientChat.Deregister(this, OnClientChat);
            base.Dispose(disposing);
        }

        // List of triggers
        private bool Moderated = false;
        private bool RawText = false;
        private bool CName = false;
        private string CPrefix = "";

        //private void Debug(CommandArgs args) //Disabled, not ready & includes Raptor test subject
        //{
        //    if (args.Parameters.Count < 1)
        //    {
        //        args.Player.SendErrorMessage("No type specified!");
        //        return;
        //    }
        //    string type = args.Parameters[0];
        //    switch (type)
        //    {
        //        case "chatplayer":
        //            {
        //                string chatplayers = "";
        //                foreach (ChatPlayer plr in ChatPlayer)
        //                {
        //                    chatplayers += "[" + plr.TSPlayer.Name + "]" + ", ";
        //                }
        //                args.Player.SendInfoMessage("Current ChatPlayers: " + chatplayers);
        //                break;
        //            }
        //        case "raptor":
        //            {
        //                if (args.Player.IsRaptor)
        //                {
        //                    args.Player.SendSuccessMessage("[ChatMod] Raptor: Client Check Successful!");
        //                    break;
        //                }
        //                else
        //                {
        //                    args.Player.SendErrorMessage("[ChatMod] Raptor: Client Check Failed!");
        //                    break;
        //                }
        //            }
        //        default:
        //            {
        //                args.Player.SendErrorMessage("Invalid Type!");
        //                break;
        //            }
        //    }
        //    return;
        //}

        private void OnJoin(GreetPlayerEventArgs args)
        {
            if (!TShock.Players[args.Who].IsLoggedIn)
            {
                return;
            }
            else
            {
                UpdateList(args.Who);
                return;
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

        private void DoStab(CommandArgs args)
        {
            string target = "";
            if (args.Parameters.Count < 1)
            {
                args.Player.SendErrorMessage("Invalid syntax! Proper syntax: /stab <player>");
                return;
            }

            if (args.Parameters.Count == 1)
            {
                target = args.Parameters[0];
            }
            else if (args.Parameters.Count > 1)
            {
                for (int i = 0; i < args.Parameters.Count; i++)
                {
                    target += args.Parameters[i];
                    if (args.Parameters.Count == i + 1)
                    {
                        break;
                    }
                    else
                    {
                        target += " ";
                    }
                }
            }

            List<TSPlayer> found = TShock.Utils.FindPlayer(target);
            if (found.Count < 1)
            {
                args.Player.SendErrorMessage("[ChatMod] Error: No player matched!");
                return;
            }
            else if (found.Count == 1)
            {
                found[0].DamagePlayer(250);
                Color color = new Color(133, 96, 155);
                TSPlayer.All.SendMessage(found[0].Name + " just got stabbed!", color);
                return;
            }
            else
            {
                args.Player.SendErrorMessage("[ChatMod] Error: More than one player matched!");
                return;
            }

        }

        private void DoEdge(CommandArgs args)
        {
            string target = "";
            if (args.Parameters.Count < 1)
            {
                args.Player.SendErrorMessage("Invalid syntax! Proper syntax: /dedge <player>");
                return;
            }

            if (args.Parameters.Count == 1)
            {
                target = args.Parameters[0];
            }
            else if (args.Parameters.Count > 1)
            {
                for (int i = 0; i < args.Parameters.Count; i++)
                {
                    target += args.Parameters[i];
                    if (args.Parameters.Count == i + 1)
                    {
                        break;
                    }
                    else
                    {
                        target += " ";
                    }
                }
            }

            List<TSPlayer> found = TShock.Utils.FindPlayer(target);
            if (found.Count < 1)
            {
                args.Player.SendErrorMessage("[ChatMod] Error: No player matched!");
                return;
            }
            else if (found.Count == 1)
            {
                found[0].DamagePlayer(300);
                args.Player.DamagePlayer(200);
                Color color = new Color(255, 255, 153);
                TSPlayer.All.SendMessage(string.Format("{0} hits {1} with a life-risking strike!", args.Player.Name, found[0].Name), color);
                args.Player.SendMessage("You have taken the recoil from the double-edged blade!", color);
                return;
            }
            else
            {
                args.Player.SendErrorMessage("[ChatMod] Error: More than one player matched!");
                return;
            }

        }

        private void DoTickle(CommandArgs args)
        {
            string target = "";
            if (args.Parameters.Count < 1)
            {
                args.Player.SendErrorMessage("Invalid syntax! Proper syntax: /tickle <player>");
                return;
            }

            if (args.Parameters.Count == 1)
            {
                target = args.Parameters[0];
            }
            else if (args.Parameters.Count > 1)
            {
                for (int i = 0; i < args.Parameters.Count; i++)
                {
                    target += args.Parameters[i];
                    if (args.Parameters.Count == i + 1)
                    {
                        break;
                    }
                    else
                    {
                        target += " ";
                    }
                }
            }

            List<TSPlayer> found = TShock.Utils.FindPlayer(target);
            if (found.Count < 1)
            {
                args.Player.SendErrorMessage("[ChatMod] Error: No player matched!");
                return;
            }
            else if (found.Count == 1)
            {
                ChatPlayer chy = ChatPlayer[found[0].Index];
                found[0].SetBuff(47, 900, false);
                Color color = new Color(243, 112, 234);
                TSPlayer.All.SendMessage(found[0].Name + " is under a freezing attack of tickles!", color);
                chy.Timer.Interval = 15000;
                chy.Timer.Start();
                return;
            }
            else
            {
                args.Player.SendErrorMessage("[ChatMod] Error: More than one player matched!");
                return;
            }

        }

        // >> Still to implement this, part of a InvMod to check for banned items <<
        //private bool AddBannedItem(string item)
        //{
        //    List<Item> itm = TShock.Utils.GetItemByIdOrName(item);
        //    if (itm == null)
        //    {
        //        return false;
        //    }
        //    if (itm.Count != 1)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        BannedItems.Add(itm[0]);
        //        return true;
        //    }
        //}

        //private void InvMod(CommandArgs args)
        //{
        //    TSPlayer ply = args.Player;
        //    if (args.Parameters[0].ToLower() == "add")
        //    {
        //        if (args.Parameters.Count != 2)
        //        {
        //            ply.SendErrorMessage("Invalid syntax! Proper syntax: /invmod add <itemname/itemid>");
        //            return;
        //        }
        //        if (AddBannedItem(args.Parameters[1]))
        //        {
        //            List<Item> result = TShock.Utils.GetItemByIdOrName(args.Parameters[1]);
        //            ply.SendSuccessMessage(string.Format("[InvMod] Success: Added {0} to the ban list!", result[0]));
        //            return;
        //        }
        //        else
        //        {
        //            ply.SendErrorMessage("[InvMod] Error: Could not find the specified item / More than one item matched!");
        //            return;
        //        }
        //    }
        //    else if (args.Parameters[0].ToLower() == "scan")
        //    {
        //        string inflist = "";
        //        foreach (Item item in ply.TPlayer.inventory)
        //        {
        //            foreach (Item listitem in BannedItems)
        //            {
        //                if (BannedItems.Contains(args.TPlayer.inventory[item.netID]))
        //                {
        //                    inflist += args.TPlayer.inventory[item.netID].name + " ";
        //                }
        //            }
        //        }
        //        if (inflist == "")
        //        {
        //            ply.SendInfoMessage("[InvMod] No banned items were found.");
        //            return;
        //        }
        //        else
        //        {
        //            ply.SendErrorMessage(string.Format("You have been disabled for using the following banned item(s): {1}", inflist));
        //            return;
        //        }
        //    }
        //    else
        //    {
        //        ply.SendErrorMessage("Invalid syntax! Proper syntax: /invmod <add/delete/scan/>
        //    }
        //}

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

        #region UTILS
        private void UpdateList(int id)
        {
            ChatPlayer.Add(new ChatPlayer(id));
            return;
        }
        #endregion
    }
}
