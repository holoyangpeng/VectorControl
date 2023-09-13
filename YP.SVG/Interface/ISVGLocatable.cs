using System;

namespace YP.SVG.Interface
{
	/// <summary>
	/// 定义ISVGLocatable 的一般行为
	/// </summary>
	public interface ISVGLocatable:Interface.ISVGElement
	{
		YP.SVG.Interface.ISVGElement NearestViewportElement{get;}

		YP.SVG.Interface.ISVGElement FarthestViewportElement{get;}

		/// <summary>
		/// 获取节点边界
		/// </summary>
		Interface.DataType.ISVGRect GetBBox();

		/// <summary>
		/// 获取节点未参与变换之前的边界
		/// </summary>
		Interface.DataType.ISVGRect GetOriBBox();

		Interface.CTS.ISVGMatrix GetCTM();

		Interface.CTS.ISVGMatrix GetScreenCTM();

		Interface.CTS.ISVGMatrix GetTransformToElement(ISVGElement element);
	}
}
