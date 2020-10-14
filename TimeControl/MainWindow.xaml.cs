using System;
using System.Windows;
using System.Windows.Threading;

namespace TimeControl
{
    public partial class MainWindow : Window
    {
        private const string TIME_FORMAT = "hh\\:mm\\:ss";
        
        private ITimer timer;
        private IState state;

        public MainWindow()
        {
            InitializeComponent();
            state = new State();
            if (state.IsActive == false)
            {
                Close();
            }
            timer = new Timer(state.Load());
            timer.Tick += TimerTickHandler;
        }

        private void TimerTickHandler(object sender, TimerEventArgs e)
        {
            DisplayCurrentTime();
        }

        private void LoadedMainWindow(object sender, RoutedEventArgs e)
        {
            DisplayCurrentTime();
            DisplayBottonContent();
        }

        private void DisplayCurrentTime()
        {
            Dispatcher.Invoke(() =>
            {
                TimeSpan elapsedTime = timer.GetElapsedTime();
                display.Content = elapsedTime.ToString(TIME_FORMAT); 
            });
        }

        private void DisplayBottonContent()
        {
            if (timer.IsRunning)
            {
                startStopButton.Content = "Pause";
            }
            else if (timer.GetElapsedTime().Ticks == 0)
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
            if (timer.IsRunning)
            {
                timer.Stop();
            }
            else
            {
                timer.Start();
            }
            DisplayBottonContent();
        }

        private void ClickOnResetButton(object sender, RoutedEventArgs e)
        {
            timer.Reset();
            DisplayCurrentTime();
            DisplayBottonContent();
        }

        private void ClosingMainWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            state.Save(timer.GetElapsedTime());
        }
    }
}
