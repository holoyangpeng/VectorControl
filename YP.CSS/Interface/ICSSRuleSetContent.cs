using System;

namespace YP.Base.CSS.Interface
{
	/// <summary>
	/// 定义CSS中单独一条声明内容的一般行为
	/// </summary>
	public interface ICSSRuleSetContent
	{
		/// <summary>
		/// 获取指定属性值
		/// </summary>
		/// <param name="propertyname">属性名称</param></param>
		/// <returns></returns>
		string GetProperty(string propertyname);

		/// <summary>
		/// 设置指定属性的值
		/// </summary>
		/// <param name="propertyname">属性名称</param>
		/// <param name="propertyvalue">属性值</param>
		/// <param name="priority">附加说明属性的优先级，比如用"important”说明属性的重要性</param>
		/// <param name="level">属性层次</param>
		void SetProperty(string propertyname,string propertyvalue,string priority,int level);

		
		/// <summary>
		/// 获取父级的CSSRuleSet
		/// </summary>
		Interface.ICSSRuleSet ParentRuleSet{get;}

		/// <summary>
		/// 获取CSS声明的内容
		/// </summary>
		string CSSText{get;set;}

		/// <summary>
		/// 获取声明中所指定的属性个数
		/// </summary>
		int NumberOfItems{get;}


	}
}
