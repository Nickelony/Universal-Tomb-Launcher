using Microsoft.Win32;

namespace UniversalTombLauncher.Helpers
{
	public static class OSVersionHelper
	{
		// Source: https://stackoverflow.com/a/37716518

		public static uint WinMajorVersion
		{
			get
			{
				// Windows 10 and above solution
				if (TryGetRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentMajorVersionNumber", out dynamic major))
					return (uint)major;

				// Windows 8 and below solution
				if (!TryGetRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentVersion", out dynamic version))
					return 0;

				var versionParts = ((string)version).Split('.');

				if (versionParts.Length != 2)
					return 0;

				return uint.TryParse(versionParts[0], out uint majorAsUInt) ? majorAsUInt : 0;
			}
		}

		public static uint WinMinorVersion
		{
			get
			{
				// Windows 10 and above solution
				if (TryGetRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentMinorVersionNumber", out dynamic minor))
					return (uint)minor;

				// Windows 8 and below solution
				if (!TryGetRegistryKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion", "CurrentVersion", out dynamic version))
					return 0;

				var versionParts = ((string)version).Split('.');

				if (versionParts.Length != 2)
					return 0;

				return uint.TryParse(versionParts[1], out uint minorAsUInt) ? minorAsUInt : 0;
			}
		}

		public static bool IsWindowsEightOrNewer()
		{
			uint major = WinMajorVersion;
			uint minor = WinMinorVersion;

			bool isWindows8 = major == 6 && minor == 2;
			bool isNewer = major > 6;

			return isWindows8 || isNewer;
		}

		private static bool TryGetRegistryKey(string path, string key, out dynamic value)
		{
			value = null;

			try
			{
				using (var rk = Registry.LocalMachine.OpenSubKey(path))
				{
					if (rk == null)
						return false;

					value = rk.GetValue(key);
					return value != null;
				}
			}
			catch
			{
				return false;
			}
		}
	}
}
