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

        //Recommended values set by me and testers for a fun and silly setup without being ludicrous
        public static float AirKnockBackDistanceRecommended => 16f;
        public static float GroundKnockBackDistanceRecommended => 0f;
        public static float MaxDistanceRecommended => 100f;
        public static float LiftVelocityRecommended => 6f;

        //Maximum value set to avoid users inputting absolutely ridiculous numbers
        public static float MaximumBoop => 1000f;
    }
}
