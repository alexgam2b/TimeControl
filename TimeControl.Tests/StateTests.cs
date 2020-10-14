using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace TimeControl.Tests
{
    [TestClass]
    public class StateTests
    {
        private static Semaphore semaphore = new Semaphore(1, 1);

        [TestMethod]
        public void TestCreation()
        {
            semaphore.WaitOne();
            State state = new State();
            bool isActive = state.IsActive;
            state.Dispose();
            semaphore.Release();
            Assert.IsTrue(isActive);
        }

        [TestMethod]
        public void TestMultipleCreation()
        {
            semaphore.WaitOne();
            State firstState = new State();
            State secondState = new State();
            bool firstIsActive = firstState.IsActive;
            bool secondIsActive = secondState.IsActive;
            firstState.Dispose();
            secondState.Dispose();
            semaphore.Release();
            Assert.IsTrue(firstIsActive);
            Assert.IsFalse(secondIsActive);
        }

        [TestMethod]
        public void TestSavingAndLoading()
        {
            semaphore.WaitOne();
            State state = new State();
            TimeSpan startedTime = new TimeSpan(54321);
            state.Save(startedTime);
            TimeSpan loadedTime = state.Load();
            state.Dispose();
            semaphore.Release();
            Assert.AreEqual(startedTime, loadedTime);
        }

        [TestMethod]
        public void TestReSaving()
        {
            semaphore.WaitOne();
            State state = new State();
            TimeSpan oldTime = new TimeSpan(12345);
            TimeSpan newTime = new TimeSpan(5678);
            state.Save(oldTime);
            state.Save(newTime);
            TimeSpan loadedTime = state.Load();
            state.Dispose();
            semaphore.Release();
            Assert.AreEqual(newTime, loadedTime);
        }

        [TestMethod]
        public void TestLoadingEmpty()
        {
            semaphore.WaitOne();
            if (File.Exists(State.STATE_FILE_NAME))
            {
                File.Delete(State.STATE_FILE_NAME);
            }
            State state = new State();
            TimeSpan time = state.Load();
            state.Dispose();
            semaphore.Release();
            Assert.AreEqual(new TimeSpan(), time);
        }
    }
}
