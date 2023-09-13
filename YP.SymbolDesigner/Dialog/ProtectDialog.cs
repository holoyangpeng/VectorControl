using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using YP.VectorControl;
using YP.VectorControl.Forms;

namespace YP.SymbolDesigner.Dialog
{
    public partial class ProtectDialog : BaseDialog
    {
        #region ..Constructor
        public ProtectDialog()
        {
            InitializeComponent();
        }
        #endregion

        #region ..属性
        /// <summary>
        /// 获取或设置变换类型
        /// </summary>
        public TransformBehavior TransformBehavior
        {
            set
            {
                bool scale = (value & TransformBehavior.Scale) == TransformBehavior.Scale;//判断变换设置是否包含Scale
                bool skew = (value & TransformBehavior.Skew) == TransformBehavior.Skew;//判断变换设置是否包含Skew
                bool translate = (value & TransformBehavior.Translate) == TransformBehavior.Translate;//判断变换设置是否包含Translate
                bool rotate = (value & TransformBehavior.Rotate) == TransformBehavior.Rotate;//判断变换设置是否包含Rotate
                this.chkRotate.Checked = rotate;
                this.chkScale.Checked = scale;
                this.chkSkew.Checked = skew;
                this.chkTranslate.Checked = translate;
                this.SynCheckStatus();
            }
            get
            {
                TransformBehavior type = TransformBehavior.None;
                if (this.chkSkew.Checked)
                    type = type | TransformBehavior.Skew; //Skew
                if (this.chkScale.Checked)
                    type = type | TransformBehavior.Scale;//Scale
                if (this.chkRotate.Checked)
                    type = type | TransformBehavior.Rotate;//Rotate
                if (this.chkTranslate.Checked)
                    type = type | TransformBehavior.Translate;//Translate
                return type;
            }
        }
        #endregion

        #region ..选项发生变化
        //当chkAll和chkNone值发生变换时，同步其他四个单项checkBox
        private void chkAll_CheckedChanged(object sender, System.EventArgs e)
        {
            if (sender == this.chkNone)
            {
                if (this.chkNone.Checked)
                    this.chkScale.Checked = this.chkSkew.Checked = this.chkTranslate.Checked = this.chkRotate.Checked = !this.chkNone.Checked;
            }
            else if (sender == this.chkAll)
            {
                if (this.chkAll.Checked)
                    this.chkScale.Checked = this.chkSkew.Checked = this.chkTranslate.Checked = this.chkRotate.Checked = true;
            }
            this.SynCheckStatus();
        }

        /// <summary>
        /// 当某一个变换的选中状态发生变化时,同步界面中的checkBox的选中状态
        /// 当四个单项全部选中，则chkAll也选中
        /// 反之，当四个单项全部未选中，等于chkNone选中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkTranslate_CheckedChanged(object sender, System.EventArgs e)
        {
            this.SynCheckStatus();
        }
        #endregion

        #region ..同步选中状态
        /// <summary>
        /// 同步界面中的checkBox的选中状态
        /// 当四个单项全部选中，则chkAll也选中
        /// 反之，当四个单项全部未选中，等于chkNone选中
        /// </summary>
        void SynCheckStatus()
        {
            this.chkAll.Checked = this.chkRotate.Checked && this.chkTranslate.Checked && this.chkSkew.Checked && this.chkScale.Checked;
            this.chkNone.Checked = !this.chkRotate.Checked && !this.chkTranslate.Checked && !this.chkSkew.Checked && !this.chkScale.Checked;
        }
        #endregion
    }
}
