using System;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// ��������Q
	/// </summary>
	public interface ISVGPathSegCurvetoQuadraticAbs:ISVGPathSeg
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
		/// ������Ƶ�ľ��Ժ�����
		/// </summary>
		float X1{set;get;}

		/// <summary>
		/// ������Ƶ�ľ���������
		/// </summary>
		float Y1{set;get;}
	}
}
