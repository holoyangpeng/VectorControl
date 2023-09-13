using System;

namespace YP.VectorControl.Operation.LabelText
{
	/// <summary>
	/// CaretUndo 的摘要说明。
	/// </summary>
	internal class CaretUndo:YP.SVG.Interface.IUndoOperation
	{
		#region ..构造及消除
		public CaretUndo(LabelTextOperation editor,int offset)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this._editor = editor;
			this.oldOffset = offset;
		}
		#endregion

		#region ..私有变量
		LabelTextOperation _editor = null;
		int oldOffset = 0;
		int newOffset = 0;
		#endregion

		#region ..Undo
		public void Undo()
		{
			if(this._editor != null &&!this._editor.Disposed)
			{
				this.newOffset = this._editor.Caret.Offset;
				this._editor.Caret.Offset = this.oldOffset;
			}
		}
		#endregion

		#region ..Redo
		public void Redo()
		{
			if(this._editor != null &&!this._editor.Disposed)
			{
				this._editor.Caret.Offset = this.newOffset;
			}
		}
		#endregion
	}
}
