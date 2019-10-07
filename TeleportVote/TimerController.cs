﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TeleportVote
{
    internal class TimerController
    {
        private PausableTimer mainTimer;
        private readonly int mainTimerInterval = 15;

        private PausableTimer lockAgainTimer;
        private readonly int lockAgainTimerInterval = 5;

        private PausableTimer finalCountdownTimer;
        private readonly int finalCountdownTimerInterval = 1;

        public void Start()
        {
            mainTimer = new PausableTimer(mainTimerInterval, true);
            mainTimer.Elapsed += MainTimer_Elapsed;

            lockAgainTimer = new PausableTimer(lockAgainTimerInterval, true);
            lockAgainTimer.Elapsed += LockAgainTimer_Elapsed;

            finalCountdownTimer = new PausableTimer(finalCountdownTimerInterval, true);
            finalCountdownTimer.Elapsed += FinalCountdownTimer_Elapsed;

            mainTimer.Start();
            Message.SendColoured("Timer started. Restrictions will be lifted in 60s.", Colours.LightBlue);
        }       

        public void Stop()
        {
            mainTimer.Stop();
            mainLoop = 0;

            lockAgainTimer.Stop();
            lockedAgainLoop = 0;

            finalCountdownTimer.Stop();
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
                mainTimer.Stop();
                mainLoop = 0;
                lockAgainTimer.Start();
            }
        }

        private int lockedAgainLoop = 0;
        private void LockAgainTimer_Elapsed(object sender, EventArgs e)
        {
            var time = (lockedAgainLoop + 1) * lockAgainTimerInterval;
            var timeRemaining = 25 - time;
            if(timeRemaining > 0)
            {
                lockedAgainLoop++;
                Message.SendColoured($"Restrictions will be reinstated in {timeRemaining}s", Colours.Yellow);
            }
            else
            {
                lockAgainTimer.Stop();
                lockedAgainLoop = 0;
                finalCountdownTimer.Start();
            }
        }

        private int countdown = 5;
        private void FinalCountdownTimer_Elapsed(object sender, EventArgs e)
        {
            if(countdown > 0)
            {
                countdown--;
                Message.SendColoured($"{countdown}...", Colours.Red);
            }
            else
            {
                Stop();
                VoteMessage.RestrictionReinstated();
            }
        } 
    }
}