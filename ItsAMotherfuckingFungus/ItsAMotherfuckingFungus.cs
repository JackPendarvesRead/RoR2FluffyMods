using BepInEx;
using BepInEx.Configuration;
using MonoMod.Cil;
using RoR2;
using UnityEngine;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;

namespace ItsAMotherfuckingFungus
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.ItsAMotherfuckingFungus", "ItsAMotherfuckingFungus", "1.0.0")]
    public class ItsAMotherfuckingFungus : BaseUnityPlugin
    {
        public void Awake()
        {
            IL.RoR2.ChestBehavior.ItemDrop += ChestBehavior_ItemDrop;            
        }

        private void ChestBehavior_ItemDrop(ILContext il)
        {
            var c = new ILCursor(il);
            c.GotoNext(x => x.MatchRet());
            c.Index += 3;
            c.EmitDelegate<Func<PickupIndex, PickupIndex>>((dropPickup) =>
            {
                if (dropPickup.itemIndex == ItemIndex.Mushroom)
                {
                    //var item = dropPickup.itemIndex.ToString();
                    Message.SendToAll($"It's a motherfucking fungus!!", Colours.Green);
                }
                return dropPickup;
            });
        }
    }
}
