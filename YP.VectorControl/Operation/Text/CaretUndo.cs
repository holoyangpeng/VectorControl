using System;

namespace YP.VectorControl.Operation.Text
{
	/// <summary>
	/// CaretUndo 的摘要说明。
	/// </summary>
	internal class CaretUndo:YP.SVG.Interface.IUndoOperation
	{
		#region ..构造及消除
		public CaretUndo(TextEditor editor,InfoPos pos)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this._editor = editor;
			this.oldPos = pos;
			
		}
		#endregion

		#region ..私有变量
		TextEditor _editor = null;
		InfoPos oldPos ;
		InfoPos newPos;
		#endregion

		#region IUndoOperation 成员

		public void Redo()
		{
			// TODO:  添加 CaretUndo.Redo 实现
			if(this._editor != null && !this._editor.Disposed)
			this._editor.Caret.AdaptCaret(this.newPos.Info,this.newPos.Offset);
		}

		public void Undo()
		{
			// TODO:  添加 CaretUndo.Undo 实现
			if(this._editor != null&& !this._editor.Disposed)
			{
				InfoPos pos = new InfoPos(this._editor.Caret.Info,this._editor.Caret.Offset);
				this.newPos = pos;
				this._editor.Caret.AdaptCaret(this.oldPos.Info,this.oldPos.Offset);
			}
		}

		#endregion
	}
}
