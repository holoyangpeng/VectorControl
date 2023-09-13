using System;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.ComponentModel;

namespace YP.SVG
{
        #region ..ConnectionType
        /// <summary>
        /// ָ������������
        /// </summary>
        public enum ConnectionType
        {
            /// <summary>
            /// ֱ��������
            /// </summary>
            Line,
            /// <summary>
            /// ��������
            /// </summary>
            FreeLine,
            /// <summary>
            /// ����������
            /// </summary>
            Spline,
            /// <summary>
            /// ��̬�����ߣ�����λ�ö�̬����
            /// </summary>
            Dynamic
        }
        #endregion

        #region ..AttributeChangedAction
        /// <summary>
        /// ���Ըı䵼�µĶ���
        /// </summary>
        public enum AttributeChangedResult
        {
            /// <summary>
            /// �������ֵ�����û�иı�
            /// </summary>
            NoVisualChanged = 0,
            /// <summary>
            /// �������ֵ�����û�иı䣬��λ��û�иı�
            /// </summary>
            VisualChanged = 1,
            /// <summary>
            /// λ�÷����ı�
            /// </summary>
            GraphicsPathChanged = 2,
            /// <summary>
            /// ��ά�任�����ı�
            /// </summary>
            TransformChanged = 4
        }
        #endregion

        #region ..ElementChangedAction
        /// <summary>
		/// ָ���ڵ�ı�����
		/// </summary>
		public enum ElementChangedAction
		{
			/// <summary>
			/// ���󱻲���
			/// </summary>
			Insert,
			/// <summary>
			/// ����ɾ��
			/// </summary>
			Remove,
			/// <summary>
			/// �������޸�
			/// </summary>
			Change,
			/// <summary>
			/// ��ָ������
			/// </summary>
			None
		}
		#endregion

        #region ..AngleType
        /// <summary>
		/// ָ���Ƕȵ����ͣ�
		/// </summary>
		public enum AngleType
		{
			/// <summary>
			/// ��
			/// </summary>
			SVG_ANGLETYPE_DEG = 2,
			/// <summary>
			/// δ����ĽǶ�����
			/// </summary>
			SVG_ANGLETYPE_UNKNOWN = 0,
			/// <summary>
			/// δָ���ĽǶ�����
			/// </summary>
			SVG_ANGLETYPE_UNSPECIFIED = 1,
			
			/// <summary>
			/// ����
			/// </summary>
			SVG_ANGLETYPE_RAD = 3,
			/// <summary>
			/// ���
			/// </summary>
			SVG_ANGLETYPE_GRAD = 4,
		}
		#endregion

        #region ..LengthType
        /// <summary>
		/// ָ����������
		/// </summary>
		public enum LengthType
		{
			SVG_LENGTHTYPE_PX = 5,
			SVG_LENGTHTYPE_UNKNOWN = 0,
			SVG_LENGTHTYPE_NUMBER = 1,
			SVG_LENGTHTYPE_PERCENTAGE = 2,
			SVG_LENGTHTYPE_EMS = 3,
			SVG_LENGTHTYPE_EXS = 4,
			
			SVG_LENGTHTYPE_CM = 6,
			SVG_LENGTHTYPE_MM = 7,
			SVG_LENGTHTYPE_IN = 8,
			SVG_LENGTHTYPE_PT = 9,
			SVG_LENGTHTYPE_PC = 10,
		}
		#endregion

        #region ..LengthDirection
        /// <summary>
		/// ָ�����ȷ���
		/// </summary>
		public enum LengthDirection
		{
			Hori,
			Vect,
			Viewport
		}
		#endregion

        #region ..ColorType
        /// <summary>
		/// ָ����ɫ����
		/// </summary>
		public enum ColorType
		{
			SVG_COLORTYPE_UNKNOWN = 0,
			SVG_COLORTYPE_RGBCOLOR = 1,
			SVG_COLORTYPE_RGBCOLOR_ICCCOLOR = 2,
			SVG_COLORTYPE_CURRENTCOLOR = 3,
		}
		#endregion

        #region ..TransformType
        /// <summary>
		/// ָ����ά�任����
		/// </summary>
		public enum TransformType
		{
			SVG_TRANSFORM_MATRIX    = 1,
			SVG_TRANSFORM_UNKNOWN   = 0,
			
