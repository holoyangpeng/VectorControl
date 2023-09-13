using System;

namespace YP.SVG.Interface.CTS
{
	/// <summary>
	/// 定义ISVGPreserveAspectRatio 的一般行为
	/// </summary>
	public interface ISVGPreserveAspectRatio:Interface.DataType.ISVGType
	{
		/// <summary>
		/// 获取或设置视图对齐属性
		/// </summary>
		SVGPreserveAspectRatioType Align{get;set;}

		/// <summary>
		/// 获取或设置视图的剪切类别
		/// </summary>
		SVGMeetOrSliceType MeetOrSlice {set;get;}
	}
}
