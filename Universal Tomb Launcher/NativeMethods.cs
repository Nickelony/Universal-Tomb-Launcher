using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

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

		public static void ClickOnPoint(IntPtr hWnd, Point clientPoint)
		{
			var cachedCursorPos = Cursor.Position;

			ClientToScreen(hWnd, ref clientPoint);
			Cursor.Position = new Point(clientPoint.X, clientPoint.Y);

			const int mouseInputType = 0;
			const int mouseLeftDown = 0x0002;
			const int mouseLeftUp = 0x0004;

			var inputMouseDown = new INPUT { Type = mouseInputType };
			inputMouseDown.Data.Mouse.Flags = mouseLeftDown;

			var inputMouseUp = new INPUT { Type = mouseInputType };
			inputMouseUp.Data.Mouse.Flags = mouseLeftUp;

			var inputs = new INPUT[] { inputMouseDown, inputMouseUp };
			SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));

			Cursor.Position = cachedCursorPos;
		}
	}
}
