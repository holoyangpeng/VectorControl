using System;

namespace YP.SVG.Interface.Paint
{
	/// <summary>
	/// ISVGPaint 的摘要说明。
	/// </summary>
	public interface ISVGPaint:DataType.ISVGColor
	{
		ulong PaintType{get;}
		string Uri{get;}

		void SetUri (string uri );
		void SetPaint (PaintType paintType, string uri, string rgbColor, string iccColor );
	}
}
