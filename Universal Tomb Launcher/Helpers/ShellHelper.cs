using System.IO;
using System.Runtime.InteropServices.ComTypes;
using UniversalTombLauncher.Native;

namespace UniversalTombLauncher.Helpers
{
	internal static class ShellHelper
	{
		public static IPersistFile CreateShortcutWithIcon(string exeFilePath, string iconLocation, string args)
		{
			var link = (IShellLink)new ShellLink();

			link.SetPath(exeFilePath);
			link.SetWorkingDirectory(Path.GetDirectoryName(exeFilePath));
			link.SetIconLocation(iconLocation, 0);
			link.SetArguments(args);

			return (IPersistFile)link;
		}

		public static string SaveShortcut(IPersistFile shortcut, string exeFilePath)
		{
			string exeFileName = Path.GetFileNameWithoutExtension(exeFilePath);
			string shortcutPath = Path.Combine(Path.GetTempPath(), exeFileName + ".lnk");

			shortcut.Save(shortcutPath, false);
			return shortcutPath;
		}
	}
}
