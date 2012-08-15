using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using MapToolsLib;

namespace MapColor
{
	/// <summary>
	/// Summary description for ColorForm.
	/// </summary>
	public class ColorForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox cbTopMost;
		private System.Windows.Forms.NumericUpDown nudRed;
		private System.Windows.Forms.NumericUpDown nudGreen;
		private System.Windows.Forms.NumericUpDown nudBlue;
		private System.Windows.Forms.GroupBox gbID;
		private System.Windows.Forms.GroupBox gbRGB;
		private System.Windows.Forms.ComboBox ddlConvertor;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.PictureBox pbID;
		private System.Windows.Forms.Label lblError;
		private System.Windows.Forms.TrackBar trbBlue;
		private System.Windows.Forms.TrackBar trbRed;
		private System.Windows.Forms.TrackBar trbGreen;
		private System.Windows.Forms.TextBox tbID;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox tbRGB;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ColorForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			ddlConvertor.Items.Add( new Inferis1IDConvertor() );
			ddlConvertor.Items.Add( new Inferis2IDConvertor() );
			ddlConvertor.Items.Add( new NathanSynIDConvertor() );
			ddlConvertor.SelectedIndex = 1;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ColorForm));
			this.gbID = new System.Windows.Forms.GroupBox();
			this.lblError = new System.Windows.Forms.Label();
			this.tbID = new System.Windows.Forms.TextBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.pbID = new System.Windows.Forms.PictureBox();
			this.gbRGB = new System.Windows.Forms.GroupBox();
			this.label5 = new System.Windows.Forms.Label();
			this.tbRGB = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.trbGreen = new System.Windows.Forms.TrackBar();
			this.trbRed = new System.Windows.Forms.TrackBar();
			this.trbBlue = new System.Windows.Forms.TrackBar();
			this.nudBlue = new System.Windows.Forms.NumericUpDown();
			this.nudGreen = new System.Windows.Forms.NumericUpDown();
			this.nudRed = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.cbTopMost = new System.Windows.Forms.CheckBox();
			this.ddlConvertor = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.gbID.SuspendLayout();
			this.panel1.SuspendLayout();
			this.gbRGB.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trbGreen)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trbRed)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trbBlue)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudBlue)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudGreen)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.nudRed)).BeginInit();
			this.SuspendLayout();
			// 
			// gbID
			// 
			this.gbID.Controls.Add(this.lblError);
			this.gbID.Controls.Add(this.tbID);
			this.gbID.Controls.Add(this.panel1);
			this.gbID.Location = new System.Drawing.Point(8, 8);
			this.gbID.Name = "gbID";
			this.gbID.Size = new System.Drawing.Size(108, 184);
			this.gbID.TabIndex = 0;
			this.gbID.TabStop = false;
			this.gbID.Text = "ID";
			// 
			// lblError
			// 
			this.lblError.AutoSize = true;
			this.lblError.BackColor = System.Drawing.SystemColors.Control;
			this.lblError.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.lblError.ForeColor = System.Drawing.Color.Red;
			this.lblError.Location = new System.Drawing.Point(8, 44);
			this.lblError.Name = "lblError";
			this.lblError.Size = new System.Drawing.Size(16, 17);
			this.lblError.TabIndex = 3;
			this.lblError.Text = "!!!";
			this.lblError.Visible = false;
			// 
			// tbID
			// 
			this.tbID.Location = new System.Drawing.Point(8, 20);
			this.tbID.Name = "tbID";
			this.tbID.Size = new System.Drawing.Size(92, 21);
			this.tbID.TabIndex = 2;
			this.tbID.Text = "0";
			this.tbID.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbID_KeyDown);
			this.tbID.TextChanged += new System.EventHandler(this.tbID_TextChanged);
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.Window;
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel1.Controls.Add(this.pbID);
			this.panel1.DockPadding.All = 2;
			this.panel1.Location = new System.Drawing.Point(8, 72);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(92, 80);
			this.panel1.TabIndex = 1;
			// 
			// pbID
			// 
			this.pbID.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pbID.Location = new System.Drawing.Point(2, 2);
			this.pbID.Name = "pbID";
			this.pbID.Size = new System.Drawing.Size(84, 72);
			this.pbID.TabIndex = 0;
			this.pbID.TabStop = false;
			this.pbID.Paint += new System.Windows.Forms.PaintEventHandler(this.pbID_Paint);
			// 
			// gbRGB
			// 
			this.gbRGB.Controls.Add(this.label5);
			this.gbRGB.Controls.Add(this.tbRGB);
			this.gbRGB.Controls.Add(this.label1);
			this.gbRGB.Controls.Add(this.trbGreen);
			this.gbRGB.Controls.Add(this.trbRed);
			this.gbRGB.Controls.Add(this.trbBlue);
			this.gbRGB.Controls.Add(this.nudBlue);
			this.gbRGB.Controls.Add(this.nudGreen);
			this.gbRGB.Controls.Add(this.nudRed);
			this.gbRGB.Controls.Add(this.label3);
			this.gbRGB.Controls.Add(this.label2);
			this.gbRGB.Location = new System.Drawing.Point(152, 8);
			this.gbRGB.Name = "gbRGB";
			this.gbRGB.Size = new System.Drawing.Size(264, 188);
			this.gbRGB.TabIndex = 3;
			this.gbRGB.TabStop = false;
			this.gbRGB.Text = "RGB";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label5.Location = new System.Drawing.Point(20, 24);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(29, 17);
			this.label5.TabIndex = 14;
			this.label5.Text = "RGB";
			// 
			// tbRGB
			// 
			this.tbRGB.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
			this.tbRGB.Location = new System.Drawing.Point(56, 20);
			this.tbRGB.Name = "tbRGB";
			this.tbRGB.Size = new System.Drawing.Size(44, 21);
			this.tbRGB.TabIndex = 13;
			this.tbRGB.Text = "000000";
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.ForeColor = System.Drawing.Color.Red;
			this.label1.Location = new System.Drawing.Point(8, 56);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(40, 12);
			this.label1.TabIndex = 6;
			this.label1.Text = "red";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// trbGreen
			// 
			this.trbGreen.LargeChange = 51;
			this.trbGreen.Location = new System.Drawing.Point(48, 96);
			this.trbGreen.Maximum = 255;
			this.trbGreen.Name = "trbGreen";
			this.trbGreen.Size = new System.Drawing.Size(160, 45);
			this.trbGreen.TabIndex = 9;
			this.trbGreen.TickFrequency = 51;
			this.trbGreen.ValueChanged += new System.EventHandler(this.trbRGB_ValueChanged);
			// 
			// trbRed
			// 
			this.trbRed.LargeChange = 51;
			this.trbRed.Location = new System.Drawing.Point(48, 52);
			this.trbRed.Maximum = 255;
			this.trbRed.Name = "trbRed";
			this.trbRed.Size = new System.Drawing.Size(160, 45);
			this.trbRed.TabIndex = 8;
			this.trbRed.TickFrequency = 51;
			this.trbRed.ValueChanged += new System.EventHandler(this.trbRGB_ValueChanged);
			// 
			// trbBlue
			// 
			this.trbBlue.LargeChange = 51;
			this.trbBlue.Location = new System.Drawing.Point(48, 140);
			this.trbBlue.Maximum = 255;
			this.trbBlue.Name = "trbBlue";
			this.trbBlue.Size = new System.Drawing.Size(160, 45);
			this.trbBlue.TabIndex = 7;
			this.trbBlue.TickFrequency = 51;
			this.trbBlue.ValueChanged += new System.EventHandler(this.trbRGB_ValueChanged);
			// 
			// nudBlue
			// 
			this.nudBlue.Hexadecimal = true;
			this.nudBlue.Location = new System.Drawing.Point(208, 144);
			this.nudBlue.Maximum = new System.Decimal(new int[] {
																	255,
																	0,
																	0,
																	0});
			this.nudBlue.Name = "nudBlue";
			this.nudBlue.Size = new System.Drawing.Size(48, 21);
			this.nudBlue.TabIndex = 2;
			this.nudBlue.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.nudBlue.ValueChanged += new System.EventHandler(this.nudRGB_ValueChanged);
			// 
			// nudGreen
			// 
			this.nudGreen.Hexadecimal = true;
			this.nudGreen.Location = new System.Drawing.Point(208, 100);
			this.nudGreen.Maximum = new System.Decimal(new int[] {
																	 255,
																	 0,
																	 0,
																	 0});
			this.nudGreen.Name = "nudGreen";
			this.nudGreen.Size = new System.Drawing.Size(48, 21);
			this.nudGreen.TabIndex = 1;
			this.nudGreen.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.nudGreen.ValueChanged += new System.EventHandler(this.nudRGB_ValueChanged);
			// 
			// nudRed
			// 
			this.nudRed.Hexadecimal = true;
			this.nudRed.Location = new System.Drawing.Point(208, 56);
			this.nudRed.Maximum = new System.Decimal(new int[] {
																   255,
																   0,
																   0,
																   0});
			this.nudRed.Name = "nudRed";
			this.nudRed.Size = new System.Drawing.Size(48, 21);
			this.nudRed.TabIndex = 0;
			this.nudRed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.nudRed.ValueChanged += new System.EventHandler(this.nudRGB_ValueChanged);
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label3.ForeColor = System.Drawing.Color.Blue;
			this.label3.Location = new System.Drawing.Point(8, 144);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(40, 12);
			this.label3.TabIndex = 5;
			this.label3.Text = "blue";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label2.ForeColor = System.Drawing.Color.ForestGreen;
			this.label2.Location = new System.Drawing.Point(4, 100);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(44, 16);
			this.label2.TabIndex = 4;
			this.label2.Text = "green";
			this.label2.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// cbTopMost
			// 
			this.cbTopMost.Location = new System.Drawing.Point(320, 204);
			this.cbTopMost.Name = "cbTopMost";
			this.cbTopMost.Size = new System.Drawing.Size(100, 24);
			this.cbTopMost.TabIndex = 4;
			this.cbTopMost.Text = "Always On Top";
			this.cbTopMost.CheckedChanged += new System.EventHandler(this.cbTopMost_CheckedChanged);
			// 
			// ddlConvertor
			// 
			this.ddlConvertor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.ddlConvertor.Location = new System.Drawing.Point(88, 204);
			this.ddlConvertor.Name = "ddlConvertor";
			this.ddlConvertor.Size = new System.Drawing.Size(208, 21);
			this.ddlConvertor.TabIndex = 5;
			this.ddlConvertor.SelectedIndexChanged += new System.EventHandler(this.ddlConvertor_SelectedIndexChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(8, 208);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(73, 17);
			this.label4.TabIndex = 6;
			this.label4.Text = "ID Convertor:";
			// 
			// ColorForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.BackColor = System.Drawing.SystemColors.Control;
			this.ClientSize = new System.Drawing.Size(422, 229);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.ddlConvertor);
			this.Controls.Add(this.cbTopMost);
			this.Controls.Add(this.gbRGB);
			this.Controls.Add(this.gbID);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "ColorForm";
			this.Text = "ColorForm";
			this.gbID.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this.gbRGB.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.trbGreen)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trbRed)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trbBlue)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudBlue)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudGreen)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.nudRed)).EndInit();
			this.ResumeLayout(false);

		}
		#endregion

		private void cbTopMost_CheckedChanged(object sender, System.EventArgs e) {
			TopMost = cbTopMost.Checked;
		}

		private void nudRGB_ValueChanged(object sender, System.EventArgs e) {
			if ( cancelEvents ) return;
		
			cancelEvents = true;
			RGB2ID();
			cancelEvents = false;
		}

		private void trbRGB_ValueChanged(object sender, System.EventArgs e) {
			if ( cancelEvents ) return;
		
			cancelEvents = true;

			nudRed.Value = trbRed.Value;
			nudGreen.Value = trbGreen.Value;
			nudBlue.Value = trbBlue.Value;
			RGB2ID();

			cancelEvents = false;
		}

		private void tbID_TextChanged(object sender, System.EventArgs e) {
			if ( cancelEvents ) return;
		
			cancelEvents = true;
			ID2RGB();
			cancelEvents = false;
		}

		private void tbID_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if ( cancelEvents ) return;

			ushort id = ushort.MaxValue;
			try {
				id = ushort.Parse( tbID.Text );
			}
			catch {
				return;
			}

			if ( e.KeyCode == Keys.Down && id <= EU2.Data.Province.MaxValue ) 
				tbID.Text = (id+1).ToString();
			else if ( e.KeyCode == Keys.Up && id > 0 ) 
				tbID.Text = (id-1).ToString();
		}

		private void RGB2ID() {
			if ( ddlConvertor.SelectedIndex < 0 ) return;

			UpdateRGB();

			int rgb = ((int)nudRed.Value << 16) | ((int)nudGreen.Value << 8) | (int)nudBlue.Value;
			rgb = ((IIDConvertor)ddlConvertor.Items[ddlConvertor.SelectedIndex]).ConvertRGB( rgb );

			tbID.Text = rgb.ToString();
			pbID.Refresh();
			lblError.Text = "Invalid ID!";
			lblError.Visible = rgb < 0 || rgb > EU2.Data.Province.MaxValue;
		}

		private void ID2RGB() {
			if ( ddlConvertor.SelectedIndex < 0 ) return;

			ushort id = ushort.MaxValue;
			try {
				id = ushort.Parse( tbID.Text );
			}
			catch {
			}
			lblError.Text = "Invalid ID!";
			lblError.Visible = id < 0 || id > EU2.Data.Province.MaxValue;
			pbID.Refresh();

			int rgb = ((IIDConvertor)ddlConvertor.Items[ddlConvertor.SelectedIndex]).ConvertID( id );
			nudRed.Value = (rgb >> 16) & 0xFF;
			nudGreen.Value = (rgb >> 8) & 0xFF;
			nudBlue.Value = rgb & 0xFF;

			UpdateRGB();
		}

		private void UpdateRGB() {
			trbRed.Value = (int)nudRed.Value;
			trbGreen.Value = (int)nudGreen.Value;
			trbBlue.Value = (int)nudBlue.Value;

			int rgb = ((int)nudRed.Value << 16) | ((int)nudGreen.Value << 8) | (int)nudBlue.Value;
			tbRGB.Text = rgb.ToString( "X6" );
		}

		private void pbID_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {
			if ( lblError.Visible ) return;
			if ( ddlConvertor.SelectedIndex < 0 ) return;

			IIDConvertor conv = (IIDConvertor)ddlConvertor.Items[ddlConvertor.SelectedIndex];
			ushort id = ushort.Parse(tbID.Text);
			int c = conv.ConvertID( id  );

			Rectangle bounds = new Rectangle( 0, 0, pbID.Width, pbID.Height );

			using ( Brush b = new SolidBrush( Color.FromArgb( c ) ) ) {
				e.Graphics.FillRectangle( b, bounds );
			}

			bounds.Inflate( -1, -1 );
			c ^= 0xFFFFFF;
			using ( Brush b = new SolidBrush( Color.FromArgb( c ) ) ) {
				e.Graphics.DrawString( id.ToString(), Font, b, bounds );
			}
		}

		private void ddlConvertor_SelectedIndexChanged(object sender, System.EventArgs e) {
			pbID.Refresh();
			ID2RGB();
		}

		private bool cancelEvents;
	}
}
