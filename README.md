TShock - Chat Mod
==============
Chat Modding tools and filters!

What does it do?
==============
Chat Mod allows you to manage your TShock Server's chat with custom chat filters and tools, such as a mod-only chat filter.

Permissions:
==============
chat.mod - allows talking with the mod chat filter activated

chat.admin - allows the use of every /chat command

Commands:
==============
/chat help - Shows Chat Mod's Help File, containing information on all the chat mods and filters

/chat **[switch]** - Changes the current chat filter. Available **switches**:
 - **default** - Resets chat mode
 - **mod** - Only players with the "chat.mod" permission are allowed to talk
 - **raw** - Sets chat to use Vanilla Terraria's "[player]: Text" format. This ignores color, prefixes and suffixes
 - **custom** *[prefix]* - Sets everyone's text to be sent with "[prefix]: Text" format

Changelog:
==============
**1.0.0:**
 - Initial Release

To Do:
==============
 - Death Message Disabler - Player-based chat filter for death messages
 - Slow Mode - Prevents spam by setting a max number of messages per player every x seconds
