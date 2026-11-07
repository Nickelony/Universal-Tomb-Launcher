using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Versioning;
using UniversalTombLauncher.Native;

namespace UniversalTombLauncher.Helpers
{
	/// <summary>
	/// Helper methods for dealing with Windows Shell features like shortcuts.
	/// </summary>
	[SupportedOSPlatform("windows")]
	internal static class ShellHelper
	{
		/// <summary>
		/// Creates a shortcut for the specified executable with given icon and arguments.
		/// </summary>
		public static IPersistFile CreateShortcutWithIcon(string exeFilePath, string iconLocation, string args)
		{
			var link = (IShellLink)new ShellLink();

			link.SetPath(exeFilePath);
			link.SetWorkingDirectory(Path.GetDirectoryName(exeFilePath) ?? string.Empty);
			link.SetIconLocation(iconLocation, 0);
			link.SetArguments(args);

			return (IPersistFile)link;
		}

		/// <summary>
		/// Saves the provided shortcut to a temporary location and returns the path to the saved file.
		/// </summary>
		public static string SaveShortcut(IPersistFile shortcut, string exeFilePath)
		{
			string exeFileName = Path.GetFileNameWithoutExtension(exeFilePath);
			string shortcutPath = Path.Combine(Path.GetTempPath(), exeFileName + ".lnk");

			shortcut.Save(shortcutPath, false);
			return shortcutPath;
		}
	}
}
