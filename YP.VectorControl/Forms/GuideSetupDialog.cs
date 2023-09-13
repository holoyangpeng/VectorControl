using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace YP.VectorControl.Forms
{
	/// <summary>
	/// <para>辅助线属性的编辑对话框,可以以可视化的方式编辑和设置<see cref="Guide">Guide</see>的相关参数。</para>
	/// <para>可以利用该对话框改变VectorControl的<see cref="VectorControl.Guide">参考线(Guide)</see>参数设置。</para>
	/// </summary>
	public class GuideSetupDialog : System.Windows.Forms.Form
	{
		#region ..私有变量
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
        private System.Windows.Forms.CheckBox chkVisible;
		private System.Windows.Forms.CheckBox chkLock;
		private ComboColors cmbColor;
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public GuideSetupDialog()
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.cmbColor = new ComboColors();
            this.chkLock = new System.Windows.Forms.CheckBox();
            this.chkVisible = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.cmbColor);
            this.groupBox1.Controls.Add(this.chkLock);
            this.groupBox1.Controls.Add(this.chkVisible);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(11, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(274, 74);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            // 
            // cmbColor
            // 
            this.cmbColor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbColor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbColor.ItemHeight = 18;
            this.cmbColor.Location = new System.Drawing.Point(53, 31);
            this.cmbColor.Name = "cmbColor";
            this.cmbColor.SelectedColor = System.Drawing.Color.Blue;
            this.cmbColor.Size = new System.Drawing.Size(299, 24);
            this.cmbColor.TabIndex = 4;
            // 
            // chkLock
            // 
            this.chkLock.Location = new System.Drawing.Point(203, 64);
            this.chkLock.Name = "chkLock";
            this.chkLock.Size = new System.Drawing.Size(149, 31);
            this.chkLock.TabIndex = 3;
            this.chkLock.Text = "锁定参考线";
            // 
            // chkVisible
            // 
            this.chkVisible.Location = new System.Drawing.Point(13, 64);
            this.chkVisible.Name = "chkVisible";
            this.chkVisible.Size = new System.Drawing.Size(128, 31);
            this.chkVisible.TabIndex = 1;
            this.chkVisible.Text = "参考线可见";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(11, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 29);
            this.label1.TabIndex = 0;
            this.label1.Text = "颜色";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(185, 97);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 30);
            this.button1.TabIndex = 1;
            this.button1.Text = "确定(&O)";
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(56, 97);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 30);
            this.button2.TabIndex = 2;
            this.button2.Text = "取消(&C)";
            // 
            // GuideSetupDialog
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleBaseSize = new System.Drawing.Size(8, 18);
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(298, 137);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GuideSetupDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "辅助线设置";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion
		#endregion

		#region ..私有变量
		Guide guide = new Guide(true,false,Color.Blue);
		#endregion

		#region ..公共属性
		/// <summary>
		/// 获取或设置参考线设置
		/// </summary>
		public Guide Guide
		{
			set
			{
				this.chkVisible.Checked = value.Visible;
				this.chkLock.Checked = value.Lock;
				this.cmbColor.SelectedColor = value.Color;
			}
			get
			{
				return new Guide(this.chkVisible.Checked,this.chkLock.Checked,cmbColor.SelectedColor);
			}
		}
		#endregion
	}
}
