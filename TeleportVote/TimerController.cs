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
        private Timer timer;

        public TimerController(int interval)
        {
            timer = new Timer
            {
                AutoReset = true,
                Interval = interval * 1000,
                Enabled = false
            };            
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }
    }
}
