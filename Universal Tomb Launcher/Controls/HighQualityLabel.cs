using System.Drawing;
using System.Windows.Forms;

namespace UniversalTombLauncher.Controls
{
	public class HighQualityLabel : Label
	{
		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
			e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

			var format = new StringFormat();

			switch (TextAlign)
			{
				case ContentAlignment.TopCenter:
					format.LineAlignment = StringAlignment.Near;
					format.Alignment = StringAlignment.Center;
					break;

				case ContentAlignment.MiddleCenter:
					format.LineAlignment = StringAlignment.Center;
					format.Alignment = StringAlignment.Center;
					break;
			}

			e.Graphics.DrawString(Text, Font, new SolidBrush(ForeColor), ClientRectangle, format);
		}
	}
}
