using System;

namespace YP.Base.CSS.Interface
{
	/// <summary>
	/// ����CSS�е���һ���������ݵ�һ����Ϊ
	/// </summary>
	public interface ICSSRuleSetContent
	{
		/// <summary>
		/// ��ȡָ������ֵ
		/// </summary>
		/// <param name="propertyname">��������</param></param>
		/// <returns></returns>
		string GetProperty(string propertyname);

		/// <summary>
		/// ����ָ�����Ե�ֵ
		/// </summary>
		/// <param name="propertyname">��������</param>
		/// <param name="propertyvalue">����ֵ</param>
		/// <param name="priority">����˵�����Ե����ȼ���������"important��˵�����Ե���Ҫ��</param>
		/// <param name="level">���Բ��</param>
		void SetProperty(string propertyname,string propertyvalue,string priority,int level);

		
		/// <summary>
		/// ��ȡ������CSSRuleSet
		/// </summary>
		Interface.ICSSRuleSet ParentRuleSet{get;}

		/// <summary>
		/// ��ȡCSS����������
		/// </summary>
		string CSSText{get;set;}

		/// <summary>
		/// ��ȡ��������ָ�������Ը���
		/// </summary>
		int NumberOfItems{get;}


	}
}
