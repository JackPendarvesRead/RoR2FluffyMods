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
        
        private Timer InitialTimer { get; set; }
        private Timer CountdownTimer { get; set; }
        
        private static string SoundString => "Play_engi_R_place";
        private readonly GameObject gameObject;

        public EngiShieldNotificationController(EngiShieldNotification parent, GameObject gameObject, double engiShieldLifetime, int noticeTime)
        {
            parent.OnDestroyExitGameObject += Parent_OnExitGameObjectExit;

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
            this.gameObject = gameObject;
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
            var num = Util.PlaySound(SoundString, gameObject);
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
