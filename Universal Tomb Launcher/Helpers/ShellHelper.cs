using System.IO;
using System.Runtime.InteropServices.ComTypes;
using UniversalTombLauncher.Native;

namespace UniversalTombLauncher.Helpers
{
	/// <summary>
	/// Helper methods for dealing with Windows Shell features like shortcuts.
	/// </summary>
	internal static class ShellHelper
	{
		/// <summary>
		/// Creates a shortcut for the specified executable with given icon and arguments.
		/// </summary>
		/// <param name="exeFilePath">The file path to the executable for which the shortcut is being created.</param>
		/// <param name="iconLocation">The file path to the icon to be used for the shortcut.</param>
		/// <param name="args">The command-line arguments to be passed to the executable when the shortcut is used.</param>
		/// <returns>The created shortcut as an <see cref="IPersistFile" /> instance.</returns>
		public static IPersistFile CreateShortcutWithIcon(string exeFilePath, string iconLocation, string args)
		{
			var link = (IShellLink)new ShellLink();

			link.SetPath(exeFilePath);
			link.SetWorkingDirectory(Path.GetDirectoryName(exeFilePath));
			link.SetIconLocation(iconLocation, 0);
			link.SetArguments(args);

			return (IPersistFile)link;
		}

		/// <summary>
		/// Saves the provided shortcut to a temporary location and returns the path to the saved file.
		/// </summary>
		/// <param name="shortcut">The shortcut to be saved, represented as an <see cref="IPersistFile" /> instance.</param>
		/// <param name="exeFilePath">The file path to the executable for which the shortcut is being created. This is used to derive the name of the shortcut file.</param>
		/// <returns>The file path to the saved shortcut.</returns>
		public static string SaveShortcut(IPersistFile shortcut, string exeFilePath)
		{
			string exeFileName = Path.GetFileNameWithoutExtension(exeFilePath);
			string shortcutPath = Path.Combine(Path.GetTempPath(), exeFileName + ".lnk");

			shortcut.Save(shortcutPath, false);
			return shortcutPath;
		}
	}
}
