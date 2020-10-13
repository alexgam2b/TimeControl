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
        private Random rnd = new Random();
        private int interval = 50;

        [TestMethod]
        public void TestCreation()
        {
            ITimer timer = new Timer();
            Assert.AreEqual(new TimeSpan(), timer.GetElapsedTime());
        }

        [TestMethod]
        public void TestCreationWithTime()
        {
            TimeSpan state = new TimeSpan(0, 20, 205);
            ITimer timer = new Timer(state);
            Assert.AreEqual(state, timer.GetElapsedTime());
        }

        [TestMethod]
        public void TestStart()
        {
            ITimer timer = new Timer();
            timer.Interval = interval;
            timer.Start();
            int delay = timer.Interval * rnd.Next(1, 11);
            Thread.Sleep(delay);
            int elapsed = Convert.ToInt32(timer.GetElapsedTime().TotalMilliseconds);
            Console.WriteLine($"Delay  : { delay }");
            Console.WriteLine($"Elapsed: { elapsed }");
            Assert.IsTrue(timer.IsRunning);
            Assert.IsTrue(Math.Abs(elapsed - delay) < timer.Interval);
        }

        [TestMethod]
        public void TestStop()
        {
            ITimer timer = new Timer();
            timer.Interval = interval;
            timer.Start();
            int delay = timer.Interval * rnd.Next(1, 11);
            Thread.Sleep(delay);
            timer.Stop();
            TimeSpan elapsed = timer.GetElapsedTime();
            Thread.Sleep(timer.Interval * 2);
            Assert.IsFalse(timer.IsRunning);
            Assert.AreEqual(elapsed, timer.GetElapsedTime());
        }

        [TestMethod]
        public void TestReset()
        {
            ITimer timer = new Timer();
            timer.Interval = interval;
            timer.Start();
            Thread.Sleep(2000);
            timer.Reset();
            Thread.Sleep(2000);
            Assert.AreEqual(new TimeSpan(), timer.GetElapsedTime());
        }

        [TestMethod]
        public void TestTicks()
        {
            int count = 0;
            ITimer timer = new Timer();
            timer.Interval = interval;
            timer.Tick += (s, e) => { count++; };
            timer.Start();
            int delay = timer.Interval * 5;
            Thread.Sleep(delay);
            Console.WriteLine($"Count: { count }");
            Assert.IsTrue(Math.Abs(5 - count) <= 1);
        }
    }
}
