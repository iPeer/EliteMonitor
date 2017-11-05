using EliteMonitor.Extensions;
using EliteMonitor.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EliteMonitor.Notifications
{
    public class Notification
    {

        public enum NotificationState
        {
            OPENING,
            OPENED,
            CLOSING,
            CLOSED
        }

        public EventHandler<int> OnNotificationDisplayed;
        public EventHandler<int> OnNotificationClosed;

        public string Title { get; set; } = "Notification";
        public string Text { get; set; } = "Notification Text";
        public int DisplayTime { get; set; } = 10; // Seconds
        public long DisplayMs
        {
            get
            {
                return this.DisplayTime * 1000L;
            }
        }
        public bool DisplayTitle { get; set; } = true;
        public Image Background { get; set; } = null;

        public bool isDisplayed { get; set; }
        private NotificationState _state = NotificationState.OPENING;
        public NotificationState State
        {
            get
            {
                return this._state;
            }
            set
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("NOTIFICATION STATE: " + value.ToString());
#endif
                this._state = value;
            }
        }
        public int PopupHeight
        {
            get
            {
                float dHeight = this.DisplayTitle ? 32f : 19f;
                Graphics graphics = MainForm.Instance.CreateGraphics();
                SizeF textBounds = graphics.MeasureString(this.Text, new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0))));
                graphics.Dispose();
                int height = (int)Math.Ceiling(textBounds.Height);
                return (int)Math.Ceiling(dHeight + height);
            }
        }
        public NotificationPopup PopupInstance { get; set; }

        public Notification(string NotificationTitle, string NotificationText, int DisplaySeconds = 10, bool DisplayTitle = true, Image NotificationBackground = null)
        {
            this.Title = NotificationTitle;
            this.Text = NotificationText;
            this.DisplayTime = DisplaySeconds;
            this.DisplayTitle = DisplayTitle;
            this.Background = NotificationBackground;
        }
    }
}
