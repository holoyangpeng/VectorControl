using System;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// ��������s
	/// </summary>
	public interface ISVGPathSegCurvetoCubicSmoothRel:ISVGPathSeg
	{
		/// <summary>
		/// �����յ����Ժ�����
		/// </summary>
		float X{set;get;}

		/// <summary>
		/// �����յ�����������
		/// </summary>
		float Y{set;get;}

		/// <summary>
		/// ����ڶ����Ƶ����Ժ�����
		/// </summary>
		float X2{set;get;}

		/// <summary>
		/// ����ڶ����Ƶ�����������
		/// </summary>
		float Y2{set;get;}
	}
}
