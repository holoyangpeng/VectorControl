using System;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// ����S
	/// </summary>
	public interface ISVGPathSegCurvetoCubicSmoothAbs:ISVGPathSeg
	{
		/// <summary>
		/// �����յ�ľ��Ժ�����
		/// </summary>
		float X{set;get;}

		/// <summary>
		/// �����յ�ľ���������
		/// </summary>
		float Y{set;get;}

		/// <summary>
		/// ����ڶ����Ƶ�ľ��Ժ�����
		/// </summary>
		float X2{set;get;}

		/// <summary>
		/// ����ڶ����Ƶ�ľ���������
		/// </summary>
		float Y2{set;get;}
	}
}
