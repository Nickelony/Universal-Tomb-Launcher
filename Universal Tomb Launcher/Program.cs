using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using UniversalTombLauncher.Enums;
using UniversalTombLauncher.Forms;
using UniversalTombLauncher.Helpers;
using UniversalTombLauncher.Utils;

namespace UniversalTombLauncher
{
	internal static class Program
	{
		[STAThread]
		private static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

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
				string programDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				string validExecutable = FileHelper.FindValidGameExecutable(programDirectory, out GameVersion version);

				if (string.IsNullOrEmpty(validExecutable))
				{
					string message = "Couldn't find a valid game executable.";

					if (FileHelper.IsTRNGDirectory(programDirectory))
					{
						message += "\n\n" +
							"Some antivirus software may flag the game executable as a false positive.\n\n" +
							"If your antivirus removed the tomb4.exe file, please restore it from quarantine or re-extract the archive.\n\n" +
							"If issues persist, please add an exception for the game folder in your antivirus software.";
					}

					throw new ArgumentException(message);
				}

				if (ProcessHelper.IsGameAlreadyRunning(version))
				{
					DialogResult result = MessageBox.Show(
						"An instance of the same game engine is already running.\n" +
						"Would you like to close the already running game instance and run the new one?",
						"Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

					if (result == DialogResult.Yes)
						ProcessHelper.FindGameProcess(version)?.Kill();
					else
						return;
				}

				DialogResult splashResult;
				const DialogResult PRESSED_CTRL = DialogResult.OK;
				const DialogResult TIME_PASSED = DialogResult.Cancel;

				if (!forceSetup)
				{
					string overrideMessage = version == GameVersion.Tomb1Main || version == GameVersion.TR1X || version == GameVersion.TR2X
						? "Launching game..."
						: null;

					splashResult = ShowSplashScreen(isPreviewMode, overrideMessage);

					if (isPreviewMode)
						return;
				}
				else
				{
					splashResult = PRESSED_CTRL;
				}

				if (splashResult == PRESSED_CTRL)
				{
					if (OSVersionHelper.IsWindowsEightOrNewer() && version == GameVersion.TR4)
					{
						DialogResult setupResult = ShowExtraSettingsSetup();

						if (setupResult == DialogResult.Cancel)
							return; // Don't start the game
					}

					RunGame(version, validExecutable, true, debugMode);
				}
				else if (splashResult == TIME_PASSED)
				{
					RunGame(version, validExecutable, false, debugMode);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private static DialogResult ShowSplashScreen(bool previewMode = false, string overrideMessage = null)
		{
			using (var form = new FormSetupSplash(previewMode, overrideMessage))
				return form.ShowDialog();
		}

		private static DialogResult ShowExtraSettingsSetup()
		{
			using (var form = new FormExtraSettings())
				return form.ShowDialog();
		}

		/// <summary>
		/// Runs the game executable, creating a shortcut to it first to apply the launcher's icon.
		/// </summary>
		private static void RunGame(GameVersion version, string exeFilePath, bool setup = false, bool debug = false)
		{
			// We must create a shortcut of the game and run it instead to apply the icon of this launcher to the game window
			string shortcutPath = CreateGameShortcut(exeFilePath, setup, debug);

			try
			{
				int exitCode = SecurityHelper.RunWithBatch(shortcutPath);

				if (version == GameVersion.TR4 && exitCode == 1)
				{
					// TR4 specific: Exit code 1 means the executable was quarantined or deleted by the antivirus
					MessageBox.Show(
						"The game unexpectedly closed due to an error.\n\n" +
						"Some antivirus software may flag the game executable as a false positive.\n\n" +
						"If your antivirus is blocking the game, please add an exception for it.",
						"Game unexpectedly closed", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			}
			catch { }
			finally
			{
				// Delete the shortcut once the game is closed
				if (File.Exists(shortcutPath))
				{
					try
					{
						File.Delete(shortcutPath);
					}
					catch { }
				}

				if (version == GameVersion.TR4) // TR4 or TRNG
				{
					try
					{
						// Clean up the logs - move them to a sub-folder
						string exeDirectory = Path.GetDirectoryName(exeFilePath);
						LogCleaner.TidyLogFiles(exeDirectory);
					}
					catch { } // Failing to clean up logs is not critical
				}
			}
		}

		/// <summary>
		/// Creates a shortcut to the game executable with the icon of this launcher.
		/// </summary>
		/// <returns>The path to the created shortcut file.</returns>
		private static string CreateGameShortcut(string exeFilePath, bool setup, bool debug)
		{
			string iconLocation = Assembly.GetExecutingAssembly().Location; // Target icon is the icon of this launcher

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
