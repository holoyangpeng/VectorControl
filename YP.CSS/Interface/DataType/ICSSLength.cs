using System;

namespace YP.Base.CSS.Interface.DataType
{
	/// <summary>
	/// ����CSS�г��ȵ�һ����Ϊ
	/// </summary>
	public interface ICSSLength
	{
		/// <summary>
		/// ��ȡ����ֵ
		/// </summary>
		float Value{get;}

		/// <summary>
		/// ��ȡ���ȷ���
		/// </summary>
		byte Direction{get;}
	}
}
