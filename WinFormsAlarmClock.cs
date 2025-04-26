 using System;
using System.Drawing;
using System.Globalization; // For TryParseExact
using System.Windows.Forms; // Essential for WinForms

namespace WinFormsAlarmClock
{
    public partial class Form1 : Form
    {
        private TimeSpan _targetAlarmTime;
        private bool _isAlarmSet = false;
        private Random _randomColorGenerator = new Random();

        public Form1()
        {
            InitializeComponent();
            // Ensure timer is disabled initially if not set in designer
            uiTimer.Enabled = false;
            uiTimer.Interval = 1000; // Set interval programmatically or in designer
        }

        private void btnStartAlarm_Click(object sender, EventArgs e)
        {
            string timeInput = txtAlarmTime.Text;

            if (TimeSpan.TryParseExact(timeInput, @"hh\:mm\:ss", CultureInfo.InvariantCulture, out _targetAlarmTime))
            {
                _isAlarmSet = true;
                txtAlarmTime.Enabled = false;
                btnStartAlarm.Enabled = false;
                this.Text = $"Alarm set for {_targetAlarmTime:hh\\:mm\\:ss}";
                uiTimer.Start(); // Enable the timer
            }
            else
            {
                MessageBox.Show("Invalid time format. Please use HH:MM:SS.",
                                "Invalid Input",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                txtAlarmTime.Focus();
                txtAlarmTime.SelectAll();
            }
        }

        private void uiTimer_Tick(object sender, EventArgs e)
        {
            if (!_isAlarmSet) return; // Should not happen if logic is correct, but safe check

            // Change Background Color
            Color randomColor = Color.FromArgb(
                _randomColorGenerator.Next(200, 256),
                _randomColorGenerator.Next(200, 256),
                _randomColorGenerator.Next(200, 256)
            );
            this.BackColor = randomColor;

            // Check for Alarm Time
            TimeSpan currentTime = DateTime.Now.TimeOfDay;

            if (currentTime.Hours == _targetAlarmTime.Hours &&
                currentTime.Minutes == _targetAlarmTime.Minutes &&
                currentTime.Seconds == _targetAlarmTime.Seconds)
            {
                // Stop timer and reset state
                uiTimer.Stop(); // Disable the timer
                _isAlarmSet = false;

                // Reset UI appearance (optional)
                this.BackColor = SystemColors.Control;
                this.Text = "Alarm Clock"; // Reset title

                // Show Alarm Message
                MessageBox.Show($"Ring! Ring! Ring!\nAlarm time {_targetAlarmTime:hh\\:mm\\:ss} reached!",
                                "Alarm!",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);

                // Re-enable controls
                txtAlarmTime.Enabled = true;
                btnStartAlarm.Enabled = true;
            }
        }

        // The designer code (InitializeComponent call etc.) is in Form1.Designer.cs
        // If you don't have a Form1.Designer.cs, ensure InitializeComponent() is called in the constructor.
    }
}
