using System;
using System.Drawing;

namespace YP.SVG.Paths
{
	/// <summary>
	/// L
	/// </summary>
	public class SVGPathSegLinetoAbs:SVGPathSegLine,Interface.Paths.ISVGPathSegLinetoAbs 
	{
		#region ..构造及消除
		public SVGPathSegLinetoAbs(float x,float y):base(x,y)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.pathSegType = (short)PathSegmentType.PATHSEG_LINETO_ABS;
			this.pathSegTypeAsLetter = "L";
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
			return new PointF(this.X,this.Y);
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
				return "L" + this.X.ToString() + " "+ this.Y.ToString();
			}
		}
		#endregion
	}
}
