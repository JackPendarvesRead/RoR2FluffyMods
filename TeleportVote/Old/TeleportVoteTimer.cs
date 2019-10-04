//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Timers;

//namespace TeleportVote
//{
//    internal class TeleportVoteTimer
//    {

//        public int EllapsedCount { get; set; }

//        private Timer Timer;
//        private readonly int interval;
//        private readonly int timeLimit;

//        public TeleportVoteTimer(int interval, bool reset, Action ellapsedAction, Action finalAction)
//        {
//            Timer = new Timer
//            {
//                AutoReset = reset,
//                Enabled = false,
//                Interval = interval * 1000
//            };
//            EllapsedCount = 0;
//            this.interval = interval;
//        }

       
//        public void Start()
//        {
//            Timer.Start();
//        }

//        public void Stop()
//        {
//            Timer.Stop();
//        }

//        public int GetEllapsedTime()
//        {
//            return (EllapsedCount + 1) * interval;
//        }
//    }
//}
