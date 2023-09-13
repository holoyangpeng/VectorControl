using System;

namespace YP.Base.StyleSheets
{
	/// <summary>
	/// 定义一个StyleSheet的基类方法，一个StyleSheet提供与之相关文档进行的CSS规则操作。
	/// 在Html中，通过link导入外部Style文件活着通过Style节点可以建立一个StyleSheet。
	/// 在XML中，通过链接一个外部文件建立一个StyleSheet
	/// </summary>
	public interface IStyleSheet
	{
		/// <summary>
		/// 获取StyleSheet链接位置
		/// </summary>
		Uri Href{get;}

		/// <summary>
		/// 获取类型
		/// </summary>
		string Type{get;}	

		/// <summary>
		/// 获取标题
		/// </summary>
		string Title{get;}
	
		/// <summary>
		/// 获取与之联系的对象
		/// </summary>
		Base.Interface.IWebElement OwnerElement{get;}
	}
}
