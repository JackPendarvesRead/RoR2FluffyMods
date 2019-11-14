using TeleportVote.StaticStuff;
using UnityEngine;

namespace TeleportVote
{
    enum TimerState
    {
        Stopped,
        InitialCountdown,
        RestrictionsLifted,
        FinalCountdown
    }

    internal class TimerController
    {
        public bool TimerRestrictionsLifted
        {
            get
            {
                if(currentState == TimerState.RestrictionsLifted)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public TimerController()
        {
            currentState = TimerState.Stopped;
        }

        private TimerState currentState;
        private float currentTime;

        public void Update(float deltaTime)
        {
            if(currentState != TimerState.Stopped)
            {
                currentTime -= deltaTime;
                if (currentTime < 0)
                {
                    switch (currentState)
                    {
                        case TimerState.InitialCountdown:
                            InitTimerElapsed();
                            break;

                        case TimerState.RestrictionsLifted:
                            LockAgainTimerElapsed();
                            break;

                        case TimerState.FinalCountdown:
                            FinalCountDownElapsed();
                            break;

                        default:
                            Stop();
                            Debug.LogError("Default switch in TimerController reached. This should never happen. Please let @Fluffatron know if this happens.");
                            break;
                    }
                }
            }           
        }

        public void Start()
        {
            if (currentState == TimerState.Stopped)
            {
                currentState = TimerState.InitialCountdown;
                currentTime = TimerConstants.InitialTimerInterval;
                Message.SendColoured("Timer started. Restriction will be lifted in 60s", Colours.LightBlue);
            }           
        }       

        public void Stop()
        {
            currentState = TimerState.Stopped;
            mainLoop = 0;
            lockedAgainLoop = 0;
            countdown = 5;
        }

        private int mainLoop = 0;
        private void InitTimerElapsed()
        {
            var time = (mainLoop + 1) * TimerConstants.InitialTimerInterval;
            var timeRemaining = TimerConstants.InitialTimer - time;
            if (timeRemaining > 0)
            {
                mainLoop++;
                currentTime = TimerConstants.InitialTimerInterval;
                Message.SendColoured($"{timeRemaining} until restrictions are lifted", Colours.LightBlue);
            }
            else
            {
                mainLoop = 0;
                currentState = TimerState.RestrictionsLifted;
                currentTime = TimerConstants.RestrictionTimerInterval;
                Message.SendColoured($"Restriction lifted. Restriction reinstated in {TimerConstants.RestrictionTimer}s", Colours.Green);
            }
        }

        private int lockedAgainLoop = 0;
        private void LockAgainTimerElapsed()
        {
            var time = (lockedAgainLoop + 1) * TimerConstants.RestrictionTimerInterval;
            var timeRemaining = TimerConstants.RestrictionTimer - time;
            if (timeRemaining > 5)
            {
                lockedAgainLoop++;
                currentTime = TimerConstants.RestrictionTimerInterval;
                if (timeRemaining % 10 == 0)
                {
                    Message.SendColoured($"Restrictions will be reinstated in {timeRemaining}s", Colours.Yellow);
                }
            }
            else
            {
                lockedAgainLoop = 0;
                currentTime = 1;
                currentState = TimerState.FinalCountdown;                
            }
        }

        private int countdown = 5;
        private void FinalCountDownElapsed()
        {
            if (countdown > 0)
            {
                Message.SendColoured($"{countdown}...", Colours.Orange);
                countdown--;
                currentTime = 1;
            }
            else
            {
                Stop();
                VoteMessage.RestrictionReinstated();
            }
        }
    }
}
