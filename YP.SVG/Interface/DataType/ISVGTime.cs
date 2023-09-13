using System;

namespace YP.SVG.Interface.DataType
{
	/// <summary>
	/// ����Svg��ʱ��
	/// </summary>
	public interface ISVGTime
	{
		/// <summary>
		/// ��ȡʱ�����
		/// </summary>
		byte SVGTimeType{get;}

		/// <summary>
		/// ��ȡʱ��ֵ
		/// </summary>
		int Value{get;}

		/// <summary>
		/// �ж϶����Ƿ���ָ���Ķ������
		/// </summary>
		/// <param name="refTime">���ȽϵĶ���</param>
		/// <returns></returns>
		bool Equal(ISVGTime refTime);
	}
}
