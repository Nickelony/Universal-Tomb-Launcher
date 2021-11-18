using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace UniversalTombLauncher
{
	internal static class NativeMethods
	{
		[DllImport("user32.dll", SetLastError = true)]
		public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

		[DllImport("user32.dll", SetLastError = true)]
		public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

		[DllImport("user32.dll")]
		public static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

		[DllImport("user32.dll")]
		public static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

		[DllImport("user32.dll")]
		public static extern int SetWindowCompositionAttribute(HandleRef hWnd, in WindowCompositionAttributeData data);
	}
}
