# EngiShieldNotification

A mod that makes an attempt at notifying the player that the Engineer's bubble shield is about to go down. This gives you a three second warning when shield is about to go down and lets you plan accordingly.

By default the bubble shield makes a sound when it is destroyed. With this mod you will hear this sound in the last 3 seconds of the shield's lifetime.

i.e. The shield lifetime is 15s. You will hear a sound on 12s, 13s, 14s and finally the shield goes down on 15s.

Note: this is a client-side mod. Only players who have the mod installed will hear the additional sounds. You will be able to hear the sounds of all Engineer bubble shields in your game regardless of whether they have the mod installed.

As usual, do please let me know if you have any feedback or suggestions for improvements, bug fixes and features.

Enjoy,
Thanks

## Installation:

Requires intallation of Bepinex and R2API. 

Place `EngiShieldNotification.dll` inside of "/Risk of Rain 2/Bepinex/Plugins/FluffyMods"

## Upcoming Features:

- Different sounds? Maybe? I might add a configuration to choose from a selection of sounds of allow a custom soundString.
- If possible would like to change the volume of specific instance of the sound. 

## Issues:

- Bug in singleplayer where if you pause the game this will not stop the sound events from triggering. So you get the sounds going on pause. Not really an issue though, not one I care enough about to actually do anything with anyway.

## Changelog:

v1.0.2 
- Updated R2API dependency string

v1.0.1 
- Released
