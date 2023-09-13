using System;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// ��������C
	/// </summary>
	public interface ISVGPathSegCurvetoCubicAbs:ISVGPathSeg
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
		/// �����һ���Ƶ�ľ��Ժ�����
		/// </summary>
		float X1{set;get;}

		/// <summary>
		/// �����һ���Ƶ�ľ���������
		/// </summary>
		float Y1{set;get;}

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
