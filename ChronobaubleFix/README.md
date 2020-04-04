NOTE: Currently this will only work in single player! For more information see Known Issues section.

# ChronobaubleFix

One of the most complained about items in the game is Chronobauble. 

"IT'S USELESS!", "IT SHOULDN'T BE A GREEN ITEM!", "SUCH A WASTE!". All things you might commonly hear people say about Chronobauble, well not anymore.

With ChronobaubleFix the chronobauble slow debuff can now stack. For each chronobauble stack in your inventory you will be able to give enemies five debuff stacks. Each debuff stack will further reduce movement speed and attack speed of the enemy. 

As mentioned above this also buffs Chronobauble to also decrease attack speed of the enemy. In testing I found this to be a huge buff to the item and made it well worth picking up a few. It was so good that I actually halved my initial calculation for how much reduction it gives.

I hope you can give this item another chance and have some fun with Chronobauble in its new found glory.

Enjoy,
Thanks

## Configuration

This mod was written for Bepinex#150+ which is as yet unreleased. Therefore there is no configuration in this build until the new bepinex build is released. If you desperately want a build with configuration if you contact me on the RoR2Modding discord I will send it to you, you will need latest build of Bepinex (not on thunderstore) though for this to work.

Once the new Bepinex build arrives you will be able to configure:
- debuff slow coefficient
- maximum number of debuff stacks per item stack

## Installation:

Requires intallation of Bepinex and R2API. 

Place `Chronobauble.dll` inside of "/Risk of Rain 2/Bepinex/Plugins/"

## Known Issues

This does not work in multiplayer. It causes some unintended de-sync between non-host players. As a temporary solution to this issue I have added logic in which the hooks will only be subscribed to if you are playing alone.

## Contact

If you have any issues you can usually find me on the ROR2 modding discord (@Fluffatron). Please bear in mind that as with all mods here this is something I do in my spare time so may not always be able to immediately fix any issues that you come up with. 

## Changelog:

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