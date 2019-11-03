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
            Debug.Log($"Current time = {currentTime}");
            if(currentTime < 0)
            {
                Debug.Log("COUNTDOWN DONE!");
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
