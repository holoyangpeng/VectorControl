using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using YP.SVG.Common;


namespace YP.SVG.BasicShapes
{
    /// <summary>
    /// 连接线类
    /// </summary>
    public class SVGConnectionElement : SVGBranchElement, 
        Interface.ISVGPathable, 
        Interface.BasicShapes.ISVGConnectElement, 
        Interface.BasicShapes.ISVGBasicShape,
        Interface.IOutlookBarPath
    {
        #region ..构造及消除
        public SVGConnectionElement(string prefix, string localname, string ns, Document.SVGDocument doc)
            : base(prefix, localname, ns, doc)
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
            this.x1 = new DataType.SVGLength("0", this, LengthDirection.Hori);
            this.y1 = new DataType.SVGLength("0", this, LengthDirection.Vect);
            render = new Render.SVGConnectionRenderer(this);
        }
        #endregion

        #region ..const string
        /// <summary>
        /// 常量，createDefaultConnectablePoint属性字符串
        /// </summary>
        public const string CreateDefaultConnectablePointAttributeString = "createDefaultConnectPoint";
        /// <summary>
        /// 常量，连接点位置属性字符串
        /// </summary>
        public const string ConnectablePointAttributeString = "connectPoints";
        #endregion

        #region ..私有变量
        DataType.SVGString connectstrart = new SVG.DataType.SVGString(string.Empty);
        DataType.SVGString connectend = new SVG.DataType.SVGString(string.Empty);
        SVGTransformableElement startElement = null;
        string oldstart = string.Empty;
        int startIndex = -1;
        Render.SVGConnectionRenderer render = null;
        #endregion

        #region ..公共属性

        public override bool SupportMarker
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// 获取或设置连接开始对象
        /// </summary>
        public SVGTransformableElement StartElement
        {
            get
            {
                if (this.oldstart != this.connectstrart.Value)
                {
                    this.oldstart = this.connectstrart.Value;
                    int index = this.oldstart.LastIndexOf(".");
                    string temp = this.oldstart;
                    this.startIndex = -1;
                    if (index >= 0)
                    {
                        temp = temp.Substring(0, index);
                        int.TryParse(this.oldstart.Substring(index + 1), out this.startIndex);
                    }
                    SVGElement ownerSvg = this.OwnerSvgElement as SVGElement;
                    SVGElement ownerG = null;
                    if (this.GetAttribute("groupfirst") == "true")
                    {
                        this.startElement = null;
                        ownerG = this.OwnerStructureElement;
                        while (this.startElement == null && ownerG != null)
                        {
                            this.startElement = this.OwnerDocument.GetReferencedNode(temp, null, ownerG) as SVGTransformableElement;
                            if (this.startElement != null)
                                break;
                            SVGElement owner = ownerG.OwnerStructureElement;
                            if (owner == ownerG)
                                break;
                            ownerG = owner;
                        }
                    }
                    if(this.startElement == null && ownerG != ownerSvg)
                        this.startElement = this.OwnerDocument.GetReferencedNode(temp, null, ownerSvg) as SVGTransformableElement;
                    if (this.startElement != null)
                    {
                        //SVGDom.SVGTransformableElement dir = this.startElement as SVGDom.SVGTransformableElement;
                        this.startElement.AddRefedConnects(this);
                    }
                }
                return this.startElement;
            }
            set
            {
                string id = this.GetAttribute("start");
                if (value != null)
                {
                    if (value.ID == string.Empty)
                        value.ID = Guid.NewGuid().ToString();
                    id = value.ID;
                    this.SetAttribute("start", "#" + id, false);
                }
                else
                    this.InternalRemoveAttribute("start");
            }
        }

        /// <summary>
        /// 获取连接到开始图元的连接点索引
        /// </summary>
        public int StartIndex
        {
            get
            {
                if (this.StartElement != null)
                    return this.startIndex;
                return -1;
            }
        }

