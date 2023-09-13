using System;

namespace YP.SVG.Interface.DataType
{
	/// <summary>
	/// ����SVG�е��б����͵�һ����Ϊ
	/// </summary>
	public interface ISVGTypeList:ISVGType
	{
		/// <summary>
		/// ��ȡ�б���Ŀ
		/// </summary>
		int NumberOfItems{get;}

		/// <summary>
		/// ����б���
		/// </summary>
		void Clear();

		/// <summary>
		/// ��յ�ǰ�б������ָ����ISVGType��ʼ���б�
		/// </summary>
		ISVGType Initialize(ISVGType newItem);

		/// <summary>
		/// ��ȡ��������ֵ
		/// </summary>
		ISVGType GetItem(int index);

		/// <summary>
		/// ��ָ��������������SvgNumber��
		/// </summary>
		ISVGType InsertItemBefore(ISVGType newItem, int index);

		/// <summary>
		/// ��ָ����ISVGType�滻ָ������������
		/// </summary>
		ISVGType ReplaceItem(ISVGType newItem, int index);

		/// <summary>
		/// �Ƴ�ָ������������
		/// </summary>
		ISVGType RemoveItem(int index);

		/// <summary>
		/// ���б�ĩβ���ISVGType��
		/// </summary>
		ISVGType AppendItem(ISVGType newItem);

		/// <summary>
		/// ȷ��ĳ��Ԫ���Ƿ����б���
		/// </summary>
		/// <param name="child"></param>
		/// <returns></returns>
		bool Contains(object child);

		/// <summary>
		/// ����ָ�������б��е�����
		/// </summary>
		/// <param name="svgType"></param>
		/// <returns></returns>
		int IndexOf(Interface.DataType.ISVGType svgType);
	}
}
