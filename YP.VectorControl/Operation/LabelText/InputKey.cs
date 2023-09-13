using System;

namespace YP.VectorControl.Operation.LabelText
{
	#region ..Delete��
	/// <summary>
	/// Delete��
	/// </summary>
	internal class Delete:Action
	{
		internal override void Execute(LabelTextOperation editor)
		{
			if(editor != null)
			{
				int offset = editor.Caret.Offset;
				if(editor.Selection.IsEmpty)
				{
					if(offset < editor.CaretRender.Label.Length)
					{
						editor.RemoveString(offset,1);	
					}
				}
				else
					editor.ClearSelect();
			}
		}
	}
	#endregion

	#region ..BackSpace��
	/// <summary>
	/// BackSpace��
	/// </summary>
	internal class BackSpace:Action
	{
		internal override void Execute(LabelTextOperation editor)
		{
			if(editor != null)
			{
				int offset = editor.Caret.Offset;
				if(offset >= 0)
				{
					if(editor.Selection.IsEmpty)
					{
						offset --;
						editor.RemoveString(offset,1);
					}
					else
					{
						editor.ClearSelect(); 
					}
				}
			}
		}
	}
	#endregion

	#region ..Tab
	/// <summary>
	/// Tab��
	/// </summary>
	internal class Tab:Action
	{
		internal override void Execute(LabelTextOperation editor)
		{
			if(editor != null)
			{
				int offset = editor.Caret.Offset;
				editor.Insert(offset,"\t");
			}
		}
	}
	#endregion

	#region ..Enter
	/// <summary>
	/// Enter��
	/// </summary>
	internal class Enter:Action
	{
		internal override void Execute(LabelTextOperation editor)
		{
			if(editor != null)
			{
				int offset = editor.Caret.Offset;
				editor.Insert(offset,"\n");
			}
		}
	}
	#endregion
}
