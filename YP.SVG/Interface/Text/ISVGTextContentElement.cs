using System;

using YP.SVG.Interface.DataType;
using YP.SVG.Interface.CTS;

namespace YP.SVG.Interface.Text
{
	/// <summary>
	/// ����SVGTextElement, SVGTSpanElement, SVGTRefElement, SVGAltGlyphElement and SVGTextPathElement��ͨ����Ϊ
	/// </summary>
	public interface ISVGTextContentElement
	{
		/// <summary>
		/// ��ȡ���ȵ������
		/// </summary>
		System.Enum LengthAdjust{get;}

		/// <summary>
		/// ��ȡTextLength����
		/// </summary>
		ISVGLength TextLength{get;}

		/// <summary>
		/// ��ȡ��ǰ�ڵ��н�Ҫ���Ƶ����ַ���Ŀ�������ɡ�tref����ָ����ı��ڵ��ַ�����
		/// </summary>
		/// <returns></returns>
		int     GetNumberOfChars ();

		/// <summary>
		/// ������ȫ���ַ�ʱ������Ҫ���ܵ�GDI����
		/// </summary>
		/// <returns></returns>
		float    GetComputedTextLength ();

		/// <summary>
		/// �������ַ�������Ҫ�ĳ���
		/// </summary>
		/// <param name="charnum">��ʼ�ַ�����</param>
		/// <param name="nchars">���ַ�������</param>
		/// <returns></returns>
		float    GetSubStringLength (int charnum,int nchars );

		/// <summary>
		/// ��ȡָ���������ַ��Ŀ�ʼ����λ��
		/// </summary>
		ISVGPoint GetStartPositionOfChar (int charnum );

		/// <summary>
		/// ��ȡָ���������ַ����ƽ���ʱ��λ��
		/// </summary>
		ISVGPoint GetEndPositionOfChar (int charnum );

		/// <summary>
		/// ��ȡָ���������ַ��Ļ��Ʊ߽�
		/// </summary>
		ISVGRect  GetExtentOfChar (int charnum );

		/// <summary>
		/// ��ȡָ���������ַ�����ڵ�ǰ�û��ռ����ת�Ƕ�
		/// </summary>
		/// <param name="charnum">�ַ�����</param>
		/// <returns></returns>
		float    GetRotationOfChar (int charnum );

		/// <summary>
		/// ��ȡָ��λ�ô����ַ�����
		/// </summary>
		/// <param name="point"></param>
		/// <returns></returns>
		//int     GetCharNumAtPosition (ISVGPoint point,ref YP.SVGDom.Text.TextContentInfo info);

		/// <summary>
		/// ѡ�����ַ���
		/// </summary>
		/// <param name="charnum">��ʼ�ַ�����</param>
		/// <param name="nchars">�ַ�������</param>
		void     SelectSubString (int charnum,int nchars );
	}
}
