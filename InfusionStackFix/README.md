# InfusionStackFix

Changes the way infusion stacks are given. This mod will give you +1 max hp per infusion per kill.

Infusion will by default give you a maximum of +100 hp per stack. If you want to you can configure this value in the configuration file associated with this mod.

E.g. if you are holding 5 infusions in your inventory you will receive +5hp per kill until a maximum of +500 health (by default).

This mod also fixed a minor "exploit" where you can get more health stacks from an infusion than you are permitted by killing a group of enemies when you are near maximum stacks.

Enjoy,
Thanks

## New features

Update 1.1.0: Added new improved functionality for Engineers who use infusion. Engineer turrets now have a configuration which when enabled will allow turrets to start with the current bonus that the Engineer who deployed it currently has. E.g. if an Engineer currently has +100 life from an infusion whenver the engineer deploys a turret that turret will also start with +100 life.

Update 1.2.0: You can now set the maximum hp gain per orb. By default this value is 0 which acts as default as above (i.e. there is no limit). Setting this value to anything except 0 limits the hp gained per kill to this number if you have more infusion stacks than the maximum. E.g. if your maximum number is 10 and you have 15 infusion stacks you will gain 10hp per kill rather than 15hp as outlined above.

Update 1.3.0: There is a new configuration bool called LegacyInfusion. If you set this value to true it overrides MaxHpPerInfusionStack and instead you have no limit to the amount of hp you can gain.

## Installation:

Requires intallation of Bepinex and R2API. 

Place `InfusionStackFix.dll` inside of "/Risk of Rain 2/Bepinex/Plugins/"

## Contact

If you have any issues you can usually find me on the ROR2 modding discord (@Fluffatron). Please bear in mind that as with all mods here this is something I do in my spare time for fun so may not always be able to immediately fix issues that you come up with but I will endeavour to do my best. 

## Changelog:

-5.0.0
- Removed dependency to DeployableOwnerInformation
- Reworked code, should be more reliable and more performant
- Works on Artifacts release of RoR2 game

-4.0.0
- Official Bepinex5 release
- Includes all previous changes that were excluded in v3 (config etc)

v3.0.3
- Updated to Bepinex5 for official release

v3.0.2
- Fixed configuration section naming issue

v3.0.1
- Added dependency attribute for DeployableOwnerInformation

v3.0.0
- Rollback Bepinex5 early release for thunderstore

v2.0.0
- Updated to Bepinex5
- Removed unnecessary R2API dependency
- Early release awaiting BepInExPack

v1.3.0
- Removed cap on MaxHpPerInfusionStack
- Added LegacyInfusion bool

v1.2.0
- Added configuration value to set maximum gain per orb
- Tidy code. Renamed variables to make code more readable

v1.1.2
- Fixed a bug in which orbs would not spawn if you set maximum values above 100 because I hard coded a 100 for orb spawn. Thank you for the report @Deathawaits4

v1.1.1
- Updated IL to work with Sirens update (stfld numbers changed)
- Updated dependencies

v1.1.0
- Updated R2API dependency string
- Added TurretsReceiveBonusFromEngineer

v1.0.0
- Released
