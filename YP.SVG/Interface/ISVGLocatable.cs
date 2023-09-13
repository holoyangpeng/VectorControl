using System;

namespace YP.SVG.Interface
{
	/// <summary>
	/// ����ISVGLocatable ��һ����Ϊ
	/// </summary>
	public interface ISVGLocatable:Interface.ISVGElement
	{
		YP.SVG.Interface.ISVGElement NearestViewportElement{get;}

		YP.SVG.Interface.ISVGElement FarthestViewportElement{get;}

		/// <summary>
		/// ��ȡ�ڵ�߽�
		/// </summary>
		Interface.DataType.ISVGRect GetBBox();

		/// <summary>
		/// ��ȡ�ڵ�δ����任֮ǰ�ı߽�
		/// </summary>
		Interface.DataType.ISVGRect GetOriBBox();

		Interface.CTS.ISVGMatrix GetCTM();

		Interface.CTS.ISVGMatrix GetScreenCTM();

		Interface.CTS.ISVGMatrix GetTransformToElement(ISVGElement element);
	}
}
