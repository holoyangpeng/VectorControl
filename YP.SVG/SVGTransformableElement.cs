using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace YP.SVG
{
	/// <summary>
	/// 实现SVG中可以进行二维变换的对象。
	/// </summary>
	public abstract class SVGTransformableElement:SVGLocatableElement,Interface.ISVGTransformable
	{
        #region ..const string
        public const string createDefaultConnectPoint = "createDefaultConnectPoint";
        #endregion

		#region ..构造及消除
		public SVGTransformableElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.transform = new DataType.SVGTransformList(string.Empty);//this);
            this.tempmatrix = new Matrix();
            this.render = new Render.SVGBaseRenderer(this);
            this.ChildElements.NotifyEvent = true;
            this.ChildElements.CollectionChanged += new CollectionChangedEventHandler(childRenders_CollectionChanged);
		}
		#endregion

		#region ..私有变量
        SVG.SVGElementCollection childRenders = new SVG.SVGElementCollection();
		DataType.SVGTransformList transform;
		public System.Drawing.Drawing2D.Matrix tempmatrix = new System.Drawing.Drawing2D.Matrix();
        public PointF[] anchors = new PointF[0];
        public RectangleF? finalBounds = null;
        public GraphicsPath graphicsPath = null;
        public GraphicsPath graphicsPathIncludingTextBlock = null;
        Color boundColor = Color.Black;
        System.Drawing.Drawing2D.Matrix totalTransform = null;
        System.Drawing.Drawing2D.Matrix totalTransform1 = new System.Drawing.Drawing2D.Matrix();
        Render.SVGBaseRenderer render;
		#endregion

		#region ..public properties
        public virtual Render.SVGBaseRenderer SVGRenderer { get { return render; } }

        public virtual GraphicsPath GraphicsPathIncludingTextBlock 
        { 
            get 
            {
                if (this.graphicsPathIncludingTextBlock == null)
                {
                    this.graphicsPathIncludingTextBlock = new GraphicsPath();
                    foreach (SVGElement child in this.childRenders)
                    {
                        Text.SVGTextBlockElement block = child as Text.SVGTextBlockElement;
                        if (child is Text.SVGTextBlockElement && (child as Text.SVGTextBlockElement).GPath != null && (child as Text.SVGTextBlockElement).GPath.PointCount > 1)
                        {
                            this.graphicsPathIncludingTextBlock.StartFigure();
                            
                            using (GraphicsPath path1 = block.GPath.Clone() as GraphicsPath)
                            {
                                path1.Transform(block.TotalTransform);
                                this.GraphicsPathIncludingTextBlock.AddPath(path1, false);
                            }
                        }
                    }
                }
                this.graphicsPathIncludingTextBlock.FillMode = FillMode.Winding;
                return this.graphicsPathIncludingTextBlock; 
            } 
        }

        public System.Drawing.Drawing2D.Matrix TotalTransform
        {
            get
            {
                if (this.totalTransform == null)
                    this.totalTransform = this.TotalMatrix;
                return this.totalTransform;
            }
        }

        /// <summary>
        /// gets the temp matrix
        /// </summary>
        public virtual System.Drawing.Drawing2D.Matrix GDITransform
        {
            get
            {
                return this.tempmatrix;
            }
        }

        public virtual System.Drawing.Drawing2D.Matrix TotalMatrix
        {
            get
            {
                YP.SVG.SVGElement root = this.OwnerDocument.RootElement as SVGElement;
                YP.SVG.SVGTransformableElement parent = this.ParentElement as SVGTransformableElement;
                System.Drawing.Drawing2D.Matrix temp = this.GDITransform.Clone();
                if(parent != null && parent != root)
                    temp.Multiply(parent.TotalTransform, MatrixOrder.Append);
                return temp;
            }
        }
		#endregion

		#region ..public properties
		/// <summary>
		/// 获取二维变换对象
		/// </summary>
		public Interface.CTS.ISVGTransformList Transform
		{
			get
			{
				if(this.refedElement is SVGTransformableElement)
					return (this.refedElement as SVGTransformableElement).transform;
				return this.transform;
			}
		}

        /// <summary>
        /// 获取子级绘制节点
        /// </summary>
        public SVG.SVGElementCollection ChildElements
        {
            get
            {
                return this.childRenders;
            }
        }

        public bool IsLock
        {
            set
            {
                if (this.IsLock != value)
                {
                    if (value)
                        this.ViewStyle = ViewStyle.Lock;
                    else
                        this.ViewStyle = ViewStyle.None;
                    //if locked, remove it from the selection
                    if (this.Selected)
                    {
                        if (this.IsLock)
                            this.OwnerDocument.SelectCollection.LockElement(this);
                        else
                            this.OwnerDocument.SelectCollection.UnLockElement(this);
                    }
                }
            }
            get
            {
                ViewStyle viewStyle = this.ViewStyle;
                return (viewStyle & ViewStyle.Lock) == ViewStyle.Lock;
            }
        }

        [Browsable(false)]
        public bool CreateDefaultConnectablePoint
        {
            set
            {
                this.InternalSetAttribute("createDefaultConnectPoint", value.ToString());
            }
            get
            {
                return this.GetAttribute("createDefaultConnectPoint").ToLower() != "false";
            }
        }
		#endregion

        #region ..TransformGraphics
        /// <summary>
		/// 变换
		/// </summary>
		/// <param name="g"></param>
		public virtual void TransformGraphics(Graphics g)
		{
			if(this.refedElement is SVGTransformableElement)
				(this.refedElement as SVGTransformableElement).TransformGraphics(g);
			else
			{
				//if scale the stroke
				if(this.OwnerDocument.ScaleStroke)
				{
					if(g != null && this.GDITransform.IsInvertible)
						g.Transform = this.GDITransform;
				}
				//this.TotalTransform.Multiply(this.GDITransform);
			}
		}

		/// <summary>
		/// 变换路径
		/// </summary>
		/// <param name="path"></param>
        public virtual void TransformPath(Graphics g, GraphicsPath path, StyleContainer.StyleOperator sp)
		{
			if(!this.OwnerDocument.ScaleStroke)
			{
				//this.TotalTransform.Multiply(this.GDITransform);
                if (path != null && path.PointCount > 1)
                {
                    path.Transform(this.TotalTransform);
                    path.Transform(sp.coordTransform);
                }
			}
			else
				this.TransformGraphics(g);
		}
		#endregion

        #region ..Update Transform
        public virtual void UpdateTotalTransform(System.Drawing.Drawing2D.Matrix baseTransform)
        {
            //lock (this.TotalTransform)
            //{
                System.Drawing.Drawing2D.Matrix temp = baseTransform.Clone();
                temp.Multiply(this.GDITransform);
                this.totalTransform = temp;
            //}
        }
        #endregion

        #region ..ResetTotalTransform
        public void ResetTotalTransform()
        {
            if (this.totalTransform == null)
                this.totalTransform = new Matrix();
            this.totalTransform.Reset();
        }
        #endregion

        #region ..AfterAttributeChanged
        public override void AfterAttributeChanged(string attributeName)
        {
            AttributeChangedResult result = this.AttributeChangeTest(attributeName);
            if ((result) != AttributeChangedResult.NoVisualChanged)
            {
                this.UpdateElement(true);
                this.UpdateElementWithAttribute(attributeName);
                if ((result & AttributeChangedResult.TransformChanged) == AttributeChangedResult.TransformChanged)
                    this.OnTransformChanged();
                if ((result & AttributeChangedResult.GraphicsPathChanged) == AttributeChangedResult.GraphicsPathChanged)
                {
                    this.UpdatePath();
                    this.UpdateConnects(true);
                }
                this.OwnerDocument.RefreshElement(this);
            }
        }
        #endregion

        #region ..OnTransformChanged
        public virtual void OnTransformChanged()
        {
            this.totalTransform = null;
            this.finalBounds = null;
            this.graphicsPathIncludingTextBlock = null;
            if (!this.IsActive)
                return;
            
            this.UpdateConnects(true);
            
            //子节点发生变化，更新父节点GPath
            if (this.ParentElement is SVGTransformableElement)
                (this.ParentElement as SVGTransformableElement).UpdatePath();

            foreach (SVGElement elm in this.ChildElements)
            {
                if (elm is SVGTransformableElement)
                    (elm as SVGTransformableElement).OnTransformChanged();
            }

            if (this.SVGRenderer != null)
                this.SVGRenderer.UpdateTotalTransform();
        }
        #endregion

        #region ..BeforeDrawing
        public virtual bool CalculateFinalBounds()
        {
            if (!finalBounds.HasValue && this is Interface.ISVGPathable)
            {
                GraphicsPath gp = (this as Interface.ISVGPathable).GPath;
                using (GraphicsPath path = gp.Clone() as GraphicsPath)
                {
                    path.Transform(this.TotalTransform);
                    RectangleF rect = path.GetBounds();
                    if (rect.IsEmpty)
                    {
                        if (rect.Width != 0 || rect.Height != 0)
                        {
                            rect.Inflate(rect.Width == 0 ? 0.1f : 0, rect.Height == 0 ? 0.1f : 0);
                        }
                    }
                    finalBounds = rect;
                    return true;
                }
            }
            return false;
        }

        public virtual bool BeforeDrawing(Graphics g, StyleContainer.StyleOperator sp)
        {
            if(!(this is Interface.ISVGPathable))
            return false;
            //if (!this.IsActive)
            //    return true;

            try
            {
                if (this.OwnerDocument.NeedReDraw(this))
                    return true;

                GraphicsPath gp = (this as Interface.ISVGPathable).GPath;
                if (gp == null)
                    return false;

                if (!sp.ClipRegion.IsEmpty(g))
                {
                    this.CalculateFinalBounds();

                    if (finalBounds.HasValue)
                    {
                        //存在无限内部，默认不绘制
                        if (!sp.ClipRegion.IsVisible(finalBounds.Value) ||  sp.ClipRegion.IsInfinite(g))
                        {
                            this.TransformGraphics(null);
                            return false;
                        }
                    }
                }
                return true;
            }
            finally
            {

                if (finalBounds.HasValue)
                {
                    float right = finalBounds.Value.Right > sp.contentBounds.Right ? finalBounds.Value.Right : sp.contentBounds.Right;
                    float bottom = finalBounds.Value.Bottom > sp.contentBounds.Bottom ? finalBounds.Value.Bottom : sp.contentBounds.Bottom;

                    sp.contentBounds = new RectangleF(0, 0, right, bottom);
                }
            }
            
        }
        #endregion

        #region ..extenal properties
        /// <summary>
        /// 是否可以连接
        /// </summary>
        public virtual bool Connectable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 设置或获取边界颜色
        /// </summary>
        public System.Drawing.Color BoundColor
        {
            set
            {
                this.boundColor = value;
            }
            get
            {
                if (this.boundColor.IsEmpty)
                {
                    Random r = new Random();
                    int r1 = r.Next(0, 155);
                    int g = r.Next(0, 155);
                    int b = r.Next(0, 155);
                    this.boundColor = Color.FromArgb(r1, g, b);
                }
                return this.boundColor;
            }
        }

        /// <summary>
        /// 是否支持Marker
        /// </summary>
        public virtual bool SupportMarker
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 是否支持阴影
        /// </summary>
        public virtual bool FillShadow
        {
            get
            {
                return true;
            }
        }
        #endregion

        #region ..Label
        #region ..fields
        //label
        string labelText = string.Empty;
        PointF labelPoint = PointF.Empty;
        System.Drawing.SolidBrush labelBrush = new SolidBrush(Color.Black);
        #endregion

        #region ..属性
        /// <summary>
        /// 获取文本标志点
        /// </summary>
        public PointF LabelPoint
        {
            get
            {
                return this.labelPoint;
            }
            set
            {
                this.labelPoint = value;
            }
        }

        public SolidBrush LabelBrush
        {
            get
            {
                return this.labelBrush;
            }
        }

        /// <summary>
        /// 获取或设置文本标志
        /// 文本标记通过textBlock节点组成，如果svg图元拥有textBlock节点，则Label会返回第一个textBlock内容
        /// </summary>
        public virtual string Label
        {
            get
            {
                Text.SVGTextBlockElement firstBlock = this.GetFirstTextBlock(string.Empty);
                if (firstBlock != null)
                    return firstBlock.InnerText;
                return string.Empty;
            }
            set
            {
                bool old = this.OwnerDocument.AcceptNodeChanged;
                this.OwnerDocument.AcceptNodeChanged = false;
                Text.SVGTextBlockElement firstBlock = this.GetFirstTextBlock(string.Empty);
                if (firstBlock != null)
                {
                    firstBlock.InnerText = value;
                }
                else
                {
                    firstBlock = CreateTextBlockForLabel();
                    firstBlock.InnerText = value;
                    this.InternalAppendChild(firstBlock);
                }
                OwnerDocument.InvokeUndos();
            }
        }
        #endregion
        #endregion

        #region ..Marker
        #region ..fields
        string strMarkerStart = null, strMarkerMid = null, strMarkerEnd = null;
        ClipAndMask.SVGMarkerElement markerStart, markerMid, markerEnd;
        #endregion

        #region ..properties
        /// <summary>
        /// 获取开始端线帽
        /// </summary>
        public YP.SVG.ClipAndMask.SVGMarkerElement MarkerStart
        {
            get
            {
                if (this.strMarkerStart != null && this.strMarkerStart.Length > 4 && this.markerStart == null)
                {
                    this.strMarkerStart = this.strMarkerStart.Substring(4, this.strMarkerStart.Length - 5);
                    this.markerStart = this.OwnerDocument.GetReferencedNode(this.strMarkerStart, new string[] { "marker" }) as YP.SVG.ClipAndMask.SVGMarkerElement;
                    this.strMarkerStart = null;
                }
                return this.markerStart;
            }
        }

        /// <summary>
        /// 获取中间端点线帽
        /// </summary>
        public YP.SVG.ClipAndMask.SVGMarkerElement MarkerMid
        {
            get
            {
                if (this.strMarkerMid != null && this.strMarkerMid.Length > 0 && this.markerMid == null)
                {
                    this.strMarkerMid = this.strMarkerMid.Substring(4, this.strMarkerMid.Length - 5);
                    this.markerMid = this.OwnerDocument.GetReferencedNode(this.strMarkerMid, new string[] { "marker" }) as YP.SVG.ClipAndMask.SVGMarkerElement;
                    this.strMarkerMid = null;
                }
                return this.markerMid;
            }
        }

        /// <summary>
        /// 获取末端点线帽
        /// </summary>
        public YP.SVG.ClipAndMask.SVGMarkerElement MarkerEnd
        {
            get
            {
                if (this.strMarkerEnd != null && this.strMarkerEnd.Length > 0 && this.markerEnd == null)
                {
                    this.strMarkerEnd = this.strMarkerEnd.Substring(4, this.strMarkerEnd.Length - 5);
                    this.markerEnd = this.OwnerDocument.GetReferencedNode(this.strMarkerEnd, new string[] { "marker" }) as YP.SVG.ClipAndMask.SVGMarkerElement;
                    this.strMarkerEnd = null;
                }
                return this.markerEnd;
            }
        }
        #endregion
        #endregion

        #region ..Connection
        #region ..private fields
        DataType.SVGPointList cntPoints = new YP.SVG.DataType.SVGPointList("");
        bool connectionChanged = true;
        SVGElementCollection refedConnects = new SVGElementCollection();

        #endregion

        #region ..properties
        public bool ConnectionChanged
        {
            get
            {
                return this.connectionChanged;
            }
            set
            {
                this.connectionChanged = value;
            }
        }

        /// <summary>
        /// 取得图元所有连接点的相对坐标值
        /// </summary>
        public PointF[] RelativeConnectionPoints
        {
            get
            {
                return this.BaseConnectionPoints.GetGDIPoints();
            }
        }

        public virtual DataType.SVGPointList BaseConnectionPoints
        {
            get
            {
                return this.cntPoints;
            }
        }

        /// <summary>
        /// determine whether use the default connected point
        /// </summary>
        public virtual bool UseDefaultConnectedPoint
        {
            get
            {

                //check whether use the default connected point
                bool useDefaultConnectPoint = this.OwnerDocument.UseDefaultConnectPoint;
                if (this.GetAttribute(createDefaultConnectPoint).Trim().Length > 0)
                    useDefaultConnectPoint = this.GetAttribute(createDefaultConnectPoint).Trim().ToLower() != "false";
                return useDefaultConnectPoint;
            }
        }

        /// <summary>
        /// 获取连接到图元上的连接线
        /// </summary>
        public YP.SVG.BasicShapes.SVGBranchElement[] Connections
        {
            get
            {
                this.OwnerDocument.InitializeConnectElement();
                if (this.refedConnects.Count > 0)
                {
                    List<SVGElement> cnns = new List<SVGElement>();
                    for(int i = 0;i < this.refedConnects.Count; i ++)
                    {
                        BasicShapes.SVGBranchElement branch = this.refedConnects[i] as BasicShapes.SVGBranchElement;
                        if (branch.ParentNode == null)
                        {
                            branch.ResetConnectElement(true);
                            branch.ResetConnectElement(false);
                            this.refedConnects.Remove(branch);
                            
                            i--;
                        }
                        else if(branch.OwnerConnection.StartElement == this
                            || branch.OwnerConnection.endElement == this)
                        {
                            cnns.Add(branch);
                        }
                    }
                    BasicShapes.SVGBranchElement[] cs = new BasicShapes.SVGBranchElement[cnns.Count];
                    cnns.CopyTo(cs, 0);
                    return cs;
                }
                return null;
            }
        }

        /// <summary>
        /// 如果当前节点有连接点，获取所有连接点的坐标
        /// </summary>
        public PointF[] ConnectionPoints
        {
            get
            {
                if (this.SVGRenderer != null)
                    return this.SVGRenderer.ConnectionPoints;
                return null;
            }
        }

        /// <summary>
        /// 获取或设置连接点字符串
        /// </summary>
        public virtual string ConnectionPointStr
        {
            get
            {
                return this.GetAttribute("connectPoints");
            }
            set
            {
                this.SetAttribute("connectPoints", value, false);
            }
        }
        #endregion

        #region ..UpdateConnects
        public void UpdateConnects(bool reCreateConnectPath)
        {
            for (int i = 0; i < this.refedConnects.Count; i++)
            {
                YP.SVG.BasicShapes.SVGBranchElement connect = this.refedConnects[i] as BasicShapes.SVGBranchElement;
                if (connect == null)
                    continue;
                if (connect.ParentNode == null)
                {
                    this.refedConnects.Remove(connect);
                    i--;
                    continue;
                }

                if (reCreateConnectPath)
                {
                    this.OwnerDocument.RefreshOriginalElement(connect);// connect.RefreshElement();
                    //connect.InternalRemoveAttribute("d");
                    connect.UpdateElement(true);
                    connect.UpdatePath();
                    if (connect.ChildElements.Count == 0 && connect.GetAttribute("d").Length > 0 && connect.anchors != null && connect.anchors.Length > 3 && connect.ConnectionPath!= null)
                    {
                        PointF[] ps = (connect as SVG.Interface.ISVGPathable).GPath.PathPoints;
                        PointF start = ps[0];
                        PointF end = ps[ps.Length - 1];
                        SVGTransformableElement startElement = null;
                        if (connect is BasicShapes.SVGConnectionElement)
                            startElement = (connect as BasicShapes.SVGConnectionElement).StartElement as SVGTransformableElement;
                        if (!connect.IsControlPointOK(start, end, startElement, connect.EndElement as SVGTransformableElement, null))
                            connect.InternalRemoveAttribute("d");
                    }
                    this.OwnerDocument.RefreshElement(connect);
                }
                else
                    connect.UpdateElement(true);
            }
        }
        #endregion

        #region ..AddRefedConnects
        public void AddRefedConnects(YP.SVG.BasicShapes.SVGBranchElement connect)
        {
            if (!this.refedConnects.Contains(connect))
                this.refedConnects.Add(connect);
        }

        public void RemoveRefedConnects(BasicShapes.SVGBranchElement connect)
        {
            if (this.refedConnects.Contains(connect))
                this.refedConnects.Remove(connect);
        }
        #endregion

        #region ..IsCenterConnectPoint
        /// <summary>
        /// judge whether the connect point at the index is the center connect point
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool IsCenterConnectPoint(int index)
        {
            //if(this.UseDefaultConnectedPoint)
            //    return index<0 || index >= this.connectPoints.Length - 1;
            //else
            //    return index< 0 || index >=this.connectPoints.Length;
            return false;
        }
        #endregion
        #endregion

        #region ..GetAnchors
        /// <summary>
        /// 获取锚点集
        /// </summary>
        public virtual PointF[] GetAnchors()
        {
            return this.anchors;
        }
        #endregion

        #region ..Clone
        public override System.Xml.XmlNode CloneNode(bool deep)
        {
            System.Xml.XmlNode node = base.CloneNode(deep);
            SVGTransformableElement render = node as SVGTransformableElement;
            if (render != null)
            {
                render.StyleContainer = this.StyleContainer;
                render.labelText = this.Label;
                render.labelPoint = this.labelPoint;
                if(this.SVGRenderer != null)
                    this.SVGRenderer.CloneRender(render.SVGRenderer);
                if(this.totalTransform != null)
                    render.totalTransform = this.totalTransform.Clone();

                if (this.graphicsPath != null)
                    render.graphicsPath = this.graphicsPath.Clone() as GraphicsPath;
                if (this.graphicsPathIncludingTextBlock != null)
                    render.graphicsPathIncludingTextBlock = this.graphicsPathIncludingTextBlock.Clone() as GraphicsPath;

                if (this.finalBounds != null)
                    render.finalBounds = this.finalBounds.Value;
            }

            
            return node;
        }
        #endregion

        #region ..SetSVGAttribute
        public override void SetSVGAttribute(string attributeName, string attributeValue)
        {
            try
            {
                switch (attributeName)
                {
                    case "backgroundImage":
                        if (this.SVGRenderer != null)
                            this.SVGRenderer.BackgroundImageSource = Common.ImageHelper.ParseImageSource(this.OwnerDocument, attributeValue);
                        break;
                    case "border":
                        if (this.SVGRenderer != null)
                            this.SVGRenderer.BorderPaint = new Paint.SVGPaint(attributeValue, this, "none");
                        break;
                    case "connectPoints":
                        this.cntPoints = new YP.SVG.DataType.SVGPointList(attributeValue);
                        this.connectionChanged = true;
                        break;
                    case "createDefaultConnectPoint":
                        this.connectionChanged = true;
                        break;
                    case "marker-start":
                        this.strMarkerStart = attributeValue;
                        this.markerStart = null;
                        break;
                    case "marker-mid":
                        this.strMarkerMid = attributeValue;
                        this.markerMid = null;
                        break;
                    case "marker-end":
                        this.strMarkerEnd = attributeValue;
                        this.markerEnd = null;
                        break;
                    case "transform":
                        this.transform = new DataType.SVGTransformList(attributeValue);
                        this.tempmatrix = this.transform.FinalMatrix.GetGDIMatrix();
                        break;
                    default:
                        base.SetSVGAttribute(attributeName, attributeValue);
                        break;
                }
            }
            catch (System.Exception e)
            {
                this.OwnerDocument.OnExceptionOccured(new ExceptionOccuredEventArgs(new object[] { e.Message }, ExceptionLevel.Normal));
            }
        }
        #endregion

        #region ..AttributeChangeTest
        public override AttributeChangedResult AttributeChangeTest(string attributeName)
        {
            if (string.Compare(attributeName,"backgroundImage")==0 ||string.Compare(attributeName,"labelColor") ==0)
                return AttributeChangedResult.VisualChanged;
            if (string.Compare(attributeName,"connectPoints")==0 ||string.Compare(attributeName,"createDefaultConnectPoint") ==0)
                return AttributeChangedResult.TransformChanged;
            if (string.Compare(attributeName,"transform")==0)
                return AttributeChangedResult.TransformChanged;
            return base.AttributeChangeTest(attributeName);
        }
        #endregion

        #region ..RemoveID
        public override bool RemoveID()
        {
            for (int i = 0; i < this.refedConnects.Count; i++)
            {
                SVGTransformableElement element = this.refedConnects[i] as SVGTransformableElement;
                if (element == null)
                    continue;

                SVGElement parent = element.TopParent;
                if (this.OwnerDocument.SelectCollection.Contains(parent) || (parent != null && this.OwnerDocument.SelectCollection.Contains(parent)))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region ..TopParent
        YP.SVG.SVGElement TopParent
        {
            get
            {
                SVGElement element = this;
                SVGElement root = this.OwnerDocument.RootElement as SVGElement;
                SVGElement parent = element.ParentElement as SVGElement;
                while (parent != null && parent != root)
                {
                    element = parent;
                    parent = parent.ParentElement as SVGElement;
                }
                return element;
            }
        }
        #endregion

        #region ..OnIDChanged
        /// <summary>
        /// occurs when the id changed
        /// </summary>
        public override void OnIDChanged(string oldValue, string newValue)
        {
            base.OnIDChanged(oldValue, newValue);
            //when id changed, refresh the attribute of the connect
            BasicShapes.SVGBranchElement[] cnns = this.Connections;
            if (cnns == null)
                return;

            foreach (BasicShapes.SVGConnectionElement cnn in cnns)
            {
                if (cnn.StartElement == this)
                {
                    string id = cnn.GetAttribute("start");
                    int index = id.LastIndexOf(".");
                    string si = string.Empty;
                    if (index > 0)
                        si = id.Substring(index);
                    cnn.InternalSetAttribute("start", "#" + newValue + si);
                }

                if (cnn.EndElement == this)
                {
                    string id = cnn.GetAttribute("end");
                    int index = id.LastIndexOf(".");
                    string si = string.Empty;
                    if (index > 0)
                        si = id.Substring(index);
                    cnn.InternalSetAttribute("end", "#" + newValue + si);
                }
            }
        }
        #endregion

        #region ..UpdatePath
        public virtual void UpdatePath()
        {
            this.UpdatePath(true);
        }

        public virtual void UpdatePath(bool updateParent)
        {
            this.graphicsPath = null;
            this.graphicsPathIncludingTextBlock = null;
            finalBounds = null;
            this.ConnectionChanged = true;

            if (this.ParentElement is SVGTransformableElement && updateParent)
                (this.ParentElement as SVGTransformableElement).UpdatePath();

            UpdateTextBlock();
        }
        #endregion

        #region ..FindAllConnects
        /// <summary>
        /// 取得连接到图元上的全部连接线
        /// </summary>
        /// <returns></returns>
        public SVG.BasicShapes.SVGBranchElement[] GetAllConnections()
        {
            return this.Connections;
        }
        #endregion

        #region ..TextBlock
        public virtual void UpdateTextBlockPath()
        {
            this.graphicsPathIncludingTextBlock = null;
            if (this.ParentElement is SVGTransformableElement)
                (this.ParentElement as SVGTransformableElement).UpdateTextBlockPath();
        }

        #region ..UpdateTextBlock
        void UpdateTextBlock()
        {
            //update text block
            if (this is Interface.ISVGTextBlockContainer)
            {
                foreach (SVGElement elm in this.childRenders)
                {
                    if (elm is Text.SVGTextBlockElement)
                        (elm as Text.SVGTextBlockElement).UpdatePath(false);
                }
            }
        }
        #endregion

        #region ..GetFirstTextBlock
        public Text.SVGTextBlockElement GetFirstTextBlock(string xpath)
        {
            string pattern = "*[local-name()='textBlock'";
            if (xpath != null && xpath.Trim().Length > 0)
                pattern = pattern+" and " + xpath ;
            pattern += "]";
            System.Xml.XmlNode node = this.SelectSingleNode(pattern);
            return node as Text.SVGTextBlockElement;
        }
        #endregion

        #region ..CreateTextBlockForLabel
        public Text.SVGTextBlockElement CreateTextBlockForLabel()
        {
            bool old = this.OwnerDocument.AcceptNodeChanged;
            this.OwnerDocument.AcceptNodeChanged = false;
            Text.SVGTextBlockElement textBlock = this.OwnerDocument.CreateElement("textBlock") as SVG.Text.SVGTextBlockElement;
            textBlock.InternalSetAttribute("fill", "none");
            textBlock.InternalSetAttribute("stroke", "none");
            textBlock.InternalSetAttribute("text-anchor", "center");
            this.OwnerDocument.AcceptNodeChanged = old;
            return textBlock;
        }
        #endregion

        #region ..ResetGraphicsPathIncludingTextBlock
        public void ResetGraphicsPathIncludingTextBlock()
        {
            if (this.graphicsPathIncludingTextBlock == null)
                this.graphicsPathIncludingTextBlock = new GraphicsPath();
            this.graphicsPathIncludingTextBlock.Reset();
        }
        #endregion
        #endregion

        #region ..子节点插入或者删除
        public virtual void childRenders_CollectionChanged(object sender, CollectionChangedEventArgs e)
        {
            if (e.Action == CollectionChangeAction.Insert && this.IsActive)
            {
                //if the element is svgtransform, update the totaltransform
                foreach (SVGElement elm in e.ChangeElements)
                {
                    if (elm is SVGTransformableElement)
                    {
                        (elm as SVGTransformableElement).UpdateTotalTransform(this.TotalTransform);
                        this.OwnerDocument.RefreshElement(elm);
                    }
                }
            }
        }
        #endregion
    }
}
