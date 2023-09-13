using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using YP.SVG.DataType;

using System.Text.RegularExpressions;

namespace YP.SVG.Paint
{
	/// <summary>
	/// ʵ�ֻ�ˢͨ�����Ժͷ���
	/// </summary>
	public struct SVGPaint:Interface.Paint.ISVGPaint
	{
		#region ..���켰����
		static SVGPaint()
		{
			paint = new SVGPaint();
		}

		public SVGPaint(string str,YP.SVG.SVGStyleable element,string defaultvalue)
		{
			this.uri = string.Empty;
			this.defaultvalue = defaultvalue;
			rgbcolor = null;
			this.renderstr = str;
			this.state = 0;
			this.ownerStyleElement = element;
			if(str == null)
			{
				paintType = (ulong)SVG.PaintType.SVG_PAINTTYPE_NONE;
				return;
			}
			paintType = (ulong)SVG.PaintType.SVG_PAINTTYPE_URI_RGBCOLOR;

			if(string.Compare(str,"currentColor")==0)
			{
				paintType = (ulong)SVG.PaintType.SVG_PAINTTYPE_CURRENTCOLOR;
			}
			else if(string.Compare(str ,"none") == 0)
			{
				paintType = (ulong)SVG.PaintType.SVG_PAINTTYPE_NONE;
			}
			else if(str.StartsWith("url("))
			{
				paintType = (ulong)SVG.PaintType.SVG_PAINTTYPE_URI;
				int index = str.IndexOf(")");
				if(index>=0)
				{
					uri = str.Substring(4, str.IndexOf(")")-4);
				}
				// TODO: check for fallbacks
			}
			else
			{
				// TODO: create a regex to validate this
				paintType = (ulong)SVG.PaintType.SVG_PAINTTYPE_RGBCOLOR;
				if(str != null)
				{
					str = str.Trim();
					if(str.Length == 0)
						str = defaultvalue;
					if(string.Compare(str,"currentColor") == 0 && element != null)
						str = element.GetFinalAttributValue("color");

					if(str.Length > 0 && string.Compare(str,"none")!= 0 && !str.StartsWith("url("))
						rgbcolor = new RGBColor(str);
				}	
			}
			this.state = 1;
		}
		#endregion

		#region ..˽�б���
		ulong paintType;
		string uri;
		string defaultvalue;
		int state;
		RGBColor rgbcolor ;
		public YP.SVG.SVGStyleable ownerStyleElement ;
		string renderstr;
		#endregion

		#region ..��̬����
		static SVGPaint paint ;

		/// <summary>;
		/// �ж϶����Ƿ�Ϊ��
		/// </summary>
		public static SVGPaint Empty
		{
			get
			{
				return paint;
			}
		}
		#endregion

		#region ..ISVGColor����
		/// <summary>
		/// ��ȡ��ʾ������Ϣ���ַ���
		/// </summary>
		public string RenderStr
		{
			get
			{
				return this.renderstr;
			}
		}

		/// <summary>
		/// ��ȡ��ɫ����
		/// </summary>
		public ColorType ColorType
		{
			get
			{
				return ColorType.SVG_COLORTYPE_RGBCOLOR;
			}
		}

		/// <summary>
		/// �ж������Ƿ�Ϊ��
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return this.state == 0;
			}

		}

		/// <summary>
		/// ��ȡ���͵�Ĭ��ֵ
		/// </summary>
		public string DefaultValue
		{
			get
			{
				return this.defaultvalue;
			}
		}

		/// <summary>
		/// ��ȡ��RGBColorɫ�ʿռ����ɫֵ
		/// </summary>
		public Interface.DataType.IRgbColor RgbColor
		{
			get
			{
				return this.rgbcolor;
			}
		}

		/// <summary>
		/// ��ȡICCColor��ɫ�ռ��ڵ���ɫֵ
		/// </summary>
		public Interface.DataType.ISVGIccColor IccColor
		{
			get
			{
				return null;
			}
		}
		#endregion

		#region ..��������
		/// <summary>
		/// ��ȡ�������
		/// </summary>
		public ulong PaintType
		{
			get
			{
				return this.paintType;
			}
		}

		public string Uri
		{
			get
			{
				return this.uri;
			}
		}
		#endregion

		#region ..��������
		public void SetUri (string uri)
		{
			this.uri = uri;
		}

		public void SetPaint (PaintType paintType, string uri, string rgbColor, string iccColor )
		{
			throw new NotImplementedException();
		}
		#endregion

		#region ..��ȡ������ı����
		public override string ToString()
		{
			return this.renderstr;
		}

		#endregion
	}
}
