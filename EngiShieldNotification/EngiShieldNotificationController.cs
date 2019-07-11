using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using UnityEngine;

namespace EngiShieldNotification
{
    internal class EngiShieldNotificationController
    {
        private readonly GameObject gameObject;
        private readonly int volume;
        private Timer InitialTimer { get; set; }
        private Timer CountdownTimer { get; set; }

        public EngiShieldNotificationController(EngiShieldNotification parent, GameObject gameObject, double engiShieldLifetime, int noticeTime, int volume)
        {
            parent.OnDestroyExitGameObject += Parent_OnExitGameObjectExit;
            this.gameObject = gameObject;
            this.volume = volume;

            InitialTimer = new Timer((engiShieldLifetime - noticeTime - 1) * 1000)
            {
                AutoReset = false,
                Enabled = false
            };
            InitialTimer.Elapsed += InitialTimer_Elapsed;

            countdownTimerLoopValue = noticeTime;
            CountdownTimer = new Timer(1 * 1000)
            {
                AutoReset = true,
                Enabled = false
            };
            CountdownTimer.Elapsed += CountdownTimer_Elapsed;
        }        

        public void Start()
        {
            InitialTimer.Start();
        }

        private void Stop()
        {
            CountdownTimer.Stop();
            countdownTimerLoopValue = 5;
            InitialTimer.Stop();
        }

        private void InitialTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CountdownTimer.Start();
        }

        private int countdownTimerLoopValue;

        private void CountdownTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (volume > 0)
            {
                for(var i =0; i < volume; i++)
                {
                    Util.PlaySound(SoundStrings.Default, gameObject);
                }             
            }            
            
            --countdownTimerLoopValue;
            if (countdownTimerLoopValue <= 0)
            {
                Stop();
            }
        }

        private void Parent_OnExitGameObjectExit(object sender, EventArgs e)
        {
            var args = e as EngiShieldNotification.OnDestroyExitGameObjectEventArgs;
            if (args.ExitGameObject == this.gameObject)
            {
                Stop();
            }
        }
    }
}
