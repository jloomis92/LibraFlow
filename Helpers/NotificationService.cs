using System;
using System.Windows;
using MaterialDesignThemes.Wpf;

namespace LibraFlow.Helpers
{
    public static class NotificationService
    {
        public static event Action<string, string, NotificationType> NotificationRequested;

        public enum NotificationType
        {
            Info,
            Success,
            Warning,
            Error
        }

        public static void ShowNotification(string message, NotificationType type = NotificationType.Info, int durationSeconds = 4)
        {
            var title = type switch
            {
                NotificationType.Success => "Success",
                NotificationType.Warning => "Warning",
                NotificationType.Error => "Error",
                _ => "Info"
            };

            NotificationRequested?.Invoke(title, message, type);
        }

        public static void ShowConnectionRestored()
        {
            ShowNotification("Database connection restored successfully.", NotificationType.Success);
        }

        public static void ShowConnectionLost()
        {
            ShowNotification("Database connection lost. Some features may be unavailable.", NotificationType.Error);
        }

        public static void ShowConnectionError(string details)
        {
            ShowNotification($"Database connection error: {details}", NotificationType.Error);
        }
    }
}
