using System;

namespace YP.SVG.Interface
{
	/// <summary>
	/// 定义ISVGTransformable 的一般行为
	/// </summary>
	public interface ISVGTransformable:Interface.ISVGElement
	{
		/// <summary>
		/// 获取二维变换对象
		/// </summary>
		Interface.CTS.ISVGTransformList Transform{get;}
	}
}
