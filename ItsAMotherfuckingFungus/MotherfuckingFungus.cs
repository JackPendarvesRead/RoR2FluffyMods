using BepInEx;
using RoR2;
using UnityEngine;
using System.Linq;

namespace MotherfuckingFungus
{
    [BepInPlugin(PluginGuid, pluginName, pluginVersion)]
    public class MotherfuckingFungus : BaseUnityPlugin
    {
        public const string PluginGuid = "com.FluffyMods." + pluginName;
        private const string pluginName = "MotherfuckingFungus";
        private const string pluginVersion = "2.0.0";

        private static bool EngineerInGame = false;

        public void Awake()
        {
            On.RoR2.Stage.Start += Stage_Start;
            On.RoR2.PickupDropletController.CreatePickupDroplet += SendMessageOnFungusDrop;
            On.RoR2.GenericPickupController.SendPickupMessage += SendMessageOnFungusPickup;
        }

        private void Stage_Start(On.RoR2.Stage.orig_Start orig, Stage self)
        {
            orig(self);
            if (RoR2.Run.instance
               && NetworkUser.readOnlyInstancesList
               .Select(x => x.GetCurrentBody())
               .Where(x => x.bodyIndex == SurvivorCatalog.GetBodyIndexFromSurvivorIndex(SurvivorIndex.Engi))
               .Any())
            {
                EngineerInGame = true;
                Message.SendToAll("Give yo' motherfucking fungus to the motherfucking Engineer motherfuckers", Colours.LightBlue);
            }
            else
            {
                EngineerInGame = false;
                Logger.LogInfo("No Engineer in party.");
            }
        }

        private void SendMessageOnFungusDrop(On.RoR2.PickupDropletController.orig_CreatePickupDroplet orig, PickupIndex pickupIndex, Vector3 position, Vector3 velocity)
        {
            if (EngineerInGame 
                && RoR2.PickupCatalog.GetPickupDef(pickupIndex).itemIndex == ItemIndex.Mushroom)
            {
                Message.SendToAll($"It's a motherfucking fungus!!", Colours.Green);
            }
            orig(pickupIndex, position, velocity);
        }

        private void SendMessageOnFungusPickup(On.RoR2.GenericPickupController.orig_SendPickupMessage orig, CharacterMaster master, PickupIndex pickupIndex)
        {
            if (EngineerInGame 
                && RoR2.PickupCatalog.GetPickupDef(pickupIndex).itemIndex == ItemIndex.Mushroom)
            {
                if (master.GetBody().bodyIndex == SurvivorCatalog.GetBodyIndexFromSurvivorIndex(SurvivorIndex.Engi))
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
