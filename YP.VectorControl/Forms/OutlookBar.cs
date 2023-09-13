using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using YP.SVG.Interface;

namespace YP.VectorControl.Forms
{
	/// <summary>
	/// <para>图元选择器和形状选择器的基类。</para>
	/// <para>该类提供了一个类似OutlookBar外观的选择方式</para>
	/// </summary>
	public abstract class OutlookBar : System.Windows.Forms.UserControl
	{

		#region ..构造及消除
		/// <summary> 
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		public OutlookBar()
		{
			// 该调用是 Windows.Forms 窗体设计器所必需的。
			InitializeComponent();
			this.tip.AutoPopDelay = 6000;

			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.ResizeRedraw,true);
			
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

		#region 组件设计器生成的代码
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

		#region ..私有变量
		internal ArrayList items = new ArrayList();
		int itemHeight = 22;
		Size iconSize = new Size(48,48);
		Size minSize = new Size(50,50);
        internal Panel iconLabel = null;
		IGroup selectedGroup = null;
        internal int IconHeight = 40;
        internal bool selectedChanged = false;
        internal int selectIconIndex = -1;
        internal Color iconColor = Color.SkyBlue;
        IOutlookBarPath selectedPath = null;
        internal bool ownerDrawIcon = false;
		bool firstselect = false;
        internal bool dodragdrap = false;
		System.Windows.Forms.ToolTip tip = new ToolTip();
        internal bool showtip = false;
		Color titleColor = SystemColors.ControlLight;
        internal Color contentColor = Color.SkyBlue;
		bool _gradientTitle = true;
        bool flatGroupBorder = false;
		#endregion

		#region ..事件
		/// <summary>
		/// 当选择对象发生变换时发生
		/// </summary>
		public event EventHandler SelectedChanged;

        /// <summary>
        /// 当双击图元对象时发生
        /// </summary>
        public event EventHandler ElementDoubleClicked;
		#endregion

		#region ..常量
        internal const int ICON_Margin = 4;
        internal const int ContentMargin = 10;
        internal const int LeftMargin = 5;
		#endregion

        #region ..ContextMenuStrip
        public override ContextMenuStrip ContextMenuStrip
        {
            get
            {
                return base.ContextMenuStrip;
            }
            set
            {
                base.ContextMenuStrip = value;
                if (this.iconLabel != null)
                    this.iconLabel.ContextMenuStrip = value;
            }
        }
        #endregion

        #region ..属性
        internal IGroup CurrentGroup
		{
			set
			{
				if(this.selectedGroup != value)
				{
					this.selectedGroup = value;
					this.AdjustIconAreaPos();
					this.Invalidate();
					this.iconLabel.AutoScrollPosition = new Point(0,0);
					this.CalculateIconViewSize();
					this.selectIconIndex = -1;
				}
			}
			get
			{
				return this.selectedGroup;
			}
		}

        /// <summary>
        /// 取得当前选择的组对象
        /// </summary>
        public System.Xml.XmlElement SelectedGroup
        {
            get
            {
                if (this.CurrentGroup is ShapeGroup)
                    return (this.CurrentGroup as ShapeGroup).Tag as System.Xml.XmlElement;
                return null;
            }
        }

        /// <summary>
        /// 获取或设置一个指，只是是否绘制平滑边界
        /// </summary>
        public bool FlatGroupBorder
        {
            set
            {
                if (this.flatGroupBorder != value)
                {
                    this.flatGroupBorder = value;
                    this.Invalidate();
                }
            }
            get
            {
                return this.flatGroupBorder;
            }
        }

		/// <summary>
		/// 获取或设置一个值，该值指示是否启用渐变标题栏
		/// </summary>
		public bool GradientTitle
		{
			set
			{
				if(this._gradientTitle != value)
				{
					this._gradientTitle = value;
					this.Invalidate();
				}
			}
			get
			{
				return this._gradientTitle;
			}
		}

		/// <summary>
		/// 获取或设置绘制分组标题时的背景色
		/// </summary>
		public Color TitleColor
		{
			set
			{
				if(this.titleColor != value)
				{
					this.titleColor = value;
					this.Invalidate();
				}
			}
			get
			{
				return this.titleColor;
			}
		}

