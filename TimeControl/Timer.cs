using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using System.Xml.Serialization;

namespace TimeControl
{
    public interface ITimer
    {
        TimeSpan ElapsedTime { get; }
        void Reset();
        void Start();
        void Stop();
    }

    [Serializable]
    public sealed class Timer: ITimer
    {
        public const int Interval = 1000;

        private CancellationTokenSource cts;
        private Task task;
        private TimeSpan elapsedTime;
        private bool running;

        public TimeSpan ElapsedTime
        {
            get { return elapsedTime; }
        }

        public Timer(TimeSpan elapsedTime = new TimeSpan())
        {
            running = false;
            this.elapsedTime = elapsedTime;
        }

        public void Start()
        {
            if (!running)
            {
                cts = new CancellationTokenSource();
                task = Task.Run(() => StartTimer(cts.Token));
                running = true;
            }
        }

        private void StartTimer(CancellationToken token)
        {
            DateTime start = DateTime.Now - elapsedTime;
            while (token.IsCancellationRequested == false)
            {
                Thread.Sleep(Interval);
                elapsedTime = DateTime.Now - start;
            }
        }

        public void Stop()
        {
            if (running)
            {
                cts.Cancel();
                cts.Dispose();
                task.Wait();
                running = false;
            }
        }

        public void Reset()
        {
            Stop();
            elapsedTime = new TimeSpan();
        }
    }
}
