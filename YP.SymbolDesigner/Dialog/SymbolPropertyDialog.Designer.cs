namespace YP.SymbolDesigner.Dialog
{
    partial class SymbolPropertyDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtID = new System.Windows.Forms.TextBox();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbTop = new System.Windows.Forms.Label();
            this.trackTop = new System.Windows.Forms.TrackBar();
            this.lbWarning = new System.Windows.Forms.Label();
            this.lbLeft = new System.Windows.Forms.Label();
            this.trackLeft = new System.Windows.Forms.TrackBar();
            this.lbBottom = new System.Windows.Forms.Label();
            this.trackBottom = new System.Windows.Forms.TrackBar();
            this.lbRight = new System.Windows.Forms.Label();
            this.trackRight = new System.Windows.Forms.TrackBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkConnectionPoint = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackRight)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(518, 460);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(422, 460);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "ID：";
            // 
            // txtID
            // 
            this.txtID.Location = new System.Drawing.Point(35, 7);
            this.txtID.Name = "txtID";
            this.txtID.ReadOnly = true;
            this.txtID.Size = new System.Drawing.Size(100, 21);
            this.txtID.TabIndex = 10;
            // 
            // txtTitle
            // 
            this.txtTitle.Location = new System.Drawing.Point(194, 7);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(100, 21);
            this.txtTitle.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(149, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 12);
            this.label2.TabIndex = 11;
            this.label2.Text = "Title：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(300, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 13;
            this.label3.Text = "添加模式：";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "通过Use引用图元",
            "复制直接添加"});
            this.comboBox1.Location = new System.Drawing.Point(371, 8);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 20);
            this.comboBox1.TabIndex = 14;
            this.comboBox1.Text = "通过Use引用图元";
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.ShowTip);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbTop);
            this.groupBox1.Controls.Add(this.trackTop);
            this.groupBox1.Controls.Add(this.lbWarning);
            this.groupBox1.Controls.Add(this.lbLeft);
            this.groupBox1.Controls.Add(this.trackLeft);
            this.groupBox1.Controls.Add(this.lbBottom);
            this.groupBox1.Controls.Add(this.trackBottom);
            this.groupBox1.Controls.Add(this.lbRight);
            this.groupBox1.Controls.Add(this.trackRight);
            this.groupBox1.Controls.Add(this.panel1);
            this.groupBox1.Controls.Add(this.chkConnectionPoint);
            this.groupBox1.Location = new System.Drawing.Point(14, 34);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(579, 414);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            // 
            // lbTop
            // 
            this.lbTop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbTop.Location = new System.Drawing.Point(254, 9);
            this.lbTop.Name = "lbTop";
            this.lbTop.Size = new System.Drawing.Size(17, 14);
            this.lbTop.TabIndex = 17;
            this.lbTop.Text = "0";
            this.lbTop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // trackTop
            // 
            this.trackTop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trackTop.AutoSize = false;
            this.trackTop.Enabled = false;
            this.trackTop.Location = new System.Drawing.Point(47, 23);
            this.trackTop.Name = "trackTop";
            this.trackTop.Size = new System.Drawing.Size(484, 25);
            this.trackTop.TabIndex = 12;
            this.trackTop.Tag = "lbTop";
            this.trackTop.ValueChanged += new System.EventHandler(this.trackBottom_ValueChanged);
            // 
            // lbWarning
            // 
            this.lbWarning.AutoSize = true;
            this.lbWarning.ForeColor = System.Drawing.Color.Red;
            this.lbWarning.Location = new System.Drawing.Point(6, 386);
            this.lbWarning.Name = "lbWarning";
            this.lbWarning.Size = new System.Drawing.Size(545, 24);
            this.lbWarning.TabIndex = 20;
            this.lbWarning.Text = "注意：当图元被“复制直接添加”时，自定义连接点的信息将被Copy到图元内容中的第一个子节点上，\r\n如果需要不同处理，可以在ElementDroped事件中控制";
            this.lbWarning.Visible = false;
            // 
            // lbLeft
            // 
            this.lbLeft.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbLeft.Location = new System.Drawing.Point(6, 168);
            this.lbLeft.Name = "lbLeft";
            this.lbLeft.Size = new System.Drawing.Size(19, 19);
            this.lbLeft.TabIndex = 16;
            this.lbLeft.Text = "0";
            this.lbLeft.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // trackLeft
            // 
            this.trackLeft.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.trackLeft.AutoSize = false;
            this.trackLeft.Enabled = false;
            this.trackLeft.Location = new System.Drawing.Point(23, 55);
            this.trackLeft.Name = "trackLeft";
            this.trackLeft.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackLeft.Size = new System.Drawing.Size(25, 298);
            this.trackLeft.TabIndex = 14;
            this.trackLeft.Tag = "lbLeft";
            this.trackLeft.ValueChanged += new System.EventHandler(this.trackBottom_ValueChanged);
            // 
            // lbBottom
            // 
            this.lbBottom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lbBottom.AutoSize = true;
            this.lbBottom.Location = new System.Drawing.Point(254, 370);
            this.lbBottom.Name = "lbBottom";
            this.lbBottom.Size = new System.Drawing.Size(11, 12);
            this.lbBottom.TabIndex = 19;
            this.lbBottom.Text = "0";
            this.lbBottom.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // trackBottom
            // 
            this.trackBottom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBottom.AutoSize = false;
            this.trackBottom.Enabled = false;
            this.trackBottom.Location = new System.Drawing.Point(47, 342);
            this.trackBottom.Name = "trackBottom";
            this.trackBottom.Size = new System.Drawing.Size(484, 25);
            this.trackBottom.TabIndex = 13;
            this.trackBottom.Tag = "lbBottom";
            this.trackBottom.ValueChanged += new System.EventHandler(this.trackBottom_ValueChanged);
            // 
            // lbRight
            // 
            this.lbRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lbRight.Location = new System.Drawing.Point(558, 168);
            this.lbRight.Name = "lbRight";
            this.lbRight.Size = new System.Drawing.Size(21, 19);
            this.lbRight.TabIndex = 18;
            this.lbRight.Text = "0";
            this.lbRight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // trackRight
            // 
            this.trackRight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trackRight.AutoSize = false;
            this.trackRight.Enabled = false;
            this.trackRight.Location = new System.Drawing.Point(536, 55);
            this.trackRight.Name = "trackRight";
            this.trackRight.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackRight.Size = new System.Drawing.Size(25, 298);
            this.trackRight.TabIndex = 15;
            this.trackRight.Tag = "lbRight";
            this.trackRight.ValueChanged += new System.EventHandler(this.trackBottom_ValueChanged);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Location = new System.Drawing.Point(61, 55);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(470, 286);
            this.panel1.TabIndex = 11;
            // 
            // chkConnectionPoint
            // 
            this.chkConnectionPoint.AutoSize = true;
            this.chkConnectionPoint.Location = new System.Drawing.Point(6, 0);
            this.chkConnectionPoint.Name = "chkConnectionPoint";
            this.chkConnectionPoint.Size = new System.Drawing.Size(96, 16);
            this.chkConnectionPoint.TabIndex = 0;
            this.chkConnectionPoint.Text = "自定义连接点";
            this.chkConnectionPoint.UseVisualStyleBackColor = true;
            this.chkConnectionPoint.CheckedChanged += new System.EventHandler(this.chkConnectionPoint_CheckedChanged);
            // 
            // SymbolPropertyDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(607, 495);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtID);
            this.Controls.Add(this.label1);
            this.Name = "SymbolPropertyDialog";
            this.Text = "属性";
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.txtID, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.txtTitle, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.comboBox1, 0);
            this.Controls.SetChildIndex(this.groupBox1, 0);
            this.Controls.SetChildIndex(this.button1, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackRight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtID;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkConnectionPoint;
        protected System.Windows.Forms.Label lbTop;
        protected System.Windows.Forms.TrackBar trackTop;
        protected System.Windows.Forms.Label lbLeft;
        protected System.Windows.Forms.TrackBar trackLeft;
        protected System.Windows.Forms.Label lbBottom;
        protected System.Windows.Forms.TrackBar trackBottom;
        protected System.Windows.Forms.Label lbRight;
        protected System.Windows.Forms.TrackBar trackRight;
        protected System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lbWarning;
    }
}