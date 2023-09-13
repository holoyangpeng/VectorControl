using System;
using System.Drawing;

namespace YP.SVG.StyleContainer
{
	/// <summary>
	/// ShadowStyle 的摘要说明。
	/// </summary>
    public struct ShadowStyle
	{
		#region ..Constructor
		public ShadowStyle(bool drawShadow,Color shadowColor,int xOffset,int yOffset,float opacity)
		{
			//
			// TODO: 在此处添加构造函数逻辑
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
			// TODO: 在此处添加构造函数逻辑
			//
			this.DrawShadow = style.DrawShadow;
			this.ShadowColor = style.ShadowColor;
			this.XOffset = style.XOffset;
			this.YOffset = style.YOffset;
			this.Opacity = style.Opacity;
		}
		#endregion

		#region ..私有变量
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

		#region ..公共方法
		/// <summary>
		/// 添加填充信息
		/// </summary>
		/// <param name="svgStyle"></param>
		public ShadowStyle MutiplyStyle(ShadowStyle newStyle)
		{
			return newStyle;
		}
		#endregion
	}
}
