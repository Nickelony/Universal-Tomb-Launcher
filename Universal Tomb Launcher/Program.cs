using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using UniversalTombLauncher.Enums;
using UniversalTombLauncher.Helpers;

namespace UniversalTombLauncher
{
	internal static class Program
	{
		[STAThread]
		private static void Main()
		{
			try
			{
				string programDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				string validExecutable = FileHelper.FindValidGameExecutable(programDirectory, out GameVersion version);

				if (string.IsNullOrEmpty(validExecutable))
					throw new ArgumentException("Couldn't find a valid game .exe file.");

				if (ProcessHelper.IsGameAlreadyRunning(version))
				{
					DialogResult result = MessageBox.Show(
						"An instance of the same game engine is already running.\n" +
						"Would you like to close the already running game instance and run the new one?",
						"Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

					if (result == DialogResult.Yes)
						ProcessHelper.FindGameProcess(version)?.Kill();
					else
						return;
				}

				using (var form = new FormSetupSplash(Path.GetDirectoryName(validExecutable)))
				{
					if (form.ShowDialog() == DialogResult.OK) // Pressed CTRL
						RunGame(validExecutable, true);
					else // 1 second passed
						RunGame(validExecutable);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private static void RunGame(string exeFilePath, bool setup = false)
		{
			// We must create a shortcut of the game and run it instead to apply the icon of this launcher to the game window
			var shortcut = ShellHelper.CreateShortcutWithIcon(exeFilePath, Assembly.GetExecutingAssembly().Location);
			shortcut.Arguments = setup ? "-setup" : string.Empty;
			shortcut.Save();

			try { Process.Start(shortcut.FullName).WaitForExit(); }
			catch { }

			if (File.Exists(shortcut.FullName))
				File.Delete(shortcut.FullName);

			string exeDirectory = Path.GetDirectoryName(exeFilePath);
			LogCleaner.TidyLogFiles(exeDirectory);
		}
	}
}
