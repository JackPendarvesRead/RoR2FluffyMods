# DeployableOwnerInformation

This is just a very small and simple plugin which adds a component to the CharacterMaster GameObject for deployables. This was created in order to make a simple link between the deployable and the CharacterMaster which summoned it.

e.g. if you can get the  CharacterBody or CharacterMaster of an EngineerTurret you can easily link it back to the Engineer who spawned it.

This mod will not do anything noticable on its own but may be used as a dependency. You will need to install this mod if any mods you want to use have this listed as a dependency or if you want to use this in your mod development.

## Instructions for developers

As with any library or dependency you will need to reference the .dll file in your Visual Studio project. Once you have added your reference you will have access to the `OwnerInformation` class which you can use to manually get the component or you can use the extension methods in `DeployableOwnerInformation.Extension` to give you easy access.

Currently this `OwnerInformation` only contains `CharacterBody OwnerBody`. This can be expanded in the future if the needs arise.

## Why use the dependency?

Why not just write your own link? Well you can, but the point of making this its own plugin as a dependency is so that this shared functionality can be used accross a number of different mods without needing to keep adding components. This just creates one component and that's it, after that any pluging can access it without needing to add anything else.

## Example code

Below is an example from `RiskOfVampirism` of using this code. The following code allows a lifesteal effect for the Engineer whenever a turret he owns deals damage. This is only made possible by making this link between the turret and the Engineer made by this mod.

```cs
private void GlobalEventManager_onServerDamageDealt(DamageReport damageReport)
{
    var damageInfo = damageReport.damageInfo;
    if (IsVampire.Value
        && damageInfo != null
        && !damageInfo.procChainMask.HasProc(ProcType.HealOnHit))
    {
        var attacker = damageInfo.attacker?.GetComponent<CharacterBody>();
        if (TurretsTransferLifeToOwner.Value
			&& attacker != null
            && attacker.teamComponent.teamIndex == TeamIndex.Player
            && attacker.name.ToLower().Contains("turret"))
        {
            var owner = attacker.GetOwnerInformation().OwnerBody;
            var procChainMask = damageInfo.procChainMask;
            procChainMask.AddProc(ProcType.HealOnHit);
            var num = (double)owner.healthComponent.Heal(damageInfo.damage * Leech.Value, procChainMask, true);
        }                       
    }
}
```

## Installation:

Requires intallation of Bepinex. 

Place `DeployableOwnerInformation.dll` inside of "/Risk of Rain 2/Bepinex/Plugins/"

## Contact

If you have any issues you can usually find me on the ROR2 modding discord (@Fluffatron). Please bear in mind that as with all mods here this is something I do in my spare time so may not always be able to immediately fix any issues that you come up with. 

## Changelog:

v1.0.2
- Updated for Bep5
- Tidy code

v1.0.1
- Removed dependency error

v1.0.0 
- Released