using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using YP.SVG.Common;

namespace YP.SVG.BasicShapes
{
    /// <summary>
    /// 连接线分支
    /// </summary>
    public class SVGBranchElement : SVGTransformableElement, 
        Interface.BasicShapes.ISVGBasicShape, 
        Interface.ISVGContainer, 
        Interface.ISVGPathable,
        Interface.ISVGTextBlockContainer,
        Interface.ILineElement
    {
        #region ..构造及消除
        public SVGBranchElement(string prefix, string localname, string ns, Document.SVGDocument doc)
            : base(prefix, localname, ns, doc)
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
            this.x2 = new DataType.SVGLength("0", this, LengthDirection.Hori);
            this.y2 = new DataType.SVGLength("0", this, LengthDirection.Vect);
            this.ChildElements.NotifyEvent = true;
            this.ChildElements.CollectionChanged += new CollectionChangedEventHandler(renders_CollectionChanged);
        }
        #endregion

        #region ..const
        public const int DynamicConnectShapeMargin = 16;
        public const int Distance = 30;
        public const int PolyMargin = 30;
        public const int SplineLength = 90;
        #endregion

        #region ..fields
        bool needAdjustPath = false;
        public DataType.SVGLength x1, y1, x2, y2;
        public string oldend = string.Empty;
        int endIndex = -1;
        public bool first = true;
        public float oriX1 = 0, oriY1 = 0;
        GraphicsPath connectionPath = new GraphicsPath();
        public float oriX2 = 0, oriY2 = 0;
        public SVG.ConnectionType type = SVG.ConnectionType.Dynamic;
        public SVGTransformableElement endElement = null;
        DataType.SVGString connectend = new SVG.DataType.SVGString(string.Empty);
        public SVG.DataType.SVGPointList controlPoints = new SVG.DataType.SVGPointList(string.Empty);
        int childBranchCount = 0;
        #endregion

        #region ..properties
        public override bool FillShadow
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 获取当前分支所属的连接线对象，如果是连接线，则返回自身
        /// </summary>
        public SVGConnectionElement OwnerConnection
        {
            get
            {
                if (this is SVGConnectionElement)
                    return this as SVGConnectionElement;
                if (this.ParentElement is SVGBranchElement)
                    return (this.ParentElement as SVGBranchElement).OwnerConnection;
                return null;
            }
        }

        public bool NeedAdjustPath
        {
            get
            {
                return this.needAdjustPath;
            }
            set
            {
                this.needAdjustPath = value;
            }
        }


        /// <summary>
        /// 获取连接线的原子路径，该路径不会进行连接，也就是说子节点的路径不会附着到父节点的原子路径上
        /// </summary>
        public GraphicsPath ConnectionPath
        {
            get
            {
                return this.connectionPath;
            }
        }

        public PointF EndPoint
        {
            get
            {
                return new PointF(this.x2.Value, this.y2.Value);
            }
        }

        public override bool SupportMarker
        {
            get
            {
                return true;
            }
        }

        public bool HasChildBranch
        {
            get
            {
                return this.childBranchCount > 0;
            }
        }

        /// <summary>
        /// 表示直线对象的X2属性（终止横坐标）
        /// </summary>
        public Interface.DataType.ISVGLength X2
        {
            get
            {
                return this.x2;
            }
        }

        /// <summary>
        /// 表示直线对象的Y2属性（终止纵坐标）
        /// </summary>
        public Interface.DataType.ISVGLength Y2
        {
            get
            {
                return this.y2;
            }
        }

        /// <summary>
        /// 表示直线对象的X1属性（起始横坐标）
        /// </summary>
        public Interface.DataType.ISVGLength X1
        {
            get
            {
                return this.x1;
            }
        }

        /// <summary>
        /// 表示直线对象的Y1属性（起始纵坐标）
        /// </summary>
        public Interface.DataType.ISVGLength Y1
        {
            get
            {
                return this.y1;
            }
        }

        /// <summary>
        /// 获取连接结束对象
        /// </summary>
        public SVGTransformableElement EndElement
        {
            get
            {
                if (this.oldend != this.connectend.Value)
                {
                    this.oldend = this.connectend.Value;
                    int index = this.oldend.LastIndexOf(".");
                    string temp = this.oldend;
                    this.endIndex = -1;
                    if (index >= 0)
                    {
                        temp = temp.Substring(0, index);
                        int.TryParse(this.oldend.Substring(index + 1), out endIndex);
                    }
                    SVGElement ownerSvg = this.OwnerSvgElement as SVGElement;
                    SVGElement ownerG = null;
                    if (this.GetAttribute("groupfirst") == "true")
                    {
                        this.endElement = null;
                        ownerG = this.OwnerStructureElement;
                        while (this.endElement == null && ownerG != null)
                        {
                            this.endElement = this.OwnerDocument.GetReferencedNode(temp, null, ownerG) as SVGTransformableElement;
                            if (this.endElement != null)
                                break;
                            var owner = ownerG.OwnerStructureElement;
                            if (owner == ownerG)
                                break;
                            ownerG = owner;
                        }
                    } 
                    if (this.endElement == null && ownerG != ownerSvg)
                        this.endElement = this.OwnerDocument.GetReferencedNode(temp, null, ownerSvg) as SVGTransformableElement;
                    if (this.endElement != null)
                    {
                        //SVGDom.SVGTransformableElement dir = this.endElement as SVGDom.SVGTransformableElement;
                        endElement.AddRefedConnects(this);
                    }
                }
                return this.endElement;
            }
            set
            {
                string id = this.GetAttribute("end");
                if (value != null)
                {
                    if (value.ID == string.Empty)
                        value.ID = Guid.NewGuid().ToString();
                    id = value.ID;
                    this.SetAttribute("end", "#" + id, false);
                }
                else
                    this.InternalRemoveAttribute("end");
            }
        }

        /// <summary>
        /// 获取连接线连接到图元的连接点索引
        /// </summary>
        public int EndIndex
        {
            get
            {
                if (this.EndElement != null)
                    return this.endIndex;
                return -1;
            }
        }

        public override StyleContainer.StyleContainer StyleContainer
        {
            get
            {
                if (this is SVGConnectionElement)
                    return base.StyleContainer;
                if (this.OwnerConnection != null)
                    return this.OwnerConnection.StyleContainer;
                return null;
            }
            set
            {
                if (this is SVGConnectionElement)
                    base.StyleContainer = value;
            }
        }
        #endregion

        #region ..SetSVGAttribute
        public override void SetSVGAttribute(string attributeName, string attributeValue)
        {
            try
            {
                SVGElement oldElement = null;
                SVGElement newElement = null;
                bool inloadProcess = this.OwnerDocument.inLoadProcess;
                SVG.SVGElement temp = null;
                switch (attributeName)
                {
                    case "end":
                        if (this.IsActive)
                        {
                            temp = this.EndElement as SVG.SVGElement;
                            if (temp != null)
                                oldElement = temp;
                        }
                        this.oldend = string.Empty;
                        //if (this.endElement is SVGTransformableElement)
                        //    (this.endElement as SVGTransformableElement).RemoveRefedConnects(this);
                        this.endElement = null;
                        this.connectend = new SVG.DataType.SVGString(attributeValue);
                        if (this.IsActive)
                        {
                            temp = this.EndElement as SVG.SVGElement;
                            if (temp != null)
                                newElement = temp;
                            this.OnConnectChanged(oldElement, newElement, ConnectionChangedType.EndElement);
                        }
                        break;
                    case "x2":
                        this.oriX2 = this.x2.Value;
                        this.x2 = new SVG.DataType.SVGLength(attributeValue, this, LengthDirection.Hori, "0");
                        break;
                    case "y2":
                        this.oriY2 = this.y2.Value;
                        this.y2 = new SVG.DataType.SVGLength(attributeValue, this, LengthDirection.Vect, "0");
                        break;
                    case "x1":
                        this.oriX1 = this.x1.Value;
                        this.x1 = new SVG.DataType.SVGLength(attributeValue, this, LengthDirection.Hori, "0");
                        break;
                    case "y1":
                        this.oriY1 = this.y1.Value;
                        this.y1 = new SVG.DataType.SVGLength(attributeValue, this, LengthDirection.Vect, "0");
                        break;
                    case "d":
                        this.controlPoints = new SVG.DataType.SVGPointList(attributeValue);
                        break;
                    default:
                        base.SetSVGAttribute(attributeName, attributeValue);
                        break;
                }
            }
            catch (Exception e)
            {
                this.OwnerDocument.OnExceptionOccured(new ExceptionOccuredEventArgs(new object[] { e.Message }, ExceptionLevel.Normal));
            }
        }
        #endregion

        #region ..OnConnectChanged
        public void OnConnectChanged(SVGElement oldElement, SVGElement newElement, ConnectionChangedType type)
        {
            if (oldElement == newElement)
                return;
            ConnectionChangedEventArgs e = new ConnectionChangedEventArgs(this, oldElement, newElement, type);
            this.OwnerDocument.InvokeConnectChanged(e);
        }
        #endregion

        #region ..Interface.BasicShapes.ISVGBasicShape
        /// <summary>
        /// 转换为路径对象
        /// </summary>
        SVG.Interface.Paths.ISVGPathElement Interface.BasicShapes.ISVGBasicShape.ConvertToPath()
        {
            return null;
        }
        #endregion

        #region ..ISVGContainer
        bool Interface.ISVGContainer.ValidChild(Interface.ISVGElement child)
        {
            return (child is SVGBranchElement && !(child is SVGConnectionElement)) || child is Text.SVGTextBlockElement;
        }
        #endregion

        #region ..CalculateGPath
        public virtual void CalculateGPath()
        {
            PointF? preStart = null;
            if (this.ParentElement is SVGBranchElement)
            {
                SVGBranchElement branch = this.ParentElement as SVGBranchElement;
                if (branch.connectionPath != null && branch.connectionPath.PointCount > 1)
                    preStart = branch.connectionPath.GetLastPoint();
            }
            this.CalculateGPath(null, -1, preStart);
        }

