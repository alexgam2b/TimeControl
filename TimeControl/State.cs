using System;
using System.IO;
using System.Text;
using System.Windows;

namespace TimeControl
{
    public interface IState
    {
        bool IsActive { get; }
        TimeSpan Load();
        void Save(TimeSpan timer);
    }

    public sealed class State: IState, IDisposable
    {
        public const string STATE_FILE_NAME = "timecontrol.stt";

        private bool disposed = false;
        private FileStream stream = null;

        public bool IsActive => !disposed;

        public State()
        {
            try
            {
                stream = new FileStream(STATE_FILE_NAME, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex);
            }
            finally
            {
                disposed = stream == null;
            }
        }

        public TimeSpan Load()
        {
            if (IsActive == false)
            {
                return new TimeSpan();
            }
            return TryLoad();
        }

        private TimeSpan TryLoad()
        {
            try
            {
                using (BinaryReader reader = new BinaryReader(stream, Encoding.Unicode, true))
                {
                    stream.Position = 0;
                    long ticks = reader.ReadInt64();
                    return new TimeSpan(ticks);
                }
            }
            catch
            {
                return new TimeSpan();
            }
        }

        public void Save(TimeSpan time)
        {
            if (IsActive == false)
            {
                return;
            }
            TrySave(time);
        }

        private void TrySave(TimeSpan time)
        {
            try
            {
                using (BinaryWriter writer = new BinaryWriter(stream, Encoding.Unicode, true))
                {
                    stream.Position = 0;
                    writer.Write(time.Ticks);
                }
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex);
            }
        }
        
        private void ShowExceptionMessage(Exception ex)
        {
            MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        ~State() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing)
            {
                stream?.Dispose();
            }
            disposed = true;
        }
    }
}
