using System;

namespace YP.SVG.DataType
{
	/// <summary>
	/// ʵ�������б���б�
	/// </summary>
	public class SVGNumberListList:DataType.SVGTypeList
	{
		#region ..���켰����
		/// <summary>
		/// ��ָ�����ַ�����ʼ���б�
		/// </summary>
		/// <param name="liststr">�����б���ַ���</param>
		/// <param name="topseperators">���ڷָ��б���ַ���</param>
		/// <param name="childseperators">���ڹ����Ӽ��б���ַ���</param>
		public SVGNumberListList(string liststr,char[] topseperators,char[] childseperators)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			string[] s = DataType.SVGStringList.ParseListStr(liststr,topseperators);
			foreach(string str in s)
			{
				string temp = str.Trim();
				if(temp.Length > 0)
					this.AppendItem(new DataType.SVGNumberList(temp,childseperators,"0"));
			}
			s = null;
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
			return svgType is Interface.DataType.ISVGNumberList;
		}
		#endregion
	}
}
