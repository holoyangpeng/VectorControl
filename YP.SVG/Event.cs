using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using YP.SVG.Interface;
using System.Drawing;

namespace YP.SVG
{
    #region ..CollectionChanged
    /// <summary>
    /// 处理集合改变事件
    /// </summary>
    public delegate void CollectionChangedEventHandler(object sender, CollectionChangedEventArgs e);

    #region ..记录集合改变的数据
    /// <summary>
    /// 记录集合改变的数据
    /// </summary>
    public class CollectionChangedEventArgs : System.EventArgs
    {
        /// <summary>
        /// 定义发生选择的对象
        /// </summary>
        SVGElement[] changeElements;
        CollectionChangeAction action = CollectionChangeAction.None;

        #region ..公共属性
        /// <summary>
        /// 获取发生改变的元素
        /// </summary>
        public SVGElement[] ChangeElements
        {
            get
            {
                return this.changeElements;
            }
        }

        /// <summary>
        /// 获取集合操作类型
        /// </summary>
        public CollectionChangeAction Action
        {
            get
            {
                return this.action;
            }
        }
        #endregion

        /// <summary>
        /// 用改变对象和改变类型初始化对象
        /// </summary>
        /// <param name="element">发生改变的对象</param>
        /// <param name="action">操作类型</param>
        public CollectionChangedEventArgs(SVGElement element, CollectionChangeAction action)
        {
            this.changeElements = new SVGElement[] { element };
            this.action = action;
        }

        /// <summary>
        /// 用改变对象集合和改变类型初始化度想
        /// </summary>
        /// <param name="list">发生改变的对象数组</param>
        /// <param name="action">操作类型</param>
        public CollectionChangedEventArgs(SVGElement[] list, CollectionChangeAction action)
        {
            this.changeElements = list;
            this.action = action;
        }
    }
    #endregion
    #endregion

    #region ..ExcetionOccur
    /// <summary>
    /// 包含文档出错事件的句柄
    /// </summary>
    public delegate void ExceptionOccuredEventHandler(object sender, ExceptionOccuredEventArgs e);

    /// <summary>
    /// 指定错误的级别
    /// </summary>
    public enum ExceptionLevel
    {
        High,
        Normal
    }

    /// <summary>
    /// 包含文档出错的数据
    /// </summary>
    public class ExceptionOccuredEventArgs : EventArgs
    {
        #region ..私有变量
        object[] _args = null;
        ExceptionLevel _level = ExceptionLevel.Normal;
        #endregion

        #region ..构造及消除
        public ExceptionOccuredEventArgs(object[] args, ExceptionLevel level)
        {
            this._args = args;
            this._level = level;
        }
        #endregion

        #region ..公共属性
        /// <summary>
        /// 指示文档出错的信息
        /// </summary>
        public object[] Args
        {
            get
            {
                return this._args;
            }
        }

        /// <summary>
        /// 指示错误级别
        /// </summary>
        public ExceptionLevel Level
        {
            get
            {
                return this._level;
            }
        }
        #endregion
    }
    #endregion

    #region ..ElementChangedEvent
    /// <summary>
    /// 提供访问事件的delegate
    /// </summary>
    public delegate void SVGElementChangedEventHandler(object sender, SVGElementChangedEventArgs e);

    /// <summary>
    /// 当对象属性发生变化时触发
    /// </summary>
    public delegate void AttributeChangedEventHandler(object sender, AttributeChangedEventArgs e);

    /// <summary>
    /// 存储AttributeChanged事件的数据
    /// </summary>
    public class AttributeChangedEventArgs : System.EventArgs
    {
        #region ..private fields
        string attributeName;
        SVGElement changedElement = null;
        #endregion

        #region ..Constructor
        public AttributeChangedEventArgs(SVGElement changedElement, string attributeName)
        {
            this.attributeName = attributeName;
            this.changedElement = changedElement;
        }
        #endregion

        #region ..public properties
        /// <summary>
        /// gets the changed attribute
        /// </summary>
        public string AttributeName
        {
            get
            {
                return this.attributeName;
            }
        }

        /// <summary>
        /// gets the changed element
        /// </summary>
        public SVGElement ChangedElement
        {
            get
            {
                return this.changedElement;
            }
        }
        #endregion
    }

    /// <summary>
    /// 指定对象改变的类型
    /// </summary>
    public enum SVGElementChangedAction
    {
        Insert,
        Remove
    }

    /// <summary>
    /// 存储<seealso cref="SVGElementChangedEventHandler">SVGElementChanged</seealso>事件的数据
    /// </summary>
    public class SVGElementChangedEventArgs : System.EventArgs
    {
        SVGElement _element;
        SVGElement _oldParent;
        SVGElement _newParent;
        SVGElementChangedAction _action;

        public SVGElementChangedEventArgs(SVGElement element, SVGElement oldparent, SVGElement newparent, SVGElementChangedAction action)
        {
            this._action = action;
            this._oldParent = oldparent;
            this._newParent = newparent;
            this._element = element;
        }

        #region ..public properties
        /// <summary>
        /// 获取发生改变的对象
        /// </summary>
        public SVGElement Element
        {
            get
            {
                return this._element;
            }
        }

