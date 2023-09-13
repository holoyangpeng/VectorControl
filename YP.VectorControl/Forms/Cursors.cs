using System;
using System.Windows.Forms;
using System.Reflection;
using System.IO;

namespace YP.VectorControl.Forms
{
	/// <summary>
	/// Cursors 的摘要说明。
	/// </summary>
	internal class Cursors
	{
		#region ..静态变量，定义鼠标样式
		public static readonly System.Windows.Forms.Cursor EqualScale = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor AnchorMove = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor Anchor = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor Shapes = System.Windows.Forms.Cursors.Cross;
		public static readonly System.Windows.Forms.Cursor Select = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor NodeEdit = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor IncreaseView = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor DecreaseView = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor NoViewChange = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor Hand = System.Windows.Forms.Cursors.Hand;
		public static readonly System.Windows.Forms.Cursor AddAnchor = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor DelAnchor = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor Path = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor Translate = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor CloseBezier = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor ChangeControl = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor FreeDraw = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor Text = System.Windows.Forms.Cursors.IBeam;
		public static readonly System.Windows.Forms.Cursor Drag = System.Windows.Forms.Cursors.SizeAll;
		public static readonly System.Windows.Forms.Cursor DragInfo = System.Windows.Forms.Cursors.SizeAll;
		public static readonly System.Windows.Forms.Cursor ShapeDrag = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor ColorSelect = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor AreaSelect = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor VScale = System.Windows.Forms.Cursors.SizeWE;
		public static readonly System.Windows.Forms.Cursor HScale = System.Windows.Forms.Cursors.SizeNS;
		public static readonly System.Windows.Forms.Cursor TopRightScale= System.Windows.Forms.Cursors.SizeNESW;
		public static readonly System.Windows.Forms.Cursor TopLeftScale = System.Windows.Forms.Cursors.SizeNWSE;
		public static readonly System.Windows.Forms.Cursor SkewX = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor SkewY = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor Rotate = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor CenterPoint = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor MoveControl = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor PaintBottle = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor InkBottle = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor GradientTransform = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor GradientTranslate = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor ColorPicker = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor AddStop = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor Default = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor MoveRect = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor PolyAdd = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor PolyDel = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor PolyDraw = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor MovePath = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor ChangeEnd = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor VGuide = System.Windows.Forms.Cursors.Default;
		public static readonly System.Windows.Forms.Cursor HGuide = System.Windows.Forms.Cursors.Default;
        public static readonly System.Windows.Forms.Cursor TextBlock = System.Windows.Forms.Cursors.Default;

		static Cursors()
		{
            Type assembletype = Type.GetType("YP.VectorControl.Forms.Cursors");
			Cursors.EqualScale = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.ChangeR.cur");
			Cursors.AnchorMove = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.AnchorMove.cur");
			Cursors.Anchor = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.Anchor.cur");
			Cursors.Shapes = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.ShapeDraw.cur");
			Cursors.Select = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.Select.cur");
			Cursors.NodeEdit = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.Subpath.cur");
			Cursors.IncreaseView = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.IncreaseView.cur");
			Cursors.DecreaseView = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.DecreaseView.cur");
			Cursors.NoViewChange = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.NoView.cur");
			Cursors.Hand = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.Roam.cur");
			Cursors.AddAnchor = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.PenAdd.cur");
			Cursors.DelAnchor = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.PenDel.cur");
			Cursors.Path = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.PenDraw.cur");
			Cursors.Translate = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.Drag.cur");
			Cursors.CloseBezier = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.PenClose.cur");
			Cursors.ChangeControl = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.SubpathDrag.cur");
			Cursors.Text = System.Windows.Forms.Cursors.IBeam;
			Cursors.Drag = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.Drag.cur");
			Cursors.DragInfo = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.DragInfo.cur");
			Cursors.ShapeDrag = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.SubpathDrag.cur");
			Cursors.AreaSelect = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.AreaSelect.cur");
//			Cursors.HScale = LoadCursor(assembletype,"YP.Canvas.Resource.Cursor.VScale.cur");
//			Cursors.VScale = LoadCursor(assembletype,"YP.Canvas.Resource.Cursor.HScale.cur");
//			Cursors.TopRightScale= LoadCursor(assembletype,"YP.Canvas.Resource.Cursor.CCWCornerScale.cur");
//			Cursors.TopLeftScale = LoadCursor(assembletype,"YP.Canvas.Resource.Cursor.CWCornerScale.cur");
			Cursors.SkewX = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.SkewX.cur");
			Cursors.SkewY = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.SkewY.cur");
			Cursors.Rotate = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.Rotate.cur");
			Cursors.CenterPoint = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.CenterPoint.cur");
			Cursors.MoveControl = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.SubpathDrag.cur");
			Cursors.GradientTransform = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.GradientTransform.cur");
			Cursors.GradientTranslate = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.ColorTranslate.cur");
			Cursors.ColorPicker = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.ColorPicker.cur");
			Cursors.AddStop = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.AddControl.cur");
			Cursors.Default = System.Windows.Forms.Cursors.Default;
			Cursors.MoveRect = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.MoveRect.cur");
			Cursors.PolyAdd = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.polyaddDraw.cur");
			Cursors.PolyDel = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.polydelDraw.cur");
			Cursors.MovePath = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.movepath.cur");
			Cursors.ChangeEnd = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.PenEnd.cur");
			Cursors.PolyDraw  = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.polydraw.cur");
			Cursors.InkBottle = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.inkbottle.cur");
			Cursors.PaintBottle = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.paintbottle.cur");
			Cursors.VGuide = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.VGuide.cur");
			Cursors.HGuide = LoadCursor(assembletype,"YP.VectorControl.Resource.Cursor.HGuide.cur");
            Cursors.TextBlock = LoadCursor(assembletype, "YP.VectorControl.Resource.Cursor.TextDraw.cur");
		}
		#endregion

		#region ..从资源文件中导入光标
		internal static Cursor LoadCursor(Type assemblyType, string cursorName)
		{
			// Get the assembly that contains the bitmap resource
			Assembly myAssembly = Assembly.GetAssembly(assemblyType);

			// Get the resource stream containing the images
			Stream iconStream = myAssembly.GetManifestResourceStream(cursorName);

			if(iconStream == null)
				return Cursors.Default;

			// Load the Icon from the stream
			return new Cursor(iconStream);
		}
		#endregion
	}
}
