using System;

namespace YP.SVG.Interface
{
	/// <summary>
	/// ����Ԫ�ؼ��ϵ�һ�����Ժͷ�����
	/// ���WebԪ����һ��͹�����Ԫ�ؼ��ϣ�����IElementCollectionʵ�ֶԼ���Ԫ�صĲ���
	/// </summary>
	public interface IElementCollection
	{
		/// <summary>
		/// �Լ��Ͻ�����ӡ����롢ɾ�����������ʱ�������޸��¼���������
		/// </summary>
		event CollectionChangedEventHandler CollectionChanged;

		/// <summary>
		/// ��ȡ�������������Ķ���
		/// </summary>
		SVGElement this[int index]{get;set;}

		/// <summary>
		/// ��ȡ���ϳ���
		/// </summary>
		int Count{get;}

		/// <summary>
		/// ��������е�������
		/// </summary>
		void Clear();
	}
}
