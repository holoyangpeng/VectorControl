using System;
using System.Drawing;

namespace YP.SVG.Paths
{
	/// <summary>
	/// t
	/// </summary>
	public class SVGPathSegCurvetoQuadraticSmoothRel:SVGPathSegCurve,Interface.Paths.ISVGPathSegCurvetoQuadraticSmoothRel,Interface.Paths.ISVGPathSegQuadratic
	{
		#region ..���켰����
		public SVGPathSegCurvetoQuadraticSmoothRel(float x,float y):base(x,y,0,0,0,0)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.pathSegType = (short)PathSegmentType.PATHSEG_CURVETO_QUADRATIC_SMOOTH_REL ;
			this.pathSegTypeAsLetter = "t";
			this.Relative = true;
		}
		#endregion

		#region ..��ȡ��һ�����Ƶ�
		/// <summary>
		/// ��ȡ��һ�����Ƶ�
		/// </summary>
		/// <param name="svgPathList"></param>
		/// <returns></returns>
		public override PointF GetFirstControl(Interface.Paths.ISVGPathSegList svgPathList)
		{
			Interface.Paths.ISVGPathSeg prevSeg = svgPathList.PreviousSibling(this);
			if(prevSeg == null)
				throw new SVGException("��Ч��·������",SVGExceptionType.SVG_INVALID_VALUE_ERR ,null);

			PointF prevPoint = prevSeg.GetLastPoint(svgPathList);
			PointF x1y1 = this.GetQuadraticControl (svgPathList);

			float x1 = prevPoint.X + (x1y1.X - prevPoint.X) * 2/3;
			float y1 = prevPoint.Y + (x1y1.Y - prevPoint.Y) * 2/3;
			
			return new PointF(x1, y1);
		}
		#endregion

		#region ..��ȡ�ڶ������Ƶ�
		/// <summary>
		/// ��ȡ�ڶ������Ƶ�
		/// </summary>
		/// <param name="svgPathList"></param>
		/// <returns></returns>
		public override PointF GetSecondControl(Interface.Paths.ISVGPathSegList svgPathList)
		{
			PointF xy = this.GetLastPoint(svgPathList);
			PointF x1y1 = this.GetQuadraticControl(svgPathList);
			float x2 = x1y1.X + (xy.X - x1y1.X) / 3;
			float y2 = x1y1.Y + (xy.Y - x1y1.Y) / 3;

			return new PointF(x2, y2);
		}
		#endregion

		#region ..��ȡ�������߿��Ƶ�
		/// <summary>
		/// ��ȡ�������߿��Ƶ�
		/// </summary>
		/// <param name="svgPathList"></param>
		/// <returns></returns>
		public PointF GetQuadraticControl(Interface.Paths.ISVGPathSegList svgPathList)
		{
			Interface.Paths.ISVGPathSeg prevSeg = svgPathList.PreviousSibling(this);
			if(prevSeg == null)
				throw new SVGException("��Ч��·������",SVGExceptionType.SVG_INVALID_VALUE_ERR,null);

			if(!(prevSeg is Interface.Paths.ISVGPathSegQuadratic))
			{
				return prevSeg.GetLastPoint (svgPathList);
			}
			else
			{
				PointF prevXY = prevSeg.GetLastPoint (svgPathList);
				PointF prevX1Y1 = ((Interface.Paths.ISVGPathSegQuadratic)prevSeg).GetQuadraticControl(svgPathList);

				return new PointF(2 * prevXY.X - prevX1Y1.X, 2 * prevXY.Y - prevX1Y1.Y);
			}
		}
		#endregion

		#region ..��������
		/// <summary>
		/// ��ȡ·�����ݵ��ı����
		/// </summary>
		public override string PathString
		{
			get
			{
				return "t" + this.X.ToString() + " "+ this.Y.ToString();
			}
		}
		#endregion
	}
}
