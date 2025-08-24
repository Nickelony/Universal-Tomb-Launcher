using System;
using System.IO;
using UniversalTombLauncher.Enums;

namespace UniversalTombLauncher.Helpers
{
	/// <summary>
	/// Helper methods for file operations.
	/// </summary>
	internal static class FileHelper
	{
		/// <summary>
		/// A list of supported game executable names.
		/// </summary>
		private static readonly string[] ValidGameExecutableNames = new string[]
		{
			"tombati.exe",
			"Tomb1Main.exe",
			"TR1X.exe",
			"TR2X.exe",
			"Tomb2.exe",
			"tomb3.exe",
			"tomb4.exe",
			"PCTomb5.exe",
			"TombEngine.exe"
		};

		/// <summary>
		/// Platform-specific subdirectories to search for the game executable. Currently only used for Tomb Engine.
		/// </summary>
		private static readonly string[] PlatformSpecificDirectories = new string[]
		{
			"Bin\\x86", "Bin\\x64"
		};

		/// <summary>
		/// Template-specific subdirectory to search for the game executable.
		/// </summary>
		private const string TemplateSpecificDirectory = "Engine";

		/// <summary>
		/// Finds a valid game executable in the specified directory or its subdirectories.
		/// </summary>
		/// <returns>The full path to the valid game executable, or <see langword="null" /> if none is found.</returns>
		public static string FindValidGameExecutable(string gameDirectory, out GameVersion version)
		{
			string result = null;
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

		/// <summary>
		/// Searches for a valid game executable in the specified directory.
		/// </summary>
		/// <returns>The full path to the valid game executable, or <see langword="null" /> if none is found.</returns>
		private static string FindValidGameExecutable(string searchingDirectory)
		{
			string[] files = Directory.GetFiles(searchingDirectory, "*.exe", SearchOption.TopDirectoryOnly);
			return Array.Find(files, x => IsValidGameExecutable(x));
		}

		/// <summary>
		/// Checks if the given file path corresponds to a valid game executable.
		/// </summary>
		private static bool IsValidGameExecutable(string filePath)
			=> Array.Exists(ValidGameExecutableNames, x => x.Equals(Path.GetFileName(filePath), StringComparison.OrdinalIgnoreCase));

		/// <summary>
		/// Determines the game version based on the executable file name.
		/// </summary>
		private static GameVersion GetGameVersionFromFile(string filePath)
		{
			string fileName = Path.GetFileNameWithoutExtension(filePath);

			if (fileName == null)
				return GameVersion.Unknown;

			switch (fileName.ToUpper())
			{
				case "TOMBATI": return GameVersion.TR1;
				case "TOMB1MAIN": return GameVersion.Tomb1Main;
				case "TR1X": return GameVersion.TR1X;
				case "TR2X": return GameVersion.TR2X;
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
