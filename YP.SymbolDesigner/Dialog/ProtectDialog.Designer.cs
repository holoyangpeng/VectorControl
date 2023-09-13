namespace YP.SymbolDesigner.Dialog
{
    partial class ProtectDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkAll = new System.Windows.Forms.CheckBox();
            this.chkSkew = new System.Windows.Forms.CheckBox();
            this.chkRotate = new System.Windows.Forms.CheckBox();
            this.chkScale = new System.Windows.Forms.CheckBox();
            this.chkTranslate = new System.Windows.Forms.CheckBox();
            this.chkNone = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(251, 52);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(251, 85);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkAll);
            this.groupBox1.Controls.Add(this.chkSkew);
            this.groupBox1.Controls.Add(this.chkRotate);
            this.groupBox1.Controls.Add(this.chkScale);
            this.groupBox1.Controls.Add(this.chkTranslate);
            this.groupBox1.Controls.Add(this.chkNone);
            this.groupBox1.Location = new System.Drawing.Point(16, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(216, 96);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "变换类型";
            // 
            // chkAll
            // 
            this.chkAll.AutoSize = true;
            this.chkAll.Location = new System.Drawing.Point(128, 64);
            this.chkAll.Name = "chkAll";
            this.chkAll.Size = new System.Drawing.Size(48, 16);
            this.chkAll.TabIndex = 5;
            this.chkAll.Text = "全部";
            this.chkAll.CheckedChanged += new System.EventHandler(this.chkAll_CheckedChanged);
            // 
            // chkSkew
            // 
            this.chkSkew.AutoSize = true;
            this.chkSkew.Location = new System.Drawing.Point(16, 64);
            this.chkSkew.Name = "chkSkew";
            this.chkSkew.Size = new System.Drawing.Size(48, 16);
            this.chkSkew.TabIndex = 4;
            this.chkSkew.Text = "扭曲";
            this.chkSkew.CheckedChanged += new System.EventHandler(this.chkTranslate_CheckedChanged);
            // 
            // chkRotate
            // 
            this.chkRotate.AutoSize = true;
            this.chkRotate.Location = new System.Drawing.Point(128, 40);
            this.chkRotate.Name = "chkRotate";
            this.chkRotate.Size = new System.Drawing.Size(48, 16);
            this.chkRotate.TabIndex = 3;
            this.chkRotate.Text = "旋转";
            this.chkRotate.CheckedChanged += new System.EventHandler(this.chkTranslate_CheckedChanged);
            // 
            // chkScale
            // 
            this.chkScale.AutoSize = true;
            this.chkScale.Location = new System.Drawing.Point(16, 40);
            this.chkScale.Name = "chkScale";
            this.chkScale.Size = new System.Drawing.Size(48, 16);
            this.chkScale.TabIndex = 2;
            this.chkScale.Text = "缩放";
            this.chkScale.CheckedChanged += new System.EventHandler(this.chkTranslate_CheckedChanged);
            // 
            // chkTranslate
            // 
            this.chkTranslate.AutoSize = true;
            this.chkTranslate.Location = new System.Drawing.Point(128, 16);
            this.chkTranslate.Name = "chkTranslate";
            this.chkTranslate.Size = new System.Drawing.Size(48, 16);
            this.chkTranslate.TabIndex = 1;
            this.chkTranslate.Text = "平移";
            this.chkTranslate.CheckedChanged += new System.EventHandler(this.chkTranslate_CheckedChanged);
            // 
            // chkNone
            // 
            this.chkNone.AutoSize = true;
            this.chkNone.Location = new System.Drawing.Point(16, 16);
            this.chkNone.Name = "chkNone";
            this.chkNone.Size = new System.Drawing.Size(36, 16);
            this.chkNone.TabIndex = 0;
            this.chkNone.Text = "无";
            this.chkNone.CheckedChanged += new System.EventHandler(this.chkAll_CheckedChanged);
            // 
            // ProtectDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(338, 124);
            this.Controls.Add(this.groupBox1);
            this.Name = "ProtectDialog";
            this.Text = "保护";
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.button1, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkAll;
        private System.Windows.Forms.CheckBox chkSkew;
        private System.Windows.Forms.CheckBox chkRotate;
        private System.Windows.Forms.CheckBox chkScale;
        private System.Windows.Forms.CheckBox chkTranslate;
        private System.Windows.Forms.CheckBox chkNone;
    }
}