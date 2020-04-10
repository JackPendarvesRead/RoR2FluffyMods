NOTE: Currently this will only work in single player! For more information see Known Issues section.

# ChronobaubleFix

One of the most complained about items in the game is Chronobauble. 

"IT'S USELESS!", "IT SHOULDN'T BE A GREEN ITEM!", "SUCH A WASTE!". All things you might commonly hear people say about Chronobauble, well not anymore.

With ChronobaubleFix the chronobauble slow debuff can now stack. For each chronobauble stack in your inventory you will be able to give enemies a number debuff stacks which is configured in mod configuration file (default 3). Each debuff stack will further reduce movement speed and attack speed of the enemy. 

As mentioned above this also buffs Chronobauble to also decrease attack speed of the enemy. In testing I found this to be a huge buff to the item and made it well worth picking up a few. It was so good that I actually more than halved my initial calculation for how much reduction it gives. I think this is still a very powerful item so it is fully configurable using the mod configuration file.

I hope you can give this item another chance and have some fun with Chronobauble in its new found glory.

Enjoy,
Thanks

## Configuration

It is strongly recommended that you use BepinexConfigurationManager when changing any configuration values. The default key to open menu to change configuration is F1.

ChronobaubleFixEnabled - enables/disables this mod
DebuffStacksPerItemStack - The number of debuffs that can stack per chronobauble in your inventory
DebuffDuration - Change the length of time the debuff lasts on an enemy
IncreasedDebuffDurationPerStack - If enabled it increases the duration of debuff for each chronobauble stack over 1
SlowScalingCoefficient - The amount that enemies movement and attack is scaled to. The higher this number the slower enemies will be with debuffs.

## Installation:

Requires intallation of Bepinex, R2API and FluffyLabsConfigManagerTool (please see dependencies).

Place `Chronobauble.dll` inside of "/Risk of Rain 2/Bepinex/Plugins/"

## Known Issues

This does not work in multiplayer. It causes some unintended de-sync between non-host players. As a temporary solution I have added logic which will automatically disable this mod logic and restore chronobauble to default.

## Contact

If you have any issues you can usually find me on the ROR2 modding discord (@Fluffatron). Please bear in mind that as with all mods here this is something I do in my spare time so may not always be able to immediately fix any issues that you come up with. 

## Changelog:

v3.0.0
- Reworked mod for Artifacts update
- Created an entirely new debuff which will be used instead of modifying the existing debuff
- Added configuration option to change length of time debuff lasts
- Added configuration option to add bonus debuff length for each chronobauble stack over 1
- Improved logic which should give a more smooth enable/disable

v2.0.2
- Tidy up of hook logic
- Edit readme to make more clear the known issues

v2.0.1
- ReadMe update

v2.0.0
- Bepinex5 Update
- Reorganised hook subscription to force it to only subscribe in single player game
- Removed R2API dependency
- Added logic for single player only (temp fix)

v1.0.0
- Released