			SVG_TRANSFORM_TRANSLATE = 2,
			SVG_TRANSFORM_SCALE     = 3,
			SVG_TRANSFORM_ROTATE    = 4,
			SVG_TRANSFORM_SKEWX     = 5,
			SVG_TRANSFORM_SKEWY     = 6,
		}
		#endregion

        #region ..SVGPreserveAspectRatioType
        /// <summary>
		/// ������ͼ��������
		/// </summary>
		public enum SVGPreserveAspectRatioType
		{
			SVG_PRESERVEASPECTRATIO_UNKNOWN = 0,
			SVG_PRESERVEASPECTRATIO_NONE = 1,
			SVG_PRESERVEASPECTRATIO_XMINYMIN = 2,
			SVG_PRESERVEASPECTRATIO_XMIDYMIN = 3,
			SVG_PRESERVEASPECTRATIO_XMAXYMIN = 4,
			SVG_PRESERVEASPECTRATIO_XMINYMID = 5,
			SVG_PRESERVEASPECTRATIO_XMIDYMID = 6,
			SVG_PRESERVEASPECTRATIO_XMAXYMID = 7,
			SVG_PRESERVEASPECTRATIO_XMINYMAX = 8,
			SVG_PRESERVEASPECTRATIO_XMIDYMAX = 9,
			SVG_PRESERVEASPECTRATIO_XMAXYMAX = 10
		}
		#endregion

        #region ..SVGMeetOrSliceType
        /// <summary>
		/// ������ͼ��������
		/// </summary>
		public enum SVGMeetOrSliceType
		{
			SVG_MEETORSLICE_UNKNOWN = 0,
			SVG_MEETORSLICE_MEET = 1,
			SVG_MEETORSLICE_SLICE = 2,
		}
		#endregion

		#region ..SVGZoomAndPanType
    /// <summary>
    /// ZoomAndPan ����
    /// </summary>
		public enum SVGZoomAndPanType
		{
			SVG_ZOOMANDPAN_UNKNOWN =0,
			SVG_ZOOMANDPAN_DISABLE =1,
			SVG_ZOOMANDPAN_MAGNIFY =2
		}
		#endregion

        #region ..PaintType
        /// <summary>
		/// ָ����������
		/// </summary>
		public enum PaintType
		{
			SVG_PAINTTYPE_URI_RGBCOLOR          = 105,
			SVG_PAINTTYPE_UNKNOWN               = 0,
			SVG_PAINTTYPE_RGBCOLOR              = 1,
			SVG_PAINTTYPE_RGBCOLOR_ICCCOLOR     = 2,
			SVG_PAINTTYPE_NONE                  = 101,
			SVG_PAINTTYPE_CURRENTCOLOR          = 102,
			SVG_PAINTTYPE_URI_NONE              = 103,
			SVG_PAINTTYPE_URI_CURRENTCOLOR      = 104,
			
			SVG_PAINTTYPE_URI_RGBCOLOR_ICCCOLOR = 106,
			SVG_PAINTTYPE_URI                   = 107,
		}
		#endregion

        #region ..SpreadMethod
        /// <summary>
		/// ָ��������չ����
		/// </summary>
		public enum SpreadMethod
		{
			SVG_SPREADMETHOD_UNKNOWN = 0,
			SVG_SPREADMETHOD_PAD     = 1,
			SVG_SPREADMETHOD_REFLECT = 2,
			SVG_SPREADMETHOD_REPEAT  = 3
		}
		#endregion

        #region ..SVGTransformType
        /// <summary>
		/// �任���
		/// </summary>
		public enum SVGTransformType
		{
			Unknown = 0,
			Matrix = 1,
			Translate = 2,
			Scale = 3,
			Rotate = 4,
			SkewX = 5,
			SkewY = 6
		}
		#endregion

        #region ..SVGUnitType
        /// <summary>
		/// �����û��ռ�����
		/// </summary>
		public enum SVGUnitType
		{
			SVG_UNIT_TYPE_UNKNOWN           = 0,
			SVG_UNIT_TYPE_USERSPACEONUSE    = 1,
			SVG_UNIT_TYPE_OBJECTBOUNDINGBOX = 2
		}
		#endregion

