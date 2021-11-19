namespace UniversalTombLauncher.Forms
{
	partial class FormSetupSplash
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
			this.panel_SplashImage = new System.Windows.Forms.Panel();
			this.timer_Animation = new System.Windows.Forms.Timer(this.components);
			this.timer_Input = new System.Windows.Forms.Timer(this.components);
			this.label_Message = new UniversalTombLauncher.Controls.HighQualityLabel();
			this.panel_Top = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// panel_SplashImage
			// 
			this.panel_SplashImage.BackColor = System.Drawing.Color.Black;
			this.panel_SplashImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
			this.panel_SplashImage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel_SplashImage.Location = new System.Drawing.Point(0, 26);
			this.panel_SplashImage.Name = "panel_SplashImage";
			this.panel_SplashImage.Size = new System.Drawing.Size(384, 230);
			this.panel_SplashImage.TabIndex = 1;
			// 
			// timer_Animation
			// 
			this.timer_Animation.Interval = 1;
			this.timer_Animation.Tick += new System.EventHandler(this.Timer_Animation_Tick);
			// 
			// timer_Input
			// 
			this.timer_Input.Interval = 1500;
			this.timer_Input.Tick += new System.EventHandler(this.Timer_Input_Tick);
			// 
			// label_Message
			// 
			this.label_Message.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.label_Message.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
			this.label_Message.ForeColor = System.Drawing.Color.White;
			this.label_Message.Location = new System.Drawing.Point(0, 256);
			this.label_Message.Name = "label_Message";
			this.label_Message.Size = new System.Drawing.Size(384, 75);
			this.label_Message.TabIndex = 0;
			this.label_Message.Text = "Press CTRL to show the SETUP dialog...";
			this.label_Message.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// panel_Top
			// 
			this.panel_Top.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel_Top.Location = new System.Drawing.Point(0, 0);
			this.panel_Top.Name = "panel_Top";
			this.panel_Top.Size = new System.Drawing.Size(384, 26);
			this.panel_Top.TabIndex = 2;
			// 
			// FormSetupSplash
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.ClientSize = new System.Drawing.Size(384, 331);
			this.Controls.Add(this.panel_SplashImage);
			this.Controls.Add(this.label_Message);
			this.Controls.Add(this.panel_Top);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormSetupSplash";
			this.Opacity = 0D;
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.TopMost = true;
			this.ResumeLayout(false);

		}

		#endregion

		private Controls.HighQualityLabel label_Message;
		private System.Windows.Forms.Panel panel_SplashImage;
		private System.Windows.Forms.Panel panel_Top;
		private System.Windows.Forms.Timer timer_Animation;
		private System.Windows.Forms.Timer timer_Input;
	}
}