using System;

namespace YP.Base.CSS.Interface
{
	/// <summary>
	/// 定义选择器的一般行为
	/// </summary>
	public interface ISelector
	{
		/// <summary>
		/// 获取选择器的层次，其用途在于表明规则的重要级别，对于具备属性的两项规则，取层次较高的规则属性为最终属性值
		/// </summary>
		int Level{get;}
	}
}
