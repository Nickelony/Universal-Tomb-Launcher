using System;
using System.Windows.Forms;
using UniversalTombLauncher.Utils;

namespace UniversalTombLauncher.Forms
{
	/// <summary>
	/// A form to manage extra settings like the TR4 fullscreen border fix.
	/// </summary>
	internal partial class FormExtraSettings : Form
	{
		/// <summary>
		/// The window style constant to remove the system menu (close button).
		/// </summary>
		private const int WS_SYSMENU = 0x80000;

		// Override the window creation parameters to remove the system menu (close button).
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.Style &= ~WS_SYSMENU;

				return cp;
			}
		}

		/// <summary>
		/// The original checked state of the border fix checkbox when the form is opened.
		/// </summary>
		private readonly bool _originalCheckedState;

		public FormExtraSettings()
		{
			InitializeComponent();

			checkBox_BorderFix.Checked = FullscreenBorderFix.IsBorderFixInstalled();
			_originalCheckedState = checkBox_BorderFix.Checked;
		}

		private void button_Next_Click(object sender, EventArgs e)
		{
			if (checkBox_BorderFix.Checked != _originalCheckedState)
			{
				if (checkBox_BorderFix.Checked)
					FullscreenBorderFix.InstallBorderFix();
				else
					FullscreenBorderFix.UninstallBorderFix();
			}

			DialogResult = DialogResult.OK;
		}
	}
}
