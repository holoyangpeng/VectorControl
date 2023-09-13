using System;
using System.Drawing;

namespace YP.SVG.StyleContainer
{
	/// <summary>
	/// ShadowStyle ��ժҪ˵����
	/// </summary>
    public struct ShadowStyle
	{
		#region ..Constructor
		public ShadowStyle(bool drawShadow,Color shadowColor,int xOffset,int yOffset,float opacity)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.DrawShadow = drawShadow;
			this.ShadowColor = shadowColor;
			this.XOffset = xOffset;
			this.YOffset = yOffset;
			this.Opacity = opacity;
		}

		public ShadowStyle(bool drawShadow,Color shadowColor,int xOffset,int yOffset):this(drawShadow,shadowColor,xOffset,yOffset,1)
		{
		}

		public ShadowStyle(ShadowStyle style)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.DrawShadow = style.DrawShadow;
			this.ShadowColor = style.ShadowColor;
			this.XOffset = style.XOffset;
			this.YOffset = style.YOffset;
			this.Opacity = style.Opacity;
		}
		#endregion

		#region ..˽�б���
		public bool DrawShadow ;
		public Color ShadowColor;
		public int XOffset ;
		public int YOffset ;
		public float Opacity;
		#endregion

		#region ..static fields
		static ShadowStyle style = new ShadowStyle(false,Color.Black,2,2);

		public static ShadowStyle Default
		{
			get
			{
				return style;
			}
		}
		#endregion

		#region ..��������
		/// <summary>
		/// ��������Ϣ
		/// </summary>
		/// <param name="svgStyle"></param>
		public ShadowStyle MutiplyStyle(ShadowStyle newStyle)
		{
			return newStyle;
		}
		#endregion
	}
}
