using System;

namespace YP.SVG.Interface
{
	/// <summary>
	/// 实现SVGElement的一般行为
	/// </summary>
	public interface ISVGElement 
	{
		/// <summary>
		/// 获取节点所属的文档对象
		/// </summary>
		Document.SVGDocument OwnerDocument{get;}

		/// <summary>
		/// 获取建立当前视图的视图对象，一般为最近的一个SVG对象
		/// </summary>
		Interface.ISVGElement ViewPortElement{get;}

		/// <summary>
		/// 获取最近一层的SVG对象
		/// </summary>
		Interface.DocumentStructure.ISVGSVGElement OwnerSvgElement{get;}

		/// <summary>
		/// 获取节点的ID名称
		/// </summary>
		string ID{get;}

		/// <summary>
		/// 判断节点是否可以绘制
		/// </summary>
		//bool CanRender{get;}

		/// <summary>
		/// 当属性发生修改时或添加时，更新对象属性
		/// </summary>
		/// <param name="attributeName">属性名称</param>
		/// <param name="attributeValue">属性值</param>
		//void AddSVGAttribute(string attributeName,string attributeValue);
	}
}
