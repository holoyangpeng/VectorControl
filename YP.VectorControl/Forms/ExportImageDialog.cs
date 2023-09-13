using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace YP.VectorControl.Forms
{
	/// <summary>
	/// ExportImageDialog 的摘要说明。
	/// </summary>
	internal class ExportImageDialog : System.Windows.Forms.Form
	{
		#region ..构造及消除
		private System.Windows.Forms.Label panel1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButton1;
		private System.Windows.Forms.RadioButton radioButton2;
		private System.Windows.Forms.ComboBox comboFormat;
		private System.Windows.Forms.CheckBox chkSelection;
		private System.Windows.Forms.CheckBox chkView;
		private System.Windows.Forms.CheckBox chkTransparent;
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ExportImageDialog(Canvas vectorcontrol)
		{
			//
			// Windows 窗体设计器支持所必需的
			//
			InitializeComponent();
			SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint,true);

			this.vectorControl = vectorcontrol;

			this.comboFormat.SelectedIndex = 0;

			this.panel1.Cursor = Cursors.Hand;
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
            this.panel1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.comboFormat = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.chkSelection = new System.Windows.Forms.CheckBox();
            this.chkView = new System.Windows.Forms.CheckBox();
            this.chkTransparent = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.DarkGray;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.panel1.Location = new System.Drawing.Point(8, 8);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(346, 289);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(378, 209);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(112, 32);
            this.button1.TabIndex = 1;
            this.button1.Text = "确     定";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(378, 257);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(112, 32);
            this.button2.TabIndex = 2;
            this.button2.Text = "取     消";
            // 
            // comboFormat
            // 
            this.comboFormat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboFormat.Items.AddRange(new object[] {
            "Bmp",
            "Jpg",
            "Gif",
            "Tiff",
            "Png",
            "Emf",
            "Wmf"});
            this.comboFormat.Location = new System.Drawing.Point(408, 18);
            this.comboFormat.Name = "comboFormat";
            this.comboFormat.Size = new System.Drawing.Size(88, 20);
            this.comboFormat.TabIndex = 4;
            this.comboFormat.SelectedIndexChanged += new System.EventHandler(this.comboFormat_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(360, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 23);
            this.label1.TabIndex = 3;
            this.label1.Text = "格式";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.radioButton1);
            this.groupBox1.Location = new System.Drawing.Point(360, 50);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(144, 80);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "质量";
            // 
            // radioButton2
            // 
            this.radioButton2.Location = new System.Drawing.Point(20, 48);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(104, 24);
            this.radioButton2.TabIndex = 1;
            this.radioButton2.Text = "低质量";
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.Checked = true;
            this.radioButton1.Location = new System.Drawing.Point(20, 24);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(104, 24);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "高质量";
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // chkSelection
            // 
            this.chkSelection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkSelection.Location = new System.Drawing.Point(380, 179);
            this.chkSelection.Name = "chkSelection";
            this.chkSelection.Size = new System.Drawing.Size(128, 24);
            this.chkSelection.TabIndex = 6;
            this.chkSelection.Text = "只绘制选区内容";
            this.chkSelection.Visible = false;
            this.chkSelection.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // chkView
            // 
            this.chkView.Location = new System.Drawing.Point(380, 219);
            this.chkView.Name = "chkView";
            this.chkView.Size = new System.Drawing.Size(128, 24);
            this.chkView.TabIndex = 7;
            this.chkView.Text = "只绘制视图内容";
            this.chkView.Visible = false;
            this.chkView.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // chkTransparent
            // 
            this.chkTransparent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkTransparent.Location = new System.Drawing.Point(368, 138);
            this.chkTransparent.Name = "chkTransparent";
            this.chkTransparent.Size = new System.Drawing.Size(128, 24);
            this.chkTransparent.TabIndex = 8;
            this.chkTransparent.Text = "允许透明";
            this.chkTransparent.Visible = false;
            // 
            // ExportImageDialog
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(508, 303);
            this.ControlBox = false;
            this.Controls.Add(this.chkTransparent);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.comboFormat);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.chkView);
            this.Controls.Add(this.chkSelection);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ExportImageDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "导出";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion
		#endregion

		#region ..私有变量
		System.Windows.Forms.SaveFileDialog savedlg = new SaveFileDialog();
		Canvas vectorControl = null;
		Point startPoint = Point.Empty;
		Point viewPoint = Point.Empty;
		RectangleF bounds = RectangleF.Empty;
		Point oriPoint = Point.Empty;
		#endregion

		#region ..选项改变
		private void radioButton1_CheckedChanged(object sender, System.EventArgs e)
		{
			this.panel1.Invalidate();
		}
		#endregion

		#region ..绘制
		private void panel1_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			if(this.radioButton1.Checked)
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			else 
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
		
			e.Graphics.FillRectangle(Brushes.White,new RectangleF(0,0,this.Width,this.Height));
			RectangleF bounds = this.vectorControl.ContentBounds;
			if(!bounds.IsEmpty)
			{
				e.Graphics.TranslateTransform(-this.viewPoint.X,-this.viewPoint.Y);
				int width = this.panel1.Width;
				int height = this.panel1.Height;
				int left = 0,top = 0;
				top = (int)Math.Max(0,(height - bounds.Height) / 2f);
				left = (int)Math.Max(0,(width - bounds.Width) / 2f);
				
				e.Graphics.TranslateTransform(-bounds.Left+ left,-bounds.Top + top);
				this.vectorControl.RenderTo(g);
			}
		}
		#endregion

		#region ..导出
		private void button1_Click(object sender, System.EventArgs e)
		{
			string text = this.comboFormat.Text.ToLower();
			this.savedlg.Filter = text + "files(*." + text +")|*." + text;
			if(this.savedlg.ShowDialog(this) == DialogResult.OK)
			{
				try
				{
					string filename = this.savedlg.FileName;
					
					this.vectorControl.Document.ExportContentAsImage(filename);
					this.Close();
				}
				catch(System.Exception e1)
				{
					MessageBox.Show(e1.Message);
				}
			}
		}
		#endregion

		#region ..RenderToGraphics
		void RenderToGraphics(Graphics g,float width,float height)
		{
			if(this.radioButton1.Checked)
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			else 
				g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
						
			g.FillRectangle((this.chkTransparent.Visible && this.chkTransparent.Checked)?Brushes.Transparent:Brushes.White,new RectangleF(0,0,width,height));
			g.TranslateTransform(-bounds.Left,-bounds.Top);
			this.vectorControl.RenderTo(g);
			g.ResetTransform();
		}
		#endregion

		#region ..OnVisibleChanged
		protected override void OnVisibleChanged(EventArgs e)
		{
			base.OnVisibleChanged (e);
			this.bounds = this.vectorControl.ContentBounds;
			this.bounds.Width += 10;
			this.bounds.Height += 10;
		}
		#endregion

		#region ..MouseEvent
		private void panel1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			this.startPoint = Point.Empty;
			if(e.Button == MouseButtons.Left)
				this.startPoint = new Point(e.X,e.Y);
			this.oriPoint = this.viewPoint;
		}

		private void panel1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Left)
			{
				int x = this.oriPoint.X;
				int y = this.oriPoint.Y;
				int deltax = e.X - this.startPoint.X;
				int deltay = e.Y - this.startPoint.Y;
				x = x - deltax;
				y = y - deltay;
				x = (int)Math.Max(0,Math.Min(x,this.bounds.Width - this.panel1.Width));
				y = (int)Math.Max(0,Math.Min(y,this.bounds.Height - this.panel1.Height));
				Point p = new Point(x,y);
				if(p != this.viewPoint)
				{
					this.viewPoint = p;
					this.panel1.Invalidate();
				}
			}
		}
		#endregion

		#region ..comboFormat_SelectedIndexChanged
		private void comboFormat_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			this.chkTransparent.Visible = this.comboFormat.Text.ToLower() == "gif";
		}	
		#endregion
	}
}
