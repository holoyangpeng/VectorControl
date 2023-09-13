using System;

namespace YP.VectorControl.Operation.LabelText
{
	/// <summary>
	/// CaretUndo ��ժҪ˵����
	/// </summary>
	internal class CaretUndo:YP.SVG.Interface.IUndoOperation
	{
		#region ..���켰����
		public CaretUndo(LabelTextOperation editor,int offset)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this._editor = editor;
			this.oldOffset = offset;
		}
		#endregion

		#region ..˽�б���
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
