using log4net;
using MissionPlanner.Utilities;
using System;
using System.Reflection;

namespace MissionPlanner.Joystick
{
    /// <summary>
    /// Entry point for opening joystick settings with an optional Avalonia host.
    /// Falls back to the legacy WinForms control when Avalonia host is unavailable.
    /// </summary>
    public static class JoystickSetupRenderer
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(JoystickSetupRenderer));

        // Optional external host. If this type exists, we treat it as the new renderer.
        private const string AvaloniaHostTypeName = "MissionPlanner.AvaloniaHost.JoystickSettingsWindow, MissionPlanner.AvaloniaHost";

        public static void ShowWindow()
        {
            if (TryShowAvaloniaWindow())
                return;

            new JoystickSetup().ShowUserControl();
        }

        private static bool TryShowAvaloniaWindow()
        {
            try
            {
                var hostType = Type.GetType(AvaloniaHostTypeName, throwOnError: false);
                if (hostType == null)
                    return false;

                // Supported signatures in host:
                //   public static bool Show()
                //   public static void Show()
                //   public static bool Show(string joystickName)
                //   public static void Show(string joystickName)
                var selectedJoystickName = Settings.Instance["joystick_name"]?.ToString() ?? string.Empty;

                var showWithName = hostType.GetMethod("Show", BindingFlags.Public | BindingFlags.Static, null,
                    new[] { typeof(string) }, null);

                if (InvokeHost(showWithName, selectedJoystickName))
                    return true;

                var showNoArgs = hostType.GetMethod("Show", BindingFlags.Public | BindingFlags.Static, null,
                    Type.EmptyTypes, null);

                if (InvokeHost(showNoArgs))
                    return true;
            }
            catch (Exception ex)
            {
                Log.Warn("Avalonia joystick host failed. Falling back to WinForms UI.", ex);
                return false;
            }

            return false;
        }

        private static bool InvokeHost(MethodInfo method, params object[] args)
        {
            if (method == null)
                return false;

            var result = method.Invoke(null, args);

            if (method.ReturnType == typeof(void))
                return true;

            if (method.ReturnType == typeof(bool) && result is bool ok)
                return ok;

            return result != null;
        }
    }
}
