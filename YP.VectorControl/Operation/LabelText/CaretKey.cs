using System;
using System.Windows.Forms;

namespace YP.VectorControl.Operation.LabelText
{
	#region ..Left
	internal class Left:Action
	{
		internal override void Execute(LabelTextOperation editor)
		{
			if(editor != null)
			{
				int offset = editor.Caret.Offset;
				if(offset>0)
				{
					if((Control.ModifierKeys & Keys.Shift) != Keys.Shift)
						editor.SelectNone();
					editor.Caret.Offset --;
				}
			}
		}
	}
	#endregion

	#region ..Right
	internal class Right:Action
	{
		internal override void Execute(LabelTextOperation editor)
		{
			if(editor != null)
			{
				int offset = editor.Caret.Offset;
				if(offset<editor.CaretRender.Label.Length)
				{
					if((Control.ModifierKeys & Keys.Shift) != Keys.Shift)
						editor.SelectNone();
					editor.Caret.Offset ++;
				}
			}
		}
	}
	#endregion

	#region ..Home
	internal class Home:Action
	{
		internal override void Execute(LabelTextOperation editor)
		{
			if(editor != null)
			{
				int offset = editor.Caret.Offset;
				if(editor.CaretRender != null)
				{
					SVG.Render.SVGBaseRenderer.Line line = editor.CaretRender.SVGRenderer.FindLineAtOffset(offset); 
					if(line != null)
					{
						if((Control.ModifierKeys & Keys.Shift) != Keys.Shift)
							editor.SelectNone();
						editor.Caret.Offset = line.Offset;
					}
				}
			}
		}
	}
	#endregion

	#region ..Home
	internal class End:Action
	{
		internal override void Execute(LabelTextOperation editor)
		{
			if(editor != null)
			{
				int offset = editor.Caret.Offset;
				if(editor.CaretRender != null)
				{
					SVG.Render.SVGBaseRenderer.Line line = editor.CaretRender.SVGRenderer.FindLineAtOffset(offset); 
					if(line != null)
					{
						if((Control.ModifierKeys & Keys.Shift) != Keys.Shift)
							editor.SelectNone();
						editor.Caret.Offset = line.Offset + line.Length;
					}
				}
			}
		}
	}
	#endregion

	#region ..Up
	internal class Up:Action
	{
		internal override void Execute(LabelTextOperation editor)
		{
			if(editor != null)
			{
				int offset = editor.Caret.Offset;
				if(editor.CaretRender != null)
				{
					if((Control.ModifierKeys & Keys.Shift) != Keys.Shift)
						editor.SelectNone();
					editor.MoveVertically(true);
				}
			}
		}
	}
	#endregion

	#region ..Down
	internal class Down:Action
	{
		internal override void Execute(LabelTextOperation editor)
		{
			if(editor != null)
			{
				int offset = editor.Caret.Offset;
				if(editor.CaretRender != null)
				{
					if((Control.ModifierKeys & Keys.Shift) != Keys.Shift)
						editor.SelectNone();
					editor.MoveVertically(false);
				}
			}
		}
	}
	#endregion
}
