using System;
using System.Windows.Forms;
using UniversalTombLauncher.Utils;

namespace UniversalTombLauncher.Forms
{
	internal partial class FormExtraSettings : Form
	{
		private const int WS_SYSMENU = 0x80000;

		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.Style &= ~WS_SYSMENU;

				return cp;
			}
		}

		private bool _originalCheckedState;

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
