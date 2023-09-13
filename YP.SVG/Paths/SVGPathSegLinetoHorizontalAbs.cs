using System;
using System.Drawing;

namespace YP.SVG.Paths
{
	/// <summary>
	/// H
	/// </summary>
	public class SVGPathSegLinetoHorizontalAbs:SVGPathSegLine,Interface.Paths.ISVGPathSegLinetoHorizontalAbs
	{
		#region ..构造及消除
		public SVGPathSegLinetoHorizontalAbs(float x):base(x,0)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.pathSegType = (short)PathSegmentType.PATHSEG_LINETO_HORIZONTAL_ABS;
			this.pathSegTypeAsLetter = "H";
		}
		#endregion

		#region ..获取路径终点
		/// <summary>
		/// 获取路径中终点
		/// </summary>
		/// <param name="svgPathList"></param>
		/// <returns></returns>
		public override PointF GetLastPoint(Interface.Paths.ISVGPathSegList svgPathList)
		{
			SVGPathSeg prevSeg = (SVGPathSeg)svgPathList.PreviousSibling(this);
			PointF prevPoint;
			if(prevSeg == null) prevPoint = new PointF(0,0);
			else prevPoint = prevSeg.GetLastPoint(svgPathList);
			return new PointF(X, prevPoint.Y);
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
				return "H" + this.X.ToString();
			}
		}
		#endregion
	}
}
