using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace MapExplorer
{
	/// <summary>
	/// Summary description for ExplorerForm.
	/// </summary>
	public class AdjacentiesForm : System.Windows.Forms.Form
	{
		private System.ComponentModel.IContainer components;

		public AdjacentiesForm( Workset source )
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			Source = source;
		}

		public Workset Source {
			get {
				return workset;
			}
			set {
				workset = value;
				if ( workset == null ) workset = new Workset();

				tvTable.Nodes.Clear();
				for ( int i=1; i<EU2.Data.Province.Count; ++i ) {
					TreeNode current = new TreeNode( i.ToString() + " - " + workset.File.Provinces[i].Name + " (" + workset.File.Provinces[i].Terrain.ToString() + ")" );
					tvTable.Nodes.Add( current );

					EU2.Map.Adjacent[] list = workset.File.AdjacencyTable[i];
					if ( list != null ) {
						for ( int j=0; j<list.Length; ++j ) {
							TreeNode sub = new TreeNode( list[j].ID.ToString() + " - " + workset.File.Provinces[list[j].ID].Name + " (" + workset.File.Provinces[list[j].ID].Terrain.ToString() + ")" );
							if ( list[j].Type == EU2.Map.AdjacencyType.River ) 
								sub.ForeColor = Color.Blue;
							else
								sub.ForeColor = Color.Brown;
							current.Nodes.Add( sub );
						}
					}
					else {
						TreeNode sub = new TreeNode( "no adjacencies" );
						sub.ForeColor = SystemColors.ControlDark;
						current.Nodes.Add( sub );
					}
				}
			}
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(AdjacentiesForm));
			this.ilTree = new System.Windows.Forms.ImageList(this.components);
			this.panel1 = new System.Windows.Forms.Panel();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.btnLookup = new System.Windows.Forms.Button();
			this.tvTable = new System.Windows.Forms.TreeView();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// ilTree
			// 
			this.ilTree.ImageSize = new System.Drawing.Size(16, 16);
			this.ilTree.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilTree.ImageStream")));
			this.ilTree.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.btnLookup);
			this.panel1.Controls.Add(this.textBox1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(428, 28);
			this.panel1.TabIndex = 0;
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(324, 4);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(68, 21);
			this.textBox1.TabIndex = 0;
			this.textBox1.Text = "textBox1";
			// 
			// btnLookup
			// 
			this.btnLookup.Font = new System.Drawing.Font("Wingdings", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(2)));
			this.btnLookup.Location = new System.Drawing.Point(396, 4);
			this.btnLookup.Name = "btnLookup";
			this.btnLookup.Size = new System.Drawing.Size(24, 21);
			this.btnLookup.TabIndex = 1;
			this.btnLookup.Text = "è";
			// 
			// tvTable
			// 
			this.tvTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tvTable.ImageIndex = -1;
			this.tvTable.Location = new System.Drawing.Point(0, 28);
			this.tvTable.Name = "tvTable";
			this.tvTable.SelectedImageIndex = -1;
			this.tvTable.Size = new System.Drawing.Size(428, 279);
			this.tvTable.TabIndex = 1;
			// 
			// AdjacentiesForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(428, 307);
			this.Controls.Add(this.tvTable);
			this.Controls.Add(this.panel1);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Name = "AdjacentiesForm";
			this.Text = "Adjacency Table";
			this.Load += new System.EventHandler(this.AdjacentiesForm_Load);
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private System.Windows.Forms.ImageList ilTree;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Button btnLookup;
		private System.Windows.Forms.TreeView tvTable;


		private Workset workset;

		private void AdjacentiesForm_Load(object sender, System.EventArgs e) {
		
		}
	}
}
