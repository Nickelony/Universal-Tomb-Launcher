using System;
using System.Diagnostics;
using System.Runtime.Versioning;
using UniversalTombLauncher.Native;

namespace UniversalTombLauncher.Platform.Windows
{
	[SupportedOSPlatform("windows")]
	internal static class WindowsProcessHelper
	{
		public static Process? FindProcessWithExactWindow(string processName, string windowClassName)
		{
			IntPtr windowHandle = NativeMethods.FindWindow(windowClassName, null);
			Process? process = GetProcessFromWindowHandle(windowHandle);

			if (process?.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase) == true)
				return process;

			windowHandle = NativeMethods.FindWindow(windowClassName, processName);
			process = GetProcessFromWindowHandle(windowHandle);

			if (process?.ProcessName.Equals(processName, StringComparison.OrdinalIgnoreCase) == true)
				return process;

			return null;
		}

		private static Process? GetProcessFromWindowHandle(IntPtr windowHandle)
		{
			if (windowHandle == IntPtr.Zero)
				return null;

			NativeMethods.GetWindowThreadProcessId(windowHandle, out uint processId);
			try
			{
				return Process.GetProcessById((int)processId);
			}
			catch
			{
				return null;
			}
		}
	}
}
