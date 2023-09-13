using System;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// ��������c
	/// </summary>
	public interface ISVGPathSegCurvetoCubicRel:ISVGPathSeg
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
		/// �����һ���Ƶ����Ժ�����
		/// </summary>
		float X1{set;get;}

		/// <summary>
		/// �����һ���Ƶ�����������
		/// </summary>
		float Y1{set;get;}

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
