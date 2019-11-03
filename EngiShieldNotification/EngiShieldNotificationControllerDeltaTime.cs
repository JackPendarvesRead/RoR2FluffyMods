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
    internal class EngiShieldNotificationControllerDeltaTime
    {
        internal readonly GameObject gameObject;
        private float currentTime;
        bool isCountdown;

        public EngiShieldNotificationControllerDeltaTime(GameObject gameObject)
        {
            this.gameObject = gameObject;
            currentTime = EngiShieldNotification.ShieldTime;
            isCountdown = false;
        }   

        public void Update(float time)
        {
            currentTime -= time;
            if(currentTime < 0)
            {
                if (isCountdown)
                {
                    currentTime = 1.0f;
                    // play sound
                }
                else
                {
                    isCountdown = true;
                    currentTime = 1.0f;
                }
            }
        }
    }
}
