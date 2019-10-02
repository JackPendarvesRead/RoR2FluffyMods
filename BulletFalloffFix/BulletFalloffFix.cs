using BepInEx;
using BepInEx.Configuration;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BulletFalloffFix
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.BulletFalloffFix", "BulletFalloffFix", "2.0.0")]
    public class BulletFalloffFix : BaseUnityPlugin
    {
        private static ConfigEntry<float> FallOffStartDistance;
        private static ConfigEntry<float> FallOffEndDistance;

        public void Awake()
        {
            const string falloffDistanceHeading = "Falloff Distance";

            FallOffStartDistance = Config.AddSetting<float>(
                new ConfigDefinition(falloffDistanceHeading, nameof(FallOffStartDistance)),
                40f,
                new ConfigDescription(                
                    "Set the distance at which damage starts to fall off (default=25, recommended=40)"                
                    )
                );

            FallOffEndDistance = Config.AddSetting<float>(
                new ConfigDefinition(falloffDistanceHeading, nameof(FallOffEndDistance)),
                80f,
                new ConfigDescription(
                    "Set the distance at which damage reaches minimum (default=60, recommended=80)"                    
                    )
                );            

            
            On.RoR2.Console.Awake += (orig, self) =>
            {
                CommandHelper.RegisterCommands(self);
                orig(self);
            };

            IL.RoR2.BulletAttack.DefaultHitCallback += BulletAttack_DefaultHitCallback;
        }

        private void BulletAttack_DefaultHitCallback(MonoMod.Cil.ILContext il)
        {
            var c = new ILCursor(il);
            c.GotoNext(
                x => x.MatchLdcR4(0.5f),  //I know this could be more robust but for now this works...
                x => x.MatchLdcR4(60f),
                x => x.MatchLdcR4(25f)
                );
            c.Index++;
            c.RemoveRange(2);
            c.Emit(OpCodes.Ldc_R4, FallOffEndDistance.Value);
            c.Emit(OpCodes.Ldc_R4, FallOffStartDistance.Value);
        }

        
        [ConCommand(commandName = "falloff_set", flags = ConVarFlags.ExecuteOnServer, helpText = "")]
        private static void FalloffSet(ConCommandArgs args)
        {
            try
            {
                var function = args[0].ToLower().Trim();
                switch (function)
                {
                    case "default":
                    case "d":
                        FallOffStartDistance.Value = BulletFalloffConstantValues.DefaultStart;
                        FallOffEndDistance.Value = BulletFalloffConstantValues.DefaultEnd;
                        Debug.Log("Default falloff values set");
                        break;

                    case "recommended":
                    case "r":
                        FallOffStartDistance.Value = BulletFalloffConstantValues.RecommendedStart;
                        FallOffEndDistance.Value = BulletFalloffConstantValues.RecommendedEnd;
                        Debug.Log("Recommended falloff values set");
                        break;

                    case "start":
                    case "s":
                        var startValue = float.Parse(args[1]);
                        FallOffStartDistance.Value = startValue;
                        Debug.Log($"Start falloff value set to {startValue}");
                        break;

                    case "end":
                    case "e":
                        var endValue = float.Parse(args[1]);
                        FallOffEndDistance.Value = endValue;
                        Debug.Log($"End falloff value set to {endValue}");
                        break;

                    default:
                        Debug.Log("Command not recognised. Allowed functions: s, e, r, d");
                        break;
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }
        }
    }
}
