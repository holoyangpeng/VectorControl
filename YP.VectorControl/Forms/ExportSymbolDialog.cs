using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace YP.VectorControl.Forms
{
	/// <summary>
	/// ExportSymbolDialog 的摘要说明。
	/// </summary>
	internal class ExportSymbolDialog : System.Windows.Forms.Form
	{
		#region ..构造及消除
		private System.Windows.Forms.RichTextBox richTextBox1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.CheckBox chkDocument;
		private System.Windows.Forms.CheckBox chkShape;
		private System.Windows.Forms.GroupBox groupbox2;
		private System.Windows.Forms.TextBox txtID;
		private System.Windows.Forms.CheckBox chkSelection;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		bool first = true;
		private System.Windows.Forms.CheckBox checkBox1;
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ExportSymbolDialog(Canvas vectorcontrol,string filefilter)
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();
			this.saveFileDialog1.Filter = "SVG文件(*.svg)|*.svg";
			if(filefilter.Trim().Length > 0)
				this.saveFileDialog1.Filter += "|" + filefilter;
			this.vectorControl = vectorcontrol;
//			if(this.vectorControl != null)
//			{
//				this.symbolElement = this.vectorControl.ExportSymbolElement(true,false,true);
//				this.shapeElement = this.vectorControl.ExportSymbolElement(true,true,true);
//				this.selectshapeElement = this.vectorControl.ExportSymbolElement(false,true,true);
//				this.selectsymbolElement = this.vectorControl.ExportSymbolElement(false,false,true);
//			}
//			this.button1_Click(this.button1,EventArgs.Empty);
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
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.groupbox2 = new System.Windows.Forms.GroupBox();
			this.chkShape = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.txtID = new System.Windows.Forms.TextBox();
			this.chkDocument = new System.Windows.Forms.CheckBox();
			this.chkSelection = new System.Windows.Forms.CheckBox();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.button4 = new System.Windows.Forms.Button();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.groupbox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// richTextBox1
			// 
			this.richTextBox1.Location = new System.Drawing.Point(8, 8);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.Size = new System.Drawing.Size(232, 280);
			this.richTextBox1.TabIndex = 0;
			this.richTextBox1.Text = "";
			this.richTextBox1.WordWrap = false;
			// 
			// groupbox2
			// 
			this.groupbox2.Controls.Add(this.chkShape);
			this.groupbox2.Controls.Add(this.label2);
			this.groupbox2.Controls.Add(this.button1);
			this.groupbox2.Controls.Add(this.txtID);
			this.groupbox2.Controls.Add(this.chkDocument);
			this.groupbox2.Controls.Add(this.chkSelection);
			this.groupbox2.Location = new System.Drawing.Point(248, 40);
			this.groupbox2.Name = "groupbox2";
			this.groupbox2.Size = new System.Drawing.Size(168, 152);
			this.groupbox2.TabIndex = 1;
			this.groupbox2.TabStop = false;
			this.groupbox2.Text = "导出";
			// 
			// chkShape
			// 
			this.chkShape.Location = new System.Drawing.Point(16, 120);
			this.chkShape.Name = "chkShape";
			this.chkShape.Size = new System.Drawing.Size(136, 24);
			this.chkShape.TabIndex = 6;
			this.chkShape.Text = "创建自定义形状";
			this.chkShape.CheckedChanged += new System.EventHandler(this.button1_Click);
			// 
			// label2
			// 
			this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label2.Location = new System.Drawing.Point(8, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(150, 2);
			this.label2.TabIndex = 5;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(96, 24);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(64, 23);
			this.button1.TabIndex = 4;
			this.button1.Text = "修改名称";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// txtID
			// 
			this.txtID.Location = new System.Drawing.Point(8, 24);
			this.txtID.MaxLength = 20;
			this.txtID.Name = "txtID";
			this.txtID.Size = new System.Drawing.Size(80, 21);
			this.txtID.TabIndex = 2;
			this.txtID.Text = "Preset1";
			// 
			// chkDocument
			// 
			this.chkDocument.Location = new System.Drawing.Point(16, 96);
			this.chkDocument.Name = "chkDocument";
			this.chkDocument.Size = new System.Drawing.Size(136, 24);
			this.chkDocument.TabIndex = 1;
			this.chkDocument.Text = "产生完整文档代码";
			this.chkDocument.CheckedChanged += new System.EventHandler(this.button1_Click);
			// 
			// chkSelection
			// 
			this.chkSelection.Checked = true;
			this.chkSelection.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkSelection.Location = new System.Drawing.Point(16, 72);
			this.chkSelection.Name = "chkSelection";
			this.chkSelection.Size = new System.Drawing.Size(136, 24);
			this.chkSelection.TabIndex = 0;
			this.chkSelection.Text = "只包含选区";
			this.chkSelection.CheckedChanged += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(256, 200);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(152, 24);
			this.button2.TabIndex = 2;
			this.button2.Text = "追加到文件(&A)";
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(256, 232);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(152, 24);
			this.button3.TabIndex = 3;
			this.button3.Text = "保存到文件(&S)";
			this.button3.Click += new System.EventHandler(this.button2_Click);
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(256, 264);
			this.button4.Name = "button4";
			this.button4.Size = new System.Drawing.Size(152, 24);
			this.button4.TabIndex = 4;
			this.button4.Text = "关　　　闭(&C)";
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// checkBox1
			// 
			this.checkBox1.Location = new System.Drawing.Point(256, 8);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(152, 24);
			this.checkBox1.TabIndex = 5;
			this.checkBox1.Text = "代码自动折行";
			this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
			// 
			// ExportSymbolDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(426, 295);
			this.Controls.Add(this.checkBox1);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.groupbox2);
			this.Controls.Add(this.richTextBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ExportSymbolDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "导出图元";
			this.groupbox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		#endregion

		#region ..私有变量
		Canvas vectorControl = null;
		YP.SVG.SVGElement symbolElement = null;
		YP.SVG.SVGElement shapeElement = null;
		YP.SVG.SVGElement selectsymbolElement = null;
		YP.SVG.SVGElement selectshapeElement = null;
		#endregion

		#region ..预览代码
		private void button1_Click(object sender, System.EventArgs e)
		{
			this.first = false;
			YP.SVG.SVGElement mainElement = null;
			if(this.chkSelection.Checked)
			{
				if(this.selectsymbolElement == null)
					this.selectsymbolElement = this.vectorControl.ExportSymbolElement(false,false,true);

				mainElement = this.selectsymbolElement;
				if(this.chkShape.Checked)
				{
					if(this.selectshapeElement == null)
						this.selectshapeElement = this.vectorControl.ExportSymbolElement(false,true,true);
					mainElement = this.selectshapeElement;
				}
			}
			else
			{
				if(this.symbolElement == null)
					this.symbolElement = this.vectorControl.ExportSymbolElement(true,false,true);
				mainElement = this.symbolElement;
				if(this.chkShape.Checked)
				{
					if(this.shapeElement == null)
						this.shapeElement = this.vectorControl.ExportSymbolElement(true,true,true);
					mainElement = this.shapeElement;
				}
			}
			if(mainElement != null)
			{
				for(int i = 0;i<mainElement.ChildNodes.Count;i++)
				{
					if(mainElement.ChildNodes[i] is YP.SVG.SVGElement)
					{
						(mainElement.ChildNodes[i] as YP.SVG.SVGElement).InternalSetAttribute("id",this.txtID.Text.Trim());
						if(!this.chkDocument.Checked)
							mainElement = mainElement.ChildNodes[i] as YP.SVG.SVGElement;
						break;
					}
				}
			}
			string temp = string.Empty;
			if(mainElement != null)
				temp = mainElement.OuterXml;
			if(this.chkDocument.Checked)
				temp = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + temp;
			this.richTextBox1.Text = temp;
		}
		#endregion

		#region ..关闭
		private void button4_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}
		#endregion

		#region ..保存
		private void button2_Click(object sender, System.EventArgs e)
		{
			Button btn = sender as Button;
			this.saveFileDialog1.FileName = this.txtID.Text.Trim();
			if(this.saveFileDialog1.ShowDialog(this) == DialogResult.OK)
			{
				System.IO.StreamWriter writer = null;
				string filename = this.saveFileDialog1.FileName;
				try
				{
					
					writer = new System.IO.StreamWriter(filename,btn == this.button2,System.Text.Encoding.UTF8);
					writer.Write(this.richTextBox1.Text);
				}
				catch(System.Exception e1)
				{
					MessageBox.Show(e1.Message);
				}
				finally
				{
					if(writer != null)
						writer.Close();
				}
			}
		}
		#endregion

		#region ..VisibleChanged
		protected override void OnVisibleChanged(EventArgs e)
		{
			if(this.first && this.Visible)
				this.button1_Click(this.button1,EventArgs.Empty);
			base.OnVisibleChanged (e);
		}
		#endregion

		#region ..自动折行
		private void checkBox1_CheckedChanged(object sender, System.EventArgs e)
		{
			this.richTextBox1.WordWrap = this.checkBox1.Checked;
		}
		#endregion
	}
}
