using System;

namespace YP.SVG.Interface.CTS
{
	/// <summary>
	/// ����ISVGPoint ��һ����Ϊ
	/// </summary>
	public interface ISVGPoint:Interface.DataType.ISVGType
	{
		/// <summary>
		/// ��ȡ�����ú�����
		/// </summary>
		float X{set;get;}

		/// <summary>
		/// ��ȡ������������
		/// </summary>
		float Y{set;get;}

		/// <summary>
		/// ����ָ����SVGMatrix���б任
		/// </summary>
		ISVGPoint MatrixTransform(ISVGMatrix matrix);
	}
}
