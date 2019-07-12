# EngiShieldNotification

A mod that makes an attempt at notifying the player that the Engineer's bubble shield is about to go down. This gives you a three second warning when shield is about to go down and lets you plan accordingly.

By default the bubble shield makes a sound when it is destroyed. With this mod you will hear this sound in the last 3 seconds of the shield's lifetime.

i.e. The shield lifetime is 15s. You will hear a sound on 12s, 13s, 14s and finally the shield goes down on 15s.

You can set the volume of the notification sound in configuration file or on-the-go with a console command.

Note: this is a client-side mod. Only players who have the mod installed will hear the additional sounds. You will be able to hear the sounds of all Engineer bubble shields in your game regardless of whether they have the mod installed.

As usual, do please let me know if you have any feedback or suggestions for improvements, bug fixes and features.

Enjoy,
Thanks

## Console Commands:

`engishield_setvolume`: set the volume of the bubble shield notification sound args\[0\]=int value for volume (0=mute, 1=default, 2=2xdefault,etc... up to 4x)


## Installation:

Requires intallation of Bepinex and R2API. 

Place `EngiShieldNotification.dll` inside of "/Risk of Rain 2/Bepinex/Plugins/"

## Upcoming Features:

- Configurable sounds. Instead of using default engi bubble end sound choose from a list of in-game sounds you'd lie to hear instead.
- Long term: would like to add a custom sound instead of using just in-game sound
- Also longer term as I don't know how to do graphics very well: if possible would like to change the colour of the shield.

## Issues:

- Bug in singleplayer where if you pause the game this will not stop the sound events from triggering. So you get the sounds going on pause. Not really an issue though, not one I care enough about to actually do anything with anyway.

## Changelog:
v1.1.1
- Changed maximum volume to be 4

v1.1.0
- Added volume configuration for sound
- Added console command to set volume configuration value

v1.0.2 
- Updated R2API dependency string

v1.0.1 
- Released
