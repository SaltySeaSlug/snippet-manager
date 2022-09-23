using snippet_manager.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace snippet_manager.Views
{
    public partial class frmNewSnippet : Form
    {
        private Singleton instance = Singleton.Instance;

        public string? LastLanguageUsed { get; set; }
        public string? LastCategoryUsed { get; set; }
        public string? SnippetName { get => textBox1.Text; }
        public string? CategoryName { get => comboBox1.Text; }
        public string? LanguageName { get => comboBox2.Text; }

        public frmNewSnippet()
        {
            InitializeComponent();

            Load += (s, e) =>
            {
                if (instance.Config.TryGetValue("Settings", out var settings) && settings is not null && string.IsNullOrEmpty(settings["DefaultSyntax"]))
                {
                    comboBox2.SelectedIndex = comboBox2.FindString(LastLanguageUsed);
                }
                else
                {
                    comboBox2.SelectedIndex = comboBox2.FindString(settings["DefaultSyntax"]);
                }

                if (!string.IsNullOrEmpty(LastCategoryUsed))
                {
                    comboBox1.SelectedIndex = comboBox1.FindString(LastCategoryUsed);
                }
            };
        }

        public frmNewSnippet(List<string> categories, List<SyntaxFileInfo>? languageSyntaxes) : this()
        {
            categories?.OrderBy(_ => _)?.ToList()?.ForEach(category => comboBox1.Items.Add(category));
            languageSyntaxes?.ForEach(file => comboBox2.Items.Add(file.DisplayName));
        }
    }
}
