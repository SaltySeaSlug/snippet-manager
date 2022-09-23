using Alsing.SourceCode;
using Alsing.Windows.Forms;
using snippet_manager.Common;
using snippet_manager.Entities;
using snippet_manager.Models;
using snippet_manager.Services;
using snippet_manager.Views;
using SQLite;
using System.Data;
using System.Drawing.Printing;
using System.Text.Json;
using System.Windows.Forms.VisualStyles;

namespace snippet_manager
{
    public partial class frmMain : Form
    {
        private readonly DatabaseHandler db;
        private Singleton instance = Singleton.Instance;

        private int _snippetCount = 0;
        public int SnippetCount 
        {
            get { return _snippetCount; } 
            set { _snippetCount = value; toolStripStatusLabel1.Text = $"{_snippetCount} snippets loaded"; } 
        }
        private List<SyntaxFileInfo> _syntaxFiles { get; set; } = new();
        private readonly FileSystemWatcher _syntaxFileWatcher;
        private bool ignore = false;
        private string? _lastWorkingSyntax;
        private string? _lastWorkingCategory;

        private readonly string syntaxDirectory = Path.Combine(Application.StartupPath, "SyntaxFiles");
        private readonly string nodeFile = Path.Combine(Application.StartupPath, "id.json");
        private readonly string treeFile = Path.Combine(Application.StartupPath, "tree.json");

        public frmMain()
        {
            InitializeComponent();

            db = new DatabaseHandler();

            Load += (s, e) => Initialize();
            FormClosing += (s, e) =>
            {
                SaveSnippet(treeView.SelectedNode);
                SaveTreeView();
                SaveDatabase();
                db.Close();
            };

            _syntaxFileWatcher = new FileSystemWatcher(syntaxDirectory, "*.syn")
            {
                EnableRaisingEvents = true,
                NotifyFilter = NotifyFilters.LastWrite
            };
            _syntaxFileWatcher.Changed += (s, e) =>
            {
                _syntaxFileWatcher.EnableRaisingEvents = false;
                ReloadSyntaxFiles();
                _syntaxFileWatcher.EnableRaisingEvents = true;
            };

            toolStripStatusLabel1.Text = null;
            toolStripStatusLabel2.Text = null;
            toolStripStatusLabel3.Text = null;

            treeView.TreeViewNodeSorter = new NodeSorter();
            treeView.ContextMenuStrip = null;

            if (instance.Config.TryGetValue("Settings", out var settings))
            {
                toolStrip1.ShowItemToolTips = settings["ShowTooltips"];
            }
        }


