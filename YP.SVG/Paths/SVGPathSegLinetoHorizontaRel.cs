using System;
using System.Drawing;

namespace YP.SVG.Paths
{
	/// <summary>
	/// h
	/// </summary>
	public class SVGPathSegLinetoHorizontalRel:SVGPathSegLine,Interface.Paths.ISVGPathSegLinetoHorizontalRel
	{
		#region ..构造及消除
		public SVGPathSegLinetoHorizontalRel(float x):base(x,0)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.pathSegType = (short)PathSegmentType.PATHSEG_LINETO_HORIZONTAL_REL;
			this.pathSegTypeAsLetter = "h";
			this.Relative = true;
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
			return new PointF(prevPoint.X + X, prevPoint.Y);
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
				return "h" + this.X.ToString();
			}
		}
		#endregion
	}
}
