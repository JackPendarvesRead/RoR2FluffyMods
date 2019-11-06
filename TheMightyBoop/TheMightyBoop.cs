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
using FluffyLabsConfigManagerTools.Util;

namespace TheMightyBoop
{
    [PluginDependency("com.FluffyMods.FluffyLabsConfigManagerTools")]
    [PluginMetadata("com.FluffyMods.TheMightyBoop", "TheMightyBoop", "2.0.0")]
    public class TheMightyBoop : BaseUnityPlugin
    {
        private ConfigEntry<bool> ClayBruiserIsMighty;
        private ConfigEntry<bool> RandomDirection;
        private ConfigEntry<float> AirKnockBackDistance;
        private ConfigEntry<float> GroundKnockBackDistance;
        private ConfigEntry<float> IdealDistanceToPlaceTargets;
        private ConfigEntry<float> LiftVelocity;
        private ConfigEntry<float> MaxDistance;

        public void Awake()
        {
            if (!RoR2Application.isModded)
            {
                RoR2Application.isModded = true;
            }

            #region ConfigSetup
            const string fireSonicBoomSection = "FireSonicBoom";
            const string clayBruiserSection = "ClayBruiser";

            var buttonUtil = new ButtonUtil(this.Config);
            buttonUtil.AddButtonConfig("Presets", "Preset", "Select preset configurations with buttons", GetButtonDictionary());

            ClayBruiserIsMighty = Config.Bind<bool>(
                new ConfigDefinition(clayBruiserSection, nameof(ClayBruiserIsMighty)), 
                false, 
                new ConfigDescription(
                    "Set whether the boop of the Clay Templar is mighty like Rex",                
                    null,
                    ConfigTags.Advanced
                    ));

            RandomDirection = Config.Bind<bool>(
                new ConfigDefinition(fireSonicBoomSection, "RandomHorizontalForce"),
                false,
                new ConfigDescription(
                    "When enabled the HorizontalForce becomes a random number between the +/- value (e.g. if force = 500, when random the force becomes any number between -500 to 500)",
                    null,
                    ConfigTags.Advanced
                    ));


            AirKnockBackDistance = Config.Bind<float>(
                new ConfigDefinition(fireSonicBoomSection, nameof(AirKnockBackDistance)), 
                BoopConstants.AirKnockBackDistanceRecommended, 
                new ConfigDescription(
                    "Set how far you knock yourself back when you boop in mid-air." +
                    $"(Game default = {BoopConstants.AirKnockBackDistanceDefault},  Recommended = {BoopConstants.AirKnockBackDistanceRecommended})",
                    new AcceptableValueRange<float>(0, 1000),
                    ConfigTags.Advanced
                    ));

            GroundKnockBackDistance = Config.Bind<float>(
                new ConfigDefinition(fireSonicBoomSection, nameof(GroundKnockBackDistance)),
                BoopConstants.GroundKnockBackDistanceRecommended,
                new ConfigDescription(
                    "Set how far you knock yourself back when you boop whilst on the ground." +
                    $"(Game default = {BoopConstants.GroundKnockBackDistanceDefault}, Recommended = {BoopConstants.GroundKnockBackDistanceRecommended})",
                    new AcceptableValueRange<float>(0, 1000),
                    ConfigTags.Advanced
                    ));

            IdealDistanceToPlaceTargets = Config.Bind<float>(
                new ConfigDefinition(fireSonicBoomSection, "HorizontalForce"), 
                BoopConstants.IdealDistanceDefault, 
                new ConfigDescription(
                    "Set the horizontal distance which enemies should be knocked back by the boop (can be set -ve for opposite effect)" +
                    $"(Game default = {BoopConstants.MaxDistanceDefault}, Recommended = {BoopConstants.MaxDistanceRecommended})",
                    new AcceptableValueRange<float>(-1000, 1000),
                    ConfigTags.Advanced
                    ));

            MaxDistance = Config.Bind<float>(
               new ConfigDefinition(fireSonicBoomSection, "BoopRange"),
               BoopConstants.MaxDistanceRecommended,
               new ConfigDescription(
                   "Range at which enemies will be effected by boop" +
                   $"(Game default = {BoopConstants.MaxDistanceDefault}, Recommended = {BoopConstants.MaxDistanceRecommended})",
                   new AcceptableValueRange<float>(0, 500),
                   ConfigTags.Advanced
                   ));

            LiftVelocity = Config.Bind<float>(
                new ConfigDefinition(fireSonicBoomSection, nameof(LiftVelocity)), 
                BoopConstants.LiftVelocityRecommended, 
                new ConfigDescription(
                "Set the vertical lift of enemies affected by the boop. " +
                $"(Game default = {BoopConstants.LiftVelocityDefault}, Recommended = {BoopConstants.LiftVelocityRecommended})",
                new AcceptableValueRange<float>(0, 100),
                ConfigTags.Advanced
                ));
            #endregion

            On.EntityStates.Treebot.Weapon.FireSonicBoom.OnEnter += FireSonicBoom_OnEnter;
            On.EntityStates.ClayBruiser.Weapon.FireSonicBoom.OnEnter += ClayBruiserFireSonicBoom_OnEnter;
        }

