using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;

namespace TeleportVote
{
    internal class RestrictionController
    {
        //private int NumberLivingPlayers { get => RoR2.Run.instance.participatingPlayerCount; }
        private int NumberLivingPlayers { get => RoR2.Run.instance.livingPlayerCount; }
        private List<string> PlayerIdList { get; set; }
        private Stopwatch sw { get; set; }
        private Timer Timer { get; set; }
        private Timer TimeoutTimer { get; set; }
        private readonly int interval;
        private readonly int timeLimit;
        private bool IsTimeRestrictionApplied { get; set; }

        public RestrictionController(int interval, int timeLimit)
        {
            PlayerIdList = new List<string>();
            this.interval = interval;
            this.timeLimit = timeLimit;
            timerElapsedCount = 0;
            timeoutTimerElapsedCount = 0;
            IsTimeRestrictionApplied = true;

            sw = new Stopwatch();

            Timer = new Timer
            {
                AutoReset = true,
                Interval = interval * 1000,
                Enabled = false
            };
            Timer.Elapsed += Timer_Elapsed;

            TimeoutTimer = new Timer
            {
                AutoReset = true,
                Interval = 1 * 1000,
                Enabled = false
            };
            TimeoutTimer.Elapsed += TimeoutTimer_Elapsed;
        }

        public bool IsInteractionLegal(string id)
        {
            bool sendMessage = false;
            if (!sw.IsRunning)
            {
                sw.Start();
            }
            if (!PlayerIdList.Contains(id))
            {
                PlayerIdList.Add(id);
                sendMessage = true;
            }
            if (PlayerIdList.Count >= NumberLivingPlayers || !IsTimeRestrictionApplied)
            {
                Stop();
                Message.SendToAll("Activated! Go go go!", Colours.Green);
                return true;
            }
            else
            {
                if (!Timer.Enabled)
                {
                    Timer.Start();
                }
                if(sendMessage)
                {
                    var timeRemaining = Math.Round(this.timeLimit - sw.ElapsedMilliseconds / 1000.0, 1);
                    Message.SendToAll($"{PlayerIdList.Count}/{NumberLivingPlayers} players are ready. {timeRemaining}s until restriction is lifted.", Colours.BluePurple);
                }
                return false;
            }
        }        

        private int timerElapsedCount;
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var time = interval * (1 + timerElapsedCount);
            if(time < timeLimit)
            {
                Message.SendToAll($"{timeLimit-time}s until restriction is lifted.", Colours.BluePurple);
                timerElapsedCount++;
            }
            else
            {
                Message.SendToAll("Restrictions lifted.", Colours.Green);
                Timer.Stop();
                IsTimeRestrictionApplied = false;
                TimeoutTimer.Start();
            }
        }

        private int timeoutTimerElapsedCount;
        private void TimeoutTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            int timeoutTime = 30;
            var time = 1 * (timeoutTimerElapsedCount + 1);
            if(time >= timeoutTime)
            {
                Message.SendToAll("Restrictions have been reinstated. You must vote again.", Colours.Red);
                Stop();
            }
            else
            {
                var timeRemaining = timeoutTime - time;
                if (timeRemaining <= 5)
                {
                    Message.SendToAll($"{timeRemaining}...", Colours.Orange);
                }
                else if(timeRemaining % 10 == 0)
                {
                    Message.SendToAll($"{timeRemaining}s until restriction is reinstated", Colours.Yellow);
                }
                timeoutTimerElapsedCount++;
            }
        }      
        
        public void AddChatReady(string name)
        {      
           var userNetId = (from u in RoR2.NetworkUser.readOnlyInstancesList
                             where u.userName == name
                             select u.netId).FirstOrDefault();            

            if (userNetId != null && !PlayerIdList.Contains(userNetId.ToString()))
            {
                PlayerIdList.Add(userNetId.ToString());
                var timeRemaining = Math.Round(this.timeLimit - sw.ElapsedMilliseconds / 1000.0, 1);
                Message.SendToAll($"{PlayerIdList.Count}/{NumberLivingPlayers} players are ready. {timeRemaining}s until restriction is lifted.", Colours.BluePurple);
            }  
        }

        /// <summary>
        /// Sets time restriction to true and clears and resets all lists, timers and stopwatches. 
        /// </summary>
        public void Stop()
        {            
            PlayerIdList.Clear();
            sw.Reset();

            if(Timer != null && Timer.Enabled)
            {
                Timer.Stop();
            }
            timerElapsedCount = 0;

            if (TimeoutTimer != null && TimeoutTimer.Enabled)
            {
                TimeoutTimer.Stop();
            }
            timeoutTimerElapsedCount = 0;

            IsTimeRestrictionApplied = true;
        }
    }
}
