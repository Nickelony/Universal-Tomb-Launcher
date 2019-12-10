using IWshRuntimeLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace UniversalTombLauncher
{
	internal static class Program
	{
		[STAThread]
		private static void Main()
		{
			try
			{
				string exeDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				string engineSubdirectory = Path.Combine(exeDirectory, "Engine");

				string validExeFilePath = string.Empty;

				// Look for a valid game .exe file

				// Try to find it in the /Engine/ directory first (if it exists)
				if (Directory.Exists(engineSubdirectory))
				{
					foreach (string file in Directory.GetFiles(engineSubdirectory, "*.exe", SearchOption.TopDirectoryOnly))
					{
						if (IsValidGameExeFile(file))
						{
							validExeFilePath = file;
							break;
						}
					}
				}

				// If the engine subdirectory doesn't exist or no valid .exe file was found in that directory
				if (string.IsNullOrEmpty(validExeFilePath))
				{
					// Try to look for the file in the executing directory

					foreach (string file in Directory.GetFiles(exeDirectory, "*.exe", SearchOption.TopDirectoryOnly))
					{
						if (IsValidGameExeFile(file))
						{
							validExeFilePath = file;
							break;
						}
					}
				}

				if (string.IsNullOrEmpty(validExeFilePath)) // No valid .exe file was found
					throw new ArgumentException("Couldn't find a valid game .exe file.");
				else
				{
					using (FormSetupSplash form = new FormSetupSplash(Path.GetDirectoryName(validExeFilePath)))
					{
						if (form.ShowDialog() == DialogResult.OK) // Pressed CTRL
							RunGame(validExeFilePath, true);
						else // 1 second passed
							RunGame(validExeFilePath);
					}
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

			string fileName = Path.GetFileNameWithoutExtension(exeFilePath);

			WshShell shell = new WshShell();
			string shortcutPath = Path.Combine(Path.GetTempPath(), fileName + ".lnk");

			IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);

			shortcut.TargetPath = exeFilePath;
			shortcut.WorkingDirectory = Path.GetDirectoryName(exeFilePath);
			shortcut.IconLocation = Assembly.GetExecutingAssembly().Location;

			if (setup)
				shortcut.Arguments = "-setup";

			shortcut.Save();

			try
			{
				Process process = Process.Start(shortcutPath);
				process.WaitForExit();
			}
			finally
			{
				if (System.IO.File.Exists(shortcutPath))
					System.IO.File.Delete(shortcutPath);

				MoveLogFiles(exeFilePath);
			}
		}

		private static void MoveLogFiles(string exeFilePath)
		{
			string validExeDirectory = Path.GetDirectoryName(exeFilePath);
			string logsDirectory = Path.Combine(validExeDirectory, "logs");

			if (!Directory.Exists(logsDirectory))
				Directory.CreateDirectory(logsDirectory);

			string[] files = Directory.GetFiles(validExeDirectory);

			foreach (string file in files)
			{
				string fileName = Path.GetFileName(file).ToLower();

				if (fileName == "db_patches_crash.bin" || fileName == "detected crash.txt"
					|| fileName == "lastextraction.lst" || fileName.EndsWith("_warm_up_log.txt"))
				{
					System.IO.File.Copy(file, Path.Combine(logsDirectory, Path.GetFileName(file)), true);
					System.IO.File.Delete(file);
				}
				else if (fileName.StartsWith("last_crash_") && fileName.EndsWith(".txt"))
				{
					string[] existingLogFiles = Directory.GetFiles(logsDirectory, "last_crash_*.txt", SearchOption.TopDirectoryOnly);

					string logTxtFilePath = string.Empty;

					if (existingLogFiles.Length > 0)
					{
						List<int> existingNumbers = new List<int>();

						foreach (string existingLogFile in existingLogFiles)
							existingNumbers.Add(int.Parse(Path.GetFileNameWithoutExtension(existingLogFile).Split('_')[2]));

						int nextLogNumber = existingNumbers.Max() + 1;

						logTxtFilePath = Path.Combine(logsDirectory, "Last_Crash_" + nextLogNumber + ".txt");
					}
					else
						logTxtFilePath = Path.Combine(logsDirectory, Path.GetFileName(file));

					System.IO.File.Copy(file, logTxtFilePath, true);
					System.IO.File.Delete(file);

					string memFilePath = Path.Combine(Path.GetDirectoryName(file), Path.GetFileNameWithoutExtension(file) + ".mem");

					if (System.IO.File.Exists(memFilePath))
					{
						System.IO.File.Copy(memFilePath, Path.Combine(logsDirectory, Path.GetFileNameWithoutExtension(logTxtFilePath) + ".mem"), true);
						System.IO.File.Delete(memFilePath);
					}
				}
			}

			if (Directory.GetFileSystemEntries(logsDirectory).Length == 0)
				Directory.Delete(logsDirectory);
		}

		private static bool IsValidGameExeFile(string file)
		{
			string fileName = Path.GetFileName(file).ToLower();
			return fileName == "tomb2.exe" || fileName == "tomb3.exe" || fileName == "tomb4.exe" || fileName == "pctomb5.exe";
		}
	}
}
