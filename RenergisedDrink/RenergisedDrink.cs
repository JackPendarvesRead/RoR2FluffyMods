using BepInEx;
using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenergisedDrink
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.ReEnergisedDrink", "ReEnergisedDrink", "0.0.0")]
    public class BulletFalloffFix : BaseUnityPlugin
    {
        private static ConfigWrapper<float> EnergyDrinkBoost;
        private static ConfigWrapper<float> EnergyDrinkCoefficient;

        public void Awake()
        {
            EnergyDrinkBoost = Config.Wrap(
                "Energy Drink",
                "Boost",
                "Flat value added to sprint speed boost on first pickup of Energy Drink [default=1.0f, recommended=0.5f]",
                0.5f
                );

            EnergyDrinkCoefficient = Config.Wrap(
                "Energy Drink",
                "Coefficient",
                "Number determines scaling adding sprint speed per Energy Drink (default=0.2f, recommended=0.3f)",
                0.3f
                );

            IL.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private void CharacterBody_RecalculateStats(MonoMod.Cil.ILContext il)
        {
            var c = new ILCursor(il);
            c.GotoNext(x => x.MatchLdloc(17));
            c.GotoNext(x => x.MatchLdloc(17));
            c.Index--;
            c.Index--;
            c.RemoveRange(2);
            c.Emit(OpCodes.Ldc_R4, EnergyDrinkBoost.Value);
            c.Emit(OpCodes.Ldc_R4, EnergyDrinkCoefficient.Value);
        }
    }
}
