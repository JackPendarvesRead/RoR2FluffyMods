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
    internal class EngiShieldNotificationTimer
    {
        internal readonly GameObject gameObject;
        private float currentTime;
        bool isCountdown;

        public EngiShieldNotificationTimer(GameObject gameObject)
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
                    var volume = EngiShieldNotification.Volume.Value;
                    if (volume > 0)
                    {
                        for (var i = 0; i < volume; i++)
                        {
                            Util.PlaySound(SoundStrings.Default, gameObject);
                        }
                    }
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
