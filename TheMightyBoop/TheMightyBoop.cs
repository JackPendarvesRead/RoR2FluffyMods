using BepInEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoR2;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using BepInEx.Configuration;
using UnityEngine;

namespace TheMightyBoop
{

    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.TheMightyBoop", "TheMightyBoop", "1.0.1")]
    public class RexMegaSonicBoop : BaseUnityPlugin
    {
        private static ConfigWrapper<float> AirKnockBackDistance;
        private static ConfigWrapper<float> GroundKnockBackDistance;
        private static ConfigWrapper<float> LiftVelocity;
        private static ConfigWrapper<float> MaxDistance;

        public void Awake()
        {
            string sectionName = "FireSonicBoom";

            #region ConfigSetup
            AirKnockBackDistance = Config.Wrap(
                                   sectionName,
                                   nameof(AirKnockBackDistance),
                                   $"Set how far you knock yourself back when you boop in mid-air. (Game default = {BoopConstants.AirKnockBackDistanceDefault},  Recommended = {BoopConstants.AirKnockBackDistanceRecommended})",
                                   BoopConstants.AirKnockBackDistanceRecommended
                                   );

            GroundKnockBackDistance = Config.Wrap(
                                   sectionName,
                                   nameof(GroundKnockBackDistance),
                                   $"Set how far you knock yourself back when you boop whilst on the ground. (Game default = {BoopConstants.GroundKnockBackDistanceDefault}, Recommended = {BoopConstants.GroundKnockBackDistanceRecommended})",
                                   BoopConstants.GroundKnockBackDistanceRecommended
                                   );

            MaxDistance = Config.Wrap(
                                   sectionName,
                                   nameof(MaxDistance),
                                   $"Set the horizontal distance which enemies should be knocked back by the boop. (Game default = {BoopConstants.MaxDistanceDefault}, Recommended = {BoopConstants.MaxDistanceRecommended})",
                                   BoopConstants.MaxDistanceRecommended
                                   );

            LiftVelocity = Config.Wrap(
                                   sectionName,
                                   nameof(LiftVelocity),
                                   $"Set the vertical lift of enemies affected by the boop. (Game default = {BoopConstants.LiftVelocityDefault}, Recommended = {BoopConstants.LiftVelocityRecommended})",
                                   BoopConstants.LiftVelocityRecommended
                                   );
            #endregion

            On.EntityStates.Treebot.Weapon.FireSonicBoom.OnEnter += FireSonicBoom_OnEnter;
        }

        private void FireSonicBoom_OnEnter(On.EntityStates.Treebot.Weapon.FireSonicBoom.orig_OnEnter orig, EntityStates.Treebot.Weapon.FireSonicBoom self)
        {
            try
            {
                self.airKnockbackDistance = AirKnockBackDistance.Value;
                self.groundKnockbackDistance = GroundKnockBackDistance.Value;
                self.maxDistance = MaxDistance.Value;
                self.liftVelocity = LiftVelocity.Value;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
            finally
            {
                orig(self);
            }
        }

        #region ConsoleCommands

        /// <summary>
        /// Set default values to all configurations.
        /// </summary>
        [ConCommand(commandName = "boop_set_default", flags = ConVarFlags.ExecuteOnServer, helpText = "Set default values to all configurations.")]
        private static void BoopSetDefault()
        {
            AirKnockBackDistance.Value = BoopConstants.AirKnockBackDistanceDefault;
            GroundKnockBackDistance.Value = BoopConstants.GroundKnockBackDistanceDefault;
            MaxDistance.Value = BoopConstants.MaxDistanceDefault;
            LiftVelocity.Value = BoopConstants.LiftVelocityDefault;
            Debug.Log("Set default values for configurations.");
        }

        /// <summary>
        /// Set recommended values to all configurations.
        /// </summary>
        [ConCommand(commandName = "boop_set_recommended", flags = ConVarFlags.ExecuteOnServer, helpText = "Set recommended values to all configurations.")]
        private static void BoopSetRecommended()
        {
            AirKnockBackDistance.Value = BoopConstants.AirKnockBackDistanceRecommended;
            GroundKnockBackDistance.Value = BoopConstants.GroundKnockBackDistanceRecommended;
            MaxDistance.Value = BoopConstants.MaxDistanceRecommended;
            LiftVelocity.Value = BoopConstants.LiftVelocityRecommended;
            Debug.Log("Set recommended values for configurations.");
        }

        /// <summary>
        /// Set value for a configuration
        /// </summary>
        /// <param name="args">args[0]=(string)configName, args[1]=(float)value</param>
        [ConCommand(commandName = "boop_set", flags = ConVarFlags.ExecuteOnServer, helpText = "args[0]=(string)configName, args[1]=(float)value")]
        private static void BoopSet(ConCommandArgs args)
        {
            try
            {
                ConfigWrapper<float> wrapper;
                switch (args[0].ToLower())
                {
                    default:
                        throw new Exception("Config wrapper not found.");

                    case "airknockbackdistance":
                    case "air":
                        wrapper = AirKnockBackDistance;
                        break;

                    case "groundknockbackdistance":
                    case "ground":
                        wrapper = GroundKnockBackDistance;
                        break;

                    case "liftvelocity":
                    case "lift":
                        wrapper = LiftVelocity;
                        break;

                    case "maxdistance":
                    case "distance":
                        wrapper = MaxDistance;
                        break;
                }

                float value = float.Parse(args[1]);
                AirKnockBackDistance.Value = value;
                Debug.Log($"Set {args[0]}={value}");
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }

        /// <summary>
        /// Get current configuration values
        /// </summary>
        [ConCommand(commandName = "boop_get", flags = ConVarFlags.ExecuteOnServer, helpText = "Get current values to display in Chat.")]
        private static void BoopGet()
        {
            Chat.AddMessage($"AirKnockback={AirKnockBackDistance.Value}\n\r" +
                $"GroundKnockback={GroundKnockBackDistance.Value}\n\r" +
                $"LiftVelocity={LiftVelocity.Value}\n\r" +
                $"MaxDistance={MaxDistance.Value}\n\r");

            Debug.Log($"AirKnockback ={ AirKnockBackDistance.Value}, " +
                $"GroundKnockback={GroundKnockBackDistance.Value}, " +
                $"LiftVelocity={LiftVelocity.Value}, " +
                $"MaxDistance={MaxDistance.Value}");
        }
        #endregion
    }
}
