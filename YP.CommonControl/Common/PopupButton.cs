using System;
using System.Drawing;
using System.Windows.Forms;

namespace YP.CommonControl.Common
{
	/// <summary>
	/// ʵ��Popup���͵İ�ť
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
			// TODO: �ڴ˴���ӹ��캯���߼�
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

		#region ..�¼�
		/// <summary>
		/// ���û���������ҿؼ�������TimerClickʱ����
		/// </summary>
		public event EventHandler TimerClick;
		#endregion

		#region ..public properties
		/// <summary>
		/// �����ؼ��Ƿ��������������¼���������ã������û�������꿪ʼ�����ͷ���������
		/// �����ڼ䣬�ؼ�����������TimeClick�¼�
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
		/// ��ȡ�����ÿؼ���ʾ��ͼ��
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
		/// ����ͼ������ڼ��ͼ�������Ϣ
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

		#region ..ʱ��
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
