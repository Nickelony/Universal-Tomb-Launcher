using System.Diagnostics;
using System.IO;
using System.Runtime.Versioning;
using System.Text;

namespace UniversalTombLauncher.Helpers
{
	/// <summary>
	/// Helper methods for dealing with security features like SmartScreen on Windows.
	/// </summary>
	[SupportedOSPlatform("windows")]
	internal static class SecurityHelper
	{
		/// <summary>
		/// Creates a batch file that can be used to launch the game, potentially bypassing some SmartScreen issues.
		/// </summary>
		public static string? CreateLaunchBatch(string targetFilePath)
		{
			try
			{
				string? dir = Path.GetDirectoryName(targetFilePath);
				string batchPath = Path.Combine(Path.GetTempPath(),
					Path.GetFileNameWithoutExtension(targetFilePath) + ".bat");

				var batchContent = new StringBuilder();
				batchContent.AppendLine("@echo off");
				if (!string.IsNullOrEmpty(dir))
					batchContent.AppendLine($"cd /d \"{dir}\"");
				batchContent.AppendLine($"\"{Path.GetFileName(targetFilePath)}\"");

				File.WriteAllText(batchPath, batchContent.ToString());
				return batchPath;
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Runs the game using a batch file wrapper.
		/// </summary>
		public static int RunWithBatch(string targetFilePath, bool debug = false)
		{
			string? batchPath = null;

			try
			{
				batchPath = CreateLaunchBatch(targetFilePath);

				if (string.IsNullOrEmpty(batchPath))
					return -1;

				var startInfo = new ProcessStartInfo
				{
					FileName = batchPath,
					UseShellExecute = true,
					WindowStyle = debug ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden
				};

				using var process = Process.Start(startInfo);
				process?.WaitForExit();
				return process?.ExitCode ?? -1;
			}
			catch
			{
				return -1;
			}
			finally
			{
				// Clean up batch file
				if (batchPath != null && File.Exists(batchPath))
				{
					try
					{
						File.Delete(batchPath);
					}
					catch { }
				}
			}
		}
	}
}
