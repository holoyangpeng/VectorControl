using System;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// ��������L
	/// </summary>
	public interface ISVGPathSegLinetoAbs:ISVGPathSeg
	{
		/// <summary>
		/// ��ȡ������Ŀ���ĺ�����
		/// </summary>
		float X{get;set;}

		/// <summary>
		/// ��ȡ������Ŀ����������
		/// </summary>
		float Y{set;get;}
	}
}
