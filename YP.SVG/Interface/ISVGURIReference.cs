using System;

namespace YP.SVG.Interface
{
	/// <summary>
	/// 定义ISVGURIReference 的一般行为，这一类对象都具备"href"属性
	/// </summary>
	public interface ISVGURIReference
	{
		DataType.ISVGString Href{get;}
	}
}
