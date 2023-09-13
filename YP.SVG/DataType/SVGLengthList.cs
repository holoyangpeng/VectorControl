using System;
using System.Text.RegularExpressions;
using System.Collections;
using YP.SVG.Interface.DataType;

namespace YP.SVG.DataType
{
	/// <summary>
	/// 定义一系列的长度度量列表
	/// </summary>
	public class SVGLengthList:DataType.SVGTypeList,Interface.DataType.ISVGLengthList
	{
		#region ..构造及消除
		public SVGLengthList(string lengthliststr,Interface.ISVGElement ownerElement,LengthDirection direction,char[] seperators)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			string[] s = SVGStringList.ParseListStr(lengthliststr,seperators);
			
			foreach(string str in s)
			{
				string temp = str.Trim();
				if(temp.Length > 0)
					AppendItem(new SVGLength(temp,ownerElement,direction));
				temp = null;
			}
			s= null;
			seperators = null;
		}

		public SVGLengthList(string lengthliststr,Interface.ISVGElement ownerElement,LengthDirection direction):this(lengthliststr,ownerElement,direction,new char[]{',',' '})
		{
		}

		public SVGLengthList(SVGLength[] svgLengthes)
		{
			this.list.AddRange(svgLengthes);
		}
		#endregion

		#region ..检测是否为有效的数据值
		/// <summary>
		/// 检测是否为有效的数据值
		/// </summary>
		/// <param name="svgType">检测的数组</param>
		/// <returns></returns>
		public override bool ValidType(Interface.DataType.ISVGType svgType)
		{
			return svgType is Interface.DataType.ISVGLength;
		}
		#endregion

		#region ..获取长度数组
		/// <summary>
		/// 获取长度数组
		/// </summary>
		/// <returns></returns>
		public SVGLength[] GetSVGLengthes()
		{
			SVGLength[] ls = new SVGLength[this.list.Count];
			this.list.CopyTo(ls);
			return ls;
		}
		#endregion

		#region ..获取对象的文本表达
		public override string ToString()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder(100);
			for(int i = 0;i<this.list.Count;i++)
				sb.Append(((SVGLength)list[i]).ValueAsString + " ");
			return sb.ToString();
		}

		#endregion
	}
}
