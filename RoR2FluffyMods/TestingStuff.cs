using BepInEx;
using RoR2;
using UnityEngine;

namespace RoR2FluffyMods
{
    [BepInPlugin("com.TEST.TEST", "TEST", "0.0.0")]
    public class TestingStuff : BaseUnityPlugin
    {
        public void Awake()
        {
            On.RoR2.Console.Awake += (orig, self) =>
            {
                RoR2FluffyMods.CommandHelper.RegisterCommands(self);
                orig(self);
            };
            On.RoR2.PickupDropletController.CreatePickupDroplet += PickupDropletController_CreatePickupDroplet2;
        }

        private void PickupDropletController_CreatePickupDroplet2(On.RoR2.PickupDropletController.orig_CreatePickupDroplet orig, PickupIndex pickupIndex, Vector3 position, Vector3 velocity)
        {
            throw new System.NotImplementedException();
        }

        private void PickupDropletController_CreatePickupDroplet1(On.RoR2.PickupDropletController.orig_CreatePickupDroplet orig, PickupIndex pickupIndex, Vector3 position, Vector3 velocity)
        {
            throw new System.NotImplementedException();
        }

        private void PickupDropletController_CreatePickupDroplet(
            On.RoR2.PickupDropletController.orig_CreatePickupDroplet orig, 
            PickupIndex pickupIndex, 
            Vector3 position, 
            Vector3 velocity)
        {
            if(pickupIndex == PickupIndex.Find("LunarCoin.Coin0"))
            {
            }
            orig(pickupIndex, position, velocity);
        }

        [ConCommand(commandName = "Lunar_Drop", flags = ConVarFlags.ExecuteOnServer, helpText = "Help text goes here")]
        private static void LunarDrop(ConCommandArgs args)
        {
            var nu = RoR2.NetworkUser.readOnlyInstancesList[0];
            PickupDropletController.CreatePickupDroplet(PickupIndex.Find("LunarCoin.Coin0"), nu.transform.position, Vector3.up * 10f);            
        }
    }   
}
