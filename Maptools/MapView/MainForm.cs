
using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using EU2MapEditorControls;
using LayerPainter;

namespace MapView
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
        private System.Windows.Forms.Panel rootPanel;
        private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.TextBox tbProvID;
		private System.Windows.Forms.Label lblGoto;
		private System.Windows.Forms.StatusBar sbStatus;
        private System.Windows.Forms.PictureBox pbMini;
		private System.Windows.Forms.StatusBarPanel sbpTracker;
		private System.Windows.Forms.StatusBarPanel sbpStatus;
		private System.Windows.Forms.ContextMenu cmMap;
        private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem miVisBlock;
		/// <summary>
		/// Required designer variable.
		/// </summary>
        private System.ComponentModel.Container components = null;
		private System.Windows.Forms.GroupBox groupBox3;
		private EU2MapEditorControls.LayeredMapView mapView1;
        private System.Windows.Forms.Panel panel3;
        private ToolStrip toolStrip1;
        private ToolStripButton MapNormalButton;
        private ToolStripButton MapIDsLightmapButton;
        private ToolStripButton MapIDsFileButton;
        private ToolStripButton MapIDsDiffButton;
        private ToolStripLabel toolStripLabel1;
        private ToolStripButton ScreenshotButton;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton ReloadButton;
        private ToolStripLabel fileLabel;
        private GroupBox groupBox2;
        private CheckBox overlayHoverCheckBox;
        private CheckBox overlayProvinceCheckBox;
        private CheckBox RegionOverlapCheckbox;
        private CheckBox DrawRegionsCheckbox;
        private GroupBox groupBox1;
        private RadioButton rbGridFull;
        private RadioButton rbGridNear;
        private RadioButton rbGridNone;
        private CheckBox cbCoordinates;
        private CheckBox cbBorders;
        private ToolStripButton RefreshButton;
        private ToolStripButton Zoom3Button;
        private ToolStripButton Zoom2Button;
        private ToolStripButton Zoom1Button;
        private ToolStripLabel toolStripLabel2;
		private int cmblockindex = -1;

		public MainForm( string file, int map )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

            mapView1.Layers.Add(new MapToolsLib.LayerPainters.MapLayerPainter());
            mapView1.Layers.Add(new ProvinceLayerPainter());
            mapView1.Layers["Province"].Enabled = false;

            fileLabel.ToolTipText = file;
            fileLabel.Text = Path.GetFileName(file);
            useMap = map;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.rootPanel = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.mapView1 = new EU2MapEditorControls.LayeredMapView();
            this.pbMini = new System.Windows.Forms.PictureBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.MapIDsDiffButton = new System.Windows.Forms.ToolStripButton();
            this.MapIDsFileButton = new System.Windows.Forms.ToolStripButton();
            this.MapIDsLightmapButton = new System.Windows.Forms.ToolStripButton();
            this.MapNormalButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.fileLabel = new System.Windows.Forms.ToolStripLabel();
            this.ReloadButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ScreenshotButton = new System.Windows.Forms.ToolStripButton();
            this.RefreshButton = new System.Windows.Forms.ToolStripButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblGoto = new System.Windows.Forms.Label();
            this.tbProvID = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.overlayHoverCheckBox = new System.Windows.Forms.CheckBox();
            this.overlayProvinceCheckBox = new System.Windows.Forms.CheckBox();
            this.RegionOverlapCheckbox = new System.Windows.Forms.CheckBox();
            this.DrawRegionsCheckbox = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbGridFull = new System.Windows.Forms.RadioButton();
            this.rbGridNear = new System.Windows.Forms.RadioButton();
            this.rbGridNone = new System.Windows.Forms.RadioButton();
            this.cbCoordinates = new System.Windows.Forms.CheckBox();
            this.cbBorders = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.sbStatus = new System.Windows.Forms.StatusBar();
            this.sbpStatus = new System.Windows.Forms.StatusBarPanel();
            this.sbpTracker = new System.Windows.Forms.StatusBarPanel();
            this.cmMap = new System.Windows.Forms.ContextMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.miVisBlock = new System.Windows.Forms.MenuItem();
            this.Zoom3Button = new System.Windows.Forms.ToolStripButton();
            this.Zoom2Button = new System.Windows.Forms.ToolStripButton();
            this.Zoom1Button = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.rootPanel.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbMini)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sbpStatus)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbpTracker)).BeginInit();
            this.SuspendLayout();
            // 
            // rootPanel
            // 
            this.rootPanel.Controls.Add(this.panel3);
            this.rootPanel.Controls.Add(this.panel2);
            this.rootPanel.Controls.Add(this.sbStatus);
            this.rootPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rootPanel.Location = new System.Drawing.Point(0, 0);
            this.rootPanel.Name = "rootPanel";
            this.rootPanel.Size = new System.Drawing.Size(707, 479);
            this.rootPanel.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.mapView1);
            this.panel3.Controls.Add(this.toolStrip1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.panel3.Size = new System.Drawing.Size(539, 459);
            this.panel3.TabIndex = 7;
            // 
            // mapView1
            // 
            this.mapView1.CoordinatesColor = System.Drawing.Color.Brown;
            this.mapView1.CoordinatesFont = new System.Drawing.Font("Tahoma", 6F);
            this.mapView1.DatelineColor = System.Drawing.Color.DarkViolet;
            this.mapView1.DatelineOpacity = 1;
            this.mapView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mapView1.DrawRegionBorders = EU2MapEditorControls.RegionDrawMode.None;
            this.mapView1.GridColor = System.Drawing.Color.DimGray;
            this.mapView1.GridMode = EU2MapEditorControls.MapGridMode.Near;
            this.mapView1.Location = new System.Drawing.Point(4, 25);
            this.mapView1.Minimap = this.pbMini;
            this.mapView1.MouseTrackerColor = System.Drawing.Color.Yellow;
            this.mapView1.Name = "mapView1";
            this.mapView1.RegionBorderColor = System.Drawing.Color.Orange;
            this.mapView1.RegionBorderOpacity = 1;
            this.mapView1.RegionOverlapColor = System.Drawing.Color.Orange;
            this.mapView1.ScrollZoneColor = System.Drawing.SystemColors.ActiveCaption;
            this.mapView1.Selection = new System.Drawing.Rectangle(0, 0, 0, 0);
            this.mapView1.ShowLoading = false;
            this.mapView1.Size = new System.Drawing.Size(535, 434);
            this.mapView1.TabIndex = 7;
            this.mapView1.TrackerMoved += new EU2MapEditorControls.LayeredMapView.TrackerMovedEventHandler(this.mapView1_TrackerMoved);
            this.mapView1.SelectionChanged += new EU2MapEditorControls.LayeredMapView.SelectionChangedHandler(this.mapView1_SelectionChanged);
            this.mapView1.ContextMenu += new EU2MapEditorControls.LayeredMapView.ContextMenuEventHandler(this.mapView1_ContextMenu);
            // 
            // pbMini
            // 
            this.pbMini.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pbMini.Image = ((System.Drawing.Image)(resources.GetObject("pbMini.Image")));
            this.pbMini.Location = new System.Drawing.Point(4, 4);
            this.pbMini.Name = "pbMini";
            this.pbMini.Size = new System.Drawing.Size(164, 76);
            this.pbMini.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbMini.TabIndex = 3;
            this.pbMini.TabStop = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MapIDsDiffButton,
            this.MapIDsFileButton,
            this.MapIDsLightmapButton,
            this.MapNormalButton,
            this.toolStripLabel1,
            this.fileLabel,
            this.ReloadButton,
            this.toolStripSeparator1,
            this.ScreenshotButton,
            this.RefreshButton,
            this.Zoom3Button,
            this.Zoom2Button,
            this.Zoom1Button,
            this.toolStripLabel2});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStrip1.Location = new System.Drawing.Point(4, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(535, 25);
            this.toolStrip1.TabIndex = 7;
            // 
            // MapIDsDiffButton
            // 
            this.MapIDsDiffButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.MapIDsDiffButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.MapIDsDiffButton.Image = ((System.Drawing.Image)(resources.GetObject("MapIDsDiffButton.Image")));
            this.MapIDsDiffButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MapIDsDiffButton.Name = "MapIDsDiffButton";
            this.MapIDsDiffButton.Size = new System.Drawing.Size(23, 22);
            this.MapIDsDiffButton.Text = "IDs (Diff)";
            this.MapIDsDiffButton.Click += new System.EventHandler(this.MapIDsDiffButton_Click);
            // 
            // MapIDsFileButton
            // 
            this.MapIDsFileButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.MapIDsFileButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.MapIDsFileButton.Image = ((System.Drawing.Image)(resources.GetObject("MapIDsFileButton.Image")));
            this.MapIDsFileButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MapIDsFileButton.Name = "MapIDsFileButton";
            this.MapIDsFileButton.Size = new System.Drawing.Size(23, 22);
            this.MapIDsFileButton.Text = "IDs (File)";
            this.MapIDsFileButton.Click += new System.EventHandler(this.MapIDsFileButton_Click);
            // 
            // MapIDsLightmapButton
            // 
            this.MapIDsLightmapButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.MapIDsLightmapButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.MapIDsLightmapButton.Image = ((System.Drawing.Image)(resources.GetObject("MapIDsLightmapButton.Image")));
            this.MapIDsLightmapButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MapIDsLightmapButton.Name = "MapIDsLightmapButton";
            this.MapIDsLightmapButton.Size = new System.Drawing.Size(23, 22);
            this.MapIDsLightmapButton.Text = "IDs (Lightmap)";
            this.MapIDsLightmapButton.Click += new System.EventHandler(this.MapIDsLightmapButton_Click);
            // 
            // MapNormalButton
            // 
            this.MapNormalButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.MapNormalButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.MapNormalButton.Image = ((System.Drawing.Image)(resources.GetObject("MapNormalButton.Image")));
            this.MapNormalButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MapNormalButton.Name = "MapNormalButton";
            this.MapNormalButton.Size = new System.Drawing.Size(23, 22);
            this.MapNormalButton.Text = "Normal";
            this.MapNormalButton.Click += new System.EventHandler(this.MapNormalButton_Click);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(35, 22);
            this.toolStripLabel1.Text = "View:";
            // 
            // fileLabel
            // 
            this.fileLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileLabel.Name = "fileLabel";
            this.fileLabel.Size = new System.Drawing.Size(16, 22);
            this.fileLabel.Text = "...";
            // 
            // ReloadButton
            // 
            this.ReloadButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ReloadButton.Image = ((System.Drawing.Image)(resources.GetObject("ReloadButton.Image")));
            this.ReloadButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ReloadButton.Name = "ReloadButton";
            this.ReloadButton.Size = new System.Drawing.Size(23, 22);
            this.ReloadButton.Text = "Reload";
            this.ReloadButton.Click += new System.EventHandler(this.ReloadButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // ScreenshotButton
            // 
            this.ScreenshotButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ScreenshotButton.Image = ((System.Drawing.Image)(resources.GetObject("ScreenshotButton.Image")));
            this.ScreenshotButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ScreenshotButton.Name = "ScreenshotButton";
            this.ScreenshotButton.Size = new System.Drawing.Size(23, 22);
            this.ScreenshotButton.Text = "Screenshot";
            this.ScreenshotButton.Click += new System.EventHandler(this.ScreenshotButton_Click);
            // 
            // RefreshButton
            // 
            this.RefreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.RefreshButton.Image = ((System.Drawing.Image)(resources.GetObject("RefreshButton.Image")));
            this.RefreshButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.RefreshButton.Name = "RefreshButton";
            this.RefreshButton.Size = new System.Drawing.Size(23, 22);
            this.RefreshButton.Text = "Refresh";
            this.RefreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.pbMini);
            this.panel2.Controls.Add(this.lblGoto);
            this.panel2.Controls.Add(this.tbProvID);
            this.panel2.Controls.Add(this.groupBox2);
            this.panel2.Controls.Add(this.groupBox3);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(539, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(168, 459);
            this.panel2.TabIndex = 2;
            // 
            // lblGoto
            // 
            this.lblGoto.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblGoto.AutoSize = true;
            this.lblGoto.Location = new System.Drawing.Point(4, 82);
            this.lblGoto.Name = "lblGoto";
            this.lblGoto.Size = new System.Drawing.Size(103, 13);
            this.lblGoto.TabIndex = 2;
            this.lblGoto.Text = "Go to Province (ID):";
            // 
            // tbProvID
            // 
            this.tbProvID.AcceptsReturn = true;
            this.tbProvID.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tbProvID.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbProvID.Location = new System.Drawing.Point(112, 80);
            this.tbProvID.MaxLength = 10;
            this.tbProvID.Name = "tbProvID";
            this.tbProvID.Size = new System.Drawing.Size(52, 18);
            this.tbProvID.TabIndex = 1;
            this.tbProvID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.tbProvID.WordWrap = false;
            this.tbProvID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbProvID_KeyPress);
            this.tbProvID.TextChanged += new System.EventHandler(this.tbProvID_TextChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.overlayHoverCheckBox);
            this.groupBox2.Controls.Add(this.overlayProvinceCheckBox);
            this.groupBox2.Controls.Add(this.RegionOverlapCheckbox);
            this.groupBox2.Controls.Add(this.DrawRegionsCheckbox);
            this.groupBox2.Controls.Add(this.groupBox1);
            this.groupBox2.Controls.Add(this.cbBorders);
            this.groupBox2.Location = new System.Drawing.Point(-100, 262);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(392, 200);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "groupBox2";
            // 
            // overlayHoverCheckBox
            // 
            this.overlayHoverCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.overlayHoverCheckBox.Checked = true;
            this.overlayHoverCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.overlayHoverCheckBox.Enabled = false;
            this.overlayHoverCheckBox.Location = new System.Drawing.Point(123, 175);
            this.overlayHoverCheckBox.Name = "overlayHoverCheckBox";
            this.overlayHoverCheckBox.Size = new System.Drawing.Size(141, 25);
            this.overlayHoverCheckBox.TabIndex = 11;
            this.overlayHoverCheckBox.Text = "Hover only";
            this.overlayHoverCheckBox.Visible = false;
            // 
            // overlayProvinceCheckBox
            // 
            this.overlayProvinceCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.overlayProvinceCheckBox.Location = new System.Drawing.Point(104, 155);
            this.overlayProvinceCheckBox.Name = "overlayProvinceCheckBox";
            this.overlayProvinceCheckBox.Size = new System.Drawing.Size(160, 25);
            this.overlayProvinceCheckBox.TabIndex = 10;
            this.overlayProvinceCheckBox.Text = "Overlay province info";
            this.overlayProvinceCheckBox.CheckedChanged += new System.EventHandler(this.overlayProvinceCheckBox_CheckedChanged);
            // 
            // RegionOverlapCheckbox
            // 
            this.RegionOverlapCheckbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.RegionOverlapCheckbox.Location = new System.Drawing.Point(123, 125);
            this.RegionOverlapCheckbox.Name = "RegionOverlapCheckbox";
            this.RegionOverlapCheckbox.Size = new System.Drawing.Size(141, 25);
            this.RegionOverlapCheckbox.TabIndex = 9;
            this.RegionOverlapCheckbox.Text = "Including overlap areas";
            this.RegionOverlapCheckbox.CheckedChanged += new System.EventHandler(this.RegionOverlapCheckbox_CheckedChanged);
            // 
            // DrawRegionsCheckbox
            // 
            this.DrawRegionsCheckbox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.DrawRegionsCheckbox.Checked = true;
            this.DrawRegionsCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.DrawRegionsCheckbox.Location = new System.Drawing.Point(104, 107);
            this.DrawRegionsCheckbox.Name = "DrawRegionsCheckbox";
            this.DrawRegionsCheckbox.Size = new System.Drawing.Size(160, 25);
            this.DrawRegionsCheckbox.TabIndex = 8;
            this.DrawRegionsCheckbox.Text = "Show Region Borders";
            this.DrawRegionsCheckbox.CheckedChanged += new System.EventHandler(this.DrawRegionsCheckbox_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.rbGridFull);
            this.groupBox1.Controls.Add(this.rbGridNear);
            this.groupBox1.Controls.Add(this.rbGridNone);
            this.groupBox1.Controls.Add(this.cbCoordinates);
            this.groupBox1.Location = new System.Drawing.Point(104, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(160, 60);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Grid";
            // 
            // rbGridFull
            // 
            this.rbGridFull.Location = new System.Drawing.Point(114, 16);
            this.rbGridFull.Name = "rbGridFull";
            this.rbGridFull.Size = new System.Drawing.Size(42, 20);
            this.rbGridFull.TabIndex = 2;
            this.rbGridFull.Text = "Full";
            this.rbGridFull.CheckedChanged += new System.EventHandler(this.rbGridFull_CheckedChanged);
            // 
            // rbGridNear
            // 
            this.rbGridNear.Checked = true;
            this.rbGridNear.Location = new System.Drawing.Point(60, 16);
            this.rbGridNear.Name = "rbGridNear";
            this.rbGridNear.Size = new System.Drawing.Size(58, 20);
            this.rbGridNear.TabIndex = 1;
            this.rbGridNear.TabStop = true;
            this.rbGridNear.Text = "Near";
            this.rbGridNear.CheckedChanged += new System.EventHandler(this.rbGridNear_CheckedChanged);
            // 
            // rbGridNone
            // 
            this.rbGridNone.Location = new System.Drawing.Point(4, 16);
            this.rbGridNone.Name = "rbGridNone";
            this.rbGridNone.Size = new System.Drawing.Size(52, 20);
            this.rbGridNone.TabIndex = 0;
            this.rbGridNone.Text = "None";
            this.rbGridNone.CheckedChanged += new System.EventHandler(this.rbGridNone_CheckedChanged);
            // 
            // cbCoordinates
            // 
            this.cbCoordinates.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbCoordinates.Location = new System.Drawing.Point(19, 38);
            this.cbCoordinates.Name = "cbCoordinates";
            this.cbCoordinates.Size = new System.Drawing.Size(140, 19);
            this.cbCoordinates.TabIndex = 5;
            this.cbCoordinates.Text = "Draw Block Coordinates";
            this.cbCoordinates.CheckedChanged += new System.EventHandler(this.cbGrid_CheckedChanged);
            // 
            // cbBorders
            // 
            this.cbBorders.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cbBorders.Location = new System.Drawing.Point(104, 76);
            this.cbBorders.Name = "cbBorders";
            this.cbBorders.Size = new System.Drawing.Size(160, 25);
            this.cbBorders.TabIndex = 4;
            this.cbBorders.Text = "Draw Borders";
            this.cbBorders.CheckedChanged += new System.EventHandler(this.cbBorders_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Location = new System.Drawing.Point(-112, -76);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(384, 184);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "groupBox3";
            this.groupBox3.Enter += new System.EventHandler(this.groupBox3_Enter);
            // 
            // sbStatus
            // 
            this.sbStatus.Location = new System.Drawing.Point(0, 459);
            this.sbStatus.Name = "sbStatus";
            this.sbStatus.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.sbpStatus,
            this.sbpTracker});
            this.sbStatus.ShowPanels = true;
            this.sbStatus.Size = new System.Drawing.Size(707, 20);
            this.sbStatus.TabIndex = 4;
            this.sbStatus.Text = "statusBar1";
            this.sbStatus.PanelClick += new System.Windows.Forms.StatusBarPanelClickEventHandler(this.sbStatus_PanelClick);
            // 
            // sbpStatus
            // 
            this.sbpStatus.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            this.sbpStatus.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.None;
            this.sbpStatus.MinWidth = 200;
            this.sbpStatus.Name = "sbpStatus";
            this.sbpStatus.Width = 536;
            // 
            // sbpTracker
            // 
            this.sbpTracker.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
            this.sbpTracker.MinWidth = 154;
            this.sbpTracker.Name = "sbpTracker";
            this.sbpTracker.Text = "tracker";
            this.sbpTracker.Width = 154;
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
            // Zoom3Button
            // 
            this.Zoom3Button.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.Zoom3Button.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.Zoom3Button.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Zoom3Button.Image = ((System.Drawing.Image)(resources.GetObject("Zoom3Button.Image")));
            this.Zoom3Button.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Zoom3Button.Name = "Zoom3Button";
            this.Zoom3Button.Size = new System.Drawing.Size(23, 22);
            this.Zoom3Button.Text = "3";
            this.Zoom3Button.Click += new System.EventHandler(this.Zoom3Button_Click);
            // 
            // Zoom2Button
            // 
            this.Zoom2Button.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.Zoom2Button.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.Zoom2Button.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Zoom2Button.Image = ((System.Drawing.Image)(resources.GetObject("Zoom2Button.Image")));
            this.Zoom2Button.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Zoom2Button.Name = "Zoom2Button";
            this.Zoom2Button.Size = new System.Drawing.Size(23, 22);
            this.Zoom2Button.Text = "2";
            this.Zoom2Button.Click += new System.EventHandler(this.Zoom2Button_Click);
            // 
            // Zoom1Button
            // 
            this.Zoom1Button.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.Zoom1Button.Checked = true;
            this.Zoom1Button.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Zoom1Button.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.Zoom1Button.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Zoom1Button.Image = ((System.Drawing.Image)(resources.GetObject("Zoom1Button.Image")));
            this.Zoom1Button.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.Zoom1Button.Name = "Zoom1Button";
            this.Zoom1Button.Size = new System.Drawing.Size(23, 22);
            this.Zoom1Button.Text = "1";
            this.Zoom1Button.Click += new System.EventHandler(this.Zoom1Button_Click);
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(39, 22);
            this.toolStripLabel2.Text = "Zoom:";
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
            this.ClientSize = new System.Drawing.Size(707, 479);
            this.Controls.Add(this.rootPanel);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(481, 374);
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.Text = "EU2Map File Viewer";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.rootPanel.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbMini)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sbpStatus)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sbpTracker)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void MainForm_Load(object sender, System.EventArgs e) {
            LocalizeMinimap();
            Show();
			mapView1.ShowLoading = true;
            RegionCheckBoxesChanged();
            Application.DoEvents();
			LoadFile();
		}

        private void LocalizeMinimap() {
            string p = Path.GetDirectoryName(Application.ExecutablePath);
            foreach ( string ext in "png|gif|jpg|bmp".Split('|')) {
                string f = Path.Combine(p, string.Format("minimap.{0}", ext));

                Image current = pbMini.Image;
                if (File.Exists(f)) {
                    try {
                        Image img = Image.FromFile(f);
                        pbMini.SizeMode = PictureBoxSizeMode.StretchImage;
                        pbMini.Image = img;
                        return;
                    }
                    catch {
                        if (pbMini.Image != current) pbMini.Image = current;
                    }
                }
            }
        }

        private void mapView1_TrackerMoved(object sender, EU2MapEditorControls.TrackerMovedEventArgs e) {
            int id = 0;
            string sid = "";
            if (file != null && file.IDMap != null) {
                id = file.IDMap[e.X, e.Y];
				sid = String.Format( "(ID: {0})", id );
			}
			sbpTracker.Text = String.Format( "{0:00000}:{1:0000} {2}", e.X, e.Y, sid );

            if (mapView1.Layers.Contains("Province")) {
                (mapView1.Layers["Province"] as ProvinceLayerPainter).HighlightId = id;
            }
		}

		private delegate void CallbackDelegate(object state);
		private delegate void InvokeDelegate();
		private void LoadFile() {
			Cursor = Cursors.WaitCursor;
			ReloadButton.Enabled = false;
			sbpStatus.Text = "Loading file...";
			Application.DoEvents();
 
			file = null;
			rootPanel.Enabled = false;
			mapView1.ShowLoading = true;
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(loadFile_callback), fileLabel.ToolTipText);
		}

		private object fileLock = new object();
		private void loadFile_callback(object state) {
			lock(fileLock) {
				try {
					file = new EU2.Edit.File();
					file.ReadFrom((string)state);
				}
				catch {
					file = null;
				}
			}

			System.Threading.Thread.Sleep(100);
			
			this.Invoke(new InvokeDelegate(loadFile_done));
		}

		private void loadFile_done() {
            try {
			    lock(fileLock) {
				    if ( file != null ) {
					    if (mapView1.Layers.Contains("Map")) {
                            MapToolsLib.LayerPainters.MapLayerPainter layer = (MapToolsLib.LayerPainters.MapLayerPainter)mapView1.Layers["Map"];
						    layer[1] = file.Lightmap1;
						    layer[2] = file.Lightmap2;
						    layer[3] = file.Lightmap3;
						    layer.CurrentSource = useMap;

						    if ( useMap < 1 || useMap > 3 || layer[useMap] == null ) {
							    Console.WriteLine( "There is no lightmap" + useMap + " in this file." );
							    Application.Exit();
							    return;
						    }
						    if ( file.BoundBoxes == null ) 
							    file.BoundBoxes = new EU2.Map.BoundBoxes( file.IDMap.CalculateBoundBoxes() );

                            mapView1.LightmapDimensions = layer[useMap];

                            Zoom1Button.Enabled = file.Lightmap1 != null;
                            Zoom2Button.Enabled = file.Lightmap2 != null;
                            Zoom3Button.Enabled = file.Lightmap3 != null;

                            Zoom1Button.Checked = false;
                            Zoom2Button.Checked = false;
                            Zoom3Button.Checked = false;
                            if (useMap == 3)
                                Zoom3Button.Checked = true;
                            else if (useMap == 2) 
                                Zoom2Button.Checked = true;
                            else
                                Zoom1Button.Checked = true;

                            SetZoom(useMap);
                        }
					    else {
						    MessageBox.Show( "The file is okay, but something goes wrong trying to visualise it. Sorry.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
					    }

                        if (mapView1.Layers.Contains("Province")) {
                            ProvinceLayerPainter layer = (ProvinceLayerPainter)mapView1.Layers["Province"];

                            layer.Source = file;
                        }
				    }
				    else {
					    MessageBox.Show( "The file could not be read.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				    }
			    }
            }
            catch {
                MessageBox.Show("The file could not be read.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            MapNormalButton.Checked = true;
            MapNormalButton_Click(MapNormalButton, EventArgs.Empty);

            rootPanel.Enabled = true;
			mapView1.ShowLoading = false;
            sbpStatus.Text = "";

			lblGoto.Enabled = (file != null && file.BoundBoxes != null);
			tbProvID.Enabled = (file != null && file.BoundBoxes != null);

			ReloadButton.Enabled = true;
			Cursor = Cursors.Default;
		}

		private EU2.Edit.File file = null;

		private void tbProvID_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e) {
			if ( e.KeyChar != '\n' && e.KeyChar != '\r' ) return;

            if (file.BoundBoxes == null) {
                if (MessageBox.Show(this, "There is no boundbox info in this file, which is needed to find the province location.\nDo you wish to generate it from the map data (might take a while)?", "Insufficient data", MessageBoxButtons.YesNo) == DialogResult.No)
                    return;
                try {
                    sbpStatus.Text = "Calculating boundboxes from idmap...";
                    file.BoundBoxes = new EU2.Map.BoundBoxes(file.IDMap.CalculateBoundBoxes());
                }
                catch {
                    MessageBox.Show(this, "An error occured while generating the boundboxes. Sorry!", "Oops, something went wrong", MessageBoxButtons.OK);
                    return;
                }
                finally {
                    sbpStatus.Text = "";
                }
            }

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
				Rectangle box = file.BoundBoxes[id].Box;
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
            ((MapToolsLib.LayerPainters.MapLayerPainter)mapView1.Layers["Map"]).ShaderProxy.DrawBorders = cbBorders.Checked;
			mapView1.RefreshMap();
		}

		private void cbGrid_CheckedChanged(object sender, System.EventArgs e) {
			mapView1.DrawCoordinates = cbCoordinates.Checked;
		}

		private void mapView1_SelectionChanged(object sender, System.EventArgs e) {
			if ( (mapView1.Selection.Width == 0 && mapView1.Selection.Height == 0) ) {
				sbpStatus.Text = "";
			}
			else {
				sbpStatus.Text = String.Format( "Selection (x,y,w,h): {0},{1},{2},{3}", mapView1.Selection.X, mapView1.Selection.Y, mapView1.Selection.Width, mapView1.Selection.Height );
			}
		}

		private void mapView1_ContextMenu(object sender, EU2MapEditorControls.ContextMenuEventArgs e) {
			if ( file == null ) return;

			menuItem1.Enabled = (mapView1.Selection.Width >= 0 && mapView1.Selection.Height >= 0);
			miVisBlock.Enabled = e.BlockIndex >= 0;
			cmblockindex = e.BlockIndex;
			cmMap.Show( mapView1, new Point( e.X, e.Y ) );
		}

		private void menuItem1_Click(object sender, System.EventArgs e) {
			Clipboard.SetDataObject( String.Format( 
				"{5} /e \"{4}\" /r:{0},{1},{2},{3} /1", 
				mapView1.Selection.X, mapView1.Selection.Y, mapView1.Selection.Width, mapView1.Selection.Height,
                fileLabel.ToolTipText, EU2.Data.Province.MaxValue > 2000 ? "mimage" : "mximage"), true);
			MessageBox.Show( this, "Done!", "Success" );
		}

		private void miVisBlock_Click(object sender, System.EventArgs e) {
           MapToolsLib.LayerPainters.MapLayerPainter painter = ((MapToolsLib.LayerPainters.MapLayerPainter)mapView1.Layers["Map"]);

            //MapToolsLib.BlockVisualiser visualiser = new MapToolsLib.BlockVisualiser( file.ColorScales, file.Provinces );
            //visualiser.WalkTree( painter[painter.CurrentSource].GetDecompressedBlock( cmblockindex ) );
			
            //BlockVisForm form = new BlockVisForm();
            //form.Visuals = visualiser.Result;
            //form.ShowDialog( this );

            BlockForm form = new BlockForm();
            form.Source = file;
            form.BlockIndex = cmblockindex;
            form.Lightmap = painter.CurrentSource;
            form.ShowDialog(this);
        }

		private int useMap = 1;

        private void ReloadButton_Click(object sender, EventArgs e) {
            LoadFile();
        }

        private void ScreenshotButton_Click(object sender, EventArgs e) {
            string file = "Shot-" + DateTime.Now.ToString("ddMMyyyyhhmmss") + ".png";
            mapView1.GetImage().Save(file, System.Drawing.Imaging.ImageFormat.Png);

            MessageBox.Show(this, "Screenshot saved to '" + file + "'...", "Export successful", MessageBoxButtons.OK);
        }

		private void rbGridNone_CheckedChanged(object sender, System.EventArgs e) {
			if (((RadioButton)sender).Checked) mapView1.GridMode = MapGridMode.None;
		}

		private void rbGridNear_CheckedChanged(object sender, System.EventArgs e) {
			if (((RadioButton)sender).Checked) mapView1.GridMode = MapGridMode.Near;
		}

		private void rbGridFull_CheckedChanged(object sender, System.EventArgs e) {
			if (((RadioButton)sender).Checked) mapView1.GridMode = MapGridMode.Full;
		}

		private void sbStatus_PanelClick(object sender, System.Windows.Forms.StatusBarPanelClickEventArgs e) {
		
		}

		private void groupBox3_Enter(object sender, System.EventArgs e) {
		
		}

        private void DrawRegionsCheckbox_CheckedChanged(object sender, EventArgs e) {
            RegionCheckBoxesChanged();
        }

        private void RegionOverlapCheckbox_CheckedChanged(object sender, EventArgs e) {
            RegionCheckBoxesChanged();
        }

        private void RegionCheckBoxesChanged() {
            RegionOverlapCheckbox.Enabled = DrawRegionsCheckbox.Checked;

            if (DrawRegionsCheckbox.Checked) {
                mapView1.DrawRegionBorders = RegionOverlapCheckbox.Checked ? RegionDrawMode.BordersAndOverlap : RegionDrawMode.BordersOnly;
            }
            else
                mapView1.DrawRegionBorders = RegionDrawMode.None;
        }

        private void MapNormalButton_Click(object sender, EventArgs e) {
            MapToolsLib.LayerPainters.MapLayerPainter layer = (MapToolsLib.LayerPainters.MapLayerPainter)mapView1.Layers["Map"];

            if (file.ColorScales != null && file.Provinces != null) {
                try {
                    file.ColorScales.CalculateShades();
                    layer.ShaderProxy = new ColoredShaderProxy(file.ColorScales, file.Provinces);
                }
                catch (DivideByZeroException) {
                    layer.ShaderProxy = new DefaultShaderProxy();
                }
            }
            else
                layer.ShaderProxy = new DefaultShaderProxy();
            layer.ShaderProxy.DrawBorders = cbBorders.Checked;
            CheckDisplayButton(sender as ToolStripButton);
        }

        private void MapIDsLightmapButton_Click(object sender, EventArgs e) {
            MapToolsLib.LayerPainters.MapLayerPainter layer = (MapToolsLib.LayerPainters.MapLayerPainter)mapView1.Layers["Map"];
            layer.ShaderProxy = new GenericShaderProxy(new IDShader(new MapToolsLib.Inferis2IDConvertor()));
            CheckDisplayButton(sender as ToolStripButton);
        }

        private void MapIDsFileButton_Click(object sender, EventArgs e) {
            MapToolsLib.LayerPainters.MapLayerPainter layer = (MapToolsLib.LayerPainters.MapLayerPainter)mapView1.Layers["Map"];
            layer.ShaderProxy = new GenericShaderProxy(new IDMapShader(file.IDMap, new MapToolsLib.Inferis2IDConvertor()));
            CheckDisplayButton(sender as ToolStripButton);
        }

        private void MapIDsDiffButton_Click(object sender, EventArgs e) {
            MapToolsLib.LayerPainters.MapLayerPainter layer = (MapToolsLib.LayerPainters.MapLayerPainter)mapView1.Layers["Map"];
            layer.ShaderProxy = new GenericShaderProxy(new IDMapShader(file.IDMap, true));
            CheckDisplayButton(sender as ToolStripButton);
        }

        private void CheckDisplayButton(ToolStripButton current) {
            foreach ( ToolStripButton button in new ToolStripButton[] { MapNormalButton, MapIDsLightmapButton, MapIDsFileButton, MapIDsDiffButton}) {
                button.Checked = button == current;
            }
        }

        private void overlayProvinceCheckBox_CheckedChanged(object sender, EventArgs e) {
            mapView1.Layers["Province"].Enabled = overlayProvinceCheckBox.Checked;
            mapView1.Invalidate();
        }

        private void RefreshButton_Click(object sender, EventArgs e) {
            mapView1.RefreshMap();
        }

        private void CheckZoomButton(ToolStripButton current) {
            foreach (ToolStripButton button in new ToolStripButton[] { Zoom1Button, Zoom2Button, Zoom3Button }) {
                button.Checked = button == current;
            }
        }

        private void SetZoom(int zoom) {
            MapToolsLib.LayerPainters.MapLayerPainter layer = (MapToolsLib.LayerPainters.MapLayerPainter)mapView1.Layers["Map"];
            layer.CurrentSource = zoom;
            mapView1.LightmapDimensions = layer[layer.CurrentSource];
            mapView1.RefreshMap();
            pbMini.Refresh();

            MapIDsFileButton.Enabled = zoom == 1;
            MapIDsDiffButton.Enabled = zoom == 1;
        }

        private void Zoom1Button_Click(object sender, EventArgs e) {
            CheckZoomButton(sender as ToolStripButton);
            SetZoom(1);
        }

        private void Zoom2Button_Click(object sender, EventArgs e) {
            CheckZoomButton(sender as ToolStripButton);
            SetZoom(2);
        }

        private void Zoom3Button_Click(object sender, EventArgs e) {
            CheckZoomButton(sender as ToolStripButton);
            SetZoom(3);
        }


	}
}
