using System;
using System.Drawing;

namespace YP.SVG.Paths
{
	/// <summary>
	/// V
	/// </summary>
	public class SVGPathSegLinetoVerticalAbs:SVGPathSegLine,Interface.Paths.ISVGPathSegLinetoVerticalAbs
	{
		#region ..构造及消除
		public SVGPathSegLinetoVerticalAbs(float y):base(0,y)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.pathSegType = (short)PathSegmentType.PATHSEG_LINETO_VERTICAL_ABS;
			this.pathSegTypeAsLetter = "V";
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
			return new PointF(prevPoint.X, Y);
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
				return "V" + this.Y.ToString();
			}
		}
		#endregion
	}
}
