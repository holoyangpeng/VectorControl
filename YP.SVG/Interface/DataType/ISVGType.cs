using System;

namespace YP.SVG.Interface.DataType
{
	/// <summary>
	/// ����SVG�������͵�һ����Ϊ
	/// </summary>
	public interface ISVGType
	{
		/// <summary>
		/// ��ȡ���������ݵ�Ĭ��ֵ
		/// </summary>
		string DefaultValue{get;}

		/// <summary>
		/// ��ȡ����ֵ���ı����
		/// </summary>
		/// <returns></returns>
		string ToString();
	}
}
