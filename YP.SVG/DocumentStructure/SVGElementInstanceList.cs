using System;
using System.Xml;
using System.Collections;

using YP.SVG.Interface.DocumentStructure;

namespace YP.SVG.DocumentStructure
{
	/// <summary>
	/// 实现对象实例列表
	/// </summary>
	public class SVGElementInstanceList:Interface.DocumentStructure.ISVGElementInstanceList
	{
		#region ..构造及消除
		public SVGElementInstanceList(System.Xml.XmlNodeList childnodes,Interface.DocumentStructure.ISVGUseElement useElement,Interface.DocumentStructure.ISVGElementInstance parent)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			ArrayList list = new ArrayList(16);
			foreach(XmlNode xmlChild in childnodes)
			{
				if(xmlChild is Interface.ISVGElement)
				{
					list.Add(new SVGElementInstance((Interface.ISVGElement)xmlChild, useElement, parent));
				}
			}
			this.Items = new Interface.DocumentStructure.ISVGElementInstance[list.Count];
			list.CopyTo(this.Items);
		}
		#endregion

		#region ..私有变量
		Interface.DocumentStructure.ISVGElementInstance[] Items;
		#endregion

		#region ..公共属性
		/// <summary>
		/// 获取列表长度
		/// </summary>
		public ulong Length
		{
			get
			{
				if(this.Items != null)
					return (ulong)this.Items.Length;
				return 0;
			}
		}

		/// <summary>
		/// 获取列表特定的项
		/// </summary>
		public ISVGElementInstance Item ( ulong index )
		{
			if(this.Items != null)
			{
				if(index >= 0 && (int)index < this.Items.Length)
					return this.Items[index];
			}
			return null;
		}
		#endregion
	}
}
