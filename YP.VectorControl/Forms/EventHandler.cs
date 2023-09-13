using System;
using System.Drawing;
using System.Windows.Forms;
using YP.SVG;

namespace YP.VectorControl.Forms
{
    /// <summary>
    /// ����ͼԪDrop�¼��ķ���
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ElementDroppedEventHandler(object sender, ElementDroppedEventArgs e);

    /// <summary>
    /// ͼԪDrop�¼�������
    /// </summary>
    public class ElementDroppedEventArgs : System.EventArgs
    {
        public ElementDroppedEventArgs(SVGElement droppedInstance)
        {
            this.DroppedInstance = droppedInstance;
        }

        /// <summary>
        /// ��ȡ��Drop�¼��н�Ҫ����ӵ�Dom����ͼԪ����
        /// </summary>
        public SVGElement DroppedInstance { private set; get; }
    }


	/// <summary>
	/// ������ͼ��������ʱ���¼�
	/// </summary>
	public delegate bool ElementConnectEventHandler(object sender,ElementConnectEventArgs e);

	/// <summary>
	/// ��ͼ��������ʱ��ص�����
	/// </summary>
	public class ElementConnectEventArgs:System.EventArgs
	{
		#region ..Constructor
		public ElementConnectEventArgs(SVGTransformableElement element,int anchorIndex, int numberOfPoints, ConnectionTargetType type,SVG.BasicShapes.SVGBranchElement connectElement)
		{
			this.type = type;
			this.targetElement = element;
			this.connectElement = connectElement;
            this.anchorIndex = anchorIndex;
            this.numberOfPoints = numberOfPoints;
		}
		#endregion

		#region ..private fields
		SVGTransformableElement targetElement = null;
		ConnectionTargetType type = ConnectionTargetType.StartElement;
        SVG.BasicShapes.SVGBranchElement connectElement = null;
        int anchorIndex = 0;
        int numberOfPoints = 0;
		#endregion

		#region ..public properties
		/// <summary>
		/// ��ͼ�������ӵ�������
		/// </summary>
        public SVG.BasicShapes.SVGBranchElement ConnectionElement
		{
			get
			{
				return this.connectElement;
			}
		}

		/// <summary>
		/// ��ͼ��������Ŀ��ͼԪ����
        /// �����������֧���򵱽�����֧ʱ�����ؽ�Ҫ���ӵ���Ŀ��������
		/// </summary>
		public SVGTransformableElement TargetElement
		{
			get
			{
				return this.targetElement;
			}
		}

		/// <summary>
        /// ��ͼ��������Ŀ��ͼԪ��������
        /// ����ǽ�����֧������Branch
		/// </summary>
		public ConnectionTargetType Type
		{
			get
			{
				return this.type;
			}
		}

        /// <summary>
        /// ��ͼ������Ŀ��ͼԪ�����ӵ�����
        /// ��������ӵ���״���򷵻�-1
        /// ���������֧������-1
        /// </summary>
        public int AnchorIndex
        {
            get
            {
                return anchorIndex;
            }
        }

        /// <summary>
        /// ��ȡĿ��ͼԪ�����ӵ����
        /// ���������֧������0
        /// </summary>
        public int NumberOfPoints
        {
            get
            {
                return this.numberOfPoints;
            }
        }
		#endregion
	}

	/// <summary>
	/// ����ElementClick�¼��ķ���
	/// </summary>
	public delegate void ElementClickEventHandler(object sender,ElementClickEventArgs e);

	/// <summary>
	/// ����ElementClick�¼�����������
	/// </summary>
	public class ElementClickEventArgs:System.EventArgs 
	{
		#region ..Constructor
		/// <summary>
		/// ��ָ�����ݳ�ʼ��ElementClickEventArgs
		/// </summary>
		/// <param name="element">����Click��SVGElement����</param>
		/// <param name="clickType">Click����</param>
		public ElementClickEventArgs(SVGElement element,MouseClickType clickType,System.Windows.Forms.MouseButtons button)
		{
			this._clickType = clickType;
			this._element = element;
			this._button = button;
		}
		#endregion

		#region ..private fields
		SVGElement _element;
		MouseClickType _clickType = MouseClickType.SingleClick;
		System.Windows.Forms.MouseButtons _button = System.Windows.Forms.MouseButtons.None;
        bool bubble = true;
		#endregion

		#region ..public properties
        /// <summary>
        /// ��ȡ������һ��ֵ��ָʾClick�¼��Ƿ�������������Ϊ����ؼ��ڲ��������ٽ��յ�Click�¼�
        /// </summary>
        public bool Bubble
        {
            set
            {
                this.bubble = value;
            }
            get
            {
                return this.bubble;
            }
        }

		/// <summary>
		/// ��ȡElementClick�¼��е�SVGElement����
		/// </summary>
		public SVGElement Element
		{
			get
			{
				return this._element;
			}
		}

		/// <summary>
		/// ��ȡClick��MouseButton����
		/// </summary>
		public System.Windows.Forms.MouseButtons MouseButton
		{
			get
			{
				return this._button;
			}
		}

		/// <summary>
		/// ��ȡClick����
		/// </summary>
		public MouseClickType ClickType
		{
			get
			{
				return this._clickType;
			}
		}
		#endregion
	}

    /// <summary>
    /// Wrap the mouse event args
    /// </summary>
    internal class TLMouseEventArgs : System.Windows.Forms.MouseEventArgs
    {
        #region ..Constructor
        public TLMouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta, int nativeX, int natvieY ):base(button, clicks, x, y, delta)
        {
            this.nativePoint = new Point(nativeX, natvieY);
        }
        #endregion

        #region ..private fields
        Point nativePoint = Point.Empty;
        #endregion

        #region ..properties
        public Point NativePoint
        {
            get
            {
                return this.nativePoint;
            }
        }
        #endregion
    }
}