        public void CalculateGPath(SVGTransformableElement startElement, int startIndex, PointF? preStartPoint)
        {
            if (this.graphicsPath != null)
                return;

            PointF endPoint = new PointF(this.x2.Value, this.y2.Value);
            PointF startPoint = new PointF(x1.Value, y1.Value);
            bool startCenter = false;
            bool endCenter = false;

            //create the control points
            List<PointF> controls = new List<PointF>();
            if (this.controlPoints != null)
                controls.AddRange(this.controlPoints.GetGDIPoints());
            PointF c1 = endPoint;
            PointF c2 = startPoint;

            //Dynamic connect
            #region ..动态连接线
            if (this.type == SVG.ConnectionType.Dynamic || this.type == SVG.ConnectionType.Line)
            {
                if (this.graphicsPath == null)
                    this.graphicsPath = new GraphicsPath();

                needAdjustPath = true;
                this.graphicsPath.Reset();

                //没有分叉
                PointF end = endPoint;
                PointF preStart = startPoint;
                if (preStartPoint.HasValue)
                    preStart = preStartPoint.Value;
                if(controls.Count == 1 && (controls[0] == startPoint || controls[0] == endPoint))
                    controls.Clear();
                GetInitializePath(this.graphicsPath, controls, preStart, end, this.type, startElement as SVGTransformableElement, this.EndElement as SVGTransformableElement, startIndex, this.endIndex, this);
                this.connectionPath.Reset();
                if (this.graphicsPath.PointCount > 1)
                    this.connectionPath.AddPath(this.graphicsPath, false);

                PointF p = preStart;
                foreach (SVGElement connect in this.ChildElements)
                {
                    if (connect is SVGBranchElement)
                    {
                        (connect as SVGBranchElement).CalculateGPath(null, -1, this.connectionPath.GetLastPoint());
                        this.graphicsPath.AddPath((connect as SVGBranchElement).graphicsPath, false);
                    }
                }
            }
            #endregion

            #region ..其他类型
            else
            {
                //开始对象
                if (startElement != null)
                {
                    RectangleF rect = this.GetPathBounds(startElement as Interface.ISVGPathable);
                    PointF center1 = new PointF(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);
                    PointF[] ps = (startElement as SVG.SVGTransformableElement).ConnectionPoints.Clone() as PointF[];
                    if (!((startElement as SVGTransformableElement).IsCenterConnectPoint(startIndex)) 
                        && startIndex >= 0 
                        && ps.Length > 0)
                    {
                        startIndex = startIndex < ps.Length ? startIndex : ps.Length - 1;
                        using (System.Drawing.Drawing2D.Matrix matrix = (startElement as SVG.SVGTransformableElement).TotalMatrix)
                        {
                            matrix.TransformPoints(ps);
                            center1 = ps[startIndex];
                        }
                    }
                    else
                        startCenter = true;
                    startPoint = center1;
                }

                //结束对象
                if (this.EndElement is SVGTransformableElement)
                {
                    RectangleF rect = this.GetPathBounds(this.endElement as Interface.ISVGPathable);
                    PointF center1 = new PointF(rect.X + rect.Width / 2f, rect.Y + rect.Height / 2f);
                    PointF[] ps = (this.endElement as SVG.SVGTransformableElement).ConnectionPoints.Clone() as PointF[];
                    if (!((this.endElement as SVGTransformableElement).IsCenterConnectPoint(this.endIndex)) 
                        && endIndex >= 0 
                        && ps.Length > 0)
                    {
                        endIndex = endIndex < ps.Length ? endIndex : ps.Length - 1;
                        using (System.Drawing.Drawing2D.Matrix matrix = (this.endElement as SVG.SVGTransformableElement).TotalMatrix)
                        {
                            matrix.TransformPoints(ps);
                            center1 = ps[this.endIndex];
                        }
                    }
                    else
                        endCenter = true;
                    endPoint = center1;
                }

                //create the initial path if need
                this.CreateInitialPath(startPoint, endPoint);

                if (this.graphicsPath == null)
                    this.graphicsPath = new GraphicsPath();

                needAdjustPath = true;
                this.graphicsPath.Reset();

                if (this.controlPoints != null)
                {
                    controls.Clear();
                    controls.AddRange(this.controlPoints.GetGDIPoints());
                }
                if (controls != null && controls.Count > 0)
                {
                    c1 = controls[0];
                    //adjust the control points
                    //if (this.type == SVGDom.ConnectionType.Polyline)
                    //    c1 = new PointF(c1.X, startPoint.Y);
                    //else if (this.type == SVGDom.ConnectionType.VPolyline)
                    //    c1 = new PointF(startPoint.X, c1.Y);
                }
                if (controls != null && controls.Count > 1)
                {
                    c2 = controls[1];
                    //adjust the control points
                    //if (this.type == SVGDom.ConnectionType.Polyline)
                    //    c2 = new PointF(c2.X, endPoint.Y);
                    //else if (this.type == SVGDom.ConnectionType.VPolyline)
                    //    c2 = new PointF(endPoint.X, c2.Y);
                }

                if (startCenter)
                {
                    using (System.Drawing.Drawing2D.GraphicsPath path = (startElement as SVG.Interface.ISVGPathable).GPath.Clone() as GraphicsPath)
                    {
                        path.Transform((startElement as SVG.SVGTransformableElement).TotalTransform);
                        PointF[] ps = SVG.PathHelper.GetTheCrossPoints(startPoint, c1, path, c1);
                        if (ps != null && ps.Length == 1)
                            startPoint = ps[0];
                    }
                }

                if (endCenter)
                {
                    using (System.Drawing.Drawing2D.GraphicsPath path = (this.endElement as SVG.Interface.ISVGPathable).GPath.Clone() as GraphicsPath)
                    {
                        path.Transform((this.endElement as SVG.SVGTransformableElement).TotalTransform);
                        PointF[] ps = SVG.PathHelper.GetTheCrossPoints(c2, endPoint, path, c2);
                        if (ps != null && ps.Length == 1)
                            endPoint = ps[0];
                    }
                }
            }
            #endregion

            //create the path
            switch (this.type)
            {
                //case SVGDom.ConnectionType.Polyline:
                //case SVGDom.ConnectionType.VPolyline:
                //    PointF[] ps1 = new PointF[4];
                //    ps1[0] = startPoint;
                //    ps1[1] = c1;
                //    ps1[2] = c2;
                //    ps1[3] = endPoint;
                //    this.graphicsPath.AddLines(ps1);
                //    break;
                case SVG.ConnectionType.FreeLine:
                    List<PointF> controlPoints = new List<PointF>();
                    controlPoints.Add(startPoint);
                    controlPoints.AddRange(controls);
                    controlPoints.Add(endPoint);
                    this.graphicsPath.AddLines(controlPoints.ToArray());
                    break;
                case SVG.ConnectionType.Spline:
                    this.graphicsPath.AddBezier(startPoint, c1, c2, endPoint);
                    //this.anchors = new PointF[]{startPoint,c1,c2,endPoint};
                    break;
            }
            if (type != ConnectionType.Line && type != ConnectionType.Dynamic)
            {
                this.connectionPath.Reset();
                if (this.graphicsPath.PointCount > 1)
                    this.connectionPath.AddPath(this.graphicsPath, false);
            }
            this.first = false;
            this.anchors = null;
            if (this.graphicsPath != null && this.graphicsPath.PointCount > 1)
                this.anchors = this.graphicsPath.PathPoints.Clone() as PointF[];
            //this.CurrentTime = this.OwnerDocument.CurrentTime;
        }
        #endregion

        #region ..获取连接对象的总体变换
        public RectangleF GetPathBounds(SVG.Interface.ISVGPathable render)
        {
            if (render == null)
                return RectangleF.Empty;
            using (GraphicsPath path = (render as SVG.Interface.ISVGPathable).GPath.Clone() as GraphicsPath)
            {
                using (Matrix matrix = (render as SVGTransformableElement).TotalMatrix)
                {
                    RectangleF rect = path.GetBounds();
                    path.Reset();
                    path.AddRectangle(rect);
                    path.Transform(matrix);
                    return path.GetBounds();
                }
            }
        }
        #endregion 

        #region ..GetInitializePath
        public static void GetInitializePath(GraphicsPath resultPath, PointF startPoint, PointF endPoint, SVG.ConnectionType type, SVGTransformableElement startElement, SVGTransformableElement endElement)
        {
            GetInitializePath(resultPath, null, startPoint, endPoint, type, startElement, endElement, 0, 0, null);
        }

        public static void GetInitializePath(GraphicsPath resultPath, List<PointF> controls, PointF startPoint, PointF endPoint, SVG.ConnectionType type, SVGTransformableElement startElement, SVGTransformableElement endElement, int startIndex, int endIndex)
        {
            GetInitializePath(resultPath, controls, startPoint, endPoint, type, startElement, endElement, startIndex, endIndex, null);
        }

