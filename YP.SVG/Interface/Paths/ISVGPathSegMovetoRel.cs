using System;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// ��������m
	/// </summary>
	public interface ISVGPathSegMovetoRel:ISVGPathSeg
	{
		/// <summary>
		/// ����Ŀ������Ժ�����
		/// </summary>
		float X{set;get;}

		/// <summary>
		/// ����Ŀ�������������
		/// </summary>
		float Y{set;get;}
	}
}
