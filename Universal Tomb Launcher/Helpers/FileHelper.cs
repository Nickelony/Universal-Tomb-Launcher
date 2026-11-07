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
		/// Supported game executable base names (without extension).
		/// </summary>
		private static readonly string[] ValidGameExecutableBaseNames = new string[]
		{
			"tombati",
			"Tomb1Main",
			"TR1X",
			"TR2X",
			"TRX",
			"Tomb2",
			"tomb3",
			"tomb4",
			"PCTomb5",
			"TombEngine"
		};

		/// <summary>
		/// Subdirectories to search for game executables (primarily used for Tomb Engine).
		/// </summary>
		private static readonly string[] PlatformSpecificDirectories = Environment.Is64BitOperatingSystem
			? new[] { Path.Combine("Bin", "x64"), Path.Combine("Bin", "linux64"), Path.Combine("Bin", "arm64"), Path.Combine("Bin", "x86") }
			: new[] { Path.Combine("Bin", "x86") };

		/// <summary>
		/// Template-specific subdirectory to search for the game executable.
		/// </summary>
		private const string TemplateSpecificDirectory = "Engine";

		/// <summary>
		/// The name of the TRNG engine DLL file.
		/// </summary>
		private const string TombNextGenerationDllName = "Tomb_NextGeneration.dll";

		/// <summary>
		/// Checks if the specified directory is a TRNG directory by looking for the presence of <c>Tomb_NextGeneration.dll</c>.
		/// </summary>
		public static bool IsTRNGDirectory(string gameDirectory)
		{
			string engineSubdirectory = Path.Combine(gameDirectory, TemplateSpecificDirectory);

			engineSubdirectory = Directory.Exists(engineSubdirectory)
				? engineSubdirectory
				: gameDirectory;

			string dllPath = Path.Combine(engineSubdirectory, TombNextGenerationDllName);
			if (File.Exists(dllPath))
				return true;

			if (Directory.Exists(engineSubdirectory))
			{
				foreach (string file in Directory.GetFiles(engineSubdirectory))
				{
					if (Path.GetFileName(file).Equals(TombNextGenerationDllName, StringComparison.OrdinalIgnoreCase))
						return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Finds a valid game executable in the specified directory or its subdirectories.
		/// </summary>
		/// <returns>The full path to the valid game executable, or <see langword="null"/> if none is found.</returns>
		public static string? FindValidGameExecutable(string gameDirectory, out GameVersion version)
		{
			string? result = null;
			string engineSubdirectory = Path.Combine(gameDirectory, TemplateSpecificDirectory);

			if (Directory.Exists(engineSubdirectory))
				result = FindValidGameExecutableInDirectory(engineSubdirectory);

			if (string.IsNullOrEmpty(result))
			{
				foreach (string platformDir in PlatformSpecificDirectories)
				{
					string candidate = Path.Combine(engineSubdirectory, platformDir);
					if (Directory.Exists(candidate))
					{
						result = FindValidGameExecutableInDirectory(candidate);
						if (!string.IsNullOrEmpty(result))
							break;
					}
				}
			}

			if (string.IsNullOrEmpty(result))
			{
				foreach (string platformDir in PlatformSpecificDirectories)
				{
					string candidate = Path.Combine(gameDirectory, platformDir);
					if (Directory.Exists(candidate))
					{
						result = FindValidGameExecutableInDirectory(candidate);
						if (!string.IsNullOrEmpty(result))
							break;
					}
				}
			}

			if (string.IsNullOrEmpty(result))
				result = FindValidGameExecutableInDirectory(gameDirectory);

			version = GetGameVersionFromFile(result);
			return result;
		}

		/// <summary>
		/// Searches for a valid game executable in the specified directory.
		/// </summary>
		/// <returns>The full path to the valid game executable, or <see langword="null"/> if none is found.</returns>
		private static string? FindValidGameExecutableInDirectory(string searchingDirectory)
		{
			if (!Directory.Exists(searchingDirectory))
				return null;

			string[] files = Directory.GetFiles(searchingDirectory, "*", SearchOption.TopDirectoryOnly);
			return Array.Find(files, IsValidGameExecutable);
		}

		/// <summary>
		/// Checks if the given file path corresponds to a valid game executable.
		/// </summary>
		private static bool IsValidGameExecutable(string filePath)
		{
			string fileName = Path.GetFileName(filePath);
			string baseName = Path.GetFileNameWithoutExtension(filePath);
			string ext = Path.GetExtension(filePath);

			// On Windows, require .exe or check base name
			// On Linux/macOS, executables often have no extension
			if (OperatingSystem.IsWindows() && !ext.Equals(".exe", StringComparison.OrdinalIgnoreCase))
				return false;

			return Array.Exists(ValidGameExecutableBaseNames, x => x.Equals(baseName, StringComparison.OrdinalIgnoreCase));
		}

		/// <summary>
		/// Determines the game version based on the executable file name.
		/// </summary>
		private static GameVersion GetGameVersionFromFile(string? filePath)
		{
			if (string.IsNullOrEmpty(filePath))
				return GameVersion.Unknown;

			string baseName = Path.GetFileNameWithoutExtension(filePath);

			switch (baseName.ToUpperInvariant())
			{
				case "TOMBATI": return GameVersion.TR1;
				case "TOMB1MAIN": return GameVersion.Tomb1Main;
				case "TR1X": return GameVersion.TR1X;
				case "TR2X": return GameVersion.TR2X;
				case "TRX": return GameVersion.TRX;
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
