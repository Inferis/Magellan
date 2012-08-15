using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MapView
{
	/// <summary>
	/// Summary description for BlockVisForm.
	/// </summary>
	public class BlockVisForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel pnlResult;
		private System.Windows.Forms.PictureBox pbVis;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public BlockVisForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.pnlResult = new System.Windows.Forms.Panel();
			this.pbVis = new System.Windows.Forms.PictureBox();
			this.pnlResult.SuspendLayout();
			this.SuspendLayout();
			// 
			// pnlResult
			// 
			this.pnlResult.AutoScroll = true;
			this.pnlResult.BackColor = System.Drawing.SystemColors.AppWorkspace;
			this.pnlResult.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pnlResult.Controls.Add(this.pbVis);
			this.pnlResult.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlResult.Location = new System.Drawing.Point(0, 0);
			this.pnlResult.Name = "pnlResult";
			this.pnlResult.Size = new System.Drawing.Size(472, 347);
			this.pnlResult.TabIndex = 0;
			// 
			// pbVis
			// 
			this.pbVis.BackColor = System.Drawing.Color.Beige;
			this.pbVis.Location = new System.Drawing.Point(0, 0);
			this.pbVis.Name = "pbVis";
			this.pbVis.Size = new System.Drawing.Size(148, 104);
			this.pbVis.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pbVis.TabIndex = 0;
			this.pbVis.TabStop = false;
			// 
			// BlockVisForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(472, 347);
			this.Controls.Add(this.pnlResult);
			this.Name = "BlockVisForm";
			this.Text = "BlockVisForm";
			this.Load += new System.EventHandler(this.BlockVisForm_Load);
			this.pnlResult.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		public Image Visuals {
			get { return pbVis.Image; }
			set { pbVis.Image = value; }
		}

		private void BlockVisForm_Load(object sender, System.EventArgs e) {
		
		}
	}
}
