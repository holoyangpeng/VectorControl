using System;
using System.Drawing;
using System.Windows.Forms;

namespace YP.CommonControl.Common
{
	/// <summary>
	/// 实现Popup类型的按钮
	/// </summary>
	public class PopupButton:Common.BaseControl 
	{
		#region ..Constructor
		private System.Windows.Forms.Timer timer1;
		private System.ComponentModel.IContainer components;
		public PopupButton()
		{
			this.InitializeComponent();
			//
			// TODO: 在此处添加构造函数逻辑
			//
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.UserPaint,true);
			SetStyle(ControlStyles.StandardDoubleClick,false);
			SetStyle(ControlStyles.Selectable,false);
			SetStyle(ControlStyles.SupportsTransparentBackColor,true);

			this.BackColor = Color.Transparent;
		}

		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			// 
			// timer1
			// 
			this.timer1.Interval = 10;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);

		}
		#endregion

		#region ..private fields
		bool timerClick = false;
		System.Drawing.Imaging.ImageAttributes imageAttribute;
		ToolTip tip = new ToolTip();
		Image image = null;
		string tooltipText = string.Empty;
		#endregion

		#region ..事件
		/// <summary>
		/// 当用户按下鼠标且控件启用了TimerClick时发生
		/// </summary>
		public event EventHandler TimerClick;
		#endregion

		#region ..public properties
		/// <summary>
		/// 决定控件是否启用连续单击事件，如果启用，则在用户按下鼠标开始，到释放鼠标结束，
		/// 在这期间，控件将连续引发TimeClick事件
		/// </summary>
		public bool EnableTimerClick
		{
			set
			{
				this.timerClick = value;
			}
			get
			{
				return this.timerClick;
			}
		}

        /// <summary>
        /// gets or sets the tool tip 
        /// </summary>
		public string ToolTipText
		{
			set
			{
				this.tooltipText = value;
			}
			get
			{
				return this.tooltipText;
			}
		}

		/// <summary>
		/// 获取或设置控件显示的图像
		/// </summary>
		public Image Image
		{
			set
			{
				if(this.image != value)
				{
					this.image = value;
					this.Invalidate();
				}
			}
			get
			{
				return this.image;
			}
		}

		/// <summary>
		/// 设置图像绘制期间的图像调整信息
		/// </summary>
		public System.Drawing.Imaging.ImageAttributes ImageAttributes
		{
			set
			{
				this.imageAttribute = value;
			}
			get
			{
				return this.imageAttribute;
			}
		}
		#endregion

		#region ..OnPaint
		protected override void OnPaint(PaintEventArgs e)
		{
			System.Drawing.Image img = this.Image;
			if(img != null)
			{
				int left = (this.Width - img.Width) / 2;
				int top = (this.Height - img.Height) / 2;
				e.Graphics.DrawImage(img,new Rectangle(left,top,img.Width,img.Height),0,0,img.Width,img.Height,GraphicsUnit.Pixel,this.imageAttribute);
			}
			if(!this.Enabled)
				return;
			Point p = this.PointToClient(MousePosition);
			bool contains = new Rectangle(0,0,this.Width,this.Height).Contains(p);
			if(contains)
			{
				if(Control.MouseButtons == MouseButtons.Left)
				{
					ControlPaint.DrawBorder(e.Graphics,new Rectangle(0,0,this.Width,this.Height),SystemColors.ControlDarkDark,1,ButtonBorderStyle.Solid,SystemColors.ControlDarkDark,1,ButtonBorderStyle.Solid,SystemColors.ControlLightLight,1,ButtonBorderStyle.Solid,SystemColors.ControlLightLight,1,ButtonBorderStyle.Solid);
				}
				else
					ControlPaint.DrawBorder(e.Graphics,new Rectangle(0,0,this.Width,this.Height),SystemColors.ControlLightLight,1,ButtonBorderStyle.Solid,SystemColors.ControlLightLight,1,ButtonBorderStyle.Solid,SystemColors.ControlDarkDark,1,ButtonBorderStyle.Solid,SystemColors.ControlDarkDark,1,ButtonBorderStyle.Solid);
			}
		}
		#endregion

		#region ..OnMouseDown
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if(e.Button == MouseButtons.Left)
			{
				this.Invalidate();
				if(this.timerClick)
					this.timer1.Start();

				this.Capture = true;
			}
		}
		#endregion

		#region ..OnMouseMove
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			this.Invalidate();
		}
		#endregion

		#region ..OnMouseUp
		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			if(e.Button == MouseButtons.Left)
				this.Invalidate();
			this.timer1.Stop();
		}
		#endregion

		#region ..OnMouseEnter
		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter (e);
			this.Cursor = Cursors.Default;
			this.Invalidate();
		}
		#endregion

		#region ..OnMouseLeave
		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			this.Invalidate();
		}
		#endregion

		#region ..时间
		private void timer1_Tick(object sender, System.EventArgs e)
		{
			if(Control.MouseButtons == MouseButtons.Left)
			{
				Point p = this.PointToClient(MousePosition);
				bool contains = new Rectangle(0,0,this.Width,this.Height).Contains(p);
				if(this.TimerClick != null && contains)
					this.TimerClick(this,new EventArgs());
			}
			else
			{
				this.timer1.Stop();
			}
		}
		#endregion	

		#region ..OnMouseHover
		protected override void OnMouseHover(EventArgs e)
		{
			base.OnMouseHover (e);
			if(this.tooltipText != null && this.tooltipText.Length > 0)
				this.tip.SetToolTip(this,this.tooltipText);
		}

		#endregion
	}
}
