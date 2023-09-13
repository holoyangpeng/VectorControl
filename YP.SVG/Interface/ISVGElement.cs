using System;

namespace YP.SVG.Interface
{
	/// <summary>
	/// ʵ��SVGElement��һ����Ϊ
	/// </summary>
	public interface ISVGElement 
	{
		/// <summary>
		/// ��ȡ�ڵ��������ĵ�����
		/// </summary>
		Document.SVGDocument OwnerDocument{get;}

		/// <summary>
		/// ��ȡ������ǰ��ͼ����ͼ����һ��Ϊ�����һ��SVG����
		/// </summary>
		Interface.ISVGElement ViewPortElement{get;}

		/// <summary>
		/// ��ȡ���һ���SVG����
		/// </summary>
		Interface.DocumentStructure.ISVGSVGElement OwnerSvgElement{get;}

		/// <summary>
		/// ��ȡ�ڵ��ID����
		/// </summary>
		string ID{get;}

		/// <summary>
		/// �жϽڵ��Ƿ���Ի���
		/// </summary>
		//bool CanRender{get;}

		/// <summary>
		/// �����Է����޸�ʱ�����ʱ�����¶�������
		/// </summary>
		/// <param name="attributeName">��������</param>
		/// <param name="attributeValue">����ֵ</param>
		//void AddSVGAttribute(string attributeName,string attributeValue);
	}
}
