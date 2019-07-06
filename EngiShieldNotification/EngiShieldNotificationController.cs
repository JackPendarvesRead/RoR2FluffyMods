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
        private Timer ShieldTimer { get; set; }
        private Timer CountdownTimer { get; set; }
        private static string SoundString => "Play_engi_R_place";
        private readonly GameObject gameObject;

        public EngiShieldNotificationController(GameObject gameObject)
        {
            ShieldTimer = new Timer(11 * 1000)
            {
                AutoReset = false,
                Enabled = false
            };
            ShieldTimer.Elapsed += ShieldTimer_Elapsed;

            countdownValue = 3;
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
            ShieldTimer.Start();
        }

        private void Stop()
        {
            CountdownTimer.Stop();
            countdownValue = 5;
            ShieldTimer.Stop();
        }

        private void ShieldTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CountdownTimer.Start();
        }

        private int countdownValue;
        private void CountdownTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var num = Util.PlaySound(SoundString, gameObject);
            --countdownValue;
            if (countdownValue <= 0)
            {
                Stop();
            }
        }

        
    }
}
