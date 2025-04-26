using System;
using System.Globalization; // For TryParseExact
using System.Threading;     // For Timer

namespace ConsoleAlarmClock
{
    // 1. Define the delegate for the event handler
    public delegate void AlarmEventHandler(object sender, EventArgs e);

    // 2. Create the Publisher class (Alarm Clock)
    public class AlarmClock
    {
        private Timer _timer;
        private TimeSpan _alarmTime;
        private bool _alarmRaised = false;

        // 3. Define the event using the delegate
        public event AlarmEventHandler RaiseAlarm;

        public void SetAlarm(TimeSpan targetTime)
        {
            _alarmTime = targetTime;
            _alarmRaised = false;
            Console.WriteLine($"Alarm set for: {_alarmTime:hh\\:mm\\:ss}");
        }

        public void Start()
        {
            if (_timer == null)
            {
                _timer = new Timer(CheckTime, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
                Console.WriteLine("Alarm clock started. Checking time...");
            }
        }

        private void CheckTime(object state)
        {
            if (_alarmRaised) return;

            TimeSpan currentTime = DateTime.Now.TimeOfDay;

            if (currentTime.Hours == _alarmTime.Hours &&
                currentTime.Minutes == _alarmTime.Minutes &&
                currentTime.Seconds == _alarmTime.Seconds)
            {
                OnRaiseAlarm();
                _alarmRaised = true;
                Stop();
            }
        }

        public void Stop()
        {
            _timer?.Dispose();
            _timer = null;
            Console.WriteLine("Alarm clock stopped.");
        }

        protected virtual void OnRaiseAlarm()
        {
            RaiseAlarm?.Invoke(this, EventArgs.Empty);
        }
    }

    // Main Program Class (acts as the Subscriber)
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--- Console Alarm Clock ---");
            TimeSpan userAlarmTime;

            while (true)
            {
                Console.Write("Enter alarm time in HH:MM:SS format: ");
                string inputTime = Console.ReadLine();
                if (TimeSpan.TryParseExact(inputTime, @"hh\:mm\:ss", CultureInfo.InvariantCulture, out userAlarmTime))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid format. Please use HH:MM:SS (e.g., 14:30:00).");
                }
            }

            AlarmClock clock = new AlarmClock();
            clock.RaiseAlarm += Ring_Alarm; // Subscribe
            clock.SetAlarm(userAlarmTime);
            clock.Start();

            Console.WriteLine("Press Enter to exit at any time.");
            Console.ReadLine();
            clock.Stop(); // Ensure stop if user exits manually
        }

        // Event Handler method (Subscriber's action)
        static void Ring_Alarm(object sender, EventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n*********************************");
            Console.WriteLine($"* RING! RING! RING! Alarm at {DateTime.Now:HH:mm:ss} *");
            Console.WriteLine("*********************************");
            Console.ResetColor();
        }
    }
}
