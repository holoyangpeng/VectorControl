using System;
using System.Drawing;
using System.Drawing.Drawing2D;

using YP.SVG.Interface.Paths;
using YP.SVG.Interface.CTS;
using YP.SVG.Interface.DataType;

namespace YP.SVG.Paths
{
	/// <summary>
	/// ʵ��Path����
	/// </summary>
	public class SVGPathElement:SVGTransformableElement,
        Interface.Paths.ISVGPathElement ,
        Interface.ISVGPathSegListElement ,
        Interface.ISVGPathable,
        Interface.ISVGTextBlockContainer,
        Interface.ISVGContainer,
        Interface.IOutlookBarPath
	{
		#region ..���켰����
		public SVGPathElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.pathdata = new Paths.SVGPathSegList(string.Empty);
            this.render = new Render.SVGDirectRenderer(this);
		}
		#endregion

		#region ..˽�б���
		Paths.SVGPathSegList pathdata = null;
		int pathTime = -1;
		#endregion

		#region ..��������
		/// <summary>
		/// ��ȡ·��
		/// </summary>
		public System.Drawing.Drawing2D.GraphicsPath SegPath
		{
			get
			{
				return (this as SVG.Interface.ISVGPathable).GPath;
			}
		}

		/// <summary>
		/// ��ȡpathLength����
		/// </summary>
		public Interface.DataType.ISVGNumber PathLength
		{
			get
			{
				return new DataType.SVGNumber(this.GetAttribute("pathLength"),"0");//,this);
			}
		}

		/// <summary>
		/// ��ȡ��SVGPathSegList���ɣ�����SVGPathElement���󣬸�����ֵ��Ӧ��PathData��ֵ
		/// </summary>
		public Interface.Paths.ISVGPathSegList SVGPathSegList
		{
			get
			{
				return this.pathdata;				 
			}
		}

