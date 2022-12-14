using Alsing.SourceCode;
using Alsing.Windows.Forms;
using snippet_manager.Common;
using snippet_manager.Controls;
using snippet_manager.Entities;
using snippet_manager.Models;
using snippet_manager.Services;
using snippet_manager.Views;
using SQLite;
using System.ComponentModel;
using System.Data;
using System.Drawing.Printing;
using System.Text.Json;

namespace snippet_manager
{
    public partial class frmMain : Form
    {
        private readonly Repository db;
        private Singleton instance = Singleton.Instance;

        private int _snippetCount = 0;
        public int SnippetCount 
        {
            get { return _snippetCount; } 
            set { _snippetCount = value; toolStripStatusLabel1.Text = $"{_snippetCount} snippets loaded"; } 
        }
        //private readonly FileSystemWatcher _syntaxFileWatcher;
        //private readonly string syntaxDirectory = Path.Combine(Application.StartupPath, "SyntaxFiles");
        private readonly string nodeFile = Path.Combine(Application.StartupPath, "id.json");
        private readonly string treeFile = Path.Combine(Application.StartupPath, "tree.json");

        private readonly ucSnippet uc;


        public frmMain()
        {
            InitializeComponent();

            db = new Repository();
            uc = new(db);

            Load += (s, e) => Initialize();
            FormClosing += (s, e) =>
            {
                SaveSnippet(treeView.SelectedNode);
                SaveTreeView();
                SaveDatabase();
                db.Close();
            };

            //_syntaxFileWatcher = new FileSystemWatcher(syntaxDirectory, "*.syn")
            //{
            //    EnableRaisingEvents = true,
            //    NotifyFilter = NotifyFilters.LastWrite
            //};
            //_syntaxFileWatcher.Changed += (s, e) =>
            //{
            //    _syntaxFileWatcher.EnableRaisingEvents = false;
            //    ReloadSyntaxFiles();
            //    _syntaxFileWatcher.EnableRaisingEvents = true;
            //};

            toolStripStatusLabel1.Text = null;
            toolStripStatusLabel2.Text = null;
            toolStripStatusLabel3.Text = null;

            treeView.TreeViewNodeSorter = new NodeSorter();
            treeView.ContextMenuStrip = null;

            if (instance.Config.TryGetValue("Settings", out var settings))
            {
                toolStrip1.ShowItemToolTips = settings["ShowTooltips"];
            }

            uc.txtTitle.TextChanged += syntaxBoxControl_TextChanged;
            uc.txtSummary.TextChanged += syntaxBoxControl_TextChanged;
            uc.txtKeywords.TextChanged += syntaxBoxControl_TextChanged;
            uc.txtVersion.TextChanged += syntaxBoxControl_TextChanged;
            uc.syntaxBoxControl1.TextChanged += syntaxBoxControl_TextChanged;
            uc.cmbGroup.SelectedValueChanged += comboBox_SelectedValueChanged;
            uc.cmbAuthor.SelectedValueChanged += comboBox_SelectedValueChanged;
            uc.cmbLanguage.SelectedValueChanged += comboBox_SelectedValueChanged;
            uc.cmbCategory.SelectedValueChanged += comboBox_SelectedValueChanged;
        }


