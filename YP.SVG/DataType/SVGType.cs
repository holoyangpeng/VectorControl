using System;

namespace YP.SVG.DataType
{
	/// <summary>
	/// ʵ��SVG�����ݵĻ��෽��
	/// </summary>
	public abstract class SVGType:Interface.DataType.ISVGType
	{
		#region ..���켰����
		public SVGType()
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
		}
		#endregion

		#region ..˽�б���
		public string defaultValue = string.Empty;
		#endregion

		#region ..��ȡ���ݵ�Ĭ��ֵ
		/// <summary>
		/// ��ȡ���ݵ�Ĭ��ֵ
		/// </summary>
		public string DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
		}
		#endregion

		#region ..��ȡ����ֵ���ı����
		/// <summary>
		/// ��ȡ����ֵ���ı����
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Empty;
		}
		#endregion
	}
}
