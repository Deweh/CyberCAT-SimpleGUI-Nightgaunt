using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CyberCAT.SimpleGUI.Core.Helpers;

namespace CyberCAT.SimpleGUI.MVVM.Model
{
    public static class MainModel
    {
        private static string _status = "No save file selected.";
        private static TaskCompletionSource<NotifyResult> _notifyCompleted;
        public static bool _notifyOpen = false;
        private static Queue<TaskCompletionSource> _notifyQueue = new Queue<TaskCompletionSource>();

        public delegate void StatusChangedHandler(string status);
        public static event StatusChangedHandler StatusChanged;

        public delegate void NotificationOpenedHandler(string text, string title = "Notice", NotifyButtons buttons = NotifyButtons.OK);
        public static event NotificationOpenedHandler NotificationOpened;

        public delegate void NotificationClosedHandler(NotifyResult result);
        public static event NotificationClosedHandler NotificationClosed;

        public static string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                StatusChanged?.Invoke(_status);
            }
        }

        public static async Task<NotifyResult> OpenNotification(string text, string title = "Notice", NotifyButtons buttons = NotifyButtons.OK)
        {
            if (_notifyOpen)
            {
                var queuePosition = new TaskCompletionSource();
                _notifyQueue.Enqueue(queuePosition);
                await queuePosition.Task;
            }

            _notifyCompleted = new TaskCompletionSource<NotifyResult>();
            _notifyOpen = true;
            NotificationOpened?.Invoke(text, title, buttons);
            NotifyResult result = await _notifyCompleted.Task;

            return result;
        }

        public static void CloseNotification(NotifyResult result)
        {
            _notifyCompleted?.TrySetResult(result);
            NotificationClosed?.Invoke(result);
            _notifyOpen = false;

            if (_notifyQueue.Count > 0)
            {
                _notifyQueue.Dequeue()?.TrySetResult();
            }
        }
    }

    public enum NotifyResult
    {
        OK,
        Yes,
        No
    }

    public enum NotifyButtons
    {
        OK,
        YesNo,
        TaskNone
    }
}