		/// <summary>
		/// ��ȡ·������
		/// </summary>
		public Interface.Paths.ISVGPathSegList PathData
		{
			get
			{
				return this.pathdata;
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
        /// ��ȡ�����GDI·��
        /// </summary>
        System.Drawing.Drawing2D.GraphicsPath Interface.ISVGPathable.GPath
        {
            get
            {
                if (this.graphicsPath == null)// || this.CurrentTime != this.OwnerDocument.CurrentTime || this.pathTime != this.OwnerDocument.CurrentTime)
                {
                    Paths.SVGPathSegList list = (Paths.SVGPathSegList)this.AnimatedPathSegList;
                    this.graphicsPath = list.GetGDIPath();
                }
                this.pathTime = this.OwnerDocument.CurrentTime;
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

		#region ..ʵ��ISVGPathElement�ӿ�
		public ISVGPathSegList PathSegList
		{
			get
			{
				return this.pathdata;
			}
		}

		public ISVGPathSegList NormalizedPathSegList
		{
			get
			{
				if(this.pathdata != null)
				{
					return this.pathdata.NormalSVGPathSegList;
				}
				return null;
			}
		}

		public ISVGPathSegList AnimatedPathSegList
		{
			get
			{
				if(this.pathdata != null)
					return this.pathdata;
				return null;
			}
		}

		public ISVGPathSegList AnimatedNormalizedPathSegList
		{
			get
			{
				if(this.pathdata != null)
					return this.pathdata.NormalSVGPathSegList;
				return null;
			}
		}

		public ISVGNumber GetPathLength()		
		{
			throw new NotImplementedException();
		}

		public float GetTotalLength()		
		{
			if(this.pathdata == null)
				return 0;
			return ((SVGPathSegList)this.pathdata).TotalLength;
		}

		public ISVGPoint GetPointAtLength(float distance)		
		{
			throw new NotImplementedException();
		}

		public int GetPathSegAtLength(float distance)		
		{
			Paths.SVGPathSegList list = (Paths.SVGPathSegList)this.pathdata;
			int right = list.NumberOfItems - 1;
			float length = 0;
			for(int i = 0 ;i<right;i++)
			{
				SVGPathSeg seg = (SVGPathSeg)list.GetItem(i);
				length += seg.GetPathLength(list);
				if(length >= distance)
					return i;
			}
			return -1;
		}

		public ISVGPathSegClosePath CreateSVGPathSegClosePath()
		{
			throw new NotImplementedException();
		}

		public ISVGPathSegMovetoAbs CreateSVGPathSegMovetoAbs(float x, float y)
		{
			throw new NotImplementedException();
		}

		public ISVGPathSegMovetoRel CreateSVGPathSegMovetoRel(float x, float y)
		{
			throw new NotImplementedException();
		}

		public ISVGPathSegLinetoAbs CreateSVGPathSegLinetoAbs(float x, float y)
		{
			throw new NotImplementedException();
		}

		public ISVGPathSegLinetoRel CreateSVGPathSegLinetoRel(float x, float y)
		{
			throw new NotImplementedException();
		}

		public ISVGPathSegCurvetoCubicAbs CreateSVGPathSegCurvetoCubicAbs(float x, 
			float y, 
			float x1, 
			float y1, 
			float x2, 
			float y2)
		{
			throw new NotImplementedException();
		}
		
		public ISVGPathSegCurvetoCubicRel CreateSVGPathSegCurvetoCubicRel(float x, 
			float y, 
			float x1, 
			float y1, 
			float x2, 
			float y2)		
		{
			throw new NotImplementedException();
		}

		
		public ISVGPathSegCurvetoQuadraticAbs CreateSVGPathSegCurvetoQuadraticAbs(float x, 
			float y, 
			float x1, 
			float y1)		
		{
			throw new NotImplementedException();
		}

		
		public ISVGPathSegCurvetoQuadraticRel CreateSVGPathSegCurvetoQuadraticRel(float x, 
			float y, 
			float x1, 
			float y1)		
		{
			throw new NotImplementedException();
		}


		public ISVGPathSegArcAbs CreateSVGPathSegArcAbs(float x,
			float y,
			float r1,
			float r2,
			float angle,
			bool largeArcFlag,
			bool sweepFlag)		
		{
			throw new NotImplementedException();
		}


		public ISVGPathSegArcRel CreateSVGPathSegArcRel(float x,
			float y,
			float r1,
			float r2,
			float angle,
			bool largeArcFlag,
			bool sweepFlag)		
		{
			throw new NotImplementedException();
		}


		public ISVGPathSegLinetoHorizontalAbs CreateSVGPathSegLinetoHorizontalAbs(float x)		
		{
			throw new NotImplementedException();
		}

		public ISVGPathSegLinetoHorizontalRel CreateSVGPathSegLinetoHorizontalRel(float x)		
		{
			throw new NotImplementedException();
		}

		public ISVGPathSegLinetoVerticalAbs CreateSVGPathSegLinetoVerticalAbs(float y)		
		{
			throw new NotImplementedException();
		}

		public ISVGPathSegLinetoVerticalRel CreateSVGPathSegLinetoVerticalRel(float y)		
		{
			throw new NotImplementedException();
		}

		public ISVGPathSegCurvetoCubicSmoothAbs CreateSVGPathSegCurvetoCubicSmoothAbs(float x,
			float y,
			float x2,
			float y2)
		{
			throw new NotImplementedException();
		}

		public ISVGPathSegCurvetoCubicSmoothRel CreateSVGPathSegCurvetoCubicSmoothRel(float x,
			float y,
			float x2,
			float y2)		
		{
			throw new NotImplementedException();
		}

		public ISVGPathSegCurvetoQuadraticSmoothAbs CreateSVGPathSegCurvetoQuadraticSmoothAbs(float x,
			float y)		
		{
			throw new NotImplementedException();
		}

		public ISVGPathSegCurvetoQuadraticSmoothRel CreateSVGPathSegCurvetoQuadraticSmoothRel(float x,
			float y)		
		{
			throw new NotImplementedException();
		}
		#endregion

		#region ..���Բ���
		/// <summary>
		/// �����Է����޸�ʱ�����¶�������
		/// </summary>
		/// <param name="attributeName">��������</param>
		/// <param name="attributeValue">����ֵ</param>
		public override void SetSVGAttribute(string attributeName,string attributeValue)
		{
            try
            {
                switch (attributeName)
                {
                    case "d":
                        this.pathdata = new Paths.SVGPathSegList(attributeValue); ;//,this);//this.svgPathSegList = new SVGPathSegList(attributeValue);
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

		#region ..��ȡ�ɶ�������
		/// <summary>
		/// ��ȡ�ɶ�������
		/// </summary>
		/// <param name="attributeName">��������</param>
		/// <returns></returns>
//		public override Interface.DataType.ISVGType GetAnimatedAttribute(string attributeName)
//		{
//			switch(attributeName)
//			{
//				case "d":
//					return this.pathdata;
//			}
//			return base.GetAnimatedAttribute(attributeName);
//		}
		#endregion

		#region ..��ָ����·������ԭ·��
		/// <summary>
		/// ��·�������ı�ʱ����·��
		/// </summary>
		public void UpdatePath(GraphicsPath path)
		{
			this.graphicsPath = (GraphicsPath)path.Clone();
		}
		#endregion

		#region ..��ȡ·���ַ���
		/// <summary>
		/// ��ȡ·���ַ���
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static string GetPathString(GraphicsPath path)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder(300);
			for(int i=0;i<path.PointCount;i++)
			{
				PointF point = path.PathPoints[i];
				byte type = path.PathTypes[i];
				switch(type)
				{
					case (byte)PathPointType.Start:
						sb.Append("M ");
						sb.Append(point.X.ToString()+" "+point.Y.ToString()+" ");
						break;
					case (byte)PathPointType.Line:
						sb.Append("L ");
						sb.Append(point.X.ToString()+" "+point.Y.ToString()+" ");
						break;
					case (byte)PathPointType.Bezier3:
						sb.Append("C ");
						for(int j=i;j<= System.Math.Min(i+2,path.PathPoints.Length-1);j++)
						{
							point = path.PathPoints[j];
							sb.Append(point.X.ToString() + " " + point.Y.ToString()+" ");
							if(path.PathTypes[j] == 131)
								sb.Append("Z");
						}
						i=  System.Math.Min(i+2,path.PathPoints.Length-1);
						break;
					case (byte)PathPointType.CloseSubpath:
						sb.Append("Z");
						break;
					case 131:
						sb.Append("C ");
						for(int j=i;j<= System.Math.Min(i+2,path.PathPoints.Length-1);j++)
						{
							point = path.PathPoints[j];
							sb.Append(point.X.ToString() + " " + point.Y.ToString()+" ");
						}
						sb.Append("Z");
						i=  System.Math.Min(i+2,path.PathPoints.Length-1);
						break;
					case 129:
						sb.Append("L ");
						sb.Append(point.X.ToString()+" "+point.Y.ToString()+" ");
						sb.Append("Z");
						break;
				}
			}

			return sb.ToString();
		}
		#endregion

		#region ..��ȡê�㼯
		/// <summary>
		/// ��ȡê�㼯
		/// </summary>
		public override PointF[] GetAnchors()
		{
			return new PointF[0];
		}
		#endregion

		#region ..��ָ���ı任�����б�
		public void TransformSegs(System.Drawing.Drawing2D.Matrix matrix)
		{
			this.pathdata = this.pathdata.TransformSegs(matrix);
		}
		#endregion

        #region ..AttributeChangeTest
        public override AttributeChangedResult AttributeChangeTest(string attributeName)
        {
            if (string.Compare(attributeName,"d")==0)
                return AttributeChangedResult.VisualChanged | AttributeChangedResult.GraphicsPathChanged;
            return base.AttributeChangeTest(attributeName);
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
	}
}
