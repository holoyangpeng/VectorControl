using System;

namespace YP.VectorControl.Operation.LabelText
{
	/// <summary>
	/// InsertUndo 的摘要说明。
	/// </summary>
	internal class InsertUndo:YP.SVG.Interface.IUndoOperation 
	{
		#region ..构造及消除
		public InsertUndo(LabelTextOperation editor,int offset,string text)
		{
			//
			// TODO: 在此处添加构造函数逻辑s
			//
			this.offset = offset;
			this._editor = editor;
			this.insertstring = text;
			this.render = this._editor.CaretRender;
		}
		#endregion

		#region ..私有变量
		LabelTextOperation _editor = null;
		int offset = 0;
		string insertstring = string.Empty;
		YP.SVG.SVGTransformableElement render = null;
		#endregion

		#region ..Undo
		public void Undo()
		{
			if(this._editor != null && !this._editor.Disposed)
			{
				this._editor.RemoveString(this.offset,this.insertstring.Length);
			}
			else if(this.render != null && this.render.ParentNode != null)
			{
				this.render.SVGRenderer.RemoveString(this.offset,this.insertstring.Length);
			}
		}
		#endregion

		#region ..Redo
		public void Redo()
		{
			if(this._editor != null && !this._editor.Disposed)
			{
				this._editor.Insert(this.offset,this.insertstring);
			}
			else if(this.render != null && this.render.ParentNode != null)
			{
				this.render.SVGRenderer.InsertStr(this.offset,this.insertstring);
			}
		}
		#endregion
	}
}
