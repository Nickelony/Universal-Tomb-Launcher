using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using UniversalTombLauncher.Forms.Bases;
using UniversalTombLauncher.Helpers;

namespace UniversalTombLauncher.Forms
{
	public partial class FormSetupSplash : FormAcrylic
	{
		#region Fields

		private readonly int[] ValidSplashWidths = new int[]
		{
			1024, 768, 512, 384
		};

		private readonly int[] ValidSplashHeight = new int[]
		{
			512, 384, 256
		};

		private bool _previewMode;

		#endregion Fields

		#region Construction

		public FormSetupSplash(string splashImageDirectory, bool previewMode, string overrideMessage = null)
		{
			InitializeComponent();

			_previewMode = previewMode;

			if (overrideMessage != null)
				label_Message.Text = overrideMessage;

			string splashImagePath = Path.Combine(splashImageDirectory, "splash.bmp");

			if (File.Exists(splashImagePath))
				InitializeSplashImage(splashImagePath);
			else
				DisposeSplashImagePanel();
		}

		private void InitializeSplashImage(string splashImagePath)
		{
			var bitmap = Image.FromFile(splashImagePath);

			bool isValidWidth = Array.Exists(ValidSplashWidths, x => x == bitmap.Width);
			bool isValidHeight = Array.Exists(ValidSplashHeight, x => x == bitmap.Height);

			if (isValidWidth && isValidHeight)
			{
				Width = bitmap.Width;
				Height = panel_Top.Height + bitmap.Height + label_Message.Height;

				panel_SplashImage.BackgroundImage = bitmap;
			}
			else
			{
				bitmap.Dispose();
				DisposeSplashImagePanel();
			}
		}

		private void DisposeSplashImagePanel()
		{
			panel_SplashImage.Dispose();

			label_Message.TextAlign = ContentAlignment.TopCenter;
			label_Message.Dock = DockStyle.Fill;

			Rectangle screenRectangle = RectangleToScreen(ClientRectangle);
			int titleBarHeight = screenRectangle.Top - Top;
			int fontHeight = (int)Math.Ceiling(label_Message.Font.GetHeight());

			if (titleBarHeight == 0 || FormBorderStyle == FormBorderStyle.None)
				titleBarHeight = panel_Top.Height;

			Height = (titleBarHeight * 2) + fontHeight;
		}

		#endregion Construction

		#region Overrides

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);

			if (SupportsAcrylic)
				SetAcrylicWindowStyle();

			if (IsUsingLightTheme)
				SetLightTheme();
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);

			if (_previewMode)
				label_Message.Text = "Press ESC to EXIT preview mode...";
			else
				timer_Input.Start();

			timer_Animation.Start();
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			if (!_previewMode && e.Control)
			{
				timer_Input.Stop();
				DialogResult = DialogResult.OK;
			}

			if (_previewMode && e.KeyCode == Keys.Escape)
				DialogResult = DialogResult.OK;
		}

		#endregion Overrides

		#region Event handlers

		private void Timer_Animation_Tick(object sender, EventArgs e)
		{
			Opacity += 0.1;

			if (Opacity == 1.0)
			{
				ForceClickOnWindow(); // Fix for dgVoodoo's wrong initial window state issue
				timer_Animation.Stop();
			}
		}

		private void Timer_Input_Tick(object sender, EventArgs e)
		{
			timer_Input.Stop();
			DialogResult = DialogResult.Cancel;
		}

		#endregion Event handlers

		#region Other methods

		private void SetAcrylicWindowStyle()
		{
			FormBorderStyle = FormBorderStyle.FixedDialog;

			panel_Top.Visible = false;

			label_Message.BackColor = Color.Transparent;
		}

		private void SetLightTheme()
		{
			label_Message.ForeColor = Color.Black;

			if (!SupportsAcrylic)
				BackColor = Color.White;
		}

		private void ForceClickOnWindow()
		{
			var windowCenterPoint = new Point(Width / 2, Height / 2);
			InputHelper.ClickOnPoint(Handle, windowCenterPoint);
		}

		#endregion Other methods
	}
}
