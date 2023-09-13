using System;

namespace YP.VectorControl.Operation.Text
{
	/// <summary>
	/// CaretUndo ��ժҪ˵����
	/// </summary>
	internal class CaretUndo:YP.SVG.Interface.IUndoOperation
	{
		#region ..���켰����
		public CaretUndo(TextEditor editor,InfoPos pos)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this._editor = editor;
			this.oldPos = pos;
			
		}
		#endregion

		#region ..˽�б���
		TextEditor _editor = null;
		InfoPos oldPos ;
		InfoPos newPos;
		#endregion

		#region IUndoOperation ��Ա

		public void Redo()
		{
			// TODO:  ��� CaretUndo.Redo ʵ��
			if(this._editor != null && !this._editor.Disposed)
			this._editor.Caret.AdaptCaret(this.newPos.Info,this.newPos.Offset);
		}

		public void Undo()
		{
			// TODO:  ��� CaretUndo.Undo ʵ��
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
