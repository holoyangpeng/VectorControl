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
    public partial class BaseDialog : Form
    {
        public BaseDialog()
        {
            InitializeComponent();
        }

        #region ..OK
        private void button2_Click(object sender, EventArgs e)
        {
            OnAccept();
        }
        #endregion

        #region ..OnAccept
        protected virtual void OnAccept()
        {

        }
        #endregion
    }
}
