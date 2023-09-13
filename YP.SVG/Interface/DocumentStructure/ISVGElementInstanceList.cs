using System;

namespace YP.SVG.Interface.DocumentStructure
{
	/// <summary>
	/// ISVGElementInstanceList ��ժҪ˵����
	/// </summary>
	public interface ISVGElementInstanceList
	{
		/// <summary>
		/// ��ȡ�б���
		/// </summary>
		ulong Length{get;}

		/// <summary>
		/// ��ȡ�б��ض�����
		/// </summary>
		ISVGElementInstance Item ( ulong index );
	}
}
