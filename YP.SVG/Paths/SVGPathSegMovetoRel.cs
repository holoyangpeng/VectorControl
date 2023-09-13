using System;

namespace YP.SVG.Paths
{
	/// <summary>
	/// m
	/// </summary>
	public class SVGPathSegMovetoRel:SVGPathSegMove,Interface.Paths.ISVGPathSegMovetoRel
	{
		#region ..构造及消除
		public SVGPathSegMovetoRel(float x,float y ):base(x,y)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			this.pathSegType = (short)PathSegmentType.PATHSEG_MOVETO_REL;
			this.pathSegTypeAsLetter = "m";
			this.Relative = true;
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
				return "m" + this.X.ToString() + " " + this.Y.ToString();
			}
		}
		#endregion
	}
}