        /// <summary>
        /// 获取或设置连接线的类型
        /// </summary>
        public SVG.ConnectionType Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.InternalSetAttribute("type", value.ToString());
            }
        }
        #endregion

        #region ..IoutlookBarPath
        GraphicsPath Interface.IOutlookBarPath.GPath
        {
            get
            {
                return (this as Interface.ISVGPathable).GPath;
            }
        }

        string Interface.IOutlookBarPath.Title
        {
            get
            {
                return this.GetAttribute("title");
            }
        }
        #endregion

        #region ..ISVGPathable
        public override Render.SVGBaseRenderer SVGRenderer
        {
            get { return this.render; }
        }

        System.Drawing.Drawing2D.GraphicsPath Interface.ISVGPathable.GPath
        {
            get
            {
                this.CalculateGPath();
                return this.graphicsPath;
            }
        }

        //Interface.ISVGRenderer Interface.ISVGPathable.Render
        //{
        //    get
        //    {
        //        return this.render;
        //    }
        //}
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
                    case "start":
                        if (this.IsActive)
                        {
                            temp = this.StartElement as SVG.SVGElement;
                            if (temp != null)
                                oldElement = temp;
                        }
                        //remove the connect from the original element
                        if (this.startElement is SVGTransformableElement)
                            (this.startElement as SVGTransformableElement).RemoveRefedConnects(this);
                        this.startElement = null;
                        this.oldstart = string.Empty;
                        this.connectstrart = new SVG.DataType.SVGString(attributeValue);

                        if (this.IsActive)
                        {
                            temp = this.StartElement as SVG.SVGElement;
                            if (temp != null)
                                newElement = temp;
                            this.OnConnectChanged(oldElement, newElement, ConnectionChangedType.StartElement);
                        }
                        break;
                    case "type":
                        var type1 = (SVG.ConnectionType)System.Enum.Parse(typeof(SVG.ConnectionType), attributeValue, true);
                        if (!first)
                        {
                            if (this.type != type1)
                                this.InternalRemoveAttribute("d");
                        }
                        this.type = type1;//(SVG.ConnectionType)System.Enum.Parse(typeof(SVG.ConnectionType), attributeValue, true);
                        this.first = true;
                        break;
                    case "transform":
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

        #region ..CalculateGPath
        public override void CalculateGPath()
        {
            this.CalculateGPath(this.StartElement as SVGTransformableElement, this.startIndex, null);
        }
        #endregion

        #region ..获取原始路径
        public System.Drawing.Drawing2D.GraphicsPath GetOriPath()
        {
            System.Drawing.Drawing2D.GraphicsPath path = new GraphicsPath();
            PointF startPoint = new PointF(this.x1.Value, this.y1.Value);
            PointF endPoint = new PointF(this.x2.Value, this.y2.Value);
            bool vert = Math.Abs(endPoint.Y - startPoint.Y) > Math.Abs(endPoint.X - startPoint.X);
            switch (this.type)
            {
                //case SVGDom.ConnectionType.Polyline:
                //    float x = (startPoint.X + endPoint.X) / 2f;
                //    path.AddLines(new PointF[] { startPoint, new PointF(x, startPoint.Y), new PointF(x, endPoint.Y), endPoint });
                //    break;
                //case SVGDom.ConnectionType.VPolyline:
                //    float y = (startPoint.Y + endPoint.Y) / 2f;
                //    path.AddLines(new PointF[] { startPoint, new PointF(startPoint.X, y), new PointF(endPoint.X, y), endPoint });
                //    break;
                case SVG.ConnectionType.Line:
                case SVG.ConnectionType.FreeLine:
                    path.AddLine(startPoint, endPoint);
                    break;
                case SVG.ConnectionType.Spline:
                    bool lower = startPoint.X < endPoint.X;
                    path.AddBezier(startPoint, new PointF(startPoint.X + SplineLength, startPoint.Y), new PointF(endPoint.X - SplineLength, endPoint.Y), endPoint);
                    break;
                case SVG.ConnectionType.Dynamic:
                    GetInitializePath(path, startPoint, endPoint, type, null, null);
                    break;
            }
            return path;
        }
        #endregion

        #region ..Interface.BasicShapes.ISVGBasicShape
        /// <summary>
        /// 转换为路径对象
        /// </summary>
        SVG.Interface.Paths.ISVGPathElement Interface.BasicShapes.ISVGBasicShape.ConvertToPath()
        {
            SVG.Paths.SVGPathElement path = (SVG.Paths.SVGPathElement)this.OwnerDocument.CreateElement(this.OwnerDocument.Prefix, "path", this.OwnerDocument.NamespaceURI);
            bool old = this.OwnerDocument.AcceptNodeChanged;
            this.OwnerDocument.AcceptNodeChanged = false;
            for (int i = 0; i < this.Attributes.Count; i++)//(System.Xml.XmlAttribute attribute in this.Attributes)
            {
                System.Xml.XmlAttribute attribute = this.Attributes[i];
                string name = attribute.Name;
                string valuestr = attribute.Value;
                if (string.Compare(name,"start") != 0 &&string.Compare(name,"start") != 0 &&string.Compare(name,"x1") != 0 &&string.Compare(name,"x2") != 0 &&string.Compare(name,"y1") != 0 &&string.Compare(name,"y2") != 0)
                    path.InternalSetAttribute(name, valuestr);
            }
            string pathstr = SVG.Paths.SVGPathElement.GetPathString((this as SVG.Interface.ISVGPathable).GPath);
            path.InternalSetAttribute("d", pathstr);
            this.OwnerDocument.AcceptNodeChanged = old;
            pathstr = null;
            return path;
        }
        #endregion

        #region ..ExportNativeSVG
        /// <summary>
        /// export to native svg element
        /// </summary>
        /// <returns></returns>
        public SVGElement ExportNativeSVG()
        {
            if (this.ChildElements.Count == 0)
                return (this as Interface.BasicShapes.ISVGBasicShape).ConvertToPath() as SVGElement;
            bool old = this.OwnerDocument.AcceptNodeChanged;
            this.OwnerDocument.AcceptNodeChanged = false;
            SVGElement elm = this.OwnerDocument.CreateElement("path") as SVGElement;
            foreach (SVGAttribute attr in this.Attributes)
                if (attr.Name != "marker-end"
                    && attr.Name != "start" && attr.Name != "start"
                    && attr.Name != "x1" && attr.Name != "x2"
                    && attr.Name != "y1" && attr.Name != "y2")
                    elm.SetAttribute(attr.Name, attr.Value);
            string pathstr = SVG.Paths.SVGPathElement.GetPathString((this as SVG.Interface.ISVGPathable).GPath);
            elm.InternalSetAttribute("d", pathstr);
            this.OwnerDocument.AcceptNodeChanged = old;
            return elm;
        }
        #endregion

        #region ..AttributeChangeTest
        public override AttributeChangedResult AttributeChangeTest(string attributeName)
        {
            if (string.Compare(attributeName,"start")==0 ||string.Compare(attributeName,"type") ==0)
                return AttributeChangedResult.GraphicsPathChanged;
            return base.AttributeChangeTest(attributeName);
        }
        #endregion

        #region ..ResetConnectElement
        /// <summary>
        /// Reset the connect element
        /// </summary>
        /// <param name="start">if it want to reset the start element</param>
        public override void ResetConnectElement(bool start)
        {
            if (start)
                this.oldstart = this.connectstrart.Value + "reset";
            base.ResetConnectElement(start);
        }
        #endregion

        #region ..GetUpdatePath
        /// <summary>
        /// Gets the updated path when using the new point update the exist anchor
        /// </summary>
        /// <param name="anchorIndex"></param>
        /// <param name="point"></param>
        /// <returns>返回一个值，指示最新的路径是否匹配控制</returns>
        public override bool GetUpdatePath(GraphicsPath path, ref int anchorIndex, PointF point, SVGTransformableElement target, int targetIndex)
        {
            return this.GetUpdatePath(path, this.StartElement as SVGTransformableElement, this.startIndex, ref anchorIndex, point, target, targetIndex);
        }
        #endregion

        #region ..UpdateConnect
        /// <summary>
        /// Update the control point
        /// </summary>
        /// <param name="anchorIndex">the anchor index you want to update</param>
        /// <param name="target">the target element you want to connect</param>
        /// <param name="p">the target point</param>
        /// <param name="targetConnectIndex">the index of the connect point of the target</param>
        public override void UpdateConnect(int anchorIndex, PointF p, SVG.Interface.ISVGPathable target, int targetConnectIndex)
        {
            this.UpdateConnect(this.startElement as SVGTransformableElement, this.startIndex, anchorIndex, p, target, targetConnectIndex);
        }
        #endregion

        #region ..Update the connector using the translate
        /// <summary>
        /// Update the connector using the translate
        /// </summary>
        /// <param name="translateX">the translate in the x director</param>
        /// <param name="translateY">the translate in the y director</param>
        /// <param name="startTarget">the start connect target</param>
        /// <param name="startConnectIndex">the start connect index</param>
        /// <param name="endTarget">the end connect target </param>
        /// <param name="endConnectIndex">the end connect index</param>
        public void UpdateConnect(float translateX, float translateY, SVG.Interface.ISVGPathable startTarget, int startConnectIndex, SVG.Interface.ISVGPathable endTarget, int endConnectIndex)
        {
            bool old = this.OwnerDocument.AcceptNodeChanged;
            this.OwnerDocument.AcceptNodeChanged = true;
            //set the start attribute
            string id = string.Empty;
            if (startTarget != null)
            {
                SVG.SVGElement element = startTarget as SVG.SVGElement;
                id = element.GetAttribute("id");
                if (id == null || id.Trim().Length == 0)
                {
                    id = this.OwnerDocument.CreateID(element, this.OwnerDocument.RootElement as SVG.SVGElement);
                    element.InternalSetAttribute("id", id);
                }
                if (startConnectIndex >= 0)
                    id = id + "." + startConnectIndex.ToString();
            }
            if (id.Length > 0)
                id = "#" + id;
            if (this.GetAttribute("start") != id)
            {
                if(this.type == ConnectionType.Dynamic)
                    this.InternalRemoveAttribute("d");
                this.InternalSetAttribute("start", id);
            }
            //set the end attribute
            id = string.Empty;
            if (endTarget != null)
            {
                SVG.SVGElement element = endTarget as SVG.SVGElement;
                id = element.GetAttribute("id");
                if (id == null || id.Trim().Length == 0)
                {
                    id = this.OwnerDocument.CreateID(element, this.OwnerDocument.RootElement as SVG.SVGElement);
                    element.InternalSetAttribute("id", id);
                }
                if (endConnectIndex >= 0)
                    id = id + "." + endConnectIndex.ToString();
            }
            if (id.Length > 0)
                id = "#" + id;
            if (this.GetAttribute("end") != id)
            {
                if(this.type == ConnectionType.Dynamic)
                    this.InternalRemoveAttribute("d");
                this.InternalSetAttribute("end", id);
            }
            using (Matrix matrix = new Matrix())
            {
                matrix.Translate(translateX, translateY);
                this.TransformConnection(matrix);
            }

            //this.UpdateControlPoints(this.StartElement as SVGTransformableElement);
            this.OwnerDocument.AcceptNodeChanged = old;
            this.OwnerDocument.InvokeUndos();
        }
        #endregion

        #region ..TransformGraphics
        public override void TransformGraphics(Graphics g)
        {
            if (g == null)
                return;
            //revert the transform
            SVGTransformableElement transform1 = this.ParentElement as SVGTransformableElement;
            if (transform1 != null)
            {
                Matrix transform2 = transform1.TotalMatrix.Clone();
                transform2.Invert();
                if (g != null)
                    g.Transform = transform2;
            }
        }
        #endregion

        #region ..TransformPath
        public override void TransformPath(Graphics g, GraphicsPath path, StyleContainer.StyleOperator sp)
        {
            base.TransformPath(g, path, sp);
            if (sp.ConnectPath != null)
            {
                sp.ConnectPath.AddPath(path, false);
            }
        }
        #endregion

        #region ..UpdateControlPoints
        public void UpdateControlPoints(Matrix matrix)
        {
            this.TransformConnection(matrix);
        }
        #endregion

        #region ..UpdateTotalTransform
        public override void UpdateTotalTransform(Matrix baseTransform)
        {
            this.TotalTransform.Reset();
        }
        #endregion

        #region ..CloneNode
        public override System.Xml.XmlNode CloneNode(bool deep)
        {
            SVGConnectionElement cnn = base.CloneNode(deep) as SVGConnectionElement;
            if (this.graphicsPath != null)
                cnn.graphicsPath = this.graphicsPath.Clone() as GraphicsPath;
            return cnn;
        }
        #endregion
    }
}
