using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UniversalTombLauncher.Extensions;

namespace UniversalTombLauncher.Utils
{
	public static class LogCleaner
	{
		private const string CrashLogFileNameTemplate = "Last_Crash_{0}.txt";

		public static void TidyLogFiles(string gameDirectory)
		{
			string logsDirectory = Path.Combine(gameDirectory, "logs");

			if (!Directory.Exists(logsDirectory))
				Directory.CreateDirectory(logsDirectory);

			foreach (string file in Directory.GetFiles(gameDirectory))
			{
				string fileName = Path.GetFileName(file);

				if (IsValidLogFile(fileName))
				{
					string destPath = Path.Combine(logsDirectory, Path.GetFileName(file));
					MoveFileEx(file, destPath, true);
				}
				else if (IsValidNumberedCrashLog(fileName))
					MoveNumberedLog(file, logsDirectory);
			}

			bool isLogsDirEmpty = Directory.GetFileSystemEntries(logsDirectory).Length == 0;

			if (isLogsDirEmpty)
				Directory.Delete(logsDirectory);
		}

		private static bool IsValidLogFile(string fileName) =>
			fileName.BulkStringComparision(StringComparison.OrdinalIgnoreCase,
				"db_patches_crash.bin", "DETECTED CRASH.txt", "LastExtraction.lst")
			|| fileName.EndsWith("_warm_up_log.txt", StringComparison.OrdinalIgnoreCase);

		private static bool IsValidNumberedCrashLog(string fileName) =>
			Regex.IsMatch(fileName, @"last_crash_\d+\.txt", RegexOptions.IgnoreCase);

		private static void MoveNumberedLog(string logFile, string logsDirectory)
		{
			int logId = GetNextAvailableCrashLogId(logsDirectory);
			string normalizedLogId = logId.ToString().PadLeft(3, '0');

			string logFileName = string.Format(CrashLogFileNameTemplate, normalizedLogId);
			string destPath = Path.Combine(logsDirectory, logFileName);

			MoveFileEx(logFile, destPath, true);

			string memFilePath = Path.Combine(Path.GetDirectoryName(logFile), Path.GetFileNameWithoutExtension(logFile) + ".mem");

			if (File.Exists(memFilePath))
			{
				destPath = Path.Combine(logsDirectory, Path.GetFileNameWithoutExtension(logFileName) + ".mem");
				MoveFileEx(memFilePath, destPath, true);
			}
		}

		private static int GetNextAvailableCrashLogId(string logsDirectory)
		{
			string[] existingLogFiles = Directory.GetFiles(logsDirectory, "last_crash_*.txt", SearchOption.TopDirectoryOnly);

			if (existingLogFiles.Length == 0)
				return 1;

			var takenNumbers = new List<int>();

			foreach (string logFile in existingLogFiles)
			{
				string fileName = Path.GetFileNameWithoutExtension(logFile);
				string logId = fileName.Split('_').Last();

				if (int.TryParse(logId, out int id))
					takenNumbers.Add(id);
			}

			return takenNumbers.Max() + 1;
		}

		private static void MoveFileEx(string source, string dest, bool overwrite = false)
		{
			File.Copy(source, dest, overwrite);
			File.Delete(source);
		}
	}
}
