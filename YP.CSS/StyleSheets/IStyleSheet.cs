using System;

namespace YP.Base.StyleSheets
{
	/// <summary>
	/// ����һ��StyleSheet�Ļ��෽����һ��StyleSheet�ṩ��֮����ĵ����е�CSS���������
	/// ��Html�У�ͨ��link�����ⲿStyle�ļ�����ͨ��Style�ڵ���Խ���һ��StyleSheet��
	/// ��XML�У�ͨ������һ���ⲿ�ļ�����һ��StyleSheet
	/// </summary>
	public interface IStyleSheet
	{
		/// <summary>
		/// ��ȡStyleSheet����λ��
		/// </summary>
		Uri Href{get;}

		/// <summary>
		/// ��ȡ����
		/// </summary>
		string Type{get;}	

		/// <summary>
		/// ��ȡ����
		/// </summary>
		string Title{get;}
	
		/// <summary>
		/// ��ȡ��֮��ϵ�Ķ���
		/// </summary>
		Base.Interface.IWebElement OwnerElement{get;}
	}
}
