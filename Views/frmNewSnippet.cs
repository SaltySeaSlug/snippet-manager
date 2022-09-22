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
        public string? LastLanguageUsed { get; set; }
        public string? SnippetName { get => textBox1.Text; }
        public string? CategoryName { get => comboBox1.Text; }
        public string? LanguageName { get => comboBox2.Text; }

        public frmNewSnippet()
        {
            InitializeComponent();

            Load += (s, e) =>
            {
                // TODO: check if default syntax language has been set; if set to treeview specific then use lastlanguageused; otherwise use default specified in config file
                //if (_config.GetValue<string>("Settings:" + Stuff._settingsDefaultSyntaxLanguage).ToLower().Equals(Stuff._settingsDefaultSyntaxItem.ToLower()))
                //{
                //    // use treeview node
                    comboBox2.SelectedIndex = comboBox2.FindString(LastLanguageUsed);
                //}
                //else
                //{
                //    // use config file default
                //    comboBox2.SelectedIndex = comboBox2.FindString(_config.GetValue<string>("Settings:" + Stuff._settingsDefaultSyntaxLanguage));
                //}
            };
        }

        public frmNewSnippet(List<string> categories, List<SyntaxFileInfo>? languageSyntaxes) : this()
        {
            categories?.OrderBy(_ => _)?.ToList()?.ForEach(category => comboBox1.Items.Add(category));
            languageSyntaxes?.ForEach(file => comboBox2.Items.Add(file.DisplayName));
        }
    }
}
