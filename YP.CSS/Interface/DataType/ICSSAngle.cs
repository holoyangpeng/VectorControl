using System;

namespace YP.Base.CSS.Interface.DataType
{
	/// <summary>
	/// ����CSS�нǶȵ�һ����Ϊ
	/// </summary>
	public interface ICSSAngle
	{
		/// <summary>
		/// ��ȡ�Ƕ�ֵ
		/// </summary>
		float Value{get;}

		/// <summary>
		/// ��ȡ�Ƕ�����
		/// </summary>
		byte AngleType{get;}
	}
}
