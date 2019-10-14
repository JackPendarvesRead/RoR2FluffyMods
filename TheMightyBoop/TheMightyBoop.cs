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
using ConfigurationManager;

namespace TheMightyBoop
{

    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.TheMightyBoop", "TheMightyBoop", "2.0.0")]
    public class TheMightyBoop : BaseUnityPlugin
    {
        private static ConfigEntry<bool> ClayBruiserIsMighty;
        private static ConfigEntry<float> AirKnockBackDistance;
        private static ConfigEntry<float> GroundKnockBackDistance;
        private static ConfigEntry<float> LiftVelocity;
        private static ConfigEntry<float> MaxDistance;
        private static ConfigEntry<string> BoopValues;

        public void Awake()
        {  
            #region ConfigSetup
            

            const string fireSonicBoomSection = "FireSonicBoom";
            const string clayBruiserSection = "ClayBruiser";
            const string presetSection = "Presets";
            
            BoopValues = Config.AddSetting<string>(
                new ConfigDefinition(presetSection, nameof(BoopValues)),
                "",
                new ConfigDescription("", null, new Action<SettingEntryBase>(BoopPresetButtons))
                );

            ClayBruiserIsMighty = Config.AddSetting<bool>(
                new ConfigDefinition(clayBruiserSection, nameof(ClayBruiserIsMighty)), 
                false, 
                new ConfigDescription(
                    "Set whether the boop of the Clay Templar is mighty like Rex",                
                    null,
                    ConfigTags.Advanced
                    ));

            AirKnockBackDistance = Config.AddSetting<float>(
                new ConfigDefinition(fireSonicBoomSection, nameof(AirKnockBackDistance)), 
                BoopConstants.AirKnockBackDistanceRecommended, 
                new ConfigDescription(
                    "Set how far you knock yourself back when you boop in mid-air. " +
                    $"(Game default = {BoopConstants.AirKnockBackDistanceDefault},  Recommended = {BoopConstants.AirKnockBackDistanceRecommended})",
                    new AcceptableValueRange<float>(0, BoopConstants.MaximumBoop),
                    ConfigTags.Advanced
                    ));

            GroundKnockBackDistance = Config.AddSetting<float>(
                new ConfigDefinition(fireSonicBoomSection, nameof(GroundKnockBackDistance)),
                BoopConstants.GroundKnockBackDistanceRecommended,
                new ConfigDescription(
                    "Set how far you knock yourself back when you boop whilst on the ground. " +
                    $"(Game default = {BoopConstants.GroundKnockBackDistanceDefault}, Recommended = {BoopConstants.GroundKnockBackDistanceRecommended})",
                    new AcceptableValueRange<float>(0, BoopConstants.MaximumBoop),
                    ConfigTags.Advanced
                    ));

            MaxDistance = Config.AddSetting<float>(
                new ConfigDefinition(fireSonicBoomSection, nameof(MaxDistance)), 
                BoopConstants.MaxDistanceRecommended, 
                new ConfigDescription(
                    "Set the horizontal distance which enemies should be knocked back by the boop. " +
                    $"(Game default = {BoopConstants.MaxDistanceDefault}, Recommended = {BoopConstants.MaxDistanceRecommended})",
                    new AcceptableValueRange<float>(0, BoopConstants.MaximumBoop),
                    ConfigTags.Advanced
                    ));

            LiftVelocity = Config.AddSetting<float>(
                new ConfigDefinition(fireSonicBoomSection, nameof(LiftVelocity)), 
                BoopConstants.LiftVelocityRecommended, 
                new ConfigDescription(
                "Set the vertical lift of enemies affected by the boop. " +
                $"(Game default = {BoopConstants.LiftVelocityDefault}, Recommended = {BoopConstants.LiftVelocityRecommended})",
                new AcceptableValueRange<float>(0, BoopConstants.MaximumBoop),
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

        private void BoopPresetButtons(SettingEntryBase entry)
        {
            GUILayout.Label(BoopValues.Value, GUILayout.ExpandWidth(true));
            GUILayout.BeginVertical();
            bool PressVanillaButton()
            {
                return GUILayout.Button("VANILLA", GUILayout.ExpandWidth(true));
            }
            bool PressRecommendedButton()
            {
                return GUILayout.Button("RECOMMENDED", GUILayout.ExpandWidth(true));
            }
            bool PressSillyButton()
            {
                return GUILayout.Button("SILLY", GUILayout.ExpandWidth(true));
            }
            if (PressVanillaButton())
            {
                AirKnockBackDistance.Value = BoopConstants.AirKnockBackDistanceDefault;
                GroundKnockBackDistance.Value = BoopConstants.GroundKnockBackDistanceDefault;
                MaxDistance.Value = BoopConstants.MaxDistanceDefault;
                LiftVelocity.Value = BoopConstants.LiftVelocityDefault;
                Debug.Log("Set Vanilla values for configurations.");
                BoopValues.Value = "Vanilla";
            }
            if (PressRecommendedButton())
            {
                AirKnockBackDistance.Value = BoopConstants.AirKnockBackDistanceRecommended;
                GroundKnockBackDistance.Value = BoopConstants.GroundKnockBackDistanceRecommended;
                MaxDistance.Value = BoopConstants.MaxDistanceRecommended;
                LiftVelocity.Value = BoopConstants.LiftVelocityRecommended;
                Debug.Log("Set recommended values for configurations.");
                BoopValues.Value = "Recommended";
            }
            if (PressSillyButton())
            {
                AirKnockBackDistance.Value = BoopConstants.AirKnockBackDistanceSilly;
                GroundKnockBackDistance.Value = BoopConstants.GroundKnockBackDistanceSilly;
                MaxDistance.Value = BoopConstants.MaxDistanceSilly;
                LiftVelocity.Value = BoopConstants.LiftVelocitySilly;
                Debug.Log("Set silly values for configurations.");
                BoopValues.Value = "Silly";
            }
            GUILayout.EndVertical();
        }
    }
}