        private void ClearUI()
        {
            uc.Clear();
        }
        private void Initialize()
        {
            InitialDatabase();
            ClearUI();
            PopulateTreeView();
            LoadTreeView();
            LoadLastSnippet();
        }
        public void InitialDatabase()
        {
            db.InsertOrUpdate(new _Group() { Description = "My Snippets" });
        }
        private void LoadLastSnippet()
        {
            if (!CheckPath(nodeFile, false))
            {
                return;
            }

            if (long.TryParse(File.ReadAllText(nodeFile), out long nodeId))
            {
                treeView.SelectedNode = treeView.Nodes.Find(nodeId.ToString(), true).SingleOrDefault();
            }
        }
        public bool PopulateTreeView()
        {
            List<_TreeviewItem> records = new();

            var treeNodeRoots = db.GetGroups();

            if (treeNodeRoots.Count > 0)
            {
                var roots = treeNodeRoots.Select(x => CreateNode(x.Description, null, 0)).ToArray();
                treeView.Nodes.AddRange(roots);
            }

            var snippets = db.GetSnippets();

            if (snippets.Count > 0)
            {
                records = snippets.Select(x => new _TreeviewItem
                {
                    Id = x.Id,
                    Author = x.Author,
                    AuthorId = x.AuthorId,
                    Category = x.Category,
                    CategoryId = x.CategoryId,
                    Code = x.Code,
                    Created = x.Created,
                    Group = x.Group,
                    GroupId = x.GroupId,
                    Summary = x.Summary,
                    Keywords = x.Keyword,
                    Language = x.Language,
                    LanguageId = x.LanguageId,
                    Title = x.Title,
                    Updated = x.Updated,
                    Version = x.Version
                }).ToList();
            }

            if (records is null || !records.Any())
            {
                return false;
            }

            IEnumerable<IGrouping<GroupRecord, _TreeviewItem>>? groupByKeys = from record in records
                                                                    group record by new GroupRecord(record.Group.Description, record.Language.Description, record.Category.Description) 
                                                                    into newRecords
                                                                    select newRecords ?? null;

            if (groupByKeys is null || !groupByKeys.Any())
            {
                return false;
            }

            foreach (IGrouping<GroupRecord, _TreeviewItem>? record in groupByKeys.OrderBy(_ => _.Key.language).ThenBy(_ => _.Key.category))
            {
                if (record is null || !record.Any())
                {
                    continue;
                }

                TreeNode? root = treeView.Nodes.Find(record.Key.group, false).SingleOrDefault() ?? CreateNode(groupByKeys?.FirstOrDefault()?.Key.group, null, 0);
                TreeNode? lang = CreateNode(record.Key.language, null, 1);
                TreeNode? cat = CreateNode(record.Key.category, null, 2);

                foreach (_TreeviewItem? code in record.OrderBy(_ => _.Group.Description))
                {
                    TreeNode? snippet = CreateNode(code?.Title, code?.Id.ToString(), 3, null, code);
                    cat?.Nodes.Add(snippet);
                    SnippetCount++;
                }

                lang?.Nodes.Add(cat);

                if (root is null)
                {
                    continue;
                }

                if (!root.Nodes.Find(record.Key.language, false).Any())
                {
                    root.Nodes.Add(lang);
                }
                else
                {
                    root?.Nodes?.Find(record.Key.language, false)?
                        .FirstOrDefault()?.Nodes.Add(cat);
                }

                if (!treeView.Nodes.Find(root?.Name, true).Any())
                {
                    treeView.Nodes.Add(root);
                }
            }

            return true;
        }
        private void LoadTreeView()
        {
            if (CheckPath(treeFile, false))
            {
                string data = File.ReadAllText(treeFile);

                if (string.IsNullOrEmpty(data))
                {
                    return;
                }

                List<bool>? nodeStatus = JsonSerializer.Deserialize<List<bool>>(data) ?? null;

                if (nodeStatus == null || nodeStatus.Count == 0)
                { 
                    return;
                }

                IEnumerable<TreeNode>? treeNodes = treeView.SearchTree();

                if (nodeStatus.Count != treeNodes.Count())
                {
                    return;
                }

                int i = 0;

                foreach (TreeNode? node in treeNodes)
                {
                    if (nodeStatus[i++] == true)
                    {
                        node.Expand();
                    }
                    else
                    {
                        node.Collapse();
                    }
                }
            }
        }
        private void SaveTreeView()
        {
            if (!CheckPath(treeFile, false))
            {
                return;
            }

            List<bool>? nodes = treeView.SearchTree()
                .Select(node => node.IsExpanded)
                .ToList();

            File.WriteAllText(treeFile, JsonSerializer.Serialize(nodes));
        }
        private void ReloadDatabases()
        {
            treeView.BeginUpdate();

            SnippetCount = 0;

            SaveSnippet(treeView.SelectedNode);

            SaveDatabase();

            SaveTreeView();

            ClearUI();

            treeView.Nodes.Clear();

            PopulateTreeView();

            LoadTreeView();

            LoadLastSnippet();

            DisplayMessage("Databases reloaded successfuly", toolStripStatusLabel3);

            treeView.EndUpdate();
        }
       
