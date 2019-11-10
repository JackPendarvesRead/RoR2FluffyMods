using BepInEx;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SupportSurv
{
    [BepInPlugin(PluginGuid, pluginName, pluginVersion)]
    public class SupporTSurv : BaseUnityPlugin
    {
        public const string PluginGuid = "com.FluffyMods." + pluginName;
        private const string pluginName = "SupportSurv";
        private const string pluginVersion = "1.0.0";

        public void Awake()
        {
            R2API.SurvivorAPI.SurvivorCatalogReady += SurvivorAPI_SurvivorCatalogReady;
        }

        private void SurvivorAPI_SurvivorCatalogReady(object sender, EventArgs e)
        {
            var survDef = new SurvivorDef
            {
                name = "Bloop",
                unlockableName = "BloopUnlock",
                descriptionToken = "This is the description token",
                primaryColor = Color.black,
                displayPrefab = Resources.Load<GameObject>("Prefabs/Characters/BanditDisplay"),
                bodyPrefab = BodyCatalog.FindBodyPrefab("BanditBody")
            };

            var skill1 = new GenericSkill();
            

            var skillFamily = new SkillFamily();

            R2API.SurvivorAPI.AddSurvivor(survDef);
        }
    }
}
