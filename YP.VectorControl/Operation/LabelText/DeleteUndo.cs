using System;

namespace YP.VectorControl.Operation.LabelText
{
	/// <summary>
	/// DeleteUndo 的摘要说明。
	/// </summary>
	internal class DeleteUndo:YP.SVG.Interface.IUndoOperation 
	{
		#region ..构造及消除
		public DeleteUndo(LabelTextOperation editor,int offset,int length)
		{
			//
			// TODO: 在此处添加构造函数逻辑s
			//
			this.offset = offset;
			this._editor = editor;
			this.length = length;
			string text = editor.CaretRender.Label;
			if(text.Length > 0)
			{
				offset = (int)Math.Min(text.Length - 1,Math.Max(0,offset));
				length = (int)Math.Max(0,Math.Min(length,text.Length - offset));
				this.oritext = text.Substring(offset,length);
			}
			this.render = this._editor.CaretRender;
		}
		#endregion

		#region ..私有变量
		LabelTextOperation _editor = null;
		int offset = 0;
		int length=0;
		string oritext = string.Empty;
		YP.SVG.SVGTransformableElement render = null;
		#endregion

		#region ..Undo
		public void Undo()
		{
			if(this._editor != null && !this._editor.Disposed)
			{
				this._editor.Insert(this.offset,this.oritext);
			}
			else if(this.render != null&& this.render.ParentNode != null)
			{
				this.render.SVGRenderer.InsertStr(this.offset,this.oritext);
			}
		}
		#endregion

		#region ..Redo
		public void Redo()
		{
			if(this._editor != null && !this._editor.Disposed)
			{
				this._editor.RemoveString(this.offset,this.length);
			}
			else if(this.render != null && this.render.ParentNode != null)
			{
				this.render.SVGRenderer.RemoveString(this.offset,this.length);
			}
		}
		#endregion
	}
}
