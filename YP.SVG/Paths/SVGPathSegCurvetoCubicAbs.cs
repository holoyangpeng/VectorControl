using System;
using System.Drawing;

namespace YP.SVG.Paths
{
	/// <summary>
	/// 定义命令C
	/// </summary>
	public class SVGPathSegCurvetoCubicAbs:SVGPathSegCurve,Interface.Paths.ISVGPathSegCurvetoCubicAbs,Interface.Paths.ISVGPathSegCubic
	{
		#region ..构造及消除
		public SVGPathSegCurvetoCubicAbs(float x,float y,float x1,float y1,float x2,float y2):base(x,y,x1,y1,x2,y2)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.pathSegTypeAsLetter = "C";
			this.pathSegType = (short)PathSegmentType.PATHSEG_CURVETO_CUBIC_ABS;
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
			return new System.Drawing.PointF(this.X1,this.Y1);
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
			return new System.Drawing.PointF(this.X2,this.Y2);
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
				return "C" + this.X1.ToString() + " " + this.Y1.ToString() + " " + this.X2.ToString() + " " + this.Y2.ToString() + " " + this.X.ToString() + " "+ this.Y.ToString();
			}
		}
		#endregion
	}
}
