using System;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// ��������A
	/// </summary>
	public interface ISVGPathSegArcAbs:ISVGPathSeg
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
		/// �����뾶
		/// </summary>
		float R1{set;get;}

		/// <summary>
		/// ������ݰ뾶
		/// </summary>
		float R2{set;get;}

		/// <summary>
		/// ����ڶ����Ƶ�ľ��Ժ�����
		/// </summary>
		float Angle{set;get;}

		/// <summary>
		/// The value of the large-arc-flag parameter
		/// </summary>
		bool LargeArcFlag {set;get;}

		/// <summary>
		/// The value of the sweep-flag parameter. 
		/// </summary>
		bool SweepFlag{set;get;}
	}
}
