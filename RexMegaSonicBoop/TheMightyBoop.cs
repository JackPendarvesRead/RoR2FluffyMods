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
            AirKnockBackDistance = Config.Wrap(
                                   "FireSonicBoom",
                                   "AirKnockBackDistance",
                                   "Set how far you knock yourself back when you boop in mid-air. (Game default = 8,  Recommended = 16)",
                                   16f
                                   );

            GroundKnockBackDistance = Config.Wrap(
                                   "FireSonicBoom",
                                   "GroundKnockBackDistance", 
                                   "Set how far you knock yourself back when you boop whilst on the ground. (Game default = 0, Recommended = 0)",
                                   0f
                                   );

            MaxDistance = Config.Wrap(
                                   "FireSonicBoom",
                                   "MaxDistance",
                                   "Set the horizontal distance which enemies should be knocked back by the boop. (Game default = 30, Recommended=100)",
                                   100f
                                   );

            LiftVelocity = Config.Wrap(
                                   "FireSonicBoom",
                                   "LiftVelocity",
                                   "Set the vertical lift of enemies affected by the boop. (Game default = 3f, Recommended = 6)",
                                   6f
                                   );

            On.EntityStates.Treebot.Weapon.FireSonicBoom.OnEnter += FireSonicBoom_OnEnter;
        }

        private void FireSonicBoom_OnEnter(On.EntityStates.Treebot.Weapon.FireSonicBoom.orig_OnEnter orig, EntityStates.Treebot.Weapon.FireSonicBoom self)
        {
            self.airKnockbackDistance = AirKnockBackDistance.Value;
            self.groundKnockbackDistance = GroundKnockBackDistance.Value;
            self.maxDistance = MaxDistance.Value;
            self.liftVelocity = LiftVelocity.Value;
            orig(self);
        }
    }
}