		/// <summary>
		/// 获取或设置绘制列表选取区的背景色
		/// </summary>
		public Color ContentColor
		{
			set
			{
				if(this.iconColor != value)
				{
					this.iconColor = value;
					this.BackColor=value;
					if(this.iconLabel != null)
					{
						this.iconLabel.BackColor = value;
						this.iconLabel.Invalidate();
					}
				}
			}
			get
			{
				return this.iconColor;
			}
		}

		/// <summary>
		/// 获取当前选中对象
		/// </summary>
		public object SelectedObject
		{
			get
			{
				return this.selectedPath;
			}
		}

		/// <summary>
		/// 获取当前选择组的索引
		/// </summary>
		protected int SelectedIndex
		{
			get
			{
				return this.items.IndexOf(this.CurrentGroup);
			}
		}

        IOutlookBarPath SelectedPath
		{
			set
			{
				if(this.selectedPath != value)
				{
					this.selectedPath = value;
					this.selectedChanged = true;
				}
			}
		}
		
		/// <summary>
		/// 设置当前选择IconIndex
		/// </summary>
        internal virtual int SelectedPathIndex
		{
			set
			{
                //if(this.selectIconIndex != value)
                //{
					this.InvalidateShape(this.selectIconIndex);
					this.selectIconIndex = value;
					this.InvalidateShape(this.selectIconIndex);
					
					if(this.selectedGroup != null && value >= 0 && value < selectedGroup.Count)
						this.SelectedPath = (selectedGroup as ShapeGroup)[value] as IOutlookBarPath;
                //}
			}
		}
		#endregion

		#region ..OnPaint
		protected override void OnPaint(PaintEventArgs e)
		{
			if(this.iconLabel != null && this.Width != this.iconLabel.Width)
				this.CalculateIconArea();
			for(int i = 0;i<this.items.Count;i++)
			{
				this.PaintGroup(e.Graphics,this.items[i] as IGroup);
			}
			base.OnPaint (e);
		}
		#endregion

		#region ..绘制组标题
		void PaintGroup(Graphics g,IGroup group)
		{
			if(group == null)
				return;
			RectangleF rect = this.GetGroupRectangle(group);
			Color color = this.titleColor;
			Color lightColor = ControlPaint.LightLight(color);
			Color darkColor = ControlPaint.Dark(color);
			g.FillRectangle(new SolidBrush(color),rect);
			using(System.Drawing.Drawing2D.GraphicsPath gp = new GraphicsPath())
			{
                rect.Inflate(-1f, -1);
                rect.Width--;

                if (this.flatGroupBorder)
                {
                    float rx = 1, ry = 1;
                    rx = Math.Min(rect.Width / 2, rx);
                    ry = Math.Min(rect.Height / 2, ry);
                    float a = rect.X + rect.Width - rx;
                    gp.AddLine(rect.X + rx, rect.Y, a, rect.Y);
                    gp.AddArc(a - rx, rect.Y, rx * 2, ry * 2, 270, 90);

                    float right = rect.X + rect.Width;	// rightmost X
                    float b = rect.Y + rect.Height - ry;

                    gp.AddLine(right, rect.Y + ry, right, b);
                    gp.AddArc(right - rx * 2, b - ry, rx * 2, ry * 2, 0, 90);

                    gp.AddLine(right - rx, rect.Y + rect.Height, rect.X + rx, rect.Y + rect.Height);
                    gp.AddArc(rect.X, b - ry, rx * 2, ry * 2, 90, 90);

                    gp.AddLine(rect.X, b, rect.X, rect.Y + ry);
                    gp.AddArc(rect.X, rect.Y, rx * 2, ry * 2, 180, 90);

                    gp.CloseFigure();
                }
                else
                    gp.AddRectangle(rect);
				//渐变标题
				if(this.GradientTitle)
				{
					using(Brush brush = new LinearGradientBrush(rect,Color.White,color,System.Drawing.Drawing2D.LinearGradientMode.Vertical))					
						g.FillPath(brush,gp);
				}
				else
				{
					using(SolidBrush brush = new SolidBrush(this.titleColor))
					g.FillPath(brush,gp);
				}
				
                if(this.flatGroupBorder)
				    g.DrawPath(new Pen(darkColor),gp);
                else
                    ControlPaint.DrawBorder3D(g, Rectangle.Round(rect));
                //draw the icon
				using(System.Drawing.StringFormat sf = new StringFormat(StringFormat.GenericTypographic))
				{
					sf.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
					sf.LineAlignment = StringAlignment.Center;
					g.DrawString(group.ID,SystemInformation.MenuFont,Brushes.Black,new RectangleF(rect.X + ICON_Margin,rect.Y + 2,rect.Width - 2 * ICON_Margin,rect.Height - 4),sf);
				}
			}
		}
		#endregion

