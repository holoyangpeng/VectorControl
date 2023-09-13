using System;
using System.Drawing;

namespace YP.SVG.DataType
{
	/// <summary>
	/// ʵ��SVG�е�Color����
	/// </summary>
	public struct SVGColor:Interface.DataType.ISVGColor 
	{
		#region ..���켰����
		public SVGColor(string colorstr,YP.SVG.SVGStyleable element,string defaultvalue)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			colorstr = colorstr.Trim();
			if(colorstr.Length == 0)
				colorstr = defaultvalue;
			this.renderstr = colorstr;
			if(string.Compare(colorstr,"currentColor")==0)
			{
				colorstr = element.GetFinalAttributValue("color");
			}

			if(colorstr.Length > 0 &&string.Compare(colorstr,"none") != 0 && !colorstr.StartsWith("url("))
			{
				rgbcolor = new RGBColor(colorstr);
			}
            else if (string.Compare(colorstr,"none") != 0 && !colorstr.StartsWith("url("))
            {
                rgbcolor = new RGBColor(Color.FromArgb(0,0,0,0));
            }
            else
                rgbcolor = null;
			this.ownerStyleElement = element;
			this.defaultValue = defaultvalue;
			this.isEmpty = false;
		}
		#endregion

		#region ..˽�б���
		RGBColor rgbcolor ;
		public YP.SVG.SVGStyleable ownerStyleElement ;
		string renderstr;
		string defaultValue;
		bool isEmpty ;
		#endregion

		#region ..��̬����
		static SVGColor color = new SVGColor();
		/// <summary>
		/// ��ȡ�յ�SVGColor;
		/// </summary>
		public static SVGColor Empty
		{
			get
			{
				color.isEmpty = true;
				return color;
			}
		}
		#endregion

		#region ..��������
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
				return this.isEmpty;
			}

		}

		/// <summary>
		/// ��ȡ���͵�Ĭ��ֵ
		/// </summary>
		public string DefaultValue
		{
			get
			{
				return this.defaultValue;
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

		#region ..SetColor
		public void SetRGBColor (string rgbColor)
		{
		}

		public void SetRGBColorICCColor ( string rgbColor, string iccColor )
		{
		}

		public void SetColor( ColorType colorType, string rgbColor, string iccColor )
		{
		}
		#endregion

		#region ..��ȡ����ֵ���ı����
		/// <summary>
		/// ��ȡ����ֵ���ı����
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.renderstr;
		}
		#endregion
	}
}
