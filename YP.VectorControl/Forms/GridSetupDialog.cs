using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace YP.VectorControl.Forms
{
	/// <summary>
	/// <para>网格属性的编辑对话框,可以以可视化的方式编辑和设置<see cref="Grid">Grid</see>的相关参数。</para>
	/// <para>可以利用该对话框改变VectorControl的<see cref="VectorControl.Grid">网格Grid</see>参数。</para>
	/// <para>通过<see cref="GridSetupDialog">GridSetupDialog</see>对话框，可以可视化的编辑Grid参数。</para>
	/// </summary>
	public class GridSetupDialog : System.Windows.Forms.Form
	{
		#region ..Constructor

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private NumericUpDown updownSize;
        private RadioButton rdoLine;
        private RadioButton rdoDot;
        private Label label2;
        private CheckBox chkDrawBorder;
        private ComboColors cmbColor;
        private CheckBox chkVisible;
        private Label label1;
        private Label label3;
        private Label label4;
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public GridSetupDialog()
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();

			//
			// TODO: 在 InitializeComponent 调用后添加任何构造函数代码
			//
		}

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows 窗体设计器生成的代码
		/// <summary>
		/// 设计器支持所需的方法 - 不要使用代码编辑器修改
		/// 此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.updownSize = new System.Windows.Forms.NumericUpDown();
            this.rdoLine = new System.Windows.Forms.RadioButton();
            this.rdoDot = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.chkDrawBorder = new System.Windows.Forms.CheckBox();
            this.cmbColor = new ComboColors();
            this.chkVisible = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.updownSize)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(219, 17);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "确定(&O)";
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(219, 48);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "取消(&C)";
            // 
            // updownSize
            // 
            this.updownSize.Location = new System.Drawing.Point(54, 83);
            this.updownSize.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.updownSize.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.updownSize.Name = "updownSize";
            this.updownSize.Size = new System.Drawing.Size(140, 21);
            this.updownSize.TabIndex = 11;
            this.updownSize.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            // 
            // rdoLine
            // 
            this.rdoLine.AutoSize = true;
            this.rdoLine.Checked = true;
            this.rdoLine.Location = new System.Drawing.Point(54, 118);
            this.rdoLine.Name = "rdoLine";
            this.rdoLine.Size = new System.Drawing.Size(35, 16);
            this.rdoLine.TabIndex = 6;
            this.rdoLine.TabStop = true;
            this.rdoLine.Text = "线";
            this.rdoLine.UseVisualStyleBackColor = true;
            // 
            // rdoDot
            // 
            this.rdoDot.AutoSize = true;
            this.rdoDot.Location = new System.Drawing.Point(159, 118);
            this.rdoDot.Name = "rdoDot";
            this.rdoDot.Size = new System.Drawing.Size(35, 16);
            this.rdoDot.TabIndex = 7;
            this.rdoDot.Text = "点";
            this.rdoDot.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 8;
            this.label2.Text = "尺寸：";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkDrawBorder
            // 
            this.chkDrawBorder.AutoSize = true;
            this.chkDrawBorder.Location = new System.Drawing.Point(97, 17);
            this.chkDrawBorder.Name = "chkDrawBorder";
            this.chkDrawBorder.Size = new System.Drawing.Size(96, 16);
            this.chkDrawBorder.TabIndex = 13;
            this.chkDrawBorder.Text = "绘制画布边框";
            // 
            // cmbColor
            // 
            this.cmbColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbColor.ItemHeight = 18;
            this.cmbColor.Location = new System.Drawing.Point(54, 48);
            this.cmbColor.Name = "cmbColor";
            this.cmbColor.SelectedColor = System.Drawing.Color.LightGray;
            this.cmbColor.Size = new System.Drawing.Size(140, 24);
            this.cmbColor.TabIndex = 12;
            // 
            // chkVisible
            // 
            this.chkVisible.AutoSize = true;
            this.chkVisible.Location = new System.Drawing.Point(9, 17);
            this.chkVisible.Name = "chkVisible";
            this.chkVisible.Size = new System.Drawing.Size(72, 16);
            this.chkVisible.TabIndex = 10;
            this.chkVisible.Text = "网格可见";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 9;
            this.label1.Text = "颜色：";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 120);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 15;
            this.label3.Text = "风格：";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label4.Location = new System.Drawing.Point(208, 14);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(1, 120);
            this.label4.TabIndex = 16;
            // 
            // GridSetupDialog
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(306, 149);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.rdoLine);
            this.Controls.Add(this.rdoDot);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.updownSize);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chkDrawBorder);
            this.Controls.Add(this.cmbColor);
            this.Controls.Add(this.chkVisible);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GridSetupDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "网格设置";
            ((System.ComponentModel.ISupportInitialize)(this.updownSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion
		#endregion 

		#region ..私有变量
		Grid grid = new Grid(true,10,Color.LightGray,true);
        #endregion

        #region ..公共属性
        /// <summary>
		/// 获取或设置网格属性值
		/// </summary>
		public Grid Grid
		{
			set
			{
				this.chkVisible.Checked = value.Visible;
				this.updownSize.Value = (int)value.Size;
				//this.updownVert.Value = value.Size.Height;
				this.chkDrawBorder.Checked = value.DrawBorder;
				this.cmbColor.SelectedColor = value.Color;
				//this.chkFillScreen.Checked = value.FillScreen;
                this.rdoLine.Checked = value.GridType == GridType.Line;
                this.rdoDot.Checked = value.GridType == GridType.Dot;
			}
			get
			{
                return new Grid(this.chkVisible.Checked, (int)this.updownSize.Value, 
                    this.cmbColor.SelectedColor,
                    this.chkDrawBorder.Checked, 
                    rdoLine.Checked ? GridType.Line : GridType.Dot);//,this.chkFillScreen.Checked );
			}
		}
		#endregion
	}
}
