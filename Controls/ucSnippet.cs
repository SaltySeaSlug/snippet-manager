using snippet_manager.Common;
using snippet_manager.Entities;
using snippet_manager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace snippet_manager.Controls
{
    public partial class ucSnippet : UserControl
    {
        private Repository db;
        public List<SyntaxFileInfo> SyntaxFiles { get; set; }
        private readonly string syntaxDirectory = Path.Combine(Application.StartupPath, "SyntaxFiles");

        private _TreeviewItem Snippet { get; set; }

        public ucSnippet(Repository databaseHandler)
        {
            InitializeComponent();

            db = databaseHandler;
            
            SyntaxFiles = new();
        }

        private void LoadComboboxes()
        {
            cmbGroup.DataSource = null;
            cmbCategory.DataSource = null;
            cmbAuthor.DataSource = null;
            cmbLanguage.DataSource = null;

            cmbGroup.DataSource = db.GetGroups().Select(_ => new _ComboBoxItem { Id = _.Id, Name = _.Description }).ToList();
            cmbCategory.DataSource = db.GetCategories().Select(_ => new _ComboBoxItem { Id = _.Id, Name = _.Description }).ToList();
            cmbAuthor.DataSource = db.GetAuthors().Select(_ => new _ComboBoxItem { Id = _.Id, Name = _.Name + " " + _.Surname }).ToList();
            cmbLanguage.DataSource = db.GetLanguages().Select(_ => new _ComboBoxItem { Id = _.Id, Name = _.Description }).ToList();

            cmbGroup.ValueMember = "Id";
            cmbGroup.DisplayMember = "Name";
            cmbCategory.ValueMember = "Id";
            cmbCategory.DisplayMember = "Name";
            cmbAuthor.ValueMember = "Id";
            cmbAuthor.DisplayMember = "Name";
            cmbLanguage.ValueMember = "Id";
            cmbLanguage.DisplayMember = "Name";
        }
        private void LoadSyntaxFiles()
        {
            SyntaxFiles.Clear();

            if (!CheckPath(syntaxDirectory))
                return;

            string[] syntaxFiles = Directory.GetFiles(syntaxDirectory, "*.syn");

            foreach (string? syntaxFile in syntaxFiles.OrderBy(_ => _))
            {
                SyntaxFiles.Add(new SyntaxFileInfo()
                {
                    DisplayName = Path.GetFileNameWithoutExtension(syntaxFile),
                    FileName = syntaxFile,
                    IsModified = false
                });
            }
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
        private void SelectSyntaxFile(string languageName)
        {
            syntaxBoxControl1.Document.SyntaxFile = SyntaxFiles
                     .Find(syntax => syntax.DisplayName == languageName)?
                     .FileName ??
                     string.Empty;
        }

        public void NewMode()
        {
            LoadComboboxes();
            LoadSyntaxFiles();

            cmbLanguage.DetachEvents();
            cmbLanguage.SelectedIndexChanged += (s, r) => 
            {
                var syntaxToUse = ((s as System.Windows.Forms.ComboBox)?.SelectedItem as _ComboBoxItem)?.Name ?? string.Empty;
                SelectSyntaxFile(syntaxToUse); 
            };

            Snippet = new()
            {
                GroupId = long.TryParse(cmbGroup.SelectedValue.ToString(), out var groupId) ? groupId : 0,
                CategoryId = long.TryParse(cmbCategory.SelectedValue.ToString(), out var categoryId) ? categoryId : 0,
                LanguageId = long.TryParse(cmbLanguage.SelectedValue.ToString(), out var languageId) ? languageId : 0,
                AuthorId = long.TryParse(cmbAuthor.SelectedValue.ToString(), out var authorId) ? authorId : 0
            };

            cmbGroup.Enabled = true;
            cmbCategory.Enabled = true;
            cmbAuthor.Enabled = true;
            cmbLanguage.Enabled = true;
            txtTitle.ReadOnly = false;
            txtCreated.ReadOnly = false;
            txtUpdated.ReadOnly = false;
            txtVersion.ReadOnly = false;
            txtSummary.ReadOnly = false;
            txtKeywords.ReadOnly = false;
            syntaxBoxControl1.ReadOnly = false;
        }

        public void EditMode(_TreeviewItem snippet)
        {
            Snippet = snippet;
            cmbLanguage.DetachEvents();
            cmbLanguage.SelectedIndexChanged += (s, r) => 
            {
                var syntaxToUse = ((s as System.Windows.Forms.ComboBox)?.SelectedItem as _ComboBoxItem)?.Name ?? string.Empty;
                SelectSyntaxFile(syntaxToUse);
            };

            cmbGroup.Enabled = true;
            cmbCategory.Enabled = true;
            cmbAuthor.Enabled = false;
            cmbLanguage.Enabled = true;
            txtTitle.ReadOnly = false;
            txtCreated.ReadOnly = true;
            txtUpdated.ReadOnly = true;
            txtVersion.ReadOnly = false;
            txtSummary.ReadOnly = false;
            txtKeywords.ReadOnly = false;
            syntaxBoxControl1.ReadOnly = false;
        }

        public void ViewMode(_TreeviewItem snippet)
        {
            Snippet = snippet;

            LoadComboboxes();
            LoadSyntaxFiles();
            
            var syntaxToUse = snippet?.Language?.Description ?? string.Empty;
            SelectSyntaxFile(syntaxToUse);

            cmbGroup.Enabled = false;
            cmbCategory.Enabled = false;
            cmbAuthor.Enabled = false;
            cmbLanguage.Enabled = false;
            txtTitle.ReadOnly = true;
            txtCreated.ReadOnly = true;
            txtUpdated.ReadOnly = true;
            txtVersion.ReadOnly = true;
            txtSummary.ReadOnly = true;
            txtKeywords.ReadOnly = true;
            syntaxBoxControl1.ReadOnly = true;

            cmbGroup.SelectedValue = snippet?.GroupId;
            cmbCategory.SelectedValue = snippet?.CategoryId;
            cmbAuthor.SelectedValue = snippet?.AuthorId;
            cmbLanguage.SelectedValue = snippet?.LanguageId;

            txtTitle.Text = snippet?.Title;
            txtCreated.Text = snippet?.Created.ToString("yyyy-MM-dd");
            txtUpdated.Text = snippet?.Updated?.ToString("yyyy-MM-dd");
            txtVersion.Text = snippet?.Version;

            txtKeywords.Text = snippet?.Keywords;
            txtSummary.Text = snippet?.Summary;

            syntaxBoxControl1.Document.Text = snippet?.Code;
        }

        public _TreeviewItem InsertMode()
        {
            if (string.IsNullOrEmpty(txtTitle.Text))
                return null;

            long groupId = long.Parse(cmbGroup.SelectedValue.ToString());
            long categoryId = long.Parse(cmbCategory.SelectedValue.ToString());
            long languageId = long.Parse(cmbLanguage.SelectedValue.ToString());
            long authorId = long.Parse(cmbAuthor.SelectedValue.ToString());

            var dbSnip = new _Snippet()
            {
                GroupId = groupId,
                LanguageId = languageId,
                AuthorId = authorId,
                CategoryId = categoryId,
                Code = syntaxBoxControl1.Document.Text.Trim(),
                Keyword = txtKeywords.Text.Trim(),
                Summary = txtSummary.Text.Trim(),
                Title = txtTitle.Text.Trim(),
                Version = txtVersion.Text.Trim()
            };

            db.InsertOrUpdate(dbSnip);

            var dbSnippet = db.GetSnippetById(dbSnip.Id);

            return new _TreeviewItem()
            {
                AuthorId = dbSnippet.AuthorId,
                CategoryId = dbSnippet.CategoryId,
                GroupId = dbSnippet.GroupId,
                LanguageId = dbSnippet.LanguageId,
                Code = dbSnippet.Code,
                Created = dbSnippet.Created,
                Id = dbSnippet.Id,
                Keywords = dbSnippet.Keyword,
                Summary = dbSnippet.Summary,
                Title = dbSnippet.Title,
                Updated = dbSnippet.Updated,
                Version = dbSnippet.Version,
                Author = dbSnippet.Author,
                Category = dbSnippet.Category,
                Group = dbSnippet.Group,
                Language = dbSnippet.Language
            };
        }

        public _TreeviewItem UpdateMode(_TreeviewItem snippet)
        {
            if (snippet is not null && txtTitle.Modified)
            {
                snippet.Title = txtTitle.Text.Trim();
            }

            if (snippet is not null && txtVersion.Modified)
            {
                snippet.Version = txtVersion.Text.Trim();
            }

            if (snippet is not null && txtSummary.Modified)
            {
                snippet.Summary = txtSummary.Text.Trim();
            }

            if (snippet is not null && txtKeywords.Modified)
            {
                snippet.Keywords = string.Join(", ", txtKeywords.Text.Trim());
            }

            if (snippet is not null && long.TryParse(cmbGroup.SelectedValue.ToString(), out var groupId) && snippet.GroupId != groupId)
            {
                snippet.GroupId = groupId;
            }

            if (snippet is not null && long.TryParse(cmbCategory.SelectedValue.ToString(), out var categoryId) && snippet.CategoryId != categoryId)
            {
                snippet.CategoryId = categoryId;
            }

            if (snippet is not null && long.TryParse(cmbAuthor.SelectedValue.ToString(), out var authorId) && snippet.AuthorId != authorId)
            {
                snippet.AuthorId = authorId;
            }

            if (snippet is not null && long.TryParse(cmbLanguage.SelectedValue.ToString(), out var languageId) && snippet.LanguageId != languageId)
            {
                snippet.LanguageId = languageId;
            }

            if (snippet is not null && syntaxBoxControl1.Document.Modified)
            {
                snippet.Code = syntaxBoxControl1.Document.Text.Trim();
            }

            return snippet;
        }

        public bool CheckForChanges()
        {
            var comboCheck = false;

            if (Snippet is not null)
            {
                var g = (!string.IsNullOrEmpty(cmbGroup?.SelectedValue?.ToString()) && long.TryParse(cmbGroup.SelectedValue.ToString(), out var groupId) && Snippet?.GroupId != groupId);
                var c = (!string.IsNullOrEmpty(cmbCategory?.SelectedValue?.ToString()) && long.TryParse(cmbCategory.SelectedValue.ToString(), out var categoryId) && Snippet?.CategoryId != categoryId);
                var l = (!string.IsNullOrEmpty(cmbLanguage?.SelectedValue?.ToString()) && long.TryParse(cmbLanguage.SelectedValue.ToString(), out var languageId) && Snippet?.LanguageId != languageId);
                var a = (!string.IsNullOrEmpty(cmbAuthor?.SelectedValue?.ToString()) && long.TryParse(cmbAuthor.SelectedValue.ToString(), out var authorId) && Snippet?.AuthorId != authorId);

                comboCheck = g || c || l || a;
            }

            return txtTitle.Modified ||
                   txtCreated.Modified ||
                   txtUpdated.Modified ||
                   txtVersion.Modified ||
                   txtSummary.Modified ||
                   txtKeywords.Modified ||
                   syntaxBoxControl1.Document.Modified ||
                   comboCheck;
        }

        public void Clear()
        {
            cmbGroup.DataSource = null;
            cmbCategory.DataSource = null;
            cmbAuthor.DataSource = null;
            cmbLanguage.DataSource = null;

            txtTitle.Clear();
            txtCreated.Clear();
            txtUpdated.Clear();
            txtVersion.Clear();
            txtSummary.Clear();
            txtKeywords.Clear();

            syntaxBoxControl1.Document.Clear();
        }
        public bool ResetChanges()
        {
            return txtTitle.Modified =
                   txtCreated.Modified =
                   txtUpdated.Modified =
                   txtVersion.Modified =
                   txtSummary.Modified =
                   txtKeywords.Modified =
                   syntaxBoxControl1.Document.Modified =
                   false;
        }
    }
}
