using System;
using Microwave.Classes.Interfaces;

namespace Microwave.Classes.Boundary
{
    public class Timer : ITimer
    {
        public int TimeRemaining { get; private set; }

        public event EventHandler Expired;
        public event EventHandler TimerTick;

        private System.Timers.Timer timer;

        public Timer()
        {
            timer = new System.Timers.Timer();
            // Bind OnTimerEvent with an object of this, and set up the event
            timer.Elapsed += OnTimerEvent;
            timer.Interval = 1000; // 1 second intervals
            timer.AutoReset = true;  // Repeatable timer
        }


        public void Start(int time)
        {
            TimeRemaining = time;
            timer.Enabled = true;
        }

        public void Stop()
        {
            timer.Enabled = false;
        }

        private void Expire()
        {
            timer.Enabled = false;
            Expired?.Invoke(this, System.EventArgs.Empty);
        }

        private void OnTimerEvent(object sender, System.Timers.ElapsedEventArgs args)
        {
            // One tick has passed
            // Do what I should
            if (TimeRemaining > 0)
            {
                TimeRemaining -= 1000; //før stod der -1000 og det virkede ikke
                TimerTick?.Invoke(this, EventArgs.Empty);
            }
            else if (TimeRemaining <= 0)
            {
                Expire();
            }

        }
    }
}