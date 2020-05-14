using BepInEx;
using CustomCharacterPlay.HelperStuff;
using EntityStates.MyCustomCharacter;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CustomCharacterPlay
{
    [R2APISubmoduleDependency(nameof(R2API.SurvivorAPI))]
    [R2APISubmoduleDependency(nameof(R2API.PrefabAPI))]
    [R2APISubmoduleDependency(nameof(R2API.LoadoutAPI))]
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(PluginGuid, pluginName, pluginVersion)]
    public class CustomCharacterPlay : BaseUnityPlugin
    {
        public const string PluginGuid = "com.FluffyMods." + pluginName;
        private const string pluginName = "CustomCharacterPlay";
        private const string pluginVersion = "1.0.0";

        public void Awake()
        {
            if (!RoR2Application.isModded)
            {
                RoR2Application.isModded = true;
            }

            // get prefabs
            var bodyPrefab = Resources.Load<GameObject>("prefabs/characterbodies/commandobody").InstantiateClone("MyWhateverBody");
            var displayPrefab = Resources.Load<GameObject>("prefabs/characterdisplays/commandodisplay").InstantiateClone("MyWhateverDisplay", false);

            // Register Skills
            new SkillRegister(bodyPrefab.GetComponentInChildren<SkillLocator>()).RegisterSkills();

            // register body cat
            var body = bodyPrefab.GetComponentInChildren<CharacterBody>();
            BodyCatalog.getAdditionalEntries += (list) => list.Add(bodyPrefab);

            // register stats

            // pod?
            if (body.preferredPodPrefab == null)
            {
                body.preferredPodPrefab = Resources.Load<GameObject>("prefabs/characterbodies/commandobody").GetComponentInChildren<CharacterBody>().preferredPodPrefab;
            }

            // register genericcharactermain
            var stateMachine = body.GetComponent<EntityStateMachine>();
            stateMachine.mainStateType = new EntityStates.SerializableEntityStateType(typeof(MyCustomCharacter));
            
            // register icon
            //body.portraitIcon = Assets

            // register surv def
            var survDef = new SurvivorDef
            {
                bodyPrefab = bodyPrefab,
                descriptionToken = "Description",
                displayPrefab = displayPrefab,
                name = "MyCustomWhatever",
                primaryColor = new Color(1, 1, 1),
                unlockableName = "UnlockableName"
            };
            SurvivorAPI.AddSurvivor(survDef);
        }       
    }
}
