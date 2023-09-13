using System;
using System.Linq;
using YP.SVG;

namespace YP.VectorControl
{
    #region ..GridType
    /// <summary>
    /// ָ�����������
    /// </summary>
    public enum GridType
    {
        /// <summary>
        /// ��״
        /// </summary>
        Line,
        /// <summary>
        /// ��״
        /// </summary>
        Dot
    }
    #endregion

    #region ..GuideResult
    /// <summary>
    /// �ο��߶�����
    /// </summary>
    internal enum GuideResult
    {
        None = 0,
        /// <summary>
        /// X����
        /// </summary>
        X,
        /// <summary>
        /// Y����
        /// </summary>
        Y
    }
    #endregion

    #region ..Operator
    /// <summary>
	/// ���������ʽ
	/// </summary>
	public enum Operator
	{
		None,
		/// <summary>
		/// ���ƾ���
		/// </summary>
		Rectangle,
		/// <summary>
		/// ������Բ��Բ(��סShift��)
		/// </summary>
		Ellipse,
		/// <summary>
		/// ��������
		/// </summary>
		Polyline ,
		/// <summary>
		/// ���ƶ����
		/// </summary>
		Polygon ,
		/// <summary>
		/// ����ֱ��
		/// </summary>
		Line,
		/// <summary>
		/// �����ı�
		/// </summary>
		Text ,
		/// <summary>
		/// ������������
		/// </summary>
		Path,
		/// <summary>
		/// ����ͼƬ
		/// </summary>
		Image,
		/// <summary>
		/// ����������״
		/// </summary>
		Star,
		/// <summary>
		/// ��ά�任����
		/// </summary>
		Transform,
		/// <summary>
		/// �ڵ�༭����
		/// </summary>
		NodeEdit ,
		/// <summary>
		/// �Ŵ���ͼ����
		/// </summary>
		ZoomIn ,
		/// <summary>
		/// ��С��ͼ����
		/// </summary>
		ZoomOut ,
		/// <summary>
		/// ����Ԥ������״
		/// </summary>
		Shape,
		/// <summary>
		/// ���β���
		/// </summary>
		Roam,
		/// <summary>
		/// ���̲���
		/// </summary>
		Pie,
		/// <summary>
		/// ��Բ������
		/// </summary>
		Arc,
		/// <summary>
		/// ����ı�
		/// </summary>
		TextBlock,
		/// <summary>
		/// ������
		/// </summary>
		Connection
	}
	#endregion

    #region ..Level
    /// <summary>
	/// ָ����β������
	/// </summary>
	public enum ElementLayer
	{
		/// <summary>
		/// ���ڶ���
		/// </summary>
		Top,
		/// <summary>
		/// ���ڵײ�
		/// </summary>
		Bottom,
		/// <summary>
		/// ����һ��
		/// </summary>
		Up,
		/// <summary>
		/// ����һ��
		/// </summary>
		Down
	}
	#endregion

    #region ..VisualAlignment
    /// <summary>
    /// ָ���ڱ༭�����У���ͼ��任����ʱ��������������
    /// </summary>
    public enum VisualAlignment
    {
        /// <summary>
        /// ���κζ���
        /// </summary>
        None = 0,
        /// <summary>
        /// ���뵽����
        /// </summary>
        Grid = 1,
        /// <summary>
        /// ���뵽�ο���
        /// </summary>
        Guide = 2,
        /// <summary>
        /// ���뵽��״
        /// </summary>
        Element = 4,
        /// <summary>
        /// ���ӵ����
        /// </summary>
        //ConnectablePoint = 8,
        /// <summary>
        /// ȫ������
        /// </summary>
        All = 7
    }
    #endregion

    #region ..ָ����������
    /// <summary>
	/// �����������
	/// </summary>
	public enum AlignElementsType
	{
		/// <summary>
		/// �������� 
		/// </summary>
		Top,
		/// <summary>
		/// �ײ�����
		/// </summary>
		Bottom,
		/// <summary>
		/// �����
		/// </summary>
		Left,
		/// <summary>
		/// �Ҷ���
		/// </summary>
		Right,
		/// <summary>
		/// ��ֱ���ĵ����
		/// </summary>
		VerticalCenter,
		/// <summary>
		/// ˮƽ���ĵ����
		/// </summary>
		HorizontalCenter
	}
	#endregion

