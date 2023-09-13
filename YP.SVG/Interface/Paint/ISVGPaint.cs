using System;

namespace YP.SVG.Interface.Paint
{
	/// <summary>
	/// ISVGPaint ��ժҪ˵����
	/// </summary>
	public interface ISVGPaint:DataType.ISVGColor
	{
		ulong PaintType{get;}
		string Uri{get;}

		void SetUri (string uri );
		void SetPaint (PaintType paintType, string uri, string rgbColor, string iccColor );
	}
}
