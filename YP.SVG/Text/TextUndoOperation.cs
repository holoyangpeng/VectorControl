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
	/// TextUndoOperation ��ժҪ˵����
	/// </summary>
	public class TextUndoOperation:Interface.IUndoOperation
	{
		#region ..���켰����
		public TextUndoOperation(TextContentInfo changedinfo,int changedOffset,int changedlength,string oritext,string newtext)
		{
			//
			// TODO: �ڴ˴���ӹ��캯���߼�
			//
			this.offset = changedOffset;
			this.length = changedlength;
			this.info = changedinfo;
			this.oritext = oritext;
			this.changedtext = newtext;
		}
		#endregion

		#region ..˽�б���
		TextContentInfo info = null;
		int offset = 0,length = 0;
		string oritext = string.Empty,changedtext = string.Empty;
		#endregion

		#region IUndoOperation ��Ա
		/// <summary>
		/// ������һ������
		/// </summary>
		public void Redo()
		{
			// TODO:  ��� TextUndoOperation.Redo ʵ��
			if(this.info != null)
			{
				this.info.ReplaceString(this.offset,this.length,this.changedtext);
			}
		}

		/// <summary>
		/// ������һ������
		/// </summary>
		public void Undo()
		{
			// TODO:  ��� TextUndoOperation.Undo ʵ��
			if(this.info != null)
			{
				info.ReplaceString(this.offset,this.changedtext.Length,this.oritext);
			}
		}

		#endregion
	}
}
