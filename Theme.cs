using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
#if TRUE
using System.Management;

using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Aurora.Misc
{
    public class Theme
    {

        public delegate void WindowsThemeChangedHandler(WindowsTheme newTheme);
        public event WindowsThemeChangedHandler WindowsThemeChanged;

        private void OnWindowsThemeChanged(WindowsTheme newTheme)
        {
            WindowsThemeChanged?.Invoke(newTheme);
        }
        
        private const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";

        private const string RegistryValueName = "AppsUseLightTheme";

        public enum WindowsTheme
        {
            Light,
            Dark
        }
        public WindowsTheme CurrentWindowsTheme { get; set; }

        public WindowsTheme WatchTheme()
        {

            var currentUser = WindowsIdentity.GetCurrent();
            string query = string.Format(
                CultureInfo.InvariantCulture,
                @"SELECT * FROM RegistryValueChangeEvent WHERE Hive = 'HKEY_USERS' AND KeyPath = '{0}\\{1}' AND ValueName = '{2}'",
                currentUser.User.Value,
                RegistryKeyPath.Replace(@"\", @"\\"),
                RegistryValueName);

            try
            {

                var watcher = new ManagementEventWatcher(query);
                watcher.EventArrived += (sender, args) =>
                                        {
                                            CurrentWindowsTheme = GetWindowsTheme();
                                            OnWindowsThemeChanged(CurrentWindowsTheme);
                                            // React to new theme
                                        };

                // Start listening for events
                watcher.Start();
            }
            catch (Exception)
            {
                throw;
            }

            CurrentWindowsTheme = GetWindowsTheme();
            OnWindowsThemeChanged(CurrentWindowsTheme);
            return (CurrentWindowsTheme);
        }

        private static WindowsTheme GetWindowsTheme()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath))
            {
                object registryValueObject = key?.GetValue(RegistryValueName);
                if (registryValueObject == null)
                {
                    return WindowsTheme.Light;
                }

                int registryValue = (int)registryValueObject;

                return registryValue > 0 ? WindowsTheme.Light : WindowsTheme.Dark;
            }
        }
    }
}
#endif
