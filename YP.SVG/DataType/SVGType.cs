using System;

namespace YP.SVG.DataType
{
	/// <summary>
	/// 实现SVG中数据的基类方法
	/// </summary>
	public abstract class SVGType:Interface.DataType.ISVGType
	{
		#region ..构造及消除
		public SVGType()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}
		#endregion

		#region ..私有变量
		public string defaultValue = string.Empty;
		#endregion

		#region ..获取数据的默认值
		/// <summary>
		/// 获取数据的默认值
		/// </summary>
		public string DefaultValue
		{
			get
			{
				return this.defaultValue;
			}
		}
		#endregion

		#region ..获取类型值的文本表达
		/// <summary>
		/// 获取类型值的文本表达
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Empty;
		}
		#endregion
	}
}
