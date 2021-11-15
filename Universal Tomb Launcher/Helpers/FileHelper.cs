﻿using System;
using System.IO;
using UniversalTombLauncher.Enums;
using UniversalTombLauncher.Extensions;

namespace UniversalTombLauncher.Helpers
{
	public static class FileHelper
	{
		public static string FindValidGameExecutable(string gameDirectory, out GameVersion version)
		{
			string result = null;
			string engineSubdirectory = Path.Combine(gameDirectory, "Engine");

			if (Directory.Exists(engineSubdirectory))
				result = FindValidGameExecutable(engineSubdirectory);

			if (result == null)
				result = FindValidGameExecutable(gameDirectory);

			version = GetGameVersionFromFile(result);
			return result;
		}

		private static string FindValidGameExecutable(string searchingDirectory)
		{
			string[] files = Directory.GetFiles(searchingDirectory, "*.exe", SearchOption.TopDirectoryOnly);
			return Array.Find(files, x => IsValidGameExecutable(x));
		}

		private static bool IsValidGameExecutable(string filePath) =>
			Path.GetFileName(filePath).BulkStringComparision(StringComparison.OrdinalIgnoreCase,
				"tombati.exe", "Tomb2.exe", "tomb3.exe", "tomb4.exe", "PCTomb5.exe");

		private static GameVersion GetGameVersionFromFile(string filePath)
		{
			string fileName = Path.GetFileNameWithoutExtension(filePath);

			if (fileName != null)
				switch (fileName.ToUpper())
				{
					case "TOMBATI": return GameVersion.TR1;
					case "TOMB2": return GameVersion.TR2;
					case "TOMB3": return GameVersion.TR3;
					case "TOMB4": return GameVersion.TR4;
					case "PCTOMB5": return GameVersion.TR5;
				}

			return GameVersion.Unknown;
		}
	}
}