        private void ClearUI()
        {
            syntaxTextBox.Document.Clear();
            syntaxTextBox.Document.Text = string.Empty;
            syntaxTextBox.Document.Tag = null;

            keywordTextBox.Clear();
            keywordTextBox.Text = string.Empty;
            keywordTextBox.Tag = null;

            importTextBox.Clear();
            importTextBox.Text = string.Empty;
            importTextBox.Tag = null;
        }
        private void Initialize()
        {
            InitialDatabase();
            PrepareDirectories();
            LoadSyntaxFiles();
            ClearUI();
            PopulateTreeView();
            LoadTreeView();
            LoadLastSnippet();
        }
        public void InitialDatabase()
        {
            db.CreateTable<Key>();
            db.CreateTable<Snippet>();
            db.CreateTable<SnippetCategory>();
            db.CreateTable<SnippetGroup>();
            db.CreateTable<SnippetLanguage>();

            db.AddGroup("My Snippets");
        }
        private void PrepareDirectories()
        {
            CheckPath(syntaxDirectory);
        }
        private void LoadSyntaxFiles()
        {
            if (!CheckPath(syntaxDirectory))
                return;

            string[] syntaxFiles = Directory.GetFiles(syntaxDirectory, "*.syn");

            foreach (string? syntaxFile in syntaxFiles.OrderBy(_ => _))
            {
                _syntaxFiles.Add(new SyntaxFileInfo()
                {
                    DisplayName = Path.GetFileNameWithoutExtension(syntaxFile),
                    FileName = syntaxFile,
                    IsModified = false
                });
            }
        }
        private void LoadLastSnippet()
        {
            if (!CheckPath(nodeFile, false))
                return;

            if (long.TryParse(File.ReadAllText(nodeFile), out long nodeId))
                treeView.SelectedNode = treeView.Nodes.Find(nodeId.ToString(), true).SingleOrDefault();
        }
        public bool PopulateTreeView()
        {
            List<ItemValue> records = new();

            var treeNodeRoots = db.GetGroups();

            if (treeNodeRoots.Count > 0)
            {
                var roots = treeNodeRoots.Select(x => CreateNode(x.Description, null, 4)).ToArray();
                treeView.Nodes.AddRange(roots);
            }

            var snippets = db.GetKeysSnippets();

            if (snippets.Count > 0)
            {
                records = snippets.Select(x => new ItemValue
                {
                    Key = new ItemKey
                    {
                        Id = x.Id,
                        GroupId = x.SnippetGroupId,
                        Group = x.SnippetGroup,
                        CategoryId = x.SnippetCategoryId,
                        Category = x.SnippetCategory,
                        LanguageId = x.SnippetLanguageId,
                        Language = x.SnippetLanguage,
                        Description = x.Name,
                        IsPublic = x.IsPublic
                    },
                    KeyId = x.Id,
                    Id = x.SnippetId.GetValueOrDefault(),
                    Keyword = x.Keywords,
                    Import = x.Imports,
                    Code = x.Code
                }).ToList();
            }

            if (records is null || !records.Any())
                return false;

            IEnumerable<IGrouping<Group, ItemValue>>? groupByKeys = from record in records
                                                                    group record by new Group(record.Key.Group, record.Key.Language, record.Key.Category) into newRecords
                                                                    select newRecords ?? null;

            if (groupByKeys is null || !groupByKeys.Any())
                return false;

            foreach (IGrouping<Group, ItemValue>? record in groupByKeys.OrderBy(_ => _.Key.language).ThenBy(_ => _.Key.category))
            {
                if (record is null || !record.Any()) 
                    continue;

                TreeNode? root = treeView.Nodes.Find(record.Key.group, false).SingleOrDefault() ?? CreateNode(groupByKeys?.FirstOrDefault()?.Key.group, null, 0);
                TreeNode? lang = CreateNode(record.Key.language, null, 1);
                TreeNode? cat = CreateNode(record.Key.category, null, 1);

                foreach (ItemValue? code in record.OrderBy(_ => _.Key.Description))
                {
                    TreeNode? snippet = CreateNode(code?.Key?.Description, code?.Key?.Id.ToString(), 2, 3, code);
                    cat?.Nodes.Add(snippet);
                    SnippetCount++;
                }

                lang?.Nodes.Add(cat);

                if (root is null)
                    continue;

                if (!root.Nodes.Find(record.Key.language, false).Any())
                    root.Nodes.Add(lang);
                else
                    root?.Nodes?.Find(record.Key.language, false)?
                        .FirstOrDefault()?.Nodes.Add(cat);

                if (!treeView.Nodes.Find(root?.Name, true).Any())
                    treeView.Nodes.Add(root);
            }

            return true;
        }
        private void LoadTreeView()
        {
            if (CheckPath(treeFile, false))
            {
                string data = File.ReadAllText(treeFile);

                if (string.IsNullOrEmpty(data))
                    return;

                List<bool>? nodeStatus = JsonSerializer.Deserialize<List<bool>>(data) ?? null;

                if (nodeStatus == null || nodeStatus.Count == 0)
                    return;

                IEnumerable<TreeNode>? treeNodes = treeView.SearchTree();

                if (nodeStatus.Count != treeNodes.Count())
                    return;

                int i = 0;

                foreach (TreeNode? node in treeNodes)
                {
                    if (nodeStatus[i++] == true)
                        node.Expand();
                    else
                        node.Collapse();
                }
            }
        }
        private void SaveTreeView()
        {
            if (!CheckPath(treeFile, false))
                return;

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
        private void ReloadSyntaxFiles()
        {
            SyntaxDocument? current = syntaxTextBox.Document;
            _syntaxFiles.Clear();

            LoadSyntaxFiles();

            syntaxTextBox.Invoke(() =>
            {
                syntaxTextBox.Invalidate();
                syntaxTextBox.Document = new SyntaxDocument
                {
                    SyntaxFile = current.SyntaxFile,
                    Text = current.Text
                };
                syntaxTextBox.Update();
            });
        }
        private bool CheckForChanges()
        {
            return syntaxTextBox.Document.Modified ||
                   keywordTextBox.Modified ||
                   importTextBox.Modified;
        }
        private void SaveDatabase()
        {
            foreach (TreeNode node in treeView.Nodes)
            {
                List<ItemValue?> snippets = node.Nodes.SearchTree()
                    .Where(x => x.Tag != null)
                    .Select(x => x.Tag as ItemValue).ToList();

                if (snippets is null || !snippets.Any())
                    continue;

                foreach (var snippet in snippets)
                {
                    if (snippet is null)
                        continue;

                    db.InsertOrUpdateSnippet(snippet);
                }
            }

            DisplayMessage("Snippets saved successfully", toolStripStatusLabel3);
        }
        private void PrintSnippet()
        {
            PrintDocument printDoc = new PrintDocument
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
                    string script = treeView.SelectedNode.Text + Environment.NewLine + Environment.NewLine + importTextBox.Text + Environment.NewLine + syntaxTextBox.Document.Text;
                    e?.Graphics?.DrawString(script, new Font("Arial", 12), Brushes.Black, (float)e.MarginBounds.X, (float)e.MarginBounds.Y);
                };
                printDoc.Print();
            }
        }
















