using System;
using System.Diagnostics;
using System.Reflection;
using UniversalTombLauncher.Enums;

namespace UniversalTombLauncher.Helpers
{
	public static class ProcessHelper
	{
		#region Constants

		/* Default TR1 - TR5 process names */

		public const string
			TR1ProcessName = "tombati",
			TR1MainProcessName = "Tomb1Main",
			TR2ProcessName = "Tomb2",
			TR3ProcessName = "tomb3",
			TR4ProcessName = "tomb4",
			TR5ProcessName = "PCTomb5";

		/* Window class names */

		public const string
			TR1WndClassName = "TRClass",
			TR1MainWndClassName = "SDL_app",
			TR2WndClassName = "Dude:TombRaiderII:DDWndClass",
			TR3WndClassName = "Window Class", // Why...
			TR4WndClassName = "MainGameWindow",
			TR5WndClassName = TR4WndClassName;

		#endregion Constants

		public static bool IsGameAlreadyRunning(GameVersion version) =>
			FindGameProcess(version) != null;

		public static Process FindGameProcess(GameVersion version)
		{
			string gameProcessName = GetDefaultGameProcessName(version);
			string gameWindowClassName = GetDefaultGameWindowClassName(version);

			return GetProcessWithExactWindow(gameProcessName, gameWindowClassName);
		}

		public static string GetDefaultGameProcessName(GameVersion version) =>
			GetConstValue<string>(version.ToString() + "ProcessName");

		public static string GetDefaultGameWindowClassName(GameVersion version) =>
			GetConstValue<string>(version.ToString() + "WndClassName");

		private static T GetConstValue<T>(string constName)
		{
			FieldInfo constField = ReflectionHelper.GetConstant(typeof(ProcessHelper), constName);
			return (T)constField?.GetValue(null);
		}

		private static Process GetProcessWithExactWindow(string processName, string windowClassName)
		{
			IntPtr windowHandle = NativeMethods.FindWindow(windowClassName, default);

			if (windowHandle != null)
			{
				NativeMethods.GetWindowThreadProcessId(windowHandle, out uint processId);

				var process = Process.GetProcessById((int)processId);

				if (process.ProcessName.Equals(processName))
					return process;
			}

			return null;
		}
	}
}
