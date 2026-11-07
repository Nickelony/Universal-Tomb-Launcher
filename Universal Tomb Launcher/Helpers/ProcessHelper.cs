using System;
using System.Diagnostics;
using System.Reflection;
using UniversalTombLauncher.Enums;
using UniversalTombLauncher.Platform.Windows;

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
			TRXProcessName = "TRX",
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
			TRXWndClassName = Tomb1MainWndClassName,
			TR2WndClassName = "Dude:TombRaiderII:DDWndClass",
			TR3WndClassName = "Window Class",
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
		public static Process? FindGameProcess(GameVersion version)
		{
			string? gameProcessName = GetDefaultGameProcessName(version);
			if (string.IsNullOrEmpty(gameProcessName))
				return null;

			// Cross-platform check by process name
			Process[] processes = Process.GetProcessesByName(gameProcessName);
			if (processes.Length == 0 && !OperatingSystem.IsWindows())
			{
				processes = Process.GetProcessesByName(gameProcessName.ToLowerInvariant());
			}

			foreach (Process process in processes)
			{
				try
				{
					if (!process.HasExited)
						return process;
				}
				catch
				{
					// Process might have exited or permission denied
				}
			}

			// On Windows, try FindWindow fallback if class name is known
			if (OperatingSystem.IsWindows())
			{
				string? gameWindowClassName = GetDefaultGameWindowClassName(version);
				if (!string.IsNullOrEmpty(gameWindowClassName))
				{
					Process? winProc = WindowsProcessHelper.FindProcessWithExactWindow(gameProcessName, gameWindowClassName);
					if (winProc != null)
						return winProc;
				}
			}

			return null;
		}

		/// <summary>
		/// Gets the default process name for the specified game version.
		/// </summary>
		public static string? GetDefaultGameProcessName(GameVersion version)
			=> GetConstValue<string>(version.ToString() + "ProcessName");

		/// <summary>
		/// Gets the default window class name for the specified game version.
		/// </summary>
		public static string? GetDefaultGameWindowClassName(GameVersion version)
			=> GetConstValue<string>(version.ToString() + "WndClassName");

		/// <summary>
		/// Gets the value of a constant field from the <see cref="ProcessHelper"/> class using reflection.
		/// </summary>
		private static T? GetConstValue<T>(string constName)
		{
			FieldInfo? constField = ReflectionHelper.GetConstant(typeof(ProcessHelper), constName);
			return constField != null ? (T?)constField.GetValue(null) : default;
		}
	}
}