        private void NewSnippet()
        {
            SaveSnippet(treeView.SelectedNode);

            ClearUI();

            if (treeView.SelectedNode is not null)
            {
                // just a helper thing for the save snippet form
                switch (treeView.SelectedNode.Level)
                {
                    case 0: break;
                    case 1: _lastWorkingSyntax = treeView.SelectedNode.Text; break;
                    case 2: _lastWorkingSyntax = treeView.SelectedNode.Parent.Text; break;
                    default: _lastWorkingSyntax = treeView.SelectedNode.Parent.Parent.Text; break;
                }

                switch (treeView.SelectedNode.Level)
                {
                    case 0:
                    case 1: break;
                    case 2: _lastWorkingCategory = treeView.SelectedNode.Text; break;
                    default: _lastWorkingCategory = treeView.SelectedNode.Parent.Text; break;
                }
            }
            // set treeview previous node to null
            treeView.PreviousSelectedNode = null;

            // set treeview selected node to null
            treeView.SelectedNode = null;

            lblNewSnippet.Visible = true;
            lblNewSnippet.Text = "New Snippet";
            //syntaxTextBox.BorderColor = Color.Red;
            //syntaxTextBox.BorderStyle = Alsing.Windows.Forms.BorderStyle.Raised;
        }
        private void DisplaySnippet(TreeNode? node)
        {
            if (node?.Tag is null || node.Tag is not ItemValue snippet)
                return;

            ClearUI();

            SelectSyntaxFile(node);

            syntaxTextBox.Document.Text = snippet?.Code;
            keywordTextBox.Text = snippet?.Keyword;
            importTextBox.Text = snippet?.Import;
        }

        private void SaveNodeId(TreeNode? node)
        {
            if (node?.Tag is null || node.Tag is not ItemValue snippet)
                return;

            if (CheckPath(nodeFile, false))
                File.WriteAllText(nodeFile, snippet?.Key?.Id.ToString());
        }

