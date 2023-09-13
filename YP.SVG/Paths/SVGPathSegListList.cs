using System;

namespace YP.SVG.Paths
{
	/// <summary>
	/// SVGPathSegListList ��ժҪ˵����
	/// </summary>
	public class SVGPathSegListList:DataType.SVGTypeList
	{
		#region ..���켰����
		public SVGPathSegListList(string pathstr,char[] seperators)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			string[] s = DataType.SVGStringList.ParseListStr(pathstr,seperators);
			foreach(string str in s)
			{
				if(str.Trim().Length > 0)
					this.AppendItem(new SVGPathSegList(str));
			}
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
			return svgType is Interface.Paths.ISVGPathSegList;
		}
		#endregion
	}
}
