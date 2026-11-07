using System;
using System.IO;

namespace UniversalTombLauncher.UI
{
	/// <summary>
	/// Helper to discover suitable system fonts across Windows, Linux, and macOS.
	/// </summary>
	internal static class FontHelper
	{
		private static readonly string[] WindowsFontCandidates = new[]
		{
			"segoeuib.ttf",
			"segoeui.ttf",
			"arialbd.ttf",
			"arial.ttf",
			"calibrib.ttf",
			"tahoma.ttf"
		};

		private static readonly string[] LinuxFontCandidates = new[]
		{
			"/usr/share/fonts/truetype/dejavu/DejaVuSans-Bold.ttf",
			"/usr/share/fonts/truetype/liberation/LiberationSans-Bold.ttf",
			"/usr/share/fonts/TTF/DejaVuSans-Bold.ttf",
			"/usr/share/fonts/noto/NotoSans-Bold.ttf",
			"/usr/share/fonts/truetype/freefont/FreeSansBold.ttf",
			"/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf",
			"/usr/share/fonts/truetype/liberation/LiberationSans-Regular.ttf",
			"/usr/share/fonts/TTF/DejaVuSans.ttf"
		};

		private static readonly string[] MacFontCandidates = new[]
		{
			"/System/Library/Fonts/SFNS.ttf",
			"/System/Library/Fonts/SFCompact.ttf",
			"/System/Library/Fonts/Helvetica.ttc",
			"/Library/Fonts/Arial.ttf"
		};

		/// <summary>
		/// Finds a valid TTF/TTC font file on the current operating system.
		/// </summary>
		public static string? FindSystemFontPath()
		{
			if (OperatingSystem.IsWindows())
			{
				string winFontsDir = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
				if (string.IsNullOrEmpty(winFontsDir))
					winFontsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Fonts");

				foreach (string candidate in WindowsFontCandidates)
				{
					string path = Path.Combine(winFontsDir, candidate);
					if (File.Exists(path))
						return path;
				}
			}
			else if (OperatingSystem.IsMacOS())
			{
				foreach (string candidate in MacFontCandidates)
				{
					if (File.Exists(candidate))
						return candidate;
				}
			}
			else // Linux / BSD / Unix
			{
				foreach (string candidate in LinuxFontCandidates)
				{
					if (File.Exists(candidate))
						return candidate;
				}
			}

			return null;
		}
	}
}
