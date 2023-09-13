using System;
using System.Drawing;

namespace YP.SVG.BasicShapes
{
	/// <summary>
	/// ����
	/// </summary>
    public class SVGPolylineElement : SVGPointsElement, 
        Interface.ISVGPathable, 
        Interface.BasicShapes.ISVGPolylineElement, 
        Interface.BasicShapes.ISVGBasicShape,
        Interface.ISVGTextBlockContainer,
        Interface.ISVGContainer
	{
		#region ..���켰����
		public SVGPolylineElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
            this.render = new Render.SVGDirectRenderer(this);
		}
		#endregion

		#region ..��������
		public override bool FillShadow
		{
			get
			{
				return false;
			}
		}

		public override bool SupportMarker
		{
			get
			{
				return true;
			}
		}

        /// <summary>
        /// ��ȡ����Ŀ�������
        /// </summary>
        public override bool Connectable
        {
            get
            {
                return true;
            }
        }
		#endregion

        #region ..SVGBaseRenderer
        Render.SVGDirectRenderer render;

        public override Render.SVGBaseRenderer SVGRenderer
        {
            get { return this.render; }
        }

        /// <summary>
        /// ��ȡ�����GDI·��
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
                        if (ps.Length > 1)
                            this.graphicsPath.AddLines(ps);
                        ps = null;
                        this.anchors = ((YP.SVG.DataType.SVGPointList)points).GetGDIPoints();
                    }

                }
                return this.graphicsPath;
            }
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

        #region ..Interface.BasicShapes.ISVGBasicShape
        /// <summary>
        /// ת��Ϊ·������
        /// </summary>
        SVG.Interface.Paths.ISVGPathElement Interface.BasicShapes.ISVGBasicShape.ConvertToPath()
        {
            return base.ConvertToPath();
        }
        #endregion

        #region ..Interface.ISVGContainer
        /// <summary>
        /// �жϽڵ��Ƿ�����Ч���Ӽ��ڵ�
        /// </summary>
        /// <param name="child">�Ӽ��ڵ�</param>
        /// <returns></returns>
        bool Interface.ISVGContainer.ValidChild(Interface.ISVGElement child)
        {
            return child is Text.SVGTextBlockElement;
        }
        #endregion
	}
}
