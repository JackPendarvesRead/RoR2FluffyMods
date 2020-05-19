# RiskOfVampirism

Power at any price. Make a sacrifice to gain powerful vampiric abilities.  

As a vampire you gain a powerful lifesteal effect but in order to stay alive you must feed. The more you kill the more powerful you become but also the more your hunger grows. 

This mod changes the way you play in which you must be constantly searching for enemies to leech off or face an inevitable death. It changes the pacing in a way that forces you to move quickly and keep killing. You gain health for each kill, which does make you more powerful but degeneration is based on your maximum health so the more you kill the stronger your hunger is and the more you have to kill to stay alive. Can you beat it?

Vampire abilities:
- % lifesteal on hitting enemies  
- Gain max hp for each enemy kill
- Lose all regeneration
- Life degenerates over time

Any survivor class is viable to be a vampire. Try them all, configure the values to your taste. Happy feeding. 

Good luck. BLEH BLEH BLEHHH!

Enjoy,
Thanks

# Configuration Presets

I have prepared some configuration presets to get you started. To use them simply press F1 to open configuration manager (or whatever keybind you have assigned to open it), navigate to RiskOfVampirism configuration section and you will see a preset section which has the following buttons. Simply click on one of the buttons and the settings will be applied.

### Default 
Sets some defaults which I believe gives an interesting balanced experience

### Relaxed 
A slow degeneration with a minor lifesteal for those who want to use it but don't want to have too major change to the game

### Dracula 
An extremely fast degeneration with exetremely strong lifesteal. One of my favourite modes and one of the most challenging.

### BloodGolem
A very slow degeneration with a very low lifesteal but each kill gives you a huge bonus to MaxHP. The catch is that as you gain more HP the faster you degenerate so you will eventually collapse under your own weight if you do not find a way to increase your survivability.

# Configurations

![Configurations](https://i.imgur.com/cspudFt.png "Configurations")

**IsVampire** - Enables/Disables the mod. 

**TurretsTransferLifeToOwner** - if this is enabled engineer turrets will lifesteal and give life to its owner

**GainMaxHealthGainOnKill** - Enable to gain health on each kill. If enabled the number is added to your max health for each kill you make.

**LifeLeech** - the % lifesteal value your attacks will give you

**DegenerationThreshold** - Your life will stop degenerating after hitting this number. Set to 0 if you want degeneration to kill you.

**HealthDecayTime** - The time (in seconds) for your life to decay from maximum to 0

**SurvivorCoefficients** - These numbers can be changed to individually change how much lifesteal a survivor gets without having to change the base configuration. In short the lifesteal will be multiplied by this value for the respective survivor.

(* these will only be shown if advanced settings are shown)

# A note on balance

I am quite aware that this is not a fine tuned and balanced experience. For this reason I have supplied you with configurations to customise your experience. If you turn on Advanced Settings in the configuration manager you will also have access to a slider which will buff/nerf each survivor independently!

This is work in progress so if you have any feedback or suggestions they are very welcome.

## Installation:

Requires intallation of Bepinex and all additional dependencies. 

Place `RiskOfVampirism.dll` inside of "/Risk of Rain 2/Bepinex/Plugins/"

## Contact

If you have any issues you can usually find me on the ROR2 modding discord (@Fluffatron). Please bear in mind that as with all mods here this is something I do in my spare time so may not always be able to immediately fix any issues that you come up with. 

## Changelog:

v3.0.0
- Updated to work on Artifacts release of game
- Removed deprecated dependencies
- Added some additional configuration options including presets
- Fixed some faulty logic

v2.1.0
- Official Bepinex5 release
- Added extra configuration options

v2.0.0
- Remove R2API dependency
- Refactor code
- Added new configuration options
- Removed obsolete console commands
- Added link from turret to engibody to allow lifesteal from turrets

v1.0.1
- Updated R2API dependency string

v1.0.0 
- Released