	#region ..ָ���ֲ������
	/// <summary>
	/// ָ���ֲ������
	/// </summary>
	public enum DistributeType
	{
		/// <summary>
		///  ���ϱ�ԵΪ�ο�ִ�зֲ�
		/// </summary>
		Top,
		/// <summary>
		/// ���±�ԵΪ�ο�ִ�зֲ�
		/// </summary>
		Bottom,
		/// <summary>
		/// �����ԵΪ�ο�ִ�зֲ�
		/// </summary>
		Left,
		/// <summary>
		/// ���ұ�ԵΪ�ο�ִ�зֲ�
		/// </summary>
		Right,
		/// <summary>
		/// �Դ�ֱ���ĵ�Ϊ�ο�ִ�зֲ�
		/// </summary>
		VerticalCenter,
		/// <summary>
		/// ��ˮƽ���ĵ�Ϊ�ο�ִ�зֲ�
		/// </summary>
		HorizontalCenter
	}
	#endregion

	#region ..ָ��������λ
	/// <summary>
	/// ָ��������λ
	/// </summary>
	internal enum UnitType
	{
		/// <summary>
		/// ����
		/// </summary>
		Pixel,
		/// <summary>
		/// ��
		/// </summary>
		Point,
		/// <summary>
		/// Ӣ��
		/// </summary>
		Inch,
		/// <summary>
		/// ����
		/// </summary>
		Centimeter ,
		/// <summary>
		/// ����
		/// </summary>
		Millimetre
	}
	#endregion

	#region ..ָ�������ߴ�����
	/// <summary>
	/// ָ�������ߴ�����
	/// </summary>
	public enum ElementSizeAdjustment
	{
		/// <summary>
		/// �����߶�
		/// </summary>
		Height,
		/// <summary>
		/// �������
		/// </summary>
		Width,
		/// <summary>
		/// �߶ȺͿ��ͬʱ����
		/// </summary>
		All
	}
	#endregion

	#region ..Behavior
	/// <summary>
	/// ָ����ǰ���Խ��е���Ϊ
	/// </summary>
	public enum Behavior
	{
		/// <summary>
		/// ����Ϊ
		/// </summary>
		None,
		/// <summary>
		/// ����
		/// </summary>
		Undo,
		/// <summary>
		/// ����
		/// </summary>
		Redo,
		/// <summary>
		/// ����
		/// </summary>
		Group,
		/// <summary>
		/// ����
		/// </summary>
		UnGroup,
		/// <summary>
		/// ����
		/// </summary>
		AlignElements,
		/// <summary>
		/// �ֲ�
		/// </summary>
		Distriute,
		/// <summary>
		/// ����
		/// </summary>
		Copy,
		/// <summary>
		/// ����
		/// </summary>
		Cut,
		/// <summary>
		/// ճ��
		/// </summary>
		Paste,
		/// <summary>
		/// �������
		/// </summary>
		AdjustLayer,
		/// <summary>
		/// ���ѡȡ
		/// </summary>
		SelectNone,
		/// <summary>
		/// ����ѡȡ����ߴ�
		/// </summary>
		AdjustSize,
		/// <summary>
		/// ȫѡ
		/// </summary>
		SelectAll,
		/// <summary>
		/// ɾ��
		/// </summary>
		Delete,
		/// <summary>
		/// �任
		/// </summary>
		Transform
	}
	#endregion

	#region ..��ά�任����
	/// <summary>
	/// ָ���ؼ�������еĶ�ά�任����
	/// </summary>
	public enum TransformBehavior
	{
		/// <summary>
		/// ָ���ؼ�����������κζ�ά�任����
		/// </summary>
		None = 0,
		/// <summary>
		/// ָ���ؼ����Խ���ƽ�Ʋ���
		/// </summary>
		Translate = 1,
		/// <summary>
		/// ָ���ؼ����Խ������Ų���
		/// </summary>
		Scale = 2,
		/// <summary>
		/// ָ���ؼ����Խ�����ת����
		/// </summary>
		Rotate = 4,
		/// <summary>
		/// ָ���ؼ����Խ���Ť������
		/// </summary>
		Skew = 0x20,
		/// <summary>
		/// ָ���������ѡȡ
		/// </summary>
		Select = 8,
		/// <summary>
		/// ָ���ؼ����Խ���ȫ����ά�任����
		/// </summary>
		All = 47
	}
	#endregion

