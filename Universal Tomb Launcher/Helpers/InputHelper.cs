using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using UniversalTombLauncher.Native;

namespace UniversalTombLauncher.Helpers
{
	/// <summary>
	/// Helper class for simulating user input.
	/// </summary>
	internal static class InputHelper
	{
		private const int InputType_Mouse = 0;
		private const int MouseLeft_Down = 0x0002;
		private const int MouseLeft_Up = 0x0004;

		/// <summary>
		/// Simulates a mouse click on a specific point within a window's client area.
		/// </summary>
		/// <param name="hWnd">The handle of the window.</param>
		/// <param name="clientPoint">The point within the window's client area where the click should occur.</param>
		public static void ClickOnPoint(IntPtr hWnd, Point clientPoint)
		{
			Point cachedCursorPos = Cursor.Position;

			NativeMethods.ClientToScreen(hWnd, ref clientPoint);
			Cursor.Position = new Point(clientPoint.X, clientPoint.Y);

			var inputMouseDown = new INPUT { Type = InputType_Mouse };
			inputMouseDown.Data.Mouse.Flags = MouseLeft_Down;

			var inputMouseUp = new INPUT { Type = InputType_Mouse };
			inputMouseUp.Data.Mouse.Flags = MouseLeft_Up;

			var inputs = new INPUT[] { inputMouseDown, inputMouseUp };
			NativeMethods.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));

			Cursor.Position = cachedCursorPos;
		}
	}
}
