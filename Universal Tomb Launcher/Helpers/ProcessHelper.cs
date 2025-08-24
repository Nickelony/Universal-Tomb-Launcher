using System;
using System.Diagnostics;
using System.Reflection;
using UniversalTombLauncher.Enums;
using UniversalTombLauncher.Native;

namespace UniversalTombLauncher.Helpers
{
	/// <summary>
	/// Helper methods for working with processes.
	/// </summary>
	internal static class ProcessHelper
	{
		#region Constants

		/* Default TR1 - TR5 process names */

		public const string
			TR1ProcessName = "tombati",
			Tomb1MainProcessName = "Tomb1Main",
			TR1XProcessName = "TR1X",
			TR2XProcessName = "TR2X",
			TR2ProcessName = "Tomb2",
			TR3ProcessName = "tomb3",
			TR4ProcessName = "tomb4",
			TR5ProcessName = "PCTomb5",
			TombEngineProcessName = "TombEngine";

		/* Window class names */

		public const string
			TR1WndClassName = "TRClass",
			Tomb1MainWndClassName = "SDL_app",
			TR1XWndClassName = Tomb1MainWndClassName,
			TR2XWndClassName = Tomb1MainWndClassName,
			TR2WndClassName = "Dude:TombRaiderII:DDWndClass",
			TR3WndClassName = "Window Class", // Why...
			TR4WndClassName = "MainGameWindow",
			TR5WndClassName = TR4WndClassName,
			TombEngineWndClassName = "TombEngine";

		#endregion Constants

		/// <summary>
		/// Checks if a game of the specified version is already running.
		/// </summary>
		public static bool IsGameAlreadyRunning(GameVersion version)
			=> FindGameProcess(version) != null;

		/// <summary>
		/// Finds the process of a running game of the specified version.
		/// </summary>
		public static Process FindGameProcess(GameVersion version)
		{
			string gameProcessName = GetDefaultGameProcessName(version);
			string gameWindowClassName = GetDefaultGameWindowClassName(version);

			return FindProcessWithExactWindow(gameProcessName, gameWindowClassName);
		}

		/// <summary>
		/// Gets the default process name for the specified game version.
		/// </summary>
		public static string GetDefaultGameProcessName(GameVersion version)
			=> GetConstValue<string>(version.ToString() + "ProcessName");

		/// <summary>
		/// Gets the default window class name for the specified game version.
		/// </summary>
		public static string GetDefaultGameWindowClassName(GameVersion version)
			=> GetConstValue<string>(version.ToString() + "WndClassName");

		/// <summary>
		/// Gets the value of a constant field from the <see cref="ProcessHelper" /> class using reflection.
		/// </summary>
		private static T GetConstValue<T>(string constName)
		{
			FieldInfo constField = ReflectionHelper.GetConstant(typeof(ProcessHelper), constName);
			return (T)constField?.GetValue(null);
		}

		/// <summary>
		/// Finds a process by its name and window class name, ensuring an exact match.
		/// </summary>
		private static Process FindProcessWithExactWindow(string processName, string windowClassName)
		{
			IntPtr windowHandle = NativeMethods.FindWindow(windowClassName, default);
			Process process = GetProcessFromWindowHandle(windowHandle);

			if (process?.ProcessName.Equals(processName) == true)
				return process;

			windowHandle = NativeMethods.FindWindow(windowClassName, processName);
			process = GetProcessFromWindowHandle(windowHandle);

			if (process?.ProcessName.Equals(processName) == true)
				return process;

			return null;
		}

		/// <summary>
		/// Gets the <see cref="Process" /> instance associated with the specified window handle.
		/// </summary>
		private static Process GetProcessFromWindowHandle(IntPtr windowHandle)
		{
			if (windowHandle == IntPtr.Zero)
				return null;

			NativeMethods.GetWindowThreadProcessId(windowHandle, out uint processId);
			return Process.GetProcessById((int)processId);
		}
	}
}
