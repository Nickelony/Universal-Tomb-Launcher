namespace UniversalTombLauncher
{
	partial class FormSetupPrompt
	{
		private System.ComponentModel.IContainer components = null;

		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.label = new System.Windows.Forms.Label();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// label
			// 
			this.label.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label.Dock = System.Windows.Forms.DockStyle.Fill;
			this.label.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label.Location = new System.Drawing.Point(0, 0);
			this.label.Name = "label";
			this.label.Size = new System.Drawing.Size(280, 60);
			this.label.TabIndex = 0;
			this.label.Text = "Press CTRL key for SETUP";
			this.label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// timer
			// 
			this.timer.Interval = 1000;
			this.timer.Tick += new System.EventHandler(this.Timer_Tick);
			// 
			// FormSetupPrompt
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(280, 60);
			this.Controls.Add(this.label);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormSetupPrompt";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.TopMost = true;
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormSetupPrompt_KeyDown);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label;
		private System.Windows.Forms.Timer timer;
	}
}