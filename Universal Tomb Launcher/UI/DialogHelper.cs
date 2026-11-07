using System;
using System.Runtime.InteropServices;
using SDL;
using UniversalTombLauncher.Utils;

namespace UniversalTombLauncher.UI
{
	/// <summary>
	/// Cross-platform message box and dialog helper using SDL3.
	/// </summary>
	internal static class DialogHelper
	{
		/// <summary>
		/// Shows an error message dialog.
		/// </summary>
		public static unsafe void ShowError(string message, string title = "Error")
		{
			EnsureSdlInit();
			SDL3.SDL_ShowSimpleMessageBox(
				SDL_MessageBoxFlags.SDL_MESSAGEBOX_ERROR,
				title,
				message,
				null);
		}

		/// <summary>
		/// Shows a warning message dialog.
		/// </summary>
		public static unsafe void ShowWarning(string message, string title = "Warning")
		{
			EnsureSdlInit();
			SDL3.SDL_ShowSimpleMessageBox(
				SDL_MessageBoxFlags.SDL_MESSAGEBOX_WARNING,
				title,
				message,
				null);
		}

		/// <summary>
		/// Shows a confirmation dialog with Yes and No buttons.
		/// </summary>
		/// <returns><see langword="true"/> if Yes was clicked; otherwise, <see langword="false"/>.</returns>
		public static unsafe bool ShowYesNo(string message, string title = "Warning")
		{
			EnsureSdlInit();

			IntPtr titlePtr = Marshal.StringToCoTaskMemUTF8(title);
			IntPtr msgPtr = Marshal.StringToCoTaskMemUTF8(message);
			IntPtr yesPtr = Marshal.StringToCoTaskMemUTF8("Yes");
			IntPtr noPtr = Marshal.StringToCoTaskMemUTF8("No");

			try
			{
				var buttons = stackalloc SDL_MessageBoxButtonData[2];
				buttons[0] = new SDL_MessageBoxButtonData
				{
					flags = SDL_MessageBoxButtonFlags.SDL_MESSAGEBOX_BUTTON_RETURNKEY_DEFAULT,
					buttonID = 1,
					text = (byte*)yesPtr
				};
				buttons[1] = new SDL_MessageBoxButtonData
				{
					flags = SDL_MessageBoxButtonFlags.SDL_MESSAGEBOX_BUTTON_ESCAPEKEY_DEFAULT,
					buttonID = 0,
					text = (byte*)noPtr
				};

				var data = new SDL_MessageBoxData
				{
					flags = SDL_MessageBoxFlags.SDL_MESSAGEBOX_WARNING,
					window = null,
					title = (byte*)titlePtr,
					message = (byte*)msgPtr,
					numbuttons = 2,
					buttons = buttons,
					colorScheme = null
				};

				int buttonId = 0;
				SDL3.SDL_ShowMessageBox(&data, &buttonId);
				return buttonId == 1;
			}
			finally
			{
				Marshal.FreeCoTaskMem(titlePtr);
				Marshal.FreeCoTaskMem(msgPtr);
				Marshal.FreeCoTaskMem(yesPtr);
				Marshal.FreeCoTaskMem(noPtr);
			}
		}

		/// <summary>
		/// Shows the setup dialog for TR4 fullscreen border fix on Windows 8+.
		/// </summary>
		/// <returns><see langword="true"/> if launching should proceed; <see langword="false"/> to cancel launching.</returns>
		public static unsafe bool ShowExtraSettingsSetup()
		{
			if (!OperatingSystem.IsWindows())
				return true;

			EnsureSdlInit();

			bool isCurrentlyInstalled = FullscreenBorderFix.IsBorderFixInstalled();
			string currentStatus = isCurrentlyInstalled ? "ENABLED" : "DISABLED";

			string title = "TR4 Setup - Fullscreen Border Fix";
			string message = "The Fullscreen Border Fix fixes an issue where window borders\n" +
			                 "remain visible in fullscreen mode on Windows 8 and newer.\n\n" +
			                 $"Current status: {currentStatus}\n\n" +
			                 "Would you like to enable, disable, or keep current settings?";

			IntPtr titlePtr = Marshal.StringToCoTaskMemUTF8(title);
			IntPtr msgPtr = Marshal.StringToCoTaskMemUTF8(message);
			IntPtr btnEnablePtr = Marshal.StringToCoTaskMemUTF8("Enable Fix");
			IntPtr btnDisablePtr = Marshal.StringToCoTaskMemUTF8("Disable Fix");
			IntPtr btnKeepPtr = Marshal.StringToCoTaskMemUTF8("Launch Game");
			IntPtr btnCancelPtr = Marshal.StringToCoTaskMemUTF8("Cancel");

			try
			{
				var buttons = stackalloc SDL_MessageBoxButtonData[4];
				buttons[0] = new SDL_MessageBoxButtonData
				{
					flags = SDL_MessageBoxButtonFlags.SDL_MESSAGEBOX_BUTTON_RETURNKEY_DEFAULT,
					buttonID = 1,
					text = (byte*)btnKeepPtr
				};
				buttons[1] = new SDL_MessageBoxButtonData
				{
					flags = 0,
					buttonID = 2,
					text = (byte*)btnEnablePtr
				};
				buttons[2] = new SDL_MessageBoxButtonData
				{
					flags = 0,
					buttonID = 3,
					text = (byte*)btnDisablePtr
				};
				buttons[3] = new SDL_MessageBoxButtonData
				{
					flags = SDL_MessageBoxButtonFlags.SDL_MESSAGEBOX_BUTTON_ESCAPEKEY_DEFAULT,
					buttonID = 0,
					text = (byte*)btnCancelPtr
				};

				var data = new SDL_MessageBoxData
				{
					flags = SDL_MessageBoxFlags.SDL_MESSAGEBOX_INFORMATION,
					window = null,
					title = (byte*)titlePtr,
					message = (byte*)msgPtr,
					numbuttons = 4,
					buttons = buttons,
					colorScheme = null
				};

				int buttonId = 0;
				SDL3.SDL_ShowMessageBox(&data, &buttonId);

				switch (buttonId)
				{
					case 2: // Enable
						if (!isCurrentlyInstalled)
							FullscreenBorderFix.InstallBorderFix();
						return true;

					case 3: // Disable
						if (isCurrentlyInstalled)
							FullscreenBorderFix.UninstallBorderFix();
						return true;

					case 1: // Launch Game (keep current)
						return true;

					default: // Cancel
						return false;
				}
			}
			finally
			{
				Marshal.FreeCoTaskMem(titlePtr);
				Marshal.FreeCoTaskMem(msgPtr);
				Marshal.FreeCoTaskMem(btnEnablePtr);
				Marshal.FreeCoTaskMem(btnDisablePtr);
				Marshal.FreeCoTaskMem(btnKeepPtr);
				Marshal.FreeCoTaskMem(btnCancelPtr);
			}
		}

		private static void EnsureSdlInit()
		{
			SDL3.SDL_Init(SDL_InitFlags.SDL_INIT_VIDEO);
		}
	}
}
