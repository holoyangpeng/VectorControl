using System;
using System.Drawing;

namespace YP.SVG.Paths
{
	/// <summary>
	/// l
	/// </summary>
	public class SVGPathSegLinetoRel:SVGPathSegLine,Interface.Paths.ISVGPathSegLinetoRel
	{
		#region ..构造及消除
		public SVGPathSegLinetoRel(float x,float y):base(x,y)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.pathSegType = (short)PathSegmentType.PATHSEG_LINETO_REL;
			this.pathSegTypeAsLetter = "l";
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
			SVGPathSeg pre = (SVGPathSeg)svgPathList.PreviousSibling(this);
			PointF p = PointF.Empty;
			if(pre != null)
			{
				p = pre.GetLastPoint(svgPathList);
			}
			return new PointF(p.X + this.X,p.Y + this.Y);
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
				return "l" + this.X.ToString() + " "+ this.Y.ToString();
			}
		}
		#endregion
	}
}
