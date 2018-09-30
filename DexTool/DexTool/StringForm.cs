using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DexTool
{
    public partial class StringForm : Form
    {
        public StringForm()
        {
            InitializeComponent();
        }

        public StringForm(string[] data)
        {
            InitializeComponent();
            listBox1.Items.AddRange(data);
        }

        private void StringForm_Load(object sender, EventArgs e)
        {

        }
    }
}
