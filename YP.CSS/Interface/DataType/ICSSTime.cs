using System;

namespace YP.Base.CSS.Interface.DataType
{
	/// <summary>
	/// ����CSS��ʱ���һ����Ϊ
	/// </summary>
	public interface ICSSTime
	{
		/// <summary>
		/// ��ȡʱ��ֵ
		/// </summary>
		float Value{get;}

		/// <summary>
		/// ��ȡʱ������
		/// </summary>
		byte TimeType{get;}
	}
}
