using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

using YP.CommonControl.TabControl.Interface;

namespace YP.CommonControl.TabControl
{
	#region ..ָ��ѡ���ť�ĳ��ȼ��㷽ʽ
	/// <summary>
	/// ָ��ѡ���ť�ĳ��ȼ��㷽ʽ
	/// </summary>
	public enum TabSizeMode
	{
		/// <summary>
		/// ������㣬TabControl�ĳ��Ȳ���ѡ��������Ӱ��
		/// </summary>
		Normal,

		/// <summary>
		/// ����ÿ��ѡ��Ŀ�ȣ�ʹ��ÿ��ѡ���λ����ͼ�ڲ�
		/// </summary>
		FixedInView
	}
	#endregion

	/// <summary>
	/// TabControl ��ժҪ˵����
	/// </summary>
	public class TabControl : Common.BaseControl,Interface.ITabControl
	{
		#region ..������ť�¼�
		public enum ButtonStyle
		{
			Close,
			Min,
			Max,
			None
		}

		/// <summary>
		/// ������ư�ť�����¼�
		/// </summary>
		public delegate void ButtonClickEventHandler(object sender,ButtonStyle style);
		#endregion

		#region ..��̬����
		static readonly ImageList _imageList = null;
		static readonly Size _imageSize = new Size(12,12);
		internal static readonly int TabHeight = 24;
		internal static readonly int TabMargin = 4;

		static TabControl()
		{
			_imageList = Common.ResourceHelper.LoadBitmapStrip(Type.GetType("YP.CommonControl.TabControl.TabControl"),
				"YP.CommonControl.Resources.ImagesTabControl.bmp",_imageSize,new Point(0,0));
		}
		#endregion

		#region ..Constructor
		private System.Windows.Forms.Label plButtonContainer;
		private Common.PopupButton btnPrev;
        private Common.PopupButton btnNext;
        private Common.PopupButton btnMin;
        private Common.PopupButton btnMax;
        private Common.PopupButton btnClose;
		private System.Windows.Forms.Label plTabPageContainer;

		public TabControl()
		{
			// �õ����� Windows.Forms ���������������ġ�
			InitializeComponent();

            this.ideBorder = false;
			this.btnNext.EnableTimerClick = this.btnPrev.EnableTimerClick = true;

			this.tabPages = new TabPageCollection(this);
			this.tabPages.Inserting += new Common.CollectionWithEvents.CollectionEventHandler(OnPageInserting);
            this.tabPages.Inserted += new Common.CollectionWithEvents.CollectionEventHandler(OnPageInserted);
            this.tabPages.Removed += new Common.CollectionWithEvents.CollectionEventHandler(OnPageRemoved);

			SetStyle(ControlStyles.Opaque | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.ResizeRedraw , true);

			this.sf.FormatFlags = StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.NoWrap;
			this.sf.LineAlignment = StringAlignment.Center;
//			this.sf.Alignment = StringAlignment.Center;
			this.sf.HotkeyPrefix = System.Drawing.Text.HotkeyPrefix.Show;
			sf.Trimming = StringTrimming.EllipsisCharacter;

			if(_imageList != null)
			{
				this.btnClose.Image = _imageList.Images[4];
				this.btnPrev.Image = _imageList.Images[0];
				this.btnNext.Image = _imageList.Images[2];
				this.btnMin.Image = _imageList.Images[6];
				this.btnMax.Image = _imageList.Images[7];
			}

			this.ShowControlBox = true;

			this.VirtualLeft = 0;

			this.plButtonContainer.Height = TabHeight;

			this.plTabPageContainer.SizeChanged += new EventHandler(ChangeMainSize);

			this.btnMin.Visible = this.btnMax.Visible = false;

			// TODO: �� InitializeComponent ���ú�����κγ�ʼ��

			this.plButtonContainer.DoubleClick += new EventHandler(DouleClickButtons);

			this.plButtonContainer.Anchor = AnchorStyles.Left | AnchorStyles.Top;
			
		}

