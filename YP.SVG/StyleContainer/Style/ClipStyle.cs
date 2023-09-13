using System;

using YP.SVG.StyleContainer;

namespace YP.SVG.StyleContainer
{
	/// <summary>
	/// ClipStyle ��ժҪ˵����
	/// </summary>
    public struct ClipStyle
	{
		#region ..���켰����
//		public ClipStyle()
//		{
//			//
//			// TODO: �ڴ˴���ӹ��캯���߼�
//			//
//			this.clip_path = new DataType.SVGString(string.Empty);
//			this.clip_rule = new DataType.SVGString("nonzero");
//			this.mask = new DataType.SVGString(string.Empty);
//			this.opacity = new DataType.SVGNumber(1);
//		}

		public ClipStyle(ClipStyle style)
		{
			this.clip_path = style.clip_path;
			this.clip_rule = style.clip_rule;
			this.mask = style.mask;
			this.opacity = style.opacity;
		}

		ClipStyle(
			YP.SVG.DataType.SVGString clip_path,
			YP.SVG.DataType.SVGString clip_rule,
			YP.SVG.DataType.SVGString mask,
			YP.SVG.DataType.SVGNumber opacity
			)
		{
			this.clip_path = clip_path;
			this.mask = mask;
			this.clip_rule = clip_rule;
			this.opacity = opacity;
		}
		#endregion

		#region ..��̬����
		static ClipStyle style = new ClipStyle(
			new DataType.SVGString(string.Empty,string.Empty),
			new DataType.SVGString("nonzero","nonzero"),
			new DataType.SVGString(string.Empty),
			new DataType.SVGNumber(1));

		static ClipStyle empty = new ClipStyle(
			DataType.SVGString.Empty,
			DataType.SVGString.Empty,
			DataType.SVGString.Empty,
			DataType.SVGNumber.Empty);

		/// <summary>
		/// ��ȡĬ�϶���
		/// </summary>
		public static ClipStyle Default
		{
			get
			{
				return style;
			}
		}

		/// <summary>
		/// ��ȡ�ն���
		/// </summary>
		public static ClipStyle Empty
		{
			get
			{
				return empty;
			}
		}
		#endregion

		#region ..�����ֶ�
		public YP.SVG.DataType.SVGString clip_path;
		public YP.SVG.DataType.SVGString mask ;
		public YP.SVG.DataType.SVGString clip_rule ;
		public YP.SVG.DataType.SVGNumber opacity;
		#endregion

		#region ..��������
		/// <summary>
		/// ��������Ϣ
		/// </summary>
		/// <param name="svgStyle"></param>
		public ClipStyle MutiplyStyle(ClipStyle newStyle)
		{
//			if(newStyle.
//				return this;
			if(this.clip_path.IsEmpty)
				this.clip_path = (DataType.SVGString)newStyle.clip_path;
			else
				this.clip_path = TypeMultiply.Multiply(this.clip_path,newStyle.clip_path);
			if(this.clip_rule.IsEmpty)
				this.clip_rule = (DataType.SVGString)newStyle.clip_rule;
			else
				this.clip_rule = TypeMultiply.Multiply(this.clip_rule,newStyle.clip_rule);
			if(this.mask.IsEmpty)
				this.mask = (DataType.SVGString)newStyle.mask;
			else
				this.mask = TypeMultiply.Multiply(this.mask,newStyle.mask);
			if(this.opacity.IsEmpty)
				this.opacity = (DataType.SVGNumber)newStyle.opacity;
			else
				this.opacity = TypeMultiply.Multiply((DataType.SVGNumber)this.opacity,newStyle.opacity);
			return this;
		}
		#endregion

		#region ..��¡
//		public ClipStyle Clone()
//		{
//			ClipStyle fillstyle = new ClipStyle();
//			if(this.mask != null)
//				fillstyle.mask = new DataType.SVGString(this.mask.Value);
//			if(this.clip_rule != null)
//				fillstyle.clip_rule = new DataType.SVGString(this.clip_rule.Value);
//			if(this.clip_path != null)
//				fillstyle.clip_path = new DataType.SVGString(this.clip_path.Value);
//			if(this.opacity != null)
//				fillstyle.opacity = new DataType.SVGNumber(this.opacity.Value);
//			return fillstyle;
//		}
		#endregion
	}
}
