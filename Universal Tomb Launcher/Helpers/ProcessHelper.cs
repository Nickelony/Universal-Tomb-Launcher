﻿using System;
using System.Diagnostics;
using System.Reflection;
using UniversalTombLauncher.Enums;
using UniversalTombLauncher.Native;

namespace UniversalTombLauncher.Helpers
{
	internal static class ProcessHelper
	{
		#region Constants

		/* Default TR1 - TR5 process names */

		public const string
			TR1ProcessName = "tombati",
			Tomb1MainProcessName = "Tomb1Main",
			TR1XProcessName = "TR1X",
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
			TR2WndClassName = "Dude:TombRaiderII:DDWndClass",
			TR3WndClassName = "Window Class", // Why...
			TR4WndClassName = "MainGameWindow",
			TR5WndClassName = TR4WndClassName,
			TombEngineWndClassName = "TombEngine";

		#endregion Constants

		public static bool IsGameAlreadyRunning(GameVersion version)
			=> FindGameProcess(version) != null;

		public static Process FindGameProcess(GameVersion version)
		{
			string gameProcessName = GetDefaultGameProcessName(version);
			string gameWindowClassName = GetDefaultGameWindowClassName(version);

			return TryGetProcessWithExactWindow(gameProcessName, gameWindowClassName);
		}

		public static string GetDefaultGameProcessName(GameVersion version)
			=> GetConstValue<string>(version.ToString() + "ProcessName");

		public static string GetDefaultGameWindowClassName(GameVersion version)
			=> GetConstValue<string>(version.ToString() + "WndClassName");

		private static T GetConstValue<T>(string constName)
		{
			FieldInfo constField = ReflectionHelper.GetConstant(typeof(ProcessHelper), constName);
			return (T)constField?.GetValue(null);
		}

		private static Process TryGetProcessWithExactWindow(string processName, string windowClassName)
		{
			IntPtr windowHandle = NativeMethods.FindWindow(windowClassName, default);
			Process process = TryGetProcessFromWindowHandle(windowHandle);

			if (process?.ProcessName.Equals(processName) == true)
				return process;

			windowHandle = NativeMethods.FindWindow(windowClassName, processName);
			process = TryGetProcessFromWindowHandle(windowHandle);

			if (process?.ProcessName.Equals(processName) == true)
				return process;

			return null;
		}

		private static Process TryGetProcessFromWindowHandle(IntPtr windowHandle)
		{
			if (windowHandle == IntPtr.Zero)
				return null;

			NativeMethods.GetWindowThreadProcessId(windowHandle, out uint processId);
			return Process.GetProcessById((int)processId);
		}
	}
}
