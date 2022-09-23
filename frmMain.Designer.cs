namespace snippet_manager
{
    partial class frmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.tsNew = new System.Windows.Forms.ToolStripButton();
            this.tsSave = new System.Windows.Forms.ToolStripButton();
            this.tsDelete = new System.Windows.Forms.ToolStripButton();
            this.tsPrint = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsReload = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsSearch = new System.Windows.Forms.ToolStripButton();
            this.tsFind = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsCut = new System.Windows.Forms.ToolStripButton();
            this.tsCopy = new System.Windows.Forms.ToolStripButton();
            this.tsPaste = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsUndo = new System.Windows.Forms.ToolStripButton();
            this.tsRedo = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.tsUpload = new System.Windows.Forms.ToolStripButton();
            this.tsDownload = new System.Windows.Forms.ToolStripButton();
            this.tsSync = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.tsSettings = new System.Windows.Forms.ToolStripButton();
            this.tsHelp = new System.Windows.Forms.ToolStripButton();
            this.tsExit = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.treeView = new snippet_manager.Controls.BufferedTreeView();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.msDeleteSnippet = new System.Windows.Forms.ToolStripMenuItem();
            this.msCreateSnippet = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.keywordTextBox = new System.Windows.Forms.RichTextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.replaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.importTextBox = new System.Windows.Forms.RichTextBox();
            this.lblNewSnippet = new System.Windows.Forms.Label();
            this.syntaxTextBox = new Alsing.Windows.Forms.SyntaxBoxControl();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2,
            this.toolStripStatusLabel3});
            this.statusStrip1.Location = new System.Drawing.Point(0, 663);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(867, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(616, 17);
            this.toolStripStatusLabel2.Spring = true;
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel3.Text = "toolStripStatusLabel3";
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsNew,
            this.tsSave,
            this.tsDelete,
            this.tsPrint,
            this.toolStripSeparator1,
            this.tsReload,
            this.toolStripSeparator2,
            this.tsSearch,
            this.tsFind,
            this.toolStripSeparator3,
            this.tsCut,
            this.tsCopy,
            this.tsPaste,
            this.toolStripSeparator4,
            this.tsUndo,
            this.tsRedo,
            this.toolStripSeparator5,
            this.tsUpload,
            this.tsDownload,
            this.tsSync,
            this.toolStripSeparator6,
            this.tsSettings,
            this.tsHelp,
            this.tsExit});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.toolStrip1.Size = new System.Drawing.Size(867, 27);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // tsNew
            // 
            this.tsNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsNew.Image = global::snippet_manager.Properties.Resources._new;
            this.tsNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsNew.Name = "tsNew";
            this.tsNew.Size = new System.Drawing.Size(24, 24);
            this.tsNew.Text = "toolStripButton1";
            this.tsNew.ToolTipText = "Create New Snippet";
            this.tsNew.Click += new System.EventHandler(this.tsNew_Click);
            // 
            // tsSave
            // 
            this.tsSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsSave.Image = global::snippet_manager.Properties.Resources.save;
            this.tsSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsSave.Name = "tsSave";
            this.tsSave.Size = new System.Drawing.Size(24, 24);
            this.tsSave.Text = "toolStripButton2";
            this.tsSave.ToolTipText = "Save Snippet";
            this.tsSave.Click += new System.EventHandler(this.tsSave_Click);
            // 
            // tsDelete
            // 
            this.tsDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsDelete.Image = global::snippet_manager.Properties.Resources.delete;
            this.tsDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsDelete.Name = "tsDelete";
            this.tsDelete.Size = new System.Drawing.Size(24, 24);
            this.tsDelete.Text = "toolStripButton3";
            this.tsDelete.ToolTipText = "Delete";
            this.tsDelete.Click += new System.EventHandler(this.tsDelete_Click);
            // 
            // tsPrint
            // 
            this.tsPrint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsPrint.Image = global::snippet_manager.Properties.Resources.print;
            this.tsPrint.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsPrint.Name = "tsPrint";
            this.tsPrint.Size = new System.Drawing.Size(24, 24);
            this.tsPrint.Text = "toolStripButton4";
            this.tsPrint.ToolTipText = "Print";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // tsReload
            // 
            this.tsReload.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsReload.Image = global::snippet_manager.Properties.Resources.reload;
            this.tsReload.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsReload.Name = "tsReload";
            this.tsReload.Size = new System.Drawing.Size(24, 24);
            this.tsReload.Text = "toolStripButton5";
            this.tsReload.ToolTipText = "Reload";
            this.tsReload.Click += new System.EventHandler(this.tsReload_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // tsSearch
            // 
            this.tsSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsSearch.Image = global::snippet_manager.Properties.Resources.search;
            this.tsSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsSearch.Name = "tsSearch";
            this.tsSearch.Size = new System.Drawing.Size(24, 24);
            this.tsSearch.Text = "toolStripButton6";
            this.tsSearch.ToolTipText = "Search";
            this.tsSearch.Visible = false;
            this.tsSearch.Click += new System.EventHandler(this.tsSearch_Click);
            // 
            // tsFind
            // 
            this.tsFind.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsFind.Image = global::snippet_manager.Properties.Resources.find;
            this.tsFind.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsFind.Name = "tsFind";
            this.tsFind.Size = new System.Drawing.Size(24, 24);
            this.tsFind.Text = "toolStripButton7";
            this.tsFind.ToolTipText = "Find";
            this.tsFind.Visible = false;
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 27);
            this.toolStripSeparator3.Visible = false;
            // 
            // tsCut
            // 
            this.tsCut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsCut.Image = global::snippet_manager.Properties.Resources.cut;
            this.tsCut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsCut.Name = "tsCut";
            this.tsCut.Size = new System.Drawing.Size(24, 24);
            this.tsCut.Text = "toolStripButton8";
            this.tsCut.ToolTipText = "Cut";
            this.tsCut.Visible = false;
            // 
            // tsCopy
            // 
            this.tsCopy.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsCopy.Image = global::snippet_manager.Properties.Resources.copy;
            this.tsCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsCopy.Name = "tsCopy";
            this.tsCopy.Size = new System.Drawing.Size(24, 24);
            this.tsCopy.Text = "toolStripButton9";
            this.tsCopy.ToolTipText = "Copy";
            this.tsCopy.Visible = false;
            // 
            // tsPaste
            // 
            this.tsPaste.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsPaste.Image = global::snippet_manager.Properties.Resources.paste;
            this.tsPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsPaste.Name = "tsPaste";
            this.tsPaste.Size = new System.Drawing.Size(24, 24);
            this.tsPaste.Text = "toolStripButton10";
            this.tsPaste.ToolTipText = "Paste";
            this.tsPaste.Visible = false;
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 27);
            this.toolStripSeparator4.Visible = false;
            // 
            // tsUndo
            // 
            this.tsUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsUndo.Image = global::snippet_manager.Properties.Resources.undo;
            this.tsUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsUndo.Name = "tsUndo";
            this.tsUndo.Size = new System.Drawing.Size(24, 24);
            this.tsUndo.Text = "toolStripButton11";
            this.tsUndo.ToolTipText = "Undo";
            this.tsUndo.Visible = false;
            // 
            // tsRedo
            // 
            this.tsRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsRedo.Image = global::snippet_manager.Properties.Resources.redo;
            this.tsRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsRedo.Name = "tsRedo";
            this.tsRedo.Size = new System.Drawing.Size(24, 24);
            this.tsRedo.Text = "toolStripButton12";
            this.tsRedo.ToolTipText = "Redo";
            this.tsRedo.Visible = false;
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 27);
            this.toolStripSeparator5.Visible = false;
            // 
            // tsUpload
            // 
            this.tsUpload.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsUpload.Image = global::snippet_manager.Properties.Resources.upload;
            this.tsUpload.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsUpload.Name = "tsUpload";
            this.tsUpload.Size = new System.Drawing.Size(24, 24);
            this.tsUpload.Text = "toolStripButton13";
            this.tsUpload.ToolTipText = "Upload";
            this.tsUpload.Visible = false;
            // 
            // tsDownload
            // 
            this.tsDownload.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsDownload.Image = global::snippet_manager.Properties.Resources.download;
            this.tsDownload.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsDownload.Name = "tsDownload";
            this.tsDownload.Size = new System.Drawing.Size(24, 24);
            this.tsDownload.Text = "toolStripButton14";
            this.tsDownload.ToolTipText = "Sync";
            this.tsDownload.Visible = false;
            // 
            // tsSync
            // 
            this.tsSync.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsSync.Image = global::snippet_manager.Properties.Resources.sync;
            this.tsSync.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsSync.Name = "tsSync";
            this.tsSync.Size = new System.Drawing.Size(24, 24);
            this.tsSync.Text = "toolStripButton15";
            this.tsSync.ToolTipText = "Download";
            this.tsSync.Visible = false;
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 27);
            this.toolStripSeparator6.Visible = false;
            // 
            // tsSettings
            // 
            this.tsSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsSettings.Image = global::snippet_manager.Properties.Resources.settings;
            this.tsSettings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsSettings.Name = "tsSettings";
            this.tsSettings.Size = new System.Drawing.Size(24, 24);
            this.tsSettings.Text = "toolStripButton16";
            this.tsSettings.ToolTipText = "Settings";
            this.tsSettings.Click += new System.EventHandler(this.tsSettings_Click);
            // 
            // tsHelp
            // 
            this.tsHelp.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsHelp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsHelp.Image = global::snippet_manager.Properties.Resources.help;
            this.tsHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsHelp.Name = "tsHelp";
            this.tsHelp.Size = new System.Drawing.Size(24, 24);
            this.tsHelp.Text = "toolStripButton18";
            this.tsHelp.ToolTipText = "Help";
            // 
            // tsExit
            // 
            this.tsExit.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.tsExit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsExit.Image = global::snippet_manager.Properties.Resources.exit;
            this.tsExit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsExit.Name = "tsExit";
            this.tsExit.Size = new System.Drawing.Size(24, 24);
            this.tsExit.Text = "toolStripButton17";
            this.tsExit.ToolTipText = "Exit";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(4, 30);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel1MinSize = 200;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lblNewSnippet);
            this.splitContainer1.Panel2.Controls.Add(this.syntaxTextBox);
            this.splitContainer1.Size = new System.Drawing.Size(859, 630);
            this.splitContainer1.SplitterDistance = 200;
            this.splitContainer1.TabIndex = 2;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.IsSplitterFixed = true;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.treeView);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer2.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer2.Size = new System.Drawing.Size(200, 630);
            this.splitContainer2.SplitterDistance = 366;
            this.splitContainer2.TabIndex = 0;
            // 
            // treeView
            // 
            this.treeView.ContextMenuStrip = this.contextMenuStrip2;
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Name = "treeView";
            this.treeView.PreviousSelectedNode = null;
            this.treeView.Size = new System.Drawing.Size(200, 366);
            this.treeView.TabIndex = 0;
            this.treeView.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView1_AfterLabelEdit);
            this.treeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeSelect);
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            this.treeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseClick);
            this.treeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyDown);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.msDeleteSnippet,
            this.msCreateSnippet});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(179, 48);
            // 
            // msDeleteSnippet
            // 
            this.msDeleteSnippet.Name = "msDeleteSnippet";
            this.msDeleteSnippet.Size = new System.Drawing.Size(178, 22);
            this.msDeleteSnippet.Text = "Delete Snippet";
            // 
            // msCreateSnippet
            // 
            this.msCreateSnippet.Name = "msCreateSnippet";
            this.msCreateSnippet.Size = new System.Drawing.Size(178, 22);
            this.msCreateSnippet.Text = "Create New Snippet";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.keywordTextBox);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(200, 130);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Keywords";
            // 
            // keywordTextBox
            // 
            this.keywordTextBox.ContextMenuStrip = this.contextMenuStrip1;
            this.keywordTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.keywordTextBox.Location = new System.Drawing.Point(3, 19);
            this.keywordTextBox.Name = "keywordTextBox";
            this.keywordTextBox.Size = new System.Drawing.Size(194, 108);
            this.keywordTextBox.TabIndex = 2;
            this.keywordTextBox.Text = "";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.toolStripSeparator7,
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.toolStripSeparator8,
            this.replaceToolStripMenuItem,
            this.findToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(116, 170);
            // 
            // cutToolStripMenuItem
            // 
            this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
            this.cutToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.cutToolStripMenuItem.Text = "Cut";
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            // 
            // pasteToolStripMenuItem
            // 
            this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
            this.pasteToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.pasteToolStripMenuItem.Text = "Paste";
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(112, 6);
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.undoToolStripMenuItem.Text = "Undo";
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.redoToolStripMenuItem.Text = "Redo";
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(112, 6);
            // 
            // replaceToolStripMenuItem
            // 
            this.replaceToolStripMenuItem.Name = "replaceToolStripMenuItem";
            this.replaceToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.replaceToolStripMenuItem.Text = "Replace";
            // 
            // findToolStripMenuItem
            // 
            this.findToolStripMenuItem.Name = "findToolStripMenuItem";
            this.findToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.findToolStripMenuItem.Text = "Find";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.importTextBox);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox1.Location = new System.Drawing.Point(0, 130);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 130);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Imports";
            // 
            // importTextBox
            // 
            this.importTextBox.ContextMenuStrip = this.contextMenuStrip1;
            this.importTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.importTextBox.Location = new System.Drawing.Point(3, 19);
            this.importTextBox.Name = "importTextBox";
            this.importTextBox.Size = new System.Drawing.Size(194, 108);
            this.importTextBox.TabIndex = 1;
            this.importTextBox.Text = "";
            this.importTextBox.TextChanged += new System.EventHandler(this.syntaxBoxControl_TextChanged);
            // 
            // lblNewSnippet
            // 
            this.lblNewSnippet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewSnippet.AutoSize = true;
            this.lblNewSnippet.BackColor = System.Drawing.Color.White;
            this.lblNewSnippet.ForeColor = System.Drawing.Color.Red;
            this.lblNewSnippet.Location = new System.Drawing.Point(561, 3);
            this.lblNewSnippet.Name = "lblNewSnippet";
            this.lblNewSnippet.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblNewSnippet.Size = new System.Drawing.Size(74, 15);
            this.lblNewSnippet.TabIndex = 1;
            this.lblNewSnippet.Text = "New Snippet";
            this.lblNewSnippet.Visible = false;
            // 
            // syntaxTextBox
            // 
            this.syntaxTextBox.ActiveView = Alsing.Windows.Forms.ActiveView.BottomRight;
            this.syntaxTextBox.AutoListPosition = null;
            this.syntaxTextBox.AutoListSelectedText = "a123";
            this.syntaxTextBox.AutoListVisible = false;
            this.syntaxTextBox.BackColor = System.Drawing.Color.White;
            this.syntaxTextBox.BorderStyle = Alsing.Windows.Forms.BorderStyle.None;
            this.syntaxTextBox.ContextMenuStrip = this.contextMenuStrip1;
            this.syntaxTextBox.CopyAsRTF = false;
            this.syntaxTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.syntaxTextBox.FontName = "Courier new";
            this.syntaxTextBox.GutterMarginWidth = 0;
            this.syntaxTextBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.syntaxTextBox.InfoTipCount = 1;
            this.syntaxTextBox.InfoTipPosition = null;
            this.syntaxTextBox.InfoTipSelectedIndex = 1;
            this.syntaxTextBox.InfoTipVisible = false;
            this.syntaxTextBox.Location = new System.Drawing.Point(0, 0);
            this.syntaxTextBox.LockCursorUpdate = false;
            this.syntaxTextBox.Name = "syntaxTextBox";
            this.syntaxTextBox.ParseOnPaste = true;
            this.syntaxTextBox.ShowScopeIndicator = false;
            this.syntaxTextBox.Size = new System.Drawing.Size(655, 630);
            this.syntaxTextBox.SmoothScroll = false;
            this.syntaxTextBox.SplitviewH = -4;
            this.syntaxTextBox.SplitviewV = -4;
            this.syntaxTextBox.TabGuideColor = System.Drawing.Color.FromArgb(((int)(((byte)(233)))), ((int)(((byte)(233)))), ((int)(((byte)(233)))));
            this.syntaxTextBox.TabIndex = 0;
            this.syntaxTextBox.WhitespaceColor = System.Drawing.SystemColors.ControlDark;
            this.syntaxTextBox.TextChanged += new System.EventHandler(this.syntaxBoxControl_TextChanged);
            // 
            // printDialog1
            // 
            this.printDialog1.UseEXDialog = true;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(867, 685);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Snippet Manager";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyDown);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private StatusStrip statusStrip1;
        private ToolStrip toolStrip1;
        private ToolStripButton tsNew;
        private ToolStripButton tsSave;
        private ToolStripButton tsDelete;
        private ToolStripButton tsPrint;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton tsReload;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton tsSearch;
        private ToolStripButton tsFind;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripButton tsCut;
        private ToolStripButton tsCopy;
        private ToolStripButton tsPaste;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripButton tsUndo;
        private ToolStripButton tsRedo;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripButton tsUpload;
        private ToolStripButton tsDownload;
        private ToolStripButton tsSync;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripButton tsSettings;
        private ToolStripButton tsHelp;
        private ToolStripButton tsExit;
        private SplitContainer splitContainer1;
        private SplitContainer splitContainer2;
        private snippet_manager.Controls.BufferedTreeView treeView;
        private GroupBox groupBox2;
        private GroupBox groupBox1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel toolStripStatusLabel2;
        private ToolStripStatusLabel toolStripStatusLabel3;
        private Alsing.Windows.Forms.SyntaxBoxControl syntaxTextBox;
        private PrintDialog printDialog1;
        private ImageList imageList1;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem cutToolStripMenuItem;
        private ToolStripMenuItem copyToolStripMenuItem;
        private ToolStripMenuItem pasteToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripMenuItem undoToolStripMenuItem;
        private ToolStripMenuItem redoToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripMenuItem replaceToolStripMenuItem;
        private ToolStripMenuItem findToolStripMenuItem;
        private ToolStripMenuItem msDeleteSnippet;
        private ToolStripMenuItem msCreateSnippet;
        private RichTextBox importTextBox;
        private RichTextBox keywordTextBox;
        private ContextMenuStrip contextMenuStrip2;
        private Label lblNewSnippet;
    }
}