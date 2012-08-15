using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using EU2MapEditorControls;
using LayerPainter;

namespace MapExplorer
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MapForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox tbProvID;
		private System.Windows.Forms.Label lblGoto;
		private EU2MapEditorControls.MapView mapView1;
		private System.Windows.Forms.PictureBox pbMini;
		private System.Windows.Forms.CheckBox cbBorders;
		private System.Windows.Forms.CheckBox cbGrid;
		private System.Windows.Forms.ContextMenu cmMap;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.TabPage tbpShading;
		private System.Windows.Forms.TabPage tbpIDL;
		private System.Windows.Forms.TabPage tbpIDF;
		private System.Windows.Forms.TabControl tcModes;
		private System.Windows.Forms.TabPage tbpIDDiff;
		private System.Windows.Forms.MenuItem miVisBlock;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.StatusBar sbStatus;
		private System.Windows.Forms.StatusBarPanel sbpTracker;
		private System.Windows.Forms.Panel panel6;
		private System.Windows.Forms.Panel panel7;
		private int cmblockindex = -1;

		public MapForm( )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			Source = null;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MapForm));
			this.cbBorders = new System.Windows.Forms.CheckBox();
			this.cbGrid = new System.Windows.Forms.CheckBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.pbMini = new System.Windows.Forms.PictureBox();
			this.sbStatus = new System.Windows.Forms.StatusBar();
			this.sbpTracker = new System.Windows.Forms.StatusBarPanel();
			this.lblGoto = new System.Windows.Forms.Label();
			this.tbProvID = new System.Windows.Forms.TextBox();
			this.mapView1 = new EU2MapEditorControls.MapView();
			this.cmMap = new System.Windows.Forms.ContextMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.miVisBlock = new System.Windows.Forms.MenuItem();
			this.tcModes = new System.Windows.Forms.TabControl();
			this.tbpShading = new System.Windows.Forms.TabPage();
			this.tbpIDL = new System.Windows.Forms.TabPage();
			this.tbpIDF = new System.Windows.Forms.TabPage();
			this.tbpIDDiff = new System.Windows.Forms.TabPage();
			this.panel6 = new System.Windows.Forms.Panel();
			this.panel7 = new System.Windows.Forms.Panel();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sbpTracker)).BeginInit();
			this.tcModes.SuspendLayout();
			this.panel6.SuspendLayout();
			this.panel7.SuspendLayout();
			this.SuspendLayout();
			// 
			// cbBorders
			// 
			this.cbBorders.Location = new System.Drawing.Point(84, 4);
			this.cbBorders.Name = "cbBorders";
			this.cbBorders.Size = new System.Drawing.Size(96, 16);
			this.cbBorders.TabIndex = 4;
			this.cbBorders.Text = "Draw Borders";
			this.cbBorders.CheckedChanged += new System.EventHandler(this.cbBorders_CheckedChanged);
			// 
			// cbGrid
			// 
			this.cbGrid.Checked = true;
			this.cbGrid.CheckState = System.Windows.Forms.CheckState.Checked;
			this.cbGrid.Location = new System.Drawing.Point(4, 4);
			this.cbGrid.Name = "cbGrid";
			this.cbGrid.Size = new System.Drawing.Size(76, 16);
			this.cbGrid.TabIndex = 5;
			this.cbGrid.Text = "Draw Grid";
			this.cbGrid.CheckedChanged += new System.EventHandler(this.cbGrid_CheckedChanged);
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.panel1.Controls.Add(this.pbMini);
			this.panel1.Location = new System.Drawing.Point(496, 4);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(164, 76);
			this.panel1.TabIndex = 6;
			// 
			// pbMini
			// 
			this.pbMini.Image = ((System.Drawing.Image)(resources.GetObject("pbMini.Image")));
			this.pbMini.Location = new System.Drawing.Point(0, 0);
			this.pbMini.Name = "pbMini";
			this.pbMini.Size = new System.Drawing.Size(160, 72);
			this.pbMini.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pbMini.TabIndex = 3;
			this.pbMini.TabStop = false;
			// 
			// sbStatus
			// 
			this.sbStatus.Location = new System.Drawing.Point(2, 529);
			this.sbStatus.Name = "sbStatus";
			this.sbStatus.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
																						this.sbpTracker});
			this.sbStatus.ShowPanels = true;
			this.sbStatus.Size = new System.Drawing.Size(664, 20);
			this.sbStatus.TabIndex = 7;
			this.sbStatus.Text = "statusBar1";
			// 
			// sbpTracker
			// 
			this.sbpTracker.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
			this.sbpTracker.MinWidth = 154;
			this.sbpTracker.Text = "tracker";
			this.sbpTracker.Width = 154;
			// 
			// lblGoto
			// 
			this.lblGoto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.lblGoto.AutoSize = true;
			this.lblGoto.Location = new System.Drawing.Point(376, 64);
			this.lblGoto.Name = "lblGoto";
			this.lblGoto.Size = new System.Drawing.Size(59, 17);
			this.lblGoto.TabIndex = 2;
			this.lblGoto.Text = "Go to (ID):";
			// 
			// tbProvID
			// 
			this.tbProvID.AcceptsReturn = true;
			this.tbProvID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.tbProvID.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.tbProvID.Location = new System.Drawing.Point(440, 60);
			this.tbProvID.MaxLength = 10;
			this.tbProvID.Name = "tbProvID";
			this.tbProvID.Size = new System.Drawing.Size(52, 18);
			this.tbProvID.TabIndex = 1;
			this.tbProvID.Text = "";
			this.tbProvID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.tbProvID.WordWrap = false;
			this.tbProvID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbProvID_KeyPress);
			this.tbProvID.TextChanged += new System.EventHandler(this.tbProvID_TextChanged);
			// 
			// mapView1
			// 
			this.mapView1.AllowSelection = true;
			this.mapView1.ColorDetail = EU2MapEditorControls.ColorDetail.High;
			this.mapView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mapView1.GridColor = System.Drawing.Color.DimGray;
			this.mapView1.GridMode = EU2MapEditorControls.MapGridMode.Near;
			this.mapView1.GridOpacity = 80;
			this.mapView1.Location = new System.Drawing.Point(2, 86);
			this.mapView1.Minimap = this.pbMini;
			this.mapView1.MouseTrackerColor = System.Drawing.Color.Yellow;
			this.mapView1.MouseTrackerOpacity = 90;
			this.mapView1.Name = "mapView1";
			this.mapView1.OriginBlocks = new System.Drawing.Point(0, 0);
			this.mapView1.ScrollZoneColor = System.Drawing.SystemColors.ActiveCaption;
			this.mapView1.Selection = new System.Drawing.Rectangle(0, 0, 0, 0);
			this.mapView1.Size = new System.Drawing.Size(664, 443);
			this.mapView1.Source = null;
			this.mapView1.TabIndex = 3;
			this.mapView1.TrackerMoved += new EU2MapEditorControls.MapView.TrackerMovedEventHandler(this.mapView1_TrackerMoved);
			this.mapView1.SelectionChanged += new EU2MapEditorControls.MapView.SelectionChangedHandler(this.mapView1_SelectionChanged);
			this.mapView1.ContextMenu += new EU2MapEditorControls.MapView.ContextMenuEventHandler(this.mapView1_ContextMenu);
			// 
			// cmMap
			// 
			this.cmMap.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																				  this.menuItem1,
																				  this.miVisBlock});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.Text = "Export \"mimage /E\" commandline to clipboard";
			this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
			// 
			// miVisBlock
			// 
			this.miVisBlock.Index = 1;
			this.miVisBlock.Text = "Visualise Block";
			this.miVisBlock.Click += new System.EventHandler(this.miVisBlock_Click);
			// 
			// tcModes
			// 
			this.tcModes.Appearance = System.Windows.Forms.TabAppearance.Buttons;
			this.tcModes.Controls.Add(this.tbpShading);
			this.tcModes.Controls.Add(this.tbpIDL);
			this.tcModes.Controls.Add(this.tbpIDF);
			this.tcModes.Controls.Add(this.tbpIDDiff);
			this.tcModes.Location = new System.Drawing.Point(0, 60);
			this.tcModes.Multiline = true;
			this.tcModes.Name = "tcModes";
			this.tcModes.SelectedIndex = 0;
			this.tcModes.Size = new System.Drawing.Size(264, 24);
			this.tcModes.TabIndex = 6;
			this.tcModes.SelectedIndexChanged += new System.EventHandler(this.tcModes_SelectedIndexChanged);
			// 
			// tbpShading
			// 
			this.tbpShading.Location = new System.Drawing.Point(4, 25);
			this.tbpShading.Name = "tbpShading";
			this.tbpShading.Size = new System.Drawing.Size(256, 0);
			this.tbpShading.TabIndex = 0;
			this.tbpShading.Text = "Normal";
			// 
			// tbpIDL
			// 
			this.tbpIDL.Location = new System.Drawing.Point(4, 22);
			this.tbpIDL.Name = "tbpIDL";
			this.tbpIDL.Size = new System.Drawing.Size(484, 521);
			this.tbpIDL.TabIndex = 1;
			this.tbpIDL.Text = "IDs (Lightmap)";
			// 
			// tbpIDF
			// 
			this.tbpIDF.Location = new System.Drawing.Point(4, 22);
			this.tbpIDF.Name = "tbpIDF";
			this.tbpIDF.Size = new System.Drawing.Size(484, 521);
			this.tbpIDF.TabIndex = 2;
			this.tbpIDF.Text = "IDs (File)";
			// 
			// tbpIDDiff
			// 
			this.tbpIDDiff.Location = new System.Drawing.Point(4, 22);
			this.tbpIDDiff.Name = "tbpIDDiff";
			this.tbpIDDiff.Size = new System.Drawing.Size(484, 521);
			this.tbpIDDiff.TabIndex = 3;
			this.tbpIDDiff.Text = "IDs (Diff)";
			// 
			// panel6
			// 
			this.panel6.Controls.Add(this.mapView1);
			this.panel6.Controls.Add(this.panel7);
			this.panel6.Controls.Add(this.sbStatus);
			this.panel6.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel6.DockPadding.All = 2;
			this.panel6.Location = new System.Drawing.Point(0, 0);
			this.panel6.Name = "panel6";
			this.panel6.Size = new System.Drawing.Size(668, 551);
			this.panel6.TabIndex = 7;
			// 
			// panel7
			// 
			this.panel7.Controls.Add(this.panel1);
			this.panel7.Controls.Add(this.cbBorders);
			this.panel7.Controls.Add(this.cbGrid);
			this.panel7.Controls.Add(this.lblGoto);
			this.panel7.Controls.Add(this.tbProvID);
			this.panel7.Controls.Add(this.tcModes);
			this.panel7.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel7.Location = new System.Drawing.Point(2, 2);
			this.panel7.Name = "panel7";
			this.panel7.Size = new System.Drawing.Size(664, 84);
			this.panel7.TabIndex = 7;
			// 
			// MapForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(668, 551);
			this.Controls.Add(this.panel6);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MapForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "Map View";
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.sbpTracker)).EndInit();
			this.tcModes.ResumeLayout(false);
			this.panel6.ResumeLayout(false);
			this.panel7.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void mapView1_TrackerMoved(object sender, EU2MapEditorControls.TrackerMovedEventArgs e) {
			string id = "";
			if ( workset.File != null && workset.File.IDMap != null ) {
				id = String.Format( "(ID: {0})", workset.File.IDMap[e.X, e.Y] );
			}
			sbpTracker.Text = String.Format( "{0:00000}:{1:0000} {2}", e.X, e.Y, id ); 
		}

		public Workset Source {
			get {
				return workset;
			}
			set {
				workset = value;
				if ( workset == null ) workset = new Workset();

				Cursor = Cursors.WaitCursor;
				mapView1.Source = null;
				if ( workset.File != null ) {
					mapView1.Source = workset.File.Lightmap1;
					tcModes.SelectedTab = null;
					tcModes.SelectedTab = tbpShading;

					if ( workset.File.BoundBoxes == null ) workset.File.BoundBoxes = new EU2.Map.BoundBoxes( workset.File.IDMap.CalculateBoundBoxes() );
				}

				lblGoto.Enabled = (workset.File != null && workset.File.BoundBoxes != null);
				tbProvID.Enabled = (workset.File != null && workset.File.BoundBoxes != null);

				Cursor = Cursors.Default;
			}
		}

		private void tbProvID_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
			if ( workset.File.BoundBoxes == null ) return;
			if ( e.KeyChar != '\n' && e.KeyChar != '\r' ) return;

			int id = -1;
			try {
				id = int.Parse( tbProvID.Text );
			}
			catch {
			}

			if ( id <= 0 || id > EU2.Data.Province.MaxID ) {
				tbProvID.ForeColor = Color.Red;
			}
			else {
				Rectangle box = workset.File.BoundBoxes[id].Box;
				tbProvID.ForeColor = SystemColors.WindowText;
				mapView1.CenterMapTo( box.Left + box.Width/2, box.Top + box.Height/2  );
			}
		}

		private void MainForm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			switch ( e.KeyCode ) {
				case Keys.Left: 
					mapView1.MoveMapBy( -EU2.Map.Lightmap.BlockSize, 0 );
					break;
				case Keys.Right: 
					mapView1.MoveMapBy( EU2.Map.Lightmap.BlockSize, 0 );
					break;
				case Keys.Up: 
					mapView1.MoveMapBy( 0, -EU2.Map.Lightmap.BlockSize );
					break;
				case Keys.Down: 
					mapView1.MoveMapBy( 0, EU2.Map.Lightmap.BlockSize );
					break;
			}
		}

		private void tbProvID_TextChanged(object sender, System.EventArgs e) {
		
		}

		private void cbBorders_CheckedChanged(object sender, System.EventArgs e) {
			mapView1.ShaderProxy.DrawBorders = cbBorders.Checked;
			mapView1.Refresh();
		}

		private void cbGrid_CheckedChanged(object sender, System.EventArgs e) {
			mapView1.GridMode = cbGrid.Checked ? MapGridMode.Full : MapGridMode.None;
		}

		private void mapView1_SelectionChanged(object sender, System.EventArgs e) {
//			if ( (mapView1.Selection.Width == 0 && mapView1.Selection.Height == 0) ) {
//				lblSelection.Text = "";
//			}
//			else {
//				lblSelection.Text = String.Format( "x,y : {0},{1}\nw,h: {2},{3}", mapView1.Selection.X, mapView1.Selection.Y, mapView1.Selection.Width, mapView1.Selection.Height );
//			}
		}

		private void mapView1_ContextMenu(object sender, EU2MapEditorControls.ContextMenuEventArgs e) {
			if ( workset.File == null ) return;

			menuItem1.Enabled = (mapView1.Selection.Width >= 0 && mapView1.Selection.Height >= 0);
			miVisBlock.Enabled = e.BlockIndex >= 0;
			cmblockindex = e.BlockIndex;
			cmMap.Show( mapView1, new Point( e.X, e.Y ) );
		}

		private void menuItem1_Click(object sender, System.EventArgs e) {
			Clipboard.SetDataObject( String.Format( 
				"mimage /e \"{4}\" /r:{0},{1},{2},{3} /1", 
				mapView1.Selection.X, mapView1.Selection.Y, mapView1.Selection.Width, mapView1.Selection.Height,
				workset.FilePath ), true );
			MessageBox.Show( this, "Done!", "Success" );
		}

		private void tcModes_SelectedIndexChanged(object sender, System.EventArgs e) {
			if ( workset.File == null ) return;

			if ( tcModes.SelectedTab == tbpShading ) {
				if ( workset.File.ColorScales != null && workset.File.Provinces != null ) {
					try {
						workset.File.ColorScales.CalculateShades();
						mapView1.ShaderProxy = new ColoredShaderProxy( workset.File.ColorScales, workset.File.Provinces );
					}
					catch ( DivideByZeroException ) {
						mapView1.ShaderProxy = new DefaultShaderProxy();
					}
				}
				else
					mapView1.ShaderProxy = new DefaultShaderProxy();
				mapView1.ShaderProxy.DrawBorders = cbBorders.Checked;
			}
			else if ( tcModes.SelectedTab == tbpIDL ) {
				mapView1.ShaderProxy = new GenericShaderProxy( new IDShader( new MapToolsLib.Inferis2IDConvertor() ) );
			}
			else if ( tcModes.SelectedTab == tbpIDF ) {
				mapView1.ShaderProxy = new GenericShaderProxy( new IDMapShader( workset.File.IDMap, new MapToolsLib.Inferis2IDConvertor() ) );
			}
			else if ( tcModes.SelectedTab == tbpIDF ) {
				mapView1.ShaderProxy = new GenericShaderProxy( new IDMapShader( workset.File.IDMap, new MapToolsLib.Inferis2IDConvertor() ) );
			}
			else if ( tcModes.SelectedTab == tbpIDDiff ) {
				mapView1.ShaderProxy = new GenericShaderProxy( new IDMapShader( workset.File.IDMap, true ) );
			}
		}

		private void miVisBlock_Click(object sender, System.EventArgs e) {
//			MapToolsLib.BlockVisualiser visualiser = new MapToolsLib.BlockVisualiser( file.ColorScales, file.Provinces );
//			visualiser.WalkTree( mapView1.Source.GetDecompressedBlock( cmblockindex ) );
//			
//			BlockVisForm form = new BlockVisForm();
//			form.Visuals = visualiser.Result;
//			form.ShowDialog( this );
		}

		private Workset workset = new Workset();
	}
}
