using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace YP.SymbolDesigner.Dialog
{
    public partial class PromptDialog : BaseDialog
    {
        #region ..Constructor
        public PromptDialog()
        {
            InitializeComponent();
        }
        #endregion

        #region ..properties
        public string Value
        {
            set
            {
                this.textBox1.Text = value;
            }
            get
            {
                return this.textBox1.Text;
            }
        }
        #endregion

        #region ..textBox1_TextChanged
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.btnOK.Enabled = this.textBox1.Text.Trim().Length > 0;
        }
        #endregion
    }
}