        private void SelectSyntaxFile(TreeNode? node)
        {
            if (node?.Parent?.Parent is null)
                return;

            if (node.Parent.Parent.Level == 1)
            {
                syntaxTextBox.Document.SyntaxFile = _syntaxFiles
                    .Find(syntax => syntax.DisplayName == node.Parent.Parent.Text)?
                    .FileName ??
                    string.Empty;
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
            tsSave.Enabled = false;

            if (node?.Level == 0 || node?.Level == 1 || node?.Level == 2)
                return;

            tsDelete.Enabled = true;
            tsPrint.Enabled = true;
        }

        private void SaveSnippet(TreeNode? node, bool prompt = true)
        {
            using Services.DialogCenteringService centeringService = new(this);

            if (!CheckForChanges() || prompt && MessageBox.Show($"Do you want to save changes to {node?.Text}?", "Save Changes", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                lblNewSnippet.Visible = false;
                syntaxTextBox.Document.Modified = keywordTextBox.Modified = importTextBox.Modified = false;
                return;
            }

            if (node is null || node?.Level == 0 && node?.Nodes?.Count == 0)
            {
                if (node?.Level == 0 && node?.Nodes?.Count == 0)
                {
                    _lastWorkingSyntax = string.Empty;
                    _lastWorkingCategory = string.Empty;
                }

                var categories = db.GetCategories();

                Views.frmNewSnippet? frm = new(categories.Select(x => x.Description).ToList(), _syntaxFiles)
                {
                    Owner = this,
                    StartPosition = FormStartPosition.CenterParent,
                    LastLanguageUsed = _lastWorkingSyntax,
                    LastCategoryUsed = _lastWorkingCategory
                };

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    string? languageName = frm?.LanguageName?.Trim();
                    string? categoryName = frm?.CategoryName?.Trim();
                    string? snippetName = frm?.SnippetName?.Trim();

                    TreeNode? root = treeView.Nodes.Find("My Snippets", false).FirstOrDefault();
                    TreeNode? lan = root?.Nodes.Find(languageName, true).SingleOrDefault() ?? CreateNode(languageName, null, 1);
                    TreeNode? cat = lan?.Nodes.Find(categoryName, true).SingleOrDefault() ?? CreateNode(categoryName, null, 1);

                    var itemKey = new ItemKey()
                    {
                        Group = root.Name,
                        Language = languageName,
                        Category = categoryName,
                        Description = snippetName,
                        IsPublic = false
                    };
                    var itemValue = new ItemValue()
                    {
                        Key = itemKey,
                        Code = syntaxTextBox.Document.Text.Trim(),
                        Keywords = keywordTextBox.Text.Trim().Split(", ").ToList(),
                        Imports = importTextBox.Text.Trim().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList(),
                    };

                    var snippetId = db.InsertOrUpdateSnippet(itemValue);
                    var newSnippet = db.GetKeysSnippets().Where(_ => _.SnippetId == snippetId).Select(x => new ItemValue
                    {
                        Key = new ItemKey
                        {
                            Id = x.Id,
                            GroupId = x.SnippetGroupId,
                            Group = x.SnippetGroup,
                            CategoryId = x.SnippetCategoryId,
                            Category = x.SnippetCategory,
                            LanguageId = x.SnippetLanguageId,
                            Language = x.SnippetLanguage,
                            Description = x.Name,
                            IsPublic = x.IsPublic
                        },
                        KeyId = x.Id,
                        Id = x.SnippetId.GetValueOrDefault(),
                        Keyword = x.Keywords,
                        Import = x.Imports,
                        Code = x.Code
                    }).Single();

                    TreeNode? snippet = CreateNode(snippetName, snippetId.ToString(), 2, 3, newSnippet);

                    cat?.Nodes.Add(snippet);

                    if (lan?.Nodes.Find(categoryName, true).Length == 0)
                    {
                        lan?.Nodes.Add(cat);
                    }

                    if (root?.Nodes.Find(languageName, true).Length == 0)
                    {
                        root?.Nodes.Add(lan);
                    }

                    syntaxTextBox.Document.Modified = keywordTextBox.Modified = importTextBox.Modified = false;

                    node = snippet;
                }
                else
                {
                    lblNewSnippet.Visible = true;

                    return;
                }
            }
            else
            {
                if (node?.Tag is not ItemValue snippet)
                {
                    MessageBox.Show($"No valid node selected, unable to update");
                    return;
                }

                if (!snippet?.Key?.Group?.ToLower()?.Equals("My Snippets"?.ToLower()) ?? false)
                {
                    MessageBox.Show(this, $"You are attempting to modify a snippet that is not in your\r\n\\My Snippets folder.\r\nYou may lose your modifications if you download snippet updates in the future.", "Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
                }

                if (snippet is not null && syntaxTextBox.Document.Modified)
                {
                    snippet.Code = syntaxTextBox.Document.Text.Trim();
                }

                if (snippet is not null && keywordTextBox.Modified)
                {
                    snippet.Keywords = keywordTextBox.Text.Trim().Split(", ").ToList();
                }

                if (snippet is not null && importTextBox.Modified)
                {
                    snippet.Imports = importTextBox.Text.Trim().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList();
                }

                SaveDatabase();
            }

            syntaxTextBox.Document.Modified = keywordTextBox.Modified = importTextBox.Modified = false;

            tsSave.Enabled = CheckForChanges();
            treeView.SelectedNode = node;
            treeView.Select();
        }
        private void DeleteSnippet(TreeNode? node)
        {
            if (node is null)
                return;

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

                        var group = db.GetGroupByDescription(node.Parent.Parent.Text);
                        var language = db.GetLanguageByDescription(node.Parent.Text);
                        var category = db.GetCategoryByDescription(node.Text);
                        var keys = db.GetKeysById(group?.Id ?? 0, language?.Id ?? 0, category?.Id ?? 0);

                        foreach (var key in keys)
                        {
                            var snippet = db.GetSnippets().SingleOrDefault(_ => _.KeyId == key.Id);
                            db.DeleteSnippetById(snippet.Id);
                            db.DeleteKeyById(key.Id);
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

            TreeNode? index = node;
            TreeNode? parent = index?.Parent;
            TreeNode? parentparent = index?.Parent?.Parent;
            TreeNode? nextNode = index?.PrevNode ?? index?.NextNode ?? parent?.FirstNode;
            treeView.Nodes.Remove(index);

            db.DeleteSnippetById((index.Tag as ItemValue).Id);
            db.DeleteKeyById((index.Tag as ItemValue).KeyId);

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
                    Directory.CreateDirectory(path);
            }
            else
            {
                if (!File.Exists(path))
                    File.Create(path).Close();
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
                return null;

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
            if (e?.Node?.Tag is null)
            {
                tsSave.Enabled = false;
                ClearUI();
            }

            DisplaySnippet(e?.Node);
            SaveNodeId(e?.Node);
            SetRights(e?.Node);
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
            if (!ignore && e?.Label?.Length > 0 && e?.Label?.IndexOfAny(new char[] { '@', '.', ',', '!' }) == -1)
            {
                ignore = true;

                e.CancelEdit = true;

                if (e.Node.Level == 0 || e.Node.Level == 1) return;
                if (e.Node.Level == 2 && !string.IsNullOrEmpty(e.Label))
                {
                    string? label = e.Label;

                    if (string.IsNullOrEmpty(label))
                        return;

                    IEnumerable<TreeNode>? children = e.Node.Nodes.SearchTree();

                    foreach (TreeNode? node in children)
                    {
                        if (node.Tag is not null && e.Node.Level == 2)
                        {
                            e.Node.Text = e.Node.Name = label;
                            ItemKey? key = (node.Tag as ItemValue)?.Key ?? null;

                            if (key is not null)
                                key.Category = label;
                        }
                    }

                }
                else if (e.Node.Level == 3)
                {
                    ItemKey? key = (e.Node.Tag as ItemValue)?.Key ?? null;

                    if (key is not null)
                        key.Description = e.Node.Text = e.Label;
                }
                else { }

                ignore = false;
            }

            treeView.LabelEdit = false;
            treeView.Sort();
            treeView.SelectedNode = e.Node;
            treeView.EndUpdate();
        }
        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
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
                    return;

                treeView.LabelEdit = true;

                if (!treeView.SelectedNode.IsEditing)
                    treeView.SelectedNode.BeginEdit();
            }
        }
        private void syntaxBoxControl_TextChanged(object sender, EventArgs e)
        {
            if (sender is SyntaxBoxControl)
            {
                SyntaxBoxControl? syntaxBox = sender as SyntaxBoxControl;

                if (syntaxBox is null)
                    return;


                tsSave.Enabled = CheckForChanges();

                tsUndo.Enabled = syntaxBox.CanUndo;
                tsRedo.Enabled = syntaxBox.CanRedo;

            }
            else
            {
                RichTextBox? syntaxBox = sender as RichTextBox;

                if (syntaxBox is null)
                    return;


                tsSave.Enabled = CheckForChanges();

                tsUndo.Enabled = syntaxBox.CanUndo;
                tsRedo.Enabled = syntaxBox.CanRedo;

            }
        }
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            treeView.LabelEdit = false;

            if (e?.Node is null && e?.Button is not MouseButtons.Right)
                return;

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
            // check if control is null
            if (control is null)
                // return
                return;

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
                return;

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
            // update tooltips
            if (instance.Config.TryGetValue("Settings", out var settings))
            {
                toolStrip1.ShowItemToolTips = settings["ShowTooltips"];
            }
        }

        private void tsSearch_Click(object sender, EventArgs e)
        {
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
    }

   
}