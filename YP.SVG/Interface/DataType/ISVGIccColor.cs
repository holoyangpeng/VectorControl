using System;

namespace YP.SVG.Interface.DataType
{
	/// <summary>
	/// 定义ISVGIccColor的一般行为
	/// </summary>
	public interface ISVGIccColor:ISVGType
	{
		/// <summary>
		/// 定义ICC Color对象的一系列值
		/// </summary>
		Interface.DataType.ISVGNumberList Colors{get;}
		
		/// <summary>
		/// 获取或设置ICC Color的ColorProfile
		/// </summary>
		string ColorProfile{get;set;}
	}
}
