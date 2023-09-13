using System;

namespace YP.SVG.DataType
{
	/// <summary>
	/// SVGPointListList ��ժҪ˵����
	/// </summary>
	public class SVGPointListList:DataType.SVGTypeList
	{
		#region ..���켰����
		public SVGPointListList(string pointstr,char[] seperators)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			string[] s = DataType.SVGStringList.ParseListStr(pointstr,seperators);
			foreach(string str in s)
			{
				if(str.Trim().Length > 0)
					this.AppendItem(new SVGPointList(str));
			}
			s = null;
			seperators = null;
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
			return svgType is Interface.CTS.ISVGPointList;
		}
		#endregion
	}
}