        private void SaveDatabase()
        {
            foreach (TreeNode node in treeView.Nodes)
            {
                List<_TreeviewItem?> snippets = node.Nodes.SearchTree()
                    .Where(x => x.Tag is not null)
                    .Select(x => x.Tag as _TreeviewItem).ToList();

                if (snippets is null || !snippets.Any())
                {
                    continue;
                }

                foreach (var snippet in snippets)
                {
                    if (snippet is null)
                    {
                        continue;
                    }

                    db.InsertOrUpdate(snippet);
                }
            }

            DisplayMessage("Snippets saved successfully", toolStripStatusLabel3);
        }
        private void PrintSnippet()
        {
            PrintDocument printDoc = new()
            {
                DocumentName = treeView.SelectedNode.Text
            };

            printDialog1 = new PrintDialog
            {
                Document = printDoc,
                AllowSelection = true,
                AllowSomePages = true,
                AllowPrintToFile = true
            };

            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                printDoc.PrintPage += (s, e) =>
                {
                    var snippet = treeView.SelectedNode.Tag as _TreeviewItem;

                    string printString = snippet.Title + Environment.NewLine + snippet.Summary + Environment.NewLine + Environment.NewLine + Environment.NewLine + snippet.Code;
                    e?.Graphics?.DrawString(printString, new Font("Arial", 12), Brushes.Black, (float)e.MarginBounds.X, (float)e.MarginBounds.Y);
                };
                printDoc.Print();
            }
        }
        private void NewSnippet()
        {      
            SaveSnippet(treeView.SelectedNode);
            ClearUI();
            treeView.PreviousSelectedNode = null;
            treeView.SelectedNode = null;

            uc.NewMode();
            uc.ResetChanges();
            SetRights(null);
        }
        private void DisplaySnippet(TreeNode? node)
        {
            if (node?.Tag is null || node.Tag is not _TreeviewItem snippet)
            {
                return; 
            }

            ClearUI();

            // TODO: ADD SUPPORT FOR DIFFERENT SNIPPET VIEW
            if (splitContainer1.Panel2.Controls.Count == 0)
            {
                splitContainer1.Panel2.Controls.Add(uc);
            }

            uc.ViewMode(snippet);

            uc.Dock = DockStyle.Fill;
        }
        private void SaveNodeId(TreeNode? node)
        {
            if (node?.Tag is null || node.Tag is not _TreeviewItem snippet)
            {
                return;
            }

            if (CheckPath(nodeFile, false))
            {
                File.WriteAllText(nodeFile, snippet?.Id.ToString());
            }
        }
      
