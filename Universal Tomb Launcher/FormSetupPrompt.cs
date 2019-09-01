using System;
using System.Windows.Forms;

namespace UniversalTombLauncher
{
	public partial class FormSetupPrompt : Form
	{
		public FormSetupPrompt()
		{
			InitializeComponent();
		}

		protected override void OnShown(EventArgs e)
		{
			base.OnShown(e);
			timer.Start();
		}

		private void FormSetupPrompt_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Control)
			{
				timer.Stop();
				DialogResult = DialogResult.OK;
			}
		}

		private void Timer_Tick(object sender, EventArgs e)
		{
			timer.Stop();
			DialogResult = DialogResult.Cancel;
		}
	}
}
