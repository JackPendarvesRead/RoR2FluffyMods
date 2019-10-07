using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace TeleportVote
{
    public sealed class PausableTimer : IDisposable
    {
        public event EventHandler Elapsed;

        private Timer timer;
        private Timer pauseTimer;
        private Stopwatch sw;

        /// <summary>
        /// Pausable System.Timers.Timer (default autoreset = true)
        /// </summary>
        /// <param name="interval">Time(s) for timer to elapse</param>
        public PausableTimer(int interval) : this(interval, true) { }
        /// <summary>
        /// Pausable System.Timers.Timer
        /// </summary>
        /// <param name="interval">Time(s) for timer to elapse</param>
        /// <param name="autoReset">Timer should autoreset</param>
        public PausableTimer(int interval, bool autoReset)
        {
            timer = new Timer
            {
                Enabled = false,
                AutoReset = autoReset,
                Interval = interval * 1000
            };
            sw = new Stopwatch();

            timer.Elapsed += Timer_Elapsed;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {            
            Elapsed.Invoke(this, new EventArgs());
            RestartOrDispose();
        }

        private void PauseTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Elapsed.Invoke(this, new EventArgs());
            pauseTimer.Dispose();
            RestartOrDispose();
        }

        public void Start()
        {
            timer.Start();
            sw.Restart();
            DisposePauseTimer();
        }

        public void Stop()
        {
            timer.Stop();
            sw.Reset();
            DisposePauseTimer();
        }

        public void Pause()
        {
            timer.Stop();
            sw.Stop();
            DisposePauseTimer();
        }

        public void UnPause()
        {
            if (sw.ElapsedMilliseconds > 0)
            {
                var timeRemaining = timer.Interval - sw.ElapsedMilliseconds;
                pauseTimer = new Timer
                {
                    Enabled = false,
                    AutoReset = false,
                    Interval = timeRemaining
                };
                pauseTimer.Elapsed += PauseTimer_Elapsed;
                pauseTimer.Start();
                sw.Start();
            }
        }

        public void Dispose()
        {
            timer.Dispose();
            if (sw != null)
            {
                sw = null;
            }
            DisposePauseTimer();
        }

        private void RestartOrDispose()
        {
            if (timer.AutoReset)
            {
                Start();
            }
            else
            {
                Dispose();
            }
        }

        private void DisposePauseTimer()
        {
            if (pauseTimer != null)
            {
                pauseTimer.Dispose();
            }
        }
    }
}
