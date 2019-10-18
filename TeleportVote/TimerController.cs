using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TeleportVote
{
    internal class TimerController
    {
        public bool TimerRestrictionsLifted { get; set; } = false;

        private PausableTimer mainTimer;
        private readonly int mainTimerInterval = 15;

        private PausableTimer lockAgainTimer;
        private readonly int lockAgainTimerInterval = 5;

        private PausableTimer finalCountdownTimer;
        private readonly int finalCountdownTimerInterval = 1;

        private bool timerRunning = false;
        public void Start()
        {
            if (!timerRunning)
            {
                TimerRestrictionsLifted = false;

                timerRunning = true;
                mainTimer = new PausableTimer(mainTimerInterval, true);
                mainTimer.Elapsed += MainTimer_Elapsed;

                lockAgainTimer = new PausableTimer(lockAgainTimerInterval, true);
                lockAgainTimer.Elapsed += LockAgainTimer_Elapsed;

                finalCountdownTimer = new PausableTimer(finalCountdownTimerInterval, true);
                finalCountdownTimer.Elapsed += FinalCountdownTimer_Elapsed;

                mainTimer.Start();
                Message.SendColoured("Timer started. Restriction will be lifted in 60s", Colours.LightBlue);
            }           
        }       

        public void Stop()
        {
            TimerRestrictionsLifted = false;

            timerRunning = false;

            mainTimer?.Dispose();
            mainLoop = 0;

            lockAgainTimer?.Dispose();
            lockedAgainLoop = 0;

            finalCountdownTimer?.Dispose();
            countdown = 5;
        }

        private int mainLoop = 0;
        private void MainTimer_Elapsed(object sender, EventArgs e)
        {
            var time = (mainLoop + 1) * mainTimerInterval;
            var timeRemaining = 60 - time;
            if(timeRemaining > 0)
            {
                mainLoop++;
                Message.SendColoured($"{timeRemaining} until restrictions are lifted", Colours.LightBlue);
            }
            else
            {
                TimerRestrictionsLifted = true;
                mainTimer.Dispose();
                mainLoop = 0;
                lockAgainTimer.Start();
                Message.SendColoured($"Restriction lifted. Restriction reinstated in 30s", Colours.Green);
            }
        }

        private int lockedAgainLoop = 0;
        private void LockAgainTimer_Elapsed(object sender, EventArgs e)
        {
            var time = (lockedAgainLoop + 1) * lockAgainTimerInterval;
            var timeRemaining = 30 - time;
            if(timeRemaining > 5)
            {
                lockedAgainLoop++;
                if(timeRemaining % 10 == 0)
                {
                    Message.SendColoured($"Restrictions will be reinstated in {timeRemaining}s", Colours.Yellow);
                }
            }
            else
            {
                lockAgainTimer.Dispose();
                lockedAgainLoop = 0;
                finalCountdownTimer.Start();
            }
        }

        private int countdown = 5;
        private void FinalCountdownTimer_Elapsed(object sender, EventArgs e)
        {
            if(countdown > 0)
            {
                Message.SendColoured($"{countdown}...", Colours.Orange);
                countdown--;
            }
            else
            {
                Stop();
                VoteMessage.RestrictionReinstated();
            }
        } 
    }
}