	#region ..�������������
	/// <summary>
	/// ָ������ı�����𣬵�����ָ���ı����󣬽����ܶԶ���ִ��ָ������
	/// </summary>
	public enum ProtectType
	{
		/// <summary>
		/// ���κα���
		/// </summary>
		None = 0,
		/*
		/// <summary>
		/// ����ƽ�ƶ���
		/// </summary>
		X = 1,
		/// <summary>
		/// ����Ť������
		/// </summary>
		Y = 2,
		/// <summary>
		/// ���ܸı���
		/// </summary>
		Width = 4,
		/// <summary>
		/// ���ܸı�߶�
		/// </summary>
		Height = 0x20,
		/// <summary>
		/// ������ת����
		/// </summary>
		Rotate = 4,
		/// <summary>
		/// ���ܽ��д�ֱ�������б
		/// </summary>
		SkewX = 0x2000,
		/// <summary>
		/// ���ܽ���ˮƽ�������б
		/// </summary>
		SkewY = 0x800,
		*/
		/// <summary>
		/// �����ݺ��
		/// </summary>
//			PerserveAspectRatio = 0x400,
		/// <summary>
		/// �����޸Ķ����ê��
		/// </summary>
		NodeEdit = 0x800,
		/// <summary>
		/// ����ɾ��
		/// </summary>
		Delete = 0x4000
		
	}
	#endregion

	#region ..MouseClickType
	/// <summary>
	/// ������ElementClick�¼���click������
	/// </summary>
	public enum MouseClickType
	{
		/// <summary>
		/// ��굥���¼�
		/// </summary>
		SingleClick,

		/// <summary>
		/// ���˫���¼�
		/// </summary>
		DoubleClick
	}
	#endregion

	#region ..SymbolAppendMode
	/// <summary>
	/// ����Ԥ����ͼԪ���õ�ģʽ
	/// </summary>
	public enum SymbolAppendMode
	{
		/// <summary>
		/// ͨ��Use��������
		/// </summary>
		UseRef,
		/// <summary>
		/// ֱ�����Symbol����
		/// </summary>
		DirectAppend
	}
	#endregion

	#region ..DisabledFeatures
	/// <summary>
	/// gets or sets the features which will be disabled
	/// </summary>
	internal enum DisabledFeatures
	{
		None = 0,
		/// <summary>
		/// Thumb view
		/// </summary>
		ThumbView = 1,
		/// <summary>
		/// disable shadow
		/// </summary>
		Shadow = 2
	}
	#endregion

	#region ..property category
	/// <summary>
	/// ���������Կ�����ʾ���������
	/// </summary>
	public enum PropertyCategory
	{
        /// <summary>
        /// ��
        /// </summary>
		None = 0,
        /// <summary>
        /// ͨ������
        /// </summary>
		Common = 1,
        /// <summary>
        /// �������
        /// </summary>
		Fill = 2,
        /// <summary>
        /// ��������
        /// </summary>
		Stroke=4,
        /// <summary>
        /// ��Ӱ����
        /// </summary>
		Shadow = 8,
        /// <summary>
        /// �ı�������
        /// </summary>
		TextBlock = 16,
        /// <summary>
        /// ��������
        /// </summary>
		Font = 32,
        /// <summary>
        /// ��ͷ����
        /// </summary>
		Arrow = 64,
        /// <summary>
        /// ��ʾ�������
        /// </summary>
		All = 127
	}
	#endregion

	#region ..ConnectTargetType
	/// <summary>
	/// ����������ʱ����������ͼԪ������
	/// </summary>
	public enum ConnectionTargetType
	{
		None = 0,
		/// <summary>
		/// ��ͼ���ӵ���ʼͼԪ
		/// </summary>
		StartElement = 1,
		/// <summary>
		/// ��ͼ����������ͼԪ
		/// </summary>
		EndElement = 2,
        /// <summary>
        /// ��ͼ������֧
        /// </summary>
        Branch = 4,
        /// <summary>
        /// ȫ������
        /// </summary>
        All = 7
	}
	#endregion

