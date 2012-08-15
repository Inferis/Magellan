using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MapExplorer
{
	/// <summary>
	/// Summary description for wndTest.
	/// </summary>
	public class wndTest : System.Windows.Forms.Form
	{
		private EU2MapEditorControls.LayeredMapView layeredMapView1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public wndTest()
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
			this.layeredMapView1 = new EU2MapEditorControls.LayeredMapView();
			this.SuspendLayout();
			// 
			// layeredMapView1
			// 
			this.layeredMapView1.CoordinatesColor = System.Drawing.Color.DimGray;
			this.layeredMapView1.CoordinatesFont = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.layeredMapView1.CoordinatesOpacity = 0.9;
			this.layeredMapView1.GridColor = System.Drawing.Color.Orange;
			this.layeredMapView1.GridMode = EU2MapEditorControls.MapGridMode.Near;
			this.layeredMapView1.Location = new System.Drawing.Point(12, 8);
			this.layeredMapView1.MouseTrackerColor = System.Drawing.Color.Yellow;
			this.layeredMapView1.Name = "layeredMapView1";
			this.layeredMapView1.ScrollZoneColor = System.Drawing.SystemColors.ActiveCaption;
			this.layeredMapView1.Size = new System.Drawing.Size(556, 320);
			this.layeredMapView1.TabIndex = 0;
			// 
			// wndTest
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(580, 338);
			this.Controls.Add(this.layeredMapView1);
			this.Name = "wndTest";
			this.Text = "wndTest";
			this.Load += new System.EventHandler(this.wndTest_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void wndTest_Load(object sender, System.EventArgs e) {
		}

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
			Application.Run(new wndTest());
		}
		}
}
