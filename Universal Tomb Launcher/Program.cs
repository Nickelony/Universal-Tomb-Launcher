using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text;
using UniversalTombLauncher.Enums;
using UniversalTombLauncher.Helpers;
using UniversalTombLauncher.UI;
using UniversalTombLauncher.Utils;

namespace UniversalTombLauncher
{
	internal static class Program
	{
		private static void Main(string[] args)
		{
			bool forceSetup = Array.Exists(args, x
				=> x.Equals("-s", StringComparison.OrdinalIgnoreCase)
				|| x.Equals("-setup", StringComparison.OrdinalIgnoreCase));

			bool isPreviewMode = Array.Exists(args, x
				=> x.Equals("-p", StringComparison.OrdinalIgnoreCase)
				|| x.Equals("-preview", StringComparison.OrdinalIgnoreCase));

			bool debugMode = Array.Exists(args, x
				=> x.Equals("-d", StringComparison.OrdinalIgnoreCase)
				|| x.Equals("-debug", StringComparison.OrdinalIgnoreCase));

			try
			{
				string programDirectory = AppContext.BaseDirectory;
				string? validExecutable = FileHelper.FindValidGameExecutable(programDirectory, out GameVersion version);

				if (string.IsNullOrEmpty(validExecutable))
				{
					string message = "Couldn't find a valid game executable.";

					if (FileHelper.IsTRNGDirectory(programDirectory))
					{
						message += "\n\n" +
							"Common causes:\n" +
							"- Your antivirus may have quarantined the file as a false positive\n" +
							"- The file was accidentally deleted during extraction\n\n" +
							"To resolve this:\n" +
							"1. Check your antivirus quarantine/history and restore tomb4.exe if found\n" +
							"2. If not found, re-extract the game archive\n" +
							"3. Add the game folder to your antivirus exclusions to prevent future issues";
					}

					DialogHelper.ShowError(message);
					return;
				}

				if (ProcessHelper.IsGameAlreadyRunning(version))
				{
					bool closeRunning = DialogHelper.ShowYesNo(
						"An instance of the same game engine is already running.\n" +
						"Would you like to close the already running game instance and run the new one?",
						"Warning");

					if (closeRunning)
					{
						try
						{
							ProcessHelper.FindGameProcess(version)?.Kill();
						}
						catch { }
					}
					else
					{
						return;
					}
				}

				SplashResult splashResult = SplashResult.Launch;

				if (!forceSetup)
				{
					string? overrideMessage = version == GameVersion.Tomb1Main || version == GameVersion.TR1X || version == GameVersion.TR2X || version == GameVersion.TRX
						? "Launching game..."
						: null;

					splashResult = SplashWindow.Show(isPreviewMode, overrideMessage);

					if (isPreviewMode || splashResult == SplashResult.Exit)
						return;
				}
				else
				{
					splashResult = SplashResult.Setup;
				}

				if (splashResult == SplashResult.Setup)
				{
					if (OperatingSystem.IsWindows() && OSVersionHelper.IsWindowsEightOrNewer() && version == GameVersion.TR4)
					{
						bool proceed = DialogHelper.ShowExtraSettingsSetup();
						if (!proceed)
							return; // Don't start the game
					}

					RunGame(version, validExecutable, true, debugMode);
				}
				else if (splashResult == SplashResult.Launch)
				{
					RunGame(version, validExecutable, false, debugMode);
				}
			}
			catch (Exception ex)
			{
				DialogHelper.ShowError(ex.Message, "Error");
			}
		}

		/// <summary>
		/// Runs the game executable.
		/// </summary>
		private static void RunGame(GameVersion version, string exeFilePath, bool setup = false, bool debug = false)
		{
			string? shortcutPath = null;
			int exitCode = -1;

			try
			{
				if (OperatingSystem.IsWindows() && (version == GameVersion.TR4 || version == GameVersion.TR1 || version == GameVersion.TR2 || version == GameVersion.TR3 || version == GameVersion.TR5))
				{
					try
					{
						shortcutPath = CreateGameShortcut(exeFilePath, setup, debug);
						exitCode = SecurityHelper.RunWithBatch(shortcutPath, debug);
					}
					catch
					{
						exitCode = LaunchDirectProcess(exeFilePath, setup, debug);
					}
				}
				else
				{
					exitCode = LaunchDirectProcess(exeFilePath, setup, debug);
				}

				if (version == GameVersion.TR4 && exitCode == 1)
				{
					DialogHelper.ShowWarning(
						"The game closed unexpectedly (Exit Code 1).\n\n" +
						"Common causes:\n" +
						"- The game crashed\n" +
						"- The game was forcibly closed by the user\n" +
						"- Your antivirus may have falsely blocked or quarantined the game executable\n\n" +
						"If issues persist:\n" +
						"1. Check your antivirus quarantine/history and restore tomb4.exe if found\n" +
						"2. Add the game folder to your antivirus exclusions",
						"Exit Code 1");
				}
			}
			catch (Exception ex)
			{
				DialogHelper.ShowError($"Failed to launch the game:\n{ex.Message}");
			}
			finally
			{
				if (!string.IsNullOrEmpty(shortcutPath) && File.Exists(shortcutPath))
				{
					try
					{
						File.Delete(shortcutPath);
					}
					catch { }
				}

				if (version == GameVersion.TR4)
				{
					try
					{
						string? exeDirectory = Path.GetDirectoryName(exeFilePath);
						if (!string.IsNullOrEmpty(exeDirectory))
							LogCleaner.TidyLogFiles(exeDirectory);
					}
					catch { }
				}
			}
		}

		/// <summary>
		/// Launches the game process directly using Process.Start.
		/// </summary>
		private static int LaunchDirectProcess(string exeFilePath, bool setup, bool debug)
		{
			string? workingDir = Path.GetDirectoryName(exeFilePath);
			if (string.IsNullOrEmpty(workingDir))
				workingDir = AppContext.BaseDirectory;

			var startInfo = new ProcessStartInfo
			{
				FileName = exeFilePath,
				WorkingDirectory = workingDir,
				UseShellExecute = false
			};

			if (setup)
				startInfo.ArgumentList.Add("-setup");

			if (debug)
				startInfo.ArgumentList.Add("-debug");

			using var process = Process.Start(startInfo);
			if (process == null)
				return -1;

			process.WaitForExit();
			return process.ExitCode;
		}

		/// <summary>
		/// Creates a shortcut to the game executable with the icon of this launcher on Windows.
		/// </summary>
		[SupportedOSPlatform("windows")]
		private static string CreateGameShortcut(string exeFilePath, bool setup, bool debug)
		{
			string iconLocation = Environment.ProcessPath ?? Path.Combine(AppContext.BaseDirectory, "launch.exe");

			var argumentsBuilder = new StringBuilder();

			if (setup)
				argumentsBuilder.Append("-setup ");

			if (debug)
				argumentsBuilder.Append("-debug ");

			var shortcut = ShellHelper.CreateShortcutWithIcon(exeFilePath, iconLocation, argumentsBuilder.ToString());
			return ShellHelper.SaveShortcut(shortcut, exeFilePath);
		}
	}
}
