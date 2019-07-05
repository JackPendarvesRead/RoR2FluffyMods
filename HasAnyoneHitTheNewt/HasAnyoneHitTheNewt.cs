using BepInEx;
using RoR2;
using RoR2.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HasAnyoneHitTheNewt
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.HasAnyoneHitTheNewt", "HasAnyoneHitTheNewt", "1.0.1")]
    public class HasAnyoneHitTheNewt : BaseUnityPlugin
    {
        BuffIcon Icon { get; set; }

        public void Awake()
        {
            On.RoR2.PortalStatueBehavior.GrantPortalEntry += PortalStatueBehavior_GrantPortalEntry;
        }

        private void PortalStatueBehavior_GrantPortalEntry(On.RoR2.PortalStatueBehavior.orig_GrantPortalEntry orig, PortalStatueBehavior self)
        {
            throw new NotImplementedException();
        }
    }
}
