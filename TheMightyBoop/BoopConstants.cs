using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheMightyBoop
{
    internal static class BoopConstants
    {
        //Defaults set by RoR2 base game
        public static float AirKnockBackDistanceDefault => 8f;
        public static float GroundKnockBackDistanceDefault => 0f;
        public static float MaxDistanceDefault => 30f;
        public static float LiftVelocityDefault => 3f;
        public static float IdealDistanceDefault => 30f;

        //Recommended values set by me and testers for a fun and silly setup without being ludicrous
        public static float AirKnockBackDistanceRecommended => 16f;
        public static float GroundKnockBackDistanceRecommended => 0f;
        public static float MaxDistanceRecommended => 30f;
        public static float LiftVelocityRecommended => 6f;
        public static float IdealDistanceRecommended => 100f;

        //Silly values for a silly game
        public static float AirKnockBackDistanceSilly => 30f;
        public static float GroundKnockBackDistanceSilly => 10f;
        public static float MaxDistanceSilly => 100f;
        public static float LiftVelocitySilly => 50f;
        public static float IdealDistanceSilly => 200f;

        //Ludicrous speed
        public static float AirKnockBackDistanceLudicrous => 100f;
        public static float GroundKnockBackDistanceLudicrous => 10f;
        public static float MaxDistanceLudicrous => 200f;
        public static float LiftVelocityLudicrous => 100f;
        public static float IdealDistanceLudicrous => 1000f;
    }
}
