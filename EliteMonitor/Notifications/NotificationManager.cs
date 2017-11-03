using EliteMonitor.Extensions;
using EliteMonitor.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EliteMonitor.Notifications
{
    public class NotificationManager
    {

        public const int NOTIFICATION_SPACING = 10;

        public Logger logger = new Logger("NotificationManager");
        public bool CurrentlyShowingNotifications { get; private set; } = false;
        List<Notification> notificationQueue = new List<Notification>();
        public int MaxNotificationsAtOnce = 5;
        public int CurrentNotificationCount { get; private set; } = 0;
        private int TotalNotificationHeight = 10;

        public void AddNotificationToQueue(string title, string text) => AddNotificationToQueue(new Notification(title, text));
        public void AddNotificationToQueue(string title, string text, int delay) => AddNotificationToQueue(new Notification(title, text, delay));
        public void AddNotificationToQueue(string title, string text, int delay, bool showTitle) => AddNotificationToQueue(new Notification(title, text, delay, showTitle));
        public void AddNotificationToQueue(string title, string text, int delay, bool showTitle, Image background) => AddNotificationToQueue(new Notification(title, text, delay, showTitle, background));
        public void AddNotificationToQueue(Notification notification, bool showNow = true)
        {
            notification.OnNotificationClosed += OnNotificationClosed;
            notification.OnNotificationDisplayed += OnNotificationDisplayed;
            this.notificationQueue.Add(notification);
            if (showNow)
                this.showNotificationQueue();
        }

        public void showNotificationQueue()
        {
            List<Notification> immutableNotificationQueue = this.notificationQueue.FindAll(a => !a.isDisplayed).ToList();
            foreach (Notification n in immutableNotificationQueue)
            {
                if (this.CurrentNotificationCount <= this.MaxNotificationsAtOnce)
                {
                    //this.notificationQueue.Remove(n);
                    //n.Display(this.TotalNotificationHeight);
                    /*n.OnNotificationDisplayed += OnNotificationDisplayed;
                    n.OnNotificationClosed += OnNotificationClosed;*/
                    NotificationPopup np = new NotificationPopup(n, TotalNotificationHeight);
                    np.InvokeIfRequired(() => np.Show());
                    this.TotalNotificationHeight += (n.PopupHeight + NOTIFICATION_SPACING);
                    this.CurrentNotificationCount++;
                }
                else { break; }
            }
        }

        private void OnNotificationDisplayed(object sender, int height)
        {
            //this.TotalNotificationHeight += (height + NOTIFICATION_SPACING);
            this.logger.Log("Notification '{0}' displayed.", ((Notification)sender).ToString());
        }
        private void OnNotificationClosed(object sender, int height)
        {
/*#if DEBUG
            System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();
            System.Diagnostics.Debug.WriteLine(st.ToString());
#endif*/
            this.TotalNotificationHeight -= (((Notification)sender).PopupHeight + NOTIFICATION_SPACING);
            if (this.TotalNotificationHeight < 10)
                this.TotalNotificationHeight = 10;
            this.CurrentNotificationCount--;
            if (this.CurrentNotificationCount > 0) {
                foreach (Notification n in this.notificationQueue.FindAll(a => a.isDisplayed))
                {
                    n.PopupInstance.MoveUpIfRequired(((Notification)sender).PopupHeight);
                }
            }
            this.notificationQueue.Remove((Notification)sender);
            if (this.notificationQueue.Count > 0)
                this.showNotificationQueue();
            this.logger.Log("Notification '{0}' closed.", ((Notification)sender).ToString());
        }
    }
}
