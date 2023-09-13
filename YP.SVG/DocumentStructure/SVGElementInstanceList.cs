using System;
using System.Xml;
using System.Collections;

using YP.SVG.Interface.DocumentStructure;

namespace YP.SVG.DocumentStructure
{
	/// <summary>
	/// ʵ�ֶ���ʵ���б�
	/// </summary>
	public class SVGElementInstanceList:Interface.DocumentStructure.ISVGElementInstanceList
	{
		#region ..���켰����
		public SVGElementInstanceList(System.Xml.XmlNodeList childnodes,Interface.DocumentStructure.ISVGUseElement useElement,Interface.DocumentStructure.ISVGElementInstance parent)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
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

		#region ..˽�б���
		Interface.DocumentStructure.ISVGElementInstance[] Items;
		#endregion

		#region ..��������
		/// <summary>
		/// ��ȡ�б���
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
		/// ��ȡ�б��ض�����
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
