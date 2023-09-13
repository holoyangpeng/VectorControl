using System;
using System.Drawing;

namespace YP.SVG.Paths
{
	/// <summary>
	/// T
	/// </summary>
	public class SVGPathSegCurvetoQuadraticSmoothAbs:SVGPathSegCurve,Interface.Paths.ISVGPathSegCurvetoQuadraticSmoothAbs,Interface.Paths.ISVGPathSegQuadratic
	{
		#region ..构造及消除
		public SVGPathSegCurvetoQuadraticSmoothAbs(float x,float y):base(x,y,0,0,0,0)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.pathSegType = (short)PathSegmentType.PATHSEG_CURVETO_QUADRATIC_SMOOTH_ABS ;
			this.pathSegTypeAsLetter = "T";
		}
		#endregion

		#region ..获取第一个控制点
		/// <summary>
		/// 获取第一个控制点
		/// </summary>
		/// <param name="svgPathList"></param>
		/// <returns></returns>
		public override PointF GetFirstControl(Interface.Paths.ISVGPathSegList svgPathList)
		{
			Interface.Paths.ISVGPathSeg prevSeg = svgPathList.PreviousSibling(this);
			if(prevSeg == null)
				throw new SVGException("无效的路径数据",SVGExceptionType.SVG_INVALID_VALUE_ERR ,null);

			PointF prevPoint = prevSeg.GetLastPoint(svgPathList);
			PointF x1y1 = this.GetQuadraticControl (svgPathList);

			float x1 = prevPoint.X + (x1y1.X - prevPoint.X) * 2/3;
			float y1 = prevPoint.Y + (x1y1.Y - prevPoint.Y) * 2/3;
			
			return new PointF(x1, y1);
		}
		#endregion

		#region ..获取第二个控制点
		/// <summary>
		/// 获取第二个控制点
		/// </summary>
		/// <param name="svgPathList"></param>
		/// <returns></returns>
		public override PointF GetSecondControl(Interface.Paths.ISVGPathSegList svgPathList)
		{
			PointF x1y1 = this.GetQuadraticControl(svgPathList);
			float x2 = x1y1.X + (X - x1y1.X) / 3;
			float y2 = x1y1.Y + (Y - x1y1.Y) / 3;

			return new PointF(x2, y2);
		}
		#endregion

		#region ..获取二次曲线控制点
		/// <summary>
		/// 获取二次曲线控制点
		/// </summary>
		/// <param name="svgPathList"></param>
		/// <returns></returns>
		public PointF GetQuadraticControl(Interface.Paths.ISVGPathSegList svgPathList)
		{
			Interface.Paths.ISVGPathSeg prevSeg = svgPathList.PreviousSibling(this);
			if(prevSeg == null)
				throw new SVGException("无效的路径数据",SVGExceptionType.SVG_INVALID_VALUE_ERR,null);

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

		#region ..公共属性
		/// <summary>
		/// 获取路径数据的文本表达
		/// </summary>
		public override string PathString
		{
			get
			{
				return "T" + this.X.ToString() + " "+ this.Y.ToString();
			}
		}
		#endregion
	}
}
