using System.Collections.Generic;

namespace TeleportVote
{
    internal static class InteractableObjectNames
    {
        public static string Teleporter => "Teleporter1(Clone)";
        public static string PortalShop => "PortalShop";
        public static string PortalShopClone => "PortalShop(Clone)";
        public static string GoldPortal => "GoldShores(Clone)";  // ???


        public static List<string> GetAllRestrictedInteractableNames()
        {
            //NOTE: this does not include teleporter as this is hooked elsewhere

            return new List<string>
            {
                PortalShop,
                PortalShopClone,
                GoldPortal
            };
        }
    }
}
