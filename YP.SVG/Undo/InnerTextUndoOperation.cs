using System;

namespace YP.SVG.Undo
{
	/// <summary>
	/// InnerTextUndoOperation 的摘要说明。
	/// </summary>
	public class InnerTextUndoOperation:Interface.IUndoOperation	
	{
		#region ..Constructor
		public InnerTextUndoOperation(SVG.SVGElement changedElement,string oldValue,string newValue)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.oldValue = oldValue;
			this.newValue = newValue;
			this.ownerElement = changedElement;
		}
		#endregion

		#region ..private fields
		string oldValue = string.Empty;
		string newValue = string.Empty;
		SVG.SVGElement ownerElement = null;
		#endregion

		#region ..Redo
		/// <summary>
		/// 重复上一步操作
		/// </summary>
		public void Redo()
		{
			if(this.ownerElement != null)
				this.ownerElement.InnerText = newValue;
		}
		#endregion

		#region ..Undo
		/// <summary>
		/// 撤销上一步操作
		/// </summary>
		public void Undo()
		{
			if(this.ownerElement != null)
				this.ownerElement.InnerText = oldValue;
		}
		#endregion
	}
}
