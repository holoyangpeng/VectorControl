using System;

namespace YP.SVG.Paths
{
	/// <summary>
	/// 实现命令a
	/// </summary>
	public class SVGPathSegArcRel:SVGPathSegArc,Interface.Paths.ISVGPathSegArcRel
	{
		#region ..构造及消除
		public SVGPathSegArcRel(float x,float y,float r1,float r2,float angle,bool largeArcFlag,bool sweepFlag):base(x,y,r1,r2,angle,largeArcFlag,sweepFlag)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.pathSegTypeAsLetter = "a";
			this.pathSegType = (short)PathSegmentType.PATHSEG_ARC_REL;
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
				return "a" + this.R1.ToString() + " " + this.R2.ToString() + " " + this.Angle + " " + (this.LargeArcFlag?"1":"0") + " " + (this.SweepFlag?"1":"0") + " " +this.X.ToString() + " " + this.Y.ToString();
			}
		}
		#endregion
	}
}
