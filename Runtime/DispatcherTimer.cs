using System;
using System.Collections.Generic;

namespace NCV.Network.WS
{
    public delegate void DispatcherTimerEventHandler(object sender, EventArgs args);

    public class DispatcherTimer : IDisposable
    {
        public DispatcherTimerEventHandler Tick;

        private bool started;

        private long lastTicks;

        private TimeSpan interval = TimeSpan.Zero;

        public TimeSpan Interval
        {
            get => interval;
            set => interval = value;
        }

        public bool Started
        {
            get => started;
            set => started = value;
        }

        public DispatcherTimer()
        {
            DispatcherTimerDriver.AddTimer(this);
        }

        public void Start()
        {
            started = true;
            lastTicks = DateTime.Now.Ticks;
        }

        public void Stop()
        {
            started = false;
        }

        public void Dispose()
        {
            DispatcherTimerDriver.RemoveTimer(this);
        }

        public void ExecuteTimer()
        {
            long ticks = DateTime.Now.Ticks;
            if (ticks - lastTicks < interval.Ticks)
            {
                return;
            }

            lastTicks = ticks;
            Tick?.Invoke(this, EventArgs.Empty);
        }
    }


    public static class DispatcherTimerDriver
    {
        private static List<DispatcherTimer> timersList = new();

        public static void AddTimer(DispatcherTimer timer)
        {
            timersList.Add(timer);
        }

        public static void RemoveTimer(DispatcherTimer timer)
        {
            timersList.Remove(timer);
        }

        public static void ClearAll()
        {
            timersList.Clear();
        }

        public static void ExecuteTimers()
        {
            int count = timersList.Count;
            for (int i = 0; i < count; i++)
            {
                timersList[i].ExecuteTimer();
            }
        }
    }
}
