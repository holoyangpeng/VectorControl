using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Drawing.Design;
using YP.SVG;
using YP.SVG.Paths;
using System.Collections.Generic;
using YP.SVG.DocumentStructure;
using YP.SVG.BasicShapes;
using YP.VectorControl.Forms;

namespace YP.VectorControl
{
	/// <summary>
    /// VectorControl类，提供画布, 封装了对SVG矢量图形的可视化呈现和矢量操作
    /// </summary>
	/// <example>
	/// 下面代码说明了如何创建VectorControl。
	/// <code>
	/// YP.Canvas control= new YP.Canvas(new Size(400,300));
	/// control.DockStyle = DockStyle.Fill;
	/// this.Control.Add(control);
	/// </code>
	/// </example>
    [DefaultProperty("AutoFitWindowWhenFirstLoading"), DefaultEvent("ScaleChanged")]
	[ToolboxItem(true),ToolboxBitmap(typeof(Canvas),"Bitmap1.bmp")]
	public class Canvas : System.Windows.Forms.ScrollableControl 
	{
		#region ..external methods
		#region Class Variables
		const int SRCCOPY = 13369376;
        private const int SB_HORZ = 0;
        private const int SB_VERT = 1;
        private const int SB_ENDSCROLL = 8;
		#endregion

		#region Class Functions
		[DllImport("gdi32.dll",EntryPoint="DeleteDC")]
		static extern IntPtr DeleteDC(IntPtr hDc);

		[DllImport("gdi32.dll",EntryPoint="DeleteObject")]
		static extern IntPtr DeleteObject(IntPtr hDc);

		[DllImport("gdi32.dll",EntryPoint="BitBlt")]
		static extern bool BitBlt(IntPtr hdcDest,int xDest,int yDest,int 
			wDest,int hDest,IntPtr hdcSource,int xSrc,int ySrc,int RasterOp);

		[DllImport ("gdi32.dll",EntryPoint="CreateCompatibleBitmap")]
		static extern IntPtr CreateCompatibleBitmap(IntPtr hdc,int nWidth, int nHeight);

		[DllImport ("gdi32.dll",EntryPoint="CreateCompatibleDC")]
		static extern IntPtr CreateCompatibleDC(IntPtr hdc);

		[DllImport ("gdi32.dll",EntryPoint="SelectObject")]
		static extern IntPtr SelectObject(IntPtr hdc,IntPtr bmp);

        [DllImport("user32.dll")]
        private static extern bool GetScrollInfo(IntPtr hwnd, int fnBar,ref SCROLLINFO ScrollInfo);
		#endregion

		#endregion

        #region ..const
        const int SelectionMargin = 100;
        const int MinScrollDistance = 5;
        internal const int MaxInvalidateRects = 500;
        internal const int MaxCalculateElements = 200;
        const int MinPadding = 10;
        internal const int MinGrap = 1;
        internal const int MaxPathPoint = 500;
        internal const int CenterGrap = 6;
        internal const int InfoGrap = 10;
        #endregion

        #region ..internal fields
        //颜色设置
        internal Color AnchorColor = Color.DodgerBlue;
        internal Color ControlColor = Color.Yellow;
        internal Color HighlightAnchor = Color.LimeGreen;
        internal Color NormalColor = Color.LightGray;

        internal bool scrolled = true;
        internal static Size ExpandSelectionSize = new Size(5, 5);
        internal static readonly int PenWidth = 3;
		internal static readonly int SnapMargin = 6;
        internal static readonly Pen InPathPen = new Pen(Color.White, PenWidth);
		internal static int dragIndent = 3;
        internal PointF oldselectPoint = PointF.Empty;
        internal Matrix selectMatrix = new Matrix();
        internal bool selectChanged = false;
        internal bool InXorDraw = false;
        internal Point firstPoint = Point.Empty;
        internal PointF CenterPoint = Point.Empty;
        internal int ruleLength = 16;
        internal Font demoFont = new Font("Tahoma", 14, FontStyle.Bold);
        //记录当前是否存在分叉连接线
        internal bool hasConnectionBranch = false;
        Operator currentOperator = Operator.None;
        internal int grapSize = 4;
        internal int clickSize = 4;
        internal ArrayList vGuides = new ArrayList();
        internal ArrayList hGuides = new ArrayList();
        internal Pen SelectedPen = new Pen(Color.DodgerBlue);
        internal SolidBrush LabelIndicator = new SolidBrush(Color.Yellow);
        internal Brush UnselectedBrush = new SolidBrush(Color.White);
        internal Brush LockBrush = new SolidBrush(Color.Gray);
        internal Brush SelectedBrush = null;
        internal SVGPathElement templateShape = null;
        internal Point ChangeCenterPoint = Point.Empty;
        internal SVG.SVGElementCollection connectableElements = new SVGElementCollection();
        internal bool InDragDrop = false;
        internal SVG.SVGElementCollection renderElements = new SVGElementCollection();
        internal System.Drawing.Drawing2D.LinearGradientBrush demoBrush = null;
        internal RectangleF viewRectangle = RectangleF.Empty;

        internal PointF? anchorPoint = null;

        internal bool finishLoaded = false;
        #endregion

        #region ..构造及消除
        /// <summary> 
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// 构造一个默认尺寸的VectorControl控件
		/// 默认状态下，文档尺寸为A4大小
		/// </summary>
		public Canvas():this(new Size((int)LengthToPixel(210,UnitType.Millimetre),(int)LengthToPixel(297,UnitType.Millimetre)))
		{
			
		}

		/// <summary>
		/// 用指定的文件初始化一个VectorControl控件实例
		/// 注意，传入的文档必须是有效的SVG文件，否则会抛出异常，在调用此方法时，请记住捕捉
		/// </summary>
		/// <param name="filepath">文档路径</param>
		public Canvas(string filepath)
		{
			this.Create();
			this._filepath = filepath;
			this.SVGDocument = SVG.Document.SvgDocumentFactory.CreateDocumentFromFile(filepath);
		}

		/// <summary>
		/// 用指定的文档初始化控件
		/// </summary>
		/// <param name="doc">初始化文档</param>
		public Canvas(SVG.Document.SVGDocument doc)
		{
			if(doc == null)
				throw new Exception("无效的SVG文档格式!");
			this.Create();
			//doc.vectorControl = this;
			this.SVGDocument= doc;
		}

		/// <summary>
		/// 用指定的尺寸初始化一个VectorControl控件实例
		/// </summary>
		/// <param name="size">文档尺寸</param>
		public Canvas(Size size)
		{
			this.Create();
			this.New(new SVG.DataType.SVGLength(size.Width), new SVG.DataType.SVGLength(size.Height));
		}

        /// <summary>
        /// 用指定的尺寸初始化一个VectorControl控件实例
        /// </summary>
        /// <param name="size">文档尺寸</param>
        public Canvas(SVG.DataType.SVGLength width, SVG.DataType.SVGLength height)
        {
            this.Create();
            this.New(width,height);
        }

        void Create()
        {
            this.AutoScroll = true;
            this.BackColor = Color.LightGray;

            // 该调用是 Windows.Forms 窗体设计器所必需的。
            InitializeComponent();
            // TODO: 在 InitializeComponent 调用后添加任何初始化
            SetStyle(
                ControlStyles.AllPaintingInWmPaint | 
                ControlStyles.OptimizedDoubleBuffer | 
                ControlStyles.UserMouse | 
                ControlStyles.UserPaint | 
                ControlStyles.Selectable | 
                ControlStyles.StandardDoubleClick, true);

            this.AttachDragDrop();
            this.ImeMode = ImeMode.NoControl;
            this.AllowDrop = true;
            this.unitMenu = this.CreateRuleContextMenu();
            this.exportdlg = new ExportImageDialog(this);
            //			this.printdlg = new Print.PrintDialog(this);
            this.demoFormat.FormatFlags = StringFormatFlags.DisplayFormatControl | StringFormatFlags.MeasureTrailingSpaces;
            this.demoBrush = new LinearGradientBrush(new Rectangle(0, 0, 400, this.Height), Color.DarkRed, Color.DarkBlue, System.Drawing.Drawing2D.LinearGradientMode.Horizontal);
            //			this.outlinePen.DashPattern = new float[]{1,1};

            try
            {
                this.demoFont = new Font("Microsoft YaHei", 14);
            }
            catch { }

            this.TransformBehavior = TransformBehavior.All;
            this.VisualAlignment = VisualAlignment.All;// VisualAlignment.Element | VisualAlignment.Grid | VisualAlignment.Guide;
            SelectedBrush = new SolidBrush(HighlightAnchor);
            this.Star = new Star(6,1);
        }

		/// <summary> 
		/// 清理所有正在使用的资源。
		/// </summary>
		protected override void Dispose( bool disposing )
		{
            if (this.IsDisposed)
                return;
			if(this.currentOperation != null)
				this.currentOperation .Dispose();
			if(this.outlinePen != null)
				this.outlinePen.Dispose();
			this.outlinePen = null;
			if(this.dragdrop != null)
				this.dragdrop.Dispose();
			//			this.savedialog.Dispose();
			//this.CoordTransform.Dispose();
			if(this.selectMatrix != null)
				this.selectMatrix.Dispose();
			if(this.selectpath != null)
				this.selectpath.Dispose();
			this.selectpath = null;
			if(this.SelectedPen != null)
				this.SelectedPen.Dispose();
			this.SelectedPen = null;
			if(this.SelectedBrush != null)
				this.SelectedBrush.Dispose();
			this.SelectedBrush = null;
			if(this.UnselectedBrush != null)
				this.UnselectedBrush.Dispose();
			this.UnselectedBrush = null;
			if(this.bmp != null)
				this.bmp.Dispose();
			this.bmp = null;
			this.vGuides.Clear();
			this.hGuides.Clear();
			this.vGuides = null;
			this.hGuides = null;
			this.boundPoints = null;
			if(this.svgDocument != null)
				this.svgDocument.Dispose();
			this.svgDocument = null;
			
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			if(this.demoFont != null)
				this.demoFont.Dispose();
			if(this.demoFormat != null)
				this.demoFormat.Dispose();
			base.Dispose( disposing );
			//			GC.SuppressFinalize(this);
			//			GC.Collect(this);
		}

		#region 组件设计器生成的代码
		/// <summary> 
		/// 设计器支持所需的方法 - 不要使用代码编辑器 
		/// 修改此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// Canvas
			// 
			this.Name = "Canvas";
			this.Size = new System.Drawing.Size(344, 304);

		}
		#endregion
		#endregion

		#region ..private fields
        Rectangle contentBounds = Rectangle.Empty;
        Size bottomMargin = new Size(40, 40);
        Rectangle preBounds = Rectangle.Empty;
        TextBlockStyle textBlockStyle = new TextBlockStyle(Color.Black, SVG.Alignment.Center, SVG.VerticalAlignment.Middle);
        Margin preMargin = new Margin(0, 0, 0, 0);
        List<Rectangle> clipRectangles = new List<Rectangle>();
        bool hasInvalidateRegion = false;
        bool scrollProgrammingChanged = false;
		SVG.Document.SVGDocument svgDocument = null;
		string _filepath = string.Empty;
		//		int virtualLeft = 0;
		//		int virtualTop = 0;
		Margin margin = new Margin(400,400,400,400);
        //Matrix CoordTransform = new Matrix();
		float scaleRatio = 1f;
		SizeF viewSize = SizeF.Empty;
		//		Size GridSize = new Size(10,10);
		//		bool grid.Visible = false;
		
		PointF[] boundPoints = new PointF[0];
		
		Matrix oriMatrix = new Matrix();
		bool updateinfo = false;
		Bitmap bmp = null;
		
		System.Drawing.Drawing2D.SmoothingMode smoothingmode = SmoothingMode.HighQuality;
		System.Drawing.Text.TextRenderingHint textrenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
		GraphicsPath selectpath = new GraphicsPath();
		bool firstLoad = true;
		int rectangleAngle = 0;
		Stroke stroke = new Stroke(Color.Black);
		Fill fill = new Fill(Color.White,1f);
		bool autoFit = false;
		bool zoomWhenWheel = false;
		Operation.Operation currentOperation = null;
		//		Operator currentOperator = Operator.None;
		Cursor defaultCursor = Forms.Cursors.Default;
		
		bool validMouse = true;
		
		Operation.DragDropEventHandler dragdrop = null;
		Operator preOp = Operator.None;
		//		Canvas drawArea =null;
		
		Point oldMousePos = Point.Empty;
		//		UnitType rule.UnitType = UnitType.Pixel;
		System.Windows.Forms.ContextMenu unitMenu = null;
		float unitStep = 100f;
		Rule rule = new Rule(true,UnitType.Pixel);
        Grid grid = new Grid(true, 10, Color.LightGray, true);
		Guide guide = new Guide(true,false,Color.Blue);
		//		System.Windows.Forms.SaveFileDialog savedialog = new SaveFileDialog();
		TextStyle textStyle = new TextStyle("Microsoft YaHei",12,false,false,false);
		Arrow startArrow = null;
		Arrow endArrow = null;
		ExportImageDialog exportdlg = null;
		//		Print.PrintDialog printdlg = null;
		
		StringFormat demoFormat = new StringFormat(StringFormat.GenericTypographic);
		//		Enum.ConnectType connectType = Enum.ConnectType.RightAngle;
		bool outline = false;
		Pen outlinePen = new Pen(Color.Black);
		TransformBehavior transformType = TransformBehavior.All;
		bool scrollable = true;
		Point oriPoint = Point.Empty;
		bool canclick = true;
		bool drawconnectPoint = true;
		Point oldPos = Point.Empty;
		float oldscale = 0;
		SVG.SVGElementCollection connectRefs = new SVGElementCollection();
		System.Windows.Forms.PropertyGrid propertyGrid = null;
		System.Random random = new Random();
		bool showCenterPointGrap = false;
		bool showResizeGrap = true;
		Color canvasColor = Color.White ;
		bool allowMoveLabel = false;
		bool showSelectedBounds = true;
        SVG.ConnectionType _connectType = SVG.ConnectionType.Dynamic;
		string bkImage = string.Empty;
		System.Drawing.Image canvasImage = null;

		//indicates whether only invalidate selection
		internal bool onlyInvalidateSelection = false;
		bool canBrokenConnector = true;
		bool createConnectorWithShape = false;
		ProtectType protectType = ProtectType.None;
		ThumbnailView thumbnailView = null;
		
		bool inProcess = false;
		/// <summary>
		/// Define the operation users want to disable
		/// </summary>
		Operator disabledOperator = Operator.None;

		/// <summary>
		/// define the features users want to disable
		/// </summary>
		DisabledFeatures disabledFeatures = DisabledFeatures.None;
		Hashtable elementsTransformType = new Hashtable();

		System.Collections.ArrayList imeChars = new ArrayList();
		bool createKeyPressEvent = true;
        DateTime preTime = DateTime.Now;
		#endregion

        #region ..internal event
		internal event EventHandler EditTextEvent;
		#endregion

		#region ..properties
        bool _validContent = true;
        internal bool validContent
        {
            set
            {
                if(this._validContent != value)
                    this._validContent = value;
            }
            get
            {
                return this._validContent;
            }
        }

        /// <summary>
        /// 计算实际运行时，画布离边界的边缘距离
        /// 当文档svg对象的长度宽度是按照100%进行缩放时，原有的margin将失效
        /// </summary>
        Margin RealMargin
        {
            get
            {
                if (this.ScaleWithWindowSize)
                {
                    return new Margin(0, 0, 0, 0);
                }
                return this.margin;
            }
        }

        bool oldScaleWithWindowSize = false;
        /// <summary>
        /// 获取当前svg对象的长度和宽度是否跟随窗口尺寸变化
        /// </summary>
        bool ScaleWithWindowSize
        {
            get
            {
                if (this.svgDocument != null)
                {
                    SVG.DocumentStructure.SVGSVGElement svg = this.svgDocument.RootElement;
                    if (svg != null)
                    {
                        if (svg.Width.UnitType == SVG.LengthType.SVG_LENGTHTYPE_PERCENTAGE
                            || svg.Height.UnitType == SVG.LengthType.SVG_LENGTHTYPE_PERCENTAGE)
                            return true;
                        return false;
                    }
                    else
                        return true;
                   
                }
                return false;
            }
        }

        /// <summary>
        /// 获取一个值，指示当前是否绘制Label标记点
        /// </summary>
        internal bool NeedDrawLabelIndicator
        {
            get
            {
                return false;
                //return this.currentOperation is Operation.SelectTransformOperation 
                //    && this.svgDocument.SelectCollection.Count == 1 
                //    && this.svgDocument.SelectCollection[0] is SVGTransformableElement 
                //    && (this.svgDocument.SelectCollection[0] as SVGTransformableElement).LabelText.Length > 0;
            }
        }

		/// <summary>
		/// 获取或设置默认鼠标
		/// </summary>
		internal new System.Windows.Forms.Cursor DefaultCursor
		{
			set
			{
				this.defaultCursor = value;
				this.Cursor = value;
			}
			get
			{
				return this.defaultCursor;
			}
		}

		/// <summary>
		/// gets the operation users want to disable
		/// </summary>
		internal Operator DisabledOperator
		{
			get
			{
				return this.disabledOperator;
			}
		}

		/// <summary>
		/// gets the features users want to disable
		/// </summary>
		internal DisabledFeatures DisabledFeatures
		{
			get
			{
				return this.disabledFeatures;
			}
		}

		/// <summary>
		/// gets or sets a value which indicates whether allow to move the label text
		/// </summary>
		internal bool AllowToMoveLabel
		{
			set
			{
				this.allowMoveLabel = value;
			}
			get
			{
				return this.allowMoveLabel;
			}
		}
		
		/// <summary>
		/// 获取或设置的水平视图位移
		/// </summary>
		internal int VirtualLeft
		{
			set
			{
				if(this.AutoScrollPosition.X != -value)
				{
					Point p = this.AutoScrollPosition;
					this.AutoScrollPosition = new Point(-value,p.Y);
					//					this.virtualLeft = value;
					this.Invalidate();
					//					this.OnViewPosChanged();
					//					this.hRule.Invalidate();
				}
			}
			get
			{
				return -this.AutoScrollPosition.X ;
			}
		}

		/// <summary>
		/// 获取当前变形
		/// </summary>
		internal Matrix CoordTransform
		{
			get
			{
                Matrix matrix = new Matrix();
                if (lastScrollPos.HasValue)
                    matrix.Translate(lastScrollPos.Value.X, lastScrollPos.Value.Y);
                else
                    matrix.Translate(-this.VirtualLeft, -this.VirtualTop);
                matrix.Translate(this.RealMargin.Left, this.RealMargin.Top);
                matrix.Scale(this.scaleRatio, this.scaleRatio);
                return matrix;
			}
		}

		/// <summary>
		/// 获取选区路径
		/// </summary>
		internal GraphicsPath SelectPath
		{
			get
			{
				return (GraphicsPath)this.selectpath.Clone();
			}
		}

		/// <summary>
		/// 获取选区变换矩阵
		/// </summary>
		internal Matrix SelectMatrix
		{
			get
			{
				return this.selectMatrix.Clone();
			}
		}

		/// <summary>
		/// 获取或设置文档
		/// </summary>
		internal SVG.Document.SVGDocument SVGDocument
		{
			get
			{
				return this.svgDocument;
			}
			set
			{
				if(this.svgDocument != value)
				{
					if(this.svgDocument != null)
					{
                        this.svgDocument.CurrentSceneChanged -= new EventHandler(svgDocument_CurrentSceneChanged);
						this.svgDocument.SelectionChanged -= new CollectionChangedEventHandler(ChangeSelect);
						this.svgDocument.NodeRemoved -= new System.Xml.XmlNodeChangedEventHandler(svgDocument_NodeRemoved);
                        this.svgDocument.ElementsChanged -= new CollectionChangedEventHandler(svgDocument_ElementsChanged);
                        svgDocument.Loaded -= new EventHandler(svgDocument_Loaded);
                        this.svgDocument.Dispose();
					}
					
					this.svgDocument = value;
                    if (this.svgDocument != null)
                    {
                        //this.svgDocument.AddRefDocument(Arrow.ArrowDocument);
                        this.svgDocument.CurrentSceneChanged += new EventHandler(svgDocument_CurrentSceneChanged);
                        this.svgDocument.SelectionChanged += new CollectionChangedEventHandler(ChangeSelect);
                        this.svgDocument.ElementsChanged += new CollectionChangedEventHandler(svgDocument_ElementsChanged);
                        svgDocument.Loaded += new EventHandler(svgDocument_Loaded);
                        this.svgDocument.NodeRemoved += new System.Xml.XmlNodeChangedEventHandler(svgDocument_NodeRemoved);
                        this.CurrentScene = this.svgDocument.RootElement;
                    }
                    else
                    {
                        this.CurrentScene = null;
                    }
					this.CalculateViewSize();
					this.firstLoad = true;
					this.oldselectPoint = PointF.Empty;
					this.selectMatrix.Reset();
					this.selectpath.Reset();
                    this.SetScroll();
					this.Invalidate();
				}
			}
		}

		/// <summary>
		/// 获取或设置当前垂直视图位移
		/// </summary>
		internal int VirtualTop
		{
			set
			{
				if(this.AutoScrollPosition.Y != -value)
				{
					Point p = this.AutoScrollPosition;
					this.AutoScrollPosition = new Point(p.X,-value);
					this.Invalidate();
				}
			}
			get
			{
				return -this.AutoScrollPosition.Y;
			}
		}
		#endregion

		#region ..OnPaint
        bool mouseDown = false;

        /// <summary>
        /// 判断当前是否在滚动视图
        /// </summary>
        bool InScroll
        {
            get
            {
                return this.scrolled && mouseDown;
            }
        }

        /// <summary>
        /// 引发Paint事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            
            if (this.svgDocument == null || this.Width == 0 || this.Height == 0)
            {
                this.firstLoad = false;
                return;
            }
            this.svgDocument.CurrentTime = 0;
            bool hasSelectionChanged = selectChanged;
            try
            {
                
                this.PaintContent(true, e.ClipRectangle);

                Graphics g = e.Graphics;
                Rectangle rect = e.ClipRectangle;
                // Get DC handle and create a compatible one    
                IntPtr hDC = g.GetHdc();
                IntPtr offscreenDC = CreateCompatibleDC(hDC);
                // Select our bitmap in to DC, recording what was there before    
                IntPtr hBitmap = bmp.GetHbitmap();
                IntPtr oldObject = SelectObject(offscreenDC, hBitmap);
                // Perform blt    
                BitBlt(hDC, rect.X, rect.Y, rect.Width, rect.Height, offscreenDC, rect.X, rect.Y, SRCCOPY);

                // Select our bitmap object back out of the DC    
                SelectObject(offscreenDC, oldObject);
                // Delete our bitmap    
                DeleteObject(hBitmap);
                // Delete memory DC and release our DC handle    
                DeleteDC(offscreenDC);
                g.ReleaseHdc(hDC);
                //e.Graphics.DrawImage(this.bmp,e.ClipRectangle,rect.X,rect.Y,rect.Width,rect.Height,GraphicsUnit.Pixel);
                this.DrawSelection(g);
                this.DrawCenterPoint(g);
                DrawLabelIndicator(g);
                base.OnPaint(e);
                this.DrawGuides(e);

                if (this.rule.Visible)
                {
                    this.vRule_Paint(this, e);
                    this.hRule_Paint(this, e);
                    this.label4_Paint(this, e);
                }
                this.connectRefs.Clear();
            }
            finally
            {
                this.onlyInvalidateSelection = false;
                this.SVGDocument.HasSomethingChanged = false;
                finishLoaded = true;
            }

            if (this.thumbnailView != null && this.scrolled)
            {
                this.thumbnailView.Invalidate();
            }