		#region ..计算IconArea区域
        internal void CalculateIconArea()
		{
			int min = this.items.Count * this.itemHeight;
			this.minSize = new Size(this.minSize.Width,min + this.iconSize.Height + 20);
			this.ValidateSize();
			if(this.iconLabel != null)
			{
				this.iconLabel.Width  = this.Width;
				this.iconLabel.Height = this.Height - this.items.Count * this.itemHeight;
				this.iconLabel.AutoScroll = true;
			}
		}
		#endregion

		#region ..校验尺寸
		void ValidateSize()
		{
			if(this.Width < this.minSize.Width)
				this.Width = this.minSize.Width;
			if(this.Height < this.minSize.Height)
				this.Height = this.minSize.Height;
		}
		#endregion

		#region ..Resize
		protected override void OnResize(EventArgs e)
		{
			base.OnResize (e);
			this.CalculateIconArea();
			this.Invalidate();
		}
		#endregion

		#region ..校正IconArea位置
		void AdjustIconAreaPos()
		{
			if(this.iconLabel == null)
				return;
			if(this.selectedGroup != null)
			{
				int index = this.items.IndexOf(this.selectedGroup);
				this.iconLabel.Location = new Point(0,(index + 1) * this.itemHeight);
				this.iconLabel.Invalidate();
			}
			else
				this.iconLabel.Visible = false;
		}
		#endregion

		#region ..获取组的矩形区域
		Rectangle GetGroupRectangle(IGroup group)
		{
			int index = this.items.IndexOf(this.selectedGroup);
			index = (int)Math.Max(0,index);
			int index1 = this.items.IndexOf(group);
			if(index1 > index)
			{
				index1 = this.items.Count - index1;
				Rectangle rect = new Rectangle(0,this.Height - index1 * this.itemHeight,this.Width,this.itemHeight);
				return rect;
			}
			else
				return new Rectangle(0,index1 * this.itemHeight,this.Width,this.itemHeight);
		}
		#endregion

		#region ..绘制IconPanel
        internal virtual void PaintIconArea(object sender, PaintEventArgs e)
		{
			this.iconLabel.BackColor = this.iconColor;
			if(this.selectedGroup!= null)
			{
				e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
				ShapeGroup group = this.selectedGroup as ShapeGroup;
				Point pos = this.iconLabel.AutoScrollPosition ;
				int index = (int)Math.Max(0,-pos.Y/ IconHeight);
				int count = this.iconLabel.Height / IconHeight;
				int right = (int)Math.Min(group.Count,index + count + 2);
				e.Graphics.TranslateTransform(pos.X,pos.Y);
				for(int i = index;i<right;i++)
				{
					IOutlookBarPath shape = group[i] as IOutlookBarPath;
					if(shape == null)
						continue;
					Color backcolor = iconColor;
					Color foreColor = Color.Black;
					if(shape == this.selectedPath)
					{
						backcolor = SystemColors.Highlight;
						foreColor = SystemColors.HighlightText;
					}
					
					Rectangle bounds = this.GetShapeRectangle(i);
					using(Brush brush = new SolidBrush(backcolor))
					{
						e.Graphics.FillRectangle(brush,new Rectangle(bounds.X,bounds.Y,bounds.Width,bounds.Height + 1));
						
						string id = null;
						GraphicsPath path1 = null;
						if(shape is IOutlookBarPath)
						{
							path1 = (shape as IOutlookBarPath).GPath;
                            id = (shape as IOutlookBarPath).ID;
						}
                        //else if(shape is Symbol)
                        //{
                        //    id = (shape as IPath).ID;
                        //    path1 = (shape as IPath).GPath;
                        //}
						if(path1 == null || path1.PointCount <= 1)
							continue;

						using(StringFormat sf = new StringFormat(StringFormat.GenericTypographic))
						{
							sf.LineAlignment = StringAlignment.Center;
							using(GraphicsPath path = path1.Clone() as GraphicsPath)
							{
								path.Flatten();
								RectangleF rect = path.GetBounds();
								if(!rect.IsEmpty)
								{
									float cx = rect.X + rect.Width /2f;
									float cy = rect.Y + rect.Height / 2f;
									using(Brush brush1 = new SolidBrush(foreColor))
									{
										using(Matrix matrix = new Matrix())
										{
											matrix.Translate(bounds.X - rect.X + LeftMargin,bounds.Y - rect.Y);
											matrix.Translate(rect.X,rect.Y);
											matrix.Scale((bounds.Height - 5) / rect.Width,(bounds.Height - 5)/rect.Height);
											matrix.Translate(-rect.X,-rect.Y);
											path.Transform(matrix);
											e.Graphics.FillPath(brush1,path);
										}
										RectangleF rect1 = new RectangleF(bounds.X + bounds.Height + LeftMargin + ContentMargin,bounds.Y,bounds.Width,bounds.Height);
										e.Graphics.DrawString(id,SystemInformation.MenuFont,brush1,rect1,sf);
									}
								}
							}
						}
					}
				
				}
			}
		}
		#endregion

