using System;
using System.Text.RegularExpressions;
using System.Collections;
using YP.SVG.Interface.DataType;

namespace YP.SVG.DataType
{
	/// <summary>
	/// ����һϵ�еĳ��ȶ����б�
	/// </summary>
	public class SVGLengthList:DataType.SVGTypeList,Interface.DataType.ISVGLengthList
	{
		#region ..���켰����
		public SVGLengthList(string lengthliststr,Interface.ISVGElement ownerElement,LengthDirection direction,char[] seperators)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
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

		#region ..����Ƿ�Ϊ��Ч������ֵ
		/// <summary>
		/// ����Ƿ�Ϊ��Ч������ֵ
		/// </summary>
		/// <param name="svgType">��������</param>
		/// <returns></returns>
		public override bool ValidType(Interface.DataType.ISVGType svgType)
		{
			return svgType is Interface.DataType.ISVGLength;
		}
		#endregion

		#region ..��ȡ��������
		/// <summary>
		/// ��ȡ��������
		/// </summary>
		/// <returns></returns>
		public SVGLength[] GetSVGLengthes()
		{
			SVGLength[] ls = new SVGLength[this.list.Count];
			this.list.CopyTo(ls);
			return ls;
		}
		#endregion

		#region ..��ȡ������ı����
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
