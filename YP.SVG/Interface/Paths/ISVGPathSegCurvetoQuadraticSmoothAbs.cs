using System;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// ��������T
	/// </summary>
	public interface ISVGPathSegCurvetoQuadraticSmoothAbs:ISVGPathSeg
	{
		/// <summary>
		/// �յ���Ժ�����
		/// </summary>
		float X{set;get;}

		/// <summary>
		/// �յ����������
		/// </summary>
		float Y{set;get;}
	}
}
