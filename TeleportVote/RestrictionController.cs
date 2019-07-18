using RoR2;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Timers;

namespace TeleportVote
{
    internal class RestrictionController
    {
        private int NumberLivingPlayers { get => RoR2.Run.instance.livingPlayerCount; }
        private List<NetworkUserId> PlayerIdList = new List<NetworkUserId>();
        private Stopwatch sw = new Stopwatch();
        private Timer Timer;
        private Timer TimeoutTimer;
        private readonly int interval;
        private readonly int timeLimit;
        private bool TimeRestrictionIsApplied;

        public bool TeleporterIsCharging { get; set; }

        public RestrictionController(int interval, int timeLimit)
        {            
            this.interval = interval;
            this.timeLimit = timeLimit;
            TimeRestrictionIsApplied = true;
            TeleporterIsCharging = false;

            Timer = new Timer
            {
                AutoReset = true,
                Interval = interval * 1000,
                Enabled = false
            };
            timerElapsedCount = 0;
            Timer.Elapsed += Timer_Elapsed;

            TimeoutTimer = new Timer
            {
                AutoReset = true,
                Interval = 1 * 1000,
                Enabled = false
            };
            timeoutTimerElapsedCount = 0;
            TimeoutTimer.Elapsed += TimeoutTimer_Elapsed;
        }

        private void CheckTimerStarts()
        {
            if (TimeRestrictionIsApplied)
            {
                if (!sw.IsRunning)
                {
                    sw.Start();
                }
                if (!Timer.Enabled)
                {
                    Timer.Start();
                }
            }            
        }

        /// <summary>
        /// Runs logic to check if all interaction is legal. Either all players need to be ready or activate it during unrestricted time window.
        /// </summary>
        /// <param name="netUser">Unique user Id for player</param>
        /// <returns>True if interactor is able to perform interaction</returns>
        public bool IsInteractionLegal(NetworkUser netUser)
        {            
            if (!TeleporterIsCharging)
            {
                CheckTimerStarts();
                bool sendMessage = false;
                if (!PlayerIdList.Contains(netUser.Network_id))
                {
                    PlayerIdList.Add(netUser.Network_id);
                    sendMessage = true;
                }
                if (PlayerIdList.Count >= NumberLivingPlayers || !TimeRestrictionIsApplied)
                {
                    Stop();                    
                    Message.SendToAll("Activated! Go go go!", Colours.Green);
                    return true;
                }
                if (sendMessage)
                {
                    var timeRemaining = Math.Round(this.timeLimit - sw.ElapsedMilliseconds / 1000.0, 1);
                    Message.SendToAll($"{PlayerIdList.Count}/{NumberLivingPlayers} players are ready. {timeRemaining}s until restriction is lifted.", Colours.LightBlue);
                }
            }
            return false;
        }

        public void ChatCommandReady(NetworkUser netUser)
        {
            if (!TeleporterIsCharging)
            {
                CheckTimerStarts();
                if (!PlayerIdList.Contains(netUser.Network_id))
                {
                    PlayerIdList.Add(netUser.Network_id);
                    var timeRemaining = Math.Round(this.timeLimit - sw.ElapsedMilliseconds / 1000.0, 1);
                    Message.SendToAll($"{PlayerIdList.Count}/{NumberLivingPlayers} players are ready. {timeRemaining}s until restriction is lifted.", Colours.LightBlue);
                }   
            }
        }

        #region Timers
        private int timerElapsedCount;
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var time = interval * (1 + timerElapsedCount);
            if (time < timeLimit)
            {
                Message.SendToAll($"{timeLimit - time}s until restriction is lifted.", Colours.LightBlue);
                timerElapsedCount++;
            }
            else
            {
                Message.SendToAll("Restrictions lifted.", Colours.Green);
                Timer.Stop();
                TimeRestrictionIsApplied = false;
                TimeoutTimer.Start();
            }
        }

        private int timeoutTimerElapsedCount;
        private void TimeoutTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            int timeoutTime = 30;
            var time = 1 * (timeoutTimerElapsedCount + 1);
            if (time >= timeoutTime)
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
                else if (timeRemaining % 10 == 0)
                {
                    Message.SendToAll($"{timeRemaining}s until restriction is reinstated", Colours.Yellow);
                }
                timeoutTimerElapsedCount++;
            }
        }
        #endregion        

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

            TimeRestrictionIsApplied = true;
        }
    }
}
