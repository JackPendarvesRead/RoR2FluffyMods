# MeteorPunishment

This was originally designed as a piece of top secret anti-trolling tech. This is no longer the case though...

This is a mod that targets a specific player who uses the meteor lunar item. By default it is set to hit whoever triggers the meteor item (i.e. originally designed that if someone tries to troll the team they just kill themselves)

Now though you can just set the target to whomever you choose. Use the console commands to set the target. 

Unfortunately, as well as targetting the player there appears to be a number of random meteors which spawn just anywhere on the map which I have not managed to stop. This could be a future improvement to remove these maybe.

Bear in mind - the more targets there are on the map, the more meteors are going to spawn, the more meteors going to hit your friend. So using this early on when there aren't many enemies you won't see much effect but later in the game when there are more enemies you will see it!

Good luck.

Enjoy,
Thanks

## Console Commands:

`meteor_default`: sets meteor back to default configuration of hitting whoever triggered the meteor

`meteor_list`: gets a list of players and their indexes to help you with targetting

`meteor_set`: sets the custom target of your meteors.  args[0]=(int)playerIndex

E.g. type `meteor_get` into the console to see that your friend you want to target is at index `2`. Then type `meteor_set 2` to set them as the target.


## Installation:

Requires intallation of Bepinex and R2API. 

Place `MeteorPunishment.dll` inside of "/Risk of Rain 2/Bepinex/Plugins/"

## Changelog:

v1.0.0
- Released