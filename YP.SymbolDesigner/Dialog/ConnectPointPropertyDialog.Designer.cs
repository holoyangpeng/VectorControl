namespace YP.SymbolDesigner.Dialog
{
    partial class ConnectPointPropertyDialog
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.trackTop = new System.Windows.Forms.TrackBar();
            this.trackBottom = new System.Windows.Forms.TrackBar();
            this.trackLeft = new System.Windows.Forms.TrackBar();
            this.trackRight = new System.Windows.Forms.TrackBar();
            this.lbLeft = new System.Windows.Forms.Label();
            this.lbTop = new System.Windows.Forms.Label();
            this.lbRight = new System.Windows.Forms.Label();
            this.lbBottom = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.trackTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackRight)).BeginInit();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(472, 411);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(379, 411);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Location = new System.Drawing.Point(53, 53);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(453, 313);
            this.panel1.TabIndex = 0;
            // 
            // trackTop
            // 
            this.trackTop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trackTop.AutoSize = false;
            this.trackTop.Enabled = false;
            this.trackTop.Location = new System.Drawing.Point(43, 22);
            this.trackTop.Name = "trackTop";
            this.trackTop.Size = new System.Drawing.Size(476, 25);
            this.trackTop.TabIndex = 1;
            this.trackTop.Tag = "lbTop";
            this.trackTop.ValueChanged += new System.EventHandler(this.trackRight_ValueChanged);
            // 
            // trackBottom
            // 
            this.trackBottom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBottom.AutoSize = false;
            this.trackBottom.Enabled = false;
            this.trackBottom.Location = new System.Drawing.Point(43, 368);
            this.trackBottom.Name = "trackBottom";
            this.trackBottom.Size = new System.Drawing.Size(484, 25);
            this.trackBottom.TabIndex = 2;
            this.trackBottom.Tag = "lbBottom";
            this.trackBottom.ValueChanged += new System.EventHandler(this.trackRight_ValueChanged);
            // 
            // trackLeft
            // 
            this.trackLeft.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.trackLeft.AutoSize = false;
            this.trackLeft.Enabled = false;
            this.trackLeft.Location = new System.Drawing.Point(22, 45);
            this.trackLeft.Name = "trackLeft";
            this.trackLeft.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackLeft.Size = new System.Drawing.Size(25, 329);
            this.trackLeft.TabIndex = 3;
            this.trackLeft.Tag = "lbLeft";
            this.trackLeft.ValueChanged += new System.EventHandler(this.trackRight_ValueChanged);
            // 
            // trackRight
            // 
            this.trackRight.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trackRight.AutoSize = false;
            this.trackRight.Enabled = false;
            this.trackRight.Location = new System.Drawing.Point(512, 45);
            this.trackRight.Name = "trackRight";
            this.trackRight.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackRight.Size = new System.Drawing.Size(25, 329);
            this.trackRight.TabIndex = 4;
            this.trackRight.Tag = "lbRight";
            this.trackRight.ValueChanged += new System.EventHandler(this.trackRight_ValueChanged);
            // 
            // lbLeft
            // 
            this.lbLeft.AutoSize = true;
            this.lbLeft.Location = new System.Drawing.Point(9, 190);
            this.lbLeft.Name = "lbLeft";
            this.lbLeft.Size = new System.Drawing.Size(11, 12);
            this.lbLeft.TabIndex = 7;
            this.lbLeft.Text = "0";
            this.lbLeft.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbTop
            // 
            this.lbTop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbTop.AutoSize = true;
            this.lbTop.Location = new System.Drawing.Point(258, 5);
            this.lbTop.Name = "lbTop";
            this.lbTop.Size = new System.Drawing.Size(11, 12);
            this.lbTop.TabIndex = 8;
            this.lbTop.Text = "0";
            this.lbTop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbRight
            // 
            this.lbRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lbRight.AutoSize = true;
            this.lbRight.Location = new System.Drawing.Point(536, 190);
            this.lbRight.Name = "lbRight";
            this.lbRight.Size = new System.Drawing.Size(11, 12);
            this.lbRight.TabIndex = 9;
            this.lbRight.Text = "0";
            this.lbRight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbBottom
            // 
            this.lbBottom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbBottom.AutoSize = true;
            this.lbBottom.Location = new System.Drawing.Point(258, 396);
            this.lbBottom.Name = "lbBottom";
            this.lbBottom.Size = new System.Drawing.Size(11, 12);
            this.lbBottom.TabIndex = 10;
            this.lbBottom.Text = "0";
            this.lbBottom.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // checkBox1
            // 
            this.checkBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(22, 415);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(96, 16);
            this.checkBox1.TabIndex = 11;
            this.checkBox1.Text = "自定义连接点";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // ConnectPointPropertyDialog
            // 
            this.AcceptButton = null;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = null;
            this.ClientSize = new System.Drawing.Size(559, 452);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.lbTop);
            this.Controls.Add(this.trackTop);
            this.Controls.Add(this.lbLeft);
            this.Controls.Add(this.trackLeft);
            this.Controls.Add(this.lbBottom);
            this.Controls.Add(this.trackBottom);
            this.Controls.Add(this.lbRight);
            this.Controls.Add(this.trackRight);
            this.Controls.Add(this.panel1);
            this.Name = "ConnectPointPropertyDialog";
            this.Padding = new System.Windows.Forms.Padding(50);
            this.Text = "连接点属性";
            this.Controls.SetChildIndex(this.panel1, 0);
            this.Controls.SetChildIndex(this.button1, 0);
            this.Controls.SetChildIndex(this.btnOK, 0);
            this.Controls.SetChildIndex(this.trackRight, 0);
            this.Controls.SetChildIndex(this.lbRight, 0);
            this.Controls.SetChildIndex(this.trackBottom, 0);
            this.Controls.SetChildIndex(this.lbBottom, 0);
            this.Controls.SetChildIndex(this.trackLeft, 0);
            this.Controls.SetChildIndex(this.lbLeft, 0);
            this.Controls.SetChildIndex(this.trackTop, 0);
            this.Controls.SetChildIndex(this.lbTop, 0);
            this.Controls.SetChildIndex(this.checkBox1, 0);
            ((System.ComponentModel.ISupportInitialize)(this.trackTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackRight)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.Panel panel1;
        protected System.Windows.Forms.TrackBar trackTop;
        protected System.Windows.Forms.TrackBar trackBottom;
        protected System.Windows.Forms.TrackBar trackLeft;
        protected System.Windows.Forms.TrackBar trackRight;
        protected System.Windows.Forms.Label lbLeft;
        protected System.Windows.Forms.Label lbTop;
        protected System.Windows.Forms.Label lbRight;
        protected System.Windows.Forms.Label lbBottom;
        private System.Windows.Forms.CheckBox checkBox1;

    }
}