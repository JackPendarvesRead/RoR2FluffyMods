using System;
using System.Collections.Generic;
using System.Text;

namespace CustomCharacterBuilder.Infrastructure
{
    public class CustomBodyStats
    {
        public float BaseJumpPower { get; set; }
        public int BaseJumpCount { get; set; }
        public float BaseMaxHealth { get; set; }
        public float LevelMaxHealth { get; set; }
        public float BaseRegen { get; set; }
        public float LevelRegen { get; set; }
        public int BaseArmour { get; set; }
    }
}
