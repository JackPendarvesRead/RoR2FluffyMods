using EntityStates.MyCustomCharacter;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CustomCharacterPlay.HelperStuff
{
    public class CharacterCreator
    {
        public void Create<T>(CharacterInformation characterInfo, PrefabInfo bodyPrefabInfo, PrefabInfo displayPrefabInfo)
        {
            var bodyPrefab = Resources.Load<GameObject>(bodyPrefabInfo.ResourceLocationString).InstantiateClone(bodyPrefabInfo.Name);
            var displayPrefab = Resources.Load<GameObject>(displayPrefabInfo.ResourceLocationString).InstantiateClone(displayPrefabInfo.Name, false);
            Create<T>(characterInfo, bodyPrefab, displayPrefab);
        }

        public void Create<T>(CharacterInformation info, GameObject bodyPrefab, GameObject displayPrefab)
        {
            //Register Skills
            var locator = bodyPrefab.GetComponentInChildren<SkillLocator>();
            new SkillRegister(locator).RegisterSkills();

            // Register Body Catalog
            var body = bodyPrefab.GetComponentInChildren<CharacterBody>();
            BodyCatalog.getAdditionalEntries += (list) => list.Add(bodyPrefab);

            // Register GenericMainCharacter
            var stateMachine = body.GetComponent<EntityStateMachine>();
            stateMachine.mainStateType = new EntityStates.SerializableEntityStateType(typeof(T));

            // register icon
            //body.portraitIcon = Assets
            
            if(info.CustomStats != null)
            {
                // Register custom stats
            }

            // Set preferred pod?
            if (body.preferredPodPrefab == null)
            {
                body.preferredPodPrefab = Resources.Load<GameObject>("prefabs/characterbodies/commandobody").GetComponentInChildren<CharacterBody>().preferredPodPrefab;
            }          

            // register surv def
            var survDef = new SurvivorDef
            {
                bodyPrefab = bodyPrefab,
                displayPrefab = displayPrefab,
                name = info.Name,
                descriptionToken = info.Description,
                primaryColor = new Color(1, 1, 1),
                unlockableName = ""
            };
            SurvivorAPI.AddSurvivor(survDef);
        }
    }
}