        #region ..SVGExceptionType
        /// <summary>
		/// �����������
		/// </summary>
		public enum SVGExceptionType
		{
			SVG_WRONG_TYPE_ERR, 
			SVG_INVALID_VALUE_ERR, 
			SVG_MATRIX_NOT_INVERTABLE, 
			SvgSHARP_UNKNOWN_ERROR
		}
		#endregion

        #region ..PathSegmentType
    /// <summary>
    /// ����<seealso cref="SVGPathSegList">SVGPathSegList</seealso>����·��������
    /// </summary>
        public enum PathSegmentType
		{
			PATHSEG_UNKNOWN                      = 0,
			PATHSEG_CLOSEPATH                    = 1,
			PATHSEG_MOVETO_ABS                   = 2,
			PATHSEG_MOVETO_REL                   = 3,
			PATHSEG_LINETO_ABS                   = 4,
			PATHSEG_LINETO_REL                   = 5,
			PATHSEG_CURVETO_CUBIC_ABS            = 6,
			PATHSEG_CURVETO_CUBIC_REL            = 7,
			PATHSEG_CURVETO_QUADRATIC_ABS        = 8,
			PATHSEG_CURVETO_QUADRATIC_REL        = 9,
			PATHSEG_ARC_ABS                      = 10,
			PATHSEG_ARC_REL                      = 11,
			PATHSEG_LINETO_HORIZONTAL_ABS        = 12,
			PATHSEG_LINETO_HORIZONTAL_REL        = 13,
			PATHSEG_LINETO_VERTICAL_ABS          = 14,
			PATHSEG_LINETO_VERTICAL_REL          = 15,
			PATHSEG_CURVETO_CUBIC_SMOOTH_ABS     = 16,
			PATHSEG_CURVETO_CUBIC_SMOOTH_REL     = 17,
			PATHSEG_CURVETO_QUADRATIC_SMOOTH_ABS = 18,
			PATHSEG_CURVETO_QUADRATIC_SMOOTH_REL = 19
		}
		#endregion

        #region ..LengthAdjustType
        /// <summary>
		/// �����ı����ȵ������
		/// </summary>
		public enum LengthAdjustType
		{
			LENGTHADJUST_UNKNOWN   = 0,
			LENGTHADJUST_SPACING     = 1,
			LENGTHADJUST_SPACINGANDGLYPHS     = 2
		}
		#endregion

		#region ..TextPath Mode Type
		/// <summary>
		/// TextPath Mode Type
		/// </summary>
		public enum TextPathMode
		{
			TEXTPATH_METHODTYPE_UNKNOWN   = 0,
			TEXTPATH_METHODTYPE_ALIGN     = 1,
			TEXTPATH_METHODTYPE_STRETCH     = 2
		}
		#endregion

		#region ..textPath Spacing Types
		/// <summary>
		/// textPath Spacing Types
		/// </summary>
		public enum TextPathSpacing
		{
			TEXTPATH_SPACINGTYPE_UNKNOWN   = 0,
			TEXTPATH_SPACINGTYPE_AUTO     = 1,
			TEXTPATH_SPACINGTYPE_EXACT     = 2
		}
		#endregion

        #region ..SVGTimeType
        /// <summary>
		/// ����ʱ�����
		/// </summary>
		public enum SVGTimeType
		{
			SVG_SVGTimeType_OffsetValue = 0,
			SVG_SVGTimeType_SyncBaseValue = 1,
			SVG_SVGTimeType_EventValue = 2,
			SVG_SVGTimeType_RepeatValue = 3,
			SVG_SVGTimeType_AccessKeyValue = 4,
			SVG_SVGTimeType_WallClock = 5,
			SVG_SVGTimeType_Indifinite = 6
		}
    #endregion

    #region ..CollectionChangeAction
    /// <summary>
    /// ָ�����ϲ���
    /// </summary>
    public enum CollectionChangeAction
		{
			/// <summary>
			/// �����������
			/// </summary>
			Insert,
			/// <summary>
			/// ����ɾ������
			/// </summary>
			Remove,
			/// <summary>
			/// �������ݷ����޸�
			/// </summary>
			Change,
			/// <summary>
			/// �޲���
			/// </summary>
			None
		}
		#endregion

