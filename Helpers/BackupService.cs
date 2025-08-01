using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using LibraFlow.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraFlow.Helpers
{
    public static class BackupService
    {
        public static event Action<DateTime>? BackupCompleted;

        public static async Task<bool> CreateBackupAsync(string? customPath = null)
        {
            try
            {
                // Get the current database path
                var currentDbPath = GetDatabasePath();

                if (!File.Exists(currentDbPath))
                {
                    throw new FileNotFoundException("Database file not found.");
                }

                // Determine backup directory
                var backupDir = customPath ?? GetDefaultBackupDirectory();
                Directory.CreateDirectory(backupDir);

                // Create backup filename with timestamp
                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var backupFileName = $"library_backup_{timestamp}.db";
                var backupPath = Path.Combine(backupDir, backupFileName);

                // Copy the database file
                await Task.Run(() => File.Copy(currentDbPath, backupPath, true));

                // Update settings
                Properties.Settings.Default.LastBackupDate = DateTime.Now;
                if (string.IsNullOrEmpty(Properties.Settings.Default.BackupDirectory))
                {
                    Properties.Settings.Default.BackupDirectory = backupDir;
                }
                Properties.Settings.Default.Save();

                // Clean up old backups (keep last 10)
                await CleanupOldBackupsAsync(backupDir);

                BackupCompleted?.Invoke(DateTime.Now);
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Backup failed: {ex.Message}", "Backup Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static async Task<bool> RestoreBackupAsync(string backupPath)
        {
            try
            {
                if (!File.Exists(backupPath))
                {
                    throw new FileNotFoundException("Backup file not found.");
                }

                var currentDbPath = GetDatabasePath();

                // Create a backup of current database before restoring
                var tempBackupPath = Path.Combine(Path.GetTempPath(),
                    $"temp_backup_{DateTime.Now:yyyyMMdd_HHmmss}.db");
                File.Copy(currentDbPath, tempBackupPath, true);

                try
                {
                    // Close any open database connections
                    using (var context = new LibraFlowContext())
                    {
                        await context.Database.CloseConnectionAsync();
                    }

                    // Restore the backup
                    await Task.Run(() => File.Copy(backupPath, currentDbPath, true));

                    // Verify the restored database
                    using (var context = new LibraFlowContext())
                    {
                        await context.Database.CanConnectAsync();
                    }

                    // Clean up temp backup
                    File.Delete(tempBackupPath);
                    return true;
                }
                catch
                {
                    // Restore the temp backup if something went wrong
                    File.Copy(tempBackupPath, currentDbPath, true);
                    File.Delete(tempBackupPath);
                    throw;
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Restore failed: {ex.Message}", "Restore Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static async Task CheckAutoBackupAsync()
        {
            if (!Properties.Settings.Default.AutoBackupEnabled)
                return;

            var lastBackup = Properties.Settings.Default.LastBackupDate;
            var daysSinceBackup = (DateTime.Now - lastBackup).Days;
            var autoBackupDays = Properties.Settings.Default.AutoBackupDays;

            if (daysSinceBackup >= autoBackupDays)
            {
                await CreateBackupAsync();
            }
        }

        public static string GetDatabasePath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "LibraFlow", "library.db");
        }

        public static string GetDefaultBackupDirectory()
        {
            var backupDir = Properties.Settings.Default.BackupDirectory;

            if (string.IsNullOrEmpty(backupDir))
            {
                backupDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "LibraFlow", "Backups");
            }

            return backupDir;
        }

        public static DateTime GetLastBackupDate()
        {
            var lastBackup = Properties.Settings.Default.LastBackupDate;
            return lastBackup == DateTime.MinValue ? DateTime.MinValue : lastBackup;
        }

        public static string GetLastBackupDisplayText()
        {
            var lastBackup = GetLastBackupDate();
            if (lastBackup == DateTime.MinValue)
                return "Never";

            var daysSince = (DateTime.Now - lastBackup).Days;

            return daysSince switch
            {
                0 => "Today",
                1 => "Yesterday",
                < 7 => $"{daysSince} days ago",
                < 30 => $"{daysSince / 7} weeks ago",
                _ => lastBackup.ToString("MM/dd/yyyy")
            };
        }

        private static async Task CleanupOldBackupsAsync(string backupDirectory)
        {
            try
            {
                await Task.Run(() =>
                {
                    var backupFiles = Directory.GetFiles(backupDirectory, "library_backup_*.db");
                    if (backupFiles.Length <= 10) return;

                    Array.Sort(backupFiles, (x, y) => File.GetCreationTime(y).CompareTo(File.GetCreationTime(x)));

                    for (int i = 10; i < backupFiles.Length; i++)
                    {
                        File.Delete(backupFiles[i]);
                    }
                });
            }
            catch
            {
                // Silently ignore cleanup errors
            }
        }
    }
}