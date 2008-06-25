namespace NArrange.Gui.Configuration
{
	partial class ConfigurationEditorControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this._splitContainer = new System.Windows.Forms.SplitContainer();
			this._configurationTreeView = new System.Windows.Forms.TreeView();
			this._propertyGrid = new System.Windows.Forms.PropertyGrid();
			this._elementCollectionContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.elementToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.regionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.elementReferenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this._splitContainer.Panel1.SuspendLayout();
			this._splitContainer.Panel2.SuspendLayout();
			this._splitContainer.SuspendLayout();
			this._elementCollectionContextMenu.SuspendLayout();
			this.SuspendLayout();
			// 
			// _splitContainer
			// 
			this._splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this._splitContainer.Location = new System.Drawing.Point(0, 0);
			this._splitContainer.Name = "_splitContainer";
			// 
			// _splitContainer.Panel1
			// 
			this._splitContainer.Panel1.Controls.Add(this._configurationTreeView);
			this._splitContainer.Panel1.Padding = new System.Windows.Forms.Padding(5);
			// 
			// _splitContainer.Panel2
			// 
			this._splitContainer.Panel2.Controls.Add(this._propertyGrid);
			this._splitContainer.Panel2.Padding = new System.Windows.Forms.Padding(5);
			this._splitContainer.Size = new System.Drawing.Size(606, 319);
			this._splitContainer.SplitterDistance = 202;
			this._splitContainer.TabIndex = 0;
			// 
			// _configurationTreeView
			// 
			this._configurationTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._configurationTreeView.Location = new System.Drawing.Point(5, 5);
			this._configurationTreeView.Name = "_configurationTreeView";
			this._configurationTreeView.Size = new System.Drawing.Size(192, 309);
			this._configurationTreeView.TabIndex = 0;
			this._configurationTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.HandleTreeNodeSelect);
			this._configurationTreeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.HandleConfigurationTreeViewKeyDown);
			// 
			// _propertyGrid
			// 
			this._propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this._propertyGrid.Location = new System.Drawing.Point(5, 5);
			this._propertyGrid.Name = "_propertyGrid";
			this._propertyGrid.Size = new System.Drawing.Size(390, 309);
			this._propertyGrid.TabIndex = 1;
			this._propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.HandlePropertyGridPropertyValueChanged);
			// 
			// _elementCollectionContextMenu
			// 
			this._elementCollectionContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearToolStripMenuItem,
            this.addToolStripMenuItem});
			this._elementCollectionContextMenu.Name = "contextMenuStrip1";
			this._elementCollectionContextMenu.Size = new System.Drawing.Size(102, 48);
			// 
			// clearToolStripMenuItem
			// 
			this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
			this.clearToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
			this.clearToolStripMenuItem.Text = "Clear";
			// 
			// addToolStripMenuItem
			// 
			this.addToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.elementToolStripMenuItem,
            this.regionToolStripMenuItem,
            this.elementReferenceToolStripMenuItem});
			this.addToolStripMenuItem.Name = "addToolStripMenuItem";
			this.addToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
			this.addToolStripMenuItem.Text = "Add";
			// 
			// elementToolStripMenuItem
			// 
			this.elementToolStripMenuItem.Name = "elementToolStripMenuItem";
			this.elementToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
			this.elementToolStripMenuItem.Text = "Element";
			// 
			// regionToolStripMenuItem
			// 
			this.regionToolStripMenuItem.Name = "regionToolStripMenuItem";
			this.regionToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
			this.regionToolStripMenuItem.Text = "Region";
			// 
			// elementReferenceToolStripMenuItem
			// 
			this.elementReferenceToolStripMenuItem.Name = "elementReferenceToolStripMenuItem";
			this.elementReferenceToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
			this.elementReferenceToolStripMenuItem.Text = "Element Reference";
			// 
			// ConfigurationEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._splitContainer);
			this.Name = "ConfigurationEditorControl";
			this.Size = new System.Drawing.Size(606, 319);
			this._splitContainer.Panel1.ResumeLayout(false);
			this._splitContainer.Panel2.ResumeLayout(false);
			this._splitContainer.ResumeLayout(false);
			this._elementCollectionContextMenu.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer _splitContainer;
		private System.Windows.Forms.TreeView _configurationTreeView;
		private System.Windows.Forms.PropertyGrid _propertyGrid;
		private System.Windows.Forms.ContextMenuStrip _elementCollectionContextMenu;
		private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem addToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem elementToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem regionToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem elementReferenceToolStripMenuItem;
	}
}
