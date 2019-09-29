# ReEnergisedDrink

I recently was made aware that in the current state of the game that after the first energy drink it is strictly better to take goat hoof. That is you actually get more speed sprinting per statck from a goat hoof than another energy drink. This simply will not do so I am releasing this as a quick patch balance fix.

NOTE: I am releasing this as "early access" v0.0.0 as I have done very little balance testing on this. I am releasing anyway because I am unlikely to find time at the moment to really test this out so I am releasing it with recommended values based on some very quick maths but nothing rigorous and nothing game-tested.

NOTE AGAIN: this is an IL hook on `CharacterBody.RecalculateStats` which is a high traffic area for mods. Until we get the bepinex update it is quite likely that this will break if you run a lot of other mods.

This initial balance patch changes it so you get less initial boost from your first Energy Drink but you get a greater scaling so it means that over time it becomes a better item and it is always the better pick for sprint speed over hoof.

Enjoy,
Thanks

## Installation:

Requires intallation of Bepinex and R2API. 

Place `ReEnergisedDrink.dll` inside of "/Risk of Rain 2/Bepinex/Plugins/"

## Contact

If you have any issues you can usually find me on the ROR2 modding discord (@Fluffatron). Please bear in mind that as with all mods here this is something I do in my spare time so may not always be able to immediately fix any issues that you come up with. 

## Changelog:

v0.0.0 
- "Early access" release