            if (this.scrollProgrammingChanged && hasInvalidateRegion)
            {
                this.scrollProgrammingChanged = false;
                selectChanged = hasSelectionChanged;
                this.Invalidate();
            }
            else if (this.ScaleWithWindowSize)
            {
                if (firstLoad)
                    this.SetScroll();
                else if (this.preBounds.IsEmpty || this.preBounds != this.contentBounds)
                {
                    this.preBounds = this.contentBounds;
                    this.SetScroll();
                }
            }
            if (this.firstLoad)
            {
                if (this.autoFit)
                {
                    this.FitCanvasToWindow();
                    this.ScrollToCenter();
                }
            }
            this.preBounds = this.contentBounds;
            this.firstLoad = false;
            this.scrolled = false;
            hasInvalidateRegion = false;
            
        }
		#endregion

		#region ..绘制中心点
		void DrawCenterPoint(Graphics g)
		{
			if(!(this.svgDocument.SelectCollection.Count == 1 && this.svgDocument.SelectCollection[0] is SVG.BasicShapes.SVGConnectionElement))
			{
				RectangleF rect = this.selectpath.GetBounds();
					
				if(this.svgDocument.SelectCollection.Count > 0)
				{
					PointF p = PointF.Empty;
					if(ChangeCenterPoint.IsEmpty)
					{
						PointF[] ps1 = new PointF[]{this.oldselectPoint};
						this.selectMatrix.TransformPoints(ps1);
						this.CenterPoint = ps1[0];
						p = ps1[0];
						ps1 = null;
					}
					else
					{
						this.CenterPoint = p = this.ChangeCenterPoint;
						using(Matrix tempm = this.selectMatrix.Clone())
						{
							tempm.Invert();
							PointF[] ps1 = new PointF[]{ChangeCenterPoint};
							tempm.TransformPoints(ps1);
							this.oldselectPoint = ps1[0];
							ps1 = null;
						}
					}
					this.ChangeCenterPoint = Point.Empty;
					int r = CenterGrap;
					System.Drawing.Drawing2D.GraphicsContainer c = g.BeginContainer();
					g.SmoothingMode = SmoothingMode.HighQuality;
					if(this.currentOperation is Operation.SelectTransformOperation && this.ShowCenterPointGrap)
					{
						g.FillEllipse(this.UnselectedBrush,p.X - r / 2,p.Y - r / 2,r,r);
						g.DrawEllipse(this.SelectedPen,p.X - r / 2,p.Y - r / 2,r,r);
						g.DrawEllipse(this.SelectedPen,p.X - 0.5f,p.Y - 0.5f,1,1);
					}
					g.EndContainer(c);
				}
			}
		}
        #endregion

        #region ..鼠标事件
        protected override void OnMouseDown(MouseEventArgs e)
		{
			if(this.SVGDocument == null || this.DesignMode)
				return;
			this.validMouse = true;
			this.Focus();
            mouseDown = e.Button == System.Windows.Forms.MouseButtons.Left;
			this.ImeMode = ImeMode.On;
			if(this.currentOperation == null || this.currentOperation.Finish)
				this.firstPoint = Point.Empty;
			if(e.Button == MouseButtons.Right && this.InRule(new Point(e.X,e.Y)) && this.unitMenu != null)
			{
				this.unitMenu.Show(this,new Point(e.X,e.Y));
                base.OnMouseDown(e);
				return ;
			}
			if(this.rule.Visible && (e.X <= this.ruleLength || e.Y <= this.ruleLength))
			{
				base.OnMouseDown(e);
			}
			else
			{
				PointF p = this.GetPoint(new PointF(e.X,e.Y),e.Button);
				if(e.Button == MouseButtons.Right && this.currentOperation is Operation.SelectTransformOperation)
				{
					this.SelectElementsInPoint(new Point(e.X,e.Y));
				}
				if(this.currentOperation == null || this.currentOperation.Finish)
					this.firstPoint = Point.Round(p);
				if(e.Button == MouseButtons.Left)
				{
					this.oriPoint = new Point(e.X,e.Y);
					this.canclick = true;
				}
				base.OnMouseDown(new TLMouseEventArgs(e.Button,e.Clicks,(int)p.X,(int)p.Y,e.Delta,e.X,e.Y));
			}
			
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			if(this.SVGDocument == null|| this.DesignMode)
				return;
            if (validMouse)
            {
                if (this.canclick && e.Button == MouseButtons.Left)
                {
                    if (new Point(e.X, e.Y) != this.oriPoint)
                        this.canclick = false;
                }
                PointF p = this.GetPoint(new PointF(e.X, e.Y), e.Button);
                base.OnMouseMove(new TLMouseEventArgs(e.Button, e.Clicks, (int)p.X, (int)p.Y, e.Delta, e.X,e.Y));
            }
            else
                base.OnMouseMove(e);
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{			
			if(this.SVGDocument == null|| this.DesignMode)
				return;
			this.validContent = true;
			if(!validMouse)
				return;
            mouseDown = false;
			//			if(this.HasSnap && e.Button == MouseButtons.Left)
			//			{
			
			
			PointF p = this.GetPoint(new PointF(e.X,e.Y),e.Button);
            base.OnMouseUp(new TLMouseEventArgs(e.Button, e.Clicks, (int)p.X, (int)p.Y, e.Delta,e.X,e.Y));
			this.validContent = true;
		}

		protected override void OnClick(EventArgs e)
		{
			if(this.canclick)
			{
                SVGElement element = this.GetElementAtLocation(this.PointToClient(Control.MousePosition));
                if (element != null)
                {
                    ElementClickEventArgs e1 = new ElementClickEventArgs(element, MouseClickType.SingleClick, Control.MouseButtons);
                    this.OnElementClick(e1);
                    if (!e1.Bubble)
                        return;
                }
                base.OnClick(e);
            }
            mouseDown = false;
		}
		#endregion

		#region ..Snap to Guide， Grid
        /// <summary>
        /// 如果吸附到网格，返回结果坐标
        /// </summary>
        /// <param name="point"></param>
        /// <param name="xAxis">对齐x</param>
        /// <param name="yAxis">对齐y</param>
        /// <returns></returns>
        internal PointF SnapPointToGrid(PointF point, bool xAxis, bool yAxis)
        {
            if ((this.VisualAlignment & VisualAlignment.Grid) != VisualAlignment.Grid)
                return point;
            PointF oriPoint = this.PointToVirtualView(point);
            
            int size = this.grid.Size;
            float vgrid = size;
            float hgrid = size;
            int x = (int)((float)(oriPoint.X + hgrid / 2f) / hgrid);
            float y = (int)((float)(oriPoint.Y + vgrid / 2f) / vgrid);
            oriPoint = new PointF( xAxis?(int)(x * hgrid):oriPoint.X, yAxis?(int)(y * vgrid):oriPoint.Y);
            return PointViewToClient(oriPoint);
        }

        internal GuideResult SnapPointToGuide(ref PointF point)
        {
            return SnapPointToGuide(ref point, true, true);
        }
        /// <summary>
        /// 如果吸附到参考线，返回结果坐标
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        internal GuideResult SnapPointToGuide(ref PointF point, bool xAXis, bool yAxis)
        {
            if ((this.VisualAlignment & VisualAlignment.Guide) != VisualAlignment.Guide)
                return GuideResult.None;
            PointF ori = point;
            PointF oriPoint = this.PointClientToView(point);
            float x = oriPoint.X;
            float y = oriPoint.Y;
            GuideResult result = GuideResult.None;
            if (this.vGuides.Count > 0 && xAXis)
            {
                int[] pos = new int[this.vGuides.Count];
                this.vGuides.CopyTo(pos);
                if (this.GuidePoint(ref x, pos))
                {
                    result = result | GuideResult.X;
                    oriPoint = new PointF(x, oriPoint.Y);
                }
            }
            if (this.hGuides.Count > 0 && yAxis)
            {
                int[] pos = new int[this.hGuides.Count];
                this.hGuides.CopyTo(pos);
                if (this.GuidePoint(ref y, pos))
                {
                    result = result | GuideResult.Y;
                    oriPoint = new PointF(oriPoint.X, y);
                }
            }
            point = this.PointViewToClient(oriPoint);
            return result;
        }

		/// <summary>
		/// 获取指定点坐标值,该坐标加入了对齐命令所产生的影响
		/// </summary>
		/// <param name="oriPoint"></param>
		/// <returns></returns>
		PointF GetPoint(PointF oriPoint,MouseButtons button)
		{
			if(this.currentOperation is Operation.Text.TextOperation)
				return oriPoint;
			if(button == MouseButtons.Left && (Control.ModifierKeys & Keys.Shift) == Keys.Shift && this.currentOperation != null && this.currentOperation.ShiftSnap && !this.firstPoint.IsEmpty)
			{
                if (this.anchorPoint.HasValue)
                    this.firstPoint = Point.Round(this.PointViewToClient(this.anchorPoint.Value));
				PointF start = this.firstPoint;
				PointF end = oriPoint;
				float angle = SVG.PathHelper.GetAngle(start,end);
				float scale = (float)angle / 45f;
				scale = (int)Math.Round(scale,0);
				scale = scale * 45;
				angle = scale;
				scale = (float)(scale / 180  * Math.PI);
				float r = (float)Math.Sqrt(Math.Pow(end.X - start.X,2) +Math.Pow(end.Y - start.Y,2));
				float x = (start.X + r * (float)Math.Cos(scale));
				float y = (start.Y + r * (float)Math.Sin(scale));
				if(angle == 90 || angle == 270)
					return new PointF(start.X,y);
				else if(angle == 180 || angle == 0 || angle == 360)
					return new PointF(x,start.Y);
				return new PointF(x,y);
			}
            if (button == MouseButtons.Left && ((this.VisualAlignment & VisualAlignment.Grid) != VisualAlignment.Grid && (this.VisualAlignment & VisualAlignment.Guide) != VisualAlignment.Guide))
                return oriPoint;
            if (button == System.Windows.Forms.MouseButtons.Left)
            {
                if (this.currentOperation!= null && this.currentOperation.NeedAlignToGrid)// || this.currentOperation is Operation.PolyOperation || this.currentOperation is Operation.BezierSplineOperation || this.currentOperation is Operation.NodeEditOperation)
                {
                    var result = this.SnapPointToGuide(ref oriPoint);
                    if (result == GuideResult.None)
                        oriPoint = this.SnapPointToGrid(oriPoint,(result & GuideResult.X)!= GuideResult.X, (result & GuideResult.Y) != GuideResult.Y);
                }
            }
            return oriPoint;
		}

		bool GuidePoint(ref float pos,int[] poses)
		{
			if(poses == null || poses.Length == 0)
				return false;
			Array.Sort(poses);
			int left = 0;
			int right = poses.Length - 1;
			int middle = (int)((left + right) / 2);
			while(left < right)
			{
				int temp = poses[middle];
                if (Math.Abs(temp - pos) < SnapMargin)
                {
                    pos = temp;
                    return true;
                }
                else if (temp > pos)
                    right = middle;
                else
                    left = middle;
				middle = (int)((left + right) / 2);
				if(middle == left || middle == right)
				{
					left = middle;
					break;
				}
			}
			float temp1 = poses[left];
            if (Math.Abs(pos - temp1) < SnapMargin)
            {
                pos = poses[left];
                return true;
            }
            else if (Math.Abs(pos - poses[right]) < SnapMargin)
            {
                pos = poses[right];
                return true;
            }
            return false;
		}
		#endregion

		#region ..处理键盘事件
		protected override bool ProcessDialogKey(Keys keyData)
		{
			if(this.currentOperation != null)
			{
				if(this.currentOperation.ProcessDialogKey(keyData))
					return true;
			}
			if(Control.MouseButtons == MouseButtons.None)
			{
				if(keyData == Keys.Delete)
				{
					if(this.currentOperation != null)
						this.currentOperation.Reset();
					this.DeleteSelection();
					return true;
				}
				using(System.Drawing.Drawing2D.Matrix matrix = new Matrix())
				{
					switch(keyData)
					{
						case Keys.Tab:
							return true;
						case Keys.Left:
							matrix.Translate(-1,0);
							break;
						case Keys.Right:
							matrix.Translate(1,0);
							break;
						case Keys.Up:
							matrix.Translate(0,-1);
							break;
						case Keys.Down:
							matrix.Translate(0,1);
							break;
					}
					if(!matrix.IsIdentity)
					{
						if(this.currentOperation != null)
							this.currentOperation.Reset();
						this.TransformSelection(matrix);
						return true;;
					}
				}
			}
			return base.ProcessDialogKey(keyData);
		}

		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			if(this.createKeyPressEvent)
				base.OnKeyPress (e);
		}
		#endregion

		#region ..删除选区
		internal void DeleteSelection()
		{
			if(this.currentOperation != null && this.currentOperation.ProcessDialogKey(Keys.Delete))
				return;
			if((this.protectType & ProtectType.Delete) == ProtectType.Delete)
				return;
			SVG.Document.SVGDocument doc = this.SVGDocument;
			if(doc.SelectCollection .Count > 0)
			{
				bool old = doc.AcceptNodeChanged;
				doc.AcceptNodeChanged = true;
				SVGElement[] list = doc.SelectCollection.ToArray();
				for(int i = 0;i<list.Length;i++)
				{
					SVG.SVGElement element1 = list[i] as SVG.SVGElement;
					if(element1 != this.SVGDocument.RootElement && element1.ParentElement != null)
                        element1.ParentElement.InternalRemoveChild(element1);
				}
				doc.InvokeUndos();
				doc.AcceptNodeChanged = old;
				doc.ChangeSelectElements(null as SVG.SVGElementCollection);
			}
		}
		#endregion

		#region ..绑定
		internal void AttachDragDrop()
		{
			if(this != null)
				this.dragdrop = new Operation.DragDropEventHandler(this);
		}
		#endregion

	    #region ..确定视图尺寸
		void CalculateViewSize()
		{
			if(this.svgDocument == null)
				return;
			SVG.DocumentStructure.SVGSVGElement svg = this.svgDocument.RootElement;
            float width = 0;
            float height = 0;
            if (svg != null)
            {
                svg.WindowViewSize = this.ClientSize;
                if (svg != null)
                {
                    width = svg.Width.Value;
                    height = svg.Height.Value;

                }
            }
			if(width == 0)
				width = this.Width;
			if(height == 0)
				height = this.Height;
			this.viewSize = new SizeF(width,height);
		}
		#endregion	

		#region ..绘制网格
		void PaintGrid(Graphics g,RectangleF rect,SizeF size)
		{
			if(!this.grid.Visible)
				return;

			float width = (float)Math.Round(size.Width ,1);
			float height = (float)Math.Round(size.Height,1);
			float w = width;
			float h = height;
			float a = width - (int)width;
			float b = height - (int)height;
			if(a != 0)
				w = width * 10;
			if(b != 0)
				h = width * 10;
			using(Bitmap bmp = new Bitmap((int)w,(int)h))
			{
				using(Pen pen = new Pen(this.grid.Color))
				{
					pen.DashStyle = DashStyle.Dot;
					using(Graphics g1 = Graphics.FromImage(bmp))
					{
                        if (grid.GridType != GridType.Line)
                            pen.DashPattern = new float[] { 0.2f, width, 0.2f};
                        for (int i = 1; i < 10; i++)
                        {
                            if (i * width > bmp.Width)
                                break;
                            g1.DrawLine(pen, i * width, 0, i * width, bmp.Height);
                        }
                        for (int i = 1; i < 10; i++)
                        {
                            if(i * height > bmp.Height)
                                break;
                            g1.DrawLine(pen, 0, i * height, bmp.Width, i * height);
                        }
                        g1.DrawRectangle(pen, 0, 0, bmp.Width, bmp.Height);
                        
						using(System.Drawing.TextureBrush brush = new TextureBrush(bmp))
						{
							GraphicsContainer c = g.BeginContainer();
							g.TranslateTransform(rect.X,rect.Y);
                            //if(!this.grid.FillScreen)
                                g.FillRectangle(brush,0,0,rect.Width,rect.Height);
                            //else
                            //{
                            //    PointF[] ps = new PointF[]{new Point(0,0),new PointF((int)Math.Max(this.Width,this.AutoScrollMinSize.Width),(int)Math.Max(this.Height,this.AutoScrollMinSize.Height))};
                            //    this.PointToVirtualView(ps);
                            //    g.FillRectangle(brush,ps[0].X,ps[0].Y,ps[1].X - ps[0].X,ps[1].Y - ps[0].Y);
                            //}
							g.EndContainer(c);
						}
					}
				}
			}
		}
		#endregion

        #region ..绘制内容
        void PaintContent(bool check)
		{
			this.PaintContent(check,new Rectangle(0,0, this.ClientSize.Width, this.ClientSize.Height));
		}

        void PaintContent(bool check, Rectangle clipRect)
        {
            CreateMemoryBitmap();
            if (check && (this.svgDocument == null || this.svgDocument.DocumentElement == null || this.svgDocument.InTransaction))
                return;
            using (Graphics g = Graphics.FromImage(bmp))
            {
                this.PaintContentToGraphics(g, clipRect);
            }
        }

        void PaintContentToGraphics(Graphics g, Rectangle clipRect)
        {
            var currentPoint = this.AutoScrollPosition;
            if (lastScrollPos.HasValue)
                currentPoint = lastScrollPos.Value;
            if (!this.svgDocument.HasSomethingChanged)
            {
                //if (InScroll && this.oldPos == currentPoint)
                //{
                //    this.clipRectangles.Clear();
                //    return;
                //}
                if ((!this.validContent) && this.oldPos == currentPoint && this.oldscale == this.scaleRatio && this.currentOperator != Operator.Connection)
                {
                    this.clipRectangles.Clear();
                    return;
                }

                //when only update the selection and no node changed,return
                if (this.onlyInvalidateSelection)
                {
                    this.clipRectangles.Clear();
                    return;
                }
            }

            List<Rectangle> clipRects = new List<Rectangle>();
            clipRects.AddRange(this.clipRectangles);
            this.clipRectangles.Clear();
            if (this.oldPos != currentPoint)
            {
                int xDelta = (int)Math.Abs(currentPoint.X - this.oldPos.X);
                int yDelta = (int)Math.Abs(currentPoint.Y - this.oldPos.Y);

                if (xDelta != 0)
                {
                    int delta = xDelta >= MinScrollDistance ? xDelta : MinScrollDistance;
                    if (currentPoint.X > this.oldPos.X)
                    {
                        Rectangle rect = new Rectangle(0, 0, delta, this.ClientSize.Height);
                        if (!clipRects.Contains(rect))
                            clipRects.Add(rect);
                    }
                    else
                    {
                        Rectangle rect = new Rectangle(this.ClientSize.Width - delta, 0, delta, this.ClientSize.Height);
                        if (!clipRects.Contains(rect))
                            clipRects.Add(rect);
                    }
                }

                if (yDelta != 0)
                {
                    int delta = yDelta >= MinScrollDistance ? yDelta : MinScrollDistance;
                    //if (currentPoint.Y > this.oldPos.Y)
                    {
                        Rectangle rect = new Rectangle(0, 0, this.ClientSize.Width, delta);
                        if (!clipRects.Contains(rect))
                            clipRects.Add(rect);
                    }
                    //else
                    {
                        Rectangle rect = new Rectangle(0, this.ClientSize.Height - delta, this.ClientSize.Width, delta);
                        if (!clipRects.Contains(rect))
                            clipRects.Add(rect);
                    }
                }
            }

            if (this.oldScaleWithWindowSize != this.ScaleWithWindowSize)
                this.CalculateViewSize();
            this.oldScaleWithWindowSize = this.ScaleWithWindowSize;

            using (SVG.StyleContainer.StyleOperator sp = this.svgDocument.CreateStyleOperator())
            {
                try
                {
                    if (clipRects.Count == 0)
                        clipRects.Add(clipRect);
                    using (Matrix coordTransform = this.CoordTransform)
                    {
                        List<Rectangle> rects = new List<Rectangle>();
                        if (this.svgDocument.RootElement != null)
                        {
                            //Normalize the clip rectangles
                            for (int i = 0; i < clipRects.Count; i++)
                            {
                                Rectangle rect = clipRects[i];
                                rect.Intersect(clipRect);
                                if (rect.IsEmpty)
                                    continue;
                                rects.Add(rect);
                            }
                            clipRects.Clear();
                            if (rects.Count == 0)
                                return;
                        }

                        //当rects对象超过最大数值时，不再做过滤动作
                        if (rects.Count < MaxInvalidateRects)
                        {
                            //Draw the previous screenshot with the offset
                            g.DrawImageUnscaledAndClipped(this.bmp, new Rectangle(currentPoint.X - oldPos.X, currentPoint.Y - oldPos.Y, bmp.Width, bmp.Height));

                            if (rects.Count > 0)
                            {
                                g.SetClip(Rectangle.Empty);
                                sp.ClipRegion.MakeEmpty();

                                //Only invalidate the clip rectangle
                                for (int i = 0; i < rects.Count; i++)
                                {
                                    Rectangle rect = rects[i];
                                    g.SetClip(rect, CombineMode.Union);
                                }

                                using (Region rg = g.Clip.Clone() as Region)
                                {
                                    using (Matrix matrix = coordTransform.Clone())
                                    {
                                        matrix.Invert();
                                        rg.Transform(matrix);
                                        sp.ClipRegion.Union(rg);
                                    }
                                }
                            }
                        }
                        g.Clear(this.BackColor);

                        //if (!DesignMode)
                        //    bmp.Save(DateTime.Now.ToFileTimeUtc().ToString() + ".bmp");
                        if (this.BackgroundImage != null)
                            this.DrawTileImage(g, this.BackgroundImage, new Rectangle(currentPoint.X, currentPoint.Y, (int)Math.Max(this.ClientSize.Width, this.AutoScrollMinSize.Width), (int)Math.Max(this.ClientSize.Height, this.AutoScrollMinSize.Height)));

                        SizeF size = this.viewSize;
                        float width = size.Width;
                        float height = size.Height;
                        PointF[] ps = new PointF[] { new PointF(0, 0), new PointF(width, height), new PointF(this.grid.Size, this.grid.Size) };
                        coordTransform.TransformPoints(ps);

                        //如果按照窗口进行缩放，则viewRectangle的尺寸最小不小于ViewSize
                        viewRectangle = new RectangleF(ps[0].X, ps[0].Y, ps[1].X - ps[0].X, ps[1].Y - ps[0].Y);
                        if (this.ScaleWithWindowSize)
                        {
                            size.Width = size.Width < this.AutoScrollMinSize.Width ? this.AutoScrollMinSize.Width : size.Width;
                            size.Height = size.Height < this.AutoScrollMinSize.Height ? this.AutoScrollMinSize.Height : size.Height;
                            viewRectangle.Width = viewRectangle.Width < size.Width ? size.Width : viewRectangle.Width;
                            viewRectangle.Height = viewRectangle.Height < size.Height ? size.Height : viewRectangle.Height;
                        }
                        using (SolidBrush brush = new SolidBrush(this.canvasColor))
                            g.FillRectangle(brush, viewRectangle);

                        if (this.canvasImage != null)
                        {
                            GraphicsContainer c1 = g.BeginContainer();
                            g.SetClip(viewRectangle);
                            this.DrawTileImage(g, this.canvasImage, Rectangle.Round(viewRectangle));//g.DrawImageUnscaled(this.canvasImage,(int)ps[0].X,(int)ps[0].Y);
                            g.EndContainer(c1);
                        }
                        this.PaintGrid(g, viewRectangle, new SizeF(this.grid.Size * coordTransform.Elements[0], this.grid.Size * coordTransform.Elements[3]));
                        if (this.grid.Visible && this.grid.DrawBorder)
                            g.DrawRectangle(new Pen(ControlPaint.DarkDark(this.canvasColor)), viewRectangle.X, viewRectangle.Y, viewRectangle.Width, viewRectangle.Height);
                        //this.connectableElements.Clear();
                        //this.renderElements.Clear();
                        sp.connectElements = this.connectableElements;
                        sp.BoundView = this.outline;
                        sp.outlinePen = this.outlinePen;
                        sp.contentBounds = RectangleF.Empty;
                        sp.drawConnects = this.drawconnectPoint;
                        sp.renderElements = this.renderElements;
                        //sp.coordTransform = coordTransform.Clone();
                        sp.RememberOperationData = true;
                        sp.AutoBridgeForConnect = this.autoCreateBridgeWhenConnectorCross;
                        if (this.PaintConnectablePoint != null)
                            sp.PaintConnectablePoint += new PaintConnectablePointEventHandler(sp_PaintConnectablePoint);
                        sp.DrawShadow = ((this.disabledFeatures & DisabledFeatures.Shadow) != DisabledFeatures.Shadow);
                        sp.ConnectPath.Reset();
                        GraphicsContainer c = g.BeginContainer();
                        g.SmoothingMode = this.smoothingmode;
                        g.TextRenderingHint = this.textrenderingHint;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.Transform = coordTransform.Clone();

                        SVGTransformableElement root = this.svgDocument.CurrentScene as SVGTransformableElement;
                        if (root != null && root.SVGRenderer != null)
                            root.SVGRenderer.Draw(g, sp);
                        else if (this.svgDocument.CurrentScene is SVG.Interface.ISVGContainer)
                        {
                            var childs = (this.svgDocument.CurrentScene as SVG.Interface.ISVGContainer).ChildElements;
                            for (int i = 0; i < childs.Count; i++)
                            {
                                SVG.SVGTransformableElement render = childs[i] as SVG.SVGTransformableElement;
                                render.SVGRenderer.Draw(g, sp);
                            }
                        }
                        g.Transform = coordTransform;
                        g.EndContainer(c);
                        if (this.PaintConnectablePoint != null)
                            sp.PaintConnectablePoint -= new PaintConnectablePointEventHandler(sp_PaintConnectablePoint);
                    }
                }
                catch (System.Exception e1)
                {
                    this.svgDocument.OnExceptionOccured(new ExceptionOccuredEventArgs(new object[] { e1.Message, e1.StackTrace }, ExceptionLevel.Normal));
                }
                finally
                {
                    if (this.PaintConnectablePoint != null)
                        sp.PaintConnectablePoint -= new PaintConnectablePointEventHandler(sp_PaintConnectablePoint);
                }

                this.contentBounds = Rectangle.Ceiling(sp.contentBounds);
            }


            this.oldPos = currentPoint;
            lastScrollPos = null;
            this.oldscale = this.scaleRatio;
            this.svgDocument.ForceRefreshAllElementToBeUpdated();
            this.svgDocument.FinishReDraw();
        }

        void sp_PaintConnectablePoint(object sender, PaintConnectablePointEventArgs e)
        {
            this.OnPaintConnectablePoint(e);
        }
		#endregion

		#region ..DrawSelection
		void DrawSelection(Graphics g)
		{
			if(this.svgDocument.SelectCollection.Count == 1 && this.svgDocument.SelectCollection[0] is SVG.Text.SVGTextElement && !(this.svgDocument.SelectCollection[0] as SVG.Text.SVGTextElement).Render)
				return;
            if (this.currentOperation != null && this.currentOperation.EditText)
                return;
            if (this.updateinfo)
            {
                this.oriMatrix.Reset();
            }
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			this.UpdateSelectInfo();
            this.selectMatrix.Reset();
            this.selectMatrix.Multiply(this.CoordTransform);
            this.selectMatrix.Multiply(this.oriMatrix);
            if (this.ShowSelectionHighlightOutline)
            {
                using (GraphicsPath path = (GraphicsPath)this.selectpath.Clone())
                {
                    path.Transform(this.selectMatrix);
                    g.DrawPath(this.SelectedPen, path);
                }
            }
			g.Transform.Reset();

            if (this.svgDocument.SelectCollection.Count == 1 && (this.svgDocument.SelectCollection[0] is SVG.BasicShapes.SVGConnectionElement))
                return;
			
			#region ..绘制操作手柄
			if(this.selectpath.PointCount > 1)
			{
				if(!(this.currentOperation is Operation.SelectTransformOperation))
					return;
				if(!this.selectMatrix.IsInvertible)
					return;
				using(GraphicsPath handpath = new GraphicsPath(),selectpath1 = new GraphicsPath())
				{
					float minX = 0;
					float minY = 0;
					//检测最小边界
					PointF[] ps1 = new PointF[]{new PointF(0,0),new PointF(0,MinGrap),new PointF(MinGrap,0)};
					this.selectMatrix.TransformPoints(ps1);
					minX = (float)Math.Sqrt(Math.Pow(ps1[2].X - ps1[0].X,2) + Math.Pow(ps1[2].Y - ps1[0].Y,2));
					minX = (float)(MinGrap * MinGrap) / minX;
					minY = (float)Math.Sqrt(Math.Pow(ps1[1].Y - ps1[0].Y,2) + Math.Pow(ps1[1].X - ps1[0].X,2));
					minY = (float)(MinGrap * MinGrap) / minY;
					ps1 = null;
					RectangleF rect1 = this.selectpath.GetBounds();
					if(this.showSelectedBounds)
					{
						selectpath1.AddPath(this.selectpath,false);
						if(rect1.Width < minX)
						{
							rect1.X = rect1.X + rect1.Width / 2 - minX/ 2;
							rect1.Width = minX;
						}
						if(rect1.Height < minY)
						{
							rect1.Y = rect1.Y + rect1.Height / 2 - minY/ 2;
							rect1.Height = minY;
						}

						handpath.AddRectangle(rect1);
						handpath.Transform(this.selectMatrix);
                        g.DrawPath(this.SelectedPen, handpath);
					}
					if(!this.showResizeGrap)
						return;
					this.boundPoints = new PointF[]{new PointF(rect1.X,rect1.Y),new PointF(rect1.X + rect1.Width / 2,rect1.Y),new PointF(rect1.Right,rect1.Y),new PointF(rect1.Right,rect1.Y + rect1.Height / 2),new PointF(rect1.Right,rect1.Bottom),new PointF(rect1.X + rect1.Width / 2,rect1.Bottom),new PointF(rect1.X,rect1.Bottom),new PointF(rect1.X ,rect1.Y + rect1.Height /2)};
					PointF[] ps = (PointF[])this.boundPoints.Clone();
					this.selectMatrix.TransformPoints(ps);
					handpath.Reset();
					int r = grapSize / 2;
					bool hasempty = rect1.Height == 0 || rect1.Width == 0;
					for(int i = 0;i<ps.Length;i++)//(Point point in ps)
					{
						if(!this.boundPoints[i].IsEmpty || !hasempty )
						{
							PointF point = ps[i];
							handpath.StartFigure();
							handpath.AddRectangle(new RectangleF(point.X -r,point.Y - r,grapSize,grapSize));
						}
					}
					handpath.FillMode = FillMode.Winding;
                    if (this.Document.SelectCollection.ContainsLockedElement)
                        g.FillPath(this.LockBrush, handpath);
                    else
                    {
                        g.FillPath(this.UnselectedBrush, handpath);
                        g.DrawPath(this.SelectedPen, handpath);
                    }
					ps = null;
				}
			}
			#endregion
		}
		#endregion

        #region ..DrawLabelIndicator
        void DrawLabelIndicator(Graphics g)
        {
            if (this.NeedDrawLabelIndicator)
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                using (Pen pen = new Pen(this.LabelIndicator, 1))
                {
                    int r = grapSize * 3 / 4;
                    PointF p = this.GetLabelIndicatorPos();
                    RectangleF rect = new RectangleF(p.X - r, p.Y - r, 2 * r, 2 * r);

                    pen.Color = ControlPaint.DarkDark(pen.Color);
                    g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
                    g.FillRectangle(LabelIndicator, rect);
                }
            }
        }
        #endregion

        #region ..FitCanvasToWindow
        /// <summary>
		/// 缩放画布，是的画布适应窗口尺寸
		/// </summary>
		public void FitCanvasToWindow()
		{
			float scale = this.scaleRatio;
			float height = this.viewSize.Height;
			float width = this.viewSize.Width;
            //适应窗口尺寸，按照内容进行缩放
            if (this.ScaleWithWindowSize && this.svgDocument.RootElement.IsActive)
            {
                if (this.contentBounds.IsEmpty)
                {
                    SVGTransformableElement svg = this.svgDocument.CurrentScene as SVGTransformableElement;
                    SVG.Interface.ISVGPathable pathable = svg as SVG.Interface.ISVGPathable;
                    if (svg != null && pathable != null && pathable.GPath != null && pathable.GPath.PointCount > 1)
                    {
                        using (GraphicsPath path = pathable.GPath.Clone() as GraphicsPath)
                        {
                            path.Transform(svg.TotalTransform);
                            //path.Transform(this.CoordTransform);
                            SizeF size = path.GetBounds().Size;
                            width = size.Width;
                            height = size.Height;
                        }
                    }
                }
                else
                {
                    width = this.contentBounds.Width;
                    height = this.contentBounds.Height;
                }
            }
			scale = (float)(this.Height - 60) / height;
			scale = (float)Math.Min(scale, (float)(this.Width - 60)/width);
			bool scroll = this.scaleRatio == scale;
			this.ScaleRatio = scale;
            if (scroll)
            {
                this.SetScroll();
            }

            this.ScrollToCenter();
		}
		#endregion

        #region ..滚动条
        /// <summary>
		/// 当缩放比率改变时，使滚动条适应窗口
		/// </summary>
		internal void SetScroll()
		{
			using(System.Drawing.Drawing2D.Matrix matrix = this.GetCurrentMatrix())
			{
				SizeF size = this.viewSize;
                if (this.ScaleWithWindowSize)
                {
                    if (this.contentBounds.IsEmpty)
                    {
                        SVG.DocumentStructure.SVGSVGElement svg = this.svgDocument.RootElement;
                        if (svg != null && svg.IsActive)
                        {
                            GraphicsPath path = (svg as SVG.Interface.ISVGPathable).GPath;
                            if (path != null)
                            {
                                RectangleF rect = path.GetBounds();
                                this.contentBounds = Rectangle.Ceiling(rect);
                            }
                        }
                    }

                    Rectangle rect1 = this.contentBounds;
                    rect1.Inflate(bottomMargin);

                    size = new SizeF(rect1.Right, rect1.Bottom);
                }
				float height = size.Height;
				float width = size.Width;
				PointF[] ps = new PointF[]{new PointF(0,0),new PointF(width,height)};
				matrix.TransformPoints(ps);
				int max = (int)(ps[1].X - ps[0].X) + this.RealMargin.Left + this.RealMargin.Right;
				int max1 = (int)(ps[1].Y - ps[0].Y) +this.RealMargin.Top + this.RealMargin.Bottom;
				if(this.AutoScroll)
					this.AutoScrollMinSize = new Size(max,max1);
				else
					this.AutoScrollMinSize = new Size(this.Width + (int)size.Width,this.Height + (int)size.Height);				
			}
		}
		#endregion

		#region ..OnResize
		protected override void OnResize(EventArgs e)
		{
			this.bmp = null;
            this.CreateMemoryBitmap();
			base.OnResize (e);
            this.CalculateViewSize();
			this.AutoScroll = this.scrollable;
			this.Invalidate();
		}
		#endregion

        #region ..CreateMemoryBitmap
        void CreateMemoryBitmap()
        {
            if (this.bmp == null && this.Width > 0 && this.Height > 0)
                this.bmp = new Bitmap(this.Width, this.Height);
        }
        #endregion

        #region ..PointToVirtualView
        /// <summary>
		/// 将控件屏幕坐标转换为视图坐标
		/// </summary>
		/// <param name="areaPoint">控件坐标空间中的坐标度量</param>
		internal PointF PointToVirtualView(PointF areaPoint)
		{
			PointF[] ps = new PointF[]{areaPoint};
			this.PointsClientToView(ps);
			PointF p = ps[0];//new Point((int)Math.Round(ps[0].X,0),(int)Math.Round(ps[0].Y,0));;
			ps = null;
			return p;
		}

		/// <summary>
		/// 将视图坐标转换为工作区坐标
		/// </summary>
		/// <param name="viewPoint">视图坐标系(画布)坐标</param>
		/// <returns>工作区(基于控件)坐标</returns>
		public PointF PointViewToClient(PointF viewPoint)
		{
			PointF[] ps = new PointF[]{viewPoint};
			this.CoordTransform.TransformPoints(ps);

			PointF p = new PointF((float)Math.Round(ps[0].X,0),(float)Math.Round(ps[0].Y,0));;
			ps = null;
			return p;
		}
		#endregion

        #region ..GetContent
        internal Bitmap GetContent()
		{
			if(this.bmp == null)
				this.PaintContent(false); 
			return this.bmp;
		}
		#endregion

        #region ..GetMousePos
        /// <summary>
		/// 判断屏幕某点相对于选区的位置
		/// </summary>
		/// <param name="screenPoint">屏幕坐标</param>
		/// <returns></returns>
		internal Operation.MousePoint GetMousePos(PointF screenPoint,ref int rotateindex)
		{
			if(this.boundPoints.Length == 0 || this.Document.SelectCollection.ContainsLockedElement)
				return Operation.MousePoint.None;
			Operator operation = this.currentOperator;
			Point point = Point.Round(screenPoint);
			//			this.selectpath.Flatten();
			RectangleF selectRect = this.selectpath.GetBounds();
			bool hasempty = selectRect.IsEmpty;
			PointF[] ps = (PointF[])this.boundPoints.Clone();
			this.selectMatrix.TransformPoints(ps);
			int r = grapSize / 2;
			RectangleF rect1 = new RectangleF(ps[0].X - r,ps[0].Y - r,grapSize ,grapSize );
			
			Operation.MousePoint mousePoint = Operation.MousePoint.None;
			
			#region ..匹配相对位置
			using(GraphicsPath 
					  left = new GraphicsPath(),
					  right = new GraphicsPath(),
					  top = new GraphicsPath(),
					  bottom = new GraphicsPath(),
					  rotate1 = new GraphicsPath(),
					  rotate2 = new GraphicsPath(),
					  rotate3 = new GraphicsPath(),
					  rotate4 = new GraphicsPath(),
					  center = new GraphicsPath(),
					  path = this.selectpath.Clone() as GraphicsPath
					  )
			{
			
				
				RectangleF topleft = RectangleF.Empty;
				if(!this.boundPoints[0].IsEmpty || !hasempty)
					topleft = new RectangleF(ps[0].X - r - 1,ps[0].Y - r - 1,grapSize + 2,grapSize + 2);
				//				topleft.AddRectangle(rect1);	

				RectangleF topright = RectangleF.Empty;
				if(!this.boundPoints[2].IsEmpty|| !hasempty)
					topright = new RectangleF(ps[2].X - r - 1,ps[2].Y - r - 1,grapSize +2,grapSize + 2);
				//				topright.AddRectangle(rect1);
				//			topright.Transform(matrix);

				RectangleF topmiddle= RectangleF.Empty;
				if(!this.boundPoints[1].IsEmpty|| !hasempty)
					topmiddle = new RectangleF(ps[1].X - r - 1,ps[1].Y - r - 1,grapSize +2,grapSize + 2);
				
				//				topmiddle.AddRectangle(rect1);
				//			topmiddle.Transform(matrix);

				RectangleF middleleft = RectangleF.Empty;
				if(!this.boundPoints[7].IsEmpty|| !hasempty)
					middleleft = new RectangleF(ps[7].X- r - 1,ps[7].Y - r - 1,grapSize + 2,grapSize + 2);
				
				//				middleleft.AddRectangle(rect1);
				//			middleleft.Transform(matrix);

				RectangleF bottomleft = RectangleF.Empty;
				if(!this.boundPoints[6].IsEmpty|| !hasempty)
					bottomleft = new RectangleF(ps[6].X - r - 1,ps[6].Y - r - 1,grapSize + 2,grapSize + 2);
				//				bottomleft.AddRectangle(rect1);
				//			bottomleft.Transform(matrix);

				RectangleF middleright = RectangleF.Empty;
				if(!this.boundPoints[3].IsEmpty|| !hasempty)
					middleright = new RectangleF(ps[3].X - r-1,ps[3].Y - r - 1,grapSize +2,grapSize + 2);
				
				//				middleright.AddRectangle(rect1);
				//			middleright.Transform(matrix);

				RectangleF bottommiddle = RectangleF.Empty;
				if(!this.boundPoints[5].IsEmpty|| !hasempty)
					bottommiddle = new RectangleF(ps[5].X - r-1,ps[5].Y - r-1,grapSize +2,grapSize +2);
				
				//				bottommiddle.AddRectangle(rect1);
				//			bottommiddle.Transform(matrix);

				RectangleF bottomright = RectangleF.Empty;
				if(!this.boundPoints[4].IsEmpty|| !hasempty)
					bottomright = new RectangleF(ps[4].X - r-1,ps[4].Y - r-1,grapSize +2,grapSize +2);
				
				//				bottomright.AddRectangle(rect1);
				//			bottomright.Transform(matrix);

				if(ps[0] != ps[6])				
					left.AddLine(ps[0],ps[6]);
				//			left.Transform(matrix);
			
				if(ps[2] != ps[4])
					right.AddLine(ps[2],ps[4]);
				//			right.AddRectangle(new RectangleF(bounds.X + bounds.Width - 1,bounds.Y ,3,bounds.Height));
				//			right.Transform(matrix);
			
				if(ps[0] != ps[2])
					top.AddLine(ps[0],ps[2]);
				//			top.AddRectangle(new RectangleF(bounds.X,bounds.Y - 1 ,bounds.Width,3));
				//			top.Transform(matrix);
			
				if(ps[6] != ps[4])
					bottom.AddLine(ps[6],ps[4]);
				//			bottom.AddRectangle(new RectangleF(bounds.X,bounds.Bottom - 1 ,bounds.Width ,3));
				//			bottom.Transform(matrix);
		
				if(!this.boundPoints[0].IsEmpty)
					rotate1.AddArc(ps[0].X - 8,ps[0].Y - 8,16,16,90,270);
				//			rotate1.AddRectangle(new RectangleF(ps[0].X - 8,ps[0].Y - 8,8,8));

				if(!this.boundPoints[2].IsEmpty)
				{
					rotate1.StartFigure();
					rotate1.AddArc(ps[2].X - 8,ps[2].Y - 8,16,16,-180,270);
				}

				if(!this.boundPoints[4].IsEmpty)
				{
					rotate1.StartFigure();
					rotate1.AddArc(ps[4].X - 8,ps[4].Y - 8,16,16,-90,270);
				}
			
				if(!this.boundPoints[6].IsEmpty)
				{
					rotate1.StartFigure();
					rotate1.AddArc(ps[6].X - 8,ps[6].Y - 8,16,16,0,270);
				}

				if(!this.boundPoints[1].IsEmpty)
				{
					rotate1.StartFigure();
					rotate1.AddArc(ps[1].X - 8,ps[1].Y - 8,16,16,0,270);
				}

				if(!this.boundPoints[3].IsEmpty)
				{
					rotate1.StartFigure();
					rotate1.AddArc(ps[3].X - 8,ps[3].Y - 8,16,16,0,270);
				}

				if(!this.boundPoints[5].IsEmpty)
				{
					rotate1.StartFigure();
					rotate1.AddArc(ps[5].X - 8,ps[5].Y - 8,16,16,0,270);
				}

				if(!this.boundPoints[7].IsEmpty)
				{
					rotate1.StartFigure();
					rotate1.AddArc(ps[7].X - 8,ps[7].Y - 8,16,16,0,270);
				}

				PointF[] ps1 = new PointF[]{this.oldselectPoint};
				this.selectMatrix.TransformPoints(ps1);
				
				if(!ps1[0].IsEmpty)
				{
					center.AddEllipse(ps1[0].X - grapSize/2,ps1[0].Y - grapSize/2,grapSize,grapSize);
				}
			
				RectangleF pointrect = new RectangleF(point.X -1,point.Y -1,2,2);

				RectangleF selectRect1 = this.selectpath.GetBounds();
				bool empty = selectRect1.Width == 0 || selectRect1.Height == 0;//.IsEmpty;
				
				using(Pen pen = new Pen(Color.Blue,PenWidth),pen1 = new Pen(Color.Blue,PenWidth + 3))
				{
					bool isconnect = this.svgDocument.SelectCollection.Count == 1 && this.svgDocument.SelectCollection[0] is SVG.BasicShapes.SVGConnectionElement;
					bool scale = (this.transformType & TransformBehavior.Scale) == TransformBehavior.Scale && !isconnect;
					bool translate = (this.transformType & TransformBehavior.Translate) == TransformBehavior.Translate&& !isconnect;
					bool skew = (this.transformType & TransformBehavior.Skew) == TransformBehavior.Skew&& !isconnect;
					bool rotate = (this.transformType & TransformBehavior.Rotate) == TransformBehavior.Rotate&& !isconnect;
					if(operation == Operator.Transform)// || operation == Operator.Scale || operation == Operator.Skew || operation == Operator.Rotate) 
					{
						if((center.IsVisible(point) || center.IsOutlineVisible(point,pen)) && !isconnect && this.showCenterPointGrap)
							mousePoint = Operation.MousePoint.CenterPoint;
							//						else if((label.IsVisible(point) || label.IsOutlineVisible(point,pen)))
							//							mousePoint = Operation.MousePoint.LabelPoint;
						else if(middleleft.Contains(point) && scale)//.IsVisible(point) || middleleft.IsOutlineVisible(point,pen))
							mousePoint = Operation.MousePoint.ScaleMiddleLeft;
						else if(bottommiddle.Contains(point) && scale)//.IsVisible(point) || bottommiddle.IsOutlineVisible(point,pen))
							mousePoint = Operation.MousePoint.ScaleBottomMiddle;
						else if(topmiddle.Contains(point) && scale)//.IsVisible(point) || topmiddle.IsOutlineVisible(point,pen))
							mousePoint = Operation.MousePoint.ScaleTopMiddle;
						else if(middleright.Contains(point) && scale)//.IsVisible(point) || middleright.IsOutlineVisible(point,pen))
							mousePoint = Operation.MousePoint.ScaleMiddleRight ;
						else if(topleft.Contains(point) && scale)//.IsVisible(point) || topleft.IsOutlineVisible(point,pen))
							mousePoint = Operation.MousePoint.ScaleTopLeft;
						
						else if(topright.Contains(point) && scale)//.IsVisible(point) || topright.IsOutlineVisible(point,pen))
							mousePoint = Operation.MousePoint.ScaleTopRight;
						
						else if(bottomleft.Contains(point) && scale)//.IsVisible(point) || bottomleft.IsOutlineVisible(point,pen))
							mousePoint = Operation.MousePoint.ScaleBottomLeft;
						
						else if(bottomright.Contains(point) && scale)//.IsVisible(point) || bottomright.IsOutlineVisible(point,pen))
							mousePoint = Operation.MousePoint.ScaleBottomRight;		
						else if((left.IsVisible(point) || left.IsOutlineVisible(point,pen1)) &&!empty && skew)
							return Operation.MousePoint.SkewYLeft;
						else if((right.IsVisible(point) || right.IsOutlineVisible(point,pen1)) && !empty&& skew)
							mousePoint = Operation.MousePoint.SkewYRight;	
						else if((top.IsVisible(point) || top.IsOutlineVisible(point,pen1)) && !empty&& skew)
							mousePoint = Operation.MousePoint.SkewXTop;	
						else if((bottom.IsVisible(point) || bottom.IsOutlineVisible(point,pen1)) &&!empty&& skew)
							mousePoint = Operation.MousePoint.SkewXBottom;	
						else if((rotate1.IsVisible(point) || rotate1.IsOutlineVisible(point,pen)) && rotate)
						{
							rotateindex = 6;
							mousePoint = Operation.MousePoint.Rotate;
						}

						else if(translate)
						{
							path.Transform(this.selectMatrix);
							if(InPath(path, point))
								mousePoint = Operation.MousePoint.Translate;
						}
					}
				}
			}
			#endregion

            //当存在分叉连接线，不能进行旋转和扭曲
            if (this.hasConnectionBranch && (mousePoint.ToString().StartsWith("Rotate") || mousePoint.ToString().StartsWith("Skew")))
                mousePoint = Operation.MousePoint.None;

            //LabelPoint
            if (this.NeedDrawLabelIndicator)
            {
                PointF pos = this.GetLabelIndicatorPos();
                rect1 = new RectangleF(pos.X - r, pos.Y - r, grapSize, grapSize);
                if (rect1.Contains(point))
                    return Operation.MousePoint.LabelPoint;
            }
			ps = null;
			return mousePoint;
		}
		#endregion

        #region ..GetLabelIndicatorPos
        internal PointF GetLabelIndicatorPos()
        {
            PointF[] ps = new PointF[] { (this.svgDocument.SelectCollection[0] as SVGTransformableElement).SVGRenderer.LabelCenterPoint };
            using (Matrix matrix = this.CoordTransform.Clone())
            {
                matrix.TransformPoints(ps);
                return ps[0];
            }
        }
        #endregion

        #region ..UpdateSelectInfo
        internal void UpdateSelectInfo(SVG.SVGElementCollection list)
        {
            if (this.selectpath == null)
                this.selectpath = new GraphicsPath();
            if (!this.selectChanged && !this.updateinfo)
                return;
            this.hasConnectionBranch = false;
            lock (this)
            {
                if (list == null)
                    return;
                if (list.Count == 1)
                {
                    this.hasConnectionBranch = list[0] is SVG.BasicShapes.SVGBranchElement && (list[0] as SVG.BasicShapes.SVGBranchElement).ChildElements.Count > 0;
                    if (list[0] is SVG.Interface.ISVGPathable && list[0] != this.SVGDocument.RootElement)
                    {
                        if (list[0] is SVG.Text.SVGTextElement)
                        {
                            if (!((SVG.Text.SVGTextContentElement)list[0]).Render)
                                return;
                        }
                        if ((list[0] as SVGElement).ParentNode == null)
                        {
                            list.Clear();
                            return;
                        }
                        GraphicsPath path1 = ((SVG.Interface.ISVGPathable)list[0]).GPath;
                        this.selectpath.Reset();
                        if (path1 != null && path1.PointCount > 1)
                        {
                            
                            this.selectpath.AddPath(path1, false);
                            this.selectpath.FillMode = FillMode.Winding;
                            this.oriMatrix.Reset();
                            this.oriMatrix.Multiply(((SVG.SVGTransformableElement)list[0]).TotalTransform);
                            //using (Matrix matrix = this.CoordTransform)
                            //{
                            //    matrix.Invert();
                            //    this.oriMatrix.Multiply(matrix, MatrixOrder.Append);
                            //}
                        }
                    }

                }
                else
                {
                    using (Matrix matrix2 = new Matrix())
                    {
                        matrix2.Multiply(this.oriMatrix);
                        matrix2.Invert();
                        this.selectpath.Reset();
                        SVG.SVGElement root = this.SVGDocument.RootElement as SVG.SVGElement;
                        using (GraphicsPath path = new GraphicsPath())
                        {
                            using (Matrix matrix = new Matrix())
                            {
                                for (int i = 0; i < list.Count; i++)
                                {
                                    SVG.SVGElement element = list[i] as SVGElement;
                                    this.hasConnectionBranch = this.hasConnectionBranch || (element is SVG.BasicShapes.SVGBranchElement && (element as SVG.BasicShapes.SVGBranchElement).ChildElements.Count > 0);
                                    if (element is SVG.Interface.ISVGPathable && element != root)
                                    {
                                        SVG.Interface.ISVGPathable childRender = element as SVG.Interface.ISVGPathable;
                                        if ((childRender as SVG.Interface.ISVGPathable).GPath == null || (childRender as SVG.Interface.ISVGPathable).GPath.PointCount <= 1)
                                            continue;
                                        if (childRender is SVG.Text.SVGTextContentElement)
                                        {
                                            if (!((SVG.Text.SVGTextContentElement)element).Render)
                                                continue;
                                        }
                                        if (element.ParentNode == null)
                                        {
                                            list.Remove(element);
                                            i--;
                                            continue;
                                        }

                                        path.Reset();
                                        matrix.Reset();

                                        path.AddPath((childRender as SVG.Interface.ISVGPathable).GPath, false);
                                        matrix.Multiply(((SVG.SVGTransformableElement)element).TotalTransform, MatrixOrder.Append);
                                        matrix.Multiply(matrix2, MatrixOrder.Append);
                                        path.Flatten();
                                        path.Transform(matrix);

                                        if (path.PointCount > 1)
                                        {
                                            this.selectpath.StartFigure();
                                            this.selectpath.AddPath(path, false);
                                        }
                                    }
                                }

                            }
                        }

                    }
                }
                this.selectpath.Flatten();
                this.selectpath.FillMode = FillMode.Winding;
                this.selectMatrix.Reset();
                this.selectMatrix.Multiply(this.CoordTransform);
                this.selectMatrix.Multiply(this.oriMatrix);

                if (this.selectChanged)
                {
                    RectangleF rect = this.selectpath.GetBounds();
                    this.oldselectPoint = new PointF((rect.X + rect.Width / 2f), (rect.Y + rect.Height / 2f));
                }
                this.selectpath.FillMode = FillMode.Winding;
                this.updateinfo = false;
                this.selectChanged = false;
            }
        }

		internal void UpdateSelectInfo()
		{
			this.UpdateSelectInfo(this.svgDocument.SelectCollection);			
		}
		#endregion

		#region ..改变选择
		void ChangeSelect(object sender,CollectionChangedEventArgs e)
		{
			this.oriMatrix.Reset();
			this.selectChanged = true;
			this.onlyInvalidateSelection = true;
			if(!this.inProcess)
			{
				this.InvalidateSelection();
				//this.InvalidateElements(new SVGElementCollection(e.ChangeElements));
                this.UpdateSelectInfo();
                this.InvalidateSelection();
			}
			try
			{
				if(this.propertyGrid != null)
				{
					if(this.svgDocument.SelectCollection.Count > 0)
					{
						SVG.SVGElement[] elements = new SVG.SVGElement[this.SVGDocument.SelectCollection.Count];
						this.SVGDocument.SelectCollection.CopyTo(elements,0);
						this.propertyGrid.SelectedObjects = elements;
					}
					else 
						this.propertyGrid.SelectedObjects = null;
				}
			}
			catch(System.Exception e1)
			{
				System.Diagnostics.Debug.Assert(true,e1.Message);
			}
			finally
			{
				this.OnSelectionChange();
			}
		}
		#endregion

		#region ..InvalidateSelection
		/// <summary>
		/// 刷新选区
		/// </summary>
		internal void InvalidateSelection()
		{
			if(this.svgDocument.SelectCollection.Count == 1 && this.svgDocument.SelectCollection[0] is SVG.Text.SVGTextContentElement && !(this.svgDocument.SelectCollection[0] as SVG.Text.SVGTextContentElement).Render)
			{
				SVG.Text.SVGTextElement text = this.svgDocument.SelectCollection[0] as SVG.Text.SVGTextElement;
                using(System.Drawing.Drawing2D.GraphicsPath path = new GraphicsPath())
				{
					GraphicsPath path1 = (text as SVG.Interface.ISVGPathable).GPath;
					if(path1 != null && path1.PointCount > 2)
					{
						path.AddPath(path1,false);
						RectangleF rect = path.GetBounds();
						path.Reset();
						int r = grapSize;
						path.AddRectangle(new RectangleF(rect.X -r,rect.Y - r,rect.Width + 3 * r,rect.Height + 3 * r));
                        path.Transform(this.GetTotalTransformForElement(text));//.TotalTransform);
						using(Region rg = new Region(path))
						{
							this.Invalidate(rg);
						}
					}
				}
				return;
			}
			if(this.selectpath.PointCount < 2)
				return;
			this.Invalidate(new Rectangle((int)this.CenterPoint.X - CenterGrap,(int)this.CenterPoint .Y - CenterGrap,2 * CenterGrap,2 * CenterGrap));
			using(GraphicsPath path = (GraphicsPath)this.selectpath.Clone())
			{
				if(path.PointCount <=1)
					return;
				RectangleF rect = path.GetBounds();
				//检测最小边界
				float minX = 0;
				float minY = 0;
				PointF[] ps1 = new PointF[]{new PointF(0,0),new PointF(0,MinGrap),new PointF(MinGrap,0)};
				this.selectMatrix.TransformPoints(ps1);
				minX = (float)Math.Sqrt(Math.Pow(ps1[2].X - ps1[0].X,2) + Math.Pow(ps1[2].Y - ps1[0].Y,2));
				minX = (float)(MinGrap * MinGrap) / minX;
				minY = (float)Math.Sqrt(Math.Pow(ps1[1].Y - ps1[0].Y,2) + Math.Pow(ps1[1].X - ps1[0].X,2));
				minY = (float)(MinGrap * MinGrap) / minY;
				ps1 = null;
				if(rect.Width < minX)
				{
					rect.X = rect.X + rect.Width / 2 - minX/ 2;
					rect.Width = minX;
				}
				if(rect.Height < minY)
				{
					rect.Y = rect.Y + rect.Height / 2 - minY/ 2;
					rect.Height = minY;
				}
				path.Reset();
				int r = grapSize /2;
				path.AddRectangle(new RectangleF(rect.X - r * 4,rect.Y - r*4,rect.Width +grapSize*4,rect.Height + grapSize*4));
				path.Transform(this.selectMatrix);
                //using(Region rg = new Region(path))
                //    this.Invalidate(rg);
                //path.Reset();
				if(OperatorHelper.IsTransformOperator(this.currentOperator))
				{
					PointF[] ps = (PointF[])this.boundPoints.Clone();
                    if (ps.Length != 0)
                    {
                        this.selectMatrix.TransformPoints(ps);

                        for (int i = 0; i < ps.Length; i++)//each(Point point in ps)
                        {
                            PointF point = ps[i];
                            path.StartFigure();
                            path.AddRectangle(new RectangleF(point.X - grapSize, point.Y - grapSize, 2 * grapSize, 2 * grapSize));
                        }
                        ps = null;
                    }
				}
				if(path.PointCount > 1)
				{
                    path.FillMode = FillMode.Winding;
					using(Region rg = new Region(path))
						this.Invalidate(rg);
				}
			}
			this.InvalidateSelectedLabel();
			this.InvalidateSelectedMarker();
		}
		#endregion

		#region ..SetNewCenterPoint
		internal void SetNewCenterPoint(PointF p)
		{
			this.oldselectPoint = p;
		}
		#endregion

		#region ..变换选择
		internal void TransformSelect(Matrix transform,MatrixOrder order)
		{
			PointF[] ps = new PointF[]{this.oldselectPoint};
			this.oriMatrix.Multiply(transform,order);
			this.selectMatrix = this.CoordTransform.Clone();
			this.selectMatrix.Multiply(this.oriMatrix);
			this.InvalidateSelection();
		}
		#endregion 

		#region ..InvalidateMarker
		void InvalidateMarkerOfElement(Control control, SVG.SVGTransformableElement render,Matrix matrix,MatrixOrder order)
		{
            //在Load 过程中，不刷新
            if (!this.svgDocument.IsActive)
                return;
            if (render == null)
				return;
            this.InvalidateMarker(control, render.MarkerEnd, matrix, order);
            this.InvalidateMarker(control,render.MarkerStart, matrix, order);
		}

		void InvalidateSelectedMarker()
		{
            //在Load 过程中，不刷新
            if (!this.svgDocument.IsActive)
                return;
			SVG.SVGElementCollection list = this.svgDocument.SelectCollection as SVG.SVGElementCollection;
			if(list.Count > 0)
			{
				for(int i = 0;i<list.Count;i++)
				{
					this.InvalidateMarkerOfElement(this,list[i] as SVG.SVGTransformableElement,null,MatrixOrder.Append);
				}
			}
		}

        void InvalidateMarker(SVG.ClipAndMask.SVGMarkerElement marker, Matrix matrix, MatrixOrder order)
        {
            InvalidateMarker(this, marker, matrix, order);
        }

		void InvalidateMarker(Control control, SVG.ClipAndMask.SVGMarkerElement marker,Matrix matrix,MatrixOrder order)
		{
            //在Load 过程中，不刷新
            if (!this.svgDocument.IsActive)
                return;
			if(marker == null)
				return;
			using(GraphicsPath path = marker.MarkerPath.Clone() as GraphicsPath)
			{
				RectangleF rect = path.GetBounds();
				path.Reset();
				rect.X --;
				rect.Y --;
				rect.Width += 2;
				rect.Height += 2;
				path.AddRectangle(rect);
				if(matrix != null)
				{
					matrix.Multiply(marker.MarkerTransform,order == MatrixOrder.Append?MatrixOrder.Prepend:MatrixOrder.Append);
					path.Transform(matrix);
				}
				else
				{
					path.Transform(marker.MarkerTransform);
				}
				using(Region rg = new Region(path))
                    control.Invalidate(rg);
			}
		}
		#endregion

		#region ..InvalidateElement
		/// <summary>
		/// 更新单个对象
		/// </summary>
        /// <param name="renderElement"></param>
		internal void InvalidateElement(SVG.Interface.ISVGPathable renderElement)
		{
            //在Load 过程中，不刷新
            if (!this.svgDocument.IsActive || renderElement == null)
                return;
            this.InvalidateElements(new SVGElement[] { renderElement as SVGElement });
		}

        internal void InvalidateElements(SVG.SVGElementCollection elements)
        {
            using (Matrix matrix = this.CoordTransform)
            {
                this.InvalidateElements(elements, matrix);
            }
        }

        internal void InvalidateElements(SVG.SVGElementCollection elements, Matrix matrix)
        {
            this.InvalidateElements(this, elements, matrix);
        }

		/// <summary>
		/// 更新对象集
		/// </summary>
		/// <param name="elements"></param>
        internal void InvalidateElements(Control control, SVG.SVGElementCollection elements, Matrix matrix)
        {
            //在Load 过程中，不刷新
            if (!this.svgDocument.IsActive)
                return;

            using (Region rg = new Region())
            {
                foreach (SVG.SVGElement element in elements)
                {
                    if (element is SVG.Interface.ISVGPathable)
                    {
                        GraphicsPath path1 = ((SVG.Interface.ISVGPathable)element).GPath;
                        if (path1 == null)
                            continue;
                        using (GraphicsPath path2 = (GraphicsPath)path1.Clone())
                        {
                            if (path2.PointCount > 1)
                            {
                                path2.Transform(((SVG.SVGTransformableElement)element).TotalTransform);
                                //path2.Transform(matrix);
                                rg.Union(path2);
                                //Rectangle rect = Rectangle.Inflate(Rectangle.Round(path2.GetBounds()), 10, 10);
                                //control.Invalidate(rect);
                                //rg.Union(rect);
                            }
                        }
                    }

                    if (element is SVG.SVGTransformableElement
                        && (element as SVGTransformableElement).GraphicsPathIncludingTextBlock != null
                        && (element as SVGTransformableElement).GraphicsPathIncludingTextBlock.PointCount > 1)
                    {
                        using (GraphicsPath path2 = (GraphicsPath)((element as SVGTransformableElement).GraphicsPathIncludingTextBlock.Clone()))
                        {
                            if (path2.PointCount > 1)
                            {
                                //path2.Transform(matrix);
                                //Rectangle rect = Rectangle.Inflate(Rectangle.Round(path2.GetBounds()), 10, 10);
                                //control.Invalidate(rect);
                                rg.Union(path2);
                            }
                        }
                    }
                }

                rg.Transform(matrix);
                control.Invalidate(rg);
            }

            foreach (SVG.SVGElement element in elements)
            {
                if (element is SVGTransformableElement)
                {
                    InvalidateLabelText(control, element as SVG.SVGTransformableElement);
                    InvalidateMarkerOfElement(control, element as SVG.SVGTransformableElement, null, MatrixOrder.Append);
                }
            }
            hasInvalidateRegion = true;
        }
		#endregion

		#region ..添加对象
		internal SVG.SVGElement AddElement(SVG.SVGElement element)
		{
			return this.AddElement(element,true,true);
		}

		internal SVG.SVGElement AddElement(SVG.SVGElement element,bool createEvent,bool invokeundo)
		{
			if(element != null)
			{
				bool old = this.svgDocument.AcceptNodeChanged;
				this.svgDocument.AcceptNodeChanged = false;
				SVG.SVGElement element1 = (SVG.SVGElement)this.svgDocument.ImportNode(element,true);
				if(element is SVG.Text.SVGTextElement)
					(element1 as SVG.Text.SVGTextElement).Render = (element as SVG.Text.SVGTextElement).Render;
				if(createEvent)
					this.RenderElement(element1);
				
				SVG.SVGElement root = this.svgDocument.CurrentScene;
				//(this.svgDocument.SVGRootElement as SVGTransformableElement).TotalTransform = this.CoordTransform.Clone();
				this.svgDocument.AcceptNodeChanged = old;
				
				#region ..匹配变换
				if(element1 is SVG.SVGTransformableElement)
				{
					using(Matrix tempmatrix = new Matrix())
					{
						if(root is SVG.Interface.ISVGTransformable)
						{
							tempmatrix.Multiply(((SVG.SVGTransformableElement)root).TotalTransform);
						}
						tempmatrix.Invert();
						//this.RoundMatrix(tempmatrix,2);
						if(!tempmatrix.IsIdentity)
						{
							string text = "matrix("+Math.Round(tempmatrix.Elements[0],2).ToString() + " " +Math.Round(tempmatrix.Elements[1],2).ToString() + " " +Math.Round(tempmatrix.Elements[2],2).ToString() + " " +Math.Round(tempmatrix.Elements[3],2).ToString() + " " +Math.Round(tempmatrix.Elements[4],2).ToString() + " " +Math.Round(tempmatrix.Elements[5],2).ToString()+")";
							element1.InternalSetAttribute("transform",text);
						}
					}
				}
				#endregion

				if(root is SVG.Interface.ISVGContainer)
				{
					if(((SVG.Interface.ISVGContainer)root).ValidChild(element1))
					{
                        element1 = root.InternalAppendChild(element1) as SVGElement;
					}
					else
						return null;
				}
				else 
				{
					return null;
				}
				this.svgDocument.AcceptNodeChanged = old;
                if (invokeundo)
                {
                    this.svgDocument.InvokeUndos();
                    this.svgDocument.ChangeSelectElement(element1);
                }
				this.selectChanged = true;
				this.onlyInvalidateSelection = false;
				return element1;
			}
			return null;
		}
		#endregion

		#region ..用当前的画笔和画刷描述对象
		/// <summary>
		/// 用当前的画笔和画刷描述对象
		/// </summary>
		/// <param name="element"></param>
		void RenderElement(SVG.SVGElement element)
		{
			string fill = SVG.ColorHelper.GetColorStringInHex(this.stroke.Color);
			
            if(!element.HasAttribute("stroke"))
			element.InternalSetAttribute("stroke",fill);

			if(this.stroke.DashPattern != null)
			{
				System.Text.StringBuilder sb = new System.Text.StringBuilder(100);
				for(int i = 0;i<this.stroke.DashPattern.Length;i++)
				{
					sb.Append(this.stroke.DashPattern[i].ToString());
					sb.Append(' ');
				}
                if (!element.HasAttribute("stroke-dasharray"))
				    element.InternalSetAttribute("stroke-dasharray",sb.ToString());
				sb = null;
			}

            if (!element.HasAttribute("stroke-width") && this.stroke.Width > 1)
            {
                element.InternalSetAttribute("stroke-width", this.stroke.Width.ToString());
            }

            float opacity = 1;
            if (!element.HasAttribute("stroke-opacity"))
            {
                opacity = (float)Math.Round(Math.Max(0, Math.Min(1, this.stroke.Opacity)), 2);
                if (opacity != 1)
                {
                    element.InternalSetAttribute("stroke-opacity", opacity.ToString());
                }
            }

			fill = SVG.ColorHelper.GetColorStringInHex(this.fill.Color);
			if(element.LocalName != "image" && element.LocalName != "line")
			{
                if(!element.HasAttribute("fill"))
				element.InternalSetAttribute("fill",fill);
				fill = null;
                if (!element.HasAttribute("fill-opacity"))
                {
                    opacity = (float)Math.Round(Math.Max(0, Math.Min(1, this.fill.Opacity)), 2);
                    if (opacity != 1)
                    {
                        element.InternalSetAttribute("fill-opacity", opacity.ToString());
                    }
                }
			}

			if(element is SVG.BasicShapes.SVGLineElement || element is SVG.BasicShapes.SVGPolylineElement || element is SVG.Paths.SVGPathElement || element is SVG.BasicShapes.SVGConnectionElement)
			{
				if(this.startArrow != null)
				{
					string id = "start" + this.startArrow.MarkerElement.GetAttribute("id");
					SVG.ClipAndMask.SVGMarkerElement marker = this.svgDocument.GetReferencedNode(id,new string[]{"marker"}) as SVG.ClipAndMask.SVGMarkerElement;
					if(marker == null && this.startArrow.MarkerElement is SVG.SVGElement)
					{
						marker = this.svgDocument.AddDefsElement(this.startArrow.MarkerElement as SVG.SVGElement) as SVG.ClipAndMask.SVGMarkerElement;
						marker.InternalSetAttribute("id",id);
					}
					element.InternalSetAttribute("marker-start","url(#"+id+")");
				}

				if(this.endArrow != null)
				{
					string id = "end" + this.endArrow.MarkerElement.GetAttribute("id");
					SVG.ClipAndMask.SVGMarkerElement marker = this.svgDocument.GetReferencedNode(id,new string[]{"marker"}) as SVG.ClipAndMask.SVGMarkerElement;
					if(marker == null && this.endArrow.MarkerElement is SVG.SVGElement)
					{
						marker = this.svgDocument.AddDefsElement(this.endArrow.MarkerElement as SVG.SVGElement) as SVG.ClipAndMask.SVGMarkerElement;
						for(int i = 0;i<marker.ChildElements.Count;i++)
						{
							SVG.SVGElement e1 = marker.ChildElements[i] as SVG.SVGElement;
							if(e1 != null)
							{
								string a = e1.GetAttribute("transform");
								a = a + " matrix(-1,0,0,1,0,0)";
								e1.InternalSetAttribute("transform",a);
								a = null;
							}
						}
						marker.InternalSetAttribute("id",id);
					}
					element.InternalSetAttribute("marker-end","url(#"+id+")");
				}
			}

            //文本Style
            if (element is SVG.Text.SVGTextContentElement || element is SVG.Text.SVGTextBlockElement)
            {
                if(!element.HasAttribute("font-family"))
                    element.InternalSetAttribute("font-family", this.textStyle.FontName);
                if (!element.HasAttribute("font-size"))
                    element.InternalSetAttribute("font-size", this.textStyle.Size.ToString());

                if (!element.HasAttribute("font-weight"))
                    element.InternalSetAttribute("font-weight", this.textStyle.Bold ? "bold":"normal");
                if (!element.HasAttribute("font-style"))
                    element.InternalSetAttribute("font-style", this.textStyle.Italic ? "italic":"normal");
                if (!element.HasAttribute("text-decoration"))
                    element.InternalSetAttribute("text-decoration", this.textStyle.Underline ? "underline" : "normal");
            }

            if(!(element is SVG.Text.SVGTextContentElement))
            {
                if (!element.HasAttribute("text-color"))
                    element.InternalSetAttribute("text-color", SVG.ColorHelper.GetColorStringInHex(this.textBlockStyle.TextColor));
                
                if (!element.HasAttribute("text-anchor"))
                    element.InternalSetAttribute("text-anchor", EnumHelper.GetAlignmentSVGString(textBlockStyle.Alignment));
                if (!element.HasAttribute("vertical-align"))
                    element.InternalSetAttribute("vertical-align", EnumHelper.GetVerticalAlignmentString(textBlockStyle.VerticalAlignment));
            }
		}
		#endregion

		#region ..近似变换
		internal Matrix RoundMatrix(Matrix matrix,int number)
		{
			float m1 = (float)Math.Round(matrix.Elements[0],number);
			float m2 = (float)Math.Round(matrix.Elements[1],number);
			float m3 = (float)Math.Round(matrix.Elements[2],number);
			float m4 = (float)Math.Round(matrix.Elements[3],number);
			float m5 = (float)Math.Round(matrix.Elements[4],number);
			float m6 = (float)Math.Round(matrix.Elements[5],number);
			matrix.Reset();
			matrix.Multiply(new Matrix(m1,m2,m3,m4,m5,m6));
			return matrix;
		}
		#endregion

		#region ..标尺
		private void hRule_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			if(this.svgDocument == null || !this.rule.Visible)
				return;
			using (Matrix matrix = this.GetCurrentMatrix())
			{
				SizeF size = this.viewSize;
				float width = size.Width;
				float height = size.Height;
				PointF[] ps = new PointF[]{new PointF(0,0),new PointF(width,height)};
				matrix.TransformPoints(ps);
				float scale = matrix.Elements[0];
				float digit = this.Approach(scale);
				float step = (float)Math.Round((digit * scale / 10),3);
				using (TextureBrush hBrush = this.CreateHRuleBrush(step))
				{
					if(hBrush != null)
					{
						//						Control c = sender as Control;
						using(StringFormat format = new StringFormat(StringFormat.GenericTypographic))
						{
							using(System.Drawing.Font font = new Font(SystemInformation.MenuFont.Name,7))
							{
								using(Pen pen = new Pen(ControlPaint.DarkDark(this.canvasColor),1))
								{
									pen.Alignment = PenAlignment.Center;

									#region ..创建画笔
									hBrush.ResetTransform();
									hBrush.TranslateTransform(matrix.OffsetX,0); 
									e.Graphics.FillRectangle(hBrush,0,-1,this.Width - 1,this.ruleLength);
									#endregion

									float start = ps[0].X;
									int n = 0;
									while(true)
									{
										if(start + n * step > this.Width)
											break;
										float temp =(float)(PixelToLength(n * step,this.rule.UnitType));
										int a = (int)Math.Round(temp/ (float)scale,0);
										e.Graphics.DrawString(a.ToString(),font,Brushes.Black,new RectangleF(start + n * step + 5,0,200,10),format);
										n += 10;
				
									}
									n = -10;

									while(true)
									{
										if(start + n * step < 0)
											break;
										e.Graphics.DrawString(Math.Round((float)(PixelToLength(n * step,this.rule.UnitType)) / (float)scale,0).ToString(),font,Brushes.Black,new RectangleF(start + n * step + 5,0,200,10),format);
										n -= 10;
				
									}
									e.Graphics.DrawLine(pen,0,this.ruleLength-1,this.Width,ruleLength-1);
									pen.DashStyle = DashStyle.Dot;
								}
							}
						}
					}
				}
				ps = null;
			}
			
		}

		private void vRule_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			using (Matrix matrix = this.GetCurrentMatrix())
			{
				SizeF size = this.viewSize;
				float width = size.Width;
				float height = size.Height;
				PointF[] ps = new PointF[]{new PointF(0,0),new PointF(width,height)};
				matrix.TransformPoints(ps);
				float scale = matrix.Elements[3];
				float digit = this.Approach(scale);
				float step = (float)Math.Round((digit * scale / 10),3);
				using(TextureBrush vBrush = this.CreateVRuleBrush(step))
				{
					if(vBrush != null)
					{
						using(Pen pen = new Pen(Color.Black))
						{
							pen.Alignment = PenAlignment.Center;
							using(StringFormat format = new StringFormat(StringFormat.GenericDefault))
							{
								format.FormatFlags = (System.Drawing.StringFormatFlags.DirectionVertical);
								using(System.Drawing.Font font = new Font(SystemInformation.MenuFont.Name,7))
								{
									vBrush.ResetTransform();
									vBrush.TranslateTransform(0,matrix.OffsetY);
									e.Graphics.FillRectangle(vBrush,-1,0,this.ruleLength ,this.Height - 1);

									float start = ps[0].Y;
									int n = 0;
									while(true)
									{
										if(start + n * step > this.Height)
											break;
										e.Graphics.DrawString(Math.Round((float)(PixelToLength(n * step,this.rule.UnitType)) / (float)scale,0).ToString(),font,Brushes.Black,new RectangleF(0,start + n * step + 5,30,100),format);
										n += 10;
									}
									n = -10;

									while(true)
									{
										if(start + n * step < 0)
											break;
										e.Graphics.DrawString(Math.Round((float)(PixelToLength(n * step,this.rule.UnitType)) / (float)scale,0).ToString(),font,Brushes.Black,new RectangleF(0,start + n * step + 5,30,100),format);
										n -= 10;
				
									}
									e.Graphics.DrawLine(pen,ruleLength-1,0,ruleLength-1,this.Height);
									//						pen.DashStyle = DashStyle.Dot;
									//						if(!this.oldViewPoint.IsEmpty)
									//							e.Graphics.DrawLine(pen,0,this.oldViewPoint.Y,this.vRule.Width,this.oldViewPoint.Y);
								}
							}
						}
					}
				}
				ps = null;
			}
		}

		private void label4_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
            //using(Pen pen = new Pen(this.guide.Color))
            //{
				Graphics g = e.Graphics;
				int r = this.ruleLength;
				int r1 = r / 2 - 1;
				
				g.FillRectangle(Brushes.White,0,0,r,r);
                g.DrawLine(Pens.Black, r - 1, 0, r - 1, r - 1);
                g.DrawLine(Pens.Black, 0, r - 1, r - 1, r - 1);
				g.DrawLine(Pens.Black,r1,0,r1,r-1);
                g.DrawLine(Pens.Black, 0, r1, r - 1, r1);
            //}
		}
		#endregion

		#region ..辅助线
		void DrawGuides(PaintEventArgs e)
		{
			if(this.guide.Visible && (this.vGuides.Count > 0 || this.hGuides.Count > 0))
			{
				using(Pen pen = new Pen(this.guide.Color,1))
				{
					pen.DashStyle = DashStyle.Dash;
					for(int i = 0;i<this.vGuides.Count;i++)
					{
						int a = (int)this.vGuides[i];
						PointF p = this.PointViewToClient(new PointF(a,0));
						e.Graphics.DrawLine(pen,(int)p.X,0,(int)p.X,this.Height);
					}

					for(int i = 0;i<this.hGuides.Count;i++)
					{
						int a = (int)this.hGuides[i];
						PointF p = this.PointViewToClient(new PointF(0,a));
						e.Graphics.DrawLine(pen,0,(int)p.Y,this.Width,(int)p.Y);
					}
				}
			}
		}
		#endregion

		#region ..获取当前变换矩阵
		Matrix GetCurrentMatrix()
		{
			Matrix matrix = new Matrix();
			matrix.Translate(-this.VirtualLeft,-this.VirtualTop);
			matrix.Translate(this.RealMargin.Left,this.RealMargin.Top);
			matrix.Scale(this.scaleRatio,scaleRatio);
			
			#region ..校正视图矩阵
            //if(this.svgDocument.CurrentScene is SVGDom.Interface.ISVGFitToViewBox)
            //{
            //    SVGDom.Interface.ISVGFitToViewBox fitToVBElm= (SVGDom.Interface.ISVGFitToViewBox )this.svgDocument.CurrentScene;
            //    if(!this.svgDocument.OnlyShowCurrentScene)
            //        fitToVBElm = this.svgDocument.SVGRootElement as SVGDom.DocumentStructure.SVGSVGElement;
            //    DataType.SVGPreserveAspectRatio spar = (DataType.SVGPreserveAspectRatio)fitToVBElm.PreserveAspectRatio;
            //    RectangleF elmRect = RectangleF.Empty;
            //    if(!this.svgDocument.OnlyShowCurrentScene)
            //    {
            //        SVGDom.DocumentStructure.SVGSVGElement svg = (SVGDom.DocumentStructure.SVGSVGElement)this.svgDocument.SVGRootElement;
            //        elmRect = new RectangleF(svg.X.Value,svg.Y.Value,svg.Width.Value,svg.Height.Value);
            //    }
            //    else
            //    {
            //        SVGDom.Interface.ISVGFitToViewBox velement = this.svgDocument.CurrentScene as SVGDom.Interface.ISVGFitToViewBox;
            //        elmRect = new RectangleF(velement.ViewBox.X,velement.ViewBox.Y,velement.ViewBox.Width,velement.ViewBox.Height);
            //    }
            //    float[] translateAndScale = spar.FitToViewBox(
            //        ((SVGDom.DataType.SVGRect)fitToVBElm.ViewBox).GDIRect,
            //        elmRect
            //        );
            //    matrix.Translate(translateAndScale[0], translateAndScale[1]);
            //    matrix.Scale(translateAndScale[2], translateAndScale[3]);
            //    matrix.Translate(translateAndScale[0], translateAndScale[1]);
            //    matrix.Scale(translateAndScale[2], translateAndScale[3]);
            //    translateAndScale = null;
            //}
			#endregion

			//			matrix.Translate(this.margin.Width,this.margin.Height,MatrixOrder.Append);
			return matrix;
		}
		#endregion

		#region ..确定标尺画笔
		TextureBrush CreateVRuleBrush(float step)
		{
			int h = (int)Math.Round(10 * step,0);
			TextureBrush vBrush = null;
			#region ..创建垂直标尺画笔
			if(h > 0)
			{
				using(Bitmap bmp = new Bitmap(this.ruleLength ,h))
				{
					using(Pen pen = new Pen(Color.Black))
					{
						using(Graphics g1 = Graphics.FromImage(bmp))
						{
							g1.Clear(Color.White);
							for(int i = 0;i<= 10;i++)
							{
								if(i % 10 == 0)
								{
									g1.DrawLine(pen,0,i * step,ruleLength,i * step);
								}
								else if(i % 2 == 0)
									g1.DrawLine(pen,ruleLength - 6,i * step,ruleLength,i * step);
								else
									g1.DrawLine(pen,ruleLength - 4,i * step,ruleLength,i * step);
							}
					
							vBrush = new TextureBrush(bmp);
						}
					}
				}
			}
			#endregion

			return vBrush;
		}

		TextureBrush CreateHRuleBrush(float step)
		{
			TextureBrush hBrush = null;
			#region ..创建水平标尺画笔
			int h = (int)Math.Round(10 * step,0);
			if(h > 0)
			{
				using(Bitmap bmp = new Bitmap(h,ruleLength))
				{
					using(Pen pen = new Pen(Color.Black))
					{
						using(Graphics g1 = Graphics.FromImage(bmp))
						{
							g1.Clear(Color.White);
							for(int i = 0;i<= 10;i++)
							{
								if(i % 10 == 0)
								{
									g1.DrawLine(pen,i * step,0,i * step,ruleLength);
								}
								else if(i % 2 == 0)
									g1.DrawLine(pen,i * step,ruleLength - 6,i * step,ruleLength);
								else
									g1.DrawLine(pen,i * step,ruleLength - 4,i * step,ruleLength);
							}
				
							hBrush = new TextureBrush(bmp);
						}
					}
				}
			}
			#endregion
			return hBrush;
		}
		#endregion

		#region ..根据指定的比例逼近适当的单位长度
		float Approach(float scale)
		{
			float num = this.unitStep / scale;
			int num1 = (int)num;

			float digit = (int)Math.Log10(num);

			float temp = (float)((num) / Math.Pow(10,digit));

			float temp1 = 1;
			if(temp >= 2 && temp < 5)
				temp1 = 2;
			else if(temp >= 5 && temp < 10)
				temp1 = 5;
			else if(temp == 10)
				temp1 = 10;

			float length = unitStep / 10f;

			temp1 = (float)(temp1 * Math.Pow(length,(int)digit));
			float temp2 = LengthToPixel(1f,this.rule.UnitType);
			if(temp1 < temp2)
				temp1 = temp2;
			
			return (int)temp1;
		}
		#endregion

		#region ..向外部公开的属性和方法
		#region ..事件
        /// <summary>
        /// 当前绘制的场景发生变化时发生
        /// </summary>
        [Category("VectorControl事件"), Description("当前绘制的场景发生变化时发生")]
        public event EventHandler SceneChanged; 

        /// <summary>
        /// 当图元被拖拽进入控件并加入到Dom树时触发
        /// </summary>
        [Category("VectorControl事件"), Description("当图元被拖拽进入控件并加入到Dom树时触发")]
        public event ElementDroppedEventHandler ElementDropped;

        /// <summary>
        /// 绘制连接点时触发
        /// </summary>
        [Category("VectorControl事件"), Description("绘制连接点时触发")]
        public event PaintConnectablePointEventHandler PaintConnectablePoint;

		/// <summary>
		/// 当文档的选区发生时触发此事件
		/// </summary>
		[Category("VectorControl事件"),Description("当选区发生改变时触发")]
		public event EventHandler SelectionChanged;

		/// <summary>
		/// 当缩放比例变化时发生
		/// </summary>
        [Category("VectorControl事件"), Description("当视图缩放比例变化时触发")]
		public event EventHandler ScaleChanged;

		/// <summary>
		/// 当操作方式发生变化时发生
		/// </summary>
        [Category("VectorControl事件"), Description("当操作方式发生变化时触发")]
		public event EventHandler OperatorChanged;

		/// <summary>
        /// 当控件内对象被鼠标单击或双击时触发
		/// </summary>
		[Category("VectorControl事件"),Description("当控件内对象被鼠标单击或双击时触发")]
		public event ElementClickEventHandler ElementClick;

        /// <summary>
        /// 当试图建立连接线时触发
        /// </summary>
        [Category("VectorControl事件"), Description("当试图建立连接线时触发")]
        public event ElementConnectEventHandler ElementConnecting;
		#endregion

		#region ..公共属性
        bool autoCreateBridgeWhenConnectorCross = false;
        /// <summary>
        /// 获取或设置一个值，指示当连接线交叉时是否桥接
        /// </summary>
        [Browsable(true), Category("显示"), Description("获取或设置一个值，指示当连接线交叉时是否桥接")]
        internal bool AutoCreateBridgeWhenConnectorCross
        {
            set
            {
                if(this.autoCreateBridgeWhenConnectorCross != value)
                    this.Invalidate();
                this.autoCreateBridgeWhenConnectorCross = value;
            }
            get{
                return this.autoCreateBridgeWhenConnectorCross;
            }
        }

        /// <summary>
        /// 获取当前选区
        /// </summary>
        [Browsable(false)]
        public SVGElementCollection Selection
        {
            get
            {
                return this.svgDocument.SelectCollection.Clone();
            }
        }

        /// <summary>
        /// 判断当前选区中是否有对象被锁住
        /// </summary>
        [Browsable(false)]
        public bool HasLockedElementsInSelection
        {
            get
            {
                return this.svgDocument.SelectCollection.ContainsLockedElement;
            }
        } 

        /// <summary>
		/// 获取或设置控件的保护类型
		/// </summary>
		[Browsable(true),Category("环境"),Description("获取或设置控件的保护类型")]
		public ProtectType ProtectType
		{
			get
			{
				return this.protectType;
			}
			set
			{
				this.protectType = value;
			}
		}

        /// <summary>
        /// 获取或设置一个值，指示当滚动鼠标滚轮时是否缩放视图
        /// </summary>
		[Browsable(true),Category("环境"),Description("鼠标滚轮是否进行缩放")]
		public bool ZoomWhenMouseWheel
		{
			get
			{
				return this.zoomWhenWheel;
			}
			set
			{
				this.zoomWhenWheel = value;
			}
		}

		/// <summary>
		/// 获取对应的缩略视图(鹰眼导航)
		/// </summary>
		[Browsable(false)]
		public ThumbnailView ThumbnailView
		{
			get
			{
				//check the disabled feature
				if((this.disabledFeatures & DisabledFeatures.ThumbView) == DisabledFeatures.ThumbView)
					return null;
				if(this.thumbnailView == null)
					this.thumbnailView = new ThumbnailView(this);
                this.thumbnailView.BackColor = this.canvasColor;
				return this.thumbnailView;
			}
		}

		/// <summary>
		/// 连接线是否可以中断
		/// </summary>
	//	[Browsable(true)]
		internal bool CanBrokenConnector
		{
			set
			{
				this.canBrokenConnector = value;
			}
			get
			{
				return this.canBrokenConnector;
			}
		}

		/// <summary>
		/// 是否通过点击图元创建连接线
		/// 默认情况下，VectorControl通过连接点创建连接线，如果此属性为真，则通过点击图元创建连接线
		/// </summary>
	//	[Browsable(true)]
		internal bool CreateConnectorWithShape
		{
			set
			{
				this.createConnectorWithShape = value;
			}
			get
			{
				return this.createConnectorWithShape;
			}
		}

        /// <summary>
        /// 获取或设置一个值，指示在编辑环境中，对象的辅助对齐方式
        /// </summary>
        [Browsable(false)]
        public VisualAlignment VisualAlignment { set; get; }

        /// <summary>
        /// 获取或设置当前场景，场景如果设置，则VectorControl只显示当前场景的内容，所有的操作（包括添加、删除)也只针对当前场景
        /// </summary>
        [Browsable(false)]
        public SVGTransformableElement CurrentScene
        {
            set
            {
                if (value != null)
                    this.svgDocument.CurrentScene = value;
                else
                    this.svgDocument.CurrentScene = null;
            }
            get
            {
                SVGTransformableElement element = this.svgDocument.CurrentScene as SVGTransformableElement;
                if (element != null)
                    return element;
                return null;
            }
        }

		/// <summary>
		/// 获取或设置当前绘制连接线的类型
		/// </summary>
		[Browsable(true),Category("环境"),Description("获取或设置当前绘制连接线的类型")]
        public SVG.ConnectionType ConnectionType
		{
			set
			{
				this._connectType = value;
			}
			get
			{
				return this._connectType;
			}
		}

		/// <summary>
		/// 获取获指定画布颜色
		/// </summary>
		[Browsable(true),Category("环境"),Description("获取或设置画布的颜色")]
		public Color CanvasColor
		{
			set
			{
				if(this.canvasColor != value)
				{
					this.canvasColor = value;
					this.Invalidate();
				}
			}
			get
			{
				return this.canvasColor;
			}
		}

		/// <summary>
		/// 获取获指定填充画布的背景图片的路径
		/// </summary>
		[Browsable(true),Category("环境"),Description("获取获指定填充画布的背景图片"),Editor("Design.ImageEditor",typeof(UITypeEditor))]
		public string CanvasBackgroudImage
		{
			set
			{
				if(this.bkImage  != value)
				{
					this.bkImage = value;
					try
					{
                        this.CanvasBackground = new Bitmap(value);
					}
					catch
                    {
                    }
				}
			}
			get
			{
				return this.bkImage;
			}
		}

        /// <summary>
        /// 获取获指定填充画布的背景图片
        /// </summary>
        [Browsable(false), Category("环境"), Description("获取获指定填充画布的背景图片")]
        public Image CanvasBackground
        {
            set
            {
                if (this.canvasImage != value)
                {
                    if (this.canvasImage != null)
                        this.canvasImage.Dispose();
                    this.canvasImage = null;
                    this.canvasImage = value;
                    this.Invalidate();
                }
            }
            get
            {
                return this.canvasImage;
            }
        }

		/// <summary>
		/// 获取或指定一个值，改值指定是否显示中心点拖动句柄
		/// </summary>
		[Browsable(true),Category("环境"),Description("获取或指定一个值，改值指定是否显示中心点拖动句柄")]
		public bool ShowCenterPointGrap
		{
			set
			{
				if(this.showCenterPointGrap != value)
				{
					this.showCenterPointGrap = value;
					this.InvalidateSelection();
				}
			}
			get
			{
				return this.showCenterPointGrap;
			}
		}

        bool showSelectionHighlightOutline = true;
        /// <summary>
        /// 获取或指定一个值，改值指定是否高亮显示选区的轮廓线
        /// </summary>
        [Browsable(true), Category("环境"), Description("改值指定是否高亮显示选区的轮廓线")]
        public bool ShowSelectionHighlightOutline
        {
            set
            {
                if (this.showSelectionHighlightOutline != value)
                {
                    this.showSelectionHighlightOutline = value;
                    this.InvalidateSelection();
                }
            }
            get
            {
                return this.showSelectionHighlightOutline;
            }
        }

		/// <summary>
		/// 获取获指定一个值，改值指定是否显示选区边框
		/// </summary>
		[Browsable(true),Category("环境")]
		public bool ShowSelectedBounds
		{
			set
			{
				if(this.showSelectedBounds != value)
				{
					this.showSelectedBounds = value;
					this.InvalidateSelection();
				}
			}
			get
			{
				return this.showSelectedBounds;
			}
		}

		/// <summary>
		/// 获取获指定一个值，改值指定是否显示拖动句柄
		/// </summary>
		[Browsable(true),Category("环境")]
		public bool ShowResizeGrap
		{
			set
			{
				if(this.showResizeGrap != value)
				{
					this.showResizeGrap = value;
					this.InvalidateSelection();
				}
			}
			get
			{
				return this.showResizeGrap;
			}
		}

		/// <summary>
		/// 获取控件所关联的Document对象
		/// </summary>
		[Browsable(false)]
        [ComVisible(false)]
		public SVG.Document.SVGDocument Document
		{
			get
			{

				return this.svgDocument;
			}
            //set
            //{
            //    this.SVGDocument = value;
            //}
		}

		/// <summary>
		/// 获取或设置一个值，该值指示在编辑环境中，是否显示连接点
		/// </summary>
        [Category("环境"), Description("设置一个值，该值指示在编辑环境中，是否显示连接点")]
		public bool ShowConnectablePoint
		{
			set
			{
				if(this.drawconnectPoint != value)
				{
					this.drawconnectPoint = value;
					this.Invalidate();
				}
			}
			get
			{
				return this.drawconnectPoint;
			}
		}

        /// <summary>
        /// 获取或设置一个值，该值指示在编辑环境中，是否需要绘制连接点
        /// </summary>
        [Category("环境"), Description("设置一个值，该值指示在编辑环境中，是否允许建立分支连接线")]
        public bool BranchSupport { set; get; }

		/// <summary>
		/// 获取或设置编辑环境中，编辑视图离控件边界之间的距离
		/// </summary>
		[Category("环境"),Description("指定编辑环境中，编辑视图离控件边界之间的距离")]
		public Margin CanvasMargin
		{
			set
			{
                if (this.margin != value)
				{
                    this.margin = value;
					if(!this.DesignMode)
						this.SetScroll();
				}
				this.Invalidate();
			}
			get
			{
                return this.margin;
			}
		}

		/// <summary>
		/// 获取或设置一个值，该值指定编辑环境中是否显示滚动条
		/// </summary>
		[Category("环境"),Description("指定编辑环境中是否显示滚动条")]
		public bool Scrollable
		{
			set
			{
				this.scrollable = value;
				this.AutoScroll = value;
			}
			get
			{
				return this.AutoScroll;
			}
		}

		/// <summary>
		/// 获取或设置控件允许进行的二维变换操作
		/// 可以是<see cref="TransformBehavior">TransformType</see>的一个或几个的组合
		/// </summary>
		/// <example>
		/// <code>
		/// //让当前控件允许进行平移、缩放和旋转操作
        /// this.vectorControl.TransformBehavior = TransformBehavior.Translate | TransformBehavior.Scale;
		/// </code>
		/// </example>
		[Browsable(false),Category("环境"),Description("指定控件可以进行的二维变换操作")]
		public TransformBehavior TransformBehavior
		{
			set
			{
				this.transformType = value;
			}
			get
			{
				return this.transformType;
			}
		}

		/// <summary>
		/// 获取或设置一个值，该值指示是否以轮廓线的方式显示当前绘制内容
		/// </summary>
		[Category("显示"),Description("指定是否以轮廓线的方式显示当前绘制内容")]
		public bool OutLine
		{
			set
			{
				if(this.outline != value)
				{
					//clear the cache
					if(this.svgDocument != null)
						this.svgDocument.ClearCache();
					this.outline = value;
					this.Invalidate();
				}
			}
			get
			{
				return this.outline;
			}
		}

		/// <summary>
		/// 获取当前绘制内容的边界
		/// </summary>
		[Browsable(false)]
		public RectangleF ContentBounds
		{
			get
			{
                return this.contentBounds;
			}
		}

		
		/// <summary>
		/// 获取当前选中对象的数目
		/// </summary>
		[Browsable(false)]
        internal int NumberOfSelectedElements
		{
			get
			{
				return this.svgDocument.SelectCollection.Count;
			}
		}

		/// <summary>
		/// 判断当前文档是否是有效的SVG文档
		/// </summary>
		[Browsable(false)]
        internal bool IsValidSVGDocument
		{
			get
			{
				return this.svgDocument.IsValid;
			}
		}

		
		/// <summary>
		/// 获取或设置文档尺寸
		/// </summary>
		[Browsable(false)]
		public Size DocumentSize
		{
			set
			{
				Size size = Size.Round(this.viewSize);
				if(size != value)
				{
					SVG.SVGElement root = this.svgDocument.RootElement as SVG.SVGElement;
					if(root != null)
					{
						bool old = this.svgDocument.AcceptNodeChanged;
						this.svgDocument.AcceptNodeChanged = true;
						root.InternalSetAttribute("width",value.Width.ToString());
						root.InternalSetAttribute("height",value.Height.ToString());
						this.svgDocument.InvokeUndos();
						this.svgDocument.AcceptNodeChanged = old;
						this.viewSize = value;
                        this.SetScroll();
						this.Invalidate();
					}	
				}
			}
			get
			{
				return Size.Round(this.viewSize);
			}
		}

		/// <summary>
		/// 获取或设置开始箭头
		/// </summary>
		[Browsable(false)]
		public Arrow StartArrow
		{
			set
			{
				this.startArrow = value;
				this.UpdateStartArrow();
			}
			get
			{
				return this.startArrow;// = value;
			}
		}

		/// <summary>
		/// 获取或设置结束箭头
		/// </summary>
		[Browsable(false)]
		public Arrow EndArrow
		{
			set
			{
				this.endArrow = value;
				this.UpdateEndArrow();
			}
			get
			{
				return this.endArrow ;//= value;
			}
		}

		/// <summary>
		/// 获取或设置图像的呈现质量
		/// </summary>
		[Category("显示"),Description("获取或设置图像的呈现质量")]
		public System.Drawing.Drawing2D.SmoothingMode SmoothingMode
		{
			set
			{
				if(System.Drawing.Drawing2D.SmoothingMode.Invalid == value)
					MessageBox.Show("参数不能为无效模式");
				if(this.smoothingmode != value)
				{
					this.smoothingmode = value;
					this.Invalidate();
				}
			}
			get
			{
				return this.smoothingmode;
			}
		}

		/// <summary>
		/// 获取或设置文本的呈现质量
		/// </summary>
		[Category("显示"),Description("获取或设置文本的呈现质量")]
		public System.Drawing.Text.TextRenderingHint TextRenderingHint
		{
			set
			{
				if(this.textrenderingHint != value)
				{
					this.textrenderingHint = value;
					this.Invalidate();
				}
			}
			get
			{
				return this.textrenderingHint;
			}
		}

		/// <summary>
		/// 设置用于编辑选中对象属性的属性面板
		/// </summary>
		[Browsable(false)]
		public System.Windows.Forms.PropertyGrid PropertyGrid
		{
			set
			{
				if(this.propertyGrid != null)
				{
					this.propertyGrid.PropertyValueChanged -= new PropertyValueChangedEventHandler(propertyGrid_PropertyValueChanged);
					this.propertyGrid.SelectedObjects = null;
				}
				this.propertyGrid = value;
				if(this.propertyGrid != null)
				{
					//					this.propertyGrid.PropertySort = PropertySort.Categorized;
					this.propertyGrid.PropertyValueChanged += new PropertyValueChangedEventHandler(propertyGrid_PropertyValueChanged);
				}
			}
		}

		/// <summary>
		/// 设置当前需要编辑的SVG文档的路径
		/// </summary>
		[Browsable(false)]
		internal string FilePath
		{
			set
			{
				if(this._filepath != value)
				{
					this._filepath = value;
					if(this.currentOperation != null)
						this.currentOperation.Reset();
					try
					{
						SVG.Document.SVGDocument doc = SVG.Document.SvgDocumentFactory.CreateDocumentFromFile(value);
						this.SVGDocument = doc;
					}
					catch(System.Exception e)
					{
						MessageBox.Show(e.Message);
					}
                    this.firstLoad = true;
				}
			}
		}

		/// <summary>
		/// 判断当前是否有选择对象
		/// </summary>
		[Browsable(false)]
		public bool HasElementsSelected
		{
			get
			{
				return this.svgDocument.SelectCollection.Count > 0;
			}
		}

		/// <summary>
		/// 在文档第一次导入时，是否自动适应窗口
		/// </summary>
		[Category("显示"),Description("在文档第一次导入时，是否自动适应窗口")]
		public bool AutoFitWindowWhenFirstLoading
		{
			set
			{
				this.autoFit = value;
			}
			get
			{
				return this.autoFit;
			}
		}

		/// <summary>
		/// 获取或设置当前的操作方式
		/// </summary>
		[Category("显示"),Description("获取或设置默认操作")]
		public Operator CurrentOperator
		{
			set
			{
				//check the disable feature
				if((value & this.disabledOperator) == value && value != Operator.None )
					return;
				if(this.currentOperator != value)
				{
					this.currentOperator = value;
                    this.Invalidate();
					
					if(this.DesignMode)
						return;

					#region ..Bezier曲线
					if(this.currentOperator == Operator.Path)
					{
						if(!(this.currentOperation is Operation.BezierSplineOperation))
						{
							if(this.currentOperation != null)
								this.currentOperation.Dispose();
                            if (this.svgDocument.SelectCollection.Count == 1 && this.svgDocument.SelectCollection[0] is SVG.Interface.ISVGPathSegListElement)
                                this.currentOperation = new Operation.BezierSplineOperation(this, this.svgDocument.SelectCollection[0] as SVG.Interface.ISVGPathSegListElement);
                            else
                                this.currentOperation = new Operation.BezierSplineOperation(this);
						}

						this.DefaultCursor = Forms.Cursors.Path;
					}
						#endregion
					
						#region ..文本操作
					else if(this.currentOperator == Operator.Text)
					{
						if(!(this.currentOperation is Operation.Text.TextOperation))
						{
							if(this.currentOperation != null)
								this.currentOperation.Dispose();
							this.currentOperation = new Operation.Text.TextOperation(this);
						}
	
						this.DefaultCursor = Forms.Cursors.Text;
					}
						#endregion

						#region ..标记文本
					else if(this.currentOperator == Operator.TextBlock)
					{
						if(!(this.currentOperation is Operation.LabelText.LabelTextOperation))
						{
							if(this.currentOperation != null)
								this.currentOperation.Dispose();
							this.currentOperation = new Operation.LabelText.LabelTextOperation(this);
						}
							
						this.DefaultCursor = Forms.Cursors.TextBlock;
					}
						#endregion

						#region ..Poly操作
					else if(OperatorHelper.IsPointsOperation(this.currentOperator))
					{
						if(!(this.currentOperation is Operation.PolyOperation))
						{
							if(this.currentOperation != null)
								this.currentOperation.Dispose();
							this.currentOperation = new Operation.PolyOperation(this);
						}
						if(this.preOp != value)
							((Operation.PolyOperation)this.currentOperation).StartPoly();
						this.preOp = value;
						this.DefaultCursor = Forms.Cursors.PolyDraw;
					}
						#endregion

						#region ..节点编辑
					else if(this.currentOperator == Operator.NodeEdit)
					{
						if(!(this.currentOperation is Operation.NodeEditOperation) || (this.currentOperation as Operation.NodeEditOperation).IsEmpty || this.preOp != value)
						{
							if(this.currentOperation != null)
								this.currentOperation.Dispose();
							this.currentOperation = new Operation.NodeEditOperation(this);
							if(this.svgDocument.SelectCollection.Count == 1 && this.svgDocument.SelectCollection[0] is SVG.SVGTransformableElement)
                                (this.currentOperation as Operation.NodeEditOperation).RenderElement = this.svgDocument.SelectCollection[0] as SVG.SVGTransformableElement;
						}
						this.preOp = value;
						this.DefaultCursor = Forms.Cursors.NodeEdit;
					}
						#endregion
					
						#region ..形状绘制
					else if(OperatorHelper.IsShapeOperator(this.currentOperator))
					{
						if(!(this.currentOperation is Operation.ShapeOperation))
						{
							if(this.currentOperation != null)
								this.currentOperation.Dispose();
							this.currentOperation = new Operation.ShapeOperation(this);
						}
						this.DefaultCursor = Forms.Cursors.Shapes;
					}
						#endregion

						#region ..选择变换
					else if(OperatorHelper.IsSelectOperation(this.currentOperator) || OperatorHelper.IsTransformOperator(this.currentOperator))
					{
						if(!(this.currentOperation is Operation.SelectTransformOperation))
						{
							if(this.currentOperation != null)
								this.currentOperation.Dispose();
							this.currentOperation = new Operation.SelectTransformOperation(this);
						}
						this.DefaultCursor = Forms.Cursors.Select;
					}
						#endregion

						#region ..颜色操作
						//					else if(Operator.IsColorOperatior(this.currentOperator))
						//					{
						//						if(!(this.currentOperation is Operation.ColorOperation))
						//						{
						//							if(this.currentOperation != null)
						//								this.currentOperation.Dispose();
						//							this.currentOperation = new Operation.ColorOperation(this);
						//						}
						//						switch(this.currentOperator)
						//						{
						//							case Operator.InkBottle:
						//								this.DefaultCursor = Cursors.InkBottle;
						//								break;
						//							case Operator.PaintBottle:
						//								this.DefaultCursor = Cursors.PaintBottle;
						//								break;
						//						}
						//					}
						#endregion

						#region ..视图
					else if(OperatorHelper.IsViewOperator(this.currentOperator))
					{
						if(!(this.currentOperation is Operation.ViewOperation))
						{
							if(this.currentOperation != null)
								this.currentOperation.Dispose();
							this.currentOperation = new Operation.ViewOperation(this);
						}
						
						switch(this.currentOperator)
						{
							case Operator.ZoomIn:
								this.DefaultCursor = Forms.Cursors.IncreaseView;
								break;
							case Operator.ZoomOut:
								this.DefaultCursor = Forms.Cursors.DecreaseView;
								break;
							case Operator.Roam:
								this.DefaultCursor = Forms.Cursors.Hand;
								break;
						}
					}
						#endregion

						#region ..吸管

						//					else if(this.currentOperator == Operator.ColorPicker)
						//					{
						//						if(!(this.currentOperation is Operation.ColorOperation))
						//						{
						//							if(this.currentOperation != null)
						//								this.currentOperation.Dispose();
						//							this.currentOperation = new Operation.ColorOperation(this);
						//						}
						//						this.DefaultCursor = Cursors.ColorPicker;
						//					}
						#endregion

						#region ..Connector
					else if(this.currentOperator == Operator.Connection)
					{
						//							if(!(this.currentOperation is Operation.ConnectorOperation))
						//							{
						if(this.currentOperation != null)
							this.currentOperation.Dispose();
						this.currentOperation = new Operation.ConnectorOperation(this);
						//							}
						this.DefaultCursor = Forms.Cursors.Select;
					}
						#endregion

					else
					{
						if(this.currentOperation != null)
							this.currentOperation.Dispose();
						this.currentOperation = null;
					}
					
					this.OnOperatorChanged();
				}
			}
			get
			{
				return this.currentOperator;
			}
		}

		/// <summary>
		/// 获取或设置网格信息
		/// </summary>
		[Category("环境"),Description("获取或设置网格信息")]
		public Grid Grid
		{
			set
			{
				if(this.grid != value)
					this.Invalidate();
				this.grid = value;
			}
			get
			{
				return this.grid;
			}
		}


		/// <summary>
		/// 获取或设置标尺信息
		/// </summary>
		[Category("环境"),Description("是否显示标尺"),ComVisible(false)]
		public bool ShowRule
		{
			set
			{
				if(this.rule.Visible != value)
				{
					this.Invalidate(new Rectangle(0,0,this.Width,this.ruleLength));
					this.Invalidate(new Rectangle(0,0,this.ruleLength,this.Height));

					this.rule.Visible = value;
					this.unitStep  = (int)PixelToLength(100f,this.rule.UnitType);
					unitStep = LengthToPixel(unitStep,this.rule.UnitType);
				}
				this.rule.Visible = value;
			}
			get
			{
                return this.rule.Visible;
			}
		}

		/// <summary>
		/// 获取或设置辅助线信息
		/// </summary>
		[Category("环境"),Description("获取或设置辅助线信息")]
		public Guide Guide
		{
			set
			{
				if(this.guide.Visible != value.Visible || this.guide.Color != value.Color)
				{
					this.Invalidate();
				}
				this.guide = value;
			}
			get
			{
				return this.guide;
			}
		}

		/// <summary>
		/// 获取或设置当前视图缩放比例
		/// </summary>
		[Category("显示"),Description("获取或设置当前视图缩放比例")]
		public float ScaleRatio
		{
			set
			{
				value = (float)Math.Round(Math.Max(0.1f,Math.Min(25,value)),2);
				if(this.scaleRatio != value)
				{
					this.scaleRatio = value;
					this.SetScroll();
					this.Invalidate();
					this.OnScaleChanged();
				}
			}
			get
			{
				return this.scaleRatio;
			}
		}

		/// <summary>
		/// 获取或设置绘制形状边缘的对象
		/// </summary>
		[Browsable(false),Category("绘制"),Description("获取或设置绘制形状边缘的对象")]
		public Stroke Stroke
		{
			set
			{
				this.stroke = value;
				this.UpdateStroke();
			}
			get
			{
				return this.stroke;
			}
		}

		/// <summary>
		/// 获取或设置绘制形状边缘的对象
		/// </summary>
		[Browsable(false)]
		public TextStyle TextStyle
		{
			set
			{
				this.textStyle = value;
				this.UpdateTextStyle();
			}
			get
			{
				return this.textStyle;
			}
		}


		/// <summary>
		/// 获取或设置填充形状内部的对象
		/// </summary>
		[Browsable(false),Category("绘制"),Description("获取或设置填充形状内部的对象")]
		public Fill Fill
		{
			set
			{
				this.fill = value;
				this.UpdateFill();
			}
			get
			{
				return this.fill;
			}
		}

        /// <summary>
        /// 在编辑TextBlock时，获取或设置TextBlock的相关属性
        /// </summary>
        [Browsable(false), Category("绘制"), Description("在编辑TextBlock时，获取或设置TextBlock的相关属性")]
        public TextBlockStyle TextBlockStyle
        {
            set
            {
                if (this.textBlockStyle != value)
                {
                    this.textBlockStyle = value;
                    UpdateTextBlockStyle();
                }
            }
            get
            {
                return this.textBlockStyle;
            }
        }

		/// <summary>
		/// 当构造矩形形状时，获取或设置矩形的圆角角度，以角度表示
		/// </summary>
		[Browsable(false),Category("绘制"),Description("当构造矩形形状时，获取或设置矩形的圆角角度，以角度表示")]
		public int RadiusOfRectangle
		{
			set
			{
				value = (int)Math.Max(0,value);
				this.rectangleAngle = value;
			}
			get
			{
				return this.rectangleAngle;
			}
		}

        /// <summary>
        /// 当绘制星形或者正多边形时，获取或者设置其相关参数
        /// </summary>
        [Browsable(false),Category("绘制"), Description("当绘制星形或者正多边形时，获取或者设置其相关参数")]
        public Star Star { set; get; }

		/// <summary>
		/// 当用形状工作构造图形时，设置形状模板
		/// </summary>
		[Browsable(false)]
        public SVGPathElement TemplateShape
		{
			set
			{
				this.templateShape = value;
			}
		}
		#endregion

		#region ..删除当前选中对象
		/// <summary>
		/// 删除当前选中对象
		/// </summary>
		public void Delete()
		{
			if(this.ExecuteBehaviorPresent(Behavior.Delete))
			{
				if(this.currentOperation != null)
				{
					if(this.currentOperation.ProcessDialogKey(Keys.Delete))
						return;
					this.currentOperation.Reset();
				}
                this.svgDocument.DoAction(delegate
                {
                    this.DeleteSelection();
                }, true);
			}
		}

		/// <summary>
		/// 清除当前所有对象
		/// </summary>
		public void Clear()
		{
			if(this.currentOperation != null)
			{
				if(this.currentOperation.ProcessDialogKey(Keys.Clear))
					return;
				this.currentOperation.Reset();
			}
            var doc = this.svgDocument;
            doc.DoAction(delegate
            {
                SVG.SVGElement root = this.svgDocument.RootElement as SVG.SVGElement;
                if (root is SVG.Interface.ISVGContainer)
                {
                    SVG.SVGElementCollection list = ((SVG.Interface.ISVGContainer)root).ChildElements.Clone();
                    bool old = doc.AcceptNodeChanged;
                    doc.AcceptNodeChanged = true;
                    for (int i = 0; i < list.Count; i++)
                        root.InternalRemoveChild(list[i] as System.Xml.XmlElement);
                    doc.InvokeUndos();
                    doc.AcceptNodeChanged = old;
                }
            }, true);
		}
		#endregion

		#region ..清除辅助线
		/// <summary>
		/// 清除辅助线
		/// </summary>
		public void ClearGuides()
		{
			if(this.vGuides.Count > 0 || this.hGuides.Count > 0)
			{
				this.vGuides.Clear();
				this.hGuides.Clear();
				this.Invalidate();
			}
		}
		#endregion

		#region ..成组
		/// <summary>
		/// 将当前选中的对象组合成组
		/// </summary>
		/// <example>
		/// <code>
		/// if(this.vectorControl.ExecuteBehaviorPresent(Behavior.Group))
		/// {
		///		this.vectorControl.Group();
		/// }
		/// </code>
		/// </example>
        public void Group()
        {
            var doc = this.svgDocument;
            doc.DoAction(delegate
            {
                this.InnerGroup();
            }, true);
        }
		
        void InnerGroup()
        {
			if(this.ExecuteBehaviorPresent(Behavior.Group))
			{
				SVG.SVGElementCollection list = (SVG.SVGElementCollection)this.SVGDocument.SelectCollection;
				if(list.Count < 1)
				{
					System.Windows.Forms.MessageBox.Show(this,"没有选择对象!","",System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Warning);
					return;
				}
				SVG.Document.SVGDocument doc = this.svgDocument;
				bool old = doc.AcceptNodeChanged;
				System.Xml.XmlNode parent = null;
				System.Xml.XmlNode next = null;
				ArrayList list1 = new ArrayList();
				ArrayList list2 = new ArrayList();
				for(int i = 0;i<list.Count;i++)//each(SVGDom.SVGElement element1 in list)
				{
					SVG.SVGElement element1 = list[i] as SVG.SVGElement;
					if(element1 is SVG.Interface.ISVGPathable)
					{
						if(i == 0)
							parent = element1.ParentNode;
						if(i == list.Count - 1)
							next = element1.NextSibling;
						if(element1.ParentNode != parent)
						{
							System.Windows.Forms.MessageBox.Show(this,"不能将不在同一层上的对象组合!","",System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Warning);
							return;
						}
						//throw new Exception("不能将不在同一层上的对象组合！");//,"",MessageBoxButtons.OK,MessageBoxIcon.Warning);
						list1.Add(element1);
						int index = i;
						if(element1.ParentElement is SVG.Interface.ISVGContainer)
							index = (element1.ParentElement as SVG.Interface.ISVGContainer).ChildElements.IndexOf(element1);
						list2.Add(index);
					}
				}

				if(this.currentOperation != null)
					this.currentOperation.Reset();
				SVG.SVGElement group = doc.CreateElement(doc.Prefix,"g",doc.NamespaceURI) as SVG.SVGElement;
				if(parent is SVG.SVGElement)
				{
					doc.AcceptNodeChanged = true;
					//				doc.CreateID((SVGDom.SVGElement)container,(SVGDom.SVGElement)parent);
					if(next != null)
					{
						(parent as SVGElement).InternalInsertBefore(group,next);
					}
					else
					{
						(parent as SVGElement).InternalAppendChild(group);
					}
					SVG.SVGElement[] elements = new SVGElement[list1.Count];
					list1.CopyTo(elements);
					int[] indexs = new int[list2.Count];
					list2.CopyTo(indexs);
					Array.Sort(indexs,elements);
					for(int i = 0;i<elements.Length;i++)//foreach(SVGDom.SVGElement element1 in list1)
					{
						SVG.SVGElement element1 = elements[i];
						element1.ParentElement.InternalRemoveChild(element1);
						group.InternalAppendChild(element1);
						//						group.InternalAppendChild(this.svgDocument.CreateTextNode("\n"));
					}
					//					next = group.NextSibling;
					//					while(next is System.Xml.XmlText)
					//					{
					//						parent.InternalRemoveChild(next);
					//						next = group.NextSibling;
					//					}
					//					next = group.PreviousSibling;
					//					while(next is System.Xml.XmlText)
					//					{
					//						parent.InternalRemoveChild(next);
					//						next = group.PreviousSibling;
					//					}
					//					parent.InternalInsertAfter(this.svgDocument.CreateTextNode("\n"),group);
					//					parent.InternalInsertBefore(this.svgDocument.CreateTextNode("\n"),group);
					//					group.InternalPrependChild(this.svgDocument.CreateTextNode("\n"));
					doc.ChangeSelectElement(group);
					//				doc.InvokeUndos();
				}
				doc.AcceptNodeChanged = old;
				doc.InvokeUndos();
			}
		}
		#endregion

		#region ..解组
		/// <summary>
		/// 如果当前选取存在着组对象，将组对象打散为单个对象
		/// </summary>
		/// <example>
		/// <code>
		/// if(this.vectorControl.ExecuteBehaviorPresent(Behavior.UnGroup))
		/// {
		///		this.vectorControl.UnGroup();
		/// }
		/// </code>
		/// </example>
        public void UnGroup()
        {
            var doc = this.svgDocument;
            doc.DoAction(delegate
            {
                this.InnerUnGroup();
            }, true);
        }

        void InnerUnGroup()
		{
			if(this.ExecuteBehaviorPresent(Behavior.UnGroup))
			{
                if (this.svgDocument.SelectCollection.Count != 1 || !(this.svgDocument.SelectCollection[0] is SVG.DocumentStructure.SVGGElement))
				{
					System.Windows.Forms.MessageBox.Show(this,"不能对当前选择对象执行该操作!","",System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Warning);
					return;
				}
				if(this.currentOperation != null)
					this.currentOperation.Reset();
				SVG.Document.SVGDocument doc = this.svgDocument;
				bool old = doc.AcceptNodeChanged;
				doc.AcceptNodeChanged = true;
				SVG.Interface.ISVGContainer container = this.svgDocument.SelectCollection[0] as SVG.Interface.ISVGContainer;
				SVG.SVGElement parent = (SVG.SVGElement)((SVG.SVGElement)container).ParentElement;
				System.Xml.XmlNode next = parent.NextSibling;
				while(next is System.Xml.XmlText)
				{
                    if(next.ParentNode is SVGElement)
					    (next.ParentNode as SVGElement).InternalRemoveChild(next);
					next = ((SVG.SVGElement)container).NextSibling;
				}
				next = parent.PreviousSibling;
				while(next is System.Xml.XmlText)
				{
                    if (next.ParentNode is SVGElement)
                        (next.ParentNode as SVGElement).InternalRemoveChild(next);
					next = ((SVG.SVGElement)container).PreviousSibling;
				}
				//				parent.ParentNode.InternalInsertAfter(this.svgDocument.CreateTextNode("\n"),parent);
				//				parent.ParentNode.InternalInsertBefore(this.svgDocument.CreateTextNode("\n"),parent);
				next = parent.NextSibling;
				SVG.SVGElementCollection list1 = container.ChildElements.Clone();
				SVG.SVGElementCollection list2 = new SVGElementCollection();
				string transform = string.Empty;
				if(container is SVG.Interface.ISVGTransformable)
					transform = ((SVG.Interface.ISVGTransformable)container).Transform.FinalMatrix.ToString();
				for(int i = 0;i<list1.Count;i++)
				{
					SVG.SVGElement element1 = list1[i] as SVG.SVGElement;
					if(transform.Length > 0 && element1 is SVG.Interface.ISVGTransformable)
					{
						string transform1 = element1.GetAttribute("transform").Trim();
						transform1 = transform + " " + transform1;
						element1.InternalSetAttribute("transform",transform1);
						System.Xml.XmlElement xmlelement = (System.Xml.XmlElement)container;
						foreach(System.Xml.XmlAttribute attri in xmlelement.Attributes)
						{
							if(attri.Name != "id" && attri.Name != "transform" && !element1.HasAttribute(attri.Name,attri.NamespaceURI))
							{
								element1.InternalSetAttribute(attri.LocalName,attri.NamespaceURI,attri.Value);
							}
						}
						((SVG.SVGStyleable)element1).UpdateElement();
						transform1 = null;
					}
					//				element1.StartPos = element1.EndPos = new Base.ElementPos(-1,-1);
					if(next == null)
					{	
						element1 = parent.InternalAppendChild(element1) as SVG.SVGElement;
						if(!(element1.NextSibling is System.Xml.XmlText))
							parent.InternalInsertAfter(this.svgDocument.CreateTextNode("\n"),element1);
					}
					else
					{

						element1 = parent.InternalInsertBefore(element1,next) as SVG.SVGElement;
						if(!(next is System.Xml.XmlText))
							parent.InternalInsertAfter(this.svgDocument.CreateTextNode("\n"),element1);
					}
					if(!list2.Contains(element1))
						list2.Add(element1);
				}
				transform= null;
				next = ((SVG.SVGElement)container).PreviousSibling;
				while(next is System.Xml.XmlText)
				{
					parent.InternalRemoveChild(next);
					next = ((SVG.SVGElement)container).PreviousSibling;
				}
				parent.InternalRemoveChild((SVG.SVGElement)container);
				doc.ChangeSelectElements(list2);
				doc.AcceptNodeChanged = old;
				doc.InvokeUndos();
			}
		}
		#endregion

		#region ..层次操作
		/// <summary>
		/// 更新当前对象在垂直方向上的层次
		/// </summary>
		/// <param name="level">指定调整层次的类别</param>
		/// <example>
		/// <code>
		/// if(this.vectorControl.ExecuteBehaviorPresent(Behavior.AdjustLevel))
		/// {
        ///		this.vectorControl.UpdateElementsLayer(Level.Top);
		/// }
		/// </code>
		/// </example>
		public void UpdateElementsLayer(ElementLayer level)
        {
            this.svgDocument.DoAction(delegate
            {
                this.InnerUpdateLevel(level);
            }, true);
        }

        void InnerUpdateLevel(ElementLayer level)
		{
			if(this.ExecuteBehaviorPresent(Behavior.AdjustLayer))
			{
				if(this.currentOperation != null)
					this.currentOperation.Reset();
				SVG.SVGElementCollection list = this.svgDocument.SelectCollection as SVG.SVGElementCollection;
				SVG.Document.SVGDocument doc = this.svgDocument;
				bool old = doc.AcceptNodeChanged;
				doc.AcceptNodeChanged = true;
				switch(level)
				{
						#region ..置于顶层
					case ElementLayer.Top:
						for(int i = 0;i<list.Count;i++)
						{
                            var e = list[i];
                            var p = e.ParentNode as SVGElement;
							if(p != null && e.NextSibling != null)
							{
								p.InternalRemoveChild(e);
								e = p.InternalAppendChild(e) as SVGElement;
								//								p.InternalInsertAfter(this.svgDocument.CreateTextNode("\n"),e);
							}
						}
						break;
						#endregion

						#region ..置于底层
					case ElementLayer.Bottom:
						for(int i = list.Count-1;i>=0;i--)
						{
							var e = list[i];
							var p = e.ParentNode as SVGElement;
							if(p != null && e.PreviousSibling != null)
							{
								p.InternalRemoveChild(e);
								//								p.InternalPrependChild(e);
								e = p.InternalPrependChild(e) as SVGElement;
								//								p.InternalInsertAfter(this.svgDocument.CreateTextNode("\n"),e);
							}
						}
						break;
						#endregion

						#region ..上移一层
					case ElementLayer.Up:
						for(int i = list.Count - 1;i>=0;i--)
						{
							SVG.SVGElement e = list[i] as SVG.SVGElement;
							SVG.SVGElement p = e.ParentElement as SVG.SVGElement;
							SVG.SVGElement n = e.NextElement as SVG.SVGElement;
							if(p != null && n != null)
							{
								p.InternalRemoveChild(e);
								//								p.InternalInsertAfter(e,n);
								p.InternalInsertAfter(e,n);
								//								p.InternalInsertAfter(this.svgDocument.CreateTextNode("\n"),e);
							}
						}
						break;
						#endregion

						#region ..下移一层
					case ElementLayer.Down:
						for(int i = 0;i<list.Count ;i++)
						{
							SVG.SVGElement e = list[i] as SVG.SVGElement;
							SVG.SVGElement p = e.ParentElement as SVG.SVGElement;
							SVG.SVGElement pre = e.PreviousElement as SVG.SVGElement;
							if(p != null && pre != null)
							{
								p.InternalRemoveChild(e);
								//								p.InternalInsertBefore(e,pre);
								p.InternalInsertBefore(e,pre);
								//								p.InternalInsertAfter(this.svgDocument.CreateTextNode("\n"),e);
							}
						}
						break;
						#endregion
				}
				doc.InvokeUndos();
				doc.AcceptNodeChanged = old;
				doc.ChangeSelectElements(list);
				this.InvalidateSelection();
			}
		}
		#endregion

		#region ..剪贴板操作
		/// <summary>
		/// 复制
		/// </summary>
		public void Copy()
        {
            this.svgDocument.DoAction(delegate
            {
                this.InnerCopy();
            }, false);
        }

        void InnerCopy()
		{
			if(this.ExecuteBehaviorPresent(Behavior.Copy))
			{
				if(this.currentOperation != null)
				{
					if(this.currentOperation.ProcessDialogKey(Keys.Control | Keys.C))
						return;
					this.currentOperation.Invalidate();
				}
				bool old = this.svgDocument.StartCopy;
				this.svgDocument.StartCopy = true;
				SVG.SVGElementCollection list = (SVG.SVGElementCollection)this.SVGDocument.SelectCollection;
				if(list.Count > 0)
				{
					string text = string.Empty;
					bool oldcopy = this.SVGDocument.AcceptNodeChanged;
					this.SVGDocument.AcceptNodeChanged = false;
					SVG.SVGElement svg = (SVG.SVGElement)((SVG.SVGElement)this.SVGDocument.RootElement).CloneNode(false);
					SVG.SVGElement defs = null;
					for(int i = 0;i<list.Count;i++)//(SVGDom.SVGElement element in list)
					{
						SVG.SVGElement element = list[i] as SVG.SVGElement;
						System.Xml.XmlElement temp = svg.InternalAppendChild((System.Xml.XmlElement)element.Clone()) as System.Xml.XmlElement;
						//产生一定的偏移
						if(temp is SVG.BasicShapes.SVGConnectionElement)
						{
							SVG.BasicShapes.SVGConnectionElement cnt = temp as SVG.BasicShapes.SVGConnectionElement;
							float x = cnt.X1.Value;
							cnt.InternalSetAttribute("x1",(x + 20).ToString());
							x = cnt.Y1.Value;
							cnt.InternalSetAttribute("y1",(x + 20).ToString());
							x = cnt.X2.Value;
							cnt.InternalSetAttribute("x2",(x + 20).ToString());
							x = cnt.Y2.Value;
							cnt.InternalSetAttribute("y2",(x + 20).ToString());
						}
						else if(temp is SVG.SVGTransformableElement)
							(temp as SVGElement).InternalSetAttribute("transform",string.Format("{0} {1}","translate(20,20)",temp.GetAttribute("transform")));
						
						//处理Use对象
						if(element is SVG.DocumentStructure.SVGUseElement)
						{
							SVG.SVGElement refelement = ((element as SVG.DocumentStructure.SVGUseElement).InstanceRoot as SVG.DocumentStructure.SVGElementInstance).CorrespondingElement as SVG.SVGElement;
							if(refelement != null)
							{
								if(defs == null)
									defs = element.OwnerDocument.CreateElement(element.OwnerDocument.Prefix,"defs",element.OwnerDocument.NamespaceURI) as SVG.SVGElement;
                                if (refelement.ID.Length > 0)
                                    if(defs.SelectSingleNode(string.Format("//*[@id='{0}']", refelement.ID)) == null)
                                        defs.InternalAppendChild(refelement.Clone());
							}
						}
						//处理箭头
						if(element is SVG.SVGTransformableElement)
						{
							SVG.SVGTransformableElement render = element as SVG.SVGTransformableElement;
							if(render.MarkerStart != null && render.MarkerStart.ParentNode != null)
							{
								if(defs == null)
									defs = element.OwnerDocument.CreateElement(element.OwnerDocument.Prefix,"defs",element.OwnerDocument.NamespaceURI) as SVG.SVGElement;
								defs.InternalAppendChild(render.MarkerStart.Clone());
							}

							if(render.MarkerEnd != null && render.MarkerEnd.ParentNode != null)
							{
								if(defs == null)
									defs = element.OwnerDocument.CreateElement(element.OwnerDocument.Prefix,"defs",element.OwnerDocument.NamespaceURI) as SVG.SVGElement;
								defs.InternalAppendChild(render.MarkerEnd.Clone());
							}
						}
						//element.InternalRemoveAttribute("id");
					}
					if(defs != null)
						svg.InternalPrependChild(defs);
					this.SVGDocument.AcceptNodeChanged = oldcopy;
					DataObject data = new DataObject("svgdata",svg.OuterXml);
					System.Windows.Forms.Clipboard.SetDataObject(data);
					svg = null;
					this.svgDocument.StartCopy = old;
				}
				
			}
		}

		/// <summary>
		/// 粘贴
		/// </summary>
		public void Paste()
        {
            this.svgDocument.DoAction(delegate
            {
                this.InnerPaste();
            }
            , true);
        }

        void InnerPaste()
		{
			if(this.ExecuteBehaviorPresent(Behavior.Paste))
			{
				if(this.currentOperation != null)
				{
					if(this.currentOperation.ProcessDialogKey(Keys.Control | Keys.V))
						return;
					this.currentOperation.Reset();
				}
				if(!this.svgDocument.IsValid)
					return;
				IDataObject data = System.Windows.Forms.Clipboard.GetDataObject();
				try
				{
					#region ..粘贴SVG对象
					//粘贴SVG对象
					if(data.GetDataPresent("svgdata"))
					{
						string text = data.GetData("svgdata").ToString();
						if(text.Length > 0)
						{
							if(this.currentOperation != null)
								this.currentOperation.Reset();
							SVG.Document.SVGDocument doc = this.SVGDocument;
							bool old = doc.AcceptNodeChanged;
							doc.AcceptNodeChanged = false;
						
							SVG.Document.SVGDocument doc1 = SVG.Document.SvgDocumentFactory.CreateSimleDocument();//(new System.Drawing.SizeF(300,300));
							doc1.LoadXml("<?xml version=\"1.0\"?>\n" + text);
							SVG.SVGElement node = (SVG.SVGElement)doc1.RootElement;
							if(node == null)
								return;
							System.Xml.XmlNodeList cnts = node.GetElementsByTagName("connect");
							Hashtable hs = new Hashtable();

							#region ..更新连接线属性
							for(int i = 0;i<cnts.Count;i++)
							{
								SVG.BasicShapes.SVGConnectionElement cnt = cnts[i] as SVG.BasicShapes.SVGConnectionElement;
								if(cnt != null)
								{
									string start = cnt.GetAttribute("start");
									SVG.SVGElement startelm = null;
									string startid = string.Empty;
									SVG.SVGElement endelm = null;
									string endid = string.Empty;
									if(start.Trim().Length > 0)
									{
										start = start.Substring(1);
										int index = start.IndexOf(".");
										if(index > 0)
											start = start.Substring(0,index);
										startelm = node.SelectSingleNode("//*[@id='"+start + "']") as SVG.SVGTransformableElement;
										if(startelm != null)
										{
											if(!connectRefs.Contains(startelm))
												connectRefs.Add(startelm);
											if(!hs.ContainsKey(startelm))
											{
												startid = this.svgDocument.CreateID(startelm,this.svgDocument.RootElement as SVG.SVGElement,false);
												hs[startelm] = startid;
											}
											else
												startid = (string)hs[startelm];
										}
									}

									string end = cnt.GetAttribute("end");
									if(end.Trim().Length > 0)
									{
										end = end.Substring(1);
										int index = end.IndexOf(".");
										if(index > 0)
											end = end.Substring(0,index);
										endelm = node.SelectSingleNode("//*[@id='"+end + "']") as SVG.SVGTransformableElement;
										if(endelm != null)
										{
											if(!connectRefs.Contains(endelm))
												connectRefs.Add(endelm);
											if(!hs.ContainsKey(endelm))
											{
												endid = this.svgDocument.CreateID(endelm,this.svgDocument.RootElement as SVG.SVGElement,false);
												hs[endelm] = endid;
											}
											else
												endid = hs[endelm].ToString();
										}
									}

									if(startelm != null)
									{
										//										startelm.InternalSetAttribute("id",startid);
										startid = "#"+startid;
									}
									if(endelm != null)
									{
										//										endelm.InternalSetAttribute("id",endid);
										endid = "#" + endid;
									}
									cnt.InternalSetAttribute("start",startid);
									cnt.InternalSetAttribute("end",endid);
								}
								
							}
							#endregion

							//更新连接线连接对象属性
							foreach(SVG.SVGElement temp in hs.Keys)
							{
								temp.InternalSetAttribute("id",hs[temp].ToString());
							}
							
                            //id
                            //var elementHasIDs = node.SelectNodes("//*[@id]");
                            System.Xml.XmlNode defNode = node.SelectSingleNode("defs") as SVG.SVGElement;
                            //System.Xml.XmlNodeList defElementsHasIDs = null;
                            //if (defNode != null)
                            //    defElementsHasIDs = defNode.SelectNodes("*[@id]");

                            //foreach (System.Xml.XmlElement elm in elementHasIDs)
                            //{
                            //    if (!hs.ContainsKey(elm))
                            //    {
                            //        elm.InternalRemoveAttribute("id");
                            //    }
                            //}
                            //去掉无用的ID
							doc.AcceptNodeChanged = true;
							
							if(defNode != null && doc.DocumentElement is SVG.SVGElement)
							{
								for(int i = 0;i<defNode.ChildNodes.Count;i++)
								{
									SVG.SVGElement child = defNode.ChildNodes[i] as SVG.SVGElement;
									if(child != null)
									{
										string id = child.ID;
										if(doc.GetReferencedNode(id) == null && id.Trim().Length > 0)
										{
											doc.AddDefsElement(child);
										}
									}
								}
							}

                            List<SVGElement> elements = new List<SVGElement>();
							using(System.Drawing.Graphics g = this.CreateGraphics())
							{
								SVG.SVGElementCollection list = ((SVG.Interface.ISVGContainer)node).ChildElements;
								for(int i = 0;i<list.Count;i++)//each(SVGDom.Interface.ISVGPathElement render in list)
								{
                                    var element = list[i];
                                    if (element.ID.Length > 0)
                                    {
                                        if (doc.RootElement.SelectSingleNode(string.Format("//*[@id='{0}']", element.ID)) != null)
                                            element.ID = this.svgDocument.CreateID(element, doc.RootElement);
                                    }
									SVG.Interface.ISVGPathable render = list[i] as SVG.Interface.ISVGPathable;
									int index = this.connectRefs.IndexOf(render);
									render = this.AddElement((SVG.SVGElement)render,false,false) as SVG.Interface.ISVGPathable;
                                    list[i] = render as SVGElement; ;
									if(index>=0)
                                        connectRefs[index] = render as SVGElement;
                                    elements.Add(render as SVGElement);
								}
							}
							doc.InvokeUndos();
							doc.AcceptNodeChanged = old;
                            doc.ChangeSelectElements(elements.ToArray());
						}
						text = null;
					}
						#endregion

						#region ..粘贴文本
					else if(data.GetDataPresent(DataFormats.Text))
					{
						//						string text = data.GetData(DataFormats.Text).ToString();
						//						string[] strings = Operation.Text.TextEditor.rg.Split(text);
						//						bool old = this.svgDocument.AcceptNodeChanged;
						//						this.svgDocument.AcceptNodeChanged = true;
						//						SVGDom.Document.SvgDocument doc = this.svgDocument;
						//						SVGDom.SVGElement element = doc.CreateElement(doc.Prefix,"text",doc.NamespaceURI) as SVGDom.SVGElement;
						//						PointF p = this.AutoScrollPosition;
						//						this.svgDocument.AcceptNodeChanged = old;
					}
					#endregion
				}
				catch(System.Exception e1)
				{
                    this.svgDocument.OnExceptionOccured(new ExceptionOccuredEventArgs(new object[] { e1.StackTrace, e1.Message }, ExceptionLevel.Normal));
				}
			}
		}

		/// <summary>
		/// 剪切
		/// </summary>
		public void Cut()
		{
			if(this.currentOperation != null)
			{
				if(this.currentOperation.ProcessDialogKey(Keys.Control | Keys.X))
					return;
				this.currentOperation.Reset();
			}
			
			this.Copy();
			this.DeleteSelection();
		}
		#endregion

		#region ..撤销重作
		/// <summary>
		/// 撤销
		/// </summary>
		public void Undo()
		{
			if(this.svgDocument != null && this.svgDocument.CanUndo)
			{
				if(this.currentOperation != null && !(this.currentOperation is Operation.Text.TextOperation))
					this.currentOperation.Invalidate();
				//please use the document not svgdocument to ensure the nodeinserted or noderemoved 
				this.Document.Undo();
				this.selectChanged = true;
				this.validContent = true;
			}
		}

		/// <summary>
		/// 重作
		/// </summary>
		public void Redo()
		{
			if(this.svgDocument != null && this.svgDocument.CanRedo)
			{
				if(this.currentOperation != null && !(this.currentOperation is Operation.Text.TextOperation))
					this.currentOperation.Invalidate();
				//please use the document not svgdocument to ensure the nodeinserted or noderemoved 
				this.Document.Redo();
				this.selectChanged = true;
				this.validContent = true;
				//this.Invalidate();
			}
		}
		#endregion

		#region ..全选
		/// <summary>
		/// 全选
		/// </summary>
		public void SelectAll()
		{
			if(this.currentOperation != null && this.currentOperation.ProcessDialogKey(Keys.Control | Keys.A))
				return;
			if(this.currentOperation != null)
				this.currentOperation.ProcessDialogKey(Keys.Control | Keys.A);
			SVG.SVGElement root = this.svgDocument.CurrentScene as SVG.SVGElement;
			if(root is SVG.Interface.ISVGContainer)
				this.SVGDocument.ChangeSelectElements(((SVG.Interface.ISVGContainer)root).ChildElements);
		}
		#endregion

        #region ..清除选区
        /// <summary>
		/// 不选中任何对象
		/// </summary>
		public void SelectNone()
		{
			if(this.currentOperation != null && this.currentOperation.ProcessDialogKey(Keys.Escape))
				return;
			if(this.currentOperation != null)
				this.currentOperation.ProcessDialogKey(Keys.Escape);
			this.svgDocument.ChangeSelectElements(null as SVG.SVGElement[]);
		}
		#endregion

		#region ..从缓存中清除所有重作信息
		/// <summary>
		/// 从缓存中清除所有重作信息
		/// </summary>
		public void ClearUndos()
		{
			this.svgDocument.ClearUndos();
		}
		#endregion

		#region ..对齐
		/// <summary>
		/// 对当前选中的对象执行对齐功能
		/// </summary>
		/// <param name="align">指定执行对齐的类别</param>
		/// <example>
		/// <code>
		/// if(this.vectorControl.ExecuteBehaviorPresent(Behavior.Align))
		/// {
		///		this.vectorControl.Align(Enum.AlignType.Top);
		/// }
		/// </code>
		/// </example>
		public void Align(AlignElementsType align)
        {
            this.svgDocument.DoAction(delegate
            {
                this.InnerAlign(align);
            }
            , true);
        }

        void InnerAlign(AlignElementsType align)
		{
			if(this.svgDocument.SelectCollection.Count <= 1 || this.selectpath == null || this.selectpath.PointCount < 2)
				return;
			if(this.currentOperation != null)
				this.currentOperation.Invalidate();
			using(GraphicsPath path = this.selectpath.Clone() as GraphicsPath,subpath = new GraphicsPath() )
			{
				path.Transform(this.selectMatrix);
				path.Flatten();
				this.InvalidateSelection();
				RectangleF bound = path.GetBounds();
				bool old = this.svgDocument.AcceptNodeChanged;
				this.svgDocument.AcceptNodeChanged = true;
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				switch(align)
				{
						#region ..顶部对齐
					case AlignElementsType.Top:
						int top = (int)bound.Top;
						for(int i = 0;i<this.svgDocument.SelectCollection.Count;i++)
						{
							SVG.Interface.ISVGPathable render = this.svgDocument.SelectCollection[i] as SVG.Interface.ISVGPathable;
                            if (render == null || (render as SVG.Interface.ISVGPathable).GPath == null || (render as SVG.Interface.ISVGPathable).GPath.PointCount <= 1)
                                continue;
							subpath.Reset();
							subpath.AddPath((render as SVG.Interface.ISVGPathable).GPath,false);
							if(subpath.PointCount < 2)
								continue;
							SVG.SVGTransformableElement transform = (SVG.SVGTransformableElement)render;
							subpath.Transform(this.GetTotalTransformForElement(transform));
							subpath.Flatten();
							int top1 = (int)subpath.GetBounds().Top;
							int delta = top - top1;
							if(delta == 0)
								continue;
							this.TransformElement(transform,new Matrix(1,0,0,1,0,delta));
						}
						break;
						#endregion

						#region ..底部对齐
					case AlignElementsType.Bottom:
						int bottom = (int)bound.Bottom;
						for(int i = 0;i<this.svgDocument.SelectCollection.Count;i++)
						{
							SVG.Interface.ISVGPathable render = this.svgDocument.SelectCollection[i] as SVG.Interface.ISVGPathable;
                            if (render == null || (render as SVG.Interface.ISVGPathable).GPath == null || (render as SVG.Interface.ISVGPathable).GPath.PointCount <= 1)
                                continue;
							subpath.Reset();
							subpath.AddPath((render as SVG.Interface.ISVGPathable).GPath,false);
							if(subpath.PointCount < 2)
								continue;
							SVG.SVGTransformableElement transform = (SVG.SVGTransformableElement)render;
							subpath.Transform(this.GetTotalTransformForElement(transform));
							subpath.Flatten();
							int bottom1 = (int)subpath.GetBounds().Bottom;
							int delta = bottom - bottom1;
							if(delta == 0)
								continue;
							this.TransformElement(transform,new Matrix(1,0,0,1,0,delta));
						}
						break;
						#endregion

						#region ..左对齐
					case AlignElementsType.Left:
						int left = (int)bound.Left;
						for(int i = 0;i<this.svgDocument.SelectCollection.Count;i++)
						{
							SVG.Interface.ISVGPathable render = this.svgDocument.SelectCollection[i] as SVG.Interface.ISVGPathable;
							if(render == null || (render as SVG.Interface.ISVGPathable).GPath == null || (render as SVG.Interface.ISVGPathable).GPath.PointCount <= 1)
								continue;
							subpath.Reset();
							subpath.AddPath((render as SVG.Interface.ISVGPathable).GPath,false);
							if(subpath.PointCount < 2)
								continue;
							SVG.SVGTransformableElement transform = (SVG.SVGTransformableElement)render;
							subpath.Transform(this.GetTotalTransformForElement(transform));
							subpath.Flatten();
							int left1 = (int)subpath.GetBounds().Left;
							int delta = left - left1;
							if(delta == 0)
								continue;
							this.TransformElement(transform,new Matrix(1,0,0,1,delta,0));	
						}
						break;
						#endregion

						#region ..右对齐
					case AlignElementsType.Right:
						int right = (int)bound.Right;
						for(int i = 0;i<this.svgDocument.SelectCollection.Count;i++)
						{
							SVG.Interface.ISVGPathable render = this.svgDocument.SelectCollection[i] as SVG.Interface.ISVGPathable;
                            if (render == null || (render as SVG.Interface.ISVGPathable).GPath == null || (render as SVG.Interface.ISVGPathable).GPath.PointCount <= 1)
                                continue;
							subpath.Reset();
							subpath.AddPath((render as SVG.Interface.ISVGPathable).GPath,false);
							if(subpath.PointCount < 2)
								continue;
							SVG.SVGTransformableElement transform = (SVG.SVGTransformableElement)render;
							subpath.Transform(this.GetTotalTransformForElement(transform));
							subpath.Flatten();
							int right1 = (int)subpath.GetBounds().Right;
							int delta = right - right1;
							if(delta == 0)
								continue;
							this.TransformElement(transform,new Matrix(1,0,0,1,delta,0));	
						}
						break;
						#endregion

						#region ..垂直中心点对齐
					case AlignElementsType.VerticalCenter:
						int middle = (int)(bound.Top + bound.Height / 2);
						for(int i = 0;i<this.svgDocument.SelectCollection.Count;i++)
						{
							SVG.Interface.ISVGPathable render = this.svgDocument.SelectCollection[i] as SVG.Interface.ISVGPathable;
                            if (render == null || (render as SVG.Interface.ISVGPathable).GPath == null || (render as SVG.Interface.ISVGPathable).GPath.PointCount <= 1)
                                continue;
							subpath.Reset();
							subpath.AddPath((render as SVG.Interface.ISVGPathable).GPath,false);
							if(subpath.PointCount < 2)
								continue;
							SVG.SVGTransformableElement transform = (SVG.SVGTransformableElement)render;
                            subpath.Transform(this.GetTotalTransformForElement(transform));
							subpath.Flatten();
							RectangleF rect1 = subpath.GetBounds();
							int middle1 = (int)(rect1.Top + rect1.Height / 2);
							int delta = middle - middle1;
							if(delta == 0)
								continue;
							this.TransformElement(transform,new Matrix(1,0,0,1,0,delta));	
						}
						break;
						#endregion

						#region ..水平中心点对齐
					case AlignElementsType.HorizontalCenter:
						int center = (int)(bound.Left+bound.Width / 2);
						for(int i = 0;i<this.svgDocument.SelectCollection.Count;i++)
						{
							SVG.Interface.ISVGPathable render = this.svgDocument.SelectCollection[i] as SVG.Interface.ISVGPathable;
                            if (render == null || (render as SVG.Interface.ISVGPathable).GPath == null || (render as SVG.Interface.ISVGPathable).GPath.PointCount <= 1)
                                continue;
							subpath.Reset();
							subpath.AddPath((render as SVG.Interface.ISVGPathable).GPath,false);
							if(subpath.PointCount < 2)
								continue;
							SVG.SVGTransformableElement transform = (SVG.SVGTransformableElement)render;
                            subpath.Transform(this.GetTotalTransformForElement(transform));
							subpath.Flatten();
							RectangleF rect1 = subpath.GetBounds();
							int center1 = (int)(rect1.Left + rect1.Width /2);
							int delta = center - center1;
							if(delta == 0)
								continue;
							this.TransformElement(transform,new Matrix(1,0,0,1,delta,0));
						}
						break;
						#endregion
				}
				this.selectChanged = true;
				this.svgDocument.AcceptNodeChanged = old;
				this.svgDocument.InvokeUndos();
				
			}
		}
		#endregion

		#region ..分布对象
		/// <summary>
		/// 将选定的对象按间距进行分布
		/// </summary>
		/// <param name="type">执行分布的类别</param>
		/// <example>
		/// <code>
		/// if(this.vectorControl.ExecuteBehaviorPresent(Behavior.Distriute))
		/// {
		///		this.vectorControl.Distriute(DistributeType.Top);
		/// }
		/// </code>
		/// </example>
		public void Distribute(DistributeType type)
        {
            this.svgDocument.DoAction(delegate
            {
                this.InnerDistribute(type);
            }
            , true);
        }

        void InnerDistribute(DistributeType type)
		{
			if(this.svgDocument.SelectCollection.Count < 3)
				return;
			if(this.currentOperation != null)
				this.currentOperation.Invalidate();
			SVG.SVGElementCollection list = (SVG.SVGElementCollection)this.SVGDocument.SelectCollection;
			bool old = this.svgDocument.AcceptNodeChanged;
			this.svgDocument.AcceptNodeChanged = true;
			ArrayList elements = new ArrayList();
			ArrayList poses = new ArrayList();
			SVG.SVGElement[] sortelements = null;
			float[] sortposes = null,sortposes1 = null,length1 = null;
			float min = 0,delta = 0;
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			this.InvalidateSelection();
			System.Collections.ArrayList lengths = new ArrayList();
			switch(type)
			{
					#region ..上边缘
				case DistributeType.Top:
					//填充位置信息
					for(int i = 0;i<list.Count;i++)
					{
						SVG.Interface.ISVGPathable render = list[i] as SVG.Interface.ISVGPathable;
						if(render == null || (render as SVG.Interface.ISVGPathable).GPath == null || (render as SVG.Interface.ISVGPathable).GPath.PointCount <= 1)
								continue;
						using(GraphicsPath path = (render as SVG.Interface.ISVGPathable).GPath.Clone() as GraphicsPath)
						{
							path.Transform(this.GetTotalTransformForElement(render as SVG.SVGTransformableElement));
							path.Flatten();
							RectangleF rect = path.GetBounds();
							poses.Add(rect.Top);
							elements.Add(render);
							lengths.Add(rect.Height);
						}
					}
					//如果不超过三个排序对象，返回
					if(elements.Count < 3)
						break;
					sortelements = new SVGElement[elements.Count];
					elements.CopyTo(sortelements,0);
					sortposes = new float[poses.Count];
					poses.CopyTo(sortposes,0);
					sortposes1 = (float[])sortposes.Clone();
					length1 = new float[lengths.Count];
					lengths.CopyTo(length1);
					Array.Sort(sortposes,sortelements);
					Array.Sort(sortposes1,length1);
					min = sortposes[0];
					delta = (sortposes[sortposes.Length - 1] - sortposes[0]) / (sortposes.Length - 1); 
					for(int i = 0;i<sortelements.Length ;i++)
					{
						float v = sortposes[i];
						for(int j = i + 1;j<sortelements.Length;j++)
						{
							float v1 = sortposes[j];
							if(v1!= v)
							{
								if(j - 1 > i)
								{
									Array.Sort(length1,sortelements,i,j-i);
									if(i == 0)
										Array.Reverse(sortelements,i ,j - i);
								}
								i = j - 1;
								break;
							}
						}
					}

					for(int i = 1;i<sortelements.Length - 1;i++)
					{
						//执行分布
						SVG.SVGTransformableElement transform = (SVG.SVGTransformableElement)sortelements[i];
						this.TransformElement(transform,new Matrix(1,0,0,1,0,min + delta * i - sortposes[i]));
					}
					break;
					#endregion

					#region ..下边缘
				case DistributeType.Bottom:
					//填充位置信息
					for(int i = 0;i<list.Count;i++)
					{
						SVG.Interface.ISVGPathable render = list[i] as SVG.Interface.ISVGPathable;
                        if (render == null || (render as SVG.Interface.ISVGPathable).GPath == null || (render as SVG.Interface.ISVGPathable).GPath.PointCount <= 1)
                            continue;
						using(GraphicsPath path = (render as SVG.Interface.ISVGPathable).GPath.Clone() as GraphicsPath)
						{
                            path.Transform(this.GetTotalTransformForElement(render as SVG.SVGTransformableElement));
							path.Flatten();
							RectangleF rect = path.GetBounds();
							poses.Add(rect.Bottom);
							elements.Add(render);
							lengths.Add(rect.Height);
						}
					}
					//如果不超过三个排序对象，返回
					if(elements.Count < 3)
						break;
					sortelements = new SVGElement[elements.Count];
					elements.CopyTo(sortelements,0);
					sortposes = new float[poses.Count];
					poses.CopyTo(sortposes,0);
					sortposes1 = (float[])sortposes.Clone();
					length1 = new float[lengths.Count];
					lengths.CopyTo(length1);
					Array.Sort(sortposes,sortelements);
					Array.Sort(sortposes1,length1);
					min = sortposes[0];
					delta = (sortposes[sortposes.Length - 1] - sortposes[0]) / (sortposes.Length - 1); 
					for(int i = 0;i<sortelements.Length ;i++)
					{
						float v = sortposes[i];
						for(int j = i + 1;j<sortelements.Length;j++)
						{
							float v1 = sortposes[j];
							if(v1!= v)
							{
								if(j - 1 > i)
								{
									Array.Sort(length1,sortelements,i,j-i);
									if(i == 0)
										Array.Reverse(sortelements,i ,j - i);
								}
								i = j - 1;
								break;
							}
						}
					}
					
					min = sortposes[0];
					delta = (sortposes[sortposes.Length - 1] - sortposes[0]) / (sortposes.Length - 1); 
					for(int i = 1;i<sortelements.Length - 1;i++)
					{
						//执行分布
						SVG.SVGTransformableElement transform = (SVG.SVGTransformableElement)sortelements[i];
						this.TransformElement(transform,new Matrix(1,0,0,1,0,min + delta * i - sortposes[i]));
					}
					break;
					#endregion

					#region ..右边缘
				case DistributeType.Right:
					//填充位置信息
					for(int i = 0;i<list.Count;i++)
					{
						SVG.Interface.ISVGPathable render = list[i] as SVG.Interface.ISVGPathable;
                        if (render == null || (render as SVG.Interface.ISVGPathable).GPath == null || (render as SVG.Interface.ISVGPathable).GPath.PointCount <= 1)
                            continue;
						using(GraphicsPath path = (render as SVG.Interface.ISVGPathable).GPath.Clone() as GraphicsPath)
						{
                            path.Transform(this.GetTotalTransformForElement(render as SVG.SVGTransformableElement));
							path.Flatten();
							RectangleF rect = path.GetBounds();
							poses.Add(rect.Right);
							elements.Add(render);
							lengths.Add(rect.Width);
						}
					}
					//如果不超过三个排序对象，返回
					if(elements.Count < 3)
						break;
					sortelements = new SVGElement[elements.Count];
					elements.CopyTo(sortelements,0);
					sortposes = new float[poses.Count];
					poses.CopyTo(sortposes,0);
					sortposes1 = (float[])sortposes.Clone();
					length1 = new float[lengths.Count];
					lengths.CopyTo(length1);
					Array.Sort(sortposes,sortelements);
					Array.Sort(sortposes1,length1);
					min = sortposes[0];
					delta = (sortposes[sortposes.Length - 1] - sortposes[0]) / (sortposes.Length - 1); 
					for(int i = 0;i<sortelements.Length ;i++)
					{
						float v = sortposes[i];
						for(int j = i + 1;j<sortelements.Length;j++)
						{
							float v1 = sortposes[j];
							if(v1!= v)
							{
								if(j - 1 > i)
								{
									Array.Sort(length1,sortelements,i,j-i);
									if(i == 0)
										Array.Reverse(sortelements,i ,j - i);
								}
								i = j - 1;
								break;
							}
						}
					}

					for(int i = 1;i<sortelements.Length - 1;i++)
					{
						//执行分布
						SVG.SVGTransformableElement transform = (SVG.SVGTransformableElement)sortelements[i];
						this.TransformElement(transform,new Matrix(1,0,0,1,min + delta * i - sortposes[i],0));
					}
					break;
					#endregion

					#region ..左边缘
				case DistributeType.Left:
					//填充位置信息
					for(int i = 0;i<list.Count;i++)
					{
						SVG.Interface.ISVGPathable render = list[i] as SVG.Interface.ISVGPathable;
                        if (render == null || (render as SVG.Interface.ISVGPathable).GPath == null || (render as SVG.Interface.ISVGPathable).GPath.PointCount <= 1)
                            continue;
						using(GraphicsPath path = (render as SVG.Interface.ISVGPathable).GPath.Clone() as GraphicsPath)
						{
                            path.Transform(this.GetTotalTransformForElement(render as SVG.SVGTransformableElement));
							path.Flatten();
							RectangleF rect = path.GetBounds();
							poses.Add(rect.Left);
							elements.Add(render);
							lengths.Add(rect.Width);
						}
					}
					//如果不超过三个排序对象，返回
					if(elements.Count < 3)
						break;
					sortelements = new SVGElement[elements.Count];
					elements.CopyTo(sortelements,0);
					sortposes = new float[poses.Count];
					poses.CopyTo(sortposes,0);
					sortposes1 = (float[])sortposes.Clone();
					length1 = new float[lengths.Count];
					lengths.CopyTo(length1);
					Array.Sort(sortposes,sortelements);
					Array.Sort(sortposes1,length1);
					min = sortposes[0];
					delta = (sortposes[sortposes.Length - 1] - sortposes[0]) / (sortposes.Length - 1); 
					for(int i = 0;i<sortelements.Length ;i++)
					{
						float v = sortposes[i];
						for(int j = i + 1;j<sortelements.Length;j++)
						{
							float v1 = sortposes[j];
							if(v1!= v)
							{
								if(j - 1 > i)
								{
									Array.Sort(length1,sortelements,i,j-i);
									if(i == 0)
										Array.Reverse(sortelements,i ,j - i);
								}
								i = j - 1;
								break;
							}
						}
					}

					for(int i = 1;i<sortelements.Length - 1;i++)
					{
						//执行分布
						SVG.SVGTransformableElement transform = (SVG.SVGTransformableElement)sortelements[i];
						this.TransformElement(transform,new Matrix(1,0,0,1,min + delta * i - sortposes[i],0));
					}
					break;
					#endregion

					#region ..垂直中心点
				case DistributeType.VerticalCenter:
					//填充位置信息
					for(int i = 0;i<list.Count;i++)
					{
						SVG.Interface.ISVGPathable render = list[i] as SVG.Interface.ISVGPathable;
                        if (render == null || (render as SVG.Interface.ISVGPathable).GPath == null || (render as SVG.Interface.ISVGPathable).GPath.PointCount <= 1)
                            continue;
						using(GraphicsPath path = (render as SVG.Interface.ISVGPathable).GPath.Clone() as GraphicsPath)
						{
                            path.Transform(this.GetTotalTransformForElement(render as SVG.SVGTransformableElement));
							path.Flatten();
							RectangleF rect = path.GetBounds();
							poses.Add(rect.Y + rect.Height / 2);
							elements.Add(render);
							lengths.Add(rect.Height);
						}
					}
					//如果不超过三个排序对象，返回
					if(elements.Count < 3)
						break;
					sortelements = new SVGElement[elements.Count];
					elements.CopyTo(sortelements,0);
					sortposes = new float[poses.Count];
					poses.CopyTo(sortposes,0);
					sortposes1 = (float[])sortposes.Clone();
					length1 = new float[lengths.Count];
					lengths.CopyTo(length1);
					Array.Sort(sortposes,sortelements);
					Array.Sort(sortposes1,length1);
					min = sortposes[0];
					delta = (sortposes[sortposes.Length - 1] - sortposes[0]) / (sortposes.Length - 1); 
					for(int i = 0;i<sortelements.Length ;i++)
					{
						float v = sortposes[i];
						for(int j = i + 1;j<sortelements.Length;j++)
						{
							float v1 = sortposes[j];
							if(v1!= v)
							{
								if(j - 1 > i)
								{
									Array.Sort(length1,sortelements,i,j-i);
									if(i == 0)
										Array.Reverse(sortelements,i ,j - i);
								}
								i = j - 1;
								break;
							}
						}
					}
					min = sortposes[0];
					delta = (sortposes[sortposes.Length - 1] - sortposes[0]) / (sortposes.Length - 1); 
					for(int i = 1;i<sortelements.Length - 1;i++)
					{
						//执行分布
						SVG.SVGTransformableElement transform = (SVG.SVGTransformableElement)sortelements[i];
						this.TransformElement(transform,new Matrix(1,0,0,1,0,min + delta * i - sortposes[i]));
					}
					break;
					#endregion

					#region ..水平中心点
				case DistributeType.HorizontalCenter:
					//填充位置信息
					for(int i = 0;i<list.Count;i++)
					{
						SVG.Interface.ISVGPathable render = list[i] as SVG.Interface.ISVGPathable;
                        if (render == null || (render as SVG.Interface.ISVGPathable).GPath == null || (render as SVG.Interface.ISVGPathable).GPath.PointCount <= 1)
                            continue;
						using(GraphicsPath path = (render as SVG.Interface.ISVGPathable).GPath.Clone() as GraphicsPath)
						{
                            path.Transform(this.GetTotalTransformForElement(render as SVG.SVGTransformableElement));
							path.Flatten();
							RectangleF rect = path.GetBounds();
							poses.Add(rect.Left + rect.Width / 2);
							elements.Add(render);
							lengths.Add(rect.Width);
						}
					}
					//如果不超过三个排序对象，返回
					if(elements.Count < 3)
						break;
					sortelements = new SVGElement[elements.Count];
					elements.CopyTo(sortelements,0);
					sortposes = new float[poses.Count];
					poses.CopyTo(sortposes,0);
					sortposes1 = (float[])sortposes.Clone();
					length1 = new float[lengths.Count];
					lengths.CopyTo(length1);
					Array.Sort(sortposes,sortelements);
					Array.Sort(sortposes1,length1);
					min = sortposes[0];
					delta = (sortposes[sortposes.Length - 1] - sortposes[0]) / (sortposes.Length - 1); 
					for(int i = 0;i<sortelements.Length ;i++)
					{
						float v = sortposes[i];
						for(int j = i + 1;j<sortelements.Length;j++)
						{
							float v1 = sortposes[j];
							if(v1!= v)
							{
								if(j - 1 > i)
								{
									Array.Sort(length1,sortelements,i,j-i);
									if(i == 0)
										Array.Reverse(sortelements,i ,j - i);
								}
								i = j - 1;
								break;
							}
						}
					}

					for(int i = 1;i<sortelements.Length - 1;i++)
					{
						//执行分布
						SVG.SVGTransformableElement transform = (SVG.SVGTransformableElement)sortelements[i];
						this.TransformElement(transform,new Matrix(1,0,0,1,min + delta * i - sortposes[i],0));
					}
					break;
					#endregion
			}
			sb = null;
			sortposes = null;
			sortelements = null;
			length1 = null;
			sortposes1 = null;
			elements = null;
			poses = null;
			this.selectChanged = true;
			this.svgDocument.AcceptNodeChanged = old;
			this.svgDocument.InvokeUndos();
		}
		#endregion

		#region ..导出栅格图像
		/// <summary>
		/// 导出栅格图像
		/// </summary>
		public void ExportImage()
		{
			this.exportdlg.ShowDialog(this);
		}
		#endregion

		#region ..调整尺寸
		/// <summary>
		/// 调整当前选中对象，使他们具备相同的宽度或高度
		/// </summary>
		/// <param name="type">指定调整尺寸的类别</param>
		/// <example>
		/// <code>
		/// if(this.vectorControl.ExecuteBehaviorPresent(Behavior.SameSize))
		/// {
        ///		this.vectorControl.AdjustElementsSize(SizeType.Height);
		/// }
		/// </code>
		/// </example>
		public void AdjustElementsSize(ElementSizeAdjustment type)
        {
            this.svgDocument.DoAction(delegate
            {
                this.InnerMakeSameSize(type);
            }, true);
        }

        void InnerMakeSameSize(ElementSizeAdjustment type)
		{
			if(this.svgDocument.SelectCollection.Count < 2)
				return;
			SVG.Interface.ISVGPathable render = this.svgDocument.SelectCollection[0] as SVG.Interface.ISVGPathable;
			RectangleF bound = RectangleF.Empty;
			bool old = this.svgDocument.AcceptNodeChanged;
			this.svgDocument.AcceptNodeChanged = true;
			this.InvalidateSelection();
			bool changed = false;
			using(System.Drawing.Drawing2D.GraphicsPath path = (render as SVG.Interface.ISVGPathable).GPath.Clone() as GraphicsPath)
			{
				path.Transform(this.GetTotalTransformForElement((SVG.SVGTransformableElement)render));
				bound = path.GetBounds();
				
				for(int i = 1;i<this.svgDocument.SelectCollection.Count;i ++)
				{
					render = this.svgDocument.SelectCollection[i] as SVG.Interface.ISVGPathable;
					path.Reset();
                    if ((render as SVG.Interface.ISVGPathable).GPath == null || (render as SVG.Interface.ISVGPathable).GPath.PointCount <= 1)
                        continue;
					path.AddPath((render as SVG.Interface.ISVGPathable).GPath,false);
					path.Transform(this.GetTotalTransformForElement((SVG.SVGTransformableElement)render));
					RectangleF rect1 = path.GetBounds();
					SVG.SVGTransformableElement transform = (SVG.SVGTransformableElement)render;
					if(rect1.Width != bound.Width || rect1.Height != bound.Height)
					{
						float x = 1,y = 1;
						if(rect1.Width > 0 && bound.Width > 0)
							x = bound.Width / rect1.Width;
						if(rect1.Height > 0 && bound.Height > 0)
							y = bound.Height / rect1.Height;
						if(type == ElementSizeAdjustment.Height)
							x = 1;
						else if(type == ElementSizeAdjustment.Width)
							y = 1;
						if(x != 1 || y != 1)
						{
							Matrix matrix = new Matrix();
							matrix.Translate(rect1.X,rect1.Y);
							matrix.Scale(x,y);
							matrix.Translate(-rect1.X,-rect1.Y);
							this.TransformElement(transform,matrix);							
							changed = true;
						}
					}
				}
			}
			this.svgDocument.AcceptNodeChanged = old;
			this.svgDocument.InvokeUndos();
			this.selectChanged = true;
			this.updateinfo = true;
			if(changed)
			{
				this.UpdateSelectInfo();
				this.InvalidateSelection();
			}
		}
		#endregion

		#region ..判断当前是否可以执行指定的行为
		/// <summary>
		/// 判断当前是否可以执行指定的行为
		/// </summary>
		/// <param name="behavior">需要执行的行为</param>
		/// <example>
		/// <code>
		/// if(this.vectorControl.ExecuteBehaviorPresent(Behavior.AdjustLevel))
		/// {
		///		this.vectorControl.UpdateLevel(Level.Top);
		/// }
		/// </code>
		/// </example>
		public bool ExecuteBehaviorPresent(Behavior behavior)
		{
			bool a = false;
			SVG.DocumentStructure.SVGSVGElement root = this.svgDocument.RootElement as SVG.DocumentStructure.SVGSVGElement;
			if(root == null)
				return false;
			if(this.currentOperation != null)
			{
				bool a1 = this.currentOperation.ExecuteBehaviorPresent(behavior);
					
				if(this.currentOperation.EditText)
					return a1;
				if(!a1)
					return a1;
			}
			switch(behavior)
			{
				case Behavior.AdjustLayer:
					a = this.svgDocument.SelectCollection.Count > 0 && root.ChildElements.Count > 1; 
					break;
				case Behavior.AlignElements:
					a = this.svgDocument.SelectCollection.Count > 1;
					break;
				case Behavior.Distriute:
					a = this.svgDocument.SelectCollection.Count > 2;
					break;
				case Behavior.Group:
					a = this.svgDocument.SelectCollection.Count > 1;
					break;
				case Behavior.UnGroup:
					a = this.svgDocument.SelectCollection.Count == 1 && this.svgDocument.SelectCollection[0] is SVG.DocumentStructure.SVGGElement;// && !(this.svgDocument.SelectCollection[0] is SVG.Text.SVGTextElement);
					break;
				case Behavior.AdjustSize:
					a = this.svgDocument.SelectCollection.Count > 1;
					break;
				case Behavior.SelectNone:
					a = this.svgDocument.SelectCollection.Count > 0;
					break;
				case Behavior.Copy:
				case Behavior.Cut:
					a = this.svgDocument.SelectCollection.Count > 0;
					break;
				case Behavior.Paste:
					IDataObject data = System.Windows.Forms.Clipboard.GetDataObject();
					a = data.GetDataPresent("svgdata");
					break;
				case Behavior.Undo:
					a = this.svgDocument.CanUndo;
					break;
				case Behavior.Redo:
					a = this.svgDocument.CanRedo;
					break;
				case Behavior.Delete:
					a = this.svgDocument.SelectCollection.Count > 0;
					break;
				case Behavior.SelectAll:
					a = root.ChildElements.Count > 0;
					break;
				case Behavior.Transform:
					a = this.svgDocument.SelectCollection.Count > 0;
					break;
			}
			return a;
		}
		#endregion

		#region ..打印
		/// <summary>
		/// 将当前文档内容打印到终端设备
		/// </summary>
		public void Print()
		{
			try
			{
                Forms.PrintDialog printdlg = new Forms.PrintDialog(this);
				printdlg.ShowDialog(this);
			}
			catch(System.Exception e1)
			{
				System.Windows.Forms.MessageBox.Show(this,e1.Message,"打印机",MessageBoxButtons.OK,MessageBoxIcon.Warning);
			}
		}
		#endregion

		#region ..对当前选中对象执行指定变换
		/// <summary>
		/// 对当前选中对象执行指定变换
		/// </summary>
		/// <param name="matrix">将要对当前选区执行的二维变换矩阵</param>
		public void TransformSelection(System.Drawing.Drawing2D.Matrix matrix)
        {
            this.svgDocument.DoAction(delegate
            {
                this.InnerTransformSelection(matrix);
            }, true);
        }

        void InnerTransformSelection(Matrix matrix)
		{
			if(this.ExecuteBehaviorPresent(Behavior.Transform))
			{
                //如果存在分叉连接线，并且Matrix包含扭曲，则不能进行
                if (this.hasConnectionBranch && (matrix.Elements[1] != 0 || matrix.Elements[1] != 0))
                    return;
				SVG.Document.SVGDocument doc = this.svgDocument;
                if (doc.SelectCollection.Count > 0)
				{
					if(this.currentOperation != null)
						this.currentOperation.Invalidate();
					this.InvalidateSelection();
					bool old = doc.AcceptNodeChanged;
					doc.AcceptNodeChanged = true;
					using(Matrix matrix1 = new Matrix())
					{
						matrix1.Translate(this.CenterPoint.X,this.CenterPoint.Y);
						matrix1.Multiply(matrix);
						matrix1.Translate(-this.CenterPoint.X,-this.CenterPoint.Y);
						SVG.SVGElementCollection list = doc.SelectCollection as SVG.SVGElementCollection;
						for(int i = 0;i<list.Count;i++)
							this.TransformElement(list[i] as SVG.SVGTransformableElement,matrix1);
					}
					doc.AcceptNodeChanged = old;
					doc.InvokeUndos();
					this.selectChanged = true;
					this.UpdateSelectInfo();
					this.InvalidateSelection();
				}
			}
		}
		#endregion

        #region ..public GetCurrentViewPosition
        /// <summary>
        /// 获取当前视图坐标系中的鼠标点位置
        /// </summary>
        /// <returns>鼠标在视图坐标系中的位置</returns>
        public Point GetCurrentViewPosition()
        {
            PointF point = this.GetPoint(this.PointToClient(Control.MousePosition), MouseButtons.Left);
            return this.PointClientToView(Point.Round(point));
        }
        #endregion

        #region ..FindElementAtLocation
        /// <summary>
        /// 获取处在指定工作区坐标上的SVGElement对象，如果有多个Element对象，取最上面一个
        /// </summary>
        /// <param name="clientPoint">工作区坐标</param>
        /// <param name="excludeGroup">是否排除组对象，如果该值为真，则只返回最底层的绘制对象，不返回组对象</param>
        /// <returns>SVGElement对象</returns>
        public SVGTransformableElement GetElementAtLocation(Point clientPoint, bool excludeGroup)
        {
            SVGElementCollection list = new SVGElementCollection();
            if (excludeGroup)
                list = this.renderElements;
            else if (this.CurrentScene is SVG.Interface.ISVGContainer)
                list = (this.CurrentScene as SVG.Interface.ISVGContainer).ChildElements;
            SVG.SVGTransformableElement element = this.GetElementAtViewPoint(this.PointClientToView(clientPoint), list) as SVGTransformableElement;
            if (element != null)
                return element;
            return null;
        }

        /// <summary>
        /// 获取处在指定工作区坐标上的SVGElement对象，如果有多个Element对象，取最上面一个
        /// </summary>
        /// <param name="clientPoint">工作区坐标</param>
        /// <returns>SVGElement对象，该对象是最底层的直接绘制对象</returns>
        public SVGTransformableElement GetElementAtLocation(Point clientPoint)
        {
            return this.GetElementAtLocation(clientPoint, true);
        }
        #endregion

		#region ..ExportSVG
		/// <summary>
		/// <para>导出SVG文件.</para>
		/// <para>VectorControl控件提供了导出SVG文件的功能，这个与属性XmlCode的差别在于：</para>
		/// <list type="bullet"><item><description>在VectorControl控件操作种，为了满足实际需要，增加了一些扩展要素（如连接线等）
		/// 采用的渲染方式也和SVG渲染有一定的差别，如果直接用XmlCode代码预览SVG文件，您可能会看到和编辑环境不一样的结果。
		/// <para>通过导出功能，VectorControl可以将编辑环境中的扩展要素转换为标准的SVG对象，使得导出的SVG文件能够完全被SVG浏览器解析并保持和编辑环境一样的效果。</para></description></item>
		/// <item><description>ExportSVG提供了一个参数，可以控制是否将编辑环境中的外部资源（如栅格图像）转换为XML文件的base64资源，
		/// 通过这种转换，SVG文件可以脱离你所插入的外部资源而独立使用。</description></item>
		/// </list>
		/// </summary>
		/// <param name="convertSourceToBase64">是否将SVG文档中的外部资源转换为base64资源</param>
        /// <param name="exportBackground">是否导出背景</param>
		public string ExportSVG(bool convertSourceToBase64, bool exportBackground)
		{
			SVGElement elm = this.Document.ExportNativeSVG(convertSourceToBase64);

            if (elm != null && exportBackground)
            {
                SVGElement bk = this.svgDocument.CreateSVGRectElement(); 
                float width = this.DocumentSize.Width;
                float height = this.DocumentSize.Height;
                if (ScaleWithWindowSize)
                {
                    width = this.contentBounds.Width;
                    height = this.contentBounds.Height;
                }
                
                {
                    bool old = this.svgDocument.AcceptNodeChanged;
                    this.svgDocument.AcceptNodeChanged = false;
                    bk.SetAttribute("width", width.ToString());
                    bk.SetAttribute("height", height.ToString());
                    bk.SetAttribute("fill", SVG.ColorHelper.GetColorStringInHex(this.canvasColor));
                    bk.SetAttribute("background", "true");
                    elm.PrependChild(bk);
                    this.svgDocument.AcceptNodeChanged = old;
                }
            }
            string code = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
            if(elm != null)
                code += elm.OuterXml.Replace("xmlns=\"\"", string.Empty);
            return code;
		}

        /// <summary>
        /// <para>导出SVG文件.</para>
        /// <para>VectorControl控件提供了导出SVG文件的功能，这个与属性XmlCode的差别在于：</para>
        /// <list type="bullet"><item><description>在VectorControl控件操作种，为了满足实际需要，增加了一些扩展要素（如连接线等）
        /// 采用的渲染方式也和SVG渲染有一定的差别，如果直接用XmlCode代码预览SVG文件，您可能会看到和编辑环境不一样的结果。
        /// <para>通过导出功能，VectorControl可以将编辑环境中的扩展要素转换为标准的SVG对象，使得导出的SVG文件能够完全被SVG浏览器解析并保持和编辑环境一样的效果。</para></description></item>
        /// <item><description>ExportSVG提供了一个参数，可以控制是否将编辑环境中的外部资源（如栅格图像）转换为XML文件的base64资源，
        /// 通过这种转换，SVG文件可以脱离你所插入的外部资源而独立使用。</description></item>
        /// </list>
        /// </summary>
        /// <param name="convertSourceToBase64">是否将SVG文档中的外部资源转换为base64资源</param>
        public string ExportSVG(bool convertSourceToBase64)
        {
            return this.ExportSVG(convertSourceToBase64, true);
        }
		#endregion

        #region ..PointClientToView
        /// <summary>
        /// 将指定工作区上的点转换为视图坐标
        /// </summary>
        /// <param name="clientPoint">工作区坐标</param>
        /// <returns>视图坐标（画布）系坐标值</returns>
        public Point PointClientToView(Point clientPoint)
        {
            PointF[] ps = new PointF[] { clientPoint };
            this.PointsClientToView(ps);
            Point p = Point.Round(ps[0]);
            ps = null;
            return p;
        }

        /// <summary>
        /// 将指定工作区上的点转换为视图坐标
        /// </summary>
        /// <param name="clientPoint">工作区坐标</param>
        /// <returns>视图坐标（画布）系坐标值</returns>
        public PointF PointClientToView(PointF clientPoint)
        {
            PointF[] ps = new PointF[] { clientPoint };
            this.PointsClientToView(ps);
            return ps[0];
        }

        /// <summary>
        /// 将控件屏幕坐标转换为视图坐标
        /// </summary>
        internal void PointsClientToView(PointF[] ps)
        {
            using (Matrix matrix = this.CoordTransform)
            {
                matrix.Invert();
                matrix.TransformPoints(ps);
            }
        }
		#endregion

        #region ..AddGuide
        /// <summary>
        /// 添加一条连接线
        /// </summary>
        /// <param name="axis">坐标值</param>
        /// <param name="isVertical">坐标系方向，垂直为true，水平为false</param>
        public void AddGuide(int axis, bool isVertical)
        {
            if (isVertical)
            {
                this.vGuides.Add(axis);
                this.Invalidate();
            }
            else
            {
                this.hGuides.Add(axis);
                this.Invalidate();
            }
        }
        #endregion

        #region ..ScrollToCenter
        /// <summary>
        /// 滚动到画布中心
        /// </summary>
        public void ScrollToCenter()
        {
            if (this.AutoScroll)
            {
                int x = this.AutoScrollMinSize.Width - this.Width;
                int y = this.AutoScrollMinSize.Height - this.Height;
                x = (int)Math.Max(0, x / 2);
                y = (int)Math.Max(0, y / 2);
                this.SetScrollPos(new Point(x, y));
            }
            else
                this.SetScrollPos(Point.Empty);
        }
        #endregion

        #region ..ScaleWithCenter
        /// <summary>
        /// 围绕指定位置进行缩放
        /// </summary>
        /// <param name="scale">缩放的目标比例 </param>
        /// <param name="centerPoint">中心点</param>
        public void ScaleAtCenter(float scale, Point centerPoint)
        {
            using (System.Drawing.Drawing2D.Matrix matrix = this.CoordTransform)
            {
                float oriscale = this.ScaleRatio;
                matrix.Translate(this.VirtualLeft, this.VirtualTop, MatrixOrder.Append);
                Margin margin = this.CanvasMargin;
                matrix.Translate(-margin.Left, -margin.Top, MatrixOrder.Append);
                matrix.Scale(1f / oriscale, 1f / oriscale, MatrixOrder.Append);
                Point p = this.PointClientToView(centerPoint);
                this.ScaleRatio = scale;
                if (oriscale != this.ScaleRatio)
                {
                    matrix.Scale(this.ScaleRatio, this.ScaleRatio, MatrixOrder.Append);
                    matrix.Translate(margin.Left, margin.Top, MatrixOrder.Append);
                    PointF[] ps = new PointF[] { p };
                    matrix.TransformPoints(ps);
                    this.SetScrollPos(new Point((int)(ps[0].X - centerPoint.X), (int)(ps[0].Y - centerPoint.Y)));
                    this.scrolled = true;
                }
            }
        }
        #endregion

        #region ..ScrollToPoint
        /// <summary>
        /// 移动画布，使得指定的坐标位于中央
        /// </summary>
        /// <param name="viewPoint">视图坐标</param>
        public void ScrollToViewPoint(Point viewPoint)
        {
            PointF clientPoint = this.PointViewToClient(viewPoint);
            using (Matrix coord = this.CoordTransform)
            {
                coord.Invert();
                PointF[] ps = new PointF[] { clientPoint };
                coord.TransformPoints(ps);
                clientPoint = ps[0];

                Point p = this.AutoScrollPosition;
                PointF[] ps1 = new PointF[] { new PointF(0, 0), new PointF(this.ClientSize.Width, this.ClientSize.Height) };
                this.PointsClientToView(ps1);
                PointF p1 = new PointF(ps1[0].X + (ps1[1].X - ps1[0].X) / 2f, ps1[0].Y + (ps1[1].Y - ps1[0].Y) / 2f);

                float deltaX = clientPoint.X - p1.X;
                float deltaY = clientPoint.Y - p1.Y;
                deltaX = deltaX * ScaleRatio;
                deltaY = deltaY * ScaleRatio;

                p = Point.Ceiling(new PointF(-p.X + deltaX, -p.Y + deltaY));
                this.SetScrollPos(p);
            }
        }
        #endregion
		#endregion

        #region ..导出图元
        /// <summary>
        /// 将当前绘图内容或选区导出为图元或自定义形状
        /// 通过此方法，您可以产生自己的图元库和自定义形状库
        /// </summary>
        /// <param name="wholecontent">指定是否将所有绘制内容导出,如果不是,则只导出选区内容</param>
        /// <param name="exportshape">指定是否导出形状,如果不是,则导出图元，注意，如果是导出形状，只能导出曲线，直线，多边形，折线，矩形，椭圆，圆，连接线等基本图形</param>
        /// <param name="id">指定导出的图元或形状的ID号</param>
        /// <param name="createdocument">指定最后导出的代码是否成为一个完整的SVG文件,如果不是,则只产生单个的图元或自定义形状节点</param>
        /// <returns>最终导出的SVG代码</returns>
        internal string ExportSymbol(bool wholecontent, bool exportshape, bool createdocument, string id)
        {
            if (this.svgDocument == null || !this.svgDocument.IsValid)
                return string.Empty;
            bool old = this.svgDocument.AcceptNodeChanged;
            this.svgDocument.AcceptNodeChanged = false;
            SVG.SVGElement mainElement = this.ExportSymbolElement(wholecontent, exportshape, createdocument);
            if (createdocument)
            {
                for (int i = 0; i < mainElement.ChildNodes.Count; i++)
                {
                    if (mainElement.ChildNodes[i] is SVGElement)
                    {
                        (mainElement.ChildNodes[i] as SVGElement).InternalSetAttribute("id", id);
                        break;
                    }
                }

            }
            else if (mainElement != null)
                mainElement.InternalSetAttribute("id", id);
            string temp = string.Empty;
            if (mainElement != null)
                temp = mainElement.OuterXml;
            if (createdocument)
                temp = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + temp;
            this.svgDocument.AcceptNodeChanged = old;
            return temp;
        }

        /// <summary>
        /// 该方法提供了<see cref="ExportSymbol">ExportSymbol</see>方法的可视化调用，显示对话框以导出图元.
        /// 在对话框中，您可以设置相关参数，预览代码，并可以将代码保存到文件中。
        /// </summary>
        /// <param name="filefilter">在保存文件时，文件格式过滤，如果为空，系统默认采用SVG</param>
        internal void ShowExportSymbolDialog(string filefilter)
        {
            try
            {
                ExportSymbolDialog dlg = new ExportSymbolDialog(this, filefilter);
                dlg.ShowDialog(this);
            }
            catch (System.Exception e1)
            {
                MessageBox.Show(e1.Message);
            }
        }
        #endregion

        #region ..导入
        /// <summary>
        /// 导入SVG代码
        /// </summary>
        /// <param name="svgFragment">SVG代码</param>
        internal void LoadSVG(string svgFragment)
        {
            if (this.svgDocument != null)
            {
                if (this.currentOperation != null)
                    this.currentOperation.Reset();
                this.svgDocument.LoadXml(svgFragment);
                this.Invalidate();
            }
        }

        /// <summary>
        /// 导入指定的SVG文档
        /// </summary>
        /// <param name="filename">ＳＶＧ文档路径</param>
        internal void LoadFile(string filename)
        {
            this.FilePath = filename;
        }
        #endregion

        #region ..将当前内容绘制到指定的Graphics上
        /// <summary>
        /// 将当前绘制内容绘制到指定的绘图表面上
        /// 通过RenderTo方法，您可以自由处理VectorControl的内容，比如说预览等。
        /// </summary>
        /// <param name="g">Graphics对象</param>
        /// <example>
        /// <code>
        /// Bitmap bmp = new Bitmap(500,400);
        /// using(System.Drawing.Graphics g = Graphics.FromImage(bmp))
        /// {
        ///		g.SmoothingMode = SmoothingMode.HighQuality;
        ///		g.TranslateTransform(50,50);
        ///		this.vectorControl.RenderTo(g);
        /// }
        /// </code>
        /// </example>
        internal void RenderTo(Graphics g)
        {
            if (g == null)
                return;
            this.validContent = false;
            if (this.svgDocument != null && this.svgDocument.DocumentElement != null)
            {
                SVGTransformableElement svg = this.svgDocument.CurrentScene as SVGTransformableElement;
                if (svg != null)
                {
                    using (SVG.StyleContainer.StyleOperator sp = this.svgDocument.CreateStyleOperator())
                    {
                        sp.ClipRegion.MakeEmpty();
                        svg.TotalTransform.Reset();
                        svg.SVGRenderer.Draw(g, sp);
                    }
                }
            }
            this.validContent = true;
        }
        #endregion

		#region ..OnScaleChanged
		protected virtual void OnScaleChanged()
		{
			if(this.ScaleChanged != null)
				this.ScaleChanged(this,EventArgs.Empty);
		}
		#endregion

		#region ..处理属性框
		//		private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		//		{
		//			if(this.svgDocument == null)
		//				return;
		//			this.svgDocument.InvokeUndos();
		//			this.Invalidate();
		//		}
		#endregion		

		#region ..WndProc
        bool EnableTearFreeWhenScroll = false;
		protected override void WndProc(ref Message m)
		{
            if (m.Msg == (int)Msgs.WM_HSCROLL)
            {
                if (this.rule.Visible)
                    this.Invalidate(new Rectangle(0, 0, this.ruleLength, this.Height - SystemInformation.HorizontalScrollBarHeight));
                scrolled = true;
                base.WndProc(ref m);
                if (EnableTearFreeWhenScroll)
                {
                    mouseDown = true;
                    this.PaintContent(true);
                    this.Update();
                    if ((uint)m.WParam.ToInt64() == SB_ENDSCROLL)
                        mouseDown = false;
                }
            }
            else if (m.Msg == (int)Msgs.WM_VSCROLL || m.Msg == (int)Msgs.WM_MOUSEWHEEL)
            {
                if (this.rule.Visible)
                    this.Invalidate(new Rectangle(0, 0, this.Width - SystemInformation.VerticalScrollBarWidth, this.ruleLength));

                scrolled = true;
                base.WndProc(ref m);
                if (EnableTearFreeWhenScroll)
                {
                    mouseDown = true;
                    this.PaintContent(true);
                    this.Update();
                    if ((uint)m.WParam.ToInt64() == SB_ENDSCROLL)
                        mouseDown = false;
                }
            }
			else
                base.WndProc(ref m);

		}

		protected override bool ProcessKeyEventArgs(ref Message m)
		{
			try
			{
				//seems like that this is a bug in .Net 2.0,this logic is to prevent two keypressevent occurs in IME_Char
				this.createKeyPressEvent = true;
				if(m.Msg == (int)Msgs.WM_KEYDOWN)
					this.imeChars.Clear();
				if(m.Msg == (int)Msgs.WM_IME_CHAR)
				{
					this.imeChars.Add(m);
				}
				else if(m.Msg == (int)Msgs.WM_CHAR && this.imeChars.Count > 0)
				{
					Message m1 = (Message)this.imeChars[0];
					//compare 
					if(m1.HWnd == m.HWnd && m1.LParam == m.LParam && m1.WParam == m.WParam)
					{
						this.createKeyPressEvent = false;
						this.imeChars.RemoveAt(0);
					}
					else
						this.imeChars.Clear();
				}
				return base.ProcessKeyEventArgs (ref m);
			}
			finally
			{
				this.createKeyPressEvent = true;
			}
		}

		#endregion

		#region ..长度转换
		/// <summary>
		/// 将长度转换为象素表示
		/// </summary>
		/// <param name="length">长度</param>
		/// <param name="type">度量单位</param>
		/// <returns></returns>
		internal static float LengthToPixel(float length,UnitType type)
		{
			switch(type)
			{
				case UnitType.Centimeter:
					length = length * 35.43307F;
					break;
				case UnitType.Millimetre :
					length = length * 3.543307F;
					break;
				case UnitType.Inch:
					length = length * 90;
					break;
				case UnitType.Point:
					length = length * 1.25F;
					break;
			}
			return length;
		}

		/// <summary>
		/// 将象素值转换指定长度值
		/// </summary>
		/// <param name="length"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		internal static float PixelToLength(float length,UnitType type)
		{
			switch(type)
			{
				case UnitType.Centimeter:
					length = length / 35.43307F;
					break;
				case UnitType.Millimetre :
					length = length / 3.543307F;
					break;
				case UnitType.Inch:
					length = length / 90f;
					break;
				case UnitType.Point:
					length = length / 1.25F;
					break;
			}
			return length;
		}
		#endregion

		#region ..创建标尺度量上下文菜单
		ContextMenu CreateRuleContextMenu()
		{
            //System.Windows.Forms.ContextMenu menu = new ContextMenu();
            //System.EventHandler click = new EventHandler(selectUnit);
            //System.EventHandler popup = new EventHandler(popupUnit);
            //menu.Popup += popup;
            //System.Windows.Forms.MenuItem item = new MenuItem("象素");
            //item.Click += click;
            //menu.MenuItems.Add(item);
            //item = new MenuItem("点");
            //item.Click += click;
            //menu.MenuItems.Add(item);
            //item = new MenuItem("英寸");
            //item.Click += click;
            //menu.MenuItems.Add(item);
            //item = new MenuItem("厘米");
            //item.Click += click;
            //menu.MenuItems.Add(item);
            //item = new MenuItem("毫米");
            //item.Click += click;
            //menu.MenuItems.Add(item);
			return null;
		}
		#endregion

		#region ..获取刻度文本表达
		string GetUnitString(UnitType type)
		{
			
			switch(type)
			{
				case UnitType.Inch:
					return "英寸";
				case UnitType.Millimetre:
					return "毫米";
				case UnitType.Point:
					return "点";
				case UnitType.Centimeter:
					return "厘米";
			}
			return "象素";
		}
		#endregion

		#region ..判断点是否在标尺区域
		internal bool InRule(Point p)
		{
			if(this.rule.Visible)
				return p.X <= this.ruleLength || p.Y <= this.ruleLength;
			return false;
		}
		#endregion

        #region ..UpdateFill
        /// <summary>
		/// 更新填充
		/// </summary>
		void UpdateFill()
		{
			if(this.svgDocument.SelectCollection.Count == 0)
				return;
			bool old = this.svgDocument.AcceptNodeChanged;
			this.svgDocument.AcceptNodeChanged = true;
			string color = SVG.ColorHelper.GetColorStringInHex(this.fill.Color).ToLower();
			string opacity = this.fill.Opacity.ToString();
			ArrayList list = new ArrayList(new string[]{"rect","use","polygon","circle","text","tspan","ellipse","path","polyline"});

			for(int i = 0;i<this.svgDocument.SelectCollection.Count;i++)
			{
				SVG.SVGElement element = this.svgDocument.SelectCollection[i] as SVG.SVGElement;
				this.UpdateElementAttribute(element,"none","fill",color,list);
				this.UpdateElementAttribute(element,"1","fill-opacity",opacity,list);
			}
			list.Clear();
			list = null;
			this.svgDocument.AcceptNodeChanged = old;
			this.svgDocument.InvokeUndos();
			color = null;
			opacity = null;
		}
		#endregion

        #region ..UpdateStroke
        void UpdateStroke()
		{
			if(this.svgDocument.SelectCollection.Count == 0)
				return;
			bool old = this.svgDocument.AcceptNodeChanged;
			this.svgDocument.AcceptNodeChanged = true;
			string color = SVG.ColorHelper.GetColorStringInHex(this.stroke.Color).ToLower();
			string opacity = this.stroke.Opacity.ToString();
			string width = this.stroke.Width.ToString();
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			if(stroke.DashPattern != null && stroke.DashPattern.Length > 0)
			{
				for(int i = 0;i<stroke.DashPattern.Length;i++)
				{
					sb.Append(stroke.DashPattern[i].ToString() +" ");
				}
			}
			ArrayList list = new ArrayList(new string[]{"rect","use","polygon","circle","text","tspan","ellipse","path","line","polyline","connect"});
			for(int i = 0;i<this.svgDocument.SelectCollection.Count;i++)
			{
				SVG.SVGElement element = this.svgDocument.SelectCollection[i] as SVG.SVGElement;
				this.UpdateElementAttribute(element,"black","stroke",color,list);
				this.UpdateElementAttribute(element,"1","stroke-opacity",opacity,list);				
				this.UpdateElementAttribute(element,"1","stroke-width",width,list);
				this.UpdateElementAttribute(element,"none","stroke-dasharray",sb.Length == 0?"none":sb.ToString(),list);
			}
			this.svgDocument.AcceptNodeChanged = old;
			this.svgDocument.InvokeUndos();
			this.InvalidateSelection();
			color = null;
			opacity = null;
			sb = null;
			width = null;
		}
		#endregion

        #region ..UpdateTextStyle
        void UpdateTextStyle()
        {
            this.UpdateTextStyle(this.svgDocument.SelectCollection, true);
        }

		void UpdateTextStyle(SVGElementCollection elements, bool invoke)
		{
            if (elements == null || elements.Count == 0)
				return;
			bool old = this.svgDocument.AcceptNodeChanged;
            this.svgDocument.AcceptNodeChanged = invoke;
			string fontname = this.textStyle.FontName;
			string size = this.textStyle.Size.ToString();
			string bold = this.textStyle.Bold?"bold":"normal";
			string italic = this.textStyle.Italic ? "italic":"normal";
			string underline = this.textStyle.Underline?"underline":"normal";
			bool change = false;
			this.InvalidateSelection();
			float size1 = this.textStyle.Size ;
			int r = grapSize / 2;
			ArrayList list = new ArrayList(new string[]{"text","textBlock","use","line","circle","rect","ellipse","path","image","polyline","polygon","connect"});
			using(Graphics g = this.CreateGraphics())
			{
				using(System.Drawing.Drawing2D.GraphicsPath path = new GraphicsPath())
				{
					for(int i = 0;i<elements.Count;i++)
					{
						bool change1 = false;
                        SVG.SVGElement element = elements[i] as SVG.SVGElement;
						change1 = this.UpdateElementAttribute(element,string.Empty,"font-family",fontname,list) || change1;
						change1 = this.UpdateElementAttribute(element,"12","font-size",size,list) || change1;
						change1 =  this.UpdateElementAttribute(element,"normal","font-weight",bold,list) || change1;
						change1 = this.UpdateElementAttribute(element,"normal","font-style",italic,list) || change1;
						change1 = this.UpdateElementAttribute(element,"normal","text-decoration",underline,list) || change1;
						change = change || change1;
					}
				
					if(change && invoke)
					{
						this.selectChanged = true;
						this.UpdateSelectInfo();
						this.InvalidateSelection();
					}
				}
			}
			this.svgDocument.AcceptNodeChanged = old;
            if(invoke)
			this.svgDocument.InvokeUndos();
			fontname = null;
			bold = null;
			italic = null;
			underline = null;
			list = null;
		}
		#endregion

		#region ..更新对象属性
		internal bool UpdateElementAttribute(SVG.SVGElement element,string defaultvalue,string attributename,string attributeValue,ArrayList validNames)
		{
			return SVG.SVGElement.UpdateElementAttribute(element,defaultvalue,attributename,attributeValue,validNames);
		}
		#endregion

		#region ..New
		/// <summary>
		/// 开始新绘制
		/// </summary>
		internal void New(SVG.DataType.SVGLength width, SVG.DataType.SVGLength height) 
		{
            System.Resources.ResourceManager rm = new System.Resources.ResourceManager(Type.GetType("YP.Canvas.Canvas"));
            string template = rm.GetString("svgtemplate");
            this.SVGDocument = SVG.Document.SVGDocument.CreateDocumentWithSize(width, height, template);
			this._filepath = string.Empty;
		}
		#endregion

        #region ..ChangeToTextEdit
        internal Operation.Operation ChangeToTextEdit(SVG.SVGTransformableElement element)
        {
            return ChangeToTextEdit(element, null);
        }

		internal Operation.Operation ChangeToTextEdit(SVG.SVGTransformableElement element, PointF? viewPoint)
		{
			if(this.currentOperation != null)
				this.currentOperation.Dispose();
			if(element is SVG.Text.SVGTextElement)
			{
				this.currentOperation = new Operation.Text.TextOperation(this,element as SVG.Text.SVGTextElement);
				this.CurrentOperator = Operator.Text;
                return this.currentOperation;
			}
			else
			{
                if (element is SVG.Interface.ISVGTextBlockContainer || element is SVG.Text.SVGTextBlockElement)
                {
                    SVG.Text.SVGTextBlockElement textBlock = element as SVG.Text.SVGTextBlockElement;
                    if (textBlock == null)
                    {
                        foreach (SVGElement elm in element.ChildElements)
                        {
                            if (elm is SVG.Text.SVGTextBlockElement)
                            {
                                textBlock = elm as SVG.Text.SVGTextBlockElement;
                                break;
                            }
                        }
                    }

                    //如果没有textBlock对象，添加
                    if (textBlock == null)
                    {
                        SVG.Document.SVGDocument doc = this.svgDocument;
                        bool old = doc.AcceptNodeChanged;
                        doc.AcceptNodeChanged = false;
                        textBlock = element.CreateTextBlockForLabel();
                        SVGElementCollection list = new SVGElementCollection(new SVGElement[] { textBlock });
                        this.UpdateTextBlockStyle(list, false);
                        this.UpdateTextStyle(list, false);
                        doc.AcceptNodeChanged = true;
                        
                        element.InternalAppendChild(textBlock);
                        doc.InvokeUndos();
                    }
                    Operator op = this.CurrentOperator;
                    this.InvalidateElement(element as SVG.Interface.ISVGPathable);
                    this.currentOperation = new Operation.LabelText.LabelTextOperation(this, textBlock,op, viewPoint);
                    this.CurrentOperator = Operator.TextBlock;
                    return this.currentOperation;
                }
			}

            return null;
		}
		#endregion

        #region ..InitializeTextBlock
        #endregion

        #region ..切换到Transform模式
        internal void ChangeToTransform()
		{
			if(this.currentOperation != null)
				this.currentOperation.Dispose();
            this.selectChanged = true;
            this.UpdateSelectInfo();
			this.CurrentOperator = Operator.Transform;// = new Operation.SelectTransformOperation(this);
			
		}
		#endregion

		#region ..OnOperatorChanged
        /// <summary>
        /// 引发OperatorChanged事件
        /// </summary>
		protected virtual void OnOperatorChanged()
		{
			if(this.OperatorChanged != null)
				this.OperatorChanged(this,EventArgs.Empty);
		}
		#endregion

		#region ..更新箭头
		void UpdateStartArrow()
		{
			if(this.svgDocument.SelectCollection.Count == 0)
				return;
			bool old = this.svgDocument.AcceptNodeChanged;
			this.svgDocument.AcceptNodeChanged = true;
			string id = string.Empty;
			string fullid = string.Empty;
			if(this.startArrow != null)
			{
                id = this.startArrow.MarkerElement.GetAttribute("id");
				fullid = "url(#"+id+")";
			}
			
			ArrayList list = new ArrayList(new string[]{"line","polyline","path","connect"});
			bool change = false;
            if (fullid.Length > 0)
            {
                SVG.ClipAndMask.SVGMarkerElement marker = this.svgDocument.GetReferencedNode(id, new string[] { "marker" }) as SVG.ClipAndMask.SVGMarkerElement;
                if (marker == null && this.startArrow != null && this.startArrow.MarkerElement is SVG.ClipAndMask.SVGMarkerElement)
                {
                    marker = this.svgDocument.AddDefsElement(this.startArrow.MarkerElement as SVG.SVGElement) as SVG.ClipAndMask.SVGMarkerElement;
                    marker.InternalSetAttribute("id", id);
                }
            }

			for(int i = 0;i<this.svgDocument.SelectCollection.Count;i++)
			{
				SVG.SVGElement element = this.svgDocument.SelectCollection[i] as SVG.SVGElement;
				change = this.UpdateElementAttribute(element,string.Empty,"marker-start",fullid,list) || change;
			}
			
			id = null;
			this.svgDocument.InvokeUndos();
			this.svgDocument.AcceptNodeChanged = old;
			this.InvalidateSelection();
			list.Clear();
			list = null;
		}

		/// <summary>
		/// 更新末尾箭头
		/// </summary>
		void UpdateEndArrow()
		{
			if(this.svgDocument.SelectCollection.Count == 0)
				return;
			bool old = this.svgDocument.AcceptNodeChanged;
			this.svgDocument.AcceptNodeChanged = true;
			string id = string.Empty;
			string fullid = string.Empty;
			if(this.endArrow != null)
			{
                id = "end" + this.endArrow.MarkerElement.GetAttribute("id");
				fullid = "url(#"+id+")";
			}
		
			ArrayList list = new ArrayList(new string[]{"line","polyline","path","connect"});
			bool change = false;

            if (fullid.Length > 0)
            {
                SVG.ClipAndMask.SVGMarkerElement marker = this.svgDocument.GetReferencedNode(id, new string[] { "marker" }) as SVG.ClipAndMask.SVGMarkerElement;
                if (marker == null && this.endArrow != null && this.endArrow.MarkerElement is SVG.ClipAndMask.SVGMarkerElement)
                {
                    marker = this.svgDocument.AddDefsElement(this.endArrow.MarkerElement as SVG.SVGElement) as SVG.ClipAndMask.SVGMarkerElement;
                    this.svgDocument.AcceptNodeChanged = false;
                    for (int j = 0; j < marker.ChildElements.Count; j++)
                    {
                        SVG.SVGElement e1 = marker.ChildElements[j] as SVG.SVGElement;
                        if (e1 != null)
                        {
                            string a = e1.GetAttribute("transform");
                            a = "matrix(-1,0,0,1,0,0) " + a;
                            e1.InternalSetAttribute("transform", a);
                            a = null;
                        }
                    }
                    marker.InternalSetAttribute("id", id);
                    this.svgDocument.AcceptNodeChanged = old;
                }
            }

            for(int i = 0;i<this.svgDocument.SelectCollection.Count;i++)
			{
				SVG.SVGElement element = this.svgDocument.SelectCollection[i] as SVG.SVGElement;
				change = this.UpdateElementAttribute(element,string.Empty,"marker-end",fullid,list) || change;
			}
			
			id = null;
			this.svgDocument.InvokeUndos();
			this.svgDocument.AcceptNodeChanged =old;
			this.InvalidateSelection();
		}
		#endregion

		#region ..用指定的变换更新对象
		void TransformElement(SVG.SVGTransformableElement transform,Matrix matrix1)
		{
			if(transform is SVG.BasicShapes.SVGConnectionElement)
				return;
			using(Matrix matrix = transform.Transform.FinalMatrix.GetGDIMatrix(),total = this.GetTotalTransformForElement(transform),total1 = this.GetTotalTransformForElement(transform))
			{
				total1.Invert();
				total.Multiply(matrix1,MatrixOrder.Append);
				total.Multiply(total1,MatrixOrder.Append);
				total.Multiply(matrix,MatrixOrder.Append);
                this.UpdateElement(transform, total);
			}
		}
		#endregion

		#region ..用指定的变换更新对象
		internal bool UpdateElement(SVG.SVGStyleable style,Matrix matrix)
		{
			if(style is SVG.BasicShapes.SVGLineElement)
			{
				SVG.BasicShapes.SVGLineElement line = style as SVG.BasicShapes.SVGLineElement;
				float x1 = line.X1.Value;
				float y1 = line.Y1.Value;
				float x2 = line.X2.Value;
				float y2 = line.Y2.Value;
				PointF[] ps = new PointF[]{new PointF(x1,y1),new PointF(x2,y2)};
				matrix.TransformPoints(ps);
				line.InternalSetAttribute("x1",ps[0].X.ToString());
				line.InternalSetAttribute("y1",ps[0].Y.ToString());
				line.InternalSetAttribute("x2",ps[1].X.ToString());
				line.InternalSetAttribute("y2",ps[1].Y.ToString());
				line.InternalSetAttribute("transform",string.Empty);
				ps = null;
				return false;
			}
            else if (style is SVG.BasicShapes.SVGPolylineElement)
            {
                SVG.BasicShapes.SVGPolylineElement line = style as SVG.BasicShapes.SVGPolylineElement;
                PointF[] ps = ((SVG.DataType.SVGPointList)line.Points).GetGDIPoints();
                if (ps.Length > 0)
                {
                    matrix.TransformPoints(ps);
                    System.Text.StringBuilder sb = new System.Text.StringBuilder(100);
                    for (int i = 0; i < ps.Length; i++)
                    {
                        sb.Append(ps[i].X.ToString() + " " + ps[i].Y.ToString() + " ");
                    }
                    style.InternalSetAttribute("points", sb.ToString());
                    style.InternalSetAttribute("transform", string.Empty);
                    sb.Remove(0, sb.Length);
                    sb = null;
                }
                ps = null;
                return false;
            }
            else
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("matrix(");
                sb.Append(matrix.Elements[0].ToString() + ",");
                sb.Append(matrix.Elements[1].ToString() + ",");
                sb.Append(matrix.Elements[2].ToString() + ",");
                sb.Append(matrix.Elements[3].ToString() + ",");
                sb.Append(matrix.Elements[4].ToString() + ",");
                sb.Append(matrix.Elements[5].ToString() + ")");
                style.InternalSetAttribute("transform", sb.ToString());
                sb.Remove(0, sb.Length);
                sb = null;
            }
			return true;
		}
		#endregion

		#region ..DragDrop
		//		protected override void OnDragDrop(DragEventArgs drgevent)
		//		{
		//			if(this.HasSnap && Control.MouseButtons== MouseButtons.Left)
		//			{
		//				Point p = this.PointToScreen(Point.Round(this.GetPoint(new Point(e.X,e.Y))));
		//				base.OnDragDrop(new DragEventArgs(drgevent.Data,drgevent.KeyState,p.X,p.Y,drgevent.AllowedEffect,drgevent.Effect));
		//			}
		//			else
		//			{
		//				base.OnDragDrop (drgevent);
		//			}
		//			
		//		}
		//
		//		protected override void OnDragOver(DragEventArgs drgevent)
		//		{
		//			if(this.HasSnap && Control.MouseButtons == MouseButtons.Left)
		//			{
		//				Point p = this.PointToScreen(Point.Round(this.GetPoint(new Point(e.X,e.Y))));
		//				base.OnDragOver(new DragEventArgs(drgevent.Data,drgevent.KeyState,p.X,p.Y,drgevent.AllowedEffect,drgevent.Effect));
		//			}
		//			else
		//			{
		//				base.OnDragOver (drgevent);
		//			}
		//		}

		#endregion

		#region ..GetSnapPoint
		internal PointF GetSnapPoint(int index)
		{
			if(index >= 0 && index < this.boundPoints.Length)
				return this.boundPoints[index];
			return PointF.Empty;
		}
		#endregion

		#region ..InvalidateConnects
        //internal void InvalidateConnects(SVGDom.SVGTransformableElement element)
        //{
        //    if(element == null)
        //        return;
        //    SVGDom.BasicShapes.SVGConnectElement[] cs = element.Connects;
        //    if(cs != null && cs.Length > 0)
        //    {
        //        for(int i = 0;i<cs.Length;i++)
        //        {
        //            this.InvalidateElement(cs[i]);
        //        }
        //    }
        //    element.UpdateConnects();
        //    if(cs != null && cs.Length > 0)
        //    {
        //        for(int i = 0;i<cs.Length;i++)
        //        {
        //            this.InvalidateElement(cs[i]);
        //        }
        //    }
        //}
		#endregion

		#region ..UpdateConnect
        //internal void UpdateConnect(SVGDom.DocumentStructure.SVGGElement render)
        //{
        //    for(int i = 0;i<render.childRenders.Count;i++)
        //    {
        //        if(render.childRenders[i] is SVGDom.SVGTransformableElement)
        //            this.InvalidateConnects(render.childRenders[i] as SVGDom.SVGTransformableElement);
        //        else if(render.childRenders[i] is SVGDom.DocumentStructure.SVGGElement)
        //            this.UpdateConnect(render.childRenders[i] as SVGDom.DocumentStructure.SVGGElement);
        //    }
        //}
		#endregion

        #region ..InSelectedBounds
        internal bool InPath(GraphicsPath path, Point point)
        {
            bool contains = false;
            using (Pen pen = new Pen(Color.Blue, PenWidth))
            {
                contains = path.IsVisible(point);
                if (!contains && path.PointCount < MaxPathPoint)
                    contains = path.IsOutlineVisible(point, pen);
            }
            return contains;
        }
        #endregion

        #region ..SelectElementsInPoint
        void SelectElementsInPoint(Point p)
		{
			SVG.SVGElementCollection list = this.svgDocument.SelectCollection;
			if(list.Count > 0)
			{
				using(GraphicsPath path1 = this.selectpath.Clone() as GraphicsPath)
				{
					path1.Transform(this.selectMatrix);
                    if (InPath(path1, p))
                        return;
				}
			}
			list.Clear();
            var element = this.GetElementAtLocation(p,false);
            if (element != null)
                this.svgDocument.ChangeSelectElement(element);
		}
		#endregion

		#region ..GetPointString
		string GetPointString(PointF[] ps)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder(100);
			for(int i = 0;i<ps.Length;i++)
			{
				sb.Append( " "+ps[i].X.ToString() + " "+ps[i].Y.ToString());
			}
			return sb.ToString();
		}
		#endregion

		#region ..将指定节点导入到Symbol中
		void ExportElementToSymbol(SVG.SVGElement symbol,SVG.SVGElement element,SVG.SVGElementCollection refNodes)
		{
			#region ..组对象
			//组对象
			if(element is SVG.DocumentStructure.SVGGElement)
			{
				string transform = element.GetAttribute("transform");
				SVG.SVGElementCollection list = (element as SVG.DocumentStructure.SVGGElement).ChildElements;
				for(int i = 0;i<list.Count;i++)
				{
					if(transform.Trim().Length > 0)
					{
						string transform1 = (list[i] as SVG.SVGElement).GetAttribute("transform") + " " + transform;
						(list[i] as SVG.SVGElement).InternalSetAttribute("transform",transform1);
						transform1 = null;
					}
					this.ExportElementToSymbol(symbol,list[i] as SVG.SVGElement,refNodes);
				}
				element.InternalRemoveAttribute("transform");
				transform = null;
			}
				#endregion 

				#region ..PointsElement
			else if(element is SVG.BasicShapes.SVGPointsElement)
			{
				string transform = element.GetAttribute("transform");
				bool valid = false;
				if(transform.Trim().Length > 0)
				{
					SVG.DataType.SVGTransformList ts = new SVG.DataType.SVGTransformList(transform);
					using(System.Drawing.Drawing2D.Matrix matrix = ts.FinalMatrix.GetGDIMatrix())
					{
						if(!matrix.IsIdentity && (matrix.Elements[0] > 1 || matrix.Elements[3] > 1))
						{
							PointF[] ps = ((element as SVG.BasicShapes.SVGPointsElement).Points as SVG.DataType.SVGPointList).GetGDIPoints();
							if(ps != null && ps.Length > 0)
							{
								matrix.TransformPoints(ps);
								element.InternalSetAttribute("points",this.GetPointString(ps));
								valid = true;
							}
							ps = null;
						}
					}
					if(valid)
						element.InternalRemoveAttribute("transform");
				}
			}
				#endregion

				#region ..直线
			else if(element is SVG.BasicShapes.SVGLineElement)
			{
				SVG.BasicShapes.SVGLineElement line = element as SVG.BasicShapes.SVGLineElement;
				string transform = element.GetAttribute("transform");
				bool valid = false;
				if(transform.Trim().Length > 0)
				{
					SVG.DataType.SVGTransformList ts = new SVG.DataType.SVGTransformList(transform);
					using(System.Drawing.Drawing2D.Matrix matrix = ts.FinalMatrix.GetGDIMatrix().Clone())
					{
						if(!matrix.IsIdentity && (matrix.Elements[0] >1 || matrix.Elements[3] > 1))
						{
							PointF[] ps = new PointF[]{new PointF(line.X1.Value,line.Y1.Value),new PointF(line.X2.Value,line.Y2.Value)};
							matrix.TransformPoints(ps);
							line.InternalSetAttribute("x1",ps[0].X.ToString());
							line.InternalSetAttribute("y1",ps[0].Y.ToString());
							line.InternalSetAttribute("x2",ps[1].X.ToString());
							line.InternalSetAttribute("y2",ps[1].Y.ToString());
							ps = null;
							valid = true;
						}
					}
				}
				transform = null;
				if(valid)
					element.InternalRemoveAttribute("transform");
			}
				#endregion

				#region ..连接线
			else if(element is SVG.BasicShapes.SVGConnectionElement)
			{
				SVG.BasicShapes.SVGConnectionElement connect = element as SVG.BasicShapes.SVGConnectionElement;
				SVG.SVGElement temp = (connect as SVG.Interface.BasicShapes.ISVGBasicShape).ConvertToPath() as SVG.SVGElement;
				if(connect.ParentElement != null)
                    connect.ParentElement.InternalReplaceChild(temp, connect);
				temp.InternalSetAttribute("fill","none");
			}
				#endregion

				#region ..曲线
			else if(element is SVG.Paths.SVGPathElement)
			{
				SVG.Paths.SVGPathElement path = element as SVG.Paths.SVGPathElement;
				string transform = element.GetAttribute("transform");
				bool valid = false;
				if(transform.Trim().Length > 0)
				{
					SVG.DataType.SVGTransformList ts = new SVG.DataType.SVGTransformList(transform);
					using(System.Drawing.Drawing2D.Matrix matrix = ts.FinalMatrix.GetGDIMatrix().Clone())
					{
						if(!matrix.IsIdentity && (matrix.Elements[0] > 1 || matrix.Elements[3] > 1))
						{
							path.TransformSegs(matrix);
							path.InternalSetAttribute("d",path.PathSegList.PathString);
							valid = true;
						}
					}
				}
				if(valid)
					element.InternalRemoveAttribute("transform");
			}
				#endregion

				#region ..栅格图像
			else if(element is SVG.DocumentStructure.SVGImageElement)
			{
				SVG.DocumentStructure.SVGImageElement image = element as SVG.DocumentStructure.SVGImageElement;
				image.ConvertImageTo64();
			}
				#endregion
			
				#region ..其他直接绘制对象
			else
			{
				string transform = element.GetAttribute("transform");
				if(transform.Trim().Length >0)
				{
					using(System.Drawing.Drawing2D.Matrix matrix = (element as SVG.SVGTransformableElement).Transform.FinalMatrix.GetGDIMatrix())
					{
						if(matrix.Elements[0] > 1 || matrix.Elements[3] > 1)
						{
							float a = 1;
							a = (element as SVG.SVGTransformableElement).StrokeStyle.strokewidth.Value;
							if(a == 0)
								a = 1;
							float scale = (float)Math.Max(matrix.Elements[0],matrix.Elements[3]);
							a = a / scale;
							element.InternalSetAttribute("stroke-width",a.ToString());
						}
					}
				}

				//文本
                //if(element is SVGDom.Text.SVGTextElement)
                //{
                //    (element as SVGDom.Text.SVGTextElement).UpdateText();
                //}
			}
			#endregion

			//添加到Symbol
			if(element is SVG.SVGTransformableElement)
			{
				symbol.InternalAppendChild(element);
				symbol.InternalAppendChild(this.svgDocument.CreateTextNode("\n"));
				element.InternalRemoveAttribute("id");
				SVG.SVGTransformableElement render = element as SVG.SVGTransformableElement;
				if(render.MarkerStart != null && !refNodes.Contains(render.MarkerStart))
					refNodes.Add(render.MarkerStart);
				if(render.MarkerEnd != null && !refNodes.Contains(render.MarkerEnd))
					refNodes.Add(render.MarkerEnd);
				if(element is SVG.DocumentStructure.SVGUseElement)
				{
					SVG.DocumentStructure.SVGUseElement use = element as SVG.DocumentStructure.SVGUseElement;
					if(use.InstanceRoot != null && use.InstanceRoot.CorrespondingElement != null && !refNodes.Contains(use.InstanceRoot.CorrespondingElement))
						refNodes.Add(use.InstanceRoot.CorrespondingElement);
				}
			}
		}
		#endregion

		#region ..导出图元
		internal SVG.SVGElement ExportSymbolElement(bool wholecontent,bool exportshape,bool createdocument)
		{
			if(this.svgDocument == null || !this.svgDocument.IsValid)
				return null;
			bool old = this.svgDocument.AcceptNodeChanged;
			this.svgDocument.AcceptNodeChanged = false;
			SVG.SVGElement mainElement = null;
			if(exportshape)
			{
				mainElement = this.svgDocument.CreateElement(this.svgDocument.Prefix,"path",this.svgDocument.NamespaceURI) as SVG.SVGElement;
                SVG.SVGElementCollection list = (this.svgDocument.RootElement as SVG.DocumentStructure.SVGGElement).ChildElements;
				if(!wholecontent)
					list = this.svgDocument.SelectCollection;
				System.Text.StringBuilder sb = new System.Text.StringBuilder(1000);
				for(int i = 0;i<list.Count;i++)
				{
					this.ExportPath((list[i] as SVG.SVGElement).Clone() as SVG.SVGElement,sb);
				}
				mainElement.InternalSetAttribute("d",sb.ToString());
				sb = null;
			}
			else
			{
				mainElement = this.svgDocument.CreateElement(this.svgDocument.Prefix,"symbol",this.svgDocument.NamespaceURI) as SVG.SVGElement;
                SVG.SVGElementCollection list = (this.svgDocument.RootElement as SVG.DocumentStructure.SVGSVGElement).ChildElements;
				if(!wholecontent)
					list = this.svgDocument.SelectCollection;
				SVG.SVGElementCollection refNodes = new SVGElementCollection();
				for(int i = 0;i<list.Count;i++)
				{
					this.ExportElementToSymbol(mainElement,(list[i] as SVG.SVGElement).Clone() as SVG.SVGElement,refNodes);
				}
				if(refNodes.Count>0)
				{
					SVG.SVGElement defs = this.svgDocument.CreateElement(this.svgDocument.Prefix,"defs",this.svgDocument.NamespaceURI) as SVG.SVGElement;
					mainElement.InternalPrependChild(defs);
					mainElement.InternalPrependChild(this.svgDocument.CreateTextNode("\n"));
					mainElement.InternalInsertAfter(this.svgDocument.CreateTextNode("\n"),defs);
					defs.InternalAppendChild(this.svgDocument.CreateTextNode("\n"));
					for(int i = 0;i<refNodes.Count;i++)
					{
						SVG.SVGElement refnode = refNodes[i] as SVG.SVGElement;
						refnode = refnode.Clone() as SVG.SVGElement;

						defs.InternalAppendChild(refnode);
						defs.InternalAppendChild(this.svgDocument.CreateTextNode("\n"));
					}
				}
				mainElement.InternalSetAttribute("overflow","visible");
			}

			if(createdocument)
			{
				SVG.SVGElement root = (this.svgDocument.RootElement as SVG.SVGElement).CloneNode(false) as SVG.SVGElement;
				if(mainElement != null)
				{
					root.InternalAppendChild(this.svgDocument.CreateTextNode("\n"));
					root.InternalAppendChild(mainElement);
					root.InternalAppendChild(this.svgDocument.CreateTextNode("\n"));
				}
				mainElement = root;
			}
			this.svgDocument.AcceptNodeChanged = old;
			return mainElement;
		}
		#endregion

		#region ..导出路径
		void ExportPath(SVG.SVGElement element,System.Text.StringBuilder sb)
		{
			#region ..组对象
			//组对象
			if(element is SVG.DocumentStructure.SVGGElement)
			{
				string transform = element.GetAttribute("transform");
				SVG.SVGElementCollection list = (element as SVG.DocumentStructure.SVGGElement).ChildElements;
				for(int i = 0;i<list.Count;i++)
				{
					if(transform.Trim().Length > 0)
					{
						string transform1 = (list[i] as SVG.SVGElement).GetAttribute("transform") + " " + transform;
						(list[i] as SVG.SVGElement).InternalSetAttribute("transform",transform1);
						transform1 = null;
					}
					this.ExportPath(list[i] as SVG.SVGElement,sb);
				}
				transform = null;
			}
				#endregion 

				#region ..曲线
			else if(element is SVG.Paths.SVGPathElement)
			{
				SVG.Paths.SVGPathElement path = element as SVG.Paths.SVGPathElement;
				using(System.Drawing.Drawing2D.Matrix matrix = path.Transform.FinalMatrix.GetGDIMatrix())
				{
					if(matrix.IsInvertible)
						path.TransformSegs(matrix);
				}
				sb.Append(path.PathData.PathString);
			}
				#endregion

				#region ..PointsElement
			else if(element is SVG.BasicShapes.SVGPointsElement)
			{
				PointF[] ps = ((element as SVG.BasicShapes.SVGPointsElement).Points as SVG.DataType.SVGPointList).GetGDIPoints();
				using(System.Drawing.Drawing2D.Matrix matrix = (element as SVG.SVGTransformableElement).Transform.FinalMatrix.GetGDIMatrix())
				{
					if(matrix.IsInvertible)
						matrix.TransformPoints(ps);
						
				}
				if(ps.Length >0)
					sb.Append("M" + ps[0].X.ToString() + " " + ps[1].Y .ToString());
				for(int i = 1;i<ps.Length;i++)
				{
					sb.Append("L" + ps[0].X.ToString() + " " + ps[1].Y .ToString());
				}
				if(element is SVG.BasicShapes.SVGPolygonElement)
					sb.Append("Z");
			}
				#endregion

				#region ..可绘制对象
			else if(element is SVG.Interface.BasicShapes.ISVGBasicShape)
			{
				SVG.SVGTransformableElement render = element as SVG.SVGTransformableElement;
				using(System.Drawing.Drawing2D.GraphicsPath path = (render as SVG.Interface.ISVGPathable).GPath)
				{
					using(System.Drawing.Drawing2D.Matrix matrix = render.Transform.FinalMatrix.GetGDIMatrix())
					{
						if(matrix.IsInvertible)
							path.Transform(matrix);
					}
					sb.Append(SVG.Paths.SVGPathElement.GetPathString(path));
				}
				
			}
			#endregion
		}
		#endregion

		#region ..获取某点的颜色
		internal Color GetColorAtPoint(Point p)
		{
			//			p = this.PointToVirtualView(p);
			if(this.bmp == null)
				this.PaintContent(false);
			return this.bmp.GetPixel(p.X,p.Y);
		}
		#endregion

		#region ..重置当前操作
		internal void ResetOperation()
		{
			if(this.currentOperation != null)
				this.currentOperation.Reset();
		}
		#endregion

		#region ..Focus
		protected override void OnGotFocus(EventArgs e)
		{
			this.validContent = true;
			base.OnGotFocus (e);
		}

		protected override void OnLostFocus(EventArgs e)
		{
			this.validContent = true;
			base.OnLostFocus (e);
		}

		#endregion

		#region ..双击，执行动作
		protected override void OnDoubleClick(EventArgs e)
		{
             SVGElement element = this.GetElementAtLocation(this.PointToClient(Control.MousePosition));
             if (element != null)
             {
                 ElementClickEventArgs e1 = new ElementClickEventArgs(element, MouseClickType.DoubleClick, Control.MouseButtons);
                 this.OnElementClick(e1);
                 if (!e1.Bubble)
                     return;
             }
			base.OnDoubleClick (e);
		}
		#endregion

		#region ..处理属性框
		private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			if(this.svgDocument == null)
				return;
			this.svgDocument.InvokeUndos();
			this.Invalidate();
		}
		#endregion	

        #region ..InvalidateSelectedLabel
        internal void InvalidateSelectedLabel()
		{
			SVG.SVGElementCollection list = this.svgDocument.SelectCollection as SVG.SVGElementCollection;
			if(list.Count > 0)
			{
				for(int i = 0;i<list.Count;i++)
				{
					this.InvalidateTextBlock(this, list[i] as SVG.SVGTransformableElement,null,MatrixOrder.Append);
				}
			}
		}

		internal void InvalidateLabelText(Control control,SVG.SVGTransformableElement render)
		{
            this.InvalidateTextBlock(control, render, null, MatrixOrder.Append);
		}

		internal void InvalidateTextBlock(Control control, SVG.SVGTransformableElement render,Matrix matrix,MatrixOrder order)
		{
			if(render == null)
				return;
            
			this.InvalidateShadow(control, render,matrix,order);
		}
		#endregion

        #region ..InvalidateShadow
        void InvalidateShadow(Control control, SVG.SVGTransformableElement render,Matrix matrix,MatrixOrder order)
		{
            if ((render as SVG.Interface.ISVGPathable) == null)
				return;
			if(render.ShadowStyle.DrawShadow)
			{
				using(GraphicsPath path = (render as SVG.Interface.ISVGPathable).GPath.Clone() as GraphicsPath)
				{
					using(Matrix matrix1 = this.GetTotalTransformForElement(render))
					{
						if(matrix != null)
							matrix1.Multiply(matrix,order);
						matrix1.Translate(render.ShadowStyle.XOffset,render.ShadowStyle.YOffset);
						path.Transform(matrix1);
					}
					
					RectangleF rect = path.GetBounds();
					Rectangle rect1 = Rectangle.Ceiling(rect);
                    control.Invalidate(new Rectangle(rect1.X - 2, rect1.Y - 2, rect1.Width + 4, rect1.Height + 4));

					//invalidate the shadow of the marker if the it has the marker
					if(render.SupportMarker)
					{
						using(Matrix temp = new Matrix())
						{
							temp.Translate(render.ShadowStyle.XOffset,render.ShadowStyle.YOffset);
							if(matrix != null)
								temp.Multiply(matrix,order);
							SVG.ClipAndMask.SVGMarkerElement marker = render.MarkerStart;
							this.InvalidateMarker(marker,temp,MatrixOrder.Append);

							marker = render.MarkerEnd;
							this.InvalidateMarker(marker,temp,MatrixOrder.Append);
						}
					}
				}
			}
		}
		#endregion

		#region ..OnSelectionChanged
        /// <summary>
        /// 引发SelectionChanged事件
        /// </summary>
		protected virtual void OnSelectionChange()
		{
			if(this.SelectionChanged != null && this.svgDocument.SelectCollection.collectionChanged)
			{
				this.svgDocument.SelectCollection.collectionChanged = false;
				this.SelectionChanged(this,EventArgs.Empty);
			}
		}
		#endregion

		#region ..InvokeEditText
		internal void InvokeEditTextEvent(string c)
		{
			if(EditTextEvent != null)
			{
				this.ImeMode = ImeMode.On;
				EditTextEvent(c,EventArgs.Empty);
			}
		}
		#endregion

		#region ..svgDocument_NodeRemoved
		private void svgDocument_NodeRemoved(object sender, System.Xml.XmlNodeChangedEventArgs e)
		{
			SVG.Interface.ISVGPathable render = e.Node as SVG.Interface.ISVGPathable;
            if (render != null)
            {
                if (this.renderElements.Contains(render))
                    this.renderElements.Remove(render);
                if (this.connectableElements.Contains(render))
                    this.connectableElements.Remove(render);
                this.InvalidateElement(render);
            }
		}
		#endregion

		#region ..OnElementClick
        /// <summary>
        /// 引发ElementClick事件
        /// </summary>
        /// <param name="e"></param>
		protected virtual void OnElementClick(ElementClickEventArgs e)
		{
			if(this.ElementClick != null)
				this.ElementClick(this,e);
		}
		#endregion

		#region ..DrawTileImage
		/// <summary>
		/// Draw the tile image
		/// </summary>
		/// <param name="bmp"></param>
		/// <param name="rect"></param>
		void DrawTileImage(Graphics g,System.Drawing.Image bmp,Rectangle rect)
		{
			if(bmp == null || bmp.Width == 0 || bmp.Height == 0)
				return;
			int h = (int)Math.Ceiling((float)rect.Width / (float)bmp.Width);
			int v = (int)Math.Ceiling((float)rect.Height / (float)bmp.Height);
			for(int i = 0;i<h;i++)
			{
				for(int j = 0;j<v;j++)
				{
                    g.DrawImage(bmp, rect.X + i * bmp.Width, rect.Y + j * bmp.Height);
				}
			}
		}
		#endregion

		#region ..SetTransformType
		/// <summary>
		/// 对指定的对象类型应用可用的TransformType
		/// </summary>
		/// <param name="tagNames">需要应用的对象名称</param>
        /// <param name="protectType">允许进行的Transform Type</param>
		internal void ProtectElement(string[] tagNames,ProtectType protectType)
		{
			foreach(string name in tagNames)
				this.elementsTransformType[name.ToLower().Trim()] = protectType;
		}
		#endregion

		#region ..GetTranformTypeForElement
		/// <summary>
		/// 获取对于指定的对象可以进行的TransformType
		/// </summary>
		/// <param name="element">需要操作的对象</param>
		/// <returns>对传入的对象所能应用的Transform Type</returns>
		internal ProtectType GetProtectTypeForElement(SVG.SVGElement element)
		{
			if(element == null)
				return ProtectType.None;
			string name = element.Name;
			if(this.elementsTransformType.ContainsKey(name.ToLower().Trim()))
				return (ProtectType)this.elementsTransformType[name.ToLower().Trim()];
			return this.protectType;
		}
		#endregion

		#region ..OnTryConnectElement
		bool OnElementConnecting(ElementConnectEventArgs e)
		{
			if(this.ElementConnecting != null)
				return this.ElementConnecting(this,e);
			return true;
		}
		#endregion

		#region ..TrytoConnectElement
		/// <summary>
		/// Try to connect the element to see whether the element could be connected or not
		/// </summary>
		/// <param name="targetElement">the element you want to connect</param>
		/// <param name="type">indicates whether the element is the startelement</param>
		/// <returns></returns>
        internal bool TrytoConnectElement(SVGTransformableElement targetElement, int connectablePointIndex, int numberOfConnectablePoints, ConnectionTargetType type, SVG.BasicShapes.SVGBranchElement connectElement)
		{
			return this.OnElementConnecting(new ElementConnectEventArgs(targetElement,connectablePointIndex, numberOfConnectablePoints, type,connectElement));
		}
		#endregion

		#region ..InvalidateElements
		internal void InvalidateElements(SVGElement[] elements)
		{
            SVG.SVGElementCollection list = new SVGElementCollection(elements);

			this.InvalidateElements(list);
		}
		#endregion

		#region ..OnMouseWheel
		protected override void OnMouseWheel(MouseEventArgs e)
		{
			if(!this.zoomWhenWheel)
				base.OnMouseWheel (e);
			else
			{
                Point viewPoint = this.PointToClient(MousePosition);
                if(!(new Rectangle(Point.Empty,this.Size).Contains(viewPoint)))
                    viewPoint = new Point(this.Width / 2, this.Height / 2);
                
				float a = (float)e.Delta / (float)(SystemInformation.MouseWheelScrollDelta * 10);
				a += this.scaleRatio;
                this.ScaleAtCenter(a, viewPoint);
			}
		}
		#endregion

        #region ..BringElementsIntoView
        /// <summary>
        /// Bring the special elements into view
        /// </summary>
        /// <param name="elements">the elements which need to show in the view</param>
        public void BringElementsIntoView(SVGElement[] elements)
        {
            if (elements == null || elements.Length == 0)
                return;
            using (GraphicsPath path = new GraphicsPath())
            {
                for (int i = 0; i < elements.Length; i++)
                {
                    SVGElement elm = elements[i];
                    if (elm is SVG.Interface.ISVGPathable)
                    {
                        SVG.Interface.ISVGPathable render = elm as SVG.Interface.ISVGPathable;
                        using (GraphicsPath path1 = (render as SVG.Interface.ISVGPathable).GPath.Clone() as GraphicsPath)
                        {
                            path1.Transform(this.GetTotalTransformForElement(render as SVGTransformableElement));
                            path.StartFigure();
                            path.AddPath(path1, false);
                        }
                    }
                }
                using (Matrix matrix = this.CoordTransform.Clone())
                {
                    matrix.Invert();
                    path.Transform(matrix);
                    RectangleF bounds = path.GetBounds();
                    if (!bounds.IsEmpty)
                    {
                        float scale = (float)(this.Width - SelectionMargin) / (float)bounds.Width;
                        scale = (float)Math.Min(scale, (float)(this.Height - SelectionMargin) / (float)bounds.Height);
                        this.ScaleRatio = scale;
                        float x = this.RealMargin.Left + bounds.X * scale - SelectionMargin /2 ;
                        float y = this.RealMargin.Top + bounds.Y * scale - SelectionMargin / 2;
                        this.AutoScrollPosition = Point.Round(new PointF(x, y));
                    }
                }
            }
        }
        #endregion

        #region ..UpdateCoordTransform
        //void UpdateCoordTransform()
        //{
        //    //lock (this.CoordTransform)
        //    //{
        //        this.CoordTransform.Reset();
        //        this.CoordTransform.Translate(-this.VirtualLeft, -this.VirtualTop);
        //        this.CoordTransform.Translate(this.margin.Left, this.margin.Top);
        //        this.CoordTransform.Scale(this.scaleRatio, this.scaleRatio);
        //    //}
        //}
        #endregion

        #region ..OnInvalidated
        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            base.OnInvalidated(e);
            Rectangle rect = e.InvalidRect;
            if (!this.clipRectangles.Contains(rect))
                this.clipRectangles.Add(rect);

