using System;

namespace YP.SVG.Text
{
	/// <summary>
	/// InfoUndoOperation ��ժҪ˵����
	/// </summary>
	public class InfoUndoOperation:Interface.IUndoOperation
	{
		#region ..���켰����
		public InfoUndoOperation(YP.SVG.Text.SVGTextContentElement owner,int index,TextContentInfo info,InfoAction action)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.ownerTextElement = owner;
			this.index = index;
			this.info = info;
			this.action = action;
		}
		#endregion

		#region ..˽�б���
		YP.SVG.Text.SVGTextContentElement ownerTextElement;
		int index = 0;
		TextContentInfo info;
		InfoAction action = InfoAction.Insert;
		#endregion

		#region IUndoOperation ��Ա

		public void Redo()
		{
			// TODO:  ��� InfoUndoOperation.Redo ʵ��
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
			// TODO:  ��� InfoUndoOperation.Undo ʵ��
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
