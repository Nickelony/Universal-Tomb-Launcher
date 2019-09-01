using IWshRuntimeLibrary;
using Microsoft.Win32;
using System;
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
			string exeDirectoryPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

			// Look for a valid game .exe file
			foreach (string file in Directory.GetFiles(exeDirectoryPath, "*.exe", SearchOption.TopDirectoryOnly))
			{
				string fileName = Path.GetFileName(file).ToLower();

				if (fileName == "tomb2.exe" || fileName == "tomb3.exe" || fileName == "tomb4.exe" || fileName == "pctomb5.exe")
				{
					// Install border fix for tomb4 games if it wasn't installed yet
					if (fileName == "tomb4.exe" && IsRequiredWindowsVersion() && !IsBorderFixInstalled())
						InstallBorderFix();

					// Add the "Press CTRL key for SETUP" prompt for vanilla TR games
					if (!System.IO.File.Exists(Path.Combine(exeDirectoryPath, "Tomb_NextGeneration.dll")))
					{
						using (FormSetupPrompt form = new FormSetupPrompt())
						{
							if (form.ShowDialog() == DialogResult.OK)
								RunGame(file, true);
							else
								RunGame(file);
						}
					}
					else // TRNG already has that feature
						RunGame(file);

					return;
				}
			}

			MessageBox.Show("Couldn't find a valid game .exe file.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
			}
		}

		private static void InstallBorderFix()
		{
			string sdbFilePath = Path.Combine(Path.GetTempPath(), "tr4-border-fix.sdb");

			using (FileStream fileStream = System.IO.File.Create(sdbFilePath))
				Assembly.GetExecutingAssembly().GetManifestResourceStream("UniversalTombLauncher.Patches.tr4-border-fix.sdb").CopyTo(fileStream);

			ProcessStartInfo startInfo = new ProcessStartInfo
			{
				FileName = "sdbinst.exe",
				Arguments = "-q \"" + sdbFilePath + "\""
			};

			try
			{
				Process process = Process.Start(startInfo);
				process.WaitForExit();
			}
			finally
			{
				if (System.IO.File.Exists(sdbFilePath))
					System.IO.File.Delete(sdbFilePath);
			}
		}

		private static bool IsBorderFixInstalled()
		{
			string seriesPatchName = "Tomb Raider series fullscreen border fix"; // A patch by Garrett from PCGamingWiki
			string tr4PatchName = "Tomb Raider 4 Fullscreen Border Fix"; // My own patch which only affects tomb4.exe files

			string displayName = string.Empty;

			string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
			RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey);

			if (key != null)
			{
				foreach (RegistryKey subkey in key.GetSubKeyNames().Select(keyName => key.OpenSubKey(keyName)))
				{
					displayName = (string)subkey.GetValue("DisplayName");

					if (displayName != null && (displayName == seriesPatchName || displayName == tr4PatchName))
						return true;
				}

				key.Close();
			}

			registryKey = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
			key = Registry.LocalMachine.OpenSubKey(registryKey);

			if (key != null)
			{
				foreach (RegistryKey subkey in key.GetSubKeyNames().Select(keyName => key.OpenSubKey(keyName)))
				{
					displayName = (string)subkey.GetValue("DisplayName");

					if (displayName != null && (displayName == seriesPatchName || displayName == tr4PatchName))
						return true;
				}

				key.Close();
			}

			return false;
		}

		private static bool IsRequiredWindowsVersion()
		{
			RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
			string productName = (string)key.GetValue("ProductName");

			return productName.StartsWith("Windows 10") || productName.StartsWith("Windows 8") || productName.StartsWith("Windows 7");
		}
	}
}