        private void ClayBruiserFireSonicBoom_OnEnter(On.EntityStates.ClayBruiser.Weapon.FireSonicBoom.orig_OnEnter orig, EntityStates.ClayBruiser.Weapon.FireSonicBoom self)
        {
            if (!ClayBruiserIsMighty.Value)
            {
                useDefaultBoop = true;
            }
            orig(self);
        }

        private System.Random rng = new System.Random();
        private bool useDefaultBoop = false;
        private void FireSonicBoom_OnEnter(On.EntityStates.Treebot.Weapon.FireSonicBoom.orig_OnEnter orig, EntityStates.Treebot.Weapon.FireSonicBoom self)
        {
            try
            {
                if (useDefaultBoop)
                {
                    useDefaultBoop = false;
                    self.airKnockbackDistance = BoopConstants.AirKnockBackDistanceDefault;
                    self.groundKnockbackDistance = BoopConstants.GroundKnockBackDistanceDefault;
                    self.liftVelocity = BoopConstants.LiftVelocityDefault;
                    self.maxDistance = BoopConstants.MaxDistanceDefault;
                }    
                else 
                {
                    self.airKnockbackDistance = AirKnockBackDistance.Value;
                    self.groundKnockbackDistance = GroundKnockBackDistance.Value;
                    self.liftVelocity = LiftVelocity.Value;
                    self.maxDistance = MaxDistance.Value;
                    if (RandomDirection.Value)
                    {
                        self.idealDistanceToPlaceTargets = RandomiseFloat(IdealDistanceToPlaceTargets.Value);
                    }
                    else
                    {                        
                        self.idealDistanceToPlaceTargets = IdealDistanceToPlaceTargets.Value;
                    }
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

        private float RandomiseFloat(float value)
        {
            var rounded = Mathf.Abs(Mathf.RoundToInt(value));
            var randomValue = rng.Next(-rounded, rounded);
            return value;
        }

        private Dictionary<string, Action> GetButtonDictionary()
        {
            return new Dictionary<string, Action>
            {
                { "Vanilla", SetVanillaConfig },
                { "Recommended", SetRecommendedConfig },
                { "Silly", SetSillyConfig },
                { "Ludicrous", SetLudicrousConfig },
                { "TractorBeam", SetTractorBeamConfig }
            };
        }

        private void SetVanillaConfig()
        {
            AirKnockBackDistance.Value = BoopConstants.AirKnockBackDistanceDefault;
            GroundKnockBackDistance.Value = BoopConstants.GroundKnockBackDistanceDefault;
            MaxDistance.Value = BoopConstants.MaxDistanceDefault;
            LiftVelocity.Value = BoopConstants.LiftVelocityDefault;
            IdealDistanceToPlaceTargets.Value = BoopConstants.IdealDistanceDefault;
            Debug.Log("Set default values for configurations.");
        }

        private void SetRecommendedConfig()
        {
            AirKnockBackDistance.Value = BoopConstants.AirKnockBackDistanceRecommended;
            GroundKnockBackDistance.Value = BoopConstants.GroundKnockBackDistanceRecommended;
            MaxDistance.Value = BoopConstants.MaxDistanceRecommended;
            LiftVelocity.Value = BoopConstants.LiftVelocityRecommended;
            IdealDistanceToPlaceTargets.Value = BoopConstants.IdealDistanceRecommended;
            Debug.Log("Set recommended values for configurations.");
        }

        private void SetSillyConfig()
        {
            AirKnockBackDistance.Value = BoopConstants.AirKnockBackDistanceSilly;
            GroundKnockBackDistance.Value = BoopConstants.GroundKnockBackDistanceSilly;
            MaxDistance.Value = BoopConstants.MaxDistanceSilly;
            LiftVelocity.Value = BoopConstants.LiftVelocitySilly;
            IdealDistanceToPlaceTargets.Value = BoopConstants.IdealDistanceSilly;
            Debug.Log("Set silly values for configurations.");
        }

        private void SetLudicrousConfig()
        {
            AirKnockBackDistance.Value = BoopConstants.AirKnockBackDistanceLudicrous;
            GroundKnockBackDistance.Value = BoopConstants.GroundKnockBackDistanceLudicrous;
            MaxDistance.Value = BoopConstants.MaxDistanceLudicrous;
            LiftVelocity.Value = BoopConstants.LiftVelocityLudicrous;
            IdealDistanceToPlaceTargets.Value = BoopConstants.IdealDistanceLudicrous;
            Debug.Log("Set LUDICROUS values for configurations.");
        }

        private void SetTractorBeamConfig()
        {
            AirKnockBackDistance.Value = BoopConstants.AirKnockBackDistanceRecommended;
            GroundKnockBackDistance.Value = BoopConstants.GroundKnockBackDistanceRecommended;
            MaxDistance.Value = 90;
            LiftVelocity.Value = BoopConstants.LiftVelocityRecommended;
            IdealDistanceToPlaceTargets.Value = 5;
            Debug.Log("Set negative values for configurations.");
        }
    }
}