    #region ..EnumHelper
    /// <summary>
    /// �����࣬�������ж�Enum
    /// </summary>
    internal static class EnumHelper
    {
        /// <summary>
        /// ��ȡAlignment��SVG����ַ���
        /// </summary>
        /// <param name="alignment"></param>
        /// <returns></returns>
        public static string GetAlignmentSVGString(Alignment alignment)
        {
            string result = "start";
            if (alignment == SVG.Alignment.Center)
                result = "center";
            else if (alignment == SVG.Alignment.Right)
                result = "end";
            return result;
        }

        /// <summary>
        /// ��ȡVerticalalignment��SVG����ַ���
        /// </summary>
        /// <param name="alignment"></param>
        /// <returns></returns>
        public static string GetVerticalAlignmentString(SVG.VerticalAlignment alignment)
        {
            string result = "middle";
            if (alignment == SVG.VerticalAlignment.Top)
                result = "top";
            else if (alignment == SVG.VerticalAlignment.Bottom)
                result = "bottom";
            return result;
        }

        public static bool IsDefined(Type type, string name)
        {
            if (type == null || name == null || name.Length ==0)
                return false;

            try
            {
                string[] names = System.Enum.GetNames(type);
                var query = from t in names select t.ToLower() == name.ToLower();

                return query.Count() > 0;
            }
            catch { }
            return false;
        }
    }
    #endregion

    #region ..Win32 enum
    internal enum MouseActivateFlags
	{
		MA_ACTIVATE			= 1,
		MA_ACTIVATEANDEAT   = 2,
		MA_NOACTIVATE       = 3,
		MA_NOACTIVATEANDEAT = 4
	}

	internal enum PeekMessageFlags
	{
		PM_NOREMOVE		= 0,
		PM_REMOVE		= 1,
		PM_NOYIELD		= 2
	}

	internal enum ShowWindowStyles : short
	{
		SW_HIDE             = 0,
		SW_SHOWNORMAL       = 1,
		SW_NORMAL           = 1,
		SW_SHOWMINIMIZED    = 2,
		SW_SHOWMAXIMIZED    = 3,
		SW_MAXIMIZE         = 3,
		SW_SHOWNOACTIVATE   = 4,
		SW_SHOW             = 5,
		SW_MINIMIZE         = 6,
		SW_SHOWMINNOACTIVE  = 7,
		SW_SHOWNA           = 8,
		SW_RESTORE          = 9,
		SW_SHOWDEFAULT      = 10,
		SW_FORCEMINIMIZE    = 11,
		SW_MAX              = 11
	}

	internal enum WindowStyles : uint
	{
		WS_OVERLAPPED       = 0x00000000,
		WS_POPUP            = 0x80000000,
		WS_CHILD            = 0x40000000,
		WS_MINIMIZE         = 0x20000000,
		WS_VISIBLE          = 0x10000000,
		WS_DISABLED         = 0x08000000,
		WS_CLIPSIBLINGS     = 0x04000000,
		WS_CLIPCHILDREN     = 0x02000000,
		WS_MAXIMIZE         = 0x01000000,
		WS_CAPTION          = 0x00C00000,
		WS_BORDER           = 0x00800000,
		WS_DLGFRAME         = 0x00400000,
		WS_VSCROLL          = 0x00200000,
		WS_HSCROLL          = 0x00100000,
		WS_SYSMENU          = 0x00080000,
		WS_THICKFRAME       = 0x00040000,
		WS_GROUP            = 0x00020000,
		WS_TABSTOP          = 0x00010000,
		WS_MINIMIZEBOX      = 0x00020000,
		WS_MAXIMIZEBOX      = 0x00010000,
		WS_TILED            = 0x00000000,
		WS_ICONIC           = 0x20000000,
		WS_SIZEBOX          = 0x00040000,
		WS_POPUPWINDOW      = 0x80880000,
		WS_OVERLAPPEDWINDOW = 0x00CF0000,
		WS_TILEDWINDOW      = 0x00CF0000,
		WS_CHILDWINDOW      = 0x40000000
	}

