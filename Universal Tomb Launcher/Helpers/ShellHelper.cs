using IWshRuntimeLibrary;
using System.IO;

namespace UniversalTombLauncher.Helpers
{
	public static class ShellHelper
	{
		public static IWshShortcut CreateShortcutWithIcon(string exeFilePath, string iconLocation)
		{
			string fileName = Path.GetFileNameWithoutExtension(exeFilePath);

			var shell = new WshShell();
			string shortcutPath = Path.Combine(Path.GetTempPath(), fileName + ".lnk");

			var shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
			shortcut.TargetPath = exeFilePath;
			shortcut.WorkingDirectory = Path.GetDirectoryName(exeFilePath);
			shortcut.IconLocation = iconLocation;

			return shortcut;
		}
	}
}
