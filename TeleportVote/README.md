# TeleportVote
If you are like me and you are tired of waiting around awkwardly asking if everyone is ready to go, wasting time when you could be charging the teleporter, then this is the mod for you.

Especially useful in public lobbies!

This mod adds a restriction when interacting with the teleporter and with portals. The restriction is that you cannot activate the interactable until all living players have registered themselves as ready (either by interacting with teleporter or a chat command) or after 60s there is a brief window in which the teleporter will be available without all players saying they are ready. 

The intention of this brief window of time is that if you have someone who is frustratingly slow or afk or trolling you still have the chance to proceed without their permission. After this brief window of time has passed restrictions are reinstated and players have to vote again. Also, if a player dies then restrictions are reinstated and remaining living players must vote again.

One side effect of this mod is that Fireworks will no longer trigger when interacting with teleporters. This is something I picked up in testing where after a player has registered as ready they may still interact with the teleporter but without proceeding so you can infinitely spam fireworks. This is why it is disabled.

Please let me know what you think, there is still plenty more which can be change or improved. I hope you enjoy it and find it as useful I do.

Enjoy,
Thanks

## Chat Commands:

You can use chat to register yourself as ready. This will not activate the teleporter but by flagging yourself as ready it will allow other players near the teleporter to start.

The chat command works simply by typing a message as normal. i.e. Press Enter -> Type message.

The recommended command is "r". i.e. Press Enter -> type "r" -> Press Enter again to send message and this will register yourself as ready.

Accepted chat strings are: "r", "rdy", "ready", "y", "go"
Note: these are not case sensitive
Note also: this does not activate the teleporter. You still have to hit the teleporter after everyone is ready!

## Installation:

Requires Bepinex, R2API. 
(NOTE: this requires latest version of R2API which is not released on thunderstore yet. You can get the .dll files from modding discord or wait for the official release.)

Place `TeleportVote.dll` inside of "/Risk of Rain 2/Bepinex/Plugins/"

## Upcoming Features:

Slightly exciting stuff
- Configuration maybe. Undecided on what I think should and shouldn't be allowed to be configurable.
- Highlight or ping the teleporter whilst restriction countdown is active.

## Issues:

- Exploitative use of fireworks on teleporter. This has been prevented by disabled fireworks trigger on this type of interactable.
- In one test game whilst using no_enemies from RoR2 cheats we found that the teleporter got stuck at 99%. I have no idea why and have not been able to reproduce this since. I do not beleive this has anything to do with this mod but noting this issue here incase it is observed again. If you see it please let me know, including steps to reproduce.

## Changelog:

v1.0.1 
- Released

## Credits:

Credit to [paddywan](https://thunderstore.io/package/paddywan/) who guided me through making the IL hook for fireworks.
Credit to [wildbook](https://github.com/wildbook/R2Mods) who helped me sort out an issue where I had not worked out how to properly get the NetworkUserId from Interactor

Thanks for testing help from everyone at the Queetle Been discord and also Elysium.