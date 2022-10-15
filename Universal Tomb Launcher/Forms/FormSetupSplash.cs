using Microsoft.Win32;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using UniversalTombLauncher.Enums;
using UniversalTombLauncher.Helpers;
using UniversalTombLauncher.Utils;

namespace UniversalTombLauncher.Forms
{
	internal partial class FormSetupSplash : Form
	{
		#region Fields

		private const string PersonalizationRegistryKey = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";

		private readonly int[] ValidSplashWidths = new int[]
		{
			1024, 768, 512, 384
		};

		private readonly int[] ValidSplashHeight = new int[]
		{
			512, 384, 256
		};

		private readonly Configuration _config = new Configuration().Load();

		private bool _isPreviewMode;
		private bool _supportsAcrylic;

		#endregion Fields

		#region Construction

		public FormSetupSplash(string splashImageDirectory, bool isPreviewMode, string overrideMessage = null)
		{
			_isPreviewMode = isPreviewMode;
			UpdatePersonalizationProperties();

			InitializeComponent();

			label_Message.ForeColor = ColorTranslator.FromHtml(_config.FontColor);
			timer_Input.Interval = _config.DisplayTimeMilliseconds;

			if (overrideMessage != null)
				label_Message.Text = overrideMessage;

			string splashImagePath = Path.Combine(splashImageDirectory, "splash.bmp");

			if (File.Exists(splashImagePath))
				InitializeSplashImage(splashImagePath);
			else
				DisposeSplashImagePanel();
		}

		private void UpdatePersonalizationProperties()
		{
			using (RegistryKey key = Registry.CurrentUser.OpenSubKey(PersonalizationRegistryKey))
			{
				if (key == null)
					return;

				object transparencyKey = key.GetValue("EnableTransparency");

				if (transparencyKey != null && int.TryParse(transparencyKey.ToString(), out int transparencyKeyValue))
					_supportsAcrylic = transparencyKeyValue > 0;
			}
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

			label_Message.Dock = DockStyle.Fill;
			Height = label_Message.Height;
		}

		#endregion Construction

		#region Overrides

		private const int CS_DROPSHADOW = 0x20000;

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ClassStyle |= CS_DROPSHADOW;

				return cp;
			}
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);

			if (OSVersionHelper.WinMajorVersion >= 10 && _supportsAcrylic)
				WindowUtils.EnableAccent(this, _config.WindowAccent, Color.Transparent);
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);

			if (_isPreviewMode)
				label_Message.Text = "Press ESC to EXIT preview mode...";
			else
				timer_Input.Start();

			timer_Animation.Start();
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			if (!_isPreviewMode && e.Control)
			{
				timer_Input.Stop();
				DialogResult = DialogResult.OK;
			}

			if (_isPreviewMode && e.KeyCode == Keys.Escape)
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

		private void Panel_Top_Paint(object sender, PaintEventArgs e)
		{
			Point start = GetGradientStartPoint(_config.TopBar_GradientFlow, panel_Top.ClientRectangle.Size);
			Point end = GetGradientEndPoint(_config.TopBar_GradientFlow, panel_Top.ClientRectangle.Size);

			Color startColor = ColorTranslator.FromHtml(_config.TopBar_GradientStartColor);
			Color endColor = ColorTranslator.FromHtml(_config.TopBar_GradientEndColor);

			if (_supportsAcrylic)
			{
				startColor = Color.FromArgb(_config.TopBar_GradientStartAlpha, startColor.R, startColor.G, startColor.B);
				endColor = Color.FromArgb(_config.TopBar_GradientEndAlpha, endColor.R, endColor.G, endColor.B);
			}

			var brush = new LinearGradientBrush(start, end, startColor, endColor);
			e.Graphics.FillRectangle(brush, 0, 0, panel_Top.Width, panel_Top.Height);
		}

		private void Panel_Message_Paint(object sender, PaintEventArgs e)
		{
			Point start = GetGradientStartPoint(_config.BottomBar_GradientFlow, panel_Bottom.ClientRectangle.Size);
			Point end = GetGradientEndPoint(_config.BottomBar_GradientFlow, panel_Bottom.ClientRectangle.Size);

			Color startColor = ColorTranslator.FromHtml(_config.BottomBar_GradientStartColor);
			Color endColor = ColorTranslator.FromHtml(_config.BottomBar_GradientEndColor);

			if (_supportsAcrylic)
			{
				startColor = Color.FromArgb(_config.BottomBar_GradientStartAlpha, startColor.R, startColor.G, startColor.B);
				endColor = Color.FromArgb(_config.BottomBar_GradientEndAlpha, endColor.R, endColor.G, endColor.B);
			}

			var brush = new LinearGradientBrush(start, end, startColor, endColor);
			e.Graphics.FillRectangle(brush, 0, 0, panel_Bottom.Width, panel_Bottom.Height);
		}

		#endregion Event handlers

		#region Other methods

		private void ForceClickOnWindow()
		{
			var windowCenterPoint = new Point(Width / 2, Height / 2);
			InputHelper.ClickOnPoint(Handle, windowCenterPoint);
		}

		private Point GetGradientStartPoint(GradientFlow flow, Size rectSize)
		{
			switch (flow)
			{
				case GradientFlow.RightToLeft:
					return new Point(rectSize.Width, 0);

				case GradientFlow.BottomToTop:
					return new Point(0, rectSize.Height);

				default:
					return Point.Empty;
			}
		}

		private Point GetGradientEndPoint(GradientFlow flow, Size rectSize)
		{
			switch (flow)
			{
				case GradientFlow.LeftToRight:
					return new Point(rectSize.Width, 0);

				case GradientFlow.TopToBottom:
					return new Point(0, rectSize.Height);

				default:
					return Point.Empty;
			}
		}

		#endregion Other methods
	}
}