//#if DEBUG
//            if (rect.Width > 1000)
//                Console.Write("fsa");
//#endif
        }
        #endregion

        #region ..SetScrollPos
        Point? lastScrollPos = null;
        internal void SetScrollPos(Point point)
        {
            //check whether the control allows scroll
            Point p = this.AutoScrollPosition;
            Size size = this.AutoScrollMinSize;
            if (size.Width < this.Width)
                point.X = 0;
            if (size.Height < this.Height)
                point.Y = 0;
            if (p != point)
            {
                if (this.rule.Visible)
                {
                    if (p.X != point.X)
                        this.Invalidate(new Rectangle(0, 0, this.ruleLength, this.Height - SystemInformation.HorizontalScrollBarHeight));
                    if (p.Y != point.Y)
                        this.Invalidate(new Rectangle(0, 0, this.Width - SystemInformation.VerticalScrollBarWidth, this.ruleLength));
                }
                int xDelta = point.X - this.AutoScrollPosition.X;
                int yDelta = point.Y - this.AutoScrollPosition.Y;
                //if (InScroll)
                //{
                //    p = point;
                //    point.X = point.X > 0 ? -point.X : point.X;
                //    point.Y = point.Y > 0 ? -point.Y : point.Y;
                //    lastScrollPos = point;
                //    this.PaintContent(true);
                //    this.AutoScrollPosition = p;
                //    this.Update();
                //}
                //else
                {
                    this.AutoScrollPosition = point;
                    this.scrollProgrammingChanged = true;
                }
            }
        }
        #endregion

        #region ..GetCurrentSegPath
        internal SVGElement GetCurrentSegPath()
        {
            if (this.currentOperation is Operation.BezierSplineOperation)
            {
                SVG.Paths.SVGPathElement path = ((Operation.BezierSplineOperation)this.currentOperation).getSelectSegPath();
                if (path != null)
                    return path;
            }

            return null;
        }
        #endregion

        #region ..svgDocument_ElementsChanged
        void svgDocument_ElementsChanged(object sender, CollectionChangedEventArgs e)
        {
            if (!this.svgDocument.IsActive)
                return;
            this.InvalidateElements(new SVGElementCollection(e.ChangeElements));
            if (!this.validContent)
                return;
            if (!selectChanged)
            {
                foreach (SVGElement elm in e.ChangeElements)
                {
                    if (IsSelect(elm,false))
                    {
                        selectChanged = true;
                        //this.UpdateSelectInfo();
                        break;
                    }
                }
            }
        }
        #endregion

        #region ..IsSelect
        bool IsSelect(SVGElement elm, bool checkParent)
        {
            if (elm == null)
                return false;
            bool selection = this.svgDocument.SelectCollection.Contains(elm);
            if (selection)
                return true;

            //if (checkParent)
            //{
            //    if (elm.ParentElement != null)
            //        return IsSelect(elm.ParentElement as SVGElement, checkParent);
            //}

            if (elm is SVG.DocumentStructure.SVGGElement)
            {
                foreach (SVGElement child in (elm as SVG.DocumentStructure.SVGGElement).ChildElements)
                {
                    bool sel = IsSelect(child, false);
                    if (sel)
                        return true;
                }
            }
            return false;
        }
        #endregion

        #region ..GetTotalTransformForElement
        /// <summary>
        /// 获取对象针对于Client坐标系的总体变换
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        internal Matrix GetTotalTransformForElement(SVGTransformableElement element)
        {
            Matrix matrix = this.CoordTransform;
            matrix.Multiply(element.TotalTransform);
            return matrix;
        }
        #endregion

        #region ..OnPaintConnectablePoint
        /// <summary>
        /// 引发PaintConnectablePoint事件
        /// </summary>
        /// <param name="args"></param>
        protected virtual void OnPaintConnectablePoint(PaintConnectablePointEventArgs args)
        {
            if (this.PaintConnectablePoint != null)
                this.PaintConnectablePoint(this, args);
        }
        #endregion

        #region ..svgDocument_CurrentSceneChanged
        void svgDocument_CurrentSceneChanged(object sender, EventArgs e)
        {
            this.OnSceneChanged();
        }
        #endregion

        #region ..svgDocument_Loaded
        void svgDocument_Loaded(object sender, EventArgs e)
        {
            this.clipRectangles.Clear();
            this.Invalidate();
            this.firstLoad = true;
            this.CalculateViewSize();
        }
        #endregion

        #region ..OnSceneChanged
        /// <summary>
        /// 引发SceneChanged事件
        /// </summary>
        protected virtual void OnSceneChanged()
        {
            //取消选区
            this.svgDocument.ChangeSelectElement(null);
            this.onlyInvalidateSelection = false;
            //场景发生变化，清除所有记录对象
            this.renderElements.Clear();
            this.connectableElements.Clear();
            this.Invalidate();
            
            if (this.SceneChanged != null)
                this.SceneChanged(this, EventArgs.Empty);
        }
        #endregion

        #region ..OnElementDropped
        /// <summary>
        /// 引发ElementDropped事件
        /// </summary>
        /// <param name="e"></param>
        protected internal virtual void OnElementDropped(ElementDroppedEventArgs e)
        {
            if (this.ElementDropped != null)
                this.ElementDropped(this, e);
        }
        #endregion

        #region ..UpdateTextBlockStyle
        void UpdateTextBlockStyle()
        {
            this.UpdateTextBlockStyle(this.svgDocument.SelectCollection, true);
        }

        void UpdateTextBlockStyle(SVGElementCollection elements, bool invoke)
        {
            if (elements == null || elements.Count == 0)
                return;
            bool old = this.svgDocument.AcceptNodeChanged;
            this.svgDocument.AcceptNodeChanged = invoke;
            string color = SVG.ColorHelper.GetColorStringInHex(this.textBlockStyle.TextColor).ToLower();
            string alignment = EnumHelper.GetAlignmentSVGString(textBlockStyle.Alignment);

            string verticalAlignment = EnumHelper.GetVerticalAlignmentString(textBlockStyle.VerticalAlignment);
            ArrayList list = new ArrayList(new string[] { "rect", "use", "polygon", "circle", "textBlock", "ellipse", "path","line","polyline","image"});

            for (int i = 0; i < elements.Count; i++)
            {
                SVG.SVGElement element = elements[i] as SVG.SVGElement;
                this.UpdateElementAttribute(element, "black", "text-color", color, list);
                this.UpdateElementAttribute(element, "start", "text-anchor", alignment, list);
                this.UpdateElementAttribute(element, "middle", "vertical-align", verticalAlignment, list);
            }
            list.Clear();
            list = null;
            this.svgDocument.AcceptNodeChanged = old;
            if(invoke)
                this.svgDocument.InvokeUndos();
        }
        #endregion

        #region ..GetElementAtViewPoint
        internal SVGTransformableElement GetElementAtViewPoint(PointF viewPoint, SVG.SVGElementCollection list)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (list[i] is SVG.Interface.ISVGPathable)
                    {
                        if (this.HitTest(list[i] as SVG.SVGStyleable, viewPoint))
                        {
                            return list[i] as SVG.SVGTransformableElement;
                        }
                    }
                }
            }
            return null;
        }
        #endregion

        #region ..HitTest
        /// <summary>
        /// HitTest
        /// </summary>
        /// <param name="styleable"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        internal bool HitTest(SVG.SVGStyleable styleable, PointF viewPoint)
        {
            if (styleable is SVG.Interface.ISVGPathable 
                && styleable.StyleContainer != null
                && styleable.StyleContainer.VisualMediaStyle.display != "none"
                && styleable.StyleContainer.VisualMediaStyle.visiblility != "hidden")
            {
                using (Pen selectPen = new Pen(Color.Black, Canvas.PenWidth))
                {
                    selectPen.Alignment = System.Drawing.Drawing2D.PenAlignment.Center;
                    SVG.ViewStyle viewStyle = (styleable as SVG.SVGStyleable).ViewStyle;
                    //if ((viewStyle & SVG.ViewStyle.Lock) == SVG.ViewStyle.Lock)
                    //    return false;
                    SVG.Interface.ISVGPathable render = (SVG.Interface.ISVGPathable)styleable;
                    if (!(render as SVG.SVGElement).IsActive)
                        return false;
                    SVG.SVGTransformableElement transformable = render as SVG.SVGTransformableElement;
                    if (transformable == null)
                        return false;
                    int count = (render as SVG.Interface.ISVGPathable).GPath.PointCount;
                    GraphicsPath temppath = (render as SVG.Interface.ISVGPathable).GPath;
                    if (temppath == null || count <= 1)
                        return false;

                    using (GraphicsPath path = new GraphicsPath())
                    {

                        bool isText = render is SVG.Text.SVGTextContentElement;
                        if (isText || count > Canvas.MaxPathPoint)
                            path.AddRectangle(temppath.GetBounds());
                        else
                            path.AddPath(temppath, false);

                        path.FillMode = FillMode.Winding;
                        path.Transform(transformable.TotalTransform);

                        bool a = false;
                        if (transformable is SVG.Text.SVGTextBlockElement)
                            a = path.IsVisible(viewPoint);
                        else
                        {
                            if (!(transformable is SVG.Interface.ILineElement)
                                && (transformable is SVG.DocumentStructure.SVGGElement ||
                                (transformable.StyleContainer.FillStyle.fillOpacity.Value != 0 
                                && transformable.StyleContainer.FillStyle.svgPaint.PaintType != (ulong)SVG.PaintType.SVG_PAINTTYPE_NONE)))
                                a = path.IsVisible(viewPoint);
                            //TextBlock
                            temppath = transformable.GraphicsPathIncludingTextBlock;
                            if (temppath != null && temppath.PointCount > 1)
                                a = a || temppath.IsVisible(viewPoint);

                            float width = (render as SVG.SVGStyleable).StrokeStyle.strokewidth.Value + 2;
                            width = width < 5 ? 5 : width;
                            selectPen.Width = width;
                            if (!a && !isText && transformable.StyleContainer != null && transformable.StyleContainer.StrokeStyle.strokeOpacity.Value != 0 && transformable.StyleContainer.StrokeStyle.svgStroke.PaintType != (ulong)SVG.PaintType.SVG_PAINTTYPE_NONE)
                                a = path.IsOutlineVisible(viewPoint, selectPen);
                        }
                        return a;
                    }
                }
            }
            return false;
        }
        #endregion

        #region ..处理Lock图元
        #endregion

        #region ..DropSymbol
        /// <summary>
        /// 将指定的组件Symbol对象添加到画布的指定位置
        /// </summary>
        /// <param name="symbol">要放置的Symbol对象</param>
        /// <param name="viewPoint">目标位置</param>
        /// <returns>返回放置后的图元，如果是use引用，则返回对应的use图元，如果复制添加，则返回复制添加后的group图元</returns>
        public SVGElement DropSymbol(SVG.DocumentStructure.SVGSymbolElement symbol, PointF viewPoint)
        {
            return DropSymbol(symbol, viewPoint, false);
        }

        internal SVGElement DropSymbol(SVG.DocumentStructure.SVGSymbolElement symbol, PointF viewPoint, bool alignCenterPoint)
        {
            SVG.DocumentStructure.SVGSymbolElement sym = symbol;
            string id = symbol.ID;
            SVG.Document.SVGDocument doc = this.SVGDocument;
            string action = string.Empty;
            bool old = doc.AcceptNodeChanged;
            doc.AcceptNodeChanged = false;
            SymbolAppendMode appendMode = SymbolAppendMode.UseRef;
            if (sym.HasAttribute("appendMode"))
            {
                try
                {
                    appendMode = (SymbolAppendMode)System.Enum.Parse(typeof(SymbolAppendMode), sym.GetAttribute("appendMode"), true);
                }
                catch { }
            }
            SVG.SVGElement toBeAppended = null;

            #region ..if the mode is use ref
            //if the mode is use ref
            if (appendMode == SymbolAppendMode.UseRef)
            {
                if (doc.GetReferencedNode(id, new string[] { "symbol" }) == null)
                {
                    SVG.SVGElement element = doc.ImportNode(sym, true) as SVG.SVGElement;
                    if (element != null)
                    {
                        element = doc.AddDefsElement(element) as SVG.SVGElement;
                        action = element.GetAttribute("action");
                    }
                }
                SVG.SVGElement use = doc.CreateElement(doc.Prefix, "use", doc.NamespaceURI) as SVG.SVGElement;
                try
                {
                    if (use != null)
                    {
                        use.InternalSetAttribute("href", SVG.Document.SVGDocument.XLinkNamespace, "#" + id);
                        System.Drawing.Drawing2D.GraphicsPath path = (sym as SVG.Interface.IOutlookBarPath).GPath;
                        if (path != null && path.PointCount > 1)
                        {
                            RectangleF rect = path.GetBounds();
                            PointF p = viewPoint;

                            string trans = alignCenterPoint ? ("translate(" + (p.X - rect.X - rect.Width / 2).ToString() + " " + (p.Y - rect.Y - rect.Height / 2).ToString() + ")") : ("translate(" + (p.X - rect.X).ToString() + " " + (p.Y - rect.Y).ToString() + ")");
                            use.InternalSetAttribute("transform", trans);
                            trans = null;

                        }
                        if (action.Trim().Length > 0)
                            use.InternalSetAttribute("action", action);
                        toBeAppended = use;
                    }
                }
                catch
                {

                }
            }
            #endregion

            #region ..DirectAppend
            else if (appendMode == SymbolAppendMode.DirectAppend)
            {
                SVG.SVGElement g = doc.CreateElement(doc.Prefix, "g", doc.NamespaceURI) as SVG.SVGElement;
                try
                {
                    if (g != null)
                    {
                        //append the childs to the group
                        if (sym != null)
                        {
                            foreach (System.Xml.XmlNode child in sym.ChildNodes)
                            {
                                if (child is System.Xml.XmlElement)
                                    g.InternalAppendChild(doc.ImportNode(child.Clone(), true));
                            }
                        }
                        //if only a child element, just direct append
                        if ((g as SVG.DocumentStructure.SVGGElement).ChildElements.Count == 1)
                            g = (g as SVG.DocumentStructure.SVGGElement).ChildElements[0] as SVG.SVGElement;

                        bool isGroup = g is SVGGElement;
                        //复制属性
                        foreach (System.Xml.XmlAttribute attri in sym.Attributes)
                        {
                            if (attri.Name != "id" && attri.Name != "title"
                                && attri.Name != "viewbox" && attri.Name != "preserveAspectRatio"
                                && attri.Name != SVGSymbolElement.SymbolAppendModeAttributeString)
                            {
                                if (!isGroup || (attri.Name != SVGConnectionElement.ConnectablePointAttributeString
                                && attri.Name != SVGConnectionElement.CreateDefaultConnectablePointAttributeString))
                                    g.InternalSetAttribute(attri.Name, attri.Value);
                            }
                        }

                        //将连接属性写入第一个子节点
                        if (g.FirstChildElement is SVGTransformableElement && (g.FirstChildElement as SVGTransformableElement).Connectable)
                        {
                            if (sym.HasAttribute(SVGConnectionElement.CreateDefaultConnectablePointAttributeString))
                                (g.FirstChildElement as SVGElement).InternalSetAttribute(SVGConnectionElement.CreateDefaultConnectablePointAttributeString,
                                    sym.GetAttribute(SVGConnectionElement.CreateDefaultConnectablePointAttributeString));
                            if (sym.HasAttribute(SVGConnectionElement.ConnectablePointAttributeString))
                                (g.FirstChildElement as SVGElement).InternalSetAttribute(SVGConnectionElement.ConnectablePointAttributeString,
                                    sym.GetAttribute(SVGConnectionElement.ConnectablePointAttributeString));
                        }

                        System.Drawing.Drawing2D.GraphicsPath path = (sym as SVG.Interface.IOutlookBarPath).GPath;
                        if (path != null && path.PointCount > 1)
                        {
                            RectangleF rect = path.GetBounds();
                            PointF p = viewPoint;

                            string trans = string.Format("{0} {1}", alignCenterPoint ? ("translate(" + (p.X - rect.X - rect.Width / 2).ToString() + " " + (p.Y - rect.Y - rect.Height / 2).ToString() + ")") : ("translate(" + (p.X - rect.X).ToString() + " " + (p.Y - rect.Y).ToString() + ")"), g.GetAttribute("transform"));
                            g.InternalSetAttribute("transform", trans);
                            trans = null;

                        }
                        if (action.Trim().Length > 0)
                        {
                            g.InternalSetAttribute("action", action);
                        }
                        toBeAppended = g;
                    }
                    doc.AcceptNodeChanged = old;
                }
                catch
                {

                }
            }
            #endregion

            if (toBeAppended != null)
            {
                doc.AcceptNodeChanged = false;
                doc.AcceptNodeChanged = old;
                if (toBeAppended != null)
                    toBeAppended = this.AddElement(toBeAppended, true, true);
            }
            doc.AcceptNodeChanged = old;

            return toBeAppended;
        }
        #endregion
    }        

    #region ..AdaptAttributeEvent
    /// <summary>
    /// 处理更新属性事件
    /// </summary>
    internal delegate void AdaptAttributeEventHandler(object sender, AdaptAttributeEventArgs e);

	/// <summary>
	/// 记录属性更新的数据
	/// </summary>
	internal class AdaptAttributeEventArgs:System.EventArgs
	{
		/// <summary>
		/// 改变的属性名称
		/// </summary>
		internal string AttributeName = string.Empty;

		/// <summary>
		/// 改变的属性值
		/// </summary>
		internal string AttributeValue = string.Empty;

		/// <summary>
		/// 如果属性值关联到特定的对象，记录值对象，如渐变
		/// </summary>
		internal YP.SVG.SVGElement AttributeValueElement = null;

		internal AdaptAttributeEventArgs(string attributeName,string attributeValue,YP.SVG.SVGElement attributeValueElement)
		{
			this.AttributeName = attributeName;
			this.AttributeValue = attributeValue;
			this.AttributeValueElement = attributeValueElement;
		}

		public AdaptAttributeEventArgs(string attributeName,string attributeValue):this(attributeName,attributeValue,null)
		{
		}
	}
	#endregion

	#region ..颜色操作事件
	/// <summary>
	/// 当进行颜色操作时发生
	/// </summary>
	internal delegate void ColorOperateEventHandler(object sender,ColorOperateEventArgs e);

	internal class ColorOperateEventArgs:System.EventArgs
	{
		#region ..私有变量
		Operator operate = Operator.None;
		YP.SVG.Interface.ISVGPathable renderElement = null;
		#endregion

		#region ..公共属性
		/// <summary>
		/// 获取颜色操作方式
		/// </summary>
		internal Operator Operate
		{
			get
			{
				return this.operate;
			}
		}

		/// <summary>
		/// 获取颜色操作对象
		/// </summary>
		internal YP.SVG.Interface.ISVGPathable RenderElement
		{
			get
			{
				return this.renderElement;
			}
		}
		#endregion

		internal ColorOperateEventArgs(Operator operate,YP.SVG.Interface.ISVGPathable render)
		{
			this.operate = operate;
			this.renderElement = render;
		}
	}
	#endregion
}
