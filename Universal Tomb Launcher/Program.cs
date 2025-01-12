using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
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
					throw new ArgumentException("Couldn't find a valid game .exe file.");

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
					string overrideMessage = version == GameVersion.Tomb1Main || version == GameVersion.TR1X
						? "Launching game..."
						: null;

					Application.VisualStyleState = VisualStyleState.ClientAndNonClientAreasEnabled;
					splashResult = ShowSplashScreen(isPreviewMode, overrideMessage);
					Application.VisualStyleState = VisualStyleState.NonClientAreaEnabled;

					if (isPreviewMode)
						return;
				}
				else
					splashResult = PRESSED_CTRL;

				if (splashResult == PRESSED_CTRL)
				{
					if (OSVersionHelper.IsWindowsEightOrNewer() && version == GameVersion.TR4)
					{
						DialogResult setupResult = ShowExtraSettingsSetup();

						if (setupResult == DialogResult.Cancel)
							return; // Don't start the game
					}

					RunGame(validExecutable, true, debugMode);
				}
				else if (splashResult == TIME_PASSED)
					RunGame(validExecutable, false, debugMode);
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

		private static void RunGame(string exeFilePath, bool setup = false, bool debug = false)
		{
			// We must create a shortcut of the game and run it instead to apply the icon of this launcher to the game window
			string shortcutPath = CreateGameShortcut(exeFilePath, setup, debug);

			try
			{
				// Start the game and wait for it to exit
				Process.Start(shortcutPath).WaitForExit();

				// Delete the shortcut once the game is closed
				if (File.Exists(shortcutPath))
					File.Delete(shortcutPath);

				// Clean up the logs - move them to a sub-folder
				string exeDirectory = Path.GetDirectoryName(exeFilePath);
				LogCleaner.TidyLogFiles(exeDirectory);
			}
			catch { }
		}

		/// <returns>Path to the shortcut.</returns>
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
