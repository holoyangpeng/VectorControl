using System;

namespace YP.VectorControl.Clipboard
{
	/// <summary>
	/// ����VectorControl����ʶ��ļ��������
	/// </summary>
	public class DragDropObject:System.Windows.Forms.DataObject
    {
        #region ..static string
        internal static string ClipboardSymbolString = "YP.SVG.Symbol";
        #endregion

        #region ..Constructor
        /// <summary>
		/// ����һ��DragDropObject
		/// </summary>
		/// <param name="o">���а�����</param>
        public DragDropObject(object o)
            : base(ClipboardSymbolString, o)
		{
		}
		#endregion
	}
}
