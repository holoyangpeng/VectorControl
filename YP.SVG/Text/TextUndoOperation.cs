using System;

namespace YP.SVG.Text
{
	#region ..InfoAction
	public enum InfoAction
	{
		Insert,
		Remove
	}
	#endregion

	/// <summary>
	/// TextUndoOperation 的摘要说明。
	/// </summary>
	public class TextUndoOperation:Interface.IUndoOperation
	{
		#region ..构造及消除
		public TextUndoOperation(TextContentInfo changedinfo,int changedOffset,int changedlength,string oritext,string newtext)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.offset = changedOffset;
			this.length = changedlength;
			this.info = changedinfo;
			this.oritext = oritext;
			this.changedtext = newtext;
		}
		#endregion

		#region ..私有变量
		TextContentInfo info = null;
		int offset = 0,length = 0;
		string oritext = string.Empty,changedtext = string.Empty;
		#endregion

		#region IUndoOperation 成员
		/// <summary>
		/// 重做上一步操作
		/// </summary>
		public void Redo()
		{
			// TODO:  添加 TextUndoOperation.Redo 实现
			if(this.info != null)
			{
				this.info.ReplaceString(this.offset,this.length,this.changedtext);
			}
		}

		/// <summary>
		/// 撤销上一步操作
		/// </summary>
		public void Undo()
		{
			// TODO:  添加 TextUndoOperation.Undo 实现
			if(this.info != null)
			{
				info.ReplaceString(this.offset,this.changedtext.Length,this.oritext);
			}
		}

		#endregion
	}
}
