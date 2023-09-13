using System;

namespace YP.SVG.DataType
{
	/// <summary>
	/// ʵ�ֱ任�б���б�
	/// </summary>
	public class SVGTransformListList:DataType.SVGTypeList
	{
		#region ..���켰����
		public SVGTransformListList(string transformstr,string type,char[] seperators)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			string[] s = DataType.SVGStringList.ParseListStr(transformstr,seperators);
			foreach(string str in s)
			{
				if(str.Length > 0)
					this.AppendItem(new SVGTransformList(type + "(" + str + ")"));
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
			return svgType is Interface.CTS.ISVGTransformList;
		}
		#endregion
	}
}
