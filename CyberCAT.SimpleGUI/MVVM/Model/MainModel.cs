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
            _notifyCompleted = new TaskCompletionSource<NotifyResult>();
            NotificationOpened?.Invoke(text, title, buttons);
            NotifyResult result = await _notifyCompleted.Task;

            return result;
        }

        public static void CloseNotification(NotifyResult result)
        {
            _notifyCompleted?.TrySetResult(result);
            NotificationClosed?.Invoke(result);
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
        YesNo
    }
}
