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
        public void Create<T>(CharacterInformation info, string bodyPrefabString, string displayPrefabString)
        {
            UnityEngine.Debug.Log("1");
            UnityEngine.Debug.Log(info.Name);
            var bodyPrefab = Resources.Load<GameObject>(bodyPrefabString).InstantiateClone("testtest");
            var displayPrefab = Resources.Load<GameObject>(displayPrefabString).InstantiateClone("testtest2", false);
            UnityEngine.Debug.Log("2");
            Create<T>(info, bodyPrefab, displayPrefab);
        }

        public void Create<T>(CharacterInformation info, GameObject bodyPrefab, GameObject displayPrefab)
        {
            UnityEngine.Debug.Log("3");

            //Register Skills
            new SkillRegister(bodyPrefab.GetComponentInChildren<SkillLocator>()).RegisterSkills();
            UnityEngine.Debug.Log("4");

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
