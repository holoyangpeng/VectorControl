using System;

namespace YP.SVG.Interface
{
	/// <summary>
	/// ����SVG�е������������������˶���֮��һ�㻹�����������Ԫ��
	/// </summary>
	public interface ISVGContainer
	{
		/// <summary>
		/// ��ȡ�Ӽ�Ԫ��
		/// </summary>
		YP.SVG.SVGElementCollection ChildElements{get;}

		/// <summary>
		/// �жϽڵ��Ƿ�����Ч���Ӽ��ڵ�
		/// </summary>
		/// <param name="child">�Ӽ��ڵ�</param>
		/// <returns></returns>
		bool ValidChild(Interface.ISVGElement child);
	}
}
