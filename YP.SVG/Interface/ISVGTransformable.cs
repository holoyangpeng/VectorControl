using System;

namespace YP.SVG.Interface
{
	/// <summary>
	/// ����ISVGTransformable ��һ����Ϊ
	/// </summary>
	public interface ISVGTransformable:Interface.ISVGElement
	{
		/// <summary>
		/// ��ȡ��ά�任����
		/// </summary>
		Interface.CTS.ISVGTransformList Transform{get;}
	}
}