		/// <summary> 
		/// ������������ʹ�õ���Դ��
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// �����֧������ķ��� - ��Ҫʹ�ô���༭�� 
		/// �޸Ĵ˷��������ݡ�
		/// </summary>
		private void InitializeComponent()
		{
			this.plButtonContainer = new System.Windows.Forms.Label();
			this.plTabPageContainer = new System.Windows.Forms.Label();
			this.btnPrev = new YP.CommonControl.Common.PopupButton();
			this.btnNext = new YP.CommonControl.Common.PopupButton();
			this.btnMin = new YP.CommonControl.Common.PopupButton();
			this.btnMax = new YP.CommonControl.Common.PopupButton();
			this.btnClose = new YP.CommonControl.Common.PopupButton();
			this.SuspendLayout();
			this.plButtonContainer.SuspendLayout();
			// 
			// plButtonContainer
			// 
			this.plButtonContainer.BackColor = System.Drawing.Color.Transparent;
			this.plButtonContainer.Dock = System.Windows.Forms.DockStyle.Top;
			this.plButtonContainer.Location = new System.Drawing.Point(0, 0);
			this.plButtonContainer.Name = "plButtonContainer";
			this.plButtonContainer.Size = new System.Drawing.Size(456, 24);
			this.plButtonContainer.TabIndex = 0;
			this.plButtonContainer.Paint += new System.Windows.Forms.PaintEventHandler(this.plButtonContainer_Paint);
			this.plButtonContainer.MouseUp += new System.Windows.Forms.MouseEventHandler(this.plButtonContainer_MouseUp);
			this.plButtonContainer.MouseMove += new System.Windows.Forms.MouseEventHandler(this.plButtonContainer_MouseMove);
			this.plButtonContainer.MouseDown += new System.Windows.Forms.MouseEventHandler(this.plButtonContainer_MouseDown);
			// 
			// plTabPageContainer
			// 
//			this.plTabPageContainer.BackColor = System.Drawing.this.BackColor;
			this.plTabPageContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.plTabPageContainer.Location = new System.Drawing.Point(0, 24);
			this.plTabPageContainer.Name = "plTabPageContainer";
			this.plTabPageContainer.Size = new System.Drawing.Size(456, 296);
			this.plTabPageContainer.TabIndex = 1;
			// 
			// btnPrev
			// 
			this.btnPrev.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnPrev.BackColor = System.Drawing.Color.Transparent;
			this.btnPrev.EnableTimerClick = true;
			this.btnPrev.Image = null;
			this.btnPrev.Location = new System.Drawing.Point(409, 4);
			this.btnPrev.Name = "btnPrev";
			this.btnPrev.Size = new System.Drawing.Size(14, 14);
			this.btnPrev.TabIndex = 2;
			this.btnPrev.Visible = false;
			this.btnPrev.EnabledChanged += new System.EventHandler(this.btnNext_EnabledChanged);
			this.btnPrev.TimerClick += new System.EventHandler(this.ClickButton);
			// 
			// btnNext
			// 
			this.btnNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnNext.BackColor = System.Drawing.Color.Transparent;
			this.btnNext.EnableTimerClick = true;
			this.btnNext.Image = null;
			this.btnNext.Location = new System.Drawing.Point(425, 4);
			this.btnNext.Name = "btnNext";
			this.btnNext.Size = new System.Drawing.Size(14, 14);
			this.btnNext.TabIndex = 3;
			this.btnNext.Visible = false;
			this.btnNext.EnabledChanged += new System.EventHandler(this.btnNext_EnabledChanged);
			this.btnNext.TimerClick += new System.EventHandler(this.ClickButton);
			// 
			// btnMin
			// 
			this.btnMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnMin.BackColor = System.Drawing.Color.Transparent;
			this.btnMin.EnableTimerClick = false;
			this.btnMin.Image = null;
			this.btnMin.Location = new System.Drawing.Point(409, 4);
			this.btnMin.Name = "btnMin";
			this.btnMin.Size = new System.Drawing.Size(14, 14);
			this.btnMin.TabIndex = 4;
			this.btnMin.Visible = false;
			this.btnMin.Click += new System.EventHandler(this.ClickButton);
			// 
			// btnMax
			// 
			this.btnMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnMax.BackColor = System.Drawing.Color.Transparent;
			this.btnMax.EnableTimerClick = false;
			this.btnMax.Image = null;
			this.btnMax.Location = new System.Drawing.Point(425, 4);
			this.btnMax.Name = "btnMax";
			this.btnMax.Size = new System.Drawing.Size(14, 14);
			this.btnMax.TabIndex = 5;
			this.btnMax.Visible = false;
			this.btnMax.Click += new System.EventHandler(this.ClickButton);
			// 
			// btnClose
			// 
			this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnClose.BackColor = System.Drawing.Color.Transparent;
			this.btnClose.EnableTimerClick = false;
			this.btnClose.Image = null;
			this.btnClose.Location = new System.Drawing.Point(441, 4);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(14, 14);
			this.btnClose.TabIndex = 6;
			this.btnClose.Visible = false;
			this.btnClose.Click += new System.EventHandler(this.ClickButton);
			// 
			// TabControl
			// 
			this.plButtonContainer.Controls.Add(this.btnMax);
			this.plButtonContainer.Controls.Add(this.btnMin);
			this.plButtonContainer.Controls.Add(this.btnClose);
			this.plButtonContainer.Controls.Add(this.btnNext);
			this.plButtonContainer.Controls.Add(this.btnPrev);
			this.Controls.Add(this.plTabPageContainer);
			this.Controls.Add(this.plButtonContainer);
			this.Name = "TabControl";
			this.Size = new System.Drawing.Size(456, 320);
			this.plButtonContainer.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
		#endregion

		#region ..private fields
		ITabPage selectedTab = null;
		TabPageCollection tabPages ;
		Color tabColor = Color.Empty ;
		bool positionTop = true;
		bool hotTrack = false;
		bool showNavigateButton = true;
		bool showControlBox = false;
		//bool ideBorder = true;
		Hashtable widthes = new Hashtable();
		StringFormat sf = new StringFormat(StringFormat.GenericTypographic);
		int virtualLeft = 0;
		int topMargin = 1;
		int bottommargin = 3;
		int buttonMargin = 6;
		bool boldSelectedPage = false;
		bool mousedown = false;
		float totalwidth = 0;
		ArrayList buttonPos = new ArrayList();
		int _pageSelected = 0;
		TabSizeMode sizeMode = TabSizeMode.Normal;
		/// <summary>
		/// ��¼ѡ�ѡ�����
		/// </summary>
		ArrayList recordSelected = new ArrayList();

		bool formMode = false;
		Size defaultFormSize = new Size(500,400);

		bool showTabButton = true;
		Size imageSize = new Size(16,16);

		Menu.PopupMenu tabContextMenu = null;
		#endregion

		#region ..public properties
		/// <summary>
		/// ��ȡ������ѡ��������
		/// </summary>
		public int SelectedIndex
		{
			set
			{
				if(value >= 0 && value < this.TabPages.Count)
				{
					this.SelectedTab = this.TabPages[value];
				}
			}
			get
			{
				return this.TabPages.IndexOf(this.selectedTab);
			}
		}

		public Menu.PopupMenu TabContextMenu
		{
			set
			{
				this.tabContextMenu = value; 
			}
		}

		/// <summary>
		/// ��ȡ��ǰ�Ƿ���ͨ��MDI�Ӵ�����ʽ��ʾ
		/// </summary>
		public bool MDIFormMode
		{
			get
			{
				return this.formMode;
			}
		}

		/// <summary>
		/// �����Ƿ���ʾѡ�������ť
		/// </summary>
		public bool ShowTabButton
		{
			set
			{
				if(this.showTabButton != value)
				{
					this.showTabButton = value;
					this.plButtonContainer.Height = value?TabHeight:0;
					this.plTabPageContainer.Height = this.Height - this.plButtonContainer.Height;
					
				}
			}
			get
			{
				return this.showTabButton;
			}
		}

		/// <summary>
		/// ��ȡ�����ÿؼ��Ŀɼ���
		/// </summary>
		public new bool Visible
		{
			set
			{
				if(this.formMode)
					return;
				base.Visible = value;
			}
			get
			{
				return base.Visible;
			}
		}

		/// <summary>
		/// ��ȡ������ѡ��ĳ��ȼ��㷽ʽ
		/// </summary>
		public TabSizeMode SizeMode
		{
			set
			{
				if(this.sizeMode != value)
				{
					this.sizeMode = value;
					this.CalculateTabs();
					this.UpdateNavi();
				}
			}
			get
			{
				return this.sizeMode;
			}
		}

		/// <summary>
		/// �ж��Ƿ������ʾ��ǰѡ���ѡ�
		/// </summary>
		public bool BoldSelectedPage
		{
			set
			{
				if(this.boldSelectedPage != value)
				{
					this.boldSelectedPage = value;
					this.plButtonContainer.Invalidate();
				}
			}
			get
			{
				return this.boldSelectedPage;
			}
		}

		/// <summary>
		/// ��ȡ�����õ�ǰѡ���ѡ�
		/// </summary>
		public ITabPage SelectedTab
		{
			set
			{
				if(this.selectedTab != value)
				{
					this.MakePageHidden(this.selectedTab);
					this._pageSelected = this.tabPages.IndexOf(this.selectedTab);
					this.UpdateSelectedButton();
					this.selectedTab = value;
					this.OnSelectedIndexChanged(EventArgs.Empty);
					this.MakePageVisible(this.selectedTab);
					this._pageSelected = this.tabPages.IndexOf(this.selectedTab);
					this.UpdateSelectedButton();
					this.MakeSelectionInView();
					if(this.recordSelected.Contains(this._pageSelected))
						this.recordSelected.Remove(this._pageSelected);
					if(this.recordSelected.Count >= 0)
                        this.recordSelected.Add(this._pageSelected);
				}
			}
			get
			{
				return this.selectedTab;
			}
		}

		/// <summary>
		/// ��ȡ������ѡ��Ƿ���ʾ�ڶ���
		/// </summary>
		public bool PositionTop
		{
			set
			{
				if(this.positionTop != value)
				{
					this.positionTop = value;
					switch(this.positionTop)
					{
						case true:
							this.plButtonContainer.Dock = DockStyle.Top;
							break;
						case false:
							this.plButtonContainer.Dock = DockStyle.Bottom;
							break;
					}
					this.plButtonContainer.Invalidate();
				}
			}
			get
			{
				return this.positionTop;
			}
		}


		/// <summary>
		/// ��ȡ������ѡ��Ƿ������ȼ�����
		/// </summary>
		public bool HotTrack
		{
			set
			{
				this.hotTrack = value;
			}
			get
			{
				return this.hotTrack;
			}
		}

		/// <summary>
		/// ��ȡ������ѡ���ѡ����������ɫ
		/// </summary>
		public Color TabColor
		{
			set
			{
				if(this.tabColor != value)
				{
					this.tabColor = value;
					if(this.btnClose != null)
						this.btnClose.BackColor = this.btnMax.BackColor = this.btnMin.BackColor = this.btnPrev.BackColor = this.btnNext.BackColor = this.plButtonContainer.BackColor = value;
				}
			}
			get
			{
				return this.tabColor;
			}
		}

		/// <summary>
		/// ��ѡ�����ά�ȷ�Χʱ���Ƿ���ʾ������ť
		/// </summary>
		public bool ShowNavigateButton
		{
			set
			{
				this.showNavigateButton = value;
			}
			get
			{
				return this.showNavigateButton;
			}
		}

		/// <summary>
		/// ѡ��Ƿ���ʾ���ư�ť
		/// </summary>
		public bool ShowControlBox
		{
			set
			{
				if(this.showControlBox != value)
				{
					this.showControlBox = value;
					//disable the min and max in current version
				//	this.btnMin.Visible = this.btnMax.Visible = 
					this.btnClose.Visible = value;
				}
			}
			get
			{
				return this.showControlBox;
			}
		}

		/// <summary>
		/// ��ȡTabControl��ѡ�����
		/// </summary>
		public ITabPageCollection TabPages
		{
			get
			{
				return this.tabPages;
			}
		}

		/// <summary>
		/// ����TabControl�Ƿ���ʾIDE���͵ı�
		/// </summary>
		public bool IDEBorder
		{
			set
			{
				if(this.ideBorder != value)
				{
					this.ideBorder = value;
					this.DockPadding.All = (value?1:0);
				}
			}
			get
			{
				return this.ideBorder;
			}
		}
		#endregion

		#region ..˽������
		/// <summary>
		/// ������ͼ����
		/// </summary>
		int VirtualLeft
		{
			set
			{
				float a = (int)Math.Max(0,Math.Min(this.totalwidth - this.btnPrev.Left + 2,value));
				if(this.virtualLeft != value)
				{
					this.virtualLeft = value;
					this.plButtonContainer.Invalidate(new Rectangle(0,this.topMargin,this.btnPrev.Left,this.Height - this.topMargin - this.bottommargin));
					this.UpdateNavi();
				}
			}
		}
		#endregion

		#region ..�¼�
		/// <summary>
		/// ����ǰѡ���ѡ������ı�ʱ����
		/// </summary>
		public event EventHandler SelectedIndexChanged;

		/// <summary>
		/// ���������ư�ťʱ����
		/// </summary>
		public event ButtonClickEventHandler ButtonClick;

		/// <summary>
		/// ˫��TabButtonʱ����
		/// </summary>
		public event EventHandler DoubleClickInTab;

		/// <summary>
		/// ������ҷTabPage�¼�
		/// </summary>
		public delegate void DragTabPageEventHandler(object sender,int index);

		/// <summary>
		/// ��ҷTabPageʱ����
		/// </summary>
		public event DragTabPageEventHandler DragTabPage;
		#endregion

		#region ..����tabLabel
		/// <summary>
		/// ���¼�������ѡ�����
		/// </summary>
		void CalculateTabs()
		{
			using(Graphics g = this.CreateGraphics())
			{
				this.totalwidth = TabMargin;
				this.buttonPos.Clear();
				foreach(TabPage tab in this.tabPages)
				{
					this.buttonPos.Add(this.totalwidth);
					this.widthes[tab] = this.MearsureTabWidth(tab);
					this.totalwidth += (float)this.widthes[tab];
				}
				this.UpdateNavi();
			}
		}

		/// <summary>
		/// ��ĳ��ѡ������ı�ʱ�����¼������Ȳ�������ز���
		/// </summary>
		/// <param name="tab"></param>
		void RecalculateTab(TabPage tab)
		{
			if(tab != null)
			{
				using(Graphics g = this.CreateGraphics())
				{
					this.widthes[tab] = this.MearsureTabWidth(tab);
					int index = this.tabPages.IndexOf(tab);
					this.totalwidth = TabMargin;
					if(index - 1>= 0)
						this.totalwidth = this.GetBoundsAtTabPage(this.tabPages[index-1]).Right;
					for(int i = index;i<this.TabPages.Count;i++)
					{
						if(i < this.buttonPos.Count)
							this.buttonPos[i] = this.totalwidth;
						else
							this.buttonPos.Add(this.totalwidth);
						TabPage tab1 = (TabPage)this.TabPages[i];
						if(!this.widthes.Contains(tab1))
							this.widthes[tab1] = this.MearsureTabWidth(tab1);
						this.totalwidth += (float)this.widthes[tab1];
					}
				}
			}
		}

		/// <summary>
		/// ����ָ��ѡ���Ӧ��ť�Ŀ��
		/// </summary>
		/// <param name="tab"></param>
		/// <returns></returns>
		float MearsureTabWidth(TabPage tab)
		{
			if(tab == null)
				return 0;
			if(!tab.Visible)
				return 0;
			string text = tab.Text;
			float width = 0;
			using(Graphics g = this.CreateGraphics())
			{
				width = g.MeasureString(text,this.Font,200000,this.sf).Width + 2 * this.buttonMargin;
				if(tab.Image != null)
				{
					width += this.imageSize.Width;
				}
				if(this.sizeMode == TabSizeMode.FixedInView)
				{
					float width2 = this.plButtonContainer.Width - 2;
					if(this.showControlBox)
						width2 = this.btnMin.Left  - 2;

					float width1 = (width2 - 2 * TabMargin) / this.tabPages.Count;
				
					width = (int)Math.Min(width,width1);
				}
				width ++;
			}
			return width;
		}
		#endregion

		#region ..OnPaint
		protected override void OnPaint(PaintEventArgs e)
		{
			e.Graphics.Clear(this.BackColor);
			if(this.ideBorder)
				e.Graphics.DrawRectangle(this.darkdarkPen,0,0,this.Width - 1,this.Height - 1);
		}
		#endregion

		#region ..����TabLabel
		private void plButtonContainer_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			Control c = sender as Control;
			int top = c.Height - this.bottommargin;
			int height = this.bottommargin;
			Color sharpColor = Color.White;
			int bottom = top;
			e.Graphics.FillRectangle(new SolidBrush(this.LightLightLightLightColor),e.ClipRectangle);
			this.TabColor = LightLightLightLightColor;
			if(!this.positionTop)
			{
				top = 0;
				bottom = this.bottommargin - 2;
				height = this.bottommargin - 2;
				sharpColor = ControlPaint.Dark(this.BackColor);
			}
			e.Graphics.DrawRectangle(new Pen(this.BackColor),0,0,c.Width - 1,c.Height - 1);
			e.Graphics.FillRectangle(new SolidBrush(this.BackColor),0,top,c.Width,height);
			e.Graphics.DrawLine(new Pen(sharpColor,1),0,bottom,c.Width,bottom);
			float x = TabMargin;
			int start = (int)Math.Max(0,this.GetTabIndexAtViewPos(this.virtualLeft));
			TabPage tab = null;
			if(start >= 0 && start < this.tabPages.Count)
				tab = (TabPage)this.tabPages[start];
			if(tab != null)
				x = this.GetBoundsAtTabPage(tab).X;
			int left = c.Width;
			if(this.btnMin.Visible)
				left = this.btnMin.Left;
			if(this.btnPrev.Visible)
				left = this.btnPrev.Left - 2;
			e.Graphics.SetClip(new Rectangle(0,0,left,c.Height));
			e.Graphics.TranslateTransform(-this.virtualLeft,0);
			for(int i = start;i<this.tabPages.Count;i++)
			{
				tab = (TabPage)this.tabPages[i];
				this.DrawTabLabel(e.Graphics,tab,ref x);
				if(x - this.virtualLeft > c.Width)
					return;
			}
			g.ResetClip();
		}