        public static void GetInitializePath(GraphicsPath resultPath, List<PointF> controls, PointF startPoint, PointF endPoint, SVG.ConnectionType type, SVGTransformableElement startElement, SVGTransformableElement endElement, int startIndex, int endIndex, SVGBranchElement refConnect)
        {
            if (resultPath == null)
                return;
            resultPath.Reset();
            if (controls == null)
                controls = new List<PointF>();

            List<PointF> centerControls = new List<PointF>();
            //如果已经存在控制点，检查是否OK
            PointF startCenterPoint = startPoint, endCenterPoint = endPoint;
            PointF[] startBoundPoints = { startPoint }, endBoundPoints = { endPoint };
            PointF[] startAnchors = { startPoint }, endAnchors = { endPoint };
            bool startCenter = false, endCenter = false;

            #region ..根据开始结束对象，计算相关参数
            if (startElement != null)
            {
                PointF[] ps = { };
                if (startElement.ConnectionPoints != null)
                    ps = startElement.ConnectionPoints.Clone() as PointF[];

                RectangleF startBounds = RectangleF.Empty;
                var gpath = (startElement as SVG.Interface.ISVGPathable).GPath;
                if (gpath != null)
                {
                    startBounds = gpath.GetBounds();
                    PointF leftTop = startBounds.Location;
                    startCenterPoint = new PointF(startBounds.X + startBounds.Width / 2, startBounds.Y + startBounds.Height / 2);
                    PointF leftBottom = new PointF(startBounds.X, startBounds.Bottom);
                    PointF rightTop = new PointF(startBounds.Right, startBounds.Top);
                    PointF rightBottom = new PointF(startBounds.Right, startBounds.Bottom);
                    startBoundPoints = new PointF[] { leftTop, rightTop, rightBottom, leftBottom };
                    startCenter = startIndex < 0;
                    //int index = startIndex < 0 ? 0 : (startIndex >= ps.Length ? ps.Length - 1 : startIndex);
                    using (System.Drawing.Drawing2D.Matrix matrix = (startElement as SVG.SVGTransformableElement).TotalMatrix)
                    {
                        if (ps.Length > 0)
                            matrix.TransformPoints(ps);
                        matrix.TransformPoints(startBoundPoints);
                        PointF[] ps1 = new PointF[] { startCenterPoint };
                        matrix.TransformPoints(ps1);
                        startCenterPoint = ps1[0];
                        if (startIndex >= 0 && startIndex < ps.Length)
                            startPoint = ps[startIndex];
                        else
                            startPoint = startCenterPoint;
                        startAnchors = new PointF[] { startPoint };
                    }

                    if (startCenter)
                    {
                        startAnchors = ps;
                        //startAnchors[startIndex] = PointF.Empty;
                    }
                }
                startElement.AddRefedConnects(refConnect);
            }

            if (endElement != null)
            {
                PointF[] ps = { };
                if (endElement.ConnectionPoints != null)
                    ps = endElement.ConnectionPoints.Clone() as PointF[];
                RectangleF startBounds = RectangleF.Empty;
                var gpath = (endElement as SVG.Interface.ISVGPathable).GPath;
                if (gpath != null)
                {
                    startBounds = gpath.GetBounds();
                    endCenterPoint = new PointF(startBounds.X + startBounds.Width / 2, startBounds.Y + startBounds.Height / 2);
                    PointF leftTop = startBounds.Location;
                    PointF leftBottom = new PointF(startBounds.X, startBounds.Bottom);
                    PointF rightTop = new PointF(startBounds.Right, startBounds.Top);
                    PointF rightBottom = new PointF(startBounds.Right, startBounds.Bottom);
                    endBoundPoints = new PointF[] { leftTop, rightTop, rightBottom, leftBottom };
                    endCenter = endIndex < 0;
                    //int index = endIndex < 0 ? 0 : (endIndex >= ps.Length ? ps.Length - 1 : endIndex);
                    using (System.Drawing.Drawing2D.Matrix matrix = (endElement as SVG.SVGTransformableElement).TotalMatrix)
                    {
                        if (ps.Length > 0)
                            matrix.TransformPoints(ps);
                        matrix.TransformPoints(endBoundPoints);
                        PointF[] ps1 = new PointF[] { endCenterPoint };
                        matrix.TransformPoints(ps1);
                        endCenterPoint = ps1[0];
                        if (endIndex >= 0 && endIndex < ps.Length)
                            endPoint = ps[endIndex];
                        else
                            endPoint = endCenterPoint;
                        endAnchors = new PointF[] { endPoint };
                    }
                    if (endCenter)
                    {
                        endAnchors = ps;
                        //endAnchors[endIndex] = PointF.Empty;
                    }
                }
                endElement.AddRefedConnects(refConnect);
            }
            #endregion

            switch (type)
            {
                case ConnectionType.Line:
                    //连接到形状
                    if (startCenter && startElement != null && (startElement as SVG.Interface.ISVGPathable).GPath != null && (startElement as SVG.Interface.ISVGPathable).GPath.PointCount > 1)
                    {
                        using (System.Drawing.Drawing2D.GraphicsPath path = (startElement as SVG.Interface.ISVGPathable).GPath.Clone() as GraphicsPath)
                        {
                            path.Transform(startElement.TotalTransform);
                            PointF[] ps = SVG.PathHelper.GetTheCrossPoints(startPoint, endPoint, path, endPoint);
                            if (ps != null && ps.Length == 1)
                                startPoint = ps[0];
                        }
                    }

                    if (endCenter && endElement != null && (endElement as SVG.Interface.ISVGPathable).GPath != null && (endElement as SVG.Interface.ISVGPathable).GPath.PointCount > 1)
                    {
                        //end point
                        using (System.Drawing.Drawing2D.GraphicsPath path = (endElement as SVG.Interface.ISVGPathable).GPath.Clone() as GraphicsPath)
                        {
                            path.Transform(endElement.TotalTransform);
                            PointF[] ps = SVG.PathHelper.GetTheCrossPoints(startPoint, endPoint, path, startPoint);
                            if (ps != null && ps.Length == 1)
                                endPoint = ps[0];
                        }
                    }
                    PointF[] tempPs = { startPoint, endPoint };
                    resultPath.AddLine(tempPs[0], tempPs[1]);
                    break;
                case ConnectionType.Dynamic:
                    //当从点连接到形状时，走连接形状逻辑
                    if (startCenter != endCenter && ((startElement == null) != (endElement == null)))
                        startCenter = endCenter = true;

                    #region ..处理连接到形状的情况
                    if (startCenter || endCenter)
                    {
                        #region ..两端都连接到形状
                        if (startCenter == endCenter)
                        {
                            var query = from t in startBoundPoints select t;
                            float minX = query.Select<PointF, float>(q => q.X).Min();
                            float maxX = query.Select<PointF, float>(q => q.X).Max();
                            float minY = query.Select<PointF, float>(q => q.Y).Min();
                            float maxY = query.Select<PointF, float>(q => q.Y).Max();
                            query = from t in endBoundPoints select t;
                            float minX1 = query.Select<PointF, float>(q => q.X).Min();
                            float maxX1 = query.Select<PointF, float>(q => q.X).Max();
                            float minY1 = query.Select<PointF, float>(q => q.Y).Min();
                            float maxY1 = query.Select<PointF, float>(q => q.Y).Max();

                            //水平方向是否有交集
                            float min = minX < minX1 ? minX : minX1;
                            float max = maxX > maxX1 ? maxX : maxX1;
                            RectangleF rect = new RectangleF(min, minY, max - min, maxY - minY);
                            RectangleF rect1 = new RectangleF(min, minY1, max - min, maxY1 - minY1);
                            rect1.Intersect(rect);
                            if (rect1.Height > 0 || rect1.Width > 1)
                            {
                                float y = rect1.Y + rect1.Height / 2;
                                PointF p1 = new PointF(min - 100, y);
                                PointF p2 = new PointF(max + 100, y);
                                //start point
                                if (startElement != null && (startElement as SVG.Interface.ISVGPathable).GPath != null && (startElement as SVG.Interface.ISVGPathable).GPath.PointCount > 1)
                                {
                                    using (System.Drawing.Drawing2D.GraphicsPath path = (startElement as SVG.Interface.ISVGPathable).GPath.Clone() as GraphicsPath)
                                    {
                                        path.Transform(startElement.TotalTransform);
                                        PointF[] ps = SVG.PathHelper.GetTheCrossPoints(p1, p2, path, new PointF((minX1 + maxX1) / 2, y));
                                        if (ps != null && ps.Length == 1)
                                            startPoint = new PointF(ps[0].X, y);
                                    }
                                }

                                if (endElement != null && (endElement as SVG.Interface.ISVGPathable).GPath != null && (endElement as SVG.Interface.ISVGPathable).GPath.PointCount > 1)
                                {
                                    //end point
                                    using (System.Drawing.Drawing2D.GraphicsPath path = (endElement as SVG.Interface.ISVGPathable).GPath.Clone() as GraphicsPath)
                                    {
                                        path.Transform(endElement.TotalTransform);
                                        PointF[] ps = SVG.PathHelper.GetTheCrossPoints(p1, p2, path, new PointF((minX + maxX) / 2, y));
                                        if (ps != null && ps.Length == 1)
                                            endPoint = new PointF(ps[0].X, y);
                                    }
                                }
                            }
                            //竖直方向是否有交集
                            else
                            {
                                min = minY < minY1 ? minY : minY1;
                                max = maxY > maxY1 ? maxY : maxY1;
                                rect = new RectangleF(minX, min, maxX - minX, max - min);
                                rect1 = new RectangleF(minX1, min, maxX1 - minX1, max - min);
                                rect1.Intersect(rect);
                                if (rect1.Height > 0 || rect1.Width > 1)
                                {
                                    float x = rect1.X + rect1.Width / 2;
                                    PointF p1 = new PointF(x, min - 100);
                                    PointF p2 = new PointF(x, max + 100);
                                    //start point
                                    if (startElement != null && (startElement as SVG.Interface.ISVGPathable).GPath != null && (startElement as SVG.Interface.ISVGPathable).GPath.PointCount > 1)
                                    {
                                        using (System.Drawing.Drawing2D.GraphicsPath path = (startElement as SVG.Interface.ISVGPathable).GPath.Clone() as GraphicsPath)
                                        {
                                            path.Transform(startElement.TotalTransform);
                                            PointF[] ps = SVG.PathHelper.GetTheCrossPoints(p1, p2, path, new PointF(x, (minY1 + maxY1) / 2));
                                            if (ps != null && ps.Length == 1)
                                                startPoint = new PointF(x, ps[0].Y);
                                        }
                                    }

                                    //end point
                                    if (endElement != null && (endElement as SVG.Interface.ISVGPathable).GPath != null && (endElement as SVG.Interface.ISVGPathable).GPath.PointCount > 1)
                                    {
                                        using (System.Drawing.Drawing2D.GraphicsPath path = (endElement as SVG.Interface.ISVGPathable).GPath.Clone() as GraphicsPath)
                                        {
                                            path.Transform(endElement.TotalTransform);
                                            PointF[] ps = SVG.PathHelper.GetTheCrossPoints(p1, p2, path, new PointF(x, (minY + maxY) / 2));
                                            if (ps != null && ps.Length == 1)
                                                endPoint = new PointF(x, ps[0].Y);
                                        }
                                    }
                                }
                                //没有交集
                                else
                                {
                                    PointF startCorner = new PointF(endCenterPoint.X, startCenterPoint.Y);
                                    PointF endCorner = new PointF(startCenterPoint.X, endCenterPoint.Y);

                                    PointF? startHCross = startPoint, startVCross = startPoint;
                                    //取开始对象在水平和垂直方向的交点
                                    if (startElement != null && (startElement as SVG.Interface.ISVGPathable).GPath != null && (startElement as SVG.Interface.ISVGPathable).GPath.PointCount > 1)
                                    {
                                        using (System.Drawing.Drawing2D.GraphicsPath path = (startElement as SVG.Interface.ISVGPathable).GPath.Clone() as GraphicsPath)
                                        {
                                            path.Transform(startElement.TotalTransform);
                                            PointF[] ps = SVG.PathHelper.GetTheCrossPoints(startCenterPoint, startCorner, path, startCorner);
                                            if (ps != null && ps.Length == 1)
                                                startHCross = new PointF(ps[0].X, startCenterPoint.Y);

                                            ps = SVG.PathHelper.GetTheCrossPoints(startCenterPoint, endCorner, path, endCorner);
                                            if (ps != null && ps.Length == 1)
                                                startVCross = new PointF(startCenterPoint.X, ps[0].Y);
                                        }
                                    }

                                    PointF? endHCross = endPoint, endVCross = endPoint;
                                    //取结束对象在水平和垂直方向的交点
                                    if (endElement != null && (endElement as SVG.Interface.ISVGPathable).GPath != null && (endElement as SVG.Interface.ISVGPathable).GPath.PointCount > 1)
                                    {
                                        using (System.Drawing.Drawing2D.GraphicsPath path = (endElement as SVG.Interface.ISVGPathable).GPath.Clone() as GraphicsPath)
                                        {
                                            path.Transform(endElement.TotalTransform);
                                            PointF[] ps = SVG.PathHelper.GetTheCrossPoints(endCenterPoint, endCorner, path, endCorner);
                                            if (ps != null && ps.Length == 1)
                                                endHCross = new PointF(ps[0].X, endCenterPoint.Y);

                                            ps = SVG.PathHelper.GetTheCrossPoints(endCenterPoint, startCorner, path, startCorner);
                                            if (ps != null && ps.Length == 1)
                                                endVCross = new PointF(endCenterPoint.X, ps[0].Y);
                                        }
                                    }
                                    //计算两条折线的长度并去最小长度为结果
                                    float? distance1 = null, distance2 = null;
                                    if (startHCross.HasValue && endVCross.HasValue)
                                        distance1 = PathHelper.Distance(startHCross.Value, endVCross.Value);

                                    if (startVCross.HasValue && endHCross.HasValue)
                                        distance2 = PathHelper.Distance(startVCross.Value, endHCross.Value);

                                    PointF? controlPoint = null;
                                    float dis = 1200000000;
                                    if (distance1.HasValue)
                                    {
                                        startPoint = startHCross.Value;
                                        endPoint = endVCross.Value;
                                        dis = distance1.Value;
                                        controlPoint = startCorner;
                                    }

                                    if (distance2.HasValue && distance2.Value < dis)
                                    {
                                        startPoint = startVCross.Value;
                                        endPoint = endHCross.Value;
                                        controlPoint = endCorner;
                                    }

                                    //直接生成，不再进行路径试探
                                    if (controlPoint.HasValue && controls.Count == 0)
                                        controls.Add(controlPoint.Value);
                                }
                            }
                            startBoundPoints = endBoundPoints = null;
                        }
                        #endregion

                        #region ..只有一端连接到形状
                        else
                        {
                            PointF start1 = startPoint, end1 = endPoint;
                            float? preDistance = null;
                            if (startAnchors.Length > 0 && endAnchors.Length > 0)
                            {
                                foreach (PointF p1 in startAnchors)
                                {
                                    if (p1.IsEmpty)
                                        continue;
                                    foreach (PointF p2 in endAnchors)
                                    {
                                        if (p2.IsEmpty)
                                            continue;
                                        float distance = PathHelper.Distance(p1, p2);
                                        bool assign = !preDistance.HasValue || preDistance.Value > distance;
                                        if (assign)
                                        {
                                            preDistance = distance;
                                            start1 = p1;
                                            end1 = p2;
                                        }
                                    }
                                }
                            }
                                //计算相交
                            else
                            {
                                Interface.ISVGPathable pathable = null;
                                PointF[] anchors = null;
                                //连接到开始节点形状
                                if (startCenter)
                                {
                                    pathable = startElement as Interface.ISVGPathable;
                                    anchors = endAnchors;
                                    startBoundPoints = null;
                                    if (pathable != null && anchors.Length > 0)
                                    {
                                        using (GraphicsPath path = pathable.GPath.Clone() as GraphicsPath)
                                        {
                                            path.Transform(startElement.TotalTransform);
                                            PointF basePoint = endAnchors[0];
                                            PointF? start = null;
                                            //水平
                                            PointF startCorner = new PointF(basePoint.X, startCenterPoint.Y);
                                            PointF[] ps = SVG.PathHelper.GetTheCrossPoints(basePoint, startCorner, path, basePoint);
                                            if (ps != null && ps.Length == 1)
                                                start = new PointF(basePoint.X, ps[0].Y);

                                            startCorner = new PointF(startCenterPoint.X, basePoint.Y);
                                            ps = SVG.PathHelper.GetTheCrossPoints(basePoint, startCorner, path, basePoint);
                                            if (ps != null && ps.Length == 1)
                                            {
                                                PointF p1 = new PointF(ps[0].X, basePoint.Y);

                                                if (start.HasValue)
                                                {
                                                    if (PathHelper.Distance(p1, basePoint) < PathHelper.Distance(start.Value, basePoint))
                                                        start = p1;
                                                }
                                                else
                                                    start = p1;
                                            }

                                            if (start.HasValue)
                                                start1 = start.Value;
                                        }
                                    }
                                }
                                else if (endCenter)
                                {
                                    pathable = endElement as Interface.ISVGPathable;
                                    anchors = startAnchors;
                                    if (pathable != null && anchors.Length > 0)
                                    {
                                        using (GraphicsPath path = pathable.GPath.Clone() as GraphicsPath)
                                        {
                                            path.Transform(endElement.TotalTransform);
                                            PointF basePoint = anchors[0];
                                            PointF? end = null;
                                            //水平
                                            PointF startCorner = new PointF(basePoint.X, endCenterPoint.Y);
                                            PointF[] ps = SVG.PathHelper.GetTheCrossPoints(basePoint, startCorner, path, basePoint);
                                            if (ps != null && ps.Length == 1)
                                                end = new PointF(basePoint.X, ps[0].Y);

                                            startCorner = new PointF(endCenterPoint.X, basePoint.Y);
                                            ps = SVG.PathHelper.GetTheCrossPoints(basePoint, startCorner, path, basePoint);
                                            if (ps != null && ps.Length == 1)
                                            {
                                                PointF p1 = new PointF(ps[0].X, basePoint.Y);
                                                if (end.HasValue)
                                                {
                                                    if (PathHelper.Distance(p1, basePoint) < PathHelper.Distance(end.Value, basePoint))
                                                        end = p1;
                                                }
                                                else
                                                    end = p1;
                                            }

                                            if (end.HasValue)
                                            {
                                                end1 = end.Value;
                                                endBoundPoints = null;
                                            }
                                        }
                                    }
                                   
                                }

                                if (endBoundPoints == null && startBoundPoints == null)
                                {
                                    controls.Add(start1);
                                    controls.Add(end1);
                                }
                            }

                            startPoint = start1;
                            endPoint = end1;
                            //}
                        }
                        #endregion
                    }
                    #endregion

                    //如果存在控制点，则以控制点为基准，以最后一个控制点开始计算后面路径
                    if (controls != null && controls.Count > 0)
                    {
                        if (controls.Count > 0 && refConnect != null && refConnect.connectionPath != null && refConnect.connectionPath.PointCount > 2)
                        {
                            if (Math.Abs(refConnect.connectionPath.PathPoints[0].X - refConnect.connectionPath.PathPoints[1].X) < 1)
                                controls.Insert(0, new PointF(startPoint.X, controls[0].Y));
                            else //if(refConnect.anchors[0].Y == refConnect.anchors[1].Y)
                                controls.Insert(0, new PointF(controls[0].X, startPoint.Y));
                            controls.Insert(0, startPoint);
                            //if (Math.Abs(refConnect.connectionPath.PathPoints.Last().X - refConnect.connectionPath.PathPoints[refConnect.connectionPath.PointCount- 2].X) < 1)
                            //    controls.Add(new PointF(endPoint.X, controls.Last().Y));
                            //else //if (refConnect.anchors.Last().Y == refConnect.anchors[refConnect.anchors.Length - 2].Y)
                            //    controls.Add(new PointF(controls.Last().X, endPoint.Y));
                            //controls.Add(endPoint);
                        }
                        else
                        //{
                        //    controls.Insert(0, startPoint);
                        //    controls.Add(endPoint);
                        //}
                        controls.Insert(0, startPoint);
                        NormalizeControlPoints(controls);
                        startPoint = controls.Last();
                        startBoundPoints = null;
                        //resultPath.AddLines(controls.ToArray());
                        //break;
                    }
                    else
                        controls.Clear();

                    #region ..遍历可能路径，寻找最短路径
                    {
                        List<List<PointF>> possibleControls = new List<List<PointF>>();
                        PointF[][] boundPoints = { startBoundPoints, endBoundPoints };
                        PointF[] initPoints = { startPoint, endPoint };
                        List<List<PointF>> controlPoints = new List<List<PointF>>();
                        possibleControls.Add(new List<PointF>());
                        possibleControls.Add(new List<PointF>());

                        PointF?[] allBoundPoints = new PointF?[8];

                        RectangleF first = RectangleF.Empty, second = RectangleF.Empty;
                        //求开始和结束的可能控制点
                        for (int i = 0; i < 2; i++)
                        {
                            if (boundPoints[i] != null && boundPoints[i].Length > 1)
                            {
                                using (GraphicsPath path = new GraphicsPath())
                                {
                                    //边界
                                    path.AddPolygon(boundPoints[i]);
                                    RectangleF bounds = path.GetBounds();
                                    
                                    Rectangle rect1 = Rectangle.Truncate(bounds);
                                    bounds = new RectangleF(bounds.X - DynamicConnectShapeMargin, bounds.Y - DynamicConnectShapeMargin, bounds.Width + 2 * DynamicConnectShapeMargin, bounds.Height + 2 * DynamicConnectShapeMargin);
                                    PointF initPoint = initPoints[i];
                                    PointF[] dirPoints = { new PointF(bounds.Left, initPoint.Y), new PointF(initPoint.X, bounds.Top), new PointF(bounds.Right, initPoint.Y), new PointF(initPoint.X, bounds.Bottom) };

                                    //如果连接到形状
                                    if ((i == 0 && startCenter) ||　(i == 1 && endCenter))
                                        possibleControls[i].AddRange(dirPoints);
                                    else
                                    {
                                        //直线
                                        PointF leftTop = boundPoints[i][0], rightTop = boundPoints[i][1];
                                        PointF rightBottom = boundPoints[i][2], leftBottom = boundPoints[i][3];
                                        PointF crossPoint = PointF.Empty;
                                        for (int j = 0; j < 4; j++)
                                        {
                                            PointF dirPoint = dirPoints[j];
                                            int r1 = SVG.PathHelper.LineIntersection(leftTop, leftBottom, initPoint, dirPoint, out crossPoint);
                                            int r2 = SVG.PathHelper.LineIntersection(leftTop, rightTop, initPoint, dirPoint, out crossPoint);
                                            int r3 = SVG.PathHelper.LineIntersection(rightTop, rightBottom, initPoint, dirPoint, out crossPoint);
                                            int r4 = SVG.PathHelper.LineIntersection(rightBottom, leftBottom, initPoint, dirPoint, out crossPoint);

                                            int r = r1 + r2 + r3 + r4;
                                            //只和一条边相交，则是可能的控制点
                                            if (r == 2 && !possibleControls[i].Contains(dirPoint))
                                            {
                                                possibleControls[i].Add(dirPoint);
                                            }
                                        }

                                    }
                                    if (i == 0)
                                        first = bounds;
                                    else
                                        second = bounds;
                                    Array.Copy(new PointF?[] { bounds.Location, new PointF(bounds.Right, bounds.Top), new PointF(bounds.Right, bounds.Bottom), new PointF(bounds.Left, bounds.Bottom) }, 0, allBoundPoints, i * 4, 4);
                                }
                            }
                            if (possibleControls[i].Count == 0)
                                possibleControls[i].Add(initPoints[i]);
                        }
                        List<List<PointF>> resultControls = new List<List<PointF>>();
                        List<int> numberOfCross = new List<int>();
                        for (int i = 0; i < possibleControls[0].Count; i++)
                        {
                            PointF startControl = possibleControls[0][i];
                            float x = startControl.X;
                            float y = startControl.Y;

                            for (int j = 0; j < possibleControls[1].Count; j++)
                            {
                                PointF endControl = possibleControls[1][j];
                                PointF start = startControl;
                                bool swap = false;
                                if (start.X > endControl.X)
                                {
                                    OperatorHelper<PointF>.Swap(ref start, ref endControl);
                                    swap = true;
                                }

                                PointF[] temp = { start, endControl };
                                float[] xValues = { }, yValues = { };
                                var query = (from t in allBoundPoints where t.HasValue select t.Value.X).Union(from t in temp select t.X);
                                xValues = query.Distinct<float>().OrderBy<float, float>(q => q).ToArray();

                                query = (from t in allBoundPoints where t.HasValue select t.Value.Y).Union(from t1 in temp select t1.Y);
                                yValues = query.Distinct<float>().OrderBy<float, float>(q => q).ToArray();

                                float preX = start.X;
                                int xIndex = Array.IndexOf(xValues, preX);

                                int minIndex = 0, maxIndex = yValues.Length;
                                int yIndex = Array.IndexOf(yValues, start.Y);

                                for (int h = minIndex; h < maxIndex; h++)
                                {
                                    float yValue = h < 0 ? start.Y : yValues[h];
                                    List<PointF> candidates = new List<PointF>();
                                    resultControls.Add(candidates);
                                    candidates.Add(startPoint);
                                    candidates.Add(start);
                                    bool lastV = false;
                                    for (int k = xIndex; k < xValues.Length; k++)
                                    {
                                        x = xValues[k];
                                        PointF last = candidates.Last();
                                        y = yValue;
                                        if (x == endControl.X)
                                        {
                                            float min = last.Y < endControl.Y ? last.Y : endControl.Y;
                                            float max = last.Y > endControl.Y ? last.Y : endControl.Y;
                                            float minX = last.X < endControl.X ? last.X : endControl.X;
                                            float maxX = last.X > endControl.X ? last.X : endControl.X;

                                            bool canConnect = !(maxX >= first.Right && minX < first.Right && min > first.Y && max < first.Bottom);
                                            canConnect = canConnect && !(maxX > first.X && minX <= first.X && min > first.Y && max < first.Bottom);
                                            canConnect = canConnect && !(maxX > second.X && minX <= second.X && min > second.Y && max < second.Bottom);
                                            canConnect = canConnect && !(maxX >= second.Right && minX < second.Right && min > second.Y && max < second.Bottom);

                                            canConnect = canConnect && !(max >= second.Bottom && min < second.Bottom && x > second.X && x < second.Right);
                                            canConnect = canConnect && !(max >= first.Bottom && min < first.Bottom && x > first.X && x < first.Right);
                                            canConnect = canConnect && !(max > first.Y && min <= first.Y && x > first.X && x < first.Right);
                                            canConnect = canConnect && !(max > second.Y && min <= second.Y && x > second.X && x < second.Right);

                                            if (canConnect)
                                            {
                                                candidates.Add(new PointF(x, last.Y));
                                                break;
                                            }
                                        }
                                        if (x == start.X)
                                        {
                                            if ((x == first.X && (y - first.Y) * (y - first.Bottom) < 0)
                                                || (x == second.X && (y - second.Y) * (y - second.Bottom) < 0)
                                                || (x > first.X && x < first.Right && ((start.Y - first.Bottom) * (y - first.Top) < 0 || (start.Y - first.Top) * (y - first.Bottom) < 0))
                                                || (x > second.X && x < second.Right && ((start.Y - second.Bottom) * (y - second.Top) < 0 || (start.Y - second.Top) * (y - second.Bottom) < 0)))
                                            {
                                                resultControls.Remove(candidates);
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            y = last.Y;
                                            if ((x >= first.X && x < first.Right && y > first.Y && y < first.Bottom))
                                            {
                                                bool bY = !(x > second.X && x < second.Right && first.Y > second.Y && first.Y < second.Bottom);
                                                bool bBottom = !(x > second.X && x < second.Right && first.Bottom > second.Y && first.Bottom < second.Bottom);
                                                //上下交点均横穿First
                                                if (!bY && !bBottom)
                                                {
                                                    resultControls.Remove(candidates);
                                                    break;
                                                }
                                                if (bY && bBottom)
                                                {
                                                    y = first.Y;
                                                    if (Math.Abs(endControl.Y - first.Y) > Math.Abs(endControl.Y - first.Bottom))
                                                        y = first.Bottom;
                                                }
                                                else if (bY)
                                                    y = first.Y;
                                                else
                                                    y = first.Bottom;
                                            }
                                            else if ((x >= second.X && x < second.Right && y > second.Y && y < second.Bottom))
                                            {
                                                bool bY = !(x > first.X && x < first.Right && second.Y > first.Y && second.Y < first.Bottom);
                                                bool bBottom = !(x > first.X && x < first.Right && second.Bottom > first.Y && second.Bottom < first.Bottom);
                                                //上下交点均横穿First
                                                if (!bY && !bBottom)
                                                {
                                                    resultControls.Remove(candidates);
                                                    break;
                                                }
                                                if (bY && bBottom)
                                                {
                                                    y = second.Y;
                                                    if (Math.Abs(endControl.Y - second.Y) > Math.Abs(endControl.Y - second.Bottom))
                                                        y = second.Bottom;
                                                }
                                                else if (bY)
                                                    y = second.Y;
                                                else
                                                    y = second.Bottom;
                                            }
                                            else if (k == xValues.Length - 1)
                                            {
                                                lastV = true;
                                                y = endControl.Y;
                                                if (x == first.Right && y > first.Y && y < first.Bottom && endControl.X != first.Right)
                                                {
                                                    y = first.Y;
                                                    if (Math.Abs(endControl.Y - first.Y) > Math.Abs(endControl.Y - first.Bottom))
                                                        y = first.Bottom;
                                                }

                                                if (x == second.Right && y > second.Y && y < second.Bottom && endControl.X != second.Right)
                                                {
                                                    y = second.Y;
                                                    if (Math.Abs(endControl.Y - second.Y) > Math.Abs(endControl.Y - second.Bottom))
                                                        y = second.Bottom;
                                                }
                                            }

                                            if (last.X != x && last.Y != y)
                                                candidates.Add(new PointF(x, last.Y));
                                        }

                                        last = candidates.Last();

                                        candidates.Add(new PointF(x, y));
                                    }

                                    if (resultControls.Contains(candidates))
                                    {
                                        PointF last1 = candidates.Last<PointF>();
                                        if (last1.X != endControl.X && last1.Y != endControl.Y)
                                        {
                                            PointF pre = candidates[candidates.Count - 2];
                                            if (lastV || (last1.Y == pre.Y && last1.X <= endControl.X))
                                                candidates.Add(new PointF(endControl.X, last1.Y));
                                            else
                                                candidates.Add(new PointF(last1.X, endControl.Y));
                                        }

                                        candidates.Add(endControl);
                                        candidates.Add(endPoint);
                                        if (swap)
                                            candidates.Reverse();

                                        candidates[0] = startPoint;
                                        candidates[candidates.Count - 1] = endPoint;
                                        //Normalize candidates
                                        NormalizeControlPoints(candidates);
                                        numberOfCross.Add(candidates.Count);
                                    }
                                }
                            }
                        }
                        if (resultControls.Count > 0)
                        {
                            int? minIndex = null;

                            if (resultControls.Count > 1)
                            {
                                int[] distances = new int[resultControls.Count];
                                int[] indexes = new int[resultControls.Count];
                                int num = 0;
                                foreach (List<PointF> candidate in resultControls)
                                {
                                    float distance = 0;
                                    for (int i = 1; i < candidate.Count; i++)
                                    {
                                        distance += PathHelper.Distance(candidate[i - 1], candidate[i]);
                                    }
                                    distances[num] = (int)distance;
                                    indexes[num] = num;
                                    num++;
                                }

                                float min = distances.Min();

                                indexes = (from t in indexes where distances[t] == min select t).ToArray<int>();

                                if (indexes.Length > 0)
                                {
                                    int[] indexes1 = new int[indexes.Length];
                                    int[] counts = new int[indexes.Length];
                                    for (int i = 0; i < indexes.Length; i++)
                                    {
                                        List<PointF> candidate = new List<PointF>();
                                        candidate.AddRange(resultControls[indexes[i]]);
                                        candidate.InsertRange(0, controls);
                                        NormalizeControlPoints(candidate);
                                        counts[i] = candidate.Count;
                                        indexes1[i] = i;
                                    }

                                    int minCount = counts.Min();
                                    int index = (from t in indexes1 where counts[t] == minCount select t).FirstOrDefault();
                                    minIndex = indexes[indexes1[index]];
                                }
                                //num = 0;
                                //for (; num < distances.Length; num++)
                                //{
                                //    if (distances[num] == min)
                                //    {
                                //        if (minIndex.HasValue)
                                //        {
                                //            if (numberOfCross[minIndex.Value] > numberOfCross[num])
                                //                minIndex = num;
                                //        }
                                //        else
                                //            minIndex = num;
                                //    }
                                //}
                            }
                            if (!minIndex.HasValue)
                                minIndex = 0;
                            controls.AddRange(resultControls[minIndex.Value]);
                        }
                        else
                        {
                            controls.Add(startPoint);
                            if (startPoint.X != endPoint.X && startPoint.Y != endPoint.Y)
                                controls.Add(new PointF(startPoint.X, endPoint.Y));
                            controls.Add(endPoint);
                        }
                    }
                    #endregion

                    NormalizeControlPoints(controls);

                    //如果是连接到形状，取连接线路径和图元路径的交点
                    if (startCenter && controls.Count >= 2)
                    {
                        var pathable = startElement as Interface.ISVGPathable;
                        if (pathable != null)
                        {
                            using (GraphicsPath path = pathable.GPath.Clone() as GraphicsPath)
                            {
                                path.Transform(startElement.TotalTransform);
                                var p1 = controls.First();
                                var p2 = controls[1];
                                PointF[] ps = SVG.PathHelper.GetTheCrossPoints(p2, p1, path, p2);
                                if (ps != null && ps.Length == 1)
                                {
                                    var p = ps[0];
                                    if (p1.X == p2.X)
                                        p = new PointF(p2.X, p.Y);
                                    if (p1.Y == p2.Y)
                                        p = new PointF(p.X, p2.Y);
                                    controls[0] = p;
                                }
                            }
                        }
                    }

                    if (endCenter && controls.Count >= 2)
                    {
                        var pathable = endElement as Interface.ISVGPathable;
                        if (pathable != null)
                        {
                            using (GraphicsPath path = pathable.GPath.Clone() as GraphicsPath)
                            {
                                path.Transform(endElement.TotalTransform);
                                var p1 = controls[controls.Count - 2];
                                var p2 = controls.Last();
                                PointF[] ps = SVG.PathHelper.GetTheCrossPoints(p2, p1, path, p1);
                                if (ps != null && ps.Length == 1)
                                {
                                    var p = ps[0];
                                    if (p1.X == p2.X)
                                        p = new PointF(p2.X, p.Y);
                                    if (p1.Y == p2.Y)
                                        p = new PointF(p.X, p2.Y);
                                    controls[controls.Count - 1] = p;
                                }
                            }
                        }

                    }
                    resultPath.AddLines(controls.ToArray());
                    break;
            }
        }
        #endregion

        #region ..NormalizeControlPoints
        /// <summary>
        /// 去除在同一条直线上的控制点
        /// </summary>
        public static void NormalizeControlPoints(List<PointF> points)
        {
            for (int k = 2; k < points.Count; k++)
            {
                if ((PathHelper.FloatEqual(points[k].X, points[k - 1].X) && PathHelper.FloatEqual(points[k].X, points[k - 2].X)) || (PathHelper.FloatEqual(points[k].Y, points[k - 1].Y) && PathHelper.FloatEqual(points[k].Y, points[k - 2].Y)))
                {
                    points.RemoveAt(k - 1);
                    k--;
                }
            }

            for (int i = 0; i < points.Count; i++)
                points[i] = Point.Round(points[i]);
        }
        #endregion

        #region ..CreateInitialPath
        /// <summary>
        /// if connect is created and has not the control ponits, create it
        /// </summary>
        public void CreateInitialPath(PointF startPoint, PointF endPoint)
        {
            if (this.first && (!this.HasAttribute("d") || this.type == SVG.ConnectionType.FreeLine))
            {
                float _x3 = 0, _y3 = 0, _x4 = 0, _y4 = 0;
                bool old = this.OwnerDocument.AcceptNodeChanged;
                this.OwnerDocument.AcceptNodeChanged = false;
                switch (this.type)
                {
                    //spline
                    case SVG.ConnectionType.Spline:
                        bool lower = startPoint.X < endPoint.X;
                        //
                        _x3 = startPoint.X + SplineLength;
                        _y3 = startPoint.Y;
                        _x4 = endPoint.X - SplineLength;
                        _y4 = endPoint.Y;
                        this.InternalSetAttribute("d", _x3.ToString() + " " + _y3.ToString() + " " + _x4.ToString() + " " + _y4.ToString());
                        break;
                    //polyline
                    //case SVGDom.ConnectionType.Polyline:
                    //    float x = (startPoint.X + endPoint.X) / 2f;
                    //    _x3 = x;
                    //    _y3 = startPoint.Y;
                    //    _x4 = x;
                    //    _y4 = endPoint.Y;
                    //    this.InternalSetAttribute("d", _x3.ToString() + " " + _y3.ToString() + " " + _x4.ToString() + " " + _y4.ToString());
                    //    break;
                    ////vpolyline
                    //case SVGDom.ConnectionType.VPolyline:
                    //    float y = (startPoint.Y + endPoint.Y) / 2f;
                    //    _y3 = y;
                    //    _x3 = startPoint.X;
                    //    _y4 = y;
                    //    _x4 = endPoint.X;
                    //    this.InternalSetAttribute("d", _x3.ToString() + " " + _y3.ToString() + " " + _x4.ToString() + " " + _y4.ToString());
                    //    break;
                    case SVG.ConnectionType.FreeLine:
                        this.first = false;
                        //if has the initial d attribute, update this attribute
                        if (this.HasCustomControlPoints())
                        {
                            //calculate the original rectangle
                            _x3 = this.oriX1;
                            _y3 = this.oriY1;
                            _x4 = this.oriX2;
                            _y4 = this.oriY2;
                            float x2 = (float)Math.Min(_x3, _x4);
                            float y2 = (float)Math.Min(_y3, _y4);
                            float width = (float)Math.Max(_x3, _x4) - x2;
                            float height = (float)Math.Max(_y3, _y4) - y2;
                            RectangleF rect = new RectangleF(x2, y2, width, height);

                            //only when the rect is not empty
                            if (!rect.IsEmpty)
                            {
                                //calculate the target rectangle
                                x2 = (float)Math.Min(startPoint.X, endPoint.X);
                                y2 = (float)Math.Min(startPoint.Y, endPoint.Y);
                                width = (float)Math.Max(startPoint.X, endPoint.X) - x2;
                                height = (float)Math.Max(startPoint.Y, endPoint.Y) - y2;
                                PointF[] ctlPoints = new PointF[] { new PointF(x2, y2), new PointF(x2 + width, y2), new PointF(x2, y2 + height) };

                                //transform the control points
                                using (Matrix matrix = new Matrix(rect, ctlPoints))
                                {
                                    PointF[] ps1 = this.controlPoints.GetGDIPoints();
                                    matrix.TransformPoints(ps1);
                                    string ptr = SVG.DataType.SVGPointList.GetPointsString(ps1);
                                    this.InternalSetAttribute("d", ptr);
                                    this.controlPoints = new SVG.DataType.SVGPointList(ptr);
                                }
                            }
                        }
                        break;
                }
                this.OwnerDocument.AcceptNodeChanged = old;
            }
            this.first = false;
        }
        #endregion

        #region ..HasCustomControlPoints
        public bool HasCustomControlPoints()
        {
            return this.HasAttribute("d");
        }
        #endregion

        #region ..childRenders_CollectionChanged
        void renders_CollectionChanged(object sender, CollectionChangedEventArgs e)
        {
            
            if (e.ChangeElements != null)
            {
                foreach (SVGElement elm in e.ChangeElements)
                    if (elm is SVGBranchElement)
                        if (e.Action == CollectionChangeAction.Insert)
                            childBranchCount++;
                        else if (e.Action == CollectionChangeAction.Remove)
                            childBranchCount--;
            }
            this.UpdatePath();
            this.UpdateElement(true);
        }
        #endregion

        #region ..UpdatePath
        public override void UpdatePath()
        {
            UpdatePath(true);

            foreach (SVGElement branch in this.ChildElements)
            {
                if(branch is SVGBranchElement)
                (branch as SVGBranchElement).UpdatePath(false);
            }
        }
        #endregion

        #region ..GetUpdatePath
        public virtual bool GetUpdatePath(GraphicsPath path, ref int anchorIndex, PointF point, SVGTransformableElement target, int targetIndex)
        {
            return this.GetUpdatePath(path, null, -1, ref anchorIndex, point, target, targetIndex);
        }

        /// <summary>
        /// Gets the updated path when using the new point update the exist anchor
        /// </summary>
        /// <param name="anchorIndex">the anchor index you want to update</param>
        /// <param name="target">the target element you want to connect</param>
        /// <param name="point">the target point</param>
        /// <param name="targetConnectIndex">the index of the connect point of the target</param>
        /// <param name="point"></param>
        /// <returns>返回一个值，指示最新的路径是否匹配控制</returns>
        public bool GetUpdatePath(GraphicsPath path, SVGTransformableElement startElement, int startIndex, ref int anchorIndex, PointF point, SVGTransformableElement target, int targetConnectIndex)
        {
            if (this.connectionPath == null || this.connectionPath.PointCount < 2)
                return true;
            List<PointF> list = new List<PointF>();
            list.AddRange(this.connectionPath.PathPoints);
            if (this.type == SVG.ConnectionType.Dynamic)
            {
                //如果是调整分支的开始端点，等同于调整父连接线的分叉点
                if (anchorIndex == 0 && this.ParentElement is SVGBranchElement)
                {
                    SVGBranchElement parentBranch = this.ParentElement as SVGBranchElement;
                    int index = parentBranch.connectionPath.PointCount - 1;
                    return parentBranch.GetUpdatePath(path, startElement, startIndex, ref index, point, target, targetConnectIndex);
                }

                bool adjustBranchAnchor = false;
                //调整开始阶段端点
                if (anchorIndex == 0 || (anchorIndex == list.Count - 1 && !this.HasChildBranch))
                {
                    list[anchorIndex] = point;
                    SVGTransformableElement start = startElement as SVGTransformableElement;
                    SVGTransformableElement end = this.endElement as SVGTransformableElement;
                    int endIndex = this.endIndex;
                    bool remainControl = true;
                    if (anchorIndex == 0)
                    {
                        remainControl = (target == this.OwnerConnection.StartElement);
                        start = target;
                        startIndex = targetConnectIndex;
                    }
                    else
                    {
                        remainControl = (target == this.endElement);
                        end = target;
                        endIndex = targetConnectIndex;
                    }
                    path.Reset();
                    List<PointF> controls = new List<PointF>();
                    //当改变连接对象时，去掉控制点
                    if (this.controlPoints != null && remainControl)
                        controls.AddRange(this.controlPoints.GetGDIPoints());
                    GetInitializePath(path, controls, list[0], list.Last(), this.type, start, end, startIndex, endIndex, this);
                    if (remainControl && path.PointCount > 1 && this.controlPoints.NumberOfItems > 0 && this.IsControlPointOK(path.PathPoints.First(), path.PathPoints.Last(), start, end, null))
                    {
                        List<PointF> contrls = new List<PointF>(this.anchors.Skip(1).Take(this.anchors.Length - 2).ToArray());
                        if (contrls.Count > 1)
                        {
                            path.Reset();
                            GetInitializePath(path, contrls, list[0], list.Last(), this.type, start, end, startIndex, endIndex, this);
                            return true;
                        }
                    }
                    return false;
                }
                else
                {
                    adjustBranchAnchor = (anchorIndex + 1 == list.Count) && this.HasChildBranch;
                    this.UpdateAnchor(list, ref anchorIndex, point, startElement);
                }

                path.Reset();
                path.AddLines(list.ToArray());

                //如果调整分支末端，则需要跟着调整分支
                if (adjustBranchAnchor)
                {
                    using (GraphicsPath tempp = new GraphicsPath())
                    {
                        foreach (SVGElement child1 in this.ChildElements)
                        {
                            if (child1 is SVGBranchElement)
                            {
                                SVGBranchElement child = child1 as SVGBranchElement;
                                GetInitializePath(tempp, new List<PointF>(child.controlPoints.GetGDIPoints()), list.Last(), new PointF(child.x2.Value, child.y2.Value), child.type, null, child.endElement as SVGTransformableElement, -1, child.endIndex, child);
                                if (tempp.PointCount > 0)
                                    path.AddPath(tempp, false);
                            }
                        }
                    }
                }
            }
            else
            {
                if (anchorIndex >= 0 && anchorIndex < this.anchors.Length)
                    list[anchorIndex] = point;
                PointF[] ps = new PointF[list.Count];
                list.CopyTo(ps);
                switch (this.type)
                {
                    //line
                    case SVG.ConnectionType.Line:
                        path.AddLine((PointF)list[0], (PointF)list[list.Count - 1]);
                        break;
                    //Bezier:
                    case SVG.ConnectionType.Spline:
                        path.AddBezier((PointF)list[0], (PointF)list[1], (PointF)list[2], (PointF)list[3]);
                        break;
                    //case SVGDom.ConnectionType.Polyline:
                    //    //update the control points
                    //    if (anchorIndex > 0 && anchorIndex < ps.Length - 1)
                    //        ps[ps.Length - 1 - anchorIndex] = new PointF(ps[anchorIndex].X, ps[ps.Length - 1 - anchorIndex].Y);
                    //    ps[1] = new PointF(ps[1].X, ps[0].Y);
                    //    ps[ps.Length - 2] = new PointF(ps[ps.Length - 2].X, ps[ps.Length - 1].Y);
                    //    path.AddLines(ps);
                    //    break;
                    //case SVGDom.ConnectionType.VPolyline:
                    //    //update the control points
                    //    if (anchorIndex > 0 && anchorIndex < ps.Length - 1)
                    //        ps[ps.Length - 1 - anchorIndex] = new PointF(ps[ps.Length - 1 - anchorIndex].X, ps[anchorIndex].Y);
                    //    ps[1] = new PointF(ps[0].X, ps[1].Y);
                    //    ps[ps.Length - 2] = new PointF(ps[ps.Length - 1].X, ps[ps.Length - 2].Y);
                    //    path.AddLines(ps);
                    //    break;
                    case ConnectionType.FreeLine:
                        path.AddLines(ps);
                        break;
                }
            }

            return true;
        }
        #endregion

        #region ..ResetConnectElement
        /// <summary>
        /// Reset the connect element
        /// </summary>
        /// <param name="start">if it want to reset the start element</param>
        public virtual void ResetConnectElement(bool start)
        {
            if (!start)
                this.oldend = this.connectend.Value + "reset";
        }
        #endregion

        #region ..IsControlPointOK
        /// <summary>
        /// 检查指定连接路径对连接对象是否OK
        /// </summary>
        /// <param name="startPoint">开始点坐标</param>
        /// <param name="endPoint">结束点坐标</param>
        /// <param name="controls">控制点</param>
        /// <param name="startElement">开始对象</param>
        /// <param name="endElement">结束对象</param>
        /// <param name="type">连接类型</param>
        /// <returns>连接路径是否穿过连接对象，如果是，返回False，如果不是，返回True</returns>
        public bool IsControlPointOK(PointF startPoint, PointF endPoint, SVGTransformableElement startElement, SVGTransformableElement endElement, Matrix coordTransform)
        {
            List<PointF> controls = new List<PointF>();
            if (this.controlPoints != null)
                controls.AddRange(this.controlPoints.GetGDIPoints());
            if (controls.Count < 2)
                return true;
            SVGTransformableElement[] renders = { startElement, endElement };
            renders = renders.Distinct<SVGTransformableElement>().ToArray();
            using (GraphicsPath path = new GraphicsPath())
            {
                using (Pen pen = new Pen(Color.White, 2))
                {
                    pen.Alignment = PenAlignment.Center;
                    List<GraphicsPath> pathes = new List<GraphicsPath>();
                    foreach (SVGTransformableElement render in renders)
                    {
                        if (render == null || (render as SVG.Interface.ISVGPathable).GPath == null || (render as SVG.Interface.ISVGPathable).GPath.PointCount <= 1)
                            continue;
                        GraphicsPath path2 = (render as SVG.Interface.ISVGPathable).GPath.Clone() as GraphicsPath;
                        path2.Transform(render.TotalMatrix);
                        if (coordTransform != null)
                            path2.Transform(coordTransform);
                        //path2.Widen(pen);
                        pathes.Add(path2);
                    }

                    if (pathes.Count == 0)
                        return true;
                    if (controls.Count > 1)
                    {
                        PointF[] starts = { new PointF(controls[0].X, startPoint.Y), new PointF(startPoint.X, controls[0].Y) };
                        PointF[] ends = { new PointF(controls.Last().X, endPoint.Y), new PointF(endPoint.X, controls.Last().Y) };
                        //如果connect不为空，按照connect原有的走向
                        //if (refConnect != null)
                        //{
                        PointF[] anchors = this.anchors;
                        if (anchors != null && anchors.Length > 3)
                        {
                            if (anchors[0].X == anchors[1].X)
                                starts = new PointF[] { new PointF(startPoint.X, controls[0].Y) };
                            else if (anchors[0].Y == anchors[1].Y)
                                starts = new PointF[] { new PointF(controls[0].X, startPoint.Y) };

                            if (anchors.Last().X == anchors[anchors.Length - 2].X)
                                ends = new PointF[] { new PointF(endPoint.X, controls.Last().Y) };
                            else if (anchors[anchors.Length - 1].Y == anchors[anchors.Length - 2].Y)
                                ends = new PointF[] { new PointF(controls.Last().X, endPoint.Y) };
                        }
                        //}
                        starts = starts.Distinct<PointF>().ToArray();
                        ends = ends.Distinct<PointF>().ToArray();

                        foreach (PointF p1 in starts)
                        {
                            foreach (PointF p2 in ends)
                            {
                                List<PointF> ps = new List<PointF>();
                                ps.Add(startPoint);
                                ps.Add(p1);
                                ps.AddRange(controls);
                                ps.Add(p2);

                                ps.Add(endPoint);
                                path.Reset();
                                path.AddLines(ps.ToArray());
                                path.Widen(pen);

                                bool isOK = true;
                                foreach (GraphicsPath path1 in pathes)
                                {
                                    using (Region rg = new Region(path1))
                                    {
                                        rg.Intersect(path);
                                        RectangleF[] rect = rg.GetRegionScans(new Matrix());
                                        isOK = isOK && (rect.Length <= 1);
                                        if (!isOK)
                                            continue;
                                        if (rect.Length > 0)
                                        {
                                            //PointF[] temp = { new PointF(rect[0].X + 0.5f, rect[0].Y + 0.5f), new PointF(rect[0].Right - 0.5f, rect[0].Y + 0.5f), new PointF(rect[0].Right - 0.5f, rect[0].Bottom - 0.5f), new PointF(rect[0].X + 0.5f, rect[0].Bottom - 0.5f) };
                                            isOK = isOK && rect[0].Width <= 2 && rect[0].Height <= 2;
                                            if (!isOK)
                                                continue;
                                        }
                                    }
                                }

                                if (isOK)
                                {
                                    return true;
                                }
                            }
                        }
                    }

                }
            }
            return false;
        }
        #endregion

        #region ..UpdateConnect
        public virtual void UpdateConnect(int anchorIndex, PointF p, SVG.Interface.ISVGPathable target, int targetConnectIndex)
        {
            this.UpdateConnect(null, -1, anchorIndex, p, target, targetConnectIndex);
        }

        /// <summary>
        /// Update the control point
        /// </summary>
        /// <param name="anchorIndex">the anchor index you want to update</param>
        /// <param name="target">the target element you want to connect</param>
        /// <param name="p">the target point</param>
        /// <param name="targetConnectIndex">the index of the connect point of the target</param>
        public void UpdateConnect(SVGTransformableElement startElement, int startIndex, int anchorIndex, PointF p, SVG.Interface.ISVGPathable target, int targetConnectIndex)
        {
            if (this.connectionPath == null || this.connectionPath.PointCount < 2)
                return;
            bool old = this.OwnerDocument.AcceptNodeChanged;
            this.OwnerDocument.AcceptNodeChanged = true;
            try
            {
                string id = string.Empty;
                bool inControl = true;
                if (this.type == SVG.ConnectionType.Dynamic)
                {
                    List<PointF> list = new List<PointF>();
                    list.AddRange(this.connectionPath.PathPoints);

                    //如果是调整分支的开始端点，等同于调整父连接线的分叉点
                    if (anchorIndex == 0 && this.ParentElement is SVGBranchElement)
                    {
                        SVGBranchElement parentBranch = this.ParentElement as SVGBranchElement;
                        parentBranch.UpdateConnect(startElement, startIndex, parentBranch.connectionPath.PointCount - 1, p, target, targetConnectIndex);
                        return;
                    }

                    //调整开始结束端点
                    if (anchorIndex == 0 || (anchorIndex == list.Count - 1 && !this.HasChildBranch))
                    {
                        inControl = false;
                        if (target != null)
                        {
                            SVG.SVGElement element = target as SVG.SVGElement;
                            id = element.GetAttribute("id");
                            if (id == null || id.Trim().Length == 0)
                            {
                                id = this.OwnerDocument.CreateID(element, this.OwnerDocument.RootElement as SVG.SVGElement);
                                element.InternalSetAttribute("id", id);
                            }
                            if (targetConnectIndex >= 0)
                                id = id + "." + targetConnectIndex.ToString();
                        }

                        bool start = anchorIndex == 0;

                        //update the connect target
                        if (id.Length > 0)
                            id = "#" + id;
                        string attributeName = start ? "start" : "end";
                        if (this.GetAttribute(attributeName) != id)
                        {
                            if (target != null)
                                this.InternalRemoveAttribute("d");
                            this.InternalSetAttribute(attributeName, id);
                        }
                        if (target != null)
                            return;
                    }

                    int index = anchorIndex;
                    this.UpdateAnchor(list, ref index, p, startElement);

                    if (this.x1.Value != list[0].X)
                        this.InternalSetAttribute("x1", list[0].X.ToString());
                    if (this.y1.Value != list[0].Y)
                        this.InternalSetAttribute("y1", list[0].Y.ToString());

                    if (this.x2.Value != list[list.Count - 1].X)
                        this.InternalSetAttribute("x2", list[list.Count - 1].X.ToString());
                    if (this.y2.Value != list[list.Count - 1].Y)
                        this.InternalSetAttribute("y2", list[list.Count - 1].Y.ToString());

                    string b = string.Empty;
                    if (inControl)
                    {
                        NormalizeControlPoints(list);
                        int count = !this.HasChildBranch ? list.Count - 1 : list.Count;
                        var query = from t in list select t;
                        //for (int i = 1; i < count; i++)
                        //    b += string.Format("{0} {1} ", list[i].X, list[i].Y);
                        this.InternalSetAttribute("d", DataType.SVGPointList.GetPointsString(query.Skip(1).Take(count - 1).ToArray()));
                    }
                }
                else
                {
                    if (anchorIndex == 0 || anchorIndex == this.anchors.Length - 1)
                    {
                        if (target != null)
                        {
                            SVG.SVGElement element = target as SVG.SVGElement;
                            id = element.GetAttribute("id");
                            if (id == null || id.Trim().Length == 0)
                            {
                                id = this.OwnerDocument.CreateID(element, this.OwnerDocument.RootElement as SVG.SVGElement);
                                element.InternalSetAttribute("id", id);
                            }
                            if (targetConnectIndex >= 0)
                                id = id + "." + targetConnectIndex.ToString();
                        }

                        bool start = anchorIndex == 0;

                        //update the anchor point
                        this.InternalSetAttribute(start ? "x1" : "x2", p.X.ToString());
                        this.InternalSetAttribute(start ? "y1" : "y2", p.Y.ToString());

                        //update the connect target
                        if (id.Length > 0)
                            id = "#" + id;
                        this.InternalSetAttribute(start ? "start" : "end", id);
                    }
                    else
                    {
                        switch (this.type)
                        {
                            case SVG.ConnectionType.Spline:
                                this.anchors[anchorIndex] = p;
                                this.UpdateControlPoints(startElement);
                                break;
                            //case SVGDom.ConnectionType.Polyline:
                            //    this.anchors[anchorIndex] = p;
                            //    //update the control points
                            //    if (anchorIndex > 0 && anchorIndex < this.anchors.Length - 1)
                            //        this.anchors[anchors.Length - 1 - anchorIndex] = new PointF(this.anchors[anchorIndex].X, this.anchors[this.anchors.Length - 1 - anchorIndex].Y);
                            //    this.anchors[1] = new PointF(this.anchors[1].X, this.anchors[0].Y);
                            //    this.anchors[this.anchors.Length - 2] = new PointF(this.anchors[this.anchors.Length - 2].X, this.anchors[this.anchors.Length - 1].Y);
                            //    this.UpdateControlPoints(startElement);
                            //    break;
                            //case SVGDom.ConnectionType.VPolyline:
                            //    this.anchors[anchorIndex] = p;
                            //    //update the control points
                            //    if (anchorIndex > 0 && anchorIndex < this.anchors.Length - 1)
                            //        this.anchors[anchors.Length - 1 - anchorIndex] = new PointF(this.anchors[this.anchors.Length - 1 - anchorIndex].X, this.anchors[anchorIndex].Y);
                            //    this.anchors[1] = new PointF(this.anchors[0].X, this.anchors[1].Y);
                            //    this.anchors[this.anchors.Length - 2] = new PointF(this.anchors[this.anchors.Length - 1].X, this.anchors[this.anchors.Length - 2].Y);
                            //    this.UpdateControlPoints(startElement);
                            //    break;
                            case SVG.ConnectionType.FreeLine:
                                this.anchors[anchorIndex] = p;
                                this.UpdateControlPoints(startElement);
                                break;
                        }
                    }

                }
                //this.UpdateElement();
                //this.CalculateGPath();
            }
            finally
            {
                this.OwnerDocument.InvokeUndos();
                this.OwnerDocument.AcceptNodeChanged = old;
                this.CurrentTime = this.OwnerDocument.CurrentTime;
            }
        }
        #endregion

        #region ..UpdateAnchor
        /// <summary>
        /// 当指定索引中的点移动到指定坐标，更新整个Anchor
        /// </summary>
        /// <param name="anchors">记录原始顶点坐标的集合</param>
        /// <param name="anchorIndex">要改变的顶点索引</param>
        /// <param name="point">改变后的顶点坐标</param>
        /// <param name="startElement">当前连接线(或分支)的开始对象</param>
        void UpdateAnchor(List<PointF> anchors, ref int anchorIndex, PointF point,SVGTransformableElement startElement)
        {
            List<PointF> list = anchors;
            if (anchorIndex >= 0 && anchorIndex < list.Count)
            {
                int index = anchorIndex;
                float deltax = point.X - list[anchorIndex].X;
                float deltay = point.Y - list[anchorIndex].Y;

                if (index + 1 == list.Count - 1)
                {
                    //存在分支
                    if (this.HasChildBranch)
                    {
                        list.Insert(index + 1, list[index + 1]);
                    }
                    else if (this.endElement != null)
                    {
                        float y = (list[index].Y + list[index + 1].Y) / 2f;
                        float x = (list[index].X + list[index + 1].X) / 2f;
                        list.Insert(index + 1, new PointF(x, y));
                        list.Insert(index + 1, new PointF(x, y));
                    }
                }

                if (index - 1 == 0)
                {
                    //分支
                    if (this.ParentElement is SVGBranchElement)
                    {
                        list.Insert(index, list[index - 1]);
                        index++;
                    }
                    else if (startElement != null)
                    {
                        float y = (list[index - 1].Y + list[index].Y) / 2f;
                        float x = (list[index - 1].X + list[index].X) / 2f;
                        list.Insert(index, new PointF(x, y));
                        index++;
                        list.Insert(index, new PointF(x, y));
                        index++;
                    }
                }

                if (index - 1 >= 0)
                    list[index - 1] = new PointF(list[index - 1].X + (list[index - 1].X == list[index].X ? deltax : 0), list[index - 1].Y + (list[index - 1].Y == list[index].Y ? deltay : 0));

                if (index + 1 < list.Count)
                    list[index + 1] = new PointF(list[index + 1].X + (list[index + 1].X == list[index].X ? deltax : 0), list[index + 1].Y + (list[index + 1].Y == list[index].Y ? deltay : 0));

                list[index] = point;
            }
            else if (anchorIndex >= list.Count)
            {
                int index = anchorIndex - list.Count;

                if (index + 1 < list.Count)
                {
                    bool vert = (list[index].X == list[index + 1].X);
                    if (vert)
                    {
                        float deltax = point.X - list[index].X;
                        if (index + 1 == list.Count - 1)
                        {
                            //存在分支
                            if (this.HasChildBranch)
                            {
                                float y = list[index + 1].Y;
                                float x = list[index].X;
                                list.Insert(index + 1, new PointF(x, y));
                            }
                            else if (endElement != null)
                            {
                                float y = list[index].Y / 4f + 3f * list[index + 1].Y / 4f;
                                float x = list[index].X;
                                list.Insert(index + 1, new PointF(x, y));
                                list.Insert(index + 1, new PointF(x, y));
                            }
                        }

                        if (index == 0)
                        {
                            //分支
                            if (this.ParentElement is SVGBranchElement)
                            {
                                float y = list[index].Y;
                                float x = list[index].X;
                                list.Insert(index + 1, new PointF(x, y));
                                index++;
                            }
                            else if (startElement != null)
                            {
                                float y = list[index].Y * 3f / 4f + list[index + 1].Y / 4f;
                                float x = list[index].X;
                                list.Insert(index + 1, new PointF(x, y));
                                index++;
                                list.Insert(index + 1, new PointF(x, y));
                                index++;
                            }
                        }
                        list[index] = new PointF(list[index].X + deltax, list[index].Y);
                        list[index + 1] = new PointF(list[index + 1].X + deltax, list[index + 1].Y);
                    }
                    else
                    {
                        float deltay = point.Y - list[index].Y;
                        if (index + 1 == list.Count - 1)
                        {
                            //存在分支
                            if (this.HasChildBranch)
                            {
                                float x = list[index + 1].X;
                                float y = list[index].Y;
                                list.Insert(index + 1, new PointF(x, y));
                                list.Insert(index + 1, new PointF(x, y));
                            }
                            else if (this.endElement != null)
                            {
                                float x = list[index].X / 4f + 3f * list[index + 1].X / 4f;
                                float y = list[index].Y;
                                list.Insert(index + 1, new PointF(x, y));
                                list.Insert(index + 1, new PointF(x, y));
                            }
                        }

                        if (index == 0)
                        {
                            //分支
                            if (this.ParentElement is SVGBranchElement)
                            {
                                float x = list[index].X;
                                float y = list[index].Y;
                                list.Insert(index + 1, new PointF(x, y));
                                index++;
                            }
                            else if (startElement != null)
                            {
                                float x = list[index].X * 3f / 4f + list[index + 1].X / 4f;
                                float y = list[index].Y;
                                list.Insert(index + 1, new PointF(x, y));
                                index++;
                                list.Insert(index + 1, new PointF(x, y));
                                index++;
                            }

                        }
                        list[index] = new PointF(list[index].X, list[index].Y + deltay);
                        list[index + 1] = new PointF(list[index + 1].X, list[index + 1].Y + deltay);
                    }
                }

                anchorIndex = index;
            }
        }
        #endregion

        #region ..UpdateControlPoints
        public void UpdateControlPoints(SVGTransformableElement startElement)
        {
            PointF[] ps = this.anchors.Clone() as PointF[];
            switch (this.type)
            {
                //bezier
                case SVG.ConnectionType.Spline:
                    string d = this.anchors[1].X.ToString() + " " + this.anchors[1].Y.ToString() + " " + this.anchors[2].X.ToString() + " " + this.anchors[2].Y.ToString();
                    this.InternalSetAttribute("d", d);
                    break;
                //case SVGDom.ConnectionType.Polyline:
                //case SVGDom.ConnectionType.VPolyline:
                case ConnectionType.FreeLine:
                    {
                        ps = new PointF[this.anchors.Length - 2];
                        Array.Copy(this.anchors, 1, ps, 0, ps.Length);
                        string d1 = SVG.DataType.SVGPointList.GetPointsString(ps);
                        this.InternalSetAttribute("d", d1);
                    }
                    break;
                case SVG.ConnectionType.Dynamic:
                    if (startElement == null && this.EndElement == null && this.anchors.Length > 3)
                    {
                        PointF[] ps1 = new PointF[this.anchors.Length - 2];
                        Array.Copy(this.anchors, 1, ps1, 0, ps1.Length);
                        string d1 = SVG.DataType.SVGPointList.GetPointsString(ps1);
                        this.InternalSetAttribute("d", d1);
                    }
                    break;
            }
            //set the start point and end point
            this.InternalSetAttribute("x1", ps[0].X.ToString());
            this.InternalSetAttribute("y1", ps[0].Y.ToString());

            this.InternalSetAttribute("x2", ps[ps.Length - 1].X.ToString());
            this.InternalSetAttribute("y2", ps[ps.Length - 1].Y.ToString());
        }

        /// <summary>
        /// using the matrix to transform the control points
        /// </summary>
        /// <param name="matrix"></param>
        public void UpdateControlPoints(Matrix matrix, SVGTransformableElement startElement)
        {
            if (matrix != null)
                matrix.TransformPoints(this.anchors);
            this.UpdateControlPoints(startElement);
        }
        #endregion

        #region ..AttributeChangeTest
        public override AttributeChangedResult AttributeChangeTest(string attributeName)
        {
            if (string.Compare(attributeName,"x1")==0 
                ||string.Compare(attributeName,"x2") ==0 
                ||string.Compare(attributeName,"y1") ==0 
                ||string.Compare(attributeName,"y2") ==0 
                ||string.Compare(attributeName,"end") ==0 
                ||string.Compare(attributeName,"d") ==0 
                ||string.Compare(attributeName,"type") ==0)
                return AttributeChangedResult.GraphicsPathChanged;
            return base.AttributeChangeTest(attributeName);
        }
        #endregion

        #region ..TransformConnection
        /// <summary>
        /// 将指定的变换应用到连接线
        /// </summary>
        /// <param name="matrix"></param>
        public void TransformConnection(Matrix matrix)
        {
            if (this.connectionPath != null && this.connectionPath.PointCount > 0)
            {
                //PointF[] list = this.connectionPath.PathPoints.Clone() as PointF[];
                //matrix.TransformPoints(list);
                PointF[] ps = new PointF[] { new PointF(x1.Value, y1.Value), this.EndPoint };
                matrix.TransformPoints(ps);
                if (this.x1.Value != ps[0].X)
                    this.InternalSetAttribute("x1", ps[0].X.ToString());
                if (this.y1.Value != ps[0].Y)
                    this.InternalSetAttribute("y1", ps[0].Y.ToString());

                if (this.x2.Value != ps[ps.Length - 1].X)
                    this.InternalSetAttribute("x2", ps[ps.Length - 1].X.ToString());
                if (this.y2.Value != ps[ps.Length - 1].Y)
                    this.InternalSetAttribute("y2", ps[ps.Length - 1].Y.ToString());
                if (this.controlPoints != null && this.controlPoints.NumberOfItems > 0)
                {
                    ps = this.controlPoints.GetGDIPoints();
                    if (ps.Length > 0)
                    {
                        matrix.TransformPoints(ps);
                        //string b = string.Empty;
                        //int count = this.ChildElements.Count == 0 ? list.Length - 1 : list.Length;
                        //var query = from t in list select t;
                        string d = DataType.SVGPointList.GetPointsString(ps);
                        this.InternalSetAttribute("d", d);
                    }
                }
            }

            foreach (SVGElement child in this.ChildElements)
            {
                if(child is SVGBranchElement)
                (child as SVGBranchElement).TransformConnection(matrix);
            }
        }
        #endregion

        #region ..ISVGPathable
        GraphicsPath Interface.ISVGPathable.GPath
        {
            get { return this.ConnectionPath; }
        }
        #endregion

        #region ..ISVGViewPort
        DataType.SVGViewport Interface.ISVGTextBlockContainer.Viewport
        {
            get
            {
                return PathHelper.GetViewport(this);
            }
        }
        #endregion

        #region ..ILineElement
        float? distance = 0;
        float Interface.ILineElement.Distance
        {
            get
            {
                if ((!this.distance.HasValue || this.CurrentTime != this.OwnerDocument.CurrentTime) && this.ConnectionPath != null)
                    this.distance = PathHelper.Distance(this.ConnectionPath.PathPoints);
                return this.distance.HasValue ? this.distance.Value : 0;
            }
        }

        PointF[] Interface.ILineElement.GetAnchorsWithDistance(float distance)
        {
            if (this.ConnectionPath != null)
            {
                PointF[] ps = this.ConnectionPath.PathPoints;
                float dis = 0;
                for (int i = 1; i < ps.Length; i++)
                {
                    float distance1 = PathHelper.Distance(ps[i - 1], ps[i]);
                    dis += distance1;
                    if (dis >= distance)
                    {
                        float dis2 = distance1 - dis + distance;
                        PointF p2 = new PointF(ps[i - 1].X + dis2 / distance1 * (ps[i].X - ps[i - 1].X), ps[i - 1].Y + dis2 / distance1 * (ps[i].Y - ps[i - 1].Y));
                        return new PointF[] { ps[i - 1], ps[i], p2 };
                    }
                }

                if (ps.Length > 1)
                    return new PointF[] { ps.Last(), ps[ps.Length - 1],new PointF((ps.Last().X + ps[ps.Length - 1].X)/2, (ps.Last().Y+ ps[ps.Length - 1].Y)/2) };
            }
            return null;
        }
        #endregion

        #region ..ExportNativeSVG
        /// <summary>
        /// export to native svg element
        /// </summary>
        /// <returns></returns>
        protected virtual SVGElement ExportNativeSVG(SVGElement parentElm)
        {
            bool old = this.OwnerDocument.AcceptNodeChanged;
            this.OwnerDocument.AcceptNodeChanged = false;
            SVGElement elm = this.OwnerDocument.CreateElement("path") as SVGElement;
            if(this.OwnerConnection.HasAttribute("marker-end"))
                elm.SetAttribute("marker-end", this.OwnerConnection.GetAttribute("marker-end"));
            string pathstr = SVG.Paths.SVGPathElement.GetPathString(this.ConnectionPath);
            elm.InternalSetAttribute("d", pathstr);
            parentElm.AppendChild(elm);

    
            this.OwnerDocument.AcceptNodeChanged = old;
            return null;
        }
        #endregion

        #region ..CloneNode
        public override System.Xml.XmlNode CloneNode(bool deep)
        {
            SVGBranchElement cnn = base.CloneNode(deep) as SVGBranchElement;
            if (this.connectionPath != null)
                cnn.connectionPath = this.connectionPath.Clone() as GraphicsPath;
            return cnn;
        }
        #endregion
    }
}
