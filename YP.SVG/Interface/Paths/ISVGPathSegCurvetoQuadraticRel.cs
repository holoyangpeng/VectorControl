using System;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// ��������q
	/// </summary>
	public interface ISVGPathSegCurvetoQuadraticRel:ISVGPathSeg
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
		/// ������Ƶ����Ժ�����
		/// </summary>
		float X1{set;get;}

		/// <summary>
		/// ������Ƶ�����������
		/// </summary>
		float Y1{set;get;}
	}
}