		void DrawTabLabel(Graphics g,TabPage tab,ref float x)
		{
			if(!tab.Visible)
				return;
			string text = tab.Text;
			float width = 0;
			if(!this.widthes.ContainsKey(tab))
				this.widthes[tab] = g.MeasureString(text,this.Font,2000000,this.sf).Width + 2 * this.buttonMargin;
			width = (float)this.widthes[tab];
			GraphicsContainer gc = g.BeginContainer();
			g.SetClip(new RectangleF(x,0,width + 1,TabHeight));
			int top = this.topMargin + 2;
			int height = this.plButtonContainer.Height - this.topMargin - this.bottommargin;
			Color sharpcolor = ControlPaint.LightLight(this.BackColor);
			if(!this.positionTop)
			{
				top = this.bottommargin;
				height = this.plButtonContainer.Height - this.topMargin - this.bottommargin ;
				sharpcolor = this.darkPen.Color;
			}
			int selecttop = this.topMargin;
			int selectheight = this.plButtonContainer.Height - this.bottommargin - this.topMargin;
			int bottom = selecttop;
			int delta = 0;
			if(!this.positionTop)
			{
				selecttop = this.bottommargin - 2;
				selectheight = this.plButtonContainer.Height - this.bottommargin - this.topMargin - 1;
				bottom = this.plButtonContainer.Height - this.topMargin - 2;
				delta = -this.topMargin + 1;
			}
			if(this.tabPages.IndexOf(tab) == this._pageSelected)
			{
				RectangleF temp = new RectangleF(x,selecttop,width,selectheight + 1);
				Color lightColor= this.LightLightLightLightColor;
				if(this.positionTop)
				{
					using(System.Drawing.Drawing2D.LinearGradientBrush brush = new LinearGradientBrush(temp,lightColor,this.BackColor,LinearGradientMode.BackwardDiagonal))
					{
						g.FillRectangle(brush,temp);
					}
					
				}
				else
				{
					RectangleF temp1 = new RectangleF(temp.X,temp.Y,temp.Width,temp.Height + 4);
					using(System.Drawing.Drawing2D.LinearGradientBrush brush = new LinearGradientBrush(temp,this.BackColor,lightColor,LinearGradientMode.Vertical))
					{
						g.FillRectangle(brush,temp);
					}
				}
			}
			Image img = null;
			float left = x;
			if(tab.Image != null)
			{
				img = tab.Image;
				left = x + (TabHeight - this.imageSize.Width) / 2;
				int top1 = this.topMargin + (TabHeight - this.imageSize.Height) / 2;
				if(left + this.imageSize.Width < x + width)
				{
					g.DrawImage(img,new Rectangle((int)left,top1,this.imageSize.Width,this.imageSize.Height),0,0,img.Width,img.Height,GraphicsUnit.Pixel);
				}
				left = x + this.imageSize.Width;
			}
			
			RectangleF rect = new RectangleF(left + this.buttonMargin,top,x + width - left - 2 * this.buttonMargin,height);
			if(this.TabPages.IndexOf(tab) != this.SelectedIndex - 1)
				g.DrawLine(this.darkPen,x + width - 1,top,x + width - 1,top + height - 4);
			Font f = this.Font;
			if(this.tabPages.IndexOf(tab) == this._pageSelected)
			{
				g.DrawLine(this.darkPen,x + width - 1,selecttop,x + width - 1,selecttop + selectheight + delta);
				g.DrawLine(new Pen(sharpcolor,1),x,bottom,x + width - 1,bottom);
				
				if(this.boldSelectedPage)
					f = new Font(this.Font.Name,this.Font.Size - 2,FontStyle.Bold);
				if(left < x + width - 2 * this.buttonMargin)
					g.DrawString(text,f,Brushes.Black,rect,this.sf);
			}
			else if(left < x + width - 2 * this.buttonMargin)
				g.DrawString(text,this.Font,SystemBrushes.ControlDark,rect,this.sf);
			x += width - 1;
			g.ResetClip();
			g.EndContainer(gc);
			this.UpateCurrentControl(tab);
		}
		#endregion

