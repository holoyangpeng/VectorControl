using System;

namespace YP.VectorControl.Clipboard
{
	/// <summary>
	/// 定义VectorControl可以识别的剪贴板对象
	/// </summary>
	public class DragDropObject:System.Windows.Forms.DataObject
    {
        #region ..static string
        internal static string ClipboardSymbolString = "YP.SVG.Symbol";
        #endregion

        #region ..Constructor
        /// <summary>
		/// 创建一个DragDropObject
		/// </summary>
		/// <param name="o">剪切板数据</param>
        public DragDropObject(object o)
            : base(ClipboardSymbolString, o)
		{
		}
		#endregion
	}
}
