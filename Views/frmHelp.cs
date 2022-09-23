using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace snippet_manager.Views
{
    public partial class frmHelp : Form
    {
        public frmHelp()
        {
            InitializeComponent();
            label1.Text = $"Database size: {Common.Extensions.BytesToString(new FileInfo(Path.Combine(Application.StartupPath, "snippet.db")).Length)}";
        }
    }
}
