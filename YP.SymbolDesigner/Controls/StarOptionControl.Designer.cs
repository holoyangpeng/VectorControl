namespace YP.SymbolDesigner.Controls
{
    partial class StarOptionControl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.label3 = new System.Windows.Forms.Label();
            this.indentPicker = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.vertexPicker = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.indentPicker)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.vertexPicker)).BeginInit();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(132, 44);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(16, 23);
            this.label3.TabIndex = 11;
            this.label3.Text = "%";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // indentPicker
            // 
            this.indentPicker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.indentPicker.Location = new System.Drawing.Point(68, 44);
            this.indentPicker.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.indentPicker.Name = "indentPicker";
            this.indentPicker.Size = new System.Drawing.Size(64, 21);
            this.indentPicker.TabIndex = 10;
            this.indentPicker.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(4, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 23);
            this.label2.TabIndex = 9;
            this.label2.Text = "缩进半径";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // vertexPicker
            // 
            this.vertexPicker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.vertexPicker.Location = new System.Drawing.Point(68, 12);
            this.vertexPicker.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.vertexPicker.Minimum = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.vertexPicker.Name = "vertexPicker";
            this.vertexPicker.Size = new System.Drawing.Size(64, 21);
            this.vertexPicker.TabIndex = 8;
            this.vertexPicker.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(4, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 23);
            this.label1.TabIndex = 7;
            this.label1.Text = "边    数";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // StarOptionControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.indentPicker);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.vertexPicker);
            this.Controls.Add(this.label1);
            this.Name = "StarOptionControl";
            this.Size = new System.Drawing.Size(150, 78);
            ((System.ComponentModel.ISupportInitialize)(this.indentPicker)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.vertexPicker)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown indentPicker;
        private System.Windows.Forms.NumericUpDown vertexPicker;

    }
}
