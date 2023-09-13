using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace YP.VectorControl.Forms
{
	internal class Win32
	{
		[DllImport("gdi32.dll", CharSet=CharSet.Unicode,ExactSpelling=true, SetLastError=true)]
		internal static extern int  SetROP2(IntPtr n, int i);

		[DllImport("gdi32.dll", CharSet=CharSet.Unicode,ExactSpelling=true, SetLastError=true)]
		internal static extern bool  Polygon(IntPtr n, Point[] p, int nCount);

		[DllImport("user32.dll", ExactSpelling=true, SetLastError=true)]
		internal static extern IntPtr GetDC(IntPtr hWnd);

		[DllImport("user32.dll", ExactSpelling=true, SetLastError=true)]
		internal static extern int ReleaseDC(IntPtr hWnd, IntPtr hdc);

		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		internal static extern int ShowWindow(IntPtr hWnd, short cmdShow);

		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		internal static extern bool WaitMessage();

		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		internal static extern bool TranslateMessage(ref MSG msg);

		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		internal static extern bool DispatchMessage(ref MSG msg);

		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		internal static extern bool PostMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);

		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		internal static extern uint SendMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);

		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		internal static extern bool GetMessage(ref MSG msg, int hWnd, uint wFilterMin, uint wFilterMax);
	
		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		internal static extern bool PeekMessage(ref MSG msg, int hWnd, uint wFilterMin, uint wFilterMax, uint wFlag);

		[DllImport("gdi32.dll", CharSet=CharSet.Unicode,ExactSpelling=true, SetLastError=true)]
		internal static extern bool PolyDraw(IntPtr n, Point[] p, byte[] b, int nCount);


		internal static bool Win32PolyPolygon(IntPtr hdc,System.Drawing.Drawing2D.GraphicsPath gPath)
		{
			if(gPath != null && gPath.PointCount > 0 )
			{
				using(GraphicsPath temppath = (GraphicsPath)gPath.Clone())
				{
//					temppath.Flatten(new Matrix(),0.25f);
					int nCount = temppath.PointCount;

					Point[] p = new Point[nCount];

					for(int i = 0;i<nCount;i++)
					{
						p[i] = Point.Round(temppath.PathPoints[i]);
					}

					Polygon (hdc,p,nCount);
					p = null;
				}
			}
			return true;
		}

		internal static bool W32PolyDraw(IntPtr hdc,System.Drawing.Drawing2D.GraphicsPath gPath)
		{
			if(gPath != null && gPath.PointCount > 0 )
			{
				byte[] bg  = gPath.PathTypes;
				int nCount = gPath.PointCount;

				Point[] p = new Point[nCount];
				byte[] b = new byte[nCount];

				if(nCount > 2000)
				{
					System.Drawing.Drawing2D.GraphicsPath path1 = new GraphicsPath();
					path1.AddRectangle(gPath.GetBounds());
					W32PolyDraw(hdc,path1);
					return true;
				}
				for(int i = 0;i<nCount;i++)
				{
					p[i] = new Point((int)gPath.PathPoints[i].X,(int)gPath.PathPoints[i].Y);
					switch(bg[i])
					{
						case (byte)PathPointType.Start:
							b[i] = 0x06;
							break;
						case (byte)PathPointType.Line:
							b[i] = 0x02;
							break;
						case (byte)PathPointType.Bezier3:
							b[i] = 0x04;
							break;
						case (byte)PathPointType.CloseSubpath:
							b[i] = 0x01;
							break;
						case (byte)PathPointType.Line | (byte)PathPointType.CloseSubpath:
							b[i] = 0x03;
							break;
						case (byte)PathPointType.Bezier3 |(byte)PathPointType.CloseSubpath:
							b[i] = 0x05;
							break;
					}
				}
				PolyDraw(hdc, p, b,nCount);
			}
			return true;
		}

		[StructLayout(LayoutKind.Sequential)]
			internal struct MSG 
		{
			internal IntPtr hwnd;
			internal int message;
			internal IntPtr wParam;
			internal IntPtr lParam;
			internal int time;
			internal int pt_x;
			internal int pt_y;
		}

		[StructLayout(LayoutKind.Sequential)]
			internal struct RECT
		{
			internal int left;
			internal int top;
			internal int right;
			internal int bottom;

			internal RECT(int l,int t,int r,int b)
			{
				this.left = l;
				this.top = t;
				this.right = r;
				this.bottom = b;
			}
		}

		[StructLayout(LayoutKind.Sequential)]
			internal struct POINT
		{
			internal int x;
			internal int y;
		}

		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		internal static extern bool ClientToScreen(IntPtr hWnd, ref POINT pt);

		[DllImport("User32.dll", CharSet=CharSet.Auto)]
		internal static extern bool ScreenToClient(IntPtr hWnd, ref POINT pt);
	}
}
