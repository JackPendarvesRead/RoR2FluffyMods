﻿using BepInEx;
using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenergisedDrink
{
    [BepInPlugin(PluginGuid, pluginName, pluginVersion)]
    public class BulletFalloffFix : BaseUnityPlugin
    {
        public const string PluginGuid = "com.FluffyMods." + pluginName;
        private const string pluginName = "ReEnergisedDrink";
        private const string pluginVersion = "1.0.1";

        private static ConfigEntry<float> EnergyDrinkBoost;
        private static ConfigEntry<float> EnergyDrinkCoefficient;

        public void Awake()
        {
            if (!RoR2Application.isModded)
            {
                RoR2Application.isModded = true;
            }

            EnergyDrinkBoost = Config.Bind<float>(
                "Energy Drink",
                "Boost",
                0.5f,
                new ConfigDescription(
                    "Flat value added to sprint speed boost on first pickup of Energy Drink [default=1.0f, recommended=0.5f]",
                    new AcceptableValueRange<float>(0f, 2f)
                    )
                );

            EnergyDrinkCoefficient = Config.Bind<float>(
                "Energy Drink",
                "Coefficient",
                0.3f,
                new ConfigDescription(
                    "Number determines scaling adding sprint speed per Energy Drink (default=0.2f, recommended=0.3f)",
                    new AcceptableValueRange<float>(0f, 2f)
                    )
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
