using System;

namespace YP.Base.StyleSheets
{
	/// <summary>
	/// ����StyleSheet���ϵ�һ����Ϊ
	/// </summary>
	public interface IStyleSheetList
	{
		/// <summary>
		/// ��ȡ������Ŀ
		/// </summary>
		int Count{get;}

		/// <summary>
		/// ��ȡ������ָ������������
		/// </summary>
		StyleSheets.IStyleSheet this[int index]{set;get;}
	}
}
