using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Printing;

namespace YP.VectorControl.Forms
{
	/// <summary>
	/// PrintDialog 的摘要说明。
	/// </summary>
	internal class PrintDialog : System.Windows.Forms.Form
	{
		#region ..构造及消除

		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.CheckBox chkLock;
		private System.Windows.Forms.NumericUpDown updownHeight;
		private System.Windows.Forms.NumericUpDown updownWidth;
		private System.Windows.Forms.RadioButton radioCustom;
		private System.Windows.Forms.RadioButton radioFit;
		private System.Windows.Forms.RadioButton radioNoSale;
		private System.Windows.Forms.Button btnPrint;
		private System.Windows.Forms.Button btnSe0Up;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton rdoHighQuality;
		private System.Windows.Forms.RadioButton rdoHighSpeed;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public PrintDialog(Canvas vectorcontrol)
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();
			this.vectorControl = vectorcontrol;

			this.label8.Cursor = Cursors.Hand;

			
			this.pageSetting = new PageSettings();
			this.printdoc = new PrintDocument();
			this.printdoc.PrintPage += new PrintPageEventHandler(printdoc_PrintPage);
			this.CalculateScale();
			
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
			this.label8 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.chkLock = new System.Windows.Forms.CheckBox();
			this.updownHeight = new System.Windows.Forms.NumericUpDown();
			this.updownWidth = new System.Windows.Forms.NumericUpDown();
			this.radioCustom = new System.Windows.Forms.RadioButton();
			this.radioFit = new System.Windows.Forms.RadioButton();
			this.radioNoSale = new System.Windows.Forms.RadioButton();
			this.btnPrint = new System.Windows.Forms.Button();
			this.btnSe0Up = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.rdoHighSpeed = new System.Windows.Forms.RadioButton();
			this.rdoHighQuality = new System.Windows.Forms.RadioButton();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.updownHeight)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.updownWidth)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label8
			// 
			this.label8.BackColor = System.Drawing.SystemColors.ControlDark;
			this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.label8.Location = new System.Drawing.Point(8, 8);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(240, 320);
			this.label8.TabIndex = 4;
			this.label8.Paint += new System.Windows.Forms.PaintEventHandler(this.label8_Paint);
			this.label8.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label8_MouseMove);
			this.label8.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label8_MouseDown);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.chkLock);
			this.groupBox2.Controls.Add(this.updownHeight);
			this.groupBox2.Controls.Add(this.updownWidth);
			this.groupBox2.Controls.Add(this.radioCustom);
			this.groupBox2.Controls.Add(this.radioFit);
			this.groupBox2.Controls.Add(this.radioNoSale);
			this.groupBox2.Location = new System.Drawing.Point(256, 8);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(304, 120);
			this.groupBox2.TabIndex = 8;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "缩 放";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(104, 88);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(16, 23);
			this.label7.TabIndex = 7;
			this.label7.Text = "W";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(176, 88);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(16, 23);
			this.label6.TabIndex = 6;
			this.label6.Text = "H";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// chkLock
			// 
			this.chkLock.Checked = true;
			this.chkLock.CheckState = System.Windows.Forms.CheckState.Checked;
			this.chkLock.Enabled = false;
			this.chkLock.Location = new System.Drawing.Point(248, 88);
			this.chkLock.Name = "chkLock";
			this.chkLock.Size = new System.Drawing.Size(48, 24);
			this.chkLock.TabIndex = 5;
			this.chkLock.Text = "锁定";
			this.chkLock.CheckedChanged += new System.EventHandler(this.chkLock_CheckedChanged);
			// 
			// updownHeight
			// 
			this.updownHeight.Enabled = false;
			this.updownHeight.Location = new System.Drawing.Point(192, 88);
			this.updownHeight.Maximum = new System.Decimal(new int[] {
																		 1000,
																		 0,
																		 0,
																		 0});
			this.updownHeight.Name = "updownHeight";
			this.updownHeight.Size = new System.Drawing.Size(48, 21);
			this.updownHeight.TabIndex = 4;
			this.updownHeight.Value = new System.Decimal(new int[] {
																	   100,
																	   0,
																	   0,
																	   0});
			this.updownHeight.ValueChanged += new System.EventHandler(this.updownHeight_ValueChanged);
			// 
			// updownWidth
			// 
			this.updownWidth.Enabled = false;
			this.updownWidth.Location = new System.Drawing.Point(120, 88);
			this.updownWidth.Maximum = new System.Decimal(new int[] {
																		1000,
																		0,
																		0,
																		0});
			this.updownWidth.Name = "updownWidth";
			this.updownWidth.Size = new System.Drawing.Size(48, 21);
			this.updownWidth.TabIndex = 3;
			this.updownWidth.Value = new System.Decimal(new int[] {
																	  100,
																	  0,
																	  0,
																	  0});
			this.updownWidth.ValueChanged += new System.EventHandler(this.updownHeight_ValueChanged);
			// 
			// radioCustom
			// 
			this.radioCustom.Location = new System.Drawing.Point(16, 88);
			this.radioCustom.Name = "radioCustom";
			this.radioCustom.Size = new System.Drawing.Size(64, 24);
			this.radioCustom.TabIndex = 2;
			this.radioCustom.Text = "自定义";
			this.radioCustom.CheckedChanged += new System.EventHandler(this.radioCustom_CheckedChanged);
			// 
			// radioFit
			// 
			this.radioFit.Location = new System.Drawing.Point(16, 56);
			this.radioFit.Name = "radioFit";
			this.radioFit.TabIndex = 1;
			this.radioFit.Text = "适应页面";
			this.radioFit.CheckedChanged += new System.EventHandler(this.radioCustom_CheckedChanged);
			// 
			// radioNoSale
			// 
			this.radioNoSale.Checked = true;
			this.radioNoSale.Location = new System.Drawing.Point(16, 24);
			this.radioNoSale.Name = "radioNoSale";
			this.radioNoSale.TabIndex = 0;
			this.radioNoSale.TabStop = true;
			this.radioNoSale.Text = "不缩放";
			this.radioNoSale.CheckedChanged += new System.EventHandler(this.radioCustom_CheckedChanged);
			// 
			// btnPrint
			// 
			this.btnPrint.Location = new System.Drawing.Point(376, 296);
			this.btnPrint.Name = "btnPrint";
			this.btnPrint.Size = new System.Drawing.Size(80, 24);
			this.btnPrint.TabIndex = 9;
			this.btnPrint.Text = "打印";
			this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
			// 
			// btnSe0Up
			// 
			this.btnSe0Up.Location = new System.Drawing.Point(272, 296);
			this.btnSe0Up.Name = "btnSe0Up";
			this.btnSe0Up.Size = new System.Drawing.Size(80, 24);
			this.btnSe0Up.TabIndex = 10;
			this.btnSe0Up.Text = "打印设置";
			this.btnSe0Up.Click += new System.EventHandler(this.btnSe0Up_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.rdoHighSpeed);
			this.groupBox1.Controls.Add(this.rdoHighQuality);
			this.groupBox1.Location = new System.Drawing.Point(256, 152);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(304, 100);
			this.groupBox1.TabIndex = 11;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "质量";
			// 
			// rdoHighSpeed
			// 
			this.rdoHighSpeed.Location = new System.Drawing.Point(16, 64);
			this.rdoHighSpeed.Name = "rdoHighSpeed";
			this.rdoHighSpeed.TabIndex = 1;
			this.rdoHighSpeed.Text = "低质量";
			this.rdoHighSpeed.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
			// 
			// rdoHighQuality
			// 
			this.rdoHighQuality.Checked = true;
			this.rdoHighQuality.Location = new System.Drawing.Point(16, 24);
			this.rdoHighQuality.Name = "rdoHighQuality";
			this.rdoHighQuality.TabIndex = 0;
			this.rdoHighQuality.TabStop = true;
			this.rdoHighQuality.Text = "高质量";
			this.rdoHighQuality.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(272, 256);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(80, 24);
			this.button1.TabIndex = 12;
			this.button1.Text = "预览";
			this.button1.Visible = false;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button2.Location = new System.Drawing.Point(480, 296);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(80, 24);
			this.button2.TabIndex = 13;
			this.button2.Text = "关闭";
			// 
			// PrintDialog
			// 
			this.AcceptButton = this.btnPrint;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.CancelButton = this.button2;
			this.ClientSize = new System.Drawing.Size(570, 335);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.btnSe0Up);
			this.Controls.Add(this.btnPrint);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.label8);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PrintDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "打印";
			this.groupBox2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.updownHeight)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.updownWidth)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		#endregion

		#region ..私有变量
		Canvas vectorControl = null;
		PageSettings pageSetting = null;
		int margin = 10;
		System.Drawing.Printing.PrintDocument printdoc = null;
		Point pos = Point.Empty;
		float scalex = 1,scaley = 1;
		Point startPoint = Point.Empty;
		Point oriPoint = Point.Empty;
		string info = string.Empty;
		#endregion

		#region ..缩放选项改变
		private void radioCustom_CheckedChanged(object sender, System.EventArgs e)
		{
			this.updownWidth.Enabled = this.updownHeight.Enabled = this.chkLock.Enabled = this.radioCustom.Checked;
			this.CalculateScale();
			this.label8.Invalidate();
		}
		#endregion

		#region ..自定义缩放
		private void updownHeight_ValueChanged(object sender, System.EventArgs e)
		{
			if(this.chkLock.Checked)
				this.updownHeight.Value = this.updownWidth.Value = (sender as NumericUpDown).Value;
			this.CalculateScale();
			this.label8.Invalidate();
		}
		#endregion		

		#region ..锁定纵横比
		private void chkLock_CheckedChanged(object sender, System.EventArgs e)
		{
			this.updownHeight.Value = this.updownWidth.Value;
		}
		#endregion

		#region ..打印设置
		private void btnSe0Up_Click(object sender, System.EventArgs e)
		{
			PageSetupDialog psDlg = new PageSetupDialog() ;
			psDlg.PageSettings = this.pageSetting ;
			if(psDlg.ShowDialog(this) == DialogResult.OK)
			{
				this.pageSetting =psDlg.PageSettings;
				this.label8.Invalidate();
			}
		}
		#endregion

		#region ..预览
		private void label8_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Label lb = sender as Label;
			Rectangle bounds = this.pageSetting.Bounds;
			int width = lb.Width;
			int height = lb.Height;
			float sy = (float)(height - 2 *margin)/ (float)bounds.Height;
			float sx = (float)(width - 2 * margin) / (float)bounds.Width;

			Rectangle rect = new Rectangle(margin,margin,(int)(sx * bounds.Width),(int)(sy * bounds.Height));
			e.Graphics.FillRectangle(Brushes.White,rect);
			e.Graphics.DrawRectangle(Pens.Black,rect);
			
			System.Drawing.Printing.Margins ms = this.pageSetting.Margins ;
			int left = (int)(margin + sx * ms.Left);
			int top = (int)(margin + sy * ms.Top);
			width = (int)(sx * bounds.Width - sx * ms.Left - sx * ms.Right);
			height = (int)(sy * bounds.Height - sy * ms.Top - sy * ms.Bottom);
			rect = new Rectangle(left,top,width,height);
			using(Pen pen = new Pen(Color.Blue,1f))
			{
				
				e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
				if(this.rdoHighSpeed.Checked)
					e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
				Size size = this.vectorControl.DocumentSize;
				e.Graphics.TranslateTransform(-this.pos.X,-this.pos.Y);
				e.Graphics.TranslateTransform(rect.X,rect.Y);
				e.Graphics.ScaleTransform(this.scalex,this.scaley);
				this.vectorControl.RenderTo(e.Graphics); 
				e.Graphics.ResetTransform();
				e.Graphics.ResetClip();
				pen.DashPattern = new float[]{2,2};
				pen.Color = Color.Black;
				e.Graphics.DrawRectangle(pen,-pos.X + rect.X,-pos.Y + rect.Y,size.Width * this.scalex,size.Height * this.scaley);
				pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
				pen.Color = Color.Blue;
				e.Graphics.DrawRectangle(pen,rect);
				
			}
		}
		#endregion

		#region ..质量改变
		private void radioButton1_CheckedChanged(object sender, System.EventArgs e)
		{
			this.label8.Invalidate();
		}	
		#endregion

		#region ..打印
		private void btnPrint_Click(object sender, System.EventArgs e)
		{
			System.Windows.Forms.PrintDialog print = new System.Windows.Forms.PrintDialog();
			this.printdoc.DefaultPageSettings = this.pageSetting;
			print.Document = this.printdoc;
			if(print.ShowDialog(this) == DialogResult.OK)
			{
				this.printdoc.Print();
				this.Close();
			}
		}
		#endregion

		#region ..设置打印文档
		private void printdoc_PrintPage(object sender, PrintPageEventArgs e)
		{
			Rectangle bounds = this.pageSetting.Bounds;
			float scale  = 1f;
			System.Drawing.Printing.Margins ms = this.pageSetting.Margins ;
			Size size = this.vectorControl.DocumentSize;
			float sy = (float)(this.label8.Height - 2 *margin)/ (float)bounds.Height;
			float sx = (float)(this.label8.Width - 2 * margin) / (float)bounds.Width;
			int left = (int)(scale * ms.Left);
			int top = (int)(scale * ms.Top);
			Rectangle rect = new Rectangle(left,top,bounds.Width -ms.Left - ms.Right,bounds.Height - ms.Top - ms.Right);
			
			e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			if(this.rdoHighSpeed.Checked)
				e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
			e.Graphics.TranslateTransform(-this.pos.X / sx,-this.pos.Y / sy);
			e.Graphics.TranslateTransform(left,top);
			e.Graphics.ScaleTransform(scalex/sx,scaley/sy);
			this.vectorControl.RenderTo(e.Graphics);
		}
		#endregion

		#region ..预打印
		private void button1_Click(object sender, System.EventArgs e)
		{
			System.Windows.Forms.PrintPreviewDialog dlg = new PrintPreviewDialog();
			this.printdoc.DefaultPageSettings = this.pageSetting;
			dlg.Document = this.printdoc;
			dlg.ShowDialog(this);
		}
		#endregion

		#region ..移动位置
		private void label8_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Left)
			{
				int x = this.oriPoint.X;
				int y = this.oriPoint.Y;
				int deltax = e.X - this.startPoint.X;
				int deltay = e.Y - this.startPoint.Y;
				x = x - deltax;
				y = y - deltay;
				Point p = new Point(x,y);
				if(p != this.pos)
				{
					this.pos = p;
					this.label8.Invalidate();
				}
			}
		}

		private void label8_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			this.startPoint = Point.Empty;
			if(e.Button == MouseButtons.Left)
				this.startPoint = new Point(e.X,e.Y);
			this.oriPoint = this.pos;
		}
		#endregion 

		#region ..计算缩放比例
		void CalculateScale()
		{
			Rectangle bounds = Rectangle.Empty;
			if(this.pageSetting != null)
				bounds = this.pageSetting.Bounds;
			int width = this.label8.Width;
			int height = label8.Height;
			scaley = (float)(height - 2 *margin)/ (float)bounds.Height;
			scalex = (float)(width - 2 * margin) / (float)bounds.Width;
			System.Drawing.Printing.Margins ms = this.pageSetting.Margins ;
			int left = (int)(margin + scalex * ms.Left);
			int top = (int)(margin + scaley * ms.Top);
			width = (int)(scalex * bounds.Width - scalex * ms.Left - scalex * ms.Right);
			height = (int)(scaley * bounds.Height - scaley * ms.Top - scaley * ms.Bottom);
			Rectangle rect = new Rectangle(left,top,width,height);
			Size size = this.vectorControl.DocumentSize;
			if(this.radioFit.Checked)
			{
				scalex = (float)rect.Width / (float)size.Width;
				scaley = (float)rect.Height / (float)size.Height;
			}
			else if(this.radioCustom.Checked)
			{
				scalex *= (float)this.updownWidth.Value / 100f;
				scaley *= (float)this.updownHeight.Value / 100f;
			}
		}
		#endregion		
	}
}
