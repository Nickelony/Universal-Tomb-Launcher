using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace UniversalTombLauncher.Native
{
	[SupportedOSPlatform("windows")]
	internal static class NativeMethods
	{
		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr FindWindow(string? lpClassName, string? lpWindowName);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
	}
}
