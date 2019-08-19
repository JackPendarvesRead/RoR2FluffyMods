# TheMightyBoop

Have you ever wanted to launch your enemies into space? Well now you can! Look no further than TheMightyBoop.

Did you think that Rex's sonic boom was too weedy for your liking? Never again! Set the configuration to your taste and boop your enemies away at supersonic speeds.

This mod turns Rex's Sonic Boom into a fully configurable sonic canon. Values can range from a nerf, to a slight buff, all the way up to ludicrous speed.

See the [introductory video](https://streamable.com/s9bxp) showing how silly things got during testing (and how silly things can get if you choose to configure it that way).

Let me know how it goes, if you want anything changed or improved or whatever else.

Note: this is a server-side mod. This means that if you are host then the same sonic boom configuration will be applied to all players in your lobby. There is no need for anyone else to install the mod.

Update 27/07/19: it has come to my attention that the Clay Templar (ClayBruiser) actually inherits from Rex's sonic boom ability. This means that the Templar enemy will also inherit any values you set for Rex! I do think this is pretty funny so I havne't entirely removed this interaction but I have added a configuration so that you can choose if the you want the enemy Templar to be mighty or not. 

Do note that this is a quick fix, it may not be as robust as we'd like it to be. If you experience any weirdness do not hesistate to contact me and I'll see if I can fix it.

Enjoy,
Thanks

## Console Commands:

`boop_get`: returns values that are currently set in configuration

`boop_set_default`: sets configuration to game base values

`boop_set_recommended`: sets configuration to our recommended values

`boop_set`: sets the configuration value of a govem configuration. Takes two arguements. args\[0\]=WrapperName, args\[1\]=Value(float)

Wrapper arguements: 
`airknockbackdistance` / `air` / `a`
`groundknockbackdistance` / `ground` / `g`
`liftvelocity` / `lift` / `l`
`maxdistance` / `distance` / `d`

E.g. To set MaxDistance to 100 you can type into the console `boop_set d 100`

`boop_mightyclay`: sets whether Clay Templar enemies are mighty. If true Clay Templar inherit the values set for Rex. Accepted arguments: `true`, `t`, `1`, `false`, `f`, `0`


## Installation:

Requires intallation of Bepinex and R2API. 

Place `TheMightyBoop.dll` inside of "/Risk of Rain 2/Bepinex/Plugins/"

## Contact

If you have any issues you can usually find me on the ROR2 modding discord (@Fluffatron). Please bear in mind that as with all mods here this is something I do in my spare time so may not always be able to immediately fix any issues that you come up with. 

## Changelog:

v1.1.1
- Updated R2API dependency string

v1.1.0
- Added a condition to logic which prevents Clay Templar from inheritting the values you set for rex
- Added console command to make Clay Templar mighty or not

v1.0.3 
- Updated some information about mod

v1.0.2 
- Updated R2API dependency string

v1.0.1 
- Released