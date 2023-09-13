using System;

namespace YP.SVG.Interface.DataType
{
	/// <summary>
	/// ����ISVGIccColor��һ����Ϊ
	/// </summary>
	public interface ISVGIccColor:ISVGType
	{
		/// <summary>
		/// ����ICC Color�����һϵ��ֵ
		/// </summary>
		Interface.DataType.ISVGNumberList Colors{get;}
		
		/// <summary>
		/// ��ȡ������ICC Color��ColorProfile
		/// </summary>
		string ColorProfile{get;set;}
	}
}
