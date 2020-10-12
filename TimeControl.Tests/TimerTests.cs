using TimeControl;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace TimeControl.Tests
{
    [TestClass]
    public class TimerTests
    {
        [TestMethod]
        public void TestCreation()
        {
            ITimer timer = new Timer();
            Assert.AreEqual(new TimeSpan(), timer.ElapsedTime);
        }

        [TestMethod]
        public void TestCreationWithTime()
        {
            TimeSpan state = new TimeSpan(0, 40, 20);
            ITimer timer = new Timer(state);
            Assert.AreEqual(state, timer.ElapsedTime);
        }

        private readonly object ConsoleWriterLock = new object();
        [TestMethod]
        public void TestStart() // Need starts many times
        {
            Random rnd = new Random();
            Parallel.For(1, 3, i =>
            {
                ITimer timer = new Timer();
                timer.Start();
                int delay;
                lock (rnd)
                {
                    delay = Timer.Interval * rnd.Next(1, 11);
                }
                Thread.Sleep(delay);
                lock (ConsoleWriterLock)
                {
                    Console.WriteLine($"Thread: { Thread.CurrentThread.ManagedThreadId }");
                    Console.WriteLine($"Delay: { delay }");
                    Console.WriteLine($"Timer: { timer.ElapsedTime.TotalMilliseconds }");
                }
                Assert.IsTrue(Math.Abs(timer.ElapsedTime.TotalMilliseconds - delay) < Timer.Interval);
            });
            
        }

        [TestMethod]
        public void TestStop()
        {
            ITimer timer = new Timer();
            timer.Start();
            Thread.Sleep(2000);
            timer.Stop();
            TimeSpan current = timer.ElapsedTime;
            Thread.Sleep(2000);
            Assert.AreEqual(current, timer.ElapsedTime);
        }

        [TestMethod]
        public void TestReset()
        {
            ITimer timer = new Timer();
            timer.Start();
            Thread.Sleep(2000);
            timer.Reset();
            Thread.Sleep(2000);
            Assert.AreEqual(new TimeSpan(), timer.ElapsedTime);
        }
    }
}
