using System.Windows.Forms;
using UniversalTombLauncher.Forms.Bases;
using UniversalTombLauncher.Utils;

namespace UniversalTombLauncher.Forms
{
	public partial class FormExtraSettings : FormNoSystemMenu
	{
		private bool _originalCheckedState;

		public FormExtraSettings()
		{
			InitializeComponent();

			checkBox_BorderFix.Checked = FullscreenBorderFix.IsBorderFixInstalled();
			_originalCheckedState = checkBox_BorderFix.Checked;
		}

		private void button_Next_Click(object sender, System.EventArgs e)
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
