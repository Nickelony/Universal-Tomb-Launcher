using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.Win32;

namespace UniversalTombLauncher.Utils
{
	/// <summary>
	/// Helper methods for installing or uninstalling the Tomb Raider 4 fullscreen border fix.
	/// </summary>
	[SupportedOSPlatform("windows")]
	public static class FullscreenBorderFix
	{
		/// <summary>
		/// The name of the Tomb Raider 4 fullscreen border fix patch as it appears in the system registry.
		/// </summary>
		public const string TR4PatchName = "Tomb Raider 4 Fullscreen Border Fix";

		/// <summary>
		/// Checks if the Tomb Raider 4 fullscreen border fix is installed by looking for its entry in the system registry.
		/// </summary>
		public static bool IsBorderFixInstalled()
		{
			if (!OperatingSystem.IsWindows())
				return false;

			string registryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";

			using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(registryKey))
			{
				if (key != null)
				{
					foreach (string keyName in key.GetSubKeyNames())
					{
						using RegistryKey? subKey = key.OpenSubKey(keyName);
						if (subKey != null && subKey.GetValue("DisplayName") is string displayName && displayName == TR4PatchName)
							return true;
					}
				}
			}

			registryKey = @"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall";

			using (RegistryKey? key = Registry.LocalMachine.OpenSubKey(registryKey))
			{
				if (key != null)
				{
					foreach (string keyName in key.GetSubKeyNames())
					{
						using RegistryKey? subKey = key.OpenSubKey(keyName);
						if (subKey != null && subKey.GetValue("DisplayName") is string displayName && displayName == TR4PatchName)
							return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Installs the Tomb Raider 4 fullscreen border fix by extracting the embedded SDB file and executing it with sdbinst.exe.
		/// </summary>
		public static void InstallBorderFix()
		{
			if (!OperatingSystem.IsWindows())
				return;

			string sdbFilePath = Path.Combine(Path.GetTempPath(), "tr4-border-fix.sdb");

			Stream? stream = Assembly.GetExecutingAssembly()
				.GetManifestResourceStream("UniversalTombLauncher.Patches.tr4-border-fix.sdb");

			if (stream == null)
				return;

			using (FileStream fileStream = File.Create(sdbFilePath))
			{
				stream.CopyTo(fileStream);
			}

			var startInfo = new ProcessStartInfo
			{
				FileName = "sdbinst.exe",
				Arguments = $"-q \"{sdbFilePath}\""
			};

			try
			{
				using var process = Process.Start(startInfo);
				process?.WaitForExit();
			}
			catch { }

			if (File.Exists(sdbFilePath))
			{
				try
				{
					File.Delete(sdbFilePath);
				}
				catch { }
			}
		}

		/// <summary>
		/// Uninstalls the Tomb Raider 4 fullscreen border fix by executing sdbinst.exe with the -n option.
		/// </summary>
		public static void UninstallBorderFix()
		{
			if (!OperatingSystem.IsWindows())
				return;

			var startInfo = new ProcessStartInfo
			{
				FileName = "sdbinst.exe",
				Arguments = $"-n \"{TR4PatchName}\""
			};

			try
			{
				using var process = Process.Start(startInfo);
				process?.WaitForExit();
			}
			catch { }
		}
	}
}
