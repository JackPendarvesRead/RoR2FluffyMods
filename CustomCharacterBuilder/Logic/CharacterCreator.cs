using CustomCharacterBuilder.Infrastructure;
using EntityStates.Treebot.UnlockInteractable;
using R2API;
using R2API.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace CustomCharacterBuilder.Logic
{
    public static class CharacterCreator
    {
       public static void Create<T>(CharacterInformation characterInfo, PrefabInfo bodyPrefabInfo, PrefabInfo displayPrefabInfo)
        {
            var bodyPrefab = Resources.Load<GameObject>(bodyPrefabInfo.ResourceLocationString).InstantiateClone(bodyPrefabInfo.Name);
            var displayPrefab = Resources.Load<GameObject>(displayPrefabInfo.ResourceLocationString).InstantiateClone(displayPrefabInfo.Name, false);
            Create<T>(characterInfo, bodyPrefab, displayPrefab);
        }

        public static void Create<T>(CharacterInformation info, GameObject bodyPrefab, GameObject displayPrefab)
        {
            RegisterSkills(info, bodyPrefab);
            CharacterBody body = bodyPrefab.GetComponentInChildren<CharacterBody>();
            RegisterBody(bodyPrefab);
            RegisterGenericMainCharacter<T>(body);
            RegisterOptionalStuff(info, body);
            RegisterSurvivorDef(info, bodyPrefab, displayPrefab);
        }

        private static void RegisterSkills(CharacterInformation info, GameObject bodyPrefab)
        {
            var locator = bodyPrefab.GetComponentInChildren<SkillLocator>();
            SkillRegister.RegisterSkills(info.Skills, locator);
        }

        private static void RegisterBody(GameObject bodyPrefab)
        {
            BodyCatalog.getAdditionalEntries += (list) => list.Add(bodyPrefab);
        }

        private static void RegisterGenericMainCharacter<T>(CharacterBody body)
        {
            var stateMachine = body.GetComponent<EntityStateMachine>();
            stateMachine.mainStateType = new EntityStates.SerializableEntityStateType(typeof(T));
        }

        private static void RegisterOptionalStuff(CharacterInformation info, CharacterBody body)
        {
            if (info.PortraitIcon != null)
            {
                body.portraitIcon = info.PortraitIcon;
            }

            if (info.CustomStats != null)
            {
                CustomStatsRegister.Register(info.CustomStats, body);
            }
            
            if (body.preferredPodPrefab == null)
            {
                body.preferredPodPrefab = info.PreferredPod ?? Resources.Load<GameObject>("prefabs/characterbodies/commandobody").GetComponentInChildren<CharacterBody>().preferredPodPrefab;
            }
        }

        private static void RegisterSurvivorDef(CharacterInformation info, GameObject bodyPrefab, GameObject displayPrefab)
        {
            var survDef = new SurvivorDef
            {
                bodyPrefab = bodyPrefab,
                displayPrefab = displayPrefab,
                name = info.Name,
                descriptionToken = info.Description,
                primaryColor = info.PrimaryColour,
                unlockableName = info.UnlockableName ?? ""
            };
            SurvivorAPI.AddSurvivor(survDef);
        }
    }
}
