using System;
using System.Collections;

namespace YP.Base.Interface
{
	/// <summary>
	/// ����֧��CSS��WebԪ�����߱���һ�����Ժͷ���
	/// </summary>
	public interface IStyleElement
	{
		/// <summary>
		/// �ж϶����Ƿ����ָ��xpath
		/// </summary>
		/// <param name="xpath"></param>
		/// <returns></returns>
		bool MatchXPath(string xpath);
	}
}