		#region ..鼠标
        //protected override void OnMouseDown(MouseEventArgs e)
        //{
        //    this.Focus();
        //    if(this.iconLabel != null)
        //    {
        //        if(e.Button == MouseButtons.Left || e.Button == System.Windows.Forms.MouseButtons.Right)
        //        {
        //            if(e.Y < this.iconLabel.Top)
        //            {
        //                int index = e.Y / this.itemHeight;
        //                if(index>= 0 && index < this.items.Count )
        //                    this.SelectedGroup = this.items[index] as IGroup;
        //            }
        //            else
        //            {
        //                int index = (this.Height - e.Y) / this.itemHeight;
        //                index = this.items.Count - index - 1;
        //                if(index>= 0 && index < this.items.Count )
        //                    this.SelectedGroup = this.items[index] as IGroup;
        //            }
        //        }
        //    }
        //    base.OnMouseDown (e);
        //}

		#endregion

        #region ..OnClick
        protected override void OnClick(EventArgs e)
        {
            this.Focus();
            Point p = this.PointToClient(Control.MousePosition);
            if (p.Y < this.iconLabel.Top)
            {
                int index = p.Y / this.itemHeight;
                if (index >= 0 && index < this.items.Count)
                    this.CurrentGroup = this.items[index] as IGroup;
            }
            else
            {
                int index = (this.Height - p.Y) / this.itemHeight;
                index = this.items.Count - index - 1;
                if (index >= 0 && index < this.items.Count)
                    this.CurrentGroup = this.items[index] as IGroup;
            }
            base.OnClick(e);
        }
        #endregion

        #region ..OnSelectedChanged
        protected virtual void OnSelectedChanged()
		{
			if(this.SelectedChanged != null)
				this.SelectedChanged(this,EventArgs.Empty);
		}
		#endregion

		#region ..protected AddIconArea
        internal void AddIconArea()
        {
            //if(this.items.Count > 0)
            //{
            this.iconLabel = new Panel();
            this.iconLabel.Width = this.Width;
            this.Controls.Add(this.iconLabel);
            this.iconLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Top;
            this.CalculateIconArea();
            if (this.items.Count > 0)
                this.CurrentGroup = this.items[0] as IGroup;
            this.iconLabel.Paint += new PaintEventHandler(PaintIconArea);
            this.iconLabel.MouseDown += new MouseEventHandler(iconLabel_MouseDown);
            this.iconLabel.MouseMove += new MouseEventHandler(iconLabel_MouseMove);
            this.iconLabel.MouseUp += new MouseEventHandler(iconLabel_MouseUp);
            this.iconLabel.DoubleClick += new EventHandler(iconLabel_DoubleClick);
            if (showtip)
                this.tip.SetToolTip(this.iconLabel, "拖动图元到编辑环境以创建实例");
            this.iconLabel.BackColor = this.iconColor;
            //}
        }
		#endregion

