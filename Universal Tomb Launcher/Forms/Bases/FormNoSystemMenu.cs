using System.Windows.Forms;

namespace UniversalTombLauncher.Forms.Bases
{
	public class FormNoSystemMenu : Form
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
	}
}
