using System;
using System.Drawing;

namespace YP.SVG.Paths
{
	/// <summary>
	/// SVGPathSegCurvetoQuadraticRel ��ժҪ˵����
	/// </summary>
	public class SVGPathSegCurvetoQuadraticRel:SVGPathSegCurve,Interface.Paths.ISVGPathSegCurvetoQuadraticRel,Interface.Paths.ISVGPathSegQuadratic
	{
		#region ..���켰����
		public SVGPathSegCurvetoQuadraticRel(float x,float y,float x1,float y1):base(x,y,x1,y1,0,0)
		{
			this.pathSegType = (short)PathSegmentType.PATHSEG_CURVETO_QUADRATIC_REL;
			this.pathSegTypeAsLetter ="q";
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
				throw new SVGException("",SVGExceptionType.SVG_INVALID_VALUE_ERR,null);

			PointF prevPoint = prevSeg.GetLastPoint(svgPathList);
			float x1 = prevPoint.X + (this.X1) * 2/3;
			float y1 = prevPoint.Y + (this.Y1) * 2/3;
			
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
			Interface.Paths.ISVGPathSeg prevSeg = svgPathList.PreviousSibling(this);
			if(prevSeg == null)
				throw new SVGException("",SVGExceptionType.SVG_INVALID_VALUE_ERR,null);

			PointF prevPoint = prevSeg.GetLastPoint(svgPathList);
			float x2 = prevPoint.X + X1 + (X - X1) / 3;
			float y2 = prevPoint.Y + Y1 + (Y - Y1) / 3;

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
			PointF prevPoint = PointF.Empty;
			if(prevSeg != null) 
				prevPoint = prevSeg.GetLastPoint(svgPathList);

			return new PointF(prevPoint.X + X1, prevPoint.Y + Y1);
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
				return "q" + this.X1.ToString() + " " + this.Y1.ToString() + " " + this.X.ToString() + " "+ this.Y.ToString();
			}
		}
		#endregion
	}
}