	internal enum WindowExStyles
	{
		WS_EX_DLGMODALFRAME     = 0x00000001,
		WS_EX_NOPARENTNOTIFY    = 0x00000004,
		WS_EX_TOPMOST           = 0x00000008,
		WS_EX_ACCEPTFILES       = 0x00000010,
		WS_EX_TRANSPARENT       = 0x00000020,
		WS_EX_MDICHILD          = 0x00000040,
		WS_EX_TOOLWINDOW        = 0x00000080,
		WS_EX_WINDOWEDGE        = 0x00000100,
		WS_EX_CLIENTEDGE        = 0x00000200,
		WS_EX_CONTEXTHELP       = 0x00000400,
		WS_EX_RIGHT             = 0x00001000,
		WS_EX_LEFT              = 0x00000000,
		WS_EX_RTLREADING        = 0x00002000,
		WS_EX_LTRREADING        = 0x00000000,
		WS_EX_LEFTSCROLLBAR     = 0x00004000,
		WS_EX_RIGHTSCROLLBAR    = 0x00000000,
		WS_EX_CONTROLPARENT     = 0x00010000,
		WS_EX_STATICEDGE        = 0x00020000,
		WS_EX_APPWINDOW         = 0x00040000,
		WS_EX_OVERLAPPEDWINDOW  = 0x00000300,
		WS_EX_PALETTEWINDOW     = 0x00000188,
		WS_EX_LAYERED			= 0x00080000
	}

