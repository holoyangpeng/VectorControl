using System;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// ��������M
	/// </summary>
	public interface ISVGPathSegMovetoAbs:ISVGPathSeg
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
