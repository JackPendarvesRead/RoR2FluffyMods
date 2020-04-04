//using RoR2;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace TeleportVote
//{
//    internal class VoteRegistrationController
//    {
//        public bool PlayersCanVote { get; set; } = true;

//        private bool hostOverride = false;
//        public bool VotesReady
//        {
//            get
//            {
//                if(RegisteredPlayers.Count >= VotesNeeded)
//                {
//                    return true;
//                }
//                return false;
//            }
//        }

//        private int VotesNeeded
//        {
//            get
//            {
//                if (hostOverride)
//                {
//                    return 0;
//                }

//                var livingPlayerCount = RoR2.Run.instance.livingPlayerCount;
//                if (!TeleportVote.MaximumVotes.Condition)
//                {
//                    return livingPlayerCount;
//                }
//                else
//                {
//                    return livingPlayerCount < TeleportVote.MaximumVotes.Value                            
//                        ? livingPlayerCount                            
//                        : TeleportVote.MaximumVotes.Value;
//                }                
//            }
//        }

//        private List<NetworkUserId> RegisteredPlayers { get; set; } = new List<NetworkUserId>();

//        public void RegisterPlayer(NetworkUser netUser)
//        {
//            var netId = netUser.Network_id;
//            if (PlayersCanVote && !RegisteredPlayers.Contains(netId))
//            {
//                RegisteredPlayers.Add(netId);
//                Message.SendColoured($"{RegisteredPlayers.Count}/{VotesNeeded} players are ready", Colours.Green);
//            }
//        }

//        public void HostOverride()
//        {
//            if (!hostOverride)
//            {
//                hostOverride = true;
//                PlayersCanVote = false;
//                Message.SendColoured("Host has overriden restrictions.", Colours.Green);
//            }
//        }

//        public void Reset()
//        {
//            RegisteredPlayers.Clear();
//            PlayersCanVote = true;
//            hostOverride = false;
//        }
//    }
//}
