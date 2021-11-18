using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using UniversalTombLauncher.Helpers;
using UniversalTombLauncher.Utils;

namespace UniversalTombLauncher.Forms.Bases
{
	public class FormAcrylic : FormNoSystemMenu
	{
		private const int CS_DROPSHADOW = 0x20000;
		private const string PersonalizationRegistryKey = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";

		[DefaultValue(false)]
		public bool IsUsingLightTheme { get; set; }

		[DefaultValue(false)]
		public bool SupportsAcrylic { get; set; }

		protected override CreateParams CreateParams
		{
			get
			{
				UpdatePersonalizationProperties();

				CreateParams cp = base.CreateParams;

				if (!SupportsAcrylic)
					cp.ClassStyle |= CS_DROPSHADOW;

				return cp;
			}
		}

		protected override void OnHandleCreated(EventArgs e)
		{
			if (OSVersionHelper.WinMajorVersion >= 10 && SupportsAcrylic)
				EnableAcrylic();

			base.OnHandleCreated(e);
		}

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			if (SupportsAcrylic)
				e.Graphics.Clear(Color.Transparent);
			else
				base.OnPaintBackground(e);
		}

		public void UpdatePersonalizationProperties()
		{
			using (RegistryKey key = Registry.CurrentUser.OpenSubKey(PersonalizationRegistryKey))
			{
				if (key != null)
				{
					object themeKey = key.GetValue("AppsUseLightTheme");
					object transparencyKey = key.GetValue("EnableTransparency");

					if (themeKey != null && int.TryParse(themeKey.ToString(), out int themeKeyValue))
						IsUsingLightTheme = themeKeyValue > 0;

					if (transparencyKey != null && int.TryParse(transparencyKey.ToString(), out int transparencyKeyValue))
						SupportsAcrylic = transparencyKeyValue > 0;
				}
			}
		}

		private void EnableAcrylic()
		{
			Color blurColor = IsUsingLightTheme ? Color.White : Color.Black;
			WindowUtils.EnableAcrylic(this, Color.FromArgb(192, blurColor));
		}
	}
}
