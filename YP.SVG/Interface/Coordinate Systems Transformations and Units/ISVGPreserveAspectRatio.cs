using System;

namespace YP.SVG.Interface.CTS
{
	/// <summary>
	/// ����ISVGPreserveAspectRatio ��һ����Ϊ
	/// </summary>
	public interface ISVGPreserveAspectRatio:Interface.DataType.ISVGType
	{
		/// <summary>
		/// ��ȡ��������ͼ��������
		/// </summary>
		SVGPreserveAspectRatioType Align{get;set;}

		/// <summary>
		/// ��ȡ��������ͼ�ļ������
		/// </summary>
		SVGMeetOrSliceType MeetOrSlice {set;get;}
	}
}
