using System;

namespace YP.SVG.Paths
{
	/// <summary>
	/// M
	/// </summary>
	public class SVGPathSegMovetoAbs:SVGPathSegMove,Interface.Paths.ISVGPathSegMovetoAbs
	{
		#region ..构造及消除
		public SVGPathSegMovetoAbs(float x,float y):base(x,y)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.pathSegType = (short)PathSegmentType.PATHSEG_MOVETO_ABS;
			this.pathSegTypeAsLetter = "M";
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
				return "M" + this.X.ToString() + " " + this.Y.ToString();
			}
		}
		#endregion
	}
}
