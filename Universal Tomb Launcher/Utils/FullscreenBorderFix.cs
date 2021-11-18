using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace UniversalTombLauncher.Utils
{
	public static class FullscreenBorderFix
	{
		public const string TR4PatchName = "Tomb Raider 4 Fullscreen Border Fix";

		public static bool IsBorderFixInstalled()
		{
			string displayName;
			string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

			using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey))
			{
				if (key != null)
				{
					foreach (RegistryKey subKey in key.GetSubKeyNames().Select(keyName => key.OpenSubKey(keyName)))
					{
						displayName = (string)subKey.GetValue("DisplayName");

						if (displayName != null && displayName == TR4PatchName)
							return true;
					}
				}
			}

			registryKey = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";

			using (RegistryKey key = Registry.LocalMachine.OpenSubKey(registryKey))
			{
				if (key != null)
				{
					foreach (RegistryKey subKey in key.GetSubKeyNames().Select(keyName => key.OpenSubKey(keyName)))
					{
						displayName = (string)subKey.GetValue("DisplayName");

						if (displayName != null && displayName == TR4PatchName)
							return true;
					}
				}
			}

			return false;
		}

		public static void InstallBorderFix()
		{
			string sdbFilePath = Path.Combine(Path.GetTempPath(), "tr4-border-fix.sdb");

			using (FileStream fileStream = File.Create(sdbFilePath))
			{
				Assembly.GetExecutingAssembly()
					.GetManifestResourceStream("UniversalTombLauncher.Patches.tr4-border-fix.sdb")
					.CopyTo(fileStream);
			}

			var startInfo = new ProcessStartInfo
			{
				FileName = "sdbinst.exe",
				Arguments = $"-q \"{sdbFilePath}\""
			};

			try { Process.Start(startInfo).WaitForExit(); }
			catch { }

			if (File.Exists(sdbFilePath))
				File.Delete(sdbFilePath);
		}

		public static void UninstallBorderFix()
		{
			var startInfo = new ProcessStartInfo
			{
				FileName = "sdbinst.exe",
				Arguments = $"-n \"{TR4PatchName}\""
			};

			try { Process.Start(startInfo).WaitForExit(); }
			catch { }
		}
	}
}
