using System;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// ��������l
	/// </summary>
	public interface ISVGPathSegLinetoRel:ISVGPathSeg
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
