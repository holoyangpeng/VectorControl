using System;

namespace YP.Base.CSS.Interface
{
	/// <summary>
	/// 定义@charset
	/// </summary>
	public interface ICSSChartSetRule
	{
		/// <summary>
		/// 获取charset所包含的encoding信息
		/// </summary>
		System.Text.Encoding Encoding{get;}
	}
}