        /// <summary>
        /// 获取操作前对象的ParentElement的值
        /// </summary>
        public SVGElement OldParent
        {
            get
            {
                return this._oldParent;
            }
        }

        /// <summary>
        /// 获取操作后对象的ParentElement的值
        /// </summary>
        public SVGElement NewParent
        {
            get
            {
                return this._newParent;
            }
        }

        /// <summary>
        /// 获取对象发生改变的类型
        /// </summary>
        public SVGElementChangedAction Action
        {
            get
            {
                return this._action;
            }
        }
        #endregion
    }
    #endregion

    #region ..ConnectChanged
    /// <summary>
    /// 处理ConnectionChanged事件的方法
    /// </summary>
    public delegate void ConnectionChangedEventHandler(object sender, ConnectionChangedEventArgs e);

    /// <summary>
    /// 指定连接线变化的类型
    /// </summary>
    public enum ConnectionChangedType
    {
        /// <summary>
        /// 连接线的开始对象发生改变
        /// </summary>
        StartElement,

        /// <summary>
        /// 连接线的结束对象发生改变
        /// </summary>
        EndElement
    }

    /// <summary>
    /// 包含<seealso cref="ConnectChangedEventHandler">ConnectionChanged</seealso>事件的数据
    /// </summary>
    public class ConnectionChangedEventArgs : System.EventArgs
    {
        #region ..private fields
        SVGElement _oldElement;
        SVGElement _newElement;
        ConnectionChangedType _type;
        SVG.BasicShapes.SVGBranchElement _changedElement;
        #endregion

        #region ..Constructor
        /// <summary>
        /// 初始化ConnectionChangedEventArgs实例
        /// </summary>
        /// <param name="oldElement"></param>
        /// <param name="newElement"></param>
        /// <param name="type"></param>
        public ConnectionChangedEventArgs(SVG.BasicShapes.SVGBranchElement changedElement, SVGElement oldElement, SVGElement newElement, ConnectionChangedType type)
        {
            this._oldElement = oldElement;
            this._newElement = newElement;
            this._type = type;
            this._changedElement = changedElement;
        }
        #endregion

        #region ..public properties
        /// <summary>
        /// 获取连接线变化的类型
        /// </summary>
        public ConnectionChangedType Type
        {
            get
            {
                return this._type;
            }
        }

        /// <summary>
        /// 获取发生改变的连接线对象
        /// </summary>
        public SVG.BasicShapes.SVGBranchElement ChangedElement
        {
            get
            {
                return this._changedElement;
            }
        }

        /// <summary>
        /// 获取变化之前的连接对象
        /// </summary>
        public SVGElement OldElement
        {
            get
            {
                return this._oldElement;
            }
        }

        /// <summary>
        /// 获取变化之后的连接对象
        /// </summary>
        public SVGElement NewElement
        {
            get
            {
                return this._newElement;
            }
        }
        #endregion
    }
    #endregion

    /// <summary>
    /// 处理绘制连接点的代理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void PaintConnectablePointEventHandler(object sender, PaintConnectablePointEventArgs e);

    /// <summary>
    /// 存储绘制连接点的数据
    /// </summary>
    public class PaintConnectablePointEventArgs : System.EventArgs
    {
        #region ..Constructor
        public PaintConnectablePointEventArgs(Graphics g, SVGTransformableElement elm, PointF point, int anchorIndex)
        {
            this.graphics = g;
            this.point = point;
            this.anchorIndex = anchorIndex;
            this.element = elm;
            this.relativePoint = PointF.Empty;
            if (anchorIndex < elm.BaseConnectionPoints.NumberOfItems)
            {
                SVG.DataType.SVGPoint p = (SVG.DataType.SVGPoint)(elm.BaseConnectionPoints.GetItem(anchorIndex));
                this.relativePoint = new PointF(p.X, p.Y);
            }
        }
        #endregion

        #region ..private
        Graphics graphics;
        PointF point;
        PointF relativePoint;
        int anchorIndex;
        SVGTransformableElement element;
        #endregion

        #region ..properties
        /// <summary>
        /// 获取绘图图面
        /// </summary>
        public Graphics Graphics
        {
            get
            {
                return this.graphics;
            }
        }

        /// <summary>
        /// 获取需要绘制的连接点的原始相对坐标
        /// </summary>
        public PointF RelativePoint
        {
            get
            {
                return this.relativePoint;
            }
        }

        /// <summary>
        /// 获取需要绘制的连接点坐标
        /// </summary>
        public PointF Point
        {
            get
            {
                return this.point;
            }
        }

        /// <summary>
        /// 获取连接点索引
        /// </summary>
        public int ConnectablePointIndex
        {
            get
            {
                return anchorIndex;
            }
        }

        /// <summary>
        /// 获取需要绘制的图元
        /// </summary>
        public SVGTransformableElement Element
        {
            get
            {
                return this.element;
            }
        }

        /// <summary>
        /// 获取或设置一个值，该值指示用户是否自己绘制连接点
        /// </summary>
        public bool OwnerDraw { set; get; }
        #endregion
    }
}