        private void SetRights(TreeNode? node)
        {
            tsPrint.Enabled =
            tsDelete.Enabled =
            tsFind.Enabled =
            tsCut.Enabled =
            tsPaste.Enabled =
            tsCopy.Enabled =
            tsRedo.Enabled =
            tsUndo.Enabled =
            tsSave.Enabled =
            tsEdit.Enabled = false;

            if (node is null || node?.Level == 0 || node?.Level == 1 || node?.Level == 2)
            {
                return;
            }

            tsDelete.Enabled = true;
            tsPrint.Enabled = true;
            tsEdit.Enabled = true;
        }
        private void SaveSnippet(TreeNode? node, bool prompt = true)
        {
            using DialogCenteringService centeringService = new(this);

            if (!uc.CheckForChanges() || prompt && MessageBox.Show($"Do you want to save changes to {node?.Text}?", "Save Changes", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                tsSave.Enabled = uc.ResetChanges();
                return;
            }

            if (node is null)
            {
                var newSnippet = uc.InsertMode();

                if (newSnippet is null)
                {
                    return;
                }

                TreeNode? root = treeView.Nodes.Find(newSnippet?.Group?.Description, false).FirstOrDefault();
                TreeNode? lan = root?.Nodes.Find(newSnippet?.Language?.Description, true).SingleOrDefault() ?? CreateNode(newSnippet?.Language?.Description, null, 1);
                TreeNode? cat = lan?.Nodes.Find(newSnippet?.Category?.Description, true).SingleOrDefault() ?? CreateNode(newSnippet?.Category?.Description, null, 1);
                TreeNode? snippet = CreateNode(newSnippet?.Title, newSnippet?.Id.ToString(), 2, 3, newSnippet);

                if (snippet is null)
                {
                    return;
                }

                cat?.Nodes.Add(snippet);

                if (lan?.Nodes.Find(newSnippet?.Category?.Description, true).Length == 0)
                {
                    lan?.Nodes.Add(cat);
                }

                if (root?.Nodes.Find(newSnippet?.Language?.Description, true).Length == 0)
                {
                    root?.Nodes.Add(lan);
                }

                node = snippet;
            }
            else
            {
                if (node?.Tag is not _TreeviewItem snippet)
                {
                    MessageBox.Show($"No valid node selected, unable to update");
                    return;
                }

                if (!snippet?.Group?.Description?.ToLower()?.Equals("My Snippets"?.ToLower()) ?? false)
                {
                    MessageBox.Show(this, $"You are attempting to modify a snippet that is not in your\r\n\\My Snippets folder.\r\nYou may lose your modifications if you download snippet updates in the future.", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                }

                uc.UpdateMode(snippet);

                SaveDatabase();
            }

            tsSave.Enabled = uc.ResetChanges();

            ReloadDatabases();

            LoadTreeView();
            LoadLastSnippet();

            treeView.SelectedNode = node;
            treeView.Select();
        }
        private void DeleteSnippet(TreeNode? node)
        {
            if (node is null)
            {
                return;
            }

            using DialogCenteringService centeringService = new(this);
            
            instance.Config.TryGetValue("Settings", out var settings);

            switch (node?.Level)
            {
                case 0:
                case 1:
                    MessageBox.Show(this, "Unable to delete this level", "Delete", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
                    return;
                case 2:
                    {
                        if (node.Nodes.Count > 0)
                        {
                            if (MessageBox.Show(this, "You are about to delete all snippets within this category\r\n\r\nAre you sure you want to continue?", "Confirm", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1) == DialogResult.Cancel)
                                return;
                        }

                        var source = node.Nodes.SearchTree()
                            .Where(x => x.Tag is not null)
                            .Select(x => x.Tag as _TreeviewItem)
                            .ToList();

                        foreach (var snippet in source)
                        {
                            db.DeleteSnippetById(snippet.Id);
                        }

                        TreeNode? indexNode = node;
                        TreeNode? indexParentNode = node?.Parent;

                        treeView.Nodes.Remove(node);

                        if (settings is not null && settings["DeleteEmptyNodes"])
                        {
                            if (indexParentNode is not null && indexParentNode?.Nodes?.Count == 0)
                            {
                                treeView.Nodes.Remove(indexParentNode);
                            }
                        }

                        return;
                    }
                // Snippet
                default:
                    if (MessageBox.Show(this, $"Delete Snippet {node?.Text}?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
                        return;

                    break;
            }

            var snippetId = (node?.Tag as _TreeviewItem)?.Id ?? null;

            if (snippetId is not null && snippetId.HasValue)
            {
                db.DeleteSnippetById(snippetId.Value);
            }

            TreeNode? index = node;
            TreeNode? parent = index?.Parent;
            TreeNode? parentparent = index?.Parent?.Parent;
            TreeNode? nextNode = index?.PrevNode ?? index?.NextNode ?? parent?.FirstNode;
            treeView.Nodes.Remove(index);

            if (settings is not null && settings["DeleteEmptyNodes"])
            {
                if (parent is not null && parent.Nodes.Count == 0)
                {
                    treeView.Nodes.Remove(parent);
                }

                if (parentparent != null && parentparent.Nodes.Count == 0)
                {
                    treeView.Nodes.Remove(parentparent);
                }
            }

            treeView.SelectedNode = null;

            ClearUI();
        }
        private static bool CheckPath(string path, bool isDirectory = true)
        {
            if (isDirectory)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            else
            {
                if (!File.Exists(path))
                {
                    File.Create(path).Close();
                }
            }

            return true;
        }
        private static async void DisplayMessage(string message, ToolStripStatusLabel label)
        {
            label.Text = message;

            await Task.Delay(5000);

            label.Text = null;
        }
        private static TreeNode? CreateNode(string? text, string? name, int icon, int? selected = null, object? tag = null)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            TreeNode? node = new(text)
            {
                Name = string.IsNullOrEmpty(name) ? text : name,
                ImageIndex = icon,
                SelectedImageIndex = selected ?? icon,
                Tag = tag ?? null
            };

            return node;
        }


        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            SetRights(e?.Node);

            if (e?.Node?.Tag is null)
            {
                ClearUI();
                return;
            }

            DisplaySnippet(e?.Node);
            SaveNodeId(e?.Node);
        }
        private void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (e?.Node?.Level == 0)
            {
                return;
            }

            SaveSnippet(treeView.PreviousSelectedNode);
        }
        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (!e.CancelEdit && e?.Label?.Length > 0 && e?.Label?.IndexOfAny(new char[] { '@', '.', ',', '!' }) == -1)
            {
                e.CancelEdit = true;

                if (e.Node.Level == 0 || e.Node.Level == 1)
                {
                    return;
                }

                if (e.Node.Level == 2 && !string.IsNullOrEmpty(e.Label))
                {
                    string? label = e.Label;

                    if (string.IsNullOrEmpty(label))
                    {
                        return;
                    }

                    IEnumerable<TreeNode>? children = e.Node.Nodes.SearchTree();

                    foreach (TreeNode? node in children)
                    {
                        if (node.Tag is not null && e.Node.Level == 2)
                        {
                            e.Node.Text = e.Node.Name = label;
                            _TreeviewItem? snippet = (node.Tag as _TreeviewItem) ?? null;

                            if (snippet is not null && snippet.Category is not null)
                            {
                                snippet.Category.Description = label;
                            }
                        }
                    }

                }
                else if (e.Node.Level == 3)
                {
                    _TreeviewItem? snippet = (e.Node.Tag as _TreeviewItem) ?? null;

                    if (snippet is not null)
                    {
                        snippet.Title = e.Node.Text = e.Label;
                    }
                }
                else { }
            }

            treeView.LabelEdit = false;
            treeView.Sort();
            treeView.SelectedNode = e.Node;
            treeView.EndUpdate();
        }
        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.N)
            {
                NewSnippet();
            }

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.S)
            {
                SaveSnippet(treeView.SelectedNode, false);
            }

            if (e.Modifiers == Keys.Control && e.KeyCode == Keys.P)
            {
                SaveSnippet(treeView.SelectedNode, false);
                PrintSnippet();
            }

            if (e.KeyCode == Keys.F2)
            {
                if (treeView.SelectedNode.Level == 0 || treeView.SelectedNode.Level == 1)
                {
                    return;
                }

                treeView.LabelEdit = true;

                if (!treeView.SelectedNode.IsEditing)
                {
                    treeView.SelectedNode.BeginEdit();
                }
            }
        }
        private void syntaxBoxControl_TextChanged(object? sender, EventArgs e)
        {
            if (sender is SyntaxBoxControl)
            {
                SyntaxBoxControl? syntaxBox = sender as SyntaxBoxControl;

                if (syntaxBox is null)
                {
                    return;
                }

                tsSave.Enabled = uc.CheckForChanges();

                tsUndo.Enabled = syntaxBox.CanUndo;
                tsRedo.Enabled = syntaxBox.CanRedo;
            }
            else
            {
                TextBox? syntaxBox = sender as TextBox;

                if (syntaxBox is null)
                {
                    return;
                }

                tsSave.Enabled = uc.CheckForChanges();
            }
        }
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeView.LabelEdit = false;

            if (e?.Node is null && e?.Button is not MouseButtons.Right)
            {
                return;
            }

            treeView.SelectedNode = e.Node;

            ShowContext(e.Node);
        }
        private void tsReload_Click(object sender, EventArgs e)
        {
            ReloadDatabases();
        }
        private void tsNew_Click(object sender, EventArgs e)
        {
            NewSnippet();
        }
        private void tsSave_Click(object sender, EventArgs e)
        {
            SaveSnippet(treeView.SelectedNode, false);
        }
        private void tsDelete_Click(object sender, EventArgs e)
        {
            DeleteSnippet(treeView.SelectedNode);
        }
        private void ShowContext(SyntaxBoxControl? control)
        {
            if (control is null)
            {
                return;
            }

            control.ContextMenuStrip = contextMenuStrip1;

            ToolStripItem? cut = contextMenuStrip1.Items.Find("cut", false)?.FirstOrDefault();
            if (cut is not null)
            {
                cut.DetachEvents();
                cut.Click += (s, e) => control.Cut();
                tsCut.Enabled = cut.Enabled = control.Selection.SelLength != 0;
            }

            ToolStripItem? copy = contextMenuStrip1.Items.Find("copy", false)?.FirstOrDefault();
            if (copy is not null)
            {
                copy.DetachEvents();
                copy.Click += (s, e) => control.Copy();
                tsCopy.Enabled = copy.Enabled = control.CanCopy;
            }

            ToolStripItem? paste = contextMenuStrip1.Items.Find("paste", false)?.FirstOrDefault();
            if (paste is not null)
            {
                paste.DetachEvents();
                paste.Click += (s, e) => control.Paste();
                tsPaste.Enabled = paste.Enabled = Clipboard.ContainsText();
            }

            ToolStripItem? undo = contextMenuStrip1.Items.Find("undo", false)?.FirstOrDefault();
            if (undo is not null)
            {
                undo.DetachEvents();
                undo.Click += (s, e) => control.Undo();
                undo.Enabled = control.CanUndo;
            }

            ToolStripItem? redo = contextMenuStrip1.Items.Find("redo", false)?.FirstOrDefault();
            if (redo is not null)
            {
                redo.DetachEvents();
                redo.Click += (s, e) => control.Redo();
                redo.Enabled = control.CanRedo;
            }

            ToolStripItem? find = contextMenuStrip1.Items.Find("find", false)?.FirstOrDefault();
            if (find is not null)
            {
                find.DetachEvents();
                find.Click += (s, e) =>
                {
                    control.Caret.Position.X = 0;
                    control.Caret.Position.Y = 0;
                    control.ShowFind();
                };
                tsFind.Enabled = find.Enabled = control.Document.Text.Length != 0;
            }

            ToolStripItem? replace = contextMenuStrip1.Items.Find("replace", false)?.FirstOrDefault();
            if (replace is not null)
            {
                replace.DetachEvents();
                replace.Click += (s, e) =>
                {
                    control.Caret.Position.X = 0;
                    control.Caret.Position.Y = 0;
                    control.ShowReplace();
                };
                replace.Enabled = control.Document.Text.Length != 0;
            }

        }
        private void ShowContext(TreeNode? node)
        {
            if (node is null)
            {
                return;
            }

            node.ContextMenuStrip = null;
            node.ContextMenuStrip = contextMenuStrip2;

            ToolStripItem? deleteSnippet = contextMenuStrip2.Items.Find("msDeleteSnippet", false)?.FirstOrDefault();
            if (deleteSnippet is not null)
            {
                deleteSnippet.DetachEvents();
                deleteSnippet.Click += (s, e) => DeleteSnippet(node);

                if (node?.Level == 3)
                {
                    deleteSnippet.Text = "Delete Snippet";
                    deleteSnippet.Visible = true;
                }
                else if (node?.Level == 2)
                {
                    deleteSnippet.Text = "Delete Category";

                    if (node.Nodes.Count > 0)
                    {
                        deleteSnippet.Visible = true;
                    }
                    else
                    {
                        //deleteSnippet.Visible = !(_config?.GetValue<bool>("Settings:" + Stuff._settingsDeleteEmptyNodes) ?? false);
                    }
                }
                else
                {
                    deleteSnippet.Visible = false;
                }
            }

            ToolStripItem? createNewSnippet = contextMenuStrip2.Items.Find("msCreateSnippet", false)?.FirstOrDefault();
            if (createNewSnippet is not null)
            {
                createNewSnippet.DetachEvents();
                createNewSnippet.Click += (s, e) => NewSnippet();
                createNewSnippet.Visible = node?.Level == 2;
            }
        }
        private void tsSettings_Click(object sender, EventArgs e)
        {
            if (instance.Config.TryGetValue("Settings", out var settings))
            {
                toolStrip1.ShowItemToolTips = settings["ShowTooltips"];
            }
        }
        private void tsSearch_Click(object sender, EventArgs e)
        {
            // search treeview
        }
        private void tsExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void tsHelp_Click(object sender, EventArgs e)
        {
            frmHelp frm = new();
            frm.Owner = this;
            frm.StartPosition = FormStartPosition.CenterParent;
            frm.ShowDialog();
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            var snippet = treeView?.SelectedNode?.Tag as _TreeviewItem ?? null;
            uc.EditMode(snippet);
            uc.Dock = DockStyle.Fill;
        }
        private void comboBox_SelectedValueChanged(object? sender, EventArgs e)
        {
            tsSave.Enabled = uc.CheckForChanges();
        }
    }
}