	internal enum Msgs
	{
		WM_ACTIVATE = 6,
		WM_ACTIVATEAPP = 0x1c,
		WM_AFXFIRST = 0x360,
		WM_AFXLAST = 0x37f,
		WM_APP = 0x8000,
		WM_ASKCBFORMATNAME = 780,
		WM_CANCELJOURNAL = 0x4b,
		WM_CANCELMODE = 0x1f,
		WM_CAPTURECHANGED = 0x215,
		WM_CHANGECBCHAIN = 0x30d,
		WM_CHANGEUISTATE = 0x127,
		WM_CHAR = 0x102,
		WM_CHARTOITEM = 0x2f,
		WM_CHILDACTIVATE = 0x22,
		WM_CHOOSEFONT_GETLOGFONT = 0x401,
		WM_CLEAR = 0x303,
		WM_CLOSE = 0x10,
		WM_COMMAND = 0x111,
		WM_COMMNOTIFY = 0x44,
		WM_COMPACTING = 0x41,
		WM_COMPAREITEM = 0x39,
		WM_CONTEXTMENU = 0x7b,
		WM_COPY = 0x301,
		WM_COPYDATA = 0x4a,
		WM_CREATE = 1,
		WM_CTLCOLOR = 0x19,
		WM_CTLCOLORBTN = 0x135,
		WM_CTLCOLORDLG = 310,
		WM_CTLCOLOREDIT = 0x133,
		WM_CTLCOLORLISTBOX = 0x134,
		WM_CTLCOLORMSGBOX = 0x132,
		WM_CTLCOLORSCROLLBAR = 0x137,
		WM_CTLCOLORSTATIC = 0x138,
		WM_CUT = 0x300,
		WM_DEADCHAR = 0x103,
		WM_DELETEITEM = 0x2d,
		WM_DESTROY = 2,
		WM_DESTROYCLIPBOARD = 0x307,
		WM_DEVICECHANGE = 0x219,
		WM_DEVMODECHANGE = 0x1b,
		WM_DISPLAYCHANGE = 0x7e,
		WM_DRAWCLIPBOARD = 0x308,
		WM_DRAWITEM = 0x2b,
		WM_DROPFILES = 0x233,
		WM_ENABLE = 10,
		WM_ENDSESSION = 0x16,
		WM_ENTERIDLE = 0x121,
		WM_ENTERMENULOOP = 0x211,
		WM_ENTERSIZEMOVE = 0x231,
		WM_ERASEBKGND = 20,
		WM_EXITMENULOOP = 530,
		WM_EXITSIZEMOVE = 0x232,
		WM_FONTCHANGE = 0x1d,
		WM_GETDLGCODE = 0x87,
		WM_GETFONT = 0x31,
		WM_GETHOTKEY = 0x33,
		WM_GETICON = 0x7f,
		WM_GETMINMAXINFO = 0x24,
		WM_GETOBJECT = 0x3d,
		WM_GETTEXT = 13,
		WM_GETTEXTLENGTH = 14,
		WM_HANDHELDFIRST = 0x358,
		WM_HANDHELDLAST = 0x35f,
		WM_HELP = 0x53,
		WM_HOTKEY = 0x312,
		WM_HSCROLL = 0x114,
		WM_HSCROLLCLIPBOARD = 0x30e,
		WM_ICONERASEBKGND = 0x27,
		WM_IME_CHAR = 0x286,
		WM_IME_COMPOSITION = 0x10f,
		WM_IME_COMPOSITIONFULL = 0x284,
		WM_IME_CONTROL = 0x283,
		WM_IME_ENDCOMPOSITION = 270,
		WM_IME_KEYDOWN = 0x290,
		WM_IME_KEYLAST = 0x10f,
		WM_IME_KEYUP = 0x291,
		WM_IME_NOTIFY = 0x282,
		WM_IME_SELECT = 0x285,
		WM_IME_SETCONTEXT = 0x281,
		WM_IME_STARTCOMPOSITION = 0x10d,
		WM_INITDIALOG = 0x110,
		WM_INITMENU = 0x116,
		WM_INITMENUPOPUP = 0x117,
		WM_INPUTLANGCHANGE = 0x51,
		WM_INPUTLANGCHANGEREQUEST = 80,
		WM_KEYDOWN = 0x100,
		WM_KEYFIRST = 0x100,
		WM_KEYLAST = 0x108,
		WM_KEYUP = 0x101,
		WM_KILLFOCUS = 8,
		WM_LBUTTONDBLCLK = 0x203,
		WM_LBUTTONDOWN = 0x201,
		WM_LBUTTONUP = 0x202,
		WM_MBUTTONDBLCLK = 0x209,
		WM_MBUTTONDOWN = 0x207,
		WM_MBUTTONUP = 520,
		WM_MDIACTIVATE = 0x222,
		WM_MDICASCADE = 0x227,
		WM_MDICREATE = 0x220,
		WM_MDIDESTROY = 0x221,
		WM_MDIGETACTIVE = 0x229,
		WM_MDIICONARRANGE = 0x228,
		WM_MDIMAXIMIZE = 0x225,
		WM_MDINEXT = 0x224,
		WM_MDIREFRESHMENU = 0x234,
		WM_MDIRESTORE = 0x223,
		WM_MDISETMENU = 560,
		WM_MDITILE = 550,
		WM_MEASUREITEM = 0x2c,
		WM_MENUCHAR = 0x120,
		WM_MENUSELECT = 0x11f,
		WM_MOUSEACTIVATE = 0x21,
		WM_MOUSEFIRST = 0x200,
		WM_MOUSEHOVER = 0x2a1,
		WM_MOUSELAST = 0x20a,
		WM_MOUSELEAVE = 0x2a3,
		WM_MOUSEMOVE = 0x200,
		WM_MOUSEWHEEL = 0x20a,
		WM_MOVE = 3,
		WM_MOVING = 0x216,
		WM_NCACTIVATE = 0x86,
		WM_NCCALCSIZE = 0x83,
		WM_NCCREATE = 0x81,
		WM_NCDESTROY = 130,
		WM_NCHITTEST = 0x84,
		WM_NCLBUTTONDBLCLK = 0xa3,
		WM_NCLBUTTONDOWN = 0xa1,
		WM_NCLBUTTONUP = 0xa2,
		WM_NCMBUTTONDBLCLK = 0xa9,
		WM_NCMBUTTONDOWN = 0xa7,
		WM_NCMBUTTONUP = 0xa8,
		WM_NCMOUSEMOVE = 160,
		WM_NCPAINT = 0x85,
		WM_NCRBUTTONDBLCLK = 0xa6,
		WM_NCRBUTTONDOWN = 0xa4,
		WM_NCRBUTTONUP = 0xa5,
		WM_NCXBUTTONDBLCLK = 0xad,
		WM_NCXBUTTONDOWN = 0xab,
		WM_NCXBUTTONUP = 0xac,
		WM_NEXTDLGCTL = 40,
		WM_NEXTMENU = 0x213,
		WM_NOTIFY = 0x4e,
		WM_NOTIFYFORMAT = 0x55,
		WM_NULL = 0,
		WM_PAINT = 15,
		WM_PAINTCLIPBOARD = 0x309,
		WM_PAINTICON = 0x26,
		WM_PALETTECHANGED = 0x311,
		WM_PALETTEISCHANGING = 0x310,
		WM_PARENTNOTIFY = 0x210,
		WM_PASTE = 770,
		WM_PENWINFIRST = 0x380,
		WM_PENWINLAST = 0x38f,
		WM_POWER = 0x48,
		WM_POWERBROADCAST = 0x218,
		WM_PRINT = 0x317,
		WM_PRINTCLIENT = 0x318,
		WM_QUERYDRAGICON = 0x37,
		WM_QUERYENDSESSION = 0x11,
		WM_QUERYNEWPALETTE = 0x30f,
		WM_QUERYOPEN = 0x13,
		WM_QUERYUISTATE = 0x129,
		WM_QUEUESYNC = 0x23,
		WM_QUIT = 0x12,
		WM_RBUTTONDBLCLK = 0x206,
		WM_RBUTTONDOWN = 0x204,
		WM_RBUTTONUP = 0x205,
		WM_REFLECT = 0x2000,
		WM_RENDERALLFORMATS = 0x306,
		WM_RENDERFORMAT = 0x305,
		WM_SETCURSOR = 0x20,
		WM_SETFOCUS = 7,
		WM_SETFONT = 0x30,
		WM_SETHOTKEY = 50,
		WM_SETICON = 0x80,
		WM_SETREDRAW = 11,
		WM_SETTEXT = 12,
		WM_SETTINGCHANGE = 0x1a,
		WM_SHOWWINDOW = 0x18,
		WM_SIZE = 5,
		WM_SIZECLIPBOARD = 0x30b,
		WM_SIZING = 0x214,
		WM_SPOOLERSTATUS = 0x2a,
		WM_STYLECHANGED = 0x7d,
		WM_STYLECHANGING = 0x7c,
		WM_SYSCHAR = 0x106,
		WM_SYSCOLORCHANGE = 0x15,
		WM_SYSCOMMAND = 0x112,
		WM_SYSDEADCHAR = 0x107,
		WM_SYSKEYDOWN = 260,
		WM_SYSKEYUP = 0x105,
		WM_TCARD = 0x52,
		WM_TIMECHANGE = 30,
		WM_TIMER = 0x113,
		WM_UNDO = 0x304,
		WM_UPDATEUISTATE = 0x128,
		WM_USER = 0x400,
		WM_USERCHANGED = 0x54,
		WM_VKEYTOITEM = 0x2e,
		WM_VSCROLL = 0x115,
		WM_VSCROLLCLIPBOARD = 0x30a,
		WM_WINDOWPOSCHANGED = 0x47,
		WM_WINDOWPOSCHANGING = 70,
		WM_WININICHANGE = 0x1a,
		WM_XBUTTONDBLCLK = 0x20d,
		WM_XBUTTONDOWN = 0x20b,
		WM_XBUTTONUP = 0x20c,
		WS_VSCROLL = 0x200000,
		EN_VSCROLL = 0x602
	}

    internal enum ScrollBarDirection
    {
        SB_HORZ = 0,
        SB_VERT = 1,
        SB_CTL = 2,
        SB_BOTH = 3
    }

    internal enum ScrollInfoMask
    {
        SIF_RANGE = 0x1,
        SIF_PAGE = 0x2,
        SIF_POS = 0x4,
        SIF_DISABLENOSCROLL = 0x8,
        SIF_TRACKPOS = 0x10,
        SIF_ALL = SIF_RANGE + SIF_PAGE + SIF_POS + SIF_TRACKPOS
    }
    #endregion
}
