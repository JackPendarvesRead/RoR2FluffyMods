using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeleportVote
{
    internal class VoteRegistrationController
    {
        public int VotesNeeded
        {
            get
            {
                return RoR2.Run.instance.livingPlayerCount;
            }
        }
        public List<NetworkUserId> RegisteredPlayers { get; set; }

        public event EventHandler PlayerRegistered;

        public void RegisterPlayer(NetworkUser netUser)
        {
            var netId = netUser.Network_id;
            if (!RegisteredPlayers.Contains(netId))
            {
                RegisteredPlayers.Add(netId);
                PlayerRegistered.Invoke(this, new PlayerRegisteredEventArgs(netUser));
            }
        }
    }

    internal class PlayerRegisteredEventArgs : EventArgs
    {
        public PlayerRegisteredEventArgs(NetworkUser registeredPlayer)
        {
            RegisteredPlayer = registeredPlayer;
        }

        public NetworkUser RegisteredPlayer { get; set; }
    }
}
