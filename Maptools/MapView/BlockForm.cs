using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using EU2.Map;
using EU2.Data;
using EU2.Map.Codec;
using EU2.Map.Drawing;

namespace MapView
{
	/// <summary>
	/// Summary description for BlockForm.
	/// </summary>
	public class BlockForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ListBox lbData;
        private System.Windows.Forms.ListBox lbIDs;
        private System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.PictureBox pbVis;
		private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lblInfo;
        private Label lblFont;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public BlockForm()
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.pbVis = new System.Windows.Forms.PictureBox();
            this.lbData = new System.Windows.Forms.ListBox();
            this.lbIDs = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblSize = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblInfo = new System.Windows.Forms.Label();
            this.lblFont = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbVis)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.pbVis);
            this.panel1.Location = new System.Drawing.Point(222, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(644, 644);
            this.panel1.TabIndex = 1;
            // 
            // pbVis
            // 
            this.pbVis.BackColor = System.Drawing.SystemColors.Window;
            this.pbVis.Location = new System.Drawing.Point(0, 0);
            this.pbVis.Name = "pbVis";
            this.pbVis.Size = new System.Drawing.Size(640, 640);
            this.pbVis.TabIndex = 1;
            this.pbVis.TabStop = false;
            this.pbVis.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbVis_MouseMove);
            this.pbVis.Paint += new System.Windows.Forms.PaintEventHandler(this.pbVis_Paint);
            // 
            // lbData
            // 
            this.lbData.ColumnWidth = 32;
            this.lbData.IntegralHeight = false;
            this.lbData.Location = new System.Drawing.Point(4, 28);
            this.lbData.MultiColumn = true;
            this.lbData.Name = "lbData";
            this.lbData.Size = new System.Drawing.Size(212, 380);
            this.lbData.TabIndex = 2;
            // 
            // lbIDs
            // 
            this.lbIDs.IntegralHeight = false;
            this.lbIDs.Location = new System.Drawing.Point(4, 439);
            this.lbIDs.Name = "lbIDs";
            this.lbIDs.Size = new System.Drawing.Size(212, 103);
            this.lbIDs.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(114, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Original Data Stream";
            // 
            // lblSize
            // 
            this.lblSize.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblSize.Location = new System.Drawing.Point(144, 8);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(72, 16);
            this.lblSize.TabIndex = 6;
            this.lblSize.Text = "...";
            this.lblSize.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 423);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "IDList";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblInfo);
            this.groupBox1.Location = new System.Drawing.Point(8, 548);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(208, 100);
            this.groupBox1.TabIndex = 23;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Info";
            // 
            // lblInfo
            // 
            this.lblInfo.Font = new System.Drawing.Font("Tahoma", 7F);
            this.lblInfo.Location = new System.Drawing.Point(6, 18);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(192, 77);
            this.lblInfo.TabIndex = 0;
            this.lblInfo.Text = "Information";
            // 
            // lblFont
            // 
            this.lblFont.Font = new System.Drawing.Font("Tahoma", 6F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFont.Location = new System.Drawing.Point(110, 2);
            this.lblFont.Name = "lblFont";
            this.lblFont.Size = new System.Drawing.Size(28, 10);
            this.lblFont.TabIndex = 13;
            this.lblFont.Text = "label2";
            this.lblFont.Visible = false;
            // 
            // BlockForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 15);
            this.ClientSize = new System.Drawing.Size(870, 653);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.lblFont);
            this.Controls.Add(this.lblSize);
            this.Controls.Add(this.lbIDs);
            this.Controls.Add(this.lbData);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BlockForm";
            this.Text = "BlockForm";
            this.Load += new System.EventHandler(this.BlockForm_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbVis)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private int index;
		ushort[] rawid;
		Point loc;

        private EU2.Edit.IFile source;
        private int lightmap;

        public int Lightmap {
            get { return lightmap; }
            set {
                lightmap = value;
            }
        }

        public EU2.Edit.IFile Source {
            get { return source; }
            set { source = value; }
        }

        public Point BlockLocation {
			get { return loc; }
			set { 
				loc = value; 
			}
		}

		public int BlockIndex {
			get { return index; }
			set { 
				index = value; 
			}
		}

        private Lightmap CurrentLightmap {
            get {
                switch (lightmap) {
                    case 1:
                        return source.Lightmap1;
                    case 2:
                        return source.Lightmap2;
                    case 3:
                        return source.Lightmap3;
                }
                throw new ArgumentOutOfRangeException("lightmap");
            }
        }

		private void Fill() {
			this.Text = "Block Visualisation [" + index.ToString() + "]";

			lbData.Items.Clear();
			try {
				CompressedBlock block = CurrentLightmap.GetCompressedBlock(index);

                lbData.MultiColumn = true;
				lbData.Enabled = true;
				lblSize.Enabled = true;

				byte[] data = block.Data;
				lblSize.Text = data.Length.ToString();
				for ( int i=0; i<data.Length; ++i ) {
					lbData.Items.Add( data[i] );
				}

				rawid = MapBlock.GetRawIDTable( block );
				for ( int i=0; i<rawid.Length; ++i ) {
					lbIDs.Items.Add( rawid[i] );
				}
			}
			catch ( NotSupportedException ) {
				lblSize.Enabled = false;
				lblSize.Text = "n/a";
				lbData.Items.Add( "Not Compressed" );
				lbData.MultiColumn = false;
				lbData.Enabled = false;
			}

            MapBlock mblock = CurrentLightmap.GetDecompressedBlock(index);

			Bitmap image = new Bitmap( pbVis.Width, pbVis.Height );
			paintgraphics = Graphics.FromImage( image );
			mblock.WalkTree( new DelegateWalker.VisitDelegate( PaintWalker ) );
			paintgraphics.Dispose();
			paintgraphics = null;

			pbVis.Image = image;

			/*
			ushort[] ids = block.ID;
			lblSize.Text = ids.Length.ToString();
			for ( int i=0; i<ids.Length; ++i ) {
				lbIDs.Items.Add( ids[i] );
			}
			*/
		}

		private void textBox1_TextChanged(object sender, System.EventArgs e) {
		
		}

		Graphics paintgraphics;

		private void pbVis_Paint(object sender, System.Windows.Forms.PaintEventArgs e) {

			/*
			for ( int y=0; y<640; y += 20 ) {
				e.Graphics.DrawLine( SystemPens.Control, 0, y, pbVis.Width, y );
			}
			for ( int x=0; x<640; x += 20 ) {
				e.Graphics.DrawLine( SystemPens.Control, x, 0, x, pbVis.Height );
			}
			*/
		}

		private void PaintWalker( Node node, int x, int y ) {
			Pen pen;
			//paintgraphics.DrawRectangle( pen, x*20 + (5-node.Level), y*20 + (5-node.Level), 20*(1<<node.Level)-(5-node.Level)*2, 20*(1<<node.Level)-(5-node.Level)*2 );
			if ( node.IsLeaf() ) {
				PaintNode( node, x, y );
				pen = Pens.Red;
			}
			else {
				pen = Pens.Blue;
				PaintNode( node.BottomRightChild, x+1, y+1 );
				PaintNode( node.BottomLeftChild, x, y+1 );
				PaintNode( node.TopRightChild, x+1, y );
				PaintNode( node.TopLeftChild, x, y );
			}
			paintgraphics.DrawRectangle( pen, x*20 + 1, y*20 + 1, 20*(1<<node.Level)-2, 20*(1<<node.Level)-2 );
		}

		private void PaintNode(  Node node, int x, int y ) {
			int v = 0xFF - (node.Data.Color << 2); 
			SolidBrush brush;
			if ( node.Data.IsBorder() ) 
				brush = new SolidBrush( Color.FromArgb( 0xFF, v, v ) );
			else
				brush = new SolidBrush( Color.FromArgb( v, v, v ) );
			paintgraphics.FillRectangle( brush, x*20 + 1, y*20 + 1, 20*(1<<node.Level)-2, 20*(1<<node.Level)-2 );
			brush.Dispose();
			paintgraphics.DrawString( node.Data.ID.ToString(), lblFont.Font, node.Data.Color > 32 ? Brushes.White : Brushes.Black, x*20 + 2, y*20 + 2 );
			//if ( chkDisplayColors.Checked ) 
				paintgraphics.DrawString( node.Data.Color.ToString(), lblFont.Font, node.Data.Color > 32 ? Brushes.Yellow : Brushes.Blue, x*20 + 2, y*20 + 10 );
			//else
			//	paintgraphics.DrawString( node.Data.Full.ToString(), lblFont.Font, node.Data.Color > 32 ? Brushes.White : Brushes.Black, x*20 + 2, y*20 + 10 );
		}

		private void BlockForm_Load(object sender, System.EventArgs e) {
            Fill();
		}

		private void pbVis_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e) {
			int x = loc.X + e.X/20;
			int y = loc.Y + e.Y/20;

			string s;
            ushort provid = source.IDMap[x, y];
			Province prov = source.Provinces[provid];
            ushort neighbourid = source.IDMap.FindClosestLand(x, y, provid, source.Provinces);

			s = new Point( x, y ).ToString() + "\n" + 
				"Province: " + provid + " = " + prov.Name + " " + (prov.IsLand() ? "land" : prov.IsRiver() ? "river" : "ocean" ) + "\n";

			ushort riverid = ushort.MaxValue;
			if ( prov.IsRiver() ) {
				riverid = provid;
                provid = source.IDMap.FindClosestLand(x, y, provid, source.Provinces);
				s += "IsRiver -> " + provid + "\n";
			}
            neighbourid = source.IDMap.FindClosestLand(x, y, provid, source.Provinces);
			s += "Neighbour of " + provid + ": " + neighbourid + "\n";

            if (provid != neighbourid && provid != riverid && source.Provinces[neighbourid].IsLand()) {
				s += "Special case!\n";

				int riveradj = Adjacent.Invalid;
				if ( riverid < ushort.MaxValue ) {
                    riveradj = source.AdjacencyTable.GetAdjacencyIndex(provid, riverid);
				}
				s += "River: " + riverid + " - adj = " + riveradj + "\n";

                int neighadj = source.AdjacencyTable.GetAdjacencyIndex(provid, neighbourid);
				s += "Neighbouradj: " + neighbourid + " - adj = " + neighadj + "\n";

				s += "= " + provid + " " + riveradj + " " + neighadj + " B\n";
			}
				
			lblInfo.Text = s;
		}

		private void btnCompare_Click(object sender, System.EventArgs e) {
            //BlockCompareForm form = new BlockCompareForm();
            //form.BlockLocation = loc;
            //form.ShowDialog( this );
		}

		private void chkDisplayColors_CheckedChanged(object sender, System.EventArgs e) {
			Fill();
		}
	}
}
