using EliteMonitor.Extensions;
using EliteMonitor.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static EliteMonitor.Notifications.Notification;

namespace EliteMonitor.Notifications
{
    public partial class NotificationPopup : Form
    {

        private Notification owningNotification;
        private int totalHeight;
        private int newHeight = 0;
        private Stopwatch timingWatch = new Stopwatch();

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        public NotificationPopup(Notification n, int totalHeight)
        {
            InitializeComponent();
            if (Properties.Settings.Default.darkModeEnabled)
                Utils.toggleNightModeForForm(this);
            this.owningNotification = n;
            this.owningNotification.PopupInstance = this;
            this.totalHeight = totalHeight;

            if (totalHeight < 10)
                totalHeight = 10;
            this.owningNotification.isDisplayed = true;
            this.owningNotification.State = Notification.NotificationState.OPENING;

            Rectangle screen = Utils.getActiveScreenResolution();
            int sWidth = screen.Width;
            int sHeight = screen.Height;

            Graphics graphics = this.CreateGraphics();

            //float dHeight = 32f;
            if (!this.owningNotification.DisplayTitle)
            {
                //dHeight = 19f;
                this.Height -= 13;
                this.labelText.Location = new Point(this.labelText.Location.X, this.labelText.Location.Y - 13);
            }

            // Get title bounds
            SizeF titleBounds = graphics.MeasureString(this.owningNotification.Title, this.labelTitle.Font);

            // Get text bounds
            SizeF textBounds = graphics.MeasureString(this.owningNotification.Text, this.labelText.Font);

            this.labelText.Text = this.owningNotification.Text;
            this.labelTitle.Text = this.owningNotification.Title;

            // We don't need this any more
            //graphics.Dispose();

            float windowWidth = (titleBounds.Width > textBounds.Width && this.owningNotification.DisplayTitle ? titleBounds.Width : textBounds.Width) + this.labelText.Location.X;
            this.Width = (int)Math.Ceiling(windowWidth);

            this.Height = this.owningNotification.PopupHeight;
            this.labelText.Height = (int)Math.Ceiling(textBounds.Height);

            this.Location = new Point(sWidth - (this.Width + 10), totalHeight);
            this.BringToFront();
        }

        private void NotificationPopup_Load(object sender, EventArgs e)
        {
            this.Opacity = 0d;
            this.timer1.Tick += timer1_Tick;
            this.timer1.Interval = 20;
            this.timer1.Enabled = true;
            this.timer1.Start();
            this.owningNotification.OnNotificationDisplayed?.Invoke(this.owningNotification, this.owningNotification.PopupHeight);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.owningNotification.State == NotificationState.OPENING)
            {
                if (this.Opacity < 1.0d)
                {
                    this.Opacity += 0.025;
                }
                else
                {
                    this.owningNotification.State = NotificationState.OPENED;
                    this.timingWatch.Start();
                }
            }
            else if (this.owningNotification.State == NotificationState.OPENED)
            {
                //this.timeOpen += this.timer1.Interval;
                //if (this.timeOpen >= this.owningNotification.DisplayMs)
                if (this.timingWatch.ElapsedMilliseconds >= this.owningNotification.DisplayMs)
                {
                    this.timingWatch.Stop();
                    this.owningNotification.State = NotificationState.CLOSING;
                }
            }
            else
            {
                if (this.Opacity > 0.0)
                {
                    this.Opacity -= 0.025;
                }
                else
                {
                    this.owningNotification.OnNotificationClosed?.Invoke(this.owningNotification, this.owningNotification.PopupHeight);
                    this.owningNotification.State = NotificationState.CLOSED;
                    timer1.Stop();
                    this.Close();
                }
            }
        }

        public void MoveUpIfRequired(int heightChange)
        {
            if (this.Location.Y > (heightChange + 10))
            {
                this.newHeight = this.Location.Y - (heightChange/* + NotificationManager.NOTIFICATION_SPACING*/);
                this.timer2.Interval = 20;
                this.timer2.Tick += timer2_Tick;
                this.timer2.Start();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (this.Location.Y > this.newHeight)
            {
                int increment = (int)Math.Ceiling((double)this.newHeight / 500d);
                this.Location = new Point(this.Location.X, this.Location.Y - increment);
            }
            else
            {
                this.Location = new Point(this.Location.X, this.newHeight);
                this.timer2.Stop();
            }

        }
    }
}
