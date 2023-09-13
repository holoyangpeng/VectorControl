using System;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// ��������a
	/// </summary>
	public interface ISVGPathSegArcRel:ISVGPathSeg
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
		/// �����뾶
		/// </summary>
		float R1{set;get;}

		/// <summary>
		/// ������ݰ뾶
		/// </summary>
		float R2{set;get;}

		/// <summary>
		/// ����ڶ����Ƶ����Ժ�����
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
