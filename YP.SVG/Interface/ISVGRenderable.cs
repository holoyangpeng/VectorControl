using System;

namespace YP.SVG.Interface
{
	/// <summary>
	/// ����SVG�пɻ��ƵĽڵ��һ����Ϊ
	/// </summary>
    public interface ISVGRenderer
	{
		/// <summary>
		/// ����Ԫ��
		/// </summary>
		/// <param name="g">����</param>
		/// <param name="sp">��������</param>
		void Draw(System.Drawing.Graphics g,YP.SVG.StyleContainer.StyleOperator sp);
	}
}
