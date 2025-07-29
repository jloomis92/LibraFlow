using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MaterialDesignThemes.Wpf;

namespace LibraFlow.Helpers
{
    public static class ThemeManager
    {
        public static event Action? ThemeChanged;

        public static void SetBaseTheme(BaseTheme baseTheme)
        {
            BundledTheme FindBundledTheme(ResourceDictionary dict)
            {
                foreach (var md in dict.MergedDictionaries)
                {
                    if (md is BundledTheme bundledTheme)
                        return bundledTheme;
                    var found = FindBundledTheme(md);
                    if (found != null)
                        return found;
                }
                return null;
            }

            var bundledTheme = FindBundledTheme(System.Windows.Application.Current.Resources);
            if (bundledTheme != null)
            {
                bundledTheme.BaseTheme = baseTheme;
            }
        }

        public static void ApplyTheme(Window window)
        {
            // Set the base theme using MaterialDesign's BundledTheme
            var theme = GetCurrentTheme(); // e.g., "Dark" or "Light"
            var baseTheme = theme.Equals("Dark", StringComparison.OrdinalIgnoreCase)
                ? BaseTheme.Dark
                : BaseTheme.Light;

            SetBaseTheme(baseTheme);
        }

        public static void ChangeTheme(string newTheme)
        {
            // Save the new theme to user settings
            Properties.Settings.Default.AppTheme = newTheme;
            Properties.Settings.Default.Save();
            ThemeChanged?.Invoke();
        }

        public static string GetCurrentTheme()
        {
            // Return the theme from user settings
            return Properties.Settings.Default.AppTheme;
        }
    }
}
