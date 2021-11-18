namespace UniversalTombLauncher.Forms
{
	partial class FormExtraSettings
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
			this.checkBox_BorderFix = new System.Windows.Forms.CheckBox();
			this.button_Next = new System.Windows.Forms.Button();
			this.label_Info = new System.Windows.Forms.Label();
			this.button_Cancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// checkBox_BorderFix
			// 
			this.checkBox_BorderFix.AutoSize = true;
			this.checkBox_BorderFix.Location = new System.Drawing.Point(12, 12);
			this.checkBox_BorderFix.Name = "checkBox_BorderFix";
			this.checkBox_BorderFix.Size = new System.Drawing.Size(167, 17);
			this.checkBox_BorderFix.TabIndex = 0;
			this.checkBox_BorderFix.Text = "Enable fullscreen border fix";
			this.checkBox_BorderFix.UseVisualStyleBackColor = true;
			// 
			// button_Next
			// 
			this.button_Next.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.button_Next.Location = new System.Drawing.Point(146, 101);
			this.button_Next.Name = "button_Next";
			this.button_Next.Size = new System.Drawing.Size(75, 23);
			this.button_Next.TabIndex = 1;
			this.button_Next.Text = "Next";
			this.button_Next.UseVisualStyleBackColor = true;
			this.button_Next.Click += new System.EventHandler(this.button_Next_Click);
			// 
			// label_Info
			// 
			this.label_Info.AutoSize = true;
			this.label_Info.ForeColor = System.Drawing.SystemColors.ControlDark;
			this.label_Info.Location = new System.Drawing.Point(41, 32);
			this.label_Info.Margin = new System.Windows.Forms.Padding(32, 0, 3, 0);
			this.label_Info.Name = "label_Info";
			this.label_Info.Size = new System.Drawing.Size(260, 52);
			this.label_Info.TabIndex = 2;
			this.label_Info.Text = "Fixes an issue where the window borders are still\r\nvisible when in fullscreen mod" +
    "e.\r\nEnable this only if you\'re actually experiencing\r\nthis issue.";
			// 
			// button_Cancel
			// 
			this.button_Cancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.button_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button_Cancel.Location = new System.Drawing.Point(227, 101);
			this.button_Cancel.Name = "button_Cancel";
			this.button_Cancel.Size = new System.Drawing.Size(75, 23);
			this.button_Cancel.TabIndex = 3;
			this.button_Cancel.Text = "Cancel";
			this.button_Cancel.UseVisualStyleBackColor = true;
			// 
			// FormExtraSettings
			// 
			this.AcceptButton = this.button_Next;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.button_Cancel;
			this.ClientSize = new System.Drawing.Size(314, 136);
			this.Controls.Add(this.button_Cancel);
			this.Controls.Add(this.label_Info);
			this.Controls.Add(this.checkBox_BorderFix);
			this.Controls.Add(this.button_Next);
			this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Name = "FormExtraSettings";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Setup";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckBox checkBox_BorderFix;
		private System.Windows.Forms.Button button_Next;
		private System.Windows.Forms.Label label_Info;
		private System.Windows.Forms.Button button_Cancel;
	}
}