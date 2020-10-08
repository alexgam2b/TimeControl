using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace TimeControl
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string APP_DATA_FILE_NAME = "timecontrol.dat";
        private const string TIME_FORMAT = "hh\\:mm\\:ss";
        
        private delegate void Timer();
        
        private bool running;
        private TimeSpan currentTime;

        public MainWindow()
        {
            running = false;
            InitializeComponent();
            LoadLastTime();
            DisplayCurrentTime();
            DisplayBottonContent();
        }

        private void LoadLastTime()
        {
            try
            {
                using (FileStream stream = new FileStream(APP_DATA_FILE_NAME, FileMode.Open, FileAccess.Read))
                {
                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        currentTime = new TimeSpan(reader.ReadInt64());
                    }
                }
            }
            catch (FileNotFoundException)
            {
                currentTime = new TimeSpan();
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

        private void DisplayCurrentTime()
        {
            display.Content = currentTime.ToString(TIME_FORMAT);
        }

        private void DisplayBottonContent()
        {
            if (running)
            {
                startStopButton.Content = "Pause";
            }
            else if (currentTime.Ticks == 0)
            {
                startStopButton.Content = "Start";
            }
            else
            {
                startStopButton.Content = "Resume";
            }
        }

        private void ClickOnStartStopButton(object sender, RoutedEventArgs e)
        {
            if (running)
            {
                running = false;
                DisplayBottonContent();
            }
            else
            {
                running = true;
                Timer t = new Timer(RunTimer);
                t.BeginInvoke(null, null);
                DisplayBottonContent();
            }
        }

        private void ClickOnResetButton(object sender, RoutedEventArgs e)
        {
            running = false;
            currentTime = new TimeSpan();
            DisplayCurrentTime();
            DisplayBottonContent();
        }

        private void RunTimer()
        {
            DateTime start = DateTime.Now - currentTime;
            while (running)
            {
                currentTime = DateTime.Now - start;
                display.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Timer(DisplayCurrentTime));
                Thread.Sleep(1000);
            }
        }

        private void ClosingMainWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveCurrentTime();
        }

        private void SaveCurrentTime()
        {
            try
            {
                using (FileStream stream = new FileStream(APP_DATA_FILE_NAME, FileMode.Create, FileAccess.Write))
                {
                    using (BinaryWriter writer = new BinaryWriter(stream))
                    {
                        writer.Write(currentTime.Ticks);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex);
            }
        }
    }
}
