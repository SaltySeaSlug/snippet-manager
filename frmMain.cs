using Alsing.SourceCode;
using Alsing.Windows.Forms;
using snippet_manager.Models;
using SQLite;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Text.Json;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace snippet_manager
{
    public partial class frmMain : Form
    {
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

        private readonly string syntaxDirectory = Path.Combine(Application.StartupPath, "SyntaxFiles");
        private readonly string nodeFile = Path.Combine(Application.StartupPath, "id.json");
        private readonly string treeFile = Path.Combine(Application.StartupPath, "tree.json");
        private readonly SQLiteConnection db = new(Path.Combine(Application.StartupPath, "snippet.db"));


        private const string SnippetQuery = @"SELECT Key.Id, SnippetGroup.Id AS SnippetGroupId, SnippetGroup.Description AS SnippetGroup, SnippetCategory.Id AS SnippetCategoryId, 
                SnippetCategory.Description AS SnippetCategory, SnippetLanguage.Id AS SnippetLanguageId, SnippetLanguage.Description AS SnippetLanguage, Key.Description AS Name,
                Snippet.Id AS SnippetId, Snippet.Keyword AS Keywords, Snippet.Import AS Imports, Snippet.Code AS Code, Key.IsPublic FROM Key LEFT JOIN Snippet ON Snippet.KeyId = Key.Id
                JOIN SnippetGroup ON SnippetGroup.Id = Key.GroupId JOIN SnippetCategory ON SnippetCategory.Id = Key.CategoryId JOIN SnippetLanguage ON SnippetLanguage.Id = Key.LanguageId";


        public frmMain()
        {
            InitializeComponent();

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

            AddGroup(db, "My Snippets");
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

            var treeNodeRoots = db.Table<SnippetGroup>().ToList();

            if (treeNodeRoots.Count > 0)
            {
                var roots = treeNodeRoots.Select(x => CreateNode(x.Description, null, 4)).ToArray();
                treeView.Nodes.AddRange(roots);
            }

            var snippets = db.Query<SnippetQuery>(SnippetQuery);

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

                    InsertOrUpdateSnippet(db, snippet);
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
            }
            // set treeview previous node to null
            treeView.PreviousSelectedNode = null;

            // set treeview selected node to null
            treeView.SelectedNode = null;
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
            // set all toolstripbuttons to false
            tsPrint.Enabled =
            tsDelete.Enabled =
            tsFind.Enabled =
            tsCut.Enabled =
            tsPaste.Enabled =
            tsCopy.Enabled =
            tsRedo.Enabled =
            tsUndo.Enabled =
            tsSave.Enabled = false;

            // check node level
            if (node?.Level == 0 || node?.Level == 1 || node?.Level == 2)
                // return
                return;

            // set to true
            tsDelete.Enabled = true;
            tsPrint.Enabled = true;
        }

        private void SaveSnippet(TreeNode? node, bool prompt = true)
        {
            using Services.DialogCenteringService centeringService = new(this);

            if (!CheckForChanges() || prompt && MessageBox.Show($"Do you want to save changes to {node?.Text}?", "Save Changes", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                syntaxTextBox.Document.Modified = keywordTextBox.Modified = importTextBox.Modified = false;
                return;
            }

            if (node is null || node?.Level == 0 && node?.Nodes?.Count == 0)
            {
                if (node?.Level == 0 && node?.Nodes?.Count == 0)
                {
                    _lastWorkingSyntax = string.Empty;
                }

                var categories = db.Table<SnippetCategory>().ToList();
                var languages = db.Table<SnippetLanguage>().ToList();
                var groups = db.Table<SnippetGroup>().ToList();

                Views.frmNewSnippet? frm = new(categories.Select(x => x.Description).ToList(), _syntaxFiles)
                {
                    Owner = this,
                    StartPosition = FormStartPosition.CenterParent,
                    LastLanguageUsed = _lastWorkingSyntax
                };

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    string? languageName = frm?.LanguageName?.Trim();
                    string? categoryName = frm?.CategoryName?.Trim();
                    string? snippetName = frm?.SnippetName?.Trim();

                    TreeNode? root = treeView.Nodes.Find("My Snippets", false).FirstOrDefault();
                    TreeNode? lan = root?.Nodes.Find(languageName, true).SingleOrDefault() ?? CreateNode(languageName, null, 1);
                    TreeNode? cat = lan?.Nodes.Find(categoryName, true).SingleOrDefault() ?? CreateNode(categoryName, null, 1);

                    if (!(categories?.Any(_ => _.Description == categoryName) ?? false) && !string.IsNullOrEmpty(categoryName))
                    {
                        AddCategory(db, categoryName);
                        categories = db.Table<SnippetCategory>().ToList();
                    }

                    if (!(languages?.Any(_ => _.Description == languageName) ?? false) && !string.IsNullOrEmpty(languageName))
                    {
                        AddLanguage(db, languageName);
                        languages = db.Table<SnippetLanguage>().ToList();
                    }

                    var dbKey = new Key()
                    {
                        GroupId = groups.SingleOrDefault(_ => _.Description == root.Name).Id,
                        CategoryId = categories.SingleOrDefault(_ => _.Description == cat.Name).Id,
                        LanguageId = languages.SingleOrDefault(_ => _.Description == lan.Name).Id,
                        Description = snippetName
                    };
                    db.Insert(dbKey);
                    var keyId = GetLastInsertId(db);

                    var dbSnippet = new Snippet()
                    {
                        KeyId = keyId,
                        Code = syntaxTextBox.Document.Text.Trim(),
                        Import = importTextBox.Text.Trim(),
                        Keyword = keywordTextBox.Text.Trim()
                    };
                    db.Insert(dbSnippet);
                    var snippetId = GetLastInsertId(db);

                    TreeNode? snippet = CreateNode(snippetName, snippetId.ToString(), 2, 3, new ItemValue()
                    {
                        Key = new ItemKey()
                        {
                            GroupId = dbKey.GroupId,
                            CategoryId = dbKey.CategoryId,
                            LanguageId = dbKey.LanguageId,
                            Group = root.Name,
                            Language = languageName,
                            Category = categoryName,
                            Description = snippetName,
                            Id = snippetId,
                            IsPublic = false
                        },
                        Id = snippetId,
                        KeyId = keyId,
                        Code = syntaxTextBox.Document.Text.Trim(),
                        Keywords = keywordTextBox.Text.Trim().Split(", ").ToList(),
                        Imports = importTextBox.Text.Trim().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList(),
                    });

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

                    treeView.SelectedNode = snippet;
                }
                else
                    return;
            }
            else
            {
                ItemValue? snippet = node?.Tag as ItemValue;

                if (snippet is null)
                {
                    MessageBox.Show($"{nameof(snippet)} is null, unable to update");
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
            }

            syntaxTextBox.Document.Modified = keywordTextBox.Modified = importTextBox.Modified = false;

            SaveDatabase();

            tsSave.Enabled = CheckForChanges();
        }



        public static void AddCategory(SQLiteConnection db, string name)
        {
            var category = new SnippetCategory()
            {
                Description = name
            };
            db.Insert(category);
        }
        public static void AddGroup(SQLiteConnection db, string name)
        {
            var group = new SnippetGroup()
            {
                Description = name
            };

            if (db.Table<SnippetGroup>().Count(x => x.Description.ToLower() == name.ToLower()) == 0)
            {
                db.Insert(group);
            }
            else
            {
                db.Update(group);
            }
        }
        public static void AddLanguage(SQLiteConnection db, string name)
        {
            var language = new SnippetLanguage()
            {
                Description = name
            };
            db.Insert(language);
        }

        public static void InsertOrUpdateSnippet(SQLiteConnection db, ItemValue item)
        {
            var dbCategory = new SnippetCategory()
            {
                 Id = item.Key.CategoryId,
                 Description = item.Key.Category
            };

            if (db.Table<SnippetCategory>().Any(x => x.Id == dbCategory.Id))
            {
                db.Update(dbCategory);
            }

            var dbKey = new Key()
            {
                Id = item.Key.Id,
                CategoryId = item.Key.CategoryId,
                GroupId = item.Key.GroupId,
                LanguageId = item.Key.LanguageId,
                Description = item.Key.Description,
                IsPublic = item.Key.IsPublic
            };

            if (db.Table<Key>().Any(x => x.Id == dbKey.Id))
            {
                db.Update(dbKey);
            }

            var dbSnippet = new Snippet()
            {
                Id = item.Id,
                KeyId = item.Key.Id,
                Code = item.Code,
                Keyword = item.Keyword,
                Import = item.Import
            };

            if (db.Table<Snippet>().Any(x => x.Id == dbSnippet.Id))
            {
                db.Update(dbSnippet);
            }
            else
            {
                db.Insert(dbKey);
                dbSnippet.KeyId = item.KeyId = item.Key.Id = GetLastInsertId(db);
                
                db.Insert(dbSnippet);
                item.Id = GetLastInsertId(db);
            }
        }
        public static long GetLastInsertId(SQLiteConnection db)
        {
            return SQLite3.LastInsertRowid(db.Handle);
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
                ClearUI();

            DisplaySnippet(e?.Node);
            SaveNodeId(e?.Node);
            SetRights(e?.Node);
        }
        private void treeView1_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (e?.Node?.Level == 0)
                return;

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
    }

    public class NodeSorter : IComparer
    {
        public int Compare(object x, object y)
        {
            TreeNode tx = (TreeNode)x;
            TreeNode ty = (TreeNode)y;

            if (tx.Level == 0)
            {
                if (tx.Text == "My Snippets")
                    return 0;
                else
                    return 1;
            }

            if (tx.Level == 1)
            {
                return tx.Text.CompareTo(ty.Text);
            }

            if (tx.Level == 2)
            {
                return tx.Text.CompareTo(ty.Text);
            }

            if (tx.Level == 3)
            {
                return tx.Text.CompareTo(ty.Text);
            }

            return 0;
        }
    }

    public static class Ex
    {
        public static IEnumerable<T> SelectFrom<T>(this IDataReader reader,
                                      Func<IDataReader, T> projection)
        {
            while (reader.Read())
            {
                yield return projection(reader);
            }
        }

        public static IEnumerable<TreeNode> SearchTree(this System.Windows.Forms.TreeView treeView)
        {
            // return all nodes for treeview
            return SearchTree(treeView.Nodes);
        }
        public static IEnumerable<TreeNode> SearchTree(this TreeNodeCollection coll)
        {
            // return all nodes
            return coll.Cast<TreeNode>().Concat(coll.Cast<TreeNode>().SelectMany(x => SearchTree(x.Nodes)));
        }

    }
}