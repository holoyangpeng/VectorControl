using System;

namespace YP.SVG.StyleContainer
{
	/// <summary>
	/// �������Դ洢�������������ơ�͸���ȡ��ɼ��Եȡ�
	/// </summary>
	public class StyleContainer
    {
		#region ..���켰����
		public StyleContainer():this(false)
		{
			
		}

		/// <summary>
		/// ��������
		/// </summary>
		/// <param name="createDefault">�Ƿ񴴽�Ĭ��ֵ</param>
		public StyleContainer(bool createDefault)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			if(createDefault)
			{
				fillStyle = FillStyle.Default;
				strokeStyle = StrokeStyle.Default;
				visualMediaStyle = VisualMediaStyle.Default;
				fontStyle = FontStyle.Default;
				textStyle = TextStyle.Default;
				clipStyle = ClipStyle.Default;
				colorAndPaintStyle = ColorAndPaintStyle.Default;
				this.shadowStyle = ShadowStyle.Default;
			}
		}

		public StyleContainer(StyleContainer container)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
            if (container != null)
            {
                fillStyle = new FillStyle(container.fillStyle);
                strokeStyle = new StrokeStyle(container.strokeStyle);
                visualMediaStyle = new VisualMediaStyle(container.visualMediaStyle);
                fontStyle = new FontStyle(container.fontStyle);
                textStyle = new TextStyle(container.textStyle);
                clipStyle = new ClipStyle(container.clipStyle);
                this.shadowStyle = new ShadowStyle(container.shadowStyle);
                colorAndPaintStyle = new ColorAndPaintStyle(container.colorAndPaintStyle);
                this.boundView = container.boundView;
                this.viewVisible = container.viewVisible;
            }
		}
		#endregion

		#region ..˽�б���
		FillStyle fillStyle;
		StrokeStyle strokeStyle;
		VisualMediaStyle visualMediaStyle ;
		FontStyle fontStyle;
		TextStyle textStyle ;
		ClipStyle clipStyle;
		ShadowStyle shadowStyle;
		ColorAndPaintStyle colorAndPaintStyle ;
		bool boundView = false,viewVisible = true;
		#endregion
	
		#region ..��������
		/// <summary>
		/// ��ȡ�����Ϣ
		/// </summary>
		public YP.SVG.StyleContainer.FillStyle FillStyle
		{
			get
			{
				return this.fillStyle;
			}
			set
			{
				this.fillStyle = value;
			}
		}

		/// <summary>
		/// �жϵ�ǰ����״̬�Ƿ�Ϊ��������
		/// </summary>
		public bool BoundView
		{
			get
			{
				return this.boundView;
			}
			set
			{
				this.boundView = value;
			}
		}

		/// <summary>
		/// ��ǰ�Ƿ��������
		/// </summary>
		public bool ViewVisible
		{
			get
			{
				return this.viewVisible;
			}
			set
			{
				this.viewVisible = value;
			}
		}

		/// <summary>
		/// ��ȡ���������Ϣ
		/// </summary>
		public YP.SVG.StyleContainer.StrokeStyle StrokeStyle
		{
			get
			{
				return this.strokeStyle;
			}
			set
			{
				this.strokeStyle = value;
			}
		}

		/// <summary>
		/// ��ȡ��ͨ��Ϣ
		/// </summary>
		public YP.SVG.StyleContainer.VisualMediaStyle VisualMediaStyle
		{
			get
			{
				return this.visualMediaStyle;
			}
			set
			{
				this.visualMediaStyle = value;
			}
		}

		/// <summary>
		/// ��ȡ��ɫ�ͻ�����Ϣ��Ϣ
		/// </summary>
		public YP.SVG.StyleContainer.ColorAndPaintStyle ColorAndPaintStyle
		{
			get
			{
				return this.colorAndPaintStyle;
			}
			set
			{
				this.colorAndPaintStyle = value;
			}
		}

		/// <summary>
		/// ��ȡ�ı���Ϣ
		/// </summary>
		public YP.SVG.StyleContainer.TextStyle  TextStyle
		{
			get
			{
				return this.textStyle;
			}
			set
			{
				this.textStyle = value;
			}
		}

		/// <summary>
		/// ��ȡ������Ϣ
		/// </summary>
		public YP.SVG.StyleContainer.FontStyle FontStyle
		{
			get
			{
				return this.fontStyle;
			}
			set
			{
				this.fontStyle = value;
			}
		}

		/// <summary>
		/// ��ȡShadow��Ϣ
		/// </summary>
		public YP.SVG.StyleContainer.ShadowStyle ShadowStyle
		{
			get
			{
				return this.shadowStyle;
			}
			set
			{
				this.shadowStyle = value;
			}
		}

		/// <summary>
		/// ��ȡ������Ϣ
		/// </summary>
		public YP.SVG.StyleContainer.ClipStyle ClipStyle
		{
			get
			{
				return this.clipStyle;
			}
			set
			{
				this.clipStyle = value;
			}
		}
		#endregion

		#region ..��¡
		public StyleContainer Clone()
		{
			StyleContainer s = new StyleContainer();
			s.fillStyle = this.fillStyle;
			s.strokeStyle = this.strokeStyle;
			s.visualMediaStyle = this.visualMediaStyle;
			s.clipStyle = this.clipStyle;
			s.fontStyle = this.fontStyle;
			s.textStyle = this.textStyle;
			s.colorAndPaintStyle = this.colorAndPaintStyle;
			return s;
		}
		#endregion
	}
}
