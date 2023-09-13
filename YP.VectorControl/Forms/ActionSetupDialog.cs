using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace YP.VectorControl.Forms
{
	/// <summary>
	/// ActionSetupDialog 的摘要说明。
	/// </summary>
	internal class ActionSetupDialog : System.Windows.Forms.Form
	{
		#region ..构造及消除
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton rdoNone;
		private System.Windows.Forms.RadioButton rdoMail;
		private System.Windows.Forms.RadioButton rdoHref;
		private System.Windows.Forms.RadioButton rdoFile;
		private System.Windows.Forms.RadioButton rdoExecute;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.TextBox txtMail;
		private System.Windows.Forms.TextBox txtHref;
		private System.Windows.Forms.TextBox txtFile;
		private System.Windows.Forms.Button btnFile;
		private System.Windows.Forms.Button btnProgram;
		private System.Windows.Forms.RadioButton rdoMsg;
		private System.Windows.Forms.TextBox txtMsg;
		private System.Windows.Forms.TextBox txtProgram;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ActionSetupDialog()
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
            this.txtMsg = new System.Windows.Forms.TextBox();
            this.rdoMsg = new System.Windows.Forms.RadioButton();
            this.btnProgram = new System.Windows.Forms.Button();
            this.btnFile = new System.Windows.Forms.Button();
            this.txtProgram = new System.Windows.Forms.TextBox();
            this.txtFile = new System.Windows.Forms.TextBox();
            this.txtHref = new System.Windows.Forms.TextBox();
            this.txtMail = new System.Windows.Forms.TextBox();
            this.rdoExecute = new System.Windows.Forms.RadioButton();
            this.rdoFile = new System.Windows.Forms.RadioButton();
            this.rdoHref = new System.Windows.Forms.RadioButton();
            this.rdoMail = new System.Windows.Forms.RadioButton();
            this.rdoNone = new System.Windows.Forms.RadioButton();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.txtMsg);
            this.groupBox1.Controls.Add(this.rdoMsg);
            this.groupBox1.Controls.Add(this.btnProgram);
            this.groupBox1.Controls.Add(this.btnFile);
            this.groupBox1.Controls.Add(this.txtProgram);
            this.groupBox1.Controls.Add(this.txtFile);
            this.groupBox1.Controls.Add(this.txtHref);
            this.groupBox1.Controls.Add(this.txtMail);
            this.groupBox1.Controls.Add(this.rdoExecute);
            this.groupBox1.Controls.Add(this.rdoFile);
            this.groupBox1.Controls.Add(this.rdoHref);
            this.groupBox1.Controls.Add(this.rdoMail);
            this.groupBox1.Controls.Add(this.rdoNone);
            this.groupBox1.Location = new System.Drawing.Point(11, 10);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(207, 292);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "双击动作";
            // 
            // txtMsg
            // 
            this.txtMsg.Enabled = false;
            this.txtMsg.Location = new System.Drawing.Point(32, 93);
            this.txtMsg.Name = "txtMsg";
            this.txtMsg.Size = new System.Drawing.Size(267, 25);
            this.txtMsg.TabIndex = 12;
            // 
            // rdoMsg
            // 
            this.rdoMsg.Location = new System.Drawing.Point(21, 62);
            this.rdoMsg.Name = "rdoMsg";
            this.rdoMsg.Size = new System.Drawing.Size(139, 31);
            this.rdoMsg.TabIndex = 11;
            this.rdoMsg.Tag = "message";
            this.rdoMsg.Text = "提示消息";
            this.rdoMsg.CheckedChanged += new System.EventHandler(this.rdoHref_CheckedChanged);
            // 
            // btnProgram
            // 
            this.btnProgram.Enabled = false;
            this.btnProgram.Location = new System.Drawing.Point(245, 339);
            this.btnProgram.Name = "btnProgram";
            this.btnProgram.Size = new System.Drawing.Size(64, 30);
            this.btnProgram.TabIndex = 10;
            this.btnProgram.Text = "浏览..";
            this.btnProgram.Click += new System.EventHandler(this.btnFile_Click);
            // 
            // btnFile
            // 
            this.btnFile.Enabled = false;
            this.btnFile.Location = new System.Drawing.Point(245, 278);
            this.btnFile.Name = "btnFile";
            this.btnFile.Size = new System.Drawing.Size(64, 29);
            this.btnFile.TabIndex = 9;
            this.btnFile.Text = "浏览...";
            this.btnFile.Click += new System.EventHandler(this.btnFile_Click);
            // 
            // txtProgram
            // 
            this.txtProgram.Enabled = false;
            this.txtProgram.Location = new System.Drawing.Point(32, 339);
            this.txtProgram.Name = "txtProgram";
            this.txtProgram.Size = new System.Drawing.Size(213, 25);
            this.txtProgram.TabIndex = 8;
            // 
            // txtFile
            // 
            this.txtFile.Enabled = false;
            this.txtFile.Location = new System.Drawing.Point(32, 278);
            this.txtFile.Name = "txtFile";
            this.txtFile.Size = new System.Drawing.Size(213, 25);
            this.txtFile.TabIndex = 7;
            // 
            // txtHref
            // 
            this.txtHref.Enabled = false;
            this.txtHref.Location = new System.Drawing.Point(32, 216);
            this.txtHref.Name = "txtHref";
            this.txtHref.Size = new System.Drawing.Size(267, 25);
            this.txtHref.TabIndex = 6;
            // 
            // txtMail
            // 
            this.txtMail.Enabled = false;
            this.txtMail.Location = new System.Drawing.Point(32, 154);
            this.txtMail.Name = "txtMail";
            this.txtMail.Size = new System.Drawing.Size(267, 25);
            this.txtMail.TabIndex = 5;
            // 
            // rdoExecute
            // 
            this.rdoExecute.Location = new System.Drawing.Point(21, 309);
            this.rdoExecute.Name = "rdoExecute";
            this.rdoExecute.Size = new System.Drawing.Size(139, 30);
            this.rdoExecute.TabIndex = 4;
            this.rdoExecute.Tag = "ExecuteProgram";
            this.rdoExecute.Text = "执行程序";
            this.rdoExecute.CheckedChanged += new System.EventHandler(this.rdoHref_CheckedChanged);
            // 
            // rdoFile
            // 
            this.rdoFile.Location = new System.Drawing.Point(21, 247);
            this.rdoFile.Name = "rdoFile";
            this.rdoFile.Size = new System.Drawing.Size(139, 31);
            this.rdoFile.TabIndex = 3;
            this.rdoFile.Tag = "OpenFile";
            this.rdoFile.Text = "打开文件";
            this.rdoFile.CheckedChanged += new System.EventHandler(this.rdoHref_CheckedChanged);
            // 
            // rdoHref
            // 
            this.rdoHref.Location = new System.Drawing.Point(21, 185);
            this.rdoHref.Name = "rdoHref";
            this.rdoHref.Size = new System.Drawing.Size(139, 31);
            this.rdoHref.TabIndex = 2;
            this.rdoHref.Tag = "OpenHref";
            this.rdoHref.Text = "打开网页";
            this.rdoHref.CheckedChanged += new System.EventHandler(this.rdoHref_CheckedChanged);
            // 
            // rdoMail
            // 
            this.rdoMail.Location = new System.Drawing.Point(21, 123);
            this.rdoMail.Name = "rdoMail";
            this.rdoMail.Size = new System.Drawing.Size(139, 31);
            this.rdoMail.TabIndex = 1;
            this.rdoMail.Tag = "mail";
            this.rdoMail.Text = "发送电子邮件";
            this.rdoMail.CheckedChanged += new System.EventHandler(this.rdoHref_CheckedChanged);
            // 
            // rdoNone
            // 
            this.rdoNone.Location = new System.Drawing.Point(21, 31);
            this.rdoNone.Name = "rdoNone";
            this.rdoNone.Size = new System.Drawing.Size(139, 31);
            this.rdoNone.TabIndex = 0;
            this.rdoNone.Tag = "none";
            this.rdoNone.Text = "无";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(228, 230);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 29);
            this.button1.TabIndex = 1;
            this.button1.Text = "确  定";
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(228, 271);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 30);
            this.button2.TabIndex = 2;
            this.button2.Text = "取  消";
            // 
            // ActionSetupDialog
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleBaseSize = new System.Drawing.Size(8, 18);
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(338, 312);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ActionSetupDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "动作";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion
		#endregion

		#region ..公共属性
		public YP.SVG.ClickAction Action
		{
			set
			{
				TextBox box = null;
				switch(value.Type)
				{
					case YP.SVG.ActionType.None:
						this.rdoNone.Checked = true;
						break;
					case YP.SVG.ActionType.Mail:
						this.rdoMail.Checked = true;
						box = this.txtMail;
						break;
					case YP.SVG.ActionType.OpenFile:
						this.rdoFile.Checked = true;
						box = this.txtFile;
						break;
					case YP.SVG.ActionType.Message:
						this.rdoMsg.Checked = true;
						box = this.txtMsg;
						break;
					case YP.SVG.ActionType.ExecuteProgram:
						this.rdoExecute.Checked = true;
						box = this.txtProgram;
						break;
					case YP.SVG.ActionType.OpenHref:
						this.rdoHref.Checked = true;
						box = this.txtHref;
						break;
				}
				if(box != null)
					box.Text = value.ActionArgs;
			}
			get
			{
				YP.SVG.ActionType type = YP.SVG.ActionType.None;
				string arg = null;
				if(this.rdoMsg.Checked)
				{
					type = YP.SVG.ActionType.Message;
					arg = this.txtMsg.Text;
				}
				else if(this.rdoMail.Checked)
				{
					type = YP.SVG.ActionType.Mail;
					arg = this.txtMail.Text;
				}
				else if(this.rdoExecute.Checked)
				{
					type = YP.SVG.ActionType.ExecuteProgram;
					arg = this.txtProgram.Text;
				}
				else if(this.rdoFile.Checked)
				{
					type = YP.SVG.ActionType.OpenFile;
					arg = this.txtFile.Text;
				}
				else if(this.rdoHref.Checked)
				{
					type = YP.SVG.ActionType.OpenHref;
					arg = this.txtHref.Text;
				}
				
				return new YP.SVG.ClickAction(type,arg);
			}
		}
		#endregion

		#region ..选项发生改变
		private void rdoHref_CheckedChanged(object sender, System.EventArgs e)
		{
			this.txtMsg.Enabled = this.rdoMsg.Checked;
			this.btnFile.Enabled = this.txtFile.Enabled = this.rdoFile.Checked;
			this.btnProgram.Enabled = this.txtProgram.Enabled = this.rdoExecute.Checked;
			this.txtHref.Enabled = this.rdoHref.Checked;
			this.txtMail.Enabled = this.rdoMail.Checked;
		}
		#endregion

		#region ..打开文件
		private void btnFile_Click(object sender, System.EventArgs e)
		{
			this.openFileDialog1.Filter = string.Empty;
			TextBox box = this.txtFile;
			if((sender as Button) == this.btnProgram)
			{
				this.openFileDialog1.Filter = "可执行文件(*.exe)|*.exe|全部文件(*.*)|*.*";
				box = this.txtProgram;
			}
			this.openFileDialog1.FileName = box.Text;
			if(this.openFileDialog1.ShowDialog(this) == DialogResult.OK)
				box.Text = this.openFileDialog1.FileName;
		}
		#endregion
	}
}