		#region ..��ȡָ����ͼλ�ô���ѡ�
		internal int GetTabIndexAtViewPos(float x)
		{
			if(this.tabPages.Count == 0)
				return -1;
			
			int leftIndex  = 0;
			int rightIndex = this.tabPages.Count - 1;
			
			Interface.ITabPage curLine = null;
			
			while (leftIndex < rightIndex) 
			{
				int pivotIndex = (leftIndex + rightIndex) / 2;
				
				curLine = this.tabPages[pivotIndex];
				
				if (x < (float)this.buttonPos[pivotIndex]) 
				{
					rightIndex = pivotIndex - 1;
				} 
				else if (x > (float)this.buttonPos[pivotIndex] + (float)this.widthes[curLine]) 
				{
					leftIndex = pivotIndex + 1;
				} 
				else 
				{
					leftIndex = pivotIndex;
					break;
				}
			}
			if(leftIndex == this.tabPages.Count - 1)
			{
				if(x > this.totalwidth)
					return -1;
			}
			return leftIndex;
		}
		#endregion

		#region ..��ȡѡ�����Ӧ����ͼ
		/// <summary>
		/// ��ȡѡ�����Ӧ����ͼ
		/// </summary>
		RectangleF GetBoundsAtTabPage(Interface.ITabPage tabpage)
		{
			int index = this.TabPages.IndexOf(tabpage);
			float x = (float)this.buttonPos[index];
			float width1 = (float)this.widthes[tabpage];
			return new RectangleF(x,this.topMargin,width1,this.plButtonContainer.Height);
		}
		#endregion

