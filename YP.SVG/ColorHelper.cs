using System;
using System.Drawing;

namespace YP.SVG
{
	/// <summary>
	/// ColorHelper 的摘要说明。
	/// </summary>
	public class ColorHelper
	{
		public ColorHelper()
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
		}

		public static string GetColorStringInHex(Color color)
		{
			if(color.IsEmpty || color == Color.Transparent)
				return "none";
			int r = color.R;
			int g = color.G;
			int b = color.B;
			string str = "#";
			string rs = r.ToString("X");
			if(rs.Length < 2)
				rs = "0" + rs;
			str += rs;
			rs = g.ToString("X");
			if(rs.Length < 2)
				rs = "0" + rs;
			str += rs;
			rs = b.ToString("X");
			if(rs.Length < 2)
				rs = "0" + rs;
			str += rs;
			return str;
		}
	}
}
