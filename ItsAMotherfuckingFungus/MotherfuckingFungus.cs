using BepInEx;
using BepInEx.Configuration;
using MonoMod.Cil;
using RoR2;
using UnityEngine;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Linq;

namespace MotherfuckingFungus
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.FluffyMods.MotherfuckingFungus", "MotherfuckingFungus", "1.0.2")]
    public class MotherfuckingFungus : BaseUnityPlugin
    {
        private static bool IsEngineerInGame = false;

        public void Awake()
        {            
            On.RoR2.Stage.Start += Stage_Start;
            On.RoR2.PickupDropletController.CreatePickupDroplet += PickupDropletController_CreatePickupDroplet;
            On.RoR2.GenericPickupController.SendPickupMessage += GenericPickupController_SendPickupMessage;
        }

        private void Stage_Start(On.RoR2.Stage.orig_Start orig, Stage self)
        {            
            orig(self); 
            if(NetworkUser.readOnlyInstancesList
                .Where(u=> u.GetCurrentBody().name.StartsWith(CharBodyStrings.Engineer))
                .FirstOrDefault()
                != null)
            {
                IsEngineerInGame = true;
                Message.SendToAll("Give yo' motherfucking fungus to the motherfucking Engineer motherfuckers", Colours.LightBlue);
            }
        }

        private void PickupDropletController_CreatePickupDroplet(On.RoR2.PickupDropletController.orig_CreatePickupDroplet orig, PickupIndex pickupIndex, Vector3 position, Vector3 velocity)
        {
            // TODO change pickup indexs to catalogs
            if (IsEngineerInGame && pickupIndex.itemIndex == ItemIndex.Mushroom)
            {
                Message.SendToAll($"It's a motherfucking fungus!!", Colours.Green);
            }
            orig(pickupIndex, position, velocity);
        }

        private void GenericPickupController_SendPickupMessage(On.RoR2.GenericPickupController.orig_SendPickupMessage orig, CharacterMaster master, PickupIndex pickupIndex)
        {
            if (IsEngineerInGame && pickupIndex.itemIndex == ItemIndex.Mushroom)
            {
                if (master.GetBody().name.StartsWith(CharBodyStrings.Engineer))
                {
                    Message.SendToAll("Mmmm! That is a tasty fungus!", Colours.Green);
                }
                else
                {                    
                    Message.SendToAll("This motherfucker stole a motherfucking fungus!", Colours.Red);
                }
            }
            orig(master, pickupIndex);
        }
    }
}
