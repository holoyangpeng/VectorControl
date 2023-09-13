using System;

namespace YP.SVG.Interface.Paths
{
	/// <summary>
	/// ���嵥Ԫ·���б�
	/// </summary>
	public interface ISVGPathSegList:Interface.DataType.ISVGTypeList
	{
//		/// <summary>
//		/// ��ȡ�б���Ŀ
//		/// </summary>
//		int NumberOfItems{get;}
//
//		/// <summary>
//		/// ����б���
//		/// </summary>
//		void Clear();

		/// <summary>
		/// ��ȡ�б������е�Ԫ�������Ӷ��ɵ��ַ���
		/// </summary>
		string PathSegTypeAsString{get;}

		/// <summary>
		/// ��ȡ·�����ݵ��ı����
		/// </summary>
		string PathString{get;}

//		/// <summary>
//		/// ��յ�ǰ�б������ָ����ISVGPathSeg��ʼ���б�
//		/// </summary>
//		ISVGPathSeg Initialize(ISVGPathSeg newItem);
//
//		/// <summary>
//		/// ��ȡ��������ֵ
//		/// </summary>
//		ISVGPathSeg GetItem(int index);
//
//		/// <summary>
//		/// ��ָ��������������ISVGPathSeg��
//		/// </summary>
//		ISVGPathSeg InsertItemBefore(ISVGPathSeg newItem, int index);
//
//		/// <summary>
//		/// ��ָ����ISVGPathSeg�滻ָ������������
//		/// </summary>
//		ISVGPathSeg ReplaceItem(ISVGPathSeg newItem, int index);
//
//		/// <summary>
//		/// �Ƴ�ָ������������
//		/// </summary>
//		ISVGPathSeg RemoveItem(int index);
//
//		/// <summary>
//		/// ���б�ĩβ���ISVGPathSeg��
//		/// </summary>
//		ISVGPathSeg AppendItem(ISVGPathSeg newItem);

		/// <summary>
		/// ��ȡָ�����ǰһ��
		/// </summary>
		ISVGPathSeg PreviousSibling(ISVGPathSeg svgPathSeg);

		/// <summary>
		/// ��ȡָ����ĺ�һ��
		/// </summary>
		ISVGPathSeg NextSibling(ISVGPathSeg svgPathSeg);

		/// <summary>
		/// ��ȡ������ת��֮����б�
		/// </summary>
		ISVGPathSegList NormalSVGPathSegList{get;}
	}
}
