TShock - Chat Mod
==============
Chat Modding tools and filters!

What does it do?
==============
Chat Mod allows you to manage your TShock Server's chat with custom chat filters and tools, such as a mod-only chat filter.
As of 1.2.6, it now includes extra-fun Chat Commands.

Permissions:
==============
chat.mod - allows talking with the mod chat filter activated

chat.admin - allows the use of every /chat command

chat.stab - allows the use of /stab
chat.dedge - allows the use of /dedge
chat.tickle - allows the use of /tickle

Commands:
==============
**TSHOCK:**
/help chat - Shows Chat Mod's Help File, containing information on all the chat mods and filters

**CHATMOD:**
/chat **[switch]** - Changes the current chat filter. Available **switches**:
 - **default** - Resets chat mode
 - **mod** - Only players with the "chat.mod" permission are allowed to talk
 - **raw** - Sets chat to use Vanilla Terraria's "player: Text" format. This ignores color, prefixes and suffixes
 - **custom** *[prefix]* - Sets everyone's text to be sent with "*prefix*: Text" format
/stab *[player]* - Hits *player* for 250 damage and broadcasts "*player* just got stabbed!"
/dedge *[player]* - Hits *player* for 300 damage, *user* takes 200 damage, and broadcasts "*user* hits *player* with a life-risking strike!"
/tickle *[player]* - Freezes *player* for 15 seconds and broadcasts "*player* is under a freezing attack of tickles!"

Changelog:
==============
**1.2.6**
 - Added ChatCommands /stab, /dedge (double-edge), /tickle
 - /chat help is now integrated in TShock, by the use of /help chat; The new ChatCommands have their own /help text as well

**1.0.0:**
 - Initial Release

To Do:
==============
 - Death Message Disabler - Player-based chat filter for death messages
 - Slow Mode - Prevents spam by setting a max number of messages per player every x seconds