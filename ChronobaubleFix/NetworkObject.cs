using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace ChronobaubleFix
{
    internal class NetworkObject : NetworkBehaviour
    {
        public NetworkObject()
        {
        }

        public void Invoke(NetworkUser user, string msg)
        {
            
        }

        private void DoThing(NetworkConnection conection, string msg)
        {

        }
    }
}
