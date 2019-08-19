# PocketMoney

Gives players extra money at the start of each stage. The amount of money given can be configured in the configuration file.

The formula for extra money given at the start of stage is: `FlatBonus + WeightedBonus * CostOfChest`

You can customise how much pocketmoney you think you deserve with console commands or you can set it in config file. 

Default Values:
FlatBonus = 0, WeightedBonus = 1

These default values give everyone the cost on 1 small chest extra at the start of round as a quickstart bonus.

Enjoy,
Thanks

## Console Commands:

`pocket_setweighted`: sets the weighted extra money given at start of each stage. args\[0\]=Value(int)

`pocket_setflat`: sets the flat value for extra money given at the start of each stage. args\[0\]=Value(int) (i.e. if this is set to 50 you will gain 50 extra gold at the start of each stage)

e.g. type `pocket_setflat 100` into console to set the flat extra money players receive to 100

## Installation:

Requires intallation of Bepinex and R2API. 

Place `PocketMoney.dll` inside of "/Risk of Rain 2/Bepinex/Plugins/"

## Contact

If you have any issues you can usually find me on the ROR2 modding discord (@Fluffatron). Please bear in mind that as with all mods here this is something I do in my spare time so may not always be able to immediately fix any issues that you come up with. 

## Changelog:

v1.1.2
- Updated R2API dependency string

v1.1.1
- Updated this readme file which I forgot to change for v1.1.0

v1.1.0
- Added feature to give money based on weighted difficulty
- Added configs and changed console commands to coincide with above change

v1.0.0 
- Released