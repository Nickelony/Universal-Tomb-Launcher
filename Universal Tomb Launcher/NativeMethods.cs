using System;
using System.Runtime.InteropServices;

namespace UniversalTombLauncher
{
	public static class NativeMethods
	{
		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
	}
}
