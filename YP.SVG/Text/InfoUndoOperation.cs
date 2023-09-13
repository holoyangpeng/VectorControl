using System;

namespace YP.SVG.Text
{
	/// <summary>
	/// InfoUndoOperation 的摘要说明。
	/// </summary>
	public class InfoUndoOperation:Interface.IUndoOperation
	{
		#region ..构造及消除
		public InfoUndoOperation(YP.SVG.Text.SVGTextContentElement owner,int index,TextContentInfo info,InfoAction action)
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.ownerTextElement = owner;
			this.index = index;
			this.info = info;
			this.action = action;
		}
		#endregion

		#region ..私有变量
		YP.SVG.Text.SVGTextContentElement ownerTextElement;
		int index = 0;
		TextContentInfo info;
		InfoAction action = InfoAction.Insert;
		#endregion

		#region IUndoOperation 成员

		public void Redo()
		{
			// TODO:  添加 InfoUndoOperation.Redo 实现
			if(this.ownerTextElement != null &&this.ownerTextElement.TextContentInfos != null&& this.ownerTextElement.ParentNode != null)
			{
				if(this.action == InfoAction.Insert)
					this.ownerTextElement.InsertInfo(this.index,this.info);
				else
					this.ownerTextElement.RemoveInfo(info);
			}
		}

		public void Undo()
		{
			// TODO:  添加 InfoUndoOperation.Undo 实现
			if(this.ownerTextElement != null &&this.ownerTextElement.TextContentInfos != null && this.ownerTextElement.ParentNode != null)
			{
				if(this.action == InfoAction.Insert)
					this.ownerTextElement.RemoveInfo(this.info);
				else
					this.ownerTextElement.InsertInfo(this.index,info);
			}
		}

		#endregion
	}
}
