using System;
using System.Drawing;

namespace YP.SVG.Paths
{
	/// <summary>
	/// c
	/// </summary>
	public class SVGPathSegCurvetoCubicRel:SVGPathSegCurve,Interface.Paths.ISVGPathSegCurvetoCubicRel,Interface.Paths.ISVGPathSegCubic
	{
		#region ..构造及消除
		public SVGPathSegCurvetoCubicRel(float x,float y,float x1,float y1,float x2,float y2):base(x,y,x1,y1,x2,y2)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.pathSegTypeAsLetter = "c";
			this.pathSegType = (short)PathSegmentType.PATHSEG_CURVETO_CUBIC_REL;
			this.Relative = true;
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
			System.Drawing.PointF p = System.Drawing.PointF.Empty;
			Interface.Paths.ISVGPathSeg pre = svgPathList.PreviousSibling(this);
			if(pre != null)
				p = pre.GetLastPoint(svgPathList);
			return new System.Drawing.PointF(p.X + this.X1,p.Y + this.Y1);
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
			System.Drawing.PointF p = System.Drawing.PointF.Empty;
			Interface.Paths.ISVGPathSeg pre = svgPathList.PreviousSibling(this);
			if(pre != null)
				p = pre.GetLastPoint(svgPathList);
			return new System.Drawing.PointF(p.X + this.X2,p.Y + this.Y2);
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
				return "c" + this.X1.ToString() + " " + this.Y1.ToString() + " " + this.X2.ToString() + " " + this.Y2.ToString() + " " + this.X.ToString() + " "+ this.Y.ToString();
			}
		}
		#endregion
	}
}
