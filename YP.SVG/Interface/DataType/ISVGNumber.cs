using System;

namespace YP.SVG.Interface.DataType
{
	/// <summary>
	/// 定义SVGNumber的一般行为
	/// </summary>
	public interface ISVGNumber:ISVGType
	{
		/// <summary>
		/// 获取或设置用浮点值表达的值
		/// </summary>
		float Value{get;set;}
	}
}
