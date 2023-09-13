using System;
using System.Drawing;

namespace YP.SVG.Paths
{
	/// <summary>
	/// 实现S
	/// </summary>
	public class SVGPathSegCurvetoCubicSmoothAbs:SVGPathSegCurve,Interface.Paths.ISVGPathSegCurvetoCubicSmoothAbs,Interface.Paths.ISVGPathSegCubic
	{
		#region ..构造及消除
		public SVGPathSegCurvetoCubicSmoothAbs(float x,float y,float x2,float y2):base(x,y,0,0,x2,y2)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.pathSegType = (short)PathSegmentType.PATHSEG_CURVETO_CUBIC_SMOOTH_ABS;
			this.pathSegTypeAsLetter = "S";
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
				throw new SVGException ("无效的路径格式",SVGExceptionType.SvgSHARP_UNKNOWN_ERROR,null);

			if(!(prevSeg is Interface.Paths.ISVGPathSegCubic) || prevSeg is Interface.Paths.ISVGPathSegQuadratic)
			{
				return prevSeg.GetLastPoint(svgPathList);
			}
			else
			{
				PointF prevXY = prevSeg.GetLastPoint(svgPathList);
				PointF prevX2Y2 = ((Interface.Paths.ISVGPathSegCubic)prevSeg).GetSecondControl(svgPathList);

				return new PointF(2 * prevXY.X - prevX2Y2.X, 2 * prevXY.Y - prevX2Y2.Y);
			}
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
			return new PointF(this.X2,this.Y2);
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
				return "S" + this.X2.ToString() + " " + this.Y2.ToString() + " " + this.X.ToString() + " "+ this.Y.ToString();
			}
		}
		#endregion
	}
}
