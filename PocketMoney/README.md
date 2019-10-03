# PocketMoney

Gives players extra money at the start of each stage. The amount of money given can be configured in the configuration file.

The formula for extra money given at the start of stage is: `FlatBonus + WeightedBonus * CostOfChest`

You can customise how much pocketmoney you think you deserve with console commands or you can set it in config file. 

Default Values:
FlatBonus = 0, WeightedBonus = 1

These default values give everyone the cost on 1 small chest extra at the start of round as a quickstart bonus.

Update v2.0.0: you may now set a LatestStageToReceiveMoney. Setting this to anything above 0 will set the last stage in which you receive your bonus. i.e. if you only want to receive the bonus for the first 4 stages set this number to 4. Leave this number set to 0 if you do not wish to set a maximum.

Enjoy,
Thanks

## Installation:

Requires intallation of Bepinex and R2API. 

Place `PocketMoney.dll` inside of "/Risk of Rain 2/Bepinex/Plugins/"

## Contact

If you have any issues you can usually find me on the ROR2 modding discord (@Fluffatron). Please bear in mind that as with all mods here this is something I do in my spare time so may not always be able to immediately fix any issues that you come up with. 

## Changelog:
v 2.0.0
- Updated for Bepinex5
- Removed obsolete console commands
- Added new configuration to allow you to choose a maximum stage in which you wish to receive bonus

v1.1.2
- Updated R2API dependency string

v1.1.1
- Updated this readme file which I forgot to change for v1.1.0

v1.1.0
- Added feature to give money based on weighted difficulty
- Added configs and changed console commands to coincide with above change

v1.0.0 
- Released