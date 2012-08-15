using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Reflection;
using TD.SandDock;

namespace MapExplorer
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
		private TD.SandBar.ToolBarContainer leftSandBarDock;
		private TD.SandBar.ToolBarContainer rightSandBarDock;
		private TD.SandBar.ToolBarContainer bottomSandBarDock;
		private TD.SandBar.ToolBarContainer topSandBarDock;
		private TD.SandBar.MenuBar menuBar1;
		private TD.SandBar.MenuBarItem menuBarItem1;
		private TD.SandBar.MenuBarItem menuBarItem2;
		private TD.SandBar.MenuBarItem menuBarItem3;
		private TD.SandBar.MenuBarItem menuBarItem4;
		private TD.SandBar.MenuBarItem menuBarItem5;
		private TD.SandBar.ToolBar toolBar1;
		private TD.SandBar.MenuButtonItem mbiHelpAbout;
		private TD.SandBar.MenuButtonItem mbiFileExit;
		private TD.SandBar.MenuButtonItem mbiFileOpen;
		private TD.SandBar.MenuButtonItem mbiFileNew;
		private TD.SandBar.MenuButtonItem mbiFileSave;
		private TD.SandBar.MenuButtonItem mbiFileSaveAs;
		private TD.SandBar.MenuButtonItem mbiFileClose;
		private TD.SandBar.SandBarManager sbmBars;
		private System.Windows.Forms.OpenFileDialog ofdSelectFile;
		private TD.SandDock.DockContainer leftSandDock;
		private TD.SandDock.DockContainer bottomSandDock;
		private TD.SandDock.DockContainer topSandDock;
		private TD.SandDock.DockContainer rightSandDock;
		private TD.SandDock.SandDockManager sdmDocks;
		private TD.SandDock.DockControl dcExplorer;
		private System.Windows.Forms.TreeView tvContents;
		private TD.SandBar.ToolBar toolBar2;
		private TD.SandBar.ButtonItem biRefresh;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public MainForm()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MainForm));
			this.sbmBars = new TD.SandBar.SandBarManager();
			this.leftSandBarDock = new TD.SandBar.ToolBarContainer();
			this.rightSandBarDock = new TD.SandBar.ToolBarContainer();
			this.bottomSandBarDock = new TD.SandBar.ToolBarContainer();
			this.topSandBarDock = new TD.SandBar.ToolBarContainer();
			this.menuBar1 = new TD.SandBar.MenuBar();
			this.menuBarItem1 = new TD.SandBar.MenuBarItem();
			this.mbiFileNew = new TD.SandBar.MenuButtonItem();
			this.mbiFileOpen = new TD.SandBar.MenuButtonItem();
			this.mbiFileClose = new TD.SandBar.MenuButtonItem();
			this.mbiFileSave = new TD.SandBar.MenuButtonItem();
			this.mbiFileSaveAs = new TD.SandBar.MenuButtonItem();
			this.mbiFileExit = new TD.SandBar.MenuButtonItem();
			this.menuBarItem2 = new TD.SandBar.MenuBarItem();
			this.menuBarItem3 = new TD.SandBar.MenuBarItem();
			this.menuBarItem4 = new TD.SandBar.MenuBarItem();
			this.menuBarItem5 = new TD.SandBar.MenuBarItem();
			this.mbiHelpAbout = new TD.SandBar.MenuButtonItem();
			this.toolBar1 = new TD.SandBar.ToolBar();
			this.ofdSelectFile = new System.Windows.Forms.OpenFileDialog();
			this.sdmDocks = new TD.SandDock.SandDockManager();
			this.leftSandDock = new TD.SandDock.DockContainer();
			this.bottomSandDock = new TD.SandDock.DockContainer();
			this.topSandDock = new TD.SandDock.DockContainer();
			this.rightSandDock = new TD.SandDock.DockContainer();
			this.dcExplorer = new TD.SandDock.DockControl();
			this.tvContents = new System.Windows.Forms.TreeView();
			this.toolBar2 = new TD.SandBar.ToolBar();
			this.biRefresh = new TD.SandBar.ButtonItem();
			this.topSandBarDock.SuspendLayout();
			this.rightSandDock.SuspendLayout();
			this.dcExplorer.SuspendLayout();
			this.SuspendLayout();
			// 
			// sbmBars
			// 
			this.sbmBars.OwnerForm = this;
			// 
			// leftSandBarDock
			// 
			this.leftSandBarDock.Dock = System.Windows.Forms.DockStyle.Left;
			this.leftSandBarDock.Guid = new System.Guid("afce231c-b6c2-4e2e-878c-4d634b1fb218");
			this.leftSandBarDock.Location = new System.Drawing.Point(0, 43);
			this.leftSandBarDock.Manager = this.sbmBars;
			this.leftSandBarDock.Name = "leftSandBarDock";
			this.leftSandBarDock.Size = new System.Drawing.Size(0, 552);
			this.leftSandBarDock.TabIndex = 0;
			// 
			// rightSandBarDock
			// 
			this.rightSandBarDock.Dock = System.Windows.Forms.DockStyle.Right;
			this.rightSandBarDock.Guid = new System.Guid("c78bf1f0-0b19-4707-ba6b-117a611959d0");
			this.rightSandBarDock.Location = new System.Drawing.Point(892, 43);
			this.rightSandBarDock.Manager = this.sbmBars;
			this.rightSandBarDock.Name = "rightSandBarDock";
			this.rightSandBarDock.Size = new System.Drawing.Size(0, 552);
			this.rightSandBarDock.TabIndex = 1;
			// 
			// bottomSandBarDock
			// 
			this.bottomSandBarDock.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.bottomSandBarDock.Guid = new System.Guid("3a1f20aa-8337-4a26-953f-fca697dbf2b5");
			this.bottomSandBarDock.Location = new System.Drawing.Point(0, 595);
			this.bottomSandBarDock.Manager = this.sbmBars;
			this.bottomSandBarDock.Name = "bottomSandBarDock";
			this.bottomSandBarDock.Size = new System.Drawing.Size(892, 0);
			this.bottomSandBarDock.TabIndex = 2;
			// 
			// topSandBarDock
			// 
			this.topSandBarDock.Controls.Add(this.menuBar1);
			this.topSandBarDock.Controls.Add(this.toolBar1);
			this.topSandBarDock.Dock = System.Windows.Forms.DockStyle.Top;
			this.topSandBarDock.Guid = new System.Guid("a4256977-d6be-4b30-b25f-105f7cb24240");
			this.topSandBarDock.Location = new System.Drawing.Point(0, 0);
			this.topSandBarDock.Manager = this.sbmBars;
			this.topSandBarDock.Name = "topSandBarDock";
			this.topSandBarDock.Size = new System.Drawing.Size(892, 43);
			this.topSandBarDock.TabIndex = 3;
			// 
			// menuBar1
			// 
			this.menuBar1.Guid = new System.Guid("2553478c-d099-475b-be89-0bdfaefe94ba");
			this.menuBar1.Items.AddRange(new TD.SandBar.ToolbarItemBase[] {
																			  this.menuBarItem1,
																			  this.menuBarItem2,
																			  this.menuBarItem3,
																			  this.menuBarItem4,
																			  this.menuBarItem5});
			this.menuBar1.Location = new System.Drawing.Point(2, 0);
			this.menuBar1.Name = "menuBar1";
			this.menuBar1.OwnerForm = this;
			this.menuBar1.Size = new System.Drawing.Size(890, 25);
			this.menuBar1.TabIndex = 0;
			this.menuBar1.Text = "menuBar1";
			// 
			// menuBarItem1
			// 
			this.menuBarItem1.Items.AddRange(new TD.SandBar.ToolbarItemBase[] {
																				  this.mbiFileNew,
																				  this.mbiFileOpen,
																				  this.mbiFileClose,
																				  this.mbiFileSave,
																				  this.mbiFileSaveAs,
																				  this.mbiFileExit});
			this.menuBarItem1.Text = "&File";
			// 
			// mbiFileNew
			// 
			this.mbiFileNew.Shortcut = System.Windows.Forms.Shortcut.CtrlN;
			this.mbiFileNew.Text = "New";
			this.mbiFileNew.Activate += new System.EventHandler(this.mbiFileNew_Activate);
			// 
			// mbiFileOpen
			// 
			this.mbiFileOpen.Shortcut = System.Windows.Forms.Shortcut.CtrlO;
			this.mbiFileOpen.Text = "&Open...";
			this.mbiFileOpen.Activate += new System.EventHandler(this.mbiFileOpen_Activate);
			// 
			// mbiFileClose
			// 
			this.mbiFileClose.Text = "Close";
			this.mbiFileClose.Activate += new System.EventHandler(this.mbiFileClose_Activate);
			// 
			// mbiFileSave
			// 
			this.mbiFileSave.BeginGroup = true;
			this.mbiFileSave.Shortcut = System.Windows.Forms.Shortcut.CtrlS;
			this.mbiFileSave.Text = "&Save";
			this.mbiFileSave.Visible = false;
			// 
			// mbiFileSaveAs
			// 
			this.mbiFileSaveAs.Text = "Save &As...";
			this.mbiFileSaveAs.Visible = false;
			// 
			// mbiFileExit
			// 
			this.mbiFileExit.BeginGroup = true;
			this.mbiFileExit.Text = "E&xit";
			this.mbiFileExit.Activate += new System.EventHandler(this.mbiFileExit_Activate);
			// 
			// menuBarItem2
			// 
			this.menuBarItem2.Text = "&Edit";
			this.menuBarItem2.Visible = false;
			// 
			// menuBarItem3
			// 
			this.menuBarItem3.Text = "&View";
			this.menuBarItem3.Visible = false;
			// 
			// menuBarItem4
			// 
			this.menuBarItem4.MdiWindowList = true;
			this.menuBarItem4.Text = "&Window";
			// 
			// menuBarItem5
			// 
			this.menuBarItem5.Items.AddRange(new TD.SandBar.ToolbarItemBase[] {
																				  this.mbiHelpAbout});
			this.menuBarItem5.Text = "&Help";
			// 
			// mbiHelpAbout
			// 
			this.mbiHelpAbout.Text = "&About...";
			// 
			// toolBar1
			// 
			this.toolBar1.DockLine = 1;
			this.toolBar1.Guid = new System.Guid("9143a79b-d608-4d18-90f4-389e26f1c27a");
			this.toolBar1.Location = new System.Drawing.Point(2, 25);
			this.toolBar1.Name = "toolBar1";
			this.toolBar1.Size = new System.Drawing.Size(24, 18);
			this.toolBar1.TabIndex = 1;
			this.toolBar1.Text = "toolBar1";
			// 
			// ofdSelectFile
			// 
			this.ofdSelectFile.DefaultExt = "eu2map";
			// 
			// sdmDocks
			// 
			this.sdmDocks.DockingManager = TD.SandDock.DockingManager.Whidbey;
			this.sdmDocks.OwnerForm = this;
			// 
			// leftSandDock
			// 
			this.leftSandDock.Dock = System.Windows.Forms.DockStyle.Left;
			this.leftSandDock.Guid = new System.Guid("d38c2649-e16f-40ac-a442-a259d51fc390");
			this.leftSandDock.LayoutSystem = new TD.SandDock.SplitLayoutSystem(250, 400);
			this.leftSandDock.Location = new System.Drawing.Point(0, 43);
			this.leftSandDock.Manager = this.sdmDocks;
			this.leftSandDock.Name = "leftSandDock";
			this.leftSandDock.Size = new System.Drawing.Size(0, 552);
			this.leftSandDock.TabIndex = 9;
			// 
			// bottomSandDock
			// 
			this.bottomSandDock.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.bottomSandDock.Guid = new System.Guid("863a86b3-7faa-443e-8a15-fdbe1aa1f0eb");
			this.bottomSandDock.LayoutSystem = new TD.SandDock.SplitLayoutSystem(250, 400);
			this.bottomSandDock.Location = new System.Drawing.Point(0, 595);
			this.bottomSandDock.Manager = this.sdmDocks;
			this.bottomSandDock.Name = "bottomSandDock";
			this.bottomSandDock.Size = new System.Drawing.Size(892, 0);
			this.bottomSandDock.TabIndex = 11;
			// 
			// topSandDock
			// 
			this.topSandDock.Dock = System.Windows.Forms.DockStyle.Top;
			this.topSandDock.Guid = new System.Guid("132e5ede-a377-4df1-a798-e8e876b05a1f");
			this.topSandDock.LayoutSystem = new TD.SandDock.SplitLayoutSystem(250, 400);
			this.topSandDock.Location = new System.Drawing.Point(0, 43);
			this.topSandDock.Manager = this.sdmDocks;
			this.topSandDock.Name = "topSandDock";
			this.topSandDock.Size = new System.Drawing.Size(892, 0);
			this.topSandDock.TabIndex = 12;
			// 
			// rightSandDock
			// 
			this.rightSandDock.Controls.Add(this.dcExplorer);
			this.rightSandDock.Dock = System.Windows.Forms.DockStyle.Right;
			this.rightSandDock.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.rightSandDock.Guid = new System.Guid("5917f22a-97ca-4ca9-8eb9-30674bdd07a4");
			this.rightSandDock.LayoutSystem = new TD.SandDock.SplitLayoutSystem(250, 400, System.Windows.Forms.Orientation.Horizontal, new TD.SandDock.LayoutSystemBase[] {
																																											  new TD.SandDock.ControlLayoutSystem(250, 552, new TD.SandDock.DockControl[] {
																																																															  this.dcExplorer}, this.dcExplorer)});
			this.rightSandDock.Location = new System.Drawing.Point(638, 43);
			this.rightSandDock.Manager = this.sdmDocks;
			this.rightSandDock.Name = "rightSandDock";
			this.rightSandDock.Size = new System.Drawing.Size(254, 552);
			this.rightSandDock.TabIndex = 10;
			// 
			// dcExplorer
			// 
			this.dcExplorer.Controls.Add(this.tvContents);
			this.dcExplorer.Controls.Add(this.toolBar2);
			this.dcExplorer.Guid = new System.Guid("19f5c70c-c553-466a-a004-c7c26a52ec34");
			this.dcExplorer.Location = new System.Drawing.Point(4, 18);
			this.dcExplorer.Name = "dcExplorer";
			this.dcExplorer.Size = new System.Drawing.Size(250, 511);
			this.dcExplorer.TabImage = ((System.Drawing.Image)(resources.GetObject("dcExplorer.TabImage")));
			this.dcExplorer.TabIndex = 0;
			this.dcExplorer.Text = "Project Explorer";
			// 
			// tvContents
			// 
			this.tvContents.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvContents.ImageIndex = -1;
			this.tvContents.Location = new System.Drawing.Point(0, 24);
			this.tvContents.Name = "tvContents";
			this.tvContents.SelectedImageIndex = -1;
			this.tvContents.Size = new System.Drawing.Size(250, 487);
			this.tvContents.TabIndex = 3;
			// 
			// toolBar2
			// 
			this.toolBar2.Guid = new System.Guid("ad8c68f1-221e-4e23-9bdd-dc8e02d68020");
			this.toolBar2.Items.AddRange(new TD.SandBar.ToolbarItemBase[] {
																			  this.biRefresh});
			this.toolBar2.Location = new System.Drawing.Point(0, 0);
			this.toolBar2.Name = "toolBar2";
			this.toolBar2.Renderer = new TD.SandBar.Office2002Renderer();
			this.toolBar2.Size = new System.Drawing.Size(250, 24);
			this.toolBar2.TabIndex = 2;
			this.toolBar2.Text = "toolBar2";
			// 
			// biRefresh
			// 
			this.biRefresh.Image = ((System.Drawing.Image)(resources.GetObject("biRefresh.Image")));
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(892, 595);
			this.Controls.Add(this.leftSandDock);
			this.Controls.Add(this.rightSandDock);
			this.Controls.Add(this.bottomSandDock);
			this.Controls.Add(this.topSandDock);
			this.Controls.Add(this.leftSandBarDock);
			this.Controls.Add(this.rightSandBarDock);
			this.Controls.Add(this.bottomSandBarDock);
			this.Controls.Add(this.topSandBarDock);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.IsMdiContainer = true;
			this.Name = "MainForm";
			this.Text = "Map Explorer";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.topSandBarDock.ResumeLayout(false);
			this.rightSandDock.ResumeLayout(false);
			this.dcExplorer.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new MainForm());
		}

		public void HandleError( Exception e ) {
			MessageBox.Show( this, "An error occured.\n\n" + e.ToString() );
		}

		private bool GetRidOfOldWorkset() {
			if ( Workset.Current.FileModified ) {
				DialogResult choice = MessageBox.Show( this, 
					"You have unsaved changes to this map. Do you want to save the changes?",
					"Map still open", 
					MessageBoxButtons.YesNoCancel, 
					MessageBoxIcon.Question );
				
				if ( choice == DialogResult.Cancel ) return false;
				if ( choice == DialogResult.Yes ) Workset.Current.SaveFile();

				return true;
			}

			// Close all windows related to this workset
			if ( dcExplorer != null ) {
				BuildExplorer( null );
			}

			return true;
		}

		private void OpenWorkset() {
			// Build explorer tree
			BuildExplorer( Workset.Current );

			// open map window
//			MapForm map = new MapForm( );
//			map.MdiParent = this;
//			map.Show();
//			map.Source = Workset.Current;

			// open map window
			AdjacentiesForm adj = new AdjacentiesForm( Workset.Current );
			adj.MdiParent = this;
			adj.Show();
		}

		private void BuildExplorer( Workset workset ) {
			tvContents.Nodes.Clear();
			if ( workset == null || workset.File == null ) return;

			// -- add project node
			TreeNode root = new TreeNode( System.IO.Path.GetFileName( workset.FilePath ), 0, 0 ); 
			if ( root.Text.Length == 0 ) root.Text = "<new map>";
			tvContents.Nodes.Add( root );

			// -- add provinces node
			root = new TreeNode( "Provinces" );
			tvContents.Nodes[0].Nodes.Add( root );

			if ( workset.File.Provinces != null ) root.Nodes.Add( "Provinces" );

			// -- add map
			root = new TreeNode( "Maps" );
			tvContents.Nodes[0].Nodes.Add( root );

			if ( workset.File.Lightmap1 != null ) root.Nodes.Add( "Lightmap 1" );
			if ( workset.File.Lightmap2 != null ) root.Nodes.Add( "Lightmap 2" );
			if ( workset.File.Lightmap3 != null ) root.Nodes.Add( "Lightmap 3" );
			if ( workset.File.ColorScales != null ) root.Nodes.Add( "Colorscales" );

			root = new TreeNode( "Map Support" );
			tvContents.Nodes[0].Nodes.Add( root );

			if ( workset.File.IDMap != null ) root.Nodes.Add( "ID Map" );
			if ( workset.File.AdjacencyTable != null ) root.Nodes.Add( "Adjacency Table" );
			if ( workset.File.BoundBoxes != null ) root.Nodes.Add( "Boundboxes" );
			if ( workset.File.IncognitaGrid != null ) root.Nodes.Add( "Incognita Grid" );
			if ( workset.File.IDGrid != null ) root.Nodes.Add( "ID Grid" );
		}

		private void MainForm_Load(object sender, System.EventArgs e) {
			Text = Text + " (v" + Assembly.GetExecutingAssembly().GetName().Version.ToString() + ")";
			Workset.Current.LastOpenDirectory = @"D:\Projects\00.Personal\06.EU2Data.Net\Maptools\sandbox";
		}

		private void mbiFileNew_Activate(object sender, System.EventArgs e) {
			if ( !GetRidOfOldWorkset() ) return;

			Workset.Current.CreateFile();
			OpenWorkset();
		}

		private void mbiFileOpen_Activate(object sender, System.EventArgs e) {
			if ( !GetRidOfOldWorkset() ) return;

			ofdSelectFile.AddExtension = true;
			ofdSelectFile.Filter = "EU2Map Files (*.eu2map)|*.eu2map|All Files (*.*)|*.*";
			ofdSelectFile.Title = "Open Map File";
			ofdSelectFile.Multiselect = false;
			ofdSelectFile.ShowReadOnly = true;
			ofdSelectFile.InitialDirectory = Workset.Current.LastOpenDirectory;

			if ( ofdSelectFile.ShowDialog( this ).Equals( DialogResult.Cancel ) ) return;

			Application.DoEvents();
			string file = ofdSelectFile.FileName;
			Workset.Current.LastOpenDirectory = System.IO.Path.GetDirectoryName( file );
			try {
				Cursor = Cursors.WaitCursor;
				Application.DoEvents();
				Workset.Current.OpenFile( file );
				Cursor = Cursors.Default;
				Application.DoEvents();
			}
			catch ( Exception ex ) {
				HandleError( ex );
				return;
			}
			finally {
				Cursor = Cursors.Default;
			}

			if ( Workset.Current.File == null ) {
				MessageBox.Show( this, "Could not open file." );
				return;
			}

			OpenWorkset();
		}

		private void mbiFileClose_Activate(object sender, System.EventArgs e) {
			if ( !GetRidOfOldWorkset() ) return;
			Workset.Current.CloseFile();
		}

		private void mbiFileExit_Activate(object sender, System.EventArgs e) {
			Application.Exit();
		}
	}
}
