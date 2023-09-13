using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace YP.CommonControl.StatusBar
{
	/// <summary>
	/// StatusBar 的摘要说明。
	/// </summary>
	public class StatusBar : System.Windows.Forms.StatusBar
	{
		
		
		#region ..构造及消除
		/// <summary> 
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public StatusBar()
		{
			// 该调用是 Windows.Forms 窗体设计器所必需的。
			InitializeComponent();
			
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer|ControlStyles.UserPaint ,true);
			// TODO: 在 InitializeComponent 调用后添加任何初始化

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

		#region Component Designer generated code
		/// <summary> 
		/// 设计器支持所需的方法 - 不要使用代码编辑器 
		/// 修改此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion
		#endregion

		#region ..private fields
		Color backColor = SystemColors.Control;
		Pen darkPen = SystemPens.ControlDark;
        System.Collections.Hashtable hash = new Hashtable(16);
        bool _drawGradientBackground = false;
		#endregion

		#region ..properties
        /// <summary>
        /// gets or sets a value indicates whether fill the gradient background
        /// </summary>
        public bool GradientBackground
        {
            set
            {
                if (this._drawGradientBackground != value)
                {
                    this._drawGradientBackground = value;
                    this.Invalidate();
                }
            }
            get
            {
                return this._drawGradientBackground;
            }
        }

		public override Color BackColor
		{
			set
			{
				if(this.backColor != value)
				{
					this.backColor = value;
					this.darkPen = new Pen(ControlPaint.Dark(this.backColor));
					this.Invalidate();
				}
			}
			get
			{
				return this.backColor;
			}
		}
		#endregion

		#region ..重绘
		protected override void OnPaint(PaintEventArgs e)
		{
            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);
            if (this._drawGradientBackground)
            {
                Color startColor = this.BackColor;
                Color endColor = ControlPaint.LightLight(ControlPaint.LightLight(ControlPaint.LightLight(this.BackColor)));
                using (Brush brush = new System.Drawing.Drawing2D.LinearGradientBrush(rect, startColor, endColor, System.Drawing.Drawing2D.LinearGradientMode.Horizontal))
                    e.Graphics.FillRectangle(brush, rect);
            }
			int totalwidth = 0;
			int springnumber = 0;
			foreach(StatusBarPanel pl in this.Panels)
			{
				switch(pl.AutoSize)
				{
					case System.Windows.Forms.StatusBarPanelAutoSize.None:
						totalwidth += pl.Width;
						break;
					case System.Windows.Forms.StatusBarPanelAutoSize.Contents:
						int width = (int)e.Graphics.MeasureString(pl.Text,this.Font,this.Width,StringFormat.GenericDefault).Width;
						totalwidth += width;
						break;
					case System.Windows.Forms.StatusBarPanelAutoSize.Spring:
						springnumber ++;
						break;
				}
			}
			int springwidth = 0;
			if(springnumber > 0)
				springwidth = (int)Math.Max(0,(this.Width - totalwidth) / springnumber);

	
			int left = 0;
			foreach(StatusBarPanel pl in this.Panels)
			{
				rect = Rectangle.Empty;
				int width = 0;
				switch(pl.AutoSize)
				{
					case System.Windows.Forms.StatusBarPanelAutoSize.None:
						width = pl.Width;
						break;
					case System.Windows.Forms.StatusBarPanelAutoSize.Contents:
						width = (int)Math.Max(pl.MinWidth,e.Graphics.MeasureString(pl.Text,this.Font,this.Width,StringFormat.GenericDefault).Width);
						break;
					case System.Windows.Forms.StatusBarPanelAutoSize.Spring:
						width = springwidth;
						break;
				}
				rect = new Rectangle(left,0,width,this.Height);
				StringFormat sf = new StringFormat(StringFormat.GenericDefault);
				switch(pl.Alignment)
				{
					case System.Windows.Forms.HorizontalAlignment.Center:
						sf.Alignment = StringAlignment.Center;
						break;
					case System.Windows.Forms.HorizontalAlignment.Right:
						sf.FormatFlags = StringFormatFlags.DirectionRightToLeft;
						break;
				}
				sf.FormatFlags = StringFormatFlags.LineLimit;;
				sf.LineAlignment = StringAlignment.Center;
				
				e.Graphics.DrawString(pl.Text,this.Font,Brushes.Black,rect,sf);
				e.Graphics.DrawRectangle(this.darkPen,rect.X + 1,rect.Y+1,rect.Width-3,rect.Height-2);
				
				if(pl.Icon != null)
					e.Graphics.DrawIconUnstretched(pl.Icon,rect);
				left += width;
			}
		}
		#endregion
	}
}