		#region ..计算IconArea的视图宽度
        internal virtual void CalculateIconViewSize()
		{
			int index = this.SelectedIndex;
			if(this.selectedGroup != null)
			{
				IGroup group = this.selectedGroup;
				int count = group.Count;
				count = count * IconHeight;
				this.iconLabel.AutoScroll = true;
				this.iconLabel.AutoScrollMinSize = new Size(0,count);
			}
		}
		#endregion

		#region ..选择Icon
		private void iconLabel_MouseDown(object sender, MouseEventArgs e)
		{
			this.iconLabel.Focus();
			if(e.Button == MouseButtons.Left || e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				Point ps = this.iconLabel.AutoScrollPosition;
				int y = e.Y - ps.Y;
				this.SelectedPathIndex = y / this.IconHeight;
				this.firstselect = true;
			}
		}
		#endregion

		#region ..鼠标弹出
		private void iconLabel_MouseUp(object sender, MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Left || e.Button == System.Windows.Forms.MouseButtons.Right)
			{
				if(this.selectedChanged)
					this.OnSelectedChanged();
				this.selectedChanged = false;
			}
		}
		#endregion

		#region ..InvalidateShape
        internal virtual void InvalidateShape(int iconindex)
		{
			if(this.selectedGroup != null)
			{
				if(iconindex >= 0)
				{
					Rectangle rect = new Rectangle(0,iconindex * IconHeight+this.iconLabel.AutoScrollPosition.Y - 1,this.iconLabel.Width,IconHeight + 2);
					this.iconLabel.Invalidate(rect);
				}
			}
		}
		#endregion

		#region ..GetShapeRect
        internal Rectangle GetShapeRectangle(int iconIndex)
		{
			int index = this.SelectedIndex;
			if(this.selectedGroup != null && iconIndex>=0)
			{
				return new Rectangle(0,iconIndex * IconHeight,this.iconLabel.Width,IconHeight);
			}
			return Rectangle.Empty;
		}
		#endregion

		#region ..确定DragDrop
		private void iconLabel_MouseMove(object sender, MouseEventArgs e)
		{
			if(e.Button == MouseButtons.Left && this.dodragdrap)
			{
				
				Point ps = this.iconLabel.AutoScrollPosition;
				int y = e.Y - ps.Y;
				int index = y / this.IconHeight;
				if(index>= 0 && this.selectIconIndex >= 0 && index != this.selectIconIndex && firstselect)
				{
                    this.DoDragDrop(new DataObject(Clipboard.DragDropObject.ClipboardSymbolString, this.selectedPath), System.Windows.Forms.DragDropEffects.All);
					firstselect = false;
				}
				else if(firstselect)
				{
					Rectangle rect = new Rectangle(0,0,this.iconLabel.Width,this.iconLabel.Height);
					if(!rect.Contains(new Point(e.X,e.Y)))
					{
                        this.DoDragDrop(new DataObject(Clipboard.DragDropObject.ClipboardSymbolString, this.selectedPath), System.Windows.Forms.DragDropEffects.All);
						firstselect = false;
					}
				}
			}
		}
		#endregion

		#region ..GotFocus
		protected override void OnGotFocus(EventArgs e)
		{
			if(this.iconLabel != null)
				this.iconLabel.Focus();
			base.OnGotFocus (e);
		}
		#endregion

        #region ..Load
        /// <summary>
        /// 重新导入配置文件
        /// </summary>
        /// <param name="filePath"></param>
        public virtual void LoadConfiguration(string filePath)
        {
        }
        #endregion

        #region ..OnElementDoubleClicked
        internal virtual void OnElementDoubleClicked()
        {
            if (this.ElementDoubleClicked != null)
                this.ElementDoubleClicked(this, EventArgs.Empty);
        }
        #endregion

        #region ..iconLabel_DoubleClick
        void iconLabel_DoubleClick(object sender, EventArgs e)
        {
            if (this.SelectedObject != null)
                this.OnElementDoubleClicked();
        }
        #endregion
    }
}
