using System;
using System.Collections;

namespace YP.Base.Interface
{
	/// <summary>
	/// ����һ��Ԫ�����߱������ԣ�����
	/// </summary>
	public interface IWebElement
	{
		/// <summary>
		/// ��ȡ���������Ӽ��ڵ���ı��б�
		/// </summary>
		string InnerText{get;}

		/// <summary>
		/// ��ȡ�ڵ��BaseURI
		/// </summary>
		string BaseURI{get;}

		/// <summary>
		/// ��ȡָ�����Ե�ԭʼֵ
		/// </summary>
		/// <param name="name">��������</param>
		/// <returns></returns>
		string GetAttribute(string attributeName);
	}
}
