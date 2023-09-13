using System;
using System.Collections;
using System.Text.RegularExpressions;

using YP.SVG.Interface.DataType;

namespace YP.SVG.DataType
{
	/// <summary>
	/// SVGNumberList ��ժҪ˵����
	/// </summary>
	public class SVGNumberList:DataType.SVGTypeList,Interface.DataType.ISVGNumberList
	{
		#region ..���켰����
		public SVGNumberList(string numberliststr,char[] seperator,string defaultValue)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			string[] s = SVGStringList.ParseListStr(numberliststr,seperator);
			
			foreach(string str in s)
			{
				string temp = str.Trim();
				if(temp.Length > 0)
					AppendItem(new SVGNumber(temp,defaultValue));
				temp = null;
			}
			s = null;
			seperator = null;
		}

		public SVGNumberList(SVGNumber[] numbers)
		{
			this.list.AddRange(numbers);
		}
		#endregion

		#region ..˽�б���
		#endregion

		#region ..����Ƿ�Ϊ��Ч������ֵ
		/// <summary>
		/// ����Ƿ�Ϊ��Ч������ֵ
		/// </summary>
		/// <param name="svgType">��������</param>
		/// <returns></returns>
		public override bool ValidType(Interface.DataType.ISVGType svgType)
		{
			return svgType is Interface.DataType.ISVGNumber;
		}
		#endregion

		#region ..��ȡ��������
		/// <summary>
		/// ��ȡ��������
		/// </summary>
		/// <returns></returns>
		public SVGNumber[] GetSVGNumbers()
		{
			SVGNumber[] ns = new SVGNumber[this.list.Count];
			this.list.CopyTo(ns);
			return ns;
		}
		#endregion

		#region ..��ȡ������ı����
		/// <summary>
		/// ��ȡ������ı����
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder(100);
			for(int i = 0;i<this.list.Count;i++)
				sb.Append(((SVGNumber)list[i]).OriValueStr + " ");
			return sb.ToString();
		}
		#endregion
	}
}