		#region ..ѡ��ѡ���ť
		private void plButtonContainer_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
//			this.dragtab = false;
//			if(e.Button == MouseButtons.Left)
//			{
				int index = this.GetTabIndexAtViewPos(e.X + this.virtualLeft - TabMargin);
				if(index >= 0 && index < this.tabPages.Count && index != this._pageSelected)
				{
					this.UpdateSelectedButton();
					this._pageSelected = index;
					this.UpdateSelectedButton();
				}
				this.mousedown = true;
//			}
		}

		private void plButtonContainer_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			int left = this.plButtonContainer.Width;
			if(this.btnMin.Visible)
				left = this.btnMin.Left - 2;
			if(this.btnPrev.Visible)
				left = this.btnPrev.Left - 2;
			if(e.Button == MouseButtons.Left && this.mousedown)
			{
				if(!new Rectangle(0,0,this.plButtonContainer.Width ,this.plButtonContainer.Height).Contains(new Point(e.X,e.Y)))
				{
					this.mousedown = false;
//					this.dragtab = false;
					this.OnDragTabPage(this._pageSelected);
				}
			}
//			if(e.Button == MouseButtons.Left && this.mousedown && e.Y >= 0 && e.Y < this.plButtonContainer.Height && e.X >= 0 && e.X < left)
//			{
//				int index = this.GetTabIndexAtViewPos(e.X + this.virtualLeft - TabMargin);
//				if(index >= 0 && index < this.tabPages.Count&& index != this._pageSelected)
//				{
//					this.UpdateSelectedButton();
//					this._pageSelected = index;
//					this.UpdateSelectedButton();
//				}
//			}
		}

		private void plButtonContainer_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			int left = this.plButtonContainer .Width;
			if(this.btnMin.Visible)
				left = this.btnMin.Left - 2;
			if(this.btnPrev.Visible)
				left = this.btnPrev.Left - 2;
			if( this.mousedown && e.Y >= 0 && e.Y < this.plButtonContainer.Height && e.X >= 0 && e.X < left)
			{
				int index = this.GetTabIndexAtViewPos(e.X + this.virtualLeft - TabMargin);
				if(index >= 0 && index < this.tabPages.Count)
				{
					if(index != this._pageSelected)
					{
						this.UpdateSelectedButton();
						this._pageSelected = index;
						this.UpdateSelectedButton();
					}
					this.SelectedTab = this.tabPages[index];
				}
			}
			if(e.Button == MouseButtons.Right && this.tabContextMenu != null)
			{
				this.tabContextMenu.Show(this.plButtonContainer.PointToScreen(new Point(e.X,e.Y)));
			}
			this.mousedown = false;
