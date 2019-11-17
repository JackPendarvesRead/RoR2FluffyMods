using BepInEx;
using EntityStates;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            GetSurvivor();
        }

        private void GetSurvivor()
        {
            Logger.LogInfo("0");

            var survDef = new SurvivorDef
            {
                name = "Bloop",
                unlockableName = "BloopUnlock",
                descriptionToken = "This is the description token",
                primaryColor = Color.black,
                displayPrefab = Resources.Load<GameObject>("Prefabs/Characters/BanditDisplay"),
                bodyPrefab = BodyCatalog.FindBodyPrefab("BanditBody")
            };

            Logger.LogInfo("1");


            int bodyIndex = BodyCatalog.FindBodyIndex("BanditBody");
            SkillLocator skillLocator = BodyCatalog.GetBodyPrefab(bodyIndex).GetComponent<SkillLocator>();
            RoR2.Skills.SkillFamily skillFamily = skillLocator.utility.skillFamily;
            RoR2.Skills.SkillDef defaultSkill = skillFamily.variants[skillFamily.defaultVariantIndex].skillDef;
            defaultSkill.activationState = new SerializableEntityStateType(typeof(EntityStates.PrimarySkill));
            object box = defaultSkill.activationState;
            var field = typeof(SerializableEntityStateType)?.GetField("_typeName", BindingFlags.NonPublic | BindingFlags.Instance);
            field?.SetValue(box, typeof(EntityStates.PrimarySkill)?.AssemblyQualifiedName);
            defaultSkill.activationState = (SerializableEntityStateType)box;
            Logger.LogInfo("2");

            R2API.SurvivorAPI.AddSurvivor(survDef);
            Logger.LogInfo("3");

        }
    }
}
