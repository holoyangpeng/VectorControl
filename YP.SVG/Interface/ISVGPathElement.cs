using System;

namespace YP.SVG.Interface
{
	/// <summary>
	/// ����SVG�й���·���Ķ����һ����Ϊ
	/// </summary>
	public interface ISVGPathable
	{
		/// <summary>
		/// ��ȡ����·��
		/// </summary>
		System.Drawing.Drawing2D.GraphicsPath GPath{get;}

        //ISVGRenderer Render { get; }
	}
}
