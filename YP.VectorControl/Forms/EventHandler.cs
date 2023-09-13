using System;
using System.Drawing;
using System.Windows.Forms;
using YP.SVG;

namespace YP.VectorControl.Forms
{
    /// <summary>
    /// 处理图元Drop事件的方法
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ElementDroppedEventHandler(object sender, ElementDroppedEventArgs e);

    /// <summary>
    /// 图元Drop事件的数据
    /// </summary>
    public class ElementDroppedEventArgs : System.EventArgs
    {
        public ElementDroppedEventArgs(SVGElement droppedInstance)
        {
            this.DroppedInstance = droppedInstance;
        }

        /// <summary>
        /// 获取在Drop事件中将要被添加到Dom树的图元对象
        /// </summary>
        public SVGElement DroppedInstance { private set; get; }
    }


	/// <summary>
	/// 处理试图建立连接时的事件
	/// </summary>
	public delegate bool ElementConnectEventHandler(object sender,ElementConnectEventArgs e);

	/// <summary>
	/// 试图建立连接时相关的数据
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
		/// 试图建立连接的连接线
		/// </summary>
        public SVG.BasicShapes.SVGBranchElement ConnectionElement
		{
			get
			{
				return this.connectElement;
			}
		}

		/// <summary>
		/// 试图建立到的目标图元对象
        /// 如果允许建立分支，则当建立分支时，返回将要连接到的目标连接线
		/// </summary>
		public SVGTransformableElement TargetElement
		{
			get
			{
				return this.targetElement;
			}
		}

		/// <summary>
        /// 试图建立到的目标图元对象类型
        /// 如果是建立分支，返回Branch
		/// </summary>
		public ConnectionTargetType Type
		{
			get
			{
				return this.type;
			}
		}

        /// <summary>
        /// 试图建立到目标图元的连接点索引
        /// 如果是连接到形状，则返回-1
        /// 如果建立分支，返回-1
        /// </summary>
        public int AnchorIndex
        {
            get
            {
                return anchorIndex;
            }
        }

        /// <summary>
        /// 获取目标图元的连接点个数
        /// 如果建立分支，返回0
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
	/// 处理ElementClick事件的方法
	/// </summary>
	public delegate void ElementClickEventHandler(object sender,ElementClickEventArgs e);

	/// <summary>
	/// 定义ElementClick事件包含的数据
	/// </summary>
	public class ElementClickEventArgs:System.EventArgs 
	{
		#region ..Constructor
		/// <summary>
		/// 用指定数据初始化ElementClickEventArgs
		/// </summary>
		/// <param name="element">发生Click的SVGElement对象</param>
		/// <param name="clickType">Click类型</param>
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
        /// 获取或设置一个值，指示Click事件是否继续，如果设置为否，则控件内部将不会再接收到Click事件
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
		/// 获取ElementClick事件中的SVGElement对象
		/// </summary>
		public SVGElement Element
		{
			get
			{
				return this._element;
			}
		}

		/// <summary>
		/// 获取Click的MouseButton类型
		/// </summary>
		public System.Windows.Forms.MouseButtons MouseButton
		{
			get
			{
				return this._button;
			}
		}

		/// <summary>
		/// 获取Click类型
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
