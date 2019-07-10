using System.Collections.Generic;

namespace TeleportVote
{
    internal static class InteractableObjectNames
    {
        public static string Teleporter => "Teleporter1(Clone)";
        public static string PortalShop => "PortalShop";
        public static string PortalShopClone => "PortalShop(Clone)";
        public static string GoldPortal => "portalgoldshores";
        public static string GoldPortalClone => "portalgoldshores(Clone)"; 
        public static string MsPortal => "portalms"; 
        public static string MsPortalClone => "portalms(Clone)";


        public static List<string> GetAllRestrictedInteractableNames()
        {
            //NOTE: this does not include teleporter as this is hooked elsewhere

            return new List<string>
            {
                PortalShop,
                PortalShopClone,
                GoldPortal,
                GoldPortalClone,
                MsPortal,
                MsPortalClone
            };
        }
    }
}
