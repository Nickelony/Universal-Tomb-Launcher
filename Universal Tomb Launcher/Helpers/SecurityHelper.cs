using System.Diagnostics;
using System.IO;
using System.Text;

namespace UniversalTombLauncher.Helpers
{
	/// <summary>
	/// Helper methods for dealing with security features like SmartScreen.
	/// </summary>
	internal static class SecurityHelper
	{
		/// <summary>
		/// Creates a batch file that can be used to launch the game, potentially bypassing some SmartScreen issues.
		/// </summary>
		/// <param name="targetFilePath">The file path to the game executable to be launched.</param>
		/// <returns>The file path to the created batch file, or <see langword="null"/> if it could not be created.</returns>
		public static string CreateLaunchBatch(string targetFilePath)
		{
			try
			{
				string batchPath = Path.Combine(Path.GetTempPath(),
					Path.GetFileNameWithoutExtension(targetFilePath) + ".bat");

				var batchContent = new StringBuilder();
				batchContent.AppendLine("@echo off");
				batchContent.AppendLine($"cd /d \"{Path.GetDirectoryName(targetFilePath)}\"");
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
		/// <param name="targetFilePath">The file path to the game executable to be launched.</param>
		/// <param name="debug">Whether the launcher has been opened with the <c>-debug</c> argument, which should show the game's console window (if supported).</param>
		/// <returns>The exit code of the game process, or -1 if it could not be started.</returns>
		public static int RunWithBatch(string targetFilePath, bool debug = false)
		{
			string batchPath = null;

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

				using (var process = Process.Start(startInfo))
				{
					process?.WaitForExit();
					return process?.ExitCode ?? -1;
				}
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