//			this.dragtab = false;
		}
		#endregion

		#region ..����ѡ��ť
		/// <summary>
		/// ����ѡ��ť
		/// </summary>
		void UpdateSelectedButton()
		{
			this.UpdateButton(this._pageSelected);
		}
		
		void UpdateButton(int pageIndex)
		{
			if(this.formMode)
				return;
			if(this.SizeMode == TabSizeMode.FixedInView)
				this.plButtonContainer.Invalidate();
			else if(pageIndex >= 0 &&pageIndex < this.tabPages.Count)
			{
				Interface.ITabPage tab = this.tabPages[pageIndex];
				RectangleF rect = this.GetBoundsAtTabPage(tab);
				rect.X -= this.virtualLeft;
				this.plButtonContainer.Invalidate(Rectangle.Inflate(Rectangle.Ceiling(rect),2,0));
			}
		}
		#endregion

		#region ..���µ�����ť
		void UpdateNavi()
		{
			this.btnMin.Visible = this.btnMax.Visible = false;
			if(this.formMode)
				return;
			this.btnNext.Visible = this.btnPrev.Visible = this.showNavigateButton && this.totalwidth > (this.showControlBox?this.btnMin.Left - 2:this.plButtonContainer.Width);
			this.btnPrev.Enabled = this.virtualLeft > 0;
			this.btnNext.Enabled = this.virtualLeft < this.totalwidth - this.btnPrev.Left + 2;
			if(!this.btnPrev.Visible)
				this.VirtualLeft = 0;
		}
		#endregion

		#region ..ClickButton
		void ClickButton(object sender,EventArgs e)
		{
			Control lb = sender as Control;
			if(lb.Enabled)
			{
				if(lb == this.btnNext)
					this.VirtualLeft = this.virtualLeft + 7;
				else if(lb == this.btnPrev)
					this.VirtualLeft = this.virtualLeft - 7;
				else if(this.ButtonClick != null)
				{
					ButtonStyle style = ButtonStyle.None;
					if(lb == this.btnMax)
						style = ButtonStyle.Max;
					else if(lb == this.btnMin)
						style = ButtonStyle.Min;
					else
						style = ButtonStyle.Close;
					switch(style)
					{
						case ButtonStyle.Min:
							this.ConvertToFormMode();
							Form form = ((TabPage)this.selectedTab).TabForm;
							if(form != null)
								form.WindowState = FormWindowState.Minimized ;
							break;
						case ButtonStyle.Max:
							if(!this.formMode)
								this.ConvertToFormMode();
							break;
					}
					this.ButtonClick(this,style);
				}
			}
		}
		#endregion

		#region ..Resize
		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