		#region ..HatchStyle
		/// <summary>
		/// ָ��ͼ��������
		/// </summary>
		//[Editor(typeof(YP.Forms.Design.HatchEditor),typeof(System.Drawing.Design.UITypeEditor))]
		public enum HatchStyle
		{
			None= 0,
			BackwardDiagonal = 1,
			Cross = 2,
			DarkDownwardDiagonal = 3,
			DarkHorizontal= 4,
			DarkUpwardDiagonal= 5,
			DarkVertical= 6,
			DashedDownwardDiagonal= 7,
			DashedHorizontal= 8,
			DashedUpwardDiagonal= 9,
			DashedVertical= 10,
			DiagonalBrick= 11,
			DiagonalCross= 12,
			Divot= 13,
			DottedDiamond= 14,
			DottedGrid= 15,
			ForwardDiagonal= 16,
			Horizontal= 17,
			HorizontalBrick= 18,
			LargeCheckerBoard= 19,
			LargeConfetti= 20,
			LargeGrid= 21,
			LightDownwardDiagonal= 22,
			LightHorizontal= 23,
			LightUpwardDiagonal= 24,
			LightVertical= 25,
			NarrowHorizontal= 26,
			NarrowVertical= 27,
			OutlinedDiamond= 29,
			Percent05= 30,
			Percent10= 31,
			Percent20= 32,
			Percent25= 33,
			Percent30= 34,
			Percent40= 35,
			Percent50= 36,
			Percent60= 37,
			Percent70= 38,
			Percent75= 39,
			Percent80= 40,
			Percent90= 41,
			Plaid= 42,
			Shingle= 43,
			SmallCheckerBoard= 44,
			SmallConfetti= 45,
			SmallGrid= 46,
			SolidDiamond= 47,
			Sphere= 48,
			Trellis= 49,
			Vertical= 50,
			Wave= 51,
			Weave= 52,
			WideDownwardDiagonal= 53,
			WideUpwardDiagonal= 54,
			ZigZag= 55,
			Center= 56,
			DiagonalLeft= 57,
			DiagonalRight= 58,
			HorizontalCenter= 59,
			LeftRight= 60,
			TopBottom= 61,
			VerticalCenter= 62
		}
		#endregion

        #region ..ActionType
        /// <summary>
		/// ָ���û���˫��������ִ�еĶ�������
		/// </summary>
		public enum ActionType
		{
			/// <summary>
			/// ��ִ���κζ���
			/// </summary>
			None,
			/// <summary>
			/// ������ʾ��
			/// </summary>
			Message,
			/// <summary>
			/// ���͵����ʼ�
			/// </summary>
			Mail,
			/// <summary>
			/// ת��һ�����ӵ�ַ
			/// </summary>
			OpenHref,
			/// <summary>
			/// ��һ���ļ�
			/// </summary>
			OpenFile,
			/// <summary>
			/// ִ��һ������
			/// </summary>
			ExecuteProgram
		}
		#endregion

		#region ..ViewStyle
		/// <summary>
		/// �������Ŀ��ӻ�״̬
        /// ע�⣬��״̬ʱ��̬����ʱ�����ܳ־ã��ĵ����´򿪺�״̬���ᶪʧ
		/// </summary>
		public enum ViewStyle
		{
			None = 0,
			Lock = 1,
			Hidden = 2
		}
		#endregion

        #region ..Align
        /// <summary>
        /// ����ˮƽ�������
        /// </summary>
        public enum Alignment
        {
            /// <summary>
            /// �����
            /// </summary>
            Left,
            /// <summary>
            /// ˮƽ�е����
            /// </summary>
            Center,
            /// <summary>
            /// �Ҷ���
            /// </summary>
            Right,
        }
        #endregion

        #region ..VAlign 
        /// <summary>
        /// ���崹ֱ�������
        /// </summary>
        public enum VerticalAlignment
        {
            /// <summary>
            /// ��������
            /// </summary>
            Top,
            /// <summary>
            /// ��ֱ���ж���
            /// </summary>
            Middle,
            /// <summary>
            /// �ײ�����
            /// </summary>
            Bottom
        }
        #endregion
}
