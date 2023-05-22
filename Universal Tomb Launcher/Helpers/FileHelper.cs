using System;
using System.IO;
using UniversalTombLauncher.Enums;

namespace UniversalTombLauncher.Helpers
{
	internal static class FileHelper
	{
		private static readonly string[] ValidGameExecutableNames = new string[]
		{
			"tombati.exe", "Tomb1Main.exe", "Tomb2.exe", "tomb3.exe", "tomb4.exe", "PCTomb5.exe", "TombEngine.exe"
		};

		private static readonly string[] PlatformSpecificDirectories = new string[]
		{
			"Bin\\x86", "Bin\\x64"
		};

		private static readonly string TemplateSpecificDirectory = "Engine";

		public static string FindValidGameExecutable(string gameDirectory, out GameVersion version)
		{
			string result = string.Empty;
			string platformSpecificDirectory = PlatformSpecificDirectories[Environment.Is64BitOperatingSystem ? 1 : 0];

			string engineSubdirectory = Path.Combine(gameDirectory, TemplateSpecificDirectory);

			if (Directory.Exists(engineSubdirectory))
				result = FindValidGameExecutable(engineSubdirectory);

			if (string.IsNullOrEmpty(result))
			{
				engineSubdirectory = Path.Combine(engineSubdirectory, platformSpecificDirectory);

				if (Directory.Exists(engineSubdirectory))
					result = FindValidGameExecutable(engineSubdirectory);
			}

			if (string.IsNullOrEmpty(result))
			{
				engineSubdirectory = Path.Combine(gameDirectory, platformSpecificDirectory);

				if (Directory.Exists(engineSubdirectory))
					result = FindValidGameExecutable(engineSubdirectory);
			}

			if (string.IsNullOrEmpty(result))
				result = FindValidGameExecutable(gameDirectory);

			version = GetGameVersionFromFile(result);
			return result;
		}

		private static string FindValidGameExecutable(string searchingDirectory)
		{
			string[] files = Directory.GetFiles(searchingDirectory, "*.exe", SearchOption.TopDirectoryOnly);
			return Array.Find(files, x => IsValidGameExecutable(x));
		}

		private static bool IsValidGameExecutable(string filePath)
			=> Array.Exists(ValidGameExecutableNames, x => x.Equals(Path.GetFileName(filePath), StringComparison.OrdinalIgnoreCase));

		private static GameVersion GetGameVersionFromFile(string filePath)
		{
			string fileName = Path.GetFileNameWithoutExtension(filePath);

			if (fileName == null)
				return GameVersion.Unknown;

			switch (fileName.ToUpper())
			{
				case "TOMBATI": return GameVersion.TR1;
				case "TOMB1MAIN": return GameVersion.Tomb1Main;
				case "TOMB2": return GameVersion.TR2;
				case "TOMB3": return GameVersion.TR3;
				case "TOMB4": return GameVersion.TR4;
				case "PCTOMB5": return GameVersion.TR5;
				case "TOMBENGINE": return GameVersion.TombEngine;
				default: return GameVersion.Unknown;
			}
		}
	}
}
