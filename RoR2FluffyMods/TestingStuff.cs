using BepInEx;
using BepInEx.Configuration;
using MonoMod.Cil;
using RoR2;
using UnityEngine;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;

namespace RoR2FluffyMods
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.RoR2FluffyMods", "RoR2FluffyMods", "0.0.0")]
    public class TestingStuff : BaseUnityPlugin
    {
        public void Awake()
        {
            On.RoR2.CharacterMaster.SpawnBody += CharacterMaster_SpawnBody;            
        }

        private CharacterBody CharacterMaster_SpawnBody(On.RoR2.CharacterMaster.orig_SpawnBody orig, 
            CharacterMaster self, 
            GameObject bodyPrefab, 
            Vector3 position, 
            Quaternion rotation)
        {
            var player = PlayerCharacterMasterController.instances
                .Where(p => p.master == self)
                .FirstOrDefault();
            //var birdShark = BodyCatalog.FindBodyPrefab("birdsharkbody");
            var birdShark = Resources.Load<GameObject>("birdsharkbody");
            Debug.Log($"name = {birdShark.name}");
            if (player != null && birdShark != null)
            {
                return orig(self, birdShark, position, rotation);
            }
            else
            {
                return orig(self, bodyPrefab, position, rotation);
            }
        }
    }
}
