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
    }
}
