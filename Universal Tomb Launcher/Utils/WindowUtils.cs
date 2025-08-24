using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using UniversalTombLauncher.Native;

namespace UniversalTombLauncher.Utils
{
	/// <summary>
	/// Utility class for window-related operations.
	/// </summary>
	internal static class WindowUtils
	{
		// Source: https://stackoverflow.com/a/56514746

		/// <summary>
		/// Enables the specified accent effect on a window with the given accent color.
		/// </summary>
		public static void EnableAccent(IWin32Window window, ACCENT targetAccent, Color accentColor)
		{
			if (window == null)
				throw new ArgumentNullException(nameof(window));

			var accentPolicy = new AccentPolicy
			{
				AccentState = targetAccent,
				GradientColor = ToAbgr(accentColor)
			};

			unsafe
			{
				NativeMethods.SetWindowCompositionAttribute(
					new HandleRef(window, window.Handle),
					new WindowCompositionAttributeData
					{
						Attribute = WCA.ACCENT_POLICY,
						Data = &accentPolicy,
						DataLength = Marshal.SizeOf(typeof(AccentPolicy))
					});
			}
		}

		/// <summary>
		/// Converts a Color to a 32-bit ABGR value.
		/// </summary>
		private static uint ToAbgr(Color color)
		{
			return ((uint)color.A << 24)
				 | ((uint)color.B << 16)
				 | ((uint)color.G << 8)
				 | color.R;
		}
	}
}
