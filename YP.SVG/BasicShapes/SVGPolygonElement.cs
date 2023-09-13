using System;
using System.Drawing;

namespace YP.SVG.BasicShapes
{
	/// <summary>
	/// polygon，多边形
	/// </summary>
    public class SVGPolygonElement : SVGPointsElement, 
        Interface.ISVGPathable, 
        Interface.BasicShapes.ISVGPolygonElement, 
        Interface.BasicShapes.ISVGBasicShape,
        Interface.ISVGTextBlockContainer,
        Interface.ISVGContainer
	{
		#region ..构造及消除
		public SVGPolygonElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
            this.render = new Render.SVGDirectRenderer(this);
		}
		#endregion

		#region ..公共属性
        /// <summary>
        /// 获取对象的可连接性
        /// </summary>
        public override bool Connectable
        {
            get
            {
                return true;
            }
        }
		#endregion

        #region ..ISVGPathable
        Render.SVGDirectRenderer render;

        public override Render.SVGBaseRenderer SVGRenderer
        {
            get { return this.render; }
        }

        /// <summary>
        /// 获取对象的GDI路径
        /// </summary>
        System.Drawing.Drawing2D.GraphicsPath Interface.ISVGPathable.GPath
        {
            get
            {
                if (this.graphicsPath == null)// || (this.CurrentTime != this.OwnerDocument.CurrentTime && this.Points.HasAnimate))
                {
                    if (this.graphicsPath == null)
                        this.graphicsPath = new System.Drawing.Drawing2D.GraphicsPath();
                    this.graphicsPath.Reset();
                    Interface.CTS.ISVGPointList points = this.Points;
                    if (points != null)
                    {
                        PointF[] ps = ((DataType.SVGPointList)points).GetGDIPoints();
                        if (ps.Length > 2)
                            this.graphicsPath.AddPolygon(ps);
                        ps = null;
                    }
                    //this.CreateConnectPoint();
                    this.anchors = ((YP.SVG.DataType.SVGPointList)this.Points).GetGDIPoints();
                }
                //				this.CurrentTime = this.OwnerDocument.CurrentTime;
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

        #region ..Interface.BasicShapes.ISVGBasicShape
        /// <summary>
        /// 转换为路径对象
        /// </summary>
        SVG.Interface.Paths.ISVGPathElement Interface.BasicShapes.ISVGBasicShape.ConvertToPath()
        {
            return base.ConvertToPath();
        }
        #endregion

		#region ..公共字段
		/// <summary>
		/// 记录星形的边数
		/// </summary>
		public int LineNumber = 3;

		/// <summary>
		/// 记录星形的缩进量
		/// </summary>
		public float Indent = 0;
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

        #region ..Interface.ISVGContainer
        /// <summary>
        /// 判断节点是否是有效的子级节点
        /// </summary>
        /// <param name="child">子级节点</param>
        /// <returns></returns>
        bool Interface.ISVGContainer.ValidChild(Interface.ISVGElement child)
        {
            return child is Text.SVGTextBlockElement;
        }
        #endregion
	}
}
