using System;

namespace YP.SVG.Interface.DataType
{
	/// <summary>
	/// ����SVGNumber��һ����Ϊ
	/// </summary>
	public interface ISVGNumber:ISVGType
	{
		/// <summary>
		/// ��ȡ�������ø���ֵ����ֵ
		/// </summary>
		float Value{get;set;}
	}
}
