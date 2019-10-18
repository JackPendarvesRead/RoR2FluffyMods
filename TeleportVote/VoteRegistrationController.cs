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
        public bool PlayersCanVote { get; set; } = true;

        public bool VotesReady
        {
            get
            {
                if(RegisteredPlayers.Count >= VotesNeeded)
                {
                    return true;
                }
                return false;
            }
        }

        private int VotesNeeded
        {
            get
            {
                return RoR2.Run.instance.livingPlayerCount < TeleportVote.MaximumVotes.Value 
                    ? RoR2.Run.instance.livingPlayerCount 
                    : TeleportVote.MaximumVotes.Value;
            }
        }

        private List<NetworkUserId> RegisteredPlayers { get; set; } = new List<NetworkUserId>();

        //public event EventHandler PlayerRegistered;

        public void RegisterPlayer(NetworkUser netUser)
        {
            var netId = netUser.Network_id;
            if (PlayersCanVote && !RegisteredPlayers.Contains(netId))
            {
                RegisteredPlayers.Add(netId);
                if (VotesReady)
                {
                    return;
                }
                else
                {
                    Message.SendColoured($"{RegisteredPlayers.Count}/{VotesNeeded} players are ready", Colours.Green);
                }
            }
        }

        public void Reset()
        {
            RegisteredPlayers.Clear();
            PlayersCanVote = true;
        }
    }

    //internal class PlayerRegisteredEventArgs : EventArgs
    //{
    //    public PlayerRegisteredEventArgs(NetworkUser registeredPlayer, bool ready, int numberOfRegisteredPlayers, int numberOfVotesNeeded)
    //    {
    //        RegisteredPlayer = registeredPlayer;
    //        Ready = ready;
    //        NumberOfRegisteredPlayers = numberOfRegisteredPlayers;
    //        NumberOfVotesNeeded = numberOfVotesNeeded;
    //    }

    //    public NetworkUser RegisteredPlayer { get; }
    //    public bool Ready { get; }
    //    public int NumberOfRegisteredPlayers { get; }
    //    public int NumberOfVotesNeeded { get; }
    //}
}
