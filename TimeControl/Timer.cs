using System;
using System.Threading;
using System.Threading.Tasks;

namespace TimeControl
{
    public interface ITimer
    {
        int Interval { get; set; }
        bool IsRunning { get; }

        TimeSpan GetElapsedTime();
        void Reset();
        void Start();
        void Stop();

        event EventHandler<TimerEventArgs> Tick;
    }

    public class TimerEventArgs: EventArgs { }

    public sealed class Timer: ITimer
    {
        private TimeSpan elapsedTime;
        private DateTime startedTime;
        private CancellationTokenSource cts;

        /// <summary>
        /// Interval in milliseconds.
        /// </summary>
        public int Interval { get; set; }
        public bool IsRunning { get; private set; }

        public event EventHandler<TimerEventArgs> Tick;

        public Timer(TimeSpan elapsed = new TimeSpan())
        {
            Interval = 1000;
            IsRunning = false;
            startedTime = new DateTime();
            elapsedTime = elapsed;
        }

        ~Timer()
        {
            cts?.Dispose();
        }

        public void Start()
        {
            if (!IsRunning)
            {
                IsRunning = true;
                startedTime = DateTime.Now - elapsedTime;
                cts = new CancellationTokenSource();
                Task.Run(() => StartTimer(cts.Token));
            }
        }

        private void StartTimer(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Thread.Sleep(Interval);
                Tick?.Invoke(this, new TimerEventArgs());
            }
        }

        public void Stop()
        {
            if (IsRunning)
            {
                IsRunning = false;
                SetElapsedTime();
                cts.Cancel();
                cts.Dispose();
            }
        }

        public TimeSpan GetElapsedTime()
        {
            if (IsRunning)
            {
                SetElapsedTime();
            }
            return elapsedTime;
        }

        private void SetElapsedTime()
        {
            elapsedTime = DateTime.Now - startedTime;
        }

        public void Reset()
        {
            Stop();
            elapsedTime = new TimeSpan();
        }
    }
}
