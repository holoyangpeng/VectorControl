namespace PerfectSVG.SymbolDesigner.Controls
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.lbLeft = new System.Windows.Forms.Label();
            this.lbTop = new System.Windows.Forms.Label();
            this.lbRight = new System.Windows.Forms.Label();
            this.lbBottom = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.trackTop)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBottom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackLeft)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackRight)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Location = new System.Drawing.Point(35, 36);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(484, 329);
            this.panel1.TabIndex = 0;
            // 
            // trackTop
            // 
            this.trackTop.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trackTop.AutoSize = false;
            this.trackTop.Location = new System.Drawing.Point(35, 4);
            this.trackTop.Name = "trackTop";
            this.trackTop.Size = new System.Drawing.Size(484, 25);
            this.trackTop.TabIndex = 1;
            this.trackTop.Tag = "lbTop";
            this.trackTop.ValueChanged += new System.EventHandler(this.trackRight_ValueChanged);
            // 
            // trackBottom
            // 
            this.trackBottom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBottom.AutoSize = false;
            this.trackBottom.Location = new System.Drawing.Point(35, 380);
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
            this.trackLeft.Location = new System.Drawing.Point(4, 36);
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
            this.trackRight.Location = new System.Drawing.Point(525, 36);
            this.trackRight.Name = "trackRight";
            this.trackRight.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackRight.Size = new System.Drawing.Size(25, 329);
            this.trackRight.TabIndex = 4;
            this.trackRight.Tag = "lbRight";
            this.trackRight.ValueChanged += new System.EventHandler(this.trackRight_ValueChanged);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(379, 411);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "取 消";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button2.Location = new System.Drawing.Point(475, 411);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "确 定";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // lbLeft
            // 
            this.lbLeft.Location = new System.Drawing.Point(4, 0);
            this.lbLeft.Name = "lbLeft";
            this.lbLeft.Size = new System.Drawing.Size(33, 29);
            this.lbLeft.TabIndex = 7;
            this.lbLeft.Text = "0";
            this.lbLeft.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbTop
            // 
            this.lbTop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lbTop.Location = new System.Drawing.Point(517, 4);
            this.lbTop.Name = "lbTop";
            this.lbTop.Size = new System.Drawing.Size(33, 29);
            this.lbTop.TabIndex = 8;
            this.lbTop.Text = "0";
            this.lbTop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbRight
            // 
            this.lbRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lbRight.Location = new System.Drawing.Point(517, 373);
            this.lbRight.Name = "lbRight";
            this.lbRight.Size = new System.Drawing.Size(33, 29);
            this.lbRight.TabIndex = 9;
            this.lbRight.Text = "0";
            this.lbRight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbBottom
            // 
            this.lbBottom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbBottom.Location = new System.Drawing.Point(4, 368);
            this.lbBottom.Name = "lbBottom";
            this.lbBottom.Size = new System.Drawing.Size(33, 29);
            this.lbBottom.TabIndex = 10;
            this.lbBottom.Text = "0";
            this.lbBottom.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ConnectPointPropertyDialog
            // 
            this.AcceptButton = this.button2;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button1;
            this.ClientSize = new System.Drawing.Size(559, 452);
            this.Controls.Add(this.lbTop);
            this.Controls.Add(this.trackTop);
            this.Controls.Add(this.lbLeft);
            this.Controls.Add(this.trackLeft);
            this.Controls.Add(this.lbBottom);
            this.Controls.Add(this.trackBottom);
            this.Controls.Add(this.lbRight);
            this.Controls.Add(this.trackRight);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConnectPointPropertyDialog";
            this.Padding = new System.Windows.Forms.Padding(50);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "连接点属性";
            ((System.ComponentModel.ISupportInitialize)(this.trackTop)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBottom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackLeft)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackRight)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TrackBar trackTop;
        private System.Windows.Forms.TrackBar trackBottom;
        private System.Windows.Forms.TrackBar trackLeft;
        private System.Windows.Forms.TrackBar trackRight;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label lbLeft;
        private System.Windows.Forms.Label lbTop;
        private System.Windows.Forms.Label lbRight;
        private System.Windows.Forms.Label lbBottom;
    }
}