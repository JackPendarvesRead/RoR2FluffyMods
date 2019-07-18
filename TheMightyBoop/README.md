# TheMightyBoop
Have you ever wanted to launch your enemies into space? Well now you can! Look no further than TheMightyBoop.

Did you think that Rex's sonic boom was too weedy for your liking? Never again! Set the configuration to your taste and boop your enemies away at supersonic speeds.

This mod turns Rex's Sonic Boom into a fully configurable sonic canon. Values can range from a nerf, to a slight buff, all the way up to ludicrous speed.

See the [introductory video](https://streamable.com/s9bxp) showing how silly things got during testing (and how silly things can get if you choose to configure it that way).

Let me know how it goes, if you want anything changed or improved or whatever else.

Note: this is a server-side mod. This means that if you are host then the same sonic boom configuration will be applied to all players in your lobby. There is no need for anyone else to install the mod.

Enjoy,
Thanks

## Console Commands:

`boop_get`: returns values that are currently set in configuration

`boop_set_default`: sets configuration to game base values

`boop_set_recommended`: sets configuration to our recommended values

`boop_set`: sets the configuration value of a govem configuration. Takes two arguements. args\[0\]=WrapperName, args\[1\]=Value(float)

Wrapper arguements: 
"airknockbackdistance / air / a"
"groundknockbackdistance / ground / g"
"liftvelocity / lift / l"
"maxdistance / distance / d"

E.g. To set MaxDistance to 100 you can type into the console `boop_set d 100`


## Installation:

Requires intallation of Bepinex and R2API. 

Place `TheMightyBoop.dll` inside of "/Risk of Rain 2/Bepinex/Plugins/"

## Upcoming Features:

Short term: 
- Nothing planned. Perhaps some extra console commands or configurable default values.

Long term:
- Thinking about making some sort of Rex minigame. Wait and see.

## Issues:

It is difficult to play when laughing too much. 
(no known issues)

## Changelog:

v1.0.2 
- Updated R2API dependency string

v1.0.1 
- Released