using System;
using System.Runtime.Versioning;
using Microsoft.Win32;

namespace UniversalTombLauncher.Helpers
{
	/// <summary>
	/// Helper methods for determining the operating system version on Windows.
	/// </summary>
	[SupportedOSPlatform("windows")]
	internal static class OSVersionHelper
	{
		/// <summary>
		/// Gets the major version of the Windows operating system.
		/// </summary>
		public static uint WinMajorVersion
		{
			get
			{
				if (!OperatingSystem.IsWindows())
					return 0;

				// Windows 10 and above solution
				if (TryGetRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentMajorVersionNumber", out object? major) && major is int majorInt)
					return (uint)majorInt;

				// Windows 8 and below solution
				if (!TryGetRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentVersion", out object? version) || version is not string versionStr)
					return 0;

				string[] versionParts = versionStr.Split('.');
				if (versionParts.Length != 2)
					return 0;

				return uint.TryParse(versionParts[0], out uint majorAsUInt) ? majorAsUInt : 0;
			}
		}

		/// <summary>
		/// Gets the minor version of the Windows operating system.
		/// </summary>
		public static uint WinMinorVersion
		{
			get
			{
				if (!OperatingSystem.IsWindows())
					return 0;

				// Windows 10 and above solution
				if (TryGetRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentMinorVersionNumber", out object? minor) && minor is int minorInt)
					return (uint)minorInt;

				// Windows 8 and below solution
				if (!TryGetRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentVersion", out object? version) || version is not string versionStr)
					return 0;

				string[] versionParts = versionStr.Split('.');
				if (versionParts.Length != 2)
					return 0;

				return uint.TryParse(versionParts[1], out uint minorAsUInt) ? minorAsUInt : 0;
			}
		}

		/// <summary>
		/// Determines whether the operating system is Windows 8 or newer.
		/// </summary>
		public static bool IsWindowsEightOrNewer()
		{
			if (!OperatingSystem.IsWindows())
				return false;

			uint major = WinMajorVersion;
			uint minor = WinMinorVersion;

			bool isWindows8 = major == 6 && minor == 2;
			bool isNewer = major > 6;

			return isWindows8 || isNewer;
		}

		/// <summary>
		/// Tries to get a registry key value from the specified path.
		/// </summary>
		private static bool TryGetRegistryKey(string path, string key, out object? value)
		{
			value = null;

			try
			{
				using RegistryKey? rk = Registry.LocalMachine.OpenSubKey(path);
				if (rk == null)
					return false;

				value = rk.GetValue(key);
				return value != null;
			}
			catch
			{
				return false;
			}
		}
	}
}
