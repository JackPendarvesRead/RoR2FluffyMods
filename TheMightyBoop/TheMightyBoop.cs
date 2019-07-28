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
    [BepInPlugin("com.FluffyMods.TheMightyBoop", "TheMightyBoop", "1.1.0")]
    public class RexMegaSonicBoop : BaseUnityPlugin
    {
        private static ConfigWrapper<bool> MightyClayBruiser;

        private static ConfigWrapper<float> AirKnockBackDistance;
        private static ConfigWrapper<float> GroundKnockBackDistance;
        private static ConfigWrapper<float> LiftVelocity;
        private static ConfigWrapper<float> MaxDistance;

        public void Awake()
        {            
            On.RoR2.Console.Awake += (orig, self) =>
            {
                CommandHelper.RegisterCommands(self);
                orig(self);
            };

            #region ConfigWrapperSetup

            string sectionName = "FireSonicBoom";

            MightyClayBruiser = Config.Wrap(                               
                "Clay Bruiser",                               
                "ClayBruiserIsMighty",                                
                $"Set whether the boop of the Clay Templar is mighty like Rex",
                false                                
                );

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

            On.EntityStates.ClayBruiser.Weapon.FireSonicBoom.OnEnter += FireSonicBoom_OnEnter1;
            On.EntityStates.Treebot.Weapon.FireSonicBoom.OnEnter += FireSonicBoom_OnEnter;
        }

        private void FireSonicBoom_OnEnter1(On.EntityStates.ClayBruiser.Weapon.FireSonicBoom.orig_OnEnter orig, EntityStates.ClayBruiser.Weapon.FireSonicBoom self)
        {
            if (MightyClayBruiser.Value == false)
            {
                isTemplarBoop = true;
            }
            orig(self);
        }
        private bool isTemplarBoop = false;

        private void FireSonicBoom_OnEnter(On.EntityStates.Treebot.Weapon.FireSonicBoom.orig_OnEnter orig, EntityStates.Treebot.Weapon.FireSonicBoom self)
        {
            try
            {
                if (isTemplarBoop)
                {
                    isTemplarBoop = false;
                    self.airKnockbackDistance = BoopConstants.AirKnockBackDistanceDefault;
                    self.groundKnockbackDistance = BoopConstants.GroundKnockBackDistanceDefault;
                    self.liftVelocity = BoopConstants.LiftVelocityDefault;
                    self.maxDistance = BoopConstants.MaxDistanceDefault;
                }    
                else 
                {
                    self.airKnockbackDistance = (AirKnockBackDistance.Value < BoopConstants.MaximumBoop) ? AirKnockBackDistance.Value : BoopConstants.MaximumBoop;
                    self.groundKnockbackDistance = (GroundKnockBackDistance.Value < BoopConstants.MaximumBoop) ? GroundKnockBackDistance.Value : BoopConstants.MaximumBoop;
                    self.liftVelocity = (LiftVelocity.Value < BoopConstants.MaximumBoop) ? LiftVelocity.Value : BoopConstants.MaximumBoop;
                    self.maxDistance = (MaxDistance.Value < BoopConstants.MaximumBoop) ? MaxDistance.Value : BoopConstants.MaximumBoop;
                }
                
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
        private static void BoopSetDefault(ConCommandArgs args)
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
        private static void BoopSetRecommended(ConCommandArgs args)
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
                if(args.Count != 2)
                {
                    throw new Exception("Arguements length must be equal to 2.");
                }

                var wrapperName = args[0];
                var value = float.Parse(args[1]);
                if(value > BoopConstants.MaximumBoop)
                {
                    value = BoopConstants.MaximumBoop;
                }                

                switch (wrapperName.ToLower())
                {
                    default:
                        throw new Exception("Config wrapper not found.");

                    case "airknockbackdistance":
                    case "air":
                    case "a":
                        AirKnockBackDistance.Value = value;
                        Debug.Log($"{nameof(AirKnockBackDistance)}={value}");
                        return;

                    case "groundknockbackdistance":
                    case "ground":
                    case "g":
                        GroundKnockBackDistance.Value = value;
                        Debug.Log($"{nameof(GroundKnockBackDistance)}={value}");
                        return;

                    case "liftvelocity":
                    case "lift":
                    case "l":
                        LiftVelocity.Value = value;
                        Debug.Log($"{nameof(LiftVelocity)}={value}");
                        return;

                    case "maxdistance":
                    case "distance":
                    case "d":
                        MaxDistance.Value = value;
                        Debug.Log($"{nameof(MaxDistance)}={value}");
                        return;
                }
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
        private static void BoopGet(ConCommandArgs args)
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

        /// <summary>
        /// Set clay templar to be mighty or not
        /// </summary>
        [ConCommand(commandName = "boop_mightyclay", flags = ConVarFlags.ExecuteOnServer, helpText = "Set clay templar to be mighty (true/false)")]
        private static void BoopSetMightyClay(ConCommandArgs args)
        {
            try
            {
                bool mighty;
                switch (args[0])
                {
                    case "true":
                    case "t":
                    case "1":
                        mighty = true;
                        break;

                    case "false":
                    case "f":
                    case "0":
                        mighty = false;
                        break;

                    default:
                        throw new Exception("Arguement string is incorrect format. Type true or false.");
                }
                MightyClayBruiser.Value = mighty;
                Debug.Log($"Setting clay bruiser is mighty to {mighty}");
            }
            catch(Exception ex)
            {
                Debug.LogError(ex);
            }
        }
        #endregion
    }
}
