using System;
using System.Drawing;

namespace YP.SVG.Paths
{
	/// <summary>
	/// Q
	/// </summary>
	public class SVGPathSegCurvetoQuadraticAbs:SVGPathSegCurve,Interface.Paths.ISVGPathSegCurvetoQuadraticAbs,Interface.Paths.ISVGPathSegQuadratic
	{
		#region ..���켰����
		public SVGPathSegCurvetoQuadraticAbs(float x,float y,float x1,float y1):base(x,y,x1,y1,0,0)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.pathSegType = (short)PathSegmentType.PATHSEG_CURVETO_QUADRATIC_ABS;
			this.pathSegTypeAsLetter ="Q";
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
			float x1 = prevPoint.X + (this.X1 - prevPoint.X) * 2/3;
			float y1 = prevPoint.Y + (this.Y1 - prevPoint.Y) * 2/3;
			
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
			float x2 = X1 + (X - X1) / 3;
			float y2 = Y1 + (Y - Y1) / 3;

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
			return new PointF(X1, Y1);
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
				return "Q" + this.X1.ToString() + " " + this.Y1.ToString() + " " + this.X.ToString() + " "+ this.Y.ToString();
			}
		}
		#endregion
	}
}