//			this.plButtonContainer.Height = TabHeight;
			if(this.sizeMode == TabSizeMode.FixedInView)
				this.CalculateTabs();
			this.UpdateNavi();
		}

		void ChangeMainSize(object sender,EventArgs e)
		{
			this.UpateCurrentControl(this.selectedTab);
		}
		#endregion

		#region ..����ѡ�
		void OnPageInserting(int index,object obj)
		{
		}

		void OnPageInserted(int index,object obj)
		{
			TabPage page = obj as TabPage;
			if(page != null)
			{
				page.PropertyChanged += new TabPage.PropertyChangedEventHandler(ChangePageProperty);
				page.Hide();
				page.TabControl = this;
				if(this.formMode)
				{
					Form form = page.TabForm;
					form.MdiParent = this.FindForm();
					if(!form.Controls.Contains(page.Control) && page.Control != null)
					{
						page.Control.Dock = DockStyle.Fill;
						form.Controls.Add(page.Control);
						form.Icon = form.MdiParent != null?form.MdiParent.Icon:null;
						form.Show();
					}
				}
				else
				{
					this.ResetTabPage(page);
					if(this.sizeMode == TabSizeMode.Normal)
						this.RecalculateTab(page);
					else
						this.CalculateTabs();
					this.UpdateNavi();
				}
				this.SelectedTab = page;
			}
			//disable the min and max in current style
			//= this.btnMax.Visible = this.btnMin.Visible
			this.btnClose.Visible  = this.tabPages.Count > 0 && this.showControlBox && this.showTabButton;
		}
		#endregion

		#region ..ɾ��ѡ�
		void OnPageRemoved(int index,object obj)
		{
			TabPage page = obj as TabPage;
			this.selectedTab = null;
			if(page != null)
			{
				this.recordSelected.Remove(index);
				page.PropertyChanged -= new TabPage.PropertyChangedEventHandler(this.ChangePageProperty);
				if(!this.formMode)
				{
					Form controlIsForm = page.Control as Form;
					if(controlIsForm == null)
					{
						if(page.Control != null)
						{
                            this.plTabPageContainer.Controls.Remove(page.Control);
							//Common.ControlHelper.RemoveChild(this.plTabPageContainer.Controls,page.Control);
						}
					}
				}
				page.TabForm.Close();
				if(this.recordSelected.Count > 0)
				{
					int s = (int)Math.Max(0,Math.Min(this.tabPages.Count - 1,(int)this.recordSelected[this.recordSelected.Count -1]));
					this.SelectedIndex = s;
					this.CalculateTabs();
					this.UpdateNavi();
					if(!this.formMode)
						this.plButtonContainer.Invalidate();
				}
				else
					this.SelectedIndex = 0;
			}
			this.btnClose.Visible = this.tabPages.Count > 0 && this.showControlBox && this.showTabButton;
			this.btnMax.Visible = this.btnMin.Visible =  false;
		}
		#endregion

		#region ..NextSelectedTab
		/// <summary>
		/// get the next selected tab index in the selected record
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public int NextSelectedTab(int index)
		{
//			index = this.recordSelected.IndexOf(index);
			if(index >= 1 && index < this.tabPages.Count)
			{
				if(this.recordSelected.Contains(index - 1))
					index = (int)this.recordSelected[index - 1];
			}
			else if(index == 0 && this.recordSelected.Count > 0)
				index = (int)this.recordSelected[0];
			else if(index == this.recordSelected.Count - 1 && this.recordSelected.Count > 0)
				index = (int)this.recordSelected[this.recordSelected.Count - 1];
			
			return index;
		}
		#endregion

		#region ..����ѡ��ı��¼�
		protected virtual void OnSelectedIndexChanged(EventArgs e)
		{
			if(this.SelectedIndexChanged != null)
				this.SelectedIndexChanged(this,e);
		}
		#endregion

		#region ..��ĳ��ѡ�����Ϊ�ɼ�
		void MakePageVisible(Interface.ITabPage page)
		{
			if(page == null)
				return;
			if(this.formMode)
			{
				Form form = ((TabPage)page).TabForm;
				form.Focus();
			}
			else
			{
				((TabPage)page).Show();
				Form controlIsForm = page.Control as Form;
				if(controlIsForm == null)
				{
					this.UpateCurrentControl(page);
					if(page.Control != null)
					{
						page.Control.Show();
						page.Control.Invalidate();
						page.Control.Focus();
					}
				}
				else
				{

				}
			}
		}
		#endregion

		#region ..��ĳ��ѡ�����Ϊ���ɼ�
		void MakePageHidden(Interface.ITabPage page)
		{
			if(page == null)
				return;
			if(this.formMode)
				return;
			((TabPage)page).Hide();
			Form controlIsForm = page.Control as Form;
			if(controlIsForm == null)
			{
				((TabPage)page).Hide();

				if(page.Control != null)
				{
					page.Control.Hide();
					page.Control.Size = Size.Empty;
				}
			}
			else
			{
			}
		}
		#endregion

		#region ..���µ�ǰ�ؼ�
		void UpateCurrentControl(Interface.ITabPage page)
		{
			if(page != null && page.Control != null)
			{
				page.Control.Location = new Point(1,1);
				page.Control.Size = new Size(this.plTabPageContainer.Width - 2,this.plTabPageContainer.Height - 2) ;
				page.Control.Invalidate();
			}
		}
		#endregion

		#region ..ȷ��ѡ����ڵ�ǰ��ͼ
		/// <summary>
		/// ȷ��ѡ����ڵ�ǰ��ͼ
		/// </summary>
		void MakeSelectionInView()
		{
			if(this.formMode )
				return;
			int index = this.TabPages.IndexOf(this.selectedTab);
			if(index >= 0 &&index < this.tabPages.Count)
			{
				int left = this.plButtonContainer.Width;
				if(this.btnMin.Visible)
					left = this.btnMin.Left - 2;
				if(this.btnPrev.Visible)
					left = this.btnPrev.Left - 2;
				float pos = (float)this.buttonPos[index];
				float width = (float)this.widthes[this.selectedTab];
				if(pos - this.virtualLeft < 0)
					this.VirtualLeft =(int) pos;
				else if(pos - this.virtualLeft + width > left)
					this.VirtualLeft = (int)(pos - left + width + 5);
			}
		}
		#endregion

		#region ..ChangePageProperty
		void ChangePageProperty(object sender,PropertyStyle property,object oldValue)
		{
			TabPage page = sender as TabPage;
			switch(property)
			{
					#region ..Title,Image
				case PropertyStyle.Title:
				case PropertyStyle.Image:
					if(this.sizeMode == TabSizeMode.FixedInView)
						this.CalculateTabs();
					else
						this.RecalculateTab((TabPage)sender);
					this.UpdateNavi();
					this.plButtonContainer.Invalidate();
					break;
					#endregion 

					#region ..Closed
				case PropertyStyle.Closed:
					this.ClickButton(this.btnClose,EventArgs.Empty);
					break;
					#endregion

					#region ..MaximumSize
				case PropertyStyle.MaximumSize:
					this.ConvertToTabMode();
					break;
					#endregion

					#region ..Control
				case PropertyStyle.Control:
					Form c = oldValue as Form;
					if(c == null)
					{
						Control c1 = oldValue as Control;
						if(c1 != null)
						{
                            this.plTabPageContainer.Controls.Remove(c1);
                            //Common.ControlHelper.RemoveChild(this.plTabPageContainer.Controls, c1);
						}
					}
					c = page.Control as Form;
					if( c == null)
					{
						Control c1 = page.Control as Control;
						if(c1 != null)
						{
							this.plTabPageContainer.Controls.Add(c1);
							c1.Location = new Point(1,1);
							c1.Size = Size.Empty;
							c1.Hide();
						}
					}
					if(page == this.selectedTab)
						this.MakePageVisible(page);
					break;
					#endregion

					#region ..Visible
				case PropertyStyle.Visible:
					if(this.sizeMode == TabSizeMode.FixedInView)
						this.CalculateTabs();
					else
						this.RecalculateTab((TabPage)sender);
					this.UpdateNavi();
					this.plButtonContainer.Invalidate();
					if(page.Visible)
					{
						this.SelectedTab = page;
					}
					else
					{
						this.recordSelected.Remove(this.tabPages.IndexOf(page));
						for(int i = this.recordSelected.Count -1 ;i>= 0;i++)
						{
							int s = (int)this.recordSelected[i];
							
							if(s >= 0 && s < this.tabPages.Count && this.tabPages[s].Visible)
							{
								this.SelectedIndex = s;
								return;
							}
						}
						this.SelectedIndex = 0;
					}
					break;
					#endregion
			}
		}
		#endregion

		#region ..ת����ʾģʽ
		void ConvertToFormMode()
		{
			if(this.formMode)
				return;
			try
			{
				this.formMode = true;
				this.Hide();
				Form parentForm = this.FindForm();
				Form selected = null;
				foreach(TabPage page in this.tabPages)
				{
					Form form = page.Control as Form;
					if(form == null)
						form = page.TabForm;
					if(page.Control != null)
					{
						page.Control.Dock = DockStyle.Fill;
						form.Controls.Add(page.Control);
						page.Control.Show();
					}
					if(page == this.selectedTab)
						selected = form;
					if(form.MdiParent != parentForm)
						form.MdiParent = parentForm;
					form.Icon = form.MdiParent != null?form.MdiParent.Icon:null;
					form.Text = page.Text;
					form.Show();
				}
				if(selected != null)
					selected.Focus();
			}
			catch(Exception e)
			{
				System.Diagnostics.Debug.Assert(true,e.Message);
			}
		}

		void ConvertToTabMode()
		{
			if(!this.formMode)
				return;
			this.formMode = false;
			foreach(TabPage page in this.tabPages)
			{
				page.TabForm.WindowState = FormWindowState.Normal;
				this.ResetTabPage(page);
			}
			this.CalculateTabs();
			this.UpdateNavi();
			this.MakePageVisible(this.selectedTab);
			this.Show();
		}
		#endregion

		#region ..����ѡ�
		void ResetTabPage(TabPage page)
		{
			page.Hide();
			Form controlIsForm = page.Control as Form;
			if(controlIsForm == null)
			{
				if(page.Control != null)
				{
					page.Control.Hide();
					page.Dock = DockStyle.None;
					page.Control.Location = new Point(1,1);
					page.Control.Size = Size.Empty;
					page.Anchor = AnchorStyles.Left | AnchorStyles.Top;
					if(!this.plTabPageContainer.Controls.Contains(page.Control))
						this.plTabPageContainer.Controls.Add(page.Control);
				}
			}
		}
		#endregion

		#region ..������ť
		private void btnNext_EnabledChanged(object sender, System.EventArgs e)
		{
			Control lb = sender as Control;
			lb.Invalidate();
			if(_imageList != null)
			{
				this.btnPrev.Image = this.btnPrev.Enabled ?_imageList.Images[0]:_imageList.Images[1];
				this.btnNext.Image = this.btnNext.Enabled ?_imageList.Images[2]:_imageList.Images[3];
			}
		}
		#endregion

		#region ..��plButtonContainer��˫��
		void DouleClickButtons(object sender,EventArgs e)
		{
			Point p = this.plButtonContainer.PointToClient(Control.MousePosition);
			int index = this.GetTabIndexAtViewPos(p.X + this.virtualLeft - TabMargin);
			if(index >= 0)
			{
				this.OnDoubleClickInTab(EventArgs.Empty);
			}
		}
		#endregion

		#region ..OnDoubleClickInTab
		protected virtual void OnDoubleClickInTab(EventArgs e)
		{
			if(this.DoubleClickInTab != null)
				this.DoubleClickInTab(this,e);
		}
		#endregion

		#region ..OnDragTabPage
		protected virtual void OnDragTabPage(int index)
		{
			if(this.DragTabPage != null)
				this.DragTabPage(this,index);
		}
		#endregion

		#region ..OnBackColorChanged
		protected override void OnBackColorChanged(EventArgs e)
		{
			base.OnBackColorChanged (e);
			this.TabColor = ControlPaint.Light(this.BackColor);
		}
		#endregion
	}
}
