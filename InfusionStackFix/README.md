# InfusionStackFix

Changes the way infusion stacks are given. This mod will give you +1 max hp per infusion per kill.

Infusion will by default give you a maximum of +100 hp per stack. If you want to you can configure this value in the configuration file associated with this mod (set it to an int from 1-500)

E.g. if you are holding 5 infusions in your inventory you will receive +5hp per kill until a maximum of +500 health (by default).

This mod also fixed a minor "exploit" where you can get more health stacks from an infusion than you are permitted by killing a group of enemies when you are near maximum stacks.

Enjoy,
Thanks

Update 1.1.0: Added new improved functionality for Engineers who use infusion. Engineer turrets now have a configuration which when enabled will allow turrets to start with the current bonus that the Engineer who deployed it currently has. E.g. if an Engineer currently has +100 life from an infusion whenver the engineer deploys a turret that turret will also start with +100 life.

## Installation:

Requires intallation of Bepinex and R2API. 

Place `InfusionStackFix.dll` inside of "/Risk of Rain 2/Bepinex/Plugins/"

## Contact

If you have any issues you can usually find me on the ROR2 modding discord (@Fluffatron). Please bear in mind that as with all mods here this is something I do in my spare time for fun so may not always be able to immediately fix issues that you come up with but I will endeavour to do my best. 

## Changelog:

v1.1.0
- Updated R2API dependency string
- Added TurretsReceiveBonusFromEngineer

v1.0.0
- Released
