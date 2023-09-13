using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Xml;

using YP.SVG.Interface.DataType;
using YP.SVG.Interface.CTS;
using YP.SVG.DataType;
using YP.SVG.Interface;

namespace YP.SVG.DocumentStructure
{
	/// <summary>
	/// 实现SVG中的SVG对象
	/// </summary>
	public class SVGSVGElement:SVGGElement,
        Interface.DocumentStructure.ISVGSVGElement,
        Interface.ISVGPathable, 
        Interface.ISVGTextBlockContainer
	{
		#region ..构造及消除
		public SVGSVGElement(string prefix, string localname, string ns, Document.SVGDocument doc) : base(prefix, localname, ns, doc) 
		{
			//
			// TODO: 在此处添加构造函数逻辑
			//
			this.canRender = true;
			this.x = new DataType.SVGLength("0",this,LengthDirection.Hori);
			this.y = new DataType.SVGLength("0",this,LengthDirection.Vect);
			this.width = new DataType.SVGLength("100%",this,LengthDirection.Hori);
			this.height = new DataType.SVGLength("100%",this,LengthDirection.Vect);
			this.per = new SVGPreserveAspectRatio("xMidYMid meet");//,this);

            this.render = new Render.SVGSVGRenderer(this);
		}
		#endregion

		#region ..私有变量
		DataType.SVGLength x,y,width,height;
		SVGPreserveAspectRatio per;
		DataType.SVGRect viewBox = new SVGRect(RectangleF.Empty);
        Size viewSize = new Size(800, 600);
		#endregion

        #region ..ISVGPathable
        Render.SVGSVGRenderer render;

        public override Render.SVGBaseRenderer SVGRenderer
        {
            get { return this.render; }
        }

        /// <summary>
        /// 获取绘制路径
        /// </summary>
        System.Drawing.Drawing2D.GraphicsPath Interface.ISVGPathable.GPath
        {
            get
            {
                return this.GetGPath();
            }
        }
        #endregion

        #region ..properties
        public Size WindowViewSize
        {
            set
            {
                viewSize = value;
            }
        }

        public Interface.DataType.ISVGRect ViewBox
		{
			get
			{
				string attr = GetAttribute("viewBox").Trim();
				if(this.viewBox.GDIRect.IsEmpty)
				{
					RectangleF rect = new RectangleF(
						X.Value, 
						Y.Value, 
						Width.Value, 
						Height.Value);
					this.viewBox = new SVGRect(rect);//,this);
				}
				return this.viewBox;
			}
		}


		public SVGZoomAndPanType ZoomAndPan
		{
			get
			{
				throw new NotImplementedException();
			}
		}
		#endregion

		#region ..Implementation of ISVGSVGElement
		public ISVGPreserveAspectRatio PreserveAspectRatio
		{
			get
			{
				return this.per;//new DataType.SVGPreserveAspectRatio(GetAttribute("preserveAspectRatio"),this);
			}
		}

		/// <summary>
		/// Corresponds to attribute x on the given 'svg' element.
		/// </summary>
		public ISVGLength X
		{
			get
			{
				return this.x;//new SVGLength(this.GetAttribute("x"), this, LengthDirection.Hori, "0px");
			}
		}
		/// <summary>
		/// Corresponds to attribute y on the given 'svg' element.
		/// </summary>
		public ISVGLength Y
		{
			get
			{
				return this.y;//new SVGLength(this.GetAttribute("y"), this, LengthDirection.Vect, "0px");
			}
		}

		private string WidthAsString
		{
			get
			{
				 return GetAttribute("width");
			}
		}

		/// <summary>
		/// Corresponds to attribute width on the given 'svg' element.
		/// </summary>
		public ISVGLength Width
		{
			get
			{
				return this.width;//new SVGLength(WidthAsString, this, LengthDirection.Hori, "100%");
			}
		}


		private string HeightAsString
		{
			get
			{
				return GetAttribute("height");	
			}
		}
		/// <summary>
		/// Corresponds to attribute height on the given 'svg' element.
		/// </summary>
		public ISVGLength Height
		{
			get
			{
				return this.height;//new SVGLength(HeightAsString, this, LengthDirection.Vect, "100%");
			}
		}		

		/// <summary>
		/// Corresponds to attribute contentScriptType on the given 'svg' element
		/// </summary>
		/// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised on an attempt to change the value of a readonly attribute.</exception>
		public string ContentScriptType
		{
			get
			{
				return GetAttribute("contentScriptType");
			}
		}

		/// <summary>
		/// Corresponds to attribute contentStyleType on the given 'svg' element.
		/// </summary>
		/// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised on an attempt to change the value of a readonly attribute.</exception>
		public string ContentStyleType
		{
			get
			{
				return GetAttribute("contentStyleType");
			}
		}

		private float GetViewportProp(string propertyName, string inValue, float calcParentVP, double defaultValue, LengthDirection dir)
		{
			double ret;
			inValue = inValue.Trim();

			if(inValue.Length > 0)
			{
				if(inValue.EndsWith("%"))
				{
					double perc = DataType.SVGNumber.ParseNumberStr(inValue.Substring(0, inValue.Length-1)) / 100;
					ret = calcParentVP * perc;
				}
				else
				{
					ret = new DataType.SVGLength(inValue, this, dir).Value;
				}
			}
			else ret = defaultValue;

			return (float)ret;
		}

		/// <summary>
		/// The position and size of the viewport (implicit or explicit) that corresponds to this 'svg' element. When the user agent is actually rendering the content, then the position and size values represent the actual values when rendering. The position and size values are unitless values in the coordinate system of the parent element. If no parent element exists (i.e., 'svg' element represents the root of the document tree), if this SVG document is embedded as part of another document (e.g., via the HTML 'object' element), then the position and size are unitless values in the coordinate system of the parent document. (If the parent uses CSS or XSL layout, then unitless values represent pixel units for the current CSS or XSL viewport, as described in the CSS2 specification.) If the parent element does not have a coordinate system, then the user agent should provide reasonable default values for this attribute.
		/// The object itself and its contents are both readonly.
		/// </summary>
		public SVGRect Viewport
		{
			get
			{
				RectangleF rect = RectangleF.Empty;
				Interface.ISVGElement view = this.ViewPortElement;
				double calcParentVPWidth = (view == null) ?
                    viewSize.Width : ((SVGSVGElement)view).Viewport.Width;

				double calcParentVPHeight = (view == null) ?
                    viewSize.Height : ((SVGSVGElement)view).Viewport.Height;
				
				return new SVGRect(new RectangleF(
					GetViewportProp(
					"x", 
					GetAttribute("x"), 
					(float) calcParentVPWidth, 
					0, 
					LengthDirection.Hori),
					GetViewportProp(
					"y", 
					GetAttribute("y"), 
					(float) calcParentVPHeight, 
					0, 
					LengthDirection.Vect),
					this.GetViewportProp(
					"width", 
					WidthAsString,
                    (float)calcParentVPWidth, viewSize.Width,
					LengthDirection.Hori),
					GetViewportProp(
					"height", 
					HeightAsString,
                    (float)calcParentVPHeight, viewSize.Height,
					LengthDirection.Vect))
					);
			}
		}

		/// <summary>
		/// Size of a pixel units (as defined by CSS2) along the x-axis of the viewport, which represents a unit somewhere in the range of 70dpi to 120dpi, and, on systems that support this, might actually match the characteristics of the target medium. On systems where it is impossible to know the size of a pixel, a suitable default pixel size is provided.
		/// </summary>
		public float PixelUnitToMillimeterX
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}
		
		/// <summary>
		/// Corresponding size of a pixel unit along the y-axis of the viewport.
		/// </summary>
		public float PixelUnitToMillimeterY
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// User interface (UI) events in DOM Level 2 indicate the screen positions at which the given UI event occurred. When the user agent actually knows the physical size of a "screen unit", this attribute will express that information; otherwise, user agents will provide a suitable default value such as .28mm.
		/// </summary>
		public float ScreenPixelToMillimeterX
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// Corresponding size of a screen pixel along the y-axis of the viewport.
		/// </summary>
		public float ScreenPixelToMillimeterY
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// The initial view (i.e., before magnification and panning) of the current innermost SVG document fragment can be either the "standard" view (i.e., based on attributes on the 'svg' element such as fitBoxToViewport) or to a "custom" view (i.e., a hyperlink into a particular 'view' or other element - see Linking into SVG content: URI fragments and SVG views). If the initial view is the "standard" view, then this attribute is false. If the initial view is a "custom" view, then this attribute is true.
		/// </summary>
		/// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised on an attempt to change the value of a readonly attribute.</exception>
		public bool UseCurrentView
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// The definition of the initial view (i.e., before magnification and panning) of the current innermost SVG document fragment. The meaning depends on the situation:
		/// * If the initial view was a "standard" view, then:
		///  o the values for viewBox, preserveAspectRatio and zoomAndPan within currentView will match the values for the corresponding DOM attributes that are on SVGSVGElement directly
		///  o the values for transform and viewTarget within currentView will be null
		/// * If the initial view was a link into a 'view' element, then:
		///  o the values for viewBox, preserveAspectRatio and zoomAndPan within currentView will correspond to the corresponding attributes for the given 'view' element
		///  o the values for transform and viewTarget within currentView will be null
		/// * If the initial view was a link into another element (i.e., other than a 'view'), then:
		///  o the values for viewBox, preserveAspectRatio and zoomAndPan within currentView will match the values for the corresponding DOM attributes that are on SVGSVGElement directly for the closest ancestor 'svg' element
		///  o the values for transform within currentView will be null
		///  o the viewTarget within currentView will represent the target of the link
		/// * If the initial view was a link into the SVG document fragment using an SVG view specification fragment identifier (i.e., #svgView(...)), then:
		///  o the values for viewBox, preserveAspectRatio, zoomAndPan, transform and viewTarget within currentView will correspond to the values from the SVG view specification fragment identifier
		/// The object itself and its contents are both readonly. 
		/// </summary>
		public ISVGViewSpec CurrentView
		{
			get
			{
				throw new NotImplementedException();
			}
    		
		}


		/// <summary>
		/// This attribute indicates the current scale factor relative to the initial view to take into account user magnification and panning operations, as described under Magnification and panning. DOM attributes currentScale and currentTranslate are equivalent to the 2x3 matrix [a b c d e f] = [currentScale 0 0 currentScale currentTranslate.x currentTranslate.y]. If "magnification" is enabled (i.e., zoomAndPan="magnify"), then the effect is as if an extra transformation were placed at the outermost level on the SVG document fragment (i.e., outside the outermost 'svg' element).
		/// </summary>
		/// <exception cref="DomException">NO_MODIFICATION_ALLOWED_ERR: Raised on an attempt to change the value of a readonly attribute</exception>
		public float CurrentScale
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>
		/// The corresponding translation factor that takes into account user "magnification".
		/// </summary>
		public ISVGPoint CurrentTranslate
		{
			get
			{
				throw new NotImplementedException();
			}
    		
		}

		/// <summary>
		/// Takes a time-out value which indicates that redraw shall not occur until: (a) the corresponding unsuspendRedraw(suspend_handle_id) call has been made, (b) an unsuspendRedrawAll() call has been made, or (c) its timer has timed out. In environments that do not support interactivity (e.g., print media), then redraw shall not be suspended. suspend_handle_id = suspendRedraw(max_wait_milliseconds) and unsuspendRedraw(suspend_handle_id) must be packaged as balanced pairs. When you want to suspend redraw actions as a collection of SVG DOM changes occur, then precede the changes to the SVG DOM with a method call similar to suspend_handle_id = suspendRedraw(max_wait_milliseconds) and follow the changes with a method call similar to unsuspendRedraw(suspend_handle_id). Note that multiple suspendRedraw calls can be used at once and that each such method call is treated independently of the other suspendRedraw method calls.
		/// </summary>
		/// <param name="max_wait_milliseconds">The amount of time in milliseconds to hold off before redrawing the device. Values greater than 60 seconds will be truncated down to 60 seconds.</param>
		/// <returns>A number which acts as a unique identifier for the given suspendRedraw() call. This value must be passed as the parameter to the corresponding unsuspendRedraw() method call.</returns>
		public int SuspendRedraw(int max_wait_milliseconds)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Cancels a specified suspendRedraw() by providing a unique suspend_handle_id.
		/// </summary>
		/// <param name="suspend_handle_id">A number which acts as a unique identifier for the desired suspendRedraw() call. The number supplied must be a value returned from a previous call to suspendRedraw()</param>
		/// <exception cref="DomException">This method will raise a DOMException with value NOT_FOUND_ERR if an invalid value (i.e., no such suspend_handle_id is active) for suspend_handle_id is provided.</exception>
		public void UnsuspendRedraw(int suspend_handle_id)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Cancels all currently active suspendRedraw() method calls. This method is most useful at the very end of a set of SVG DOM calls to ensure that all pending suspendRedraw() method calls have been cancelled.
		/// </summary>
		public void UnsuspendRedrawAll()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// In rendering environments supporting interactivity, forces the user agent to immediately redraw all regions of the viewport that require updating.
		/// </summary>
		public void ForceRedraw()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Suspends (i.e., pauses) all currently running animations that are defined within the SVG document fragment corresponding to this 'svg' element, causing the animation clock corresponding to this document fragment to stand still until it is unpaused.
		/// </summary>
		public void PauseAnimations()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Unsuspends (i.e., unpauses) currently running animations that are defined within the SVG document fragment, causing the animation clock to continue from the time at which it was suspended.
		/// </summary>
		public void UnpauseAnimations()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns true if this SVG document fragment is in a paused state
		/// </summary>
		/// <returns>Boolean indicating whether this SVG document fragment is in a paused state.</returns>
		public bool AnimationsPaused()
		{
			throw new NotImplementedException();
		}

		
		/// <summary>
		/// Returns the current time in seconds relative to the start time for the current SVG document fragment.
		/// </summary>
		/// <returns>The current time in seconds.</returns>
		public float GetCurrentTime()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Adjusts the clock for this SVG document fragment, establishing a new current time.
		/// </summary>
		/// <param name="seconds">The new current time in seconds relative to the start time for the current SVG document fragment.</param>
		public void SetCurrentTime(float seconds)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns the list of graphics elements whose rendered content intersects the supplied rectangle, honoring the 'pointer-events' property value on each candidate graphics element.
		/// </summary>
		/// <param name="rect">The test rectangle. The values are in the initial coordinate system for the current 'svg' element.</param>
		/// <param name="referenceElement">If not null, then only return elements whose drawing order has them below the given reference element.</param>
		/// <returns>A list of Elements whose content intersects the supplied rectangle.</returns>
		public XmlNodeList GetIntersectionList(ISVGRect rect, ISVGElement referenceElement)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns the list of graphics elements whose rendered content is entirely contained within the supplied rectangle, honoring the 'pointer-events' property value on each candidate graphics element.
		/// </summary>
		/// <param name="rect">The test rectangle. The values are in the initial coordinate system for the current 'svg' element.</param>
		/// <param name="referenceElement">If not null, then only return elements whose drawing order has them below the given reference element.</param>
		/// <returns>A list of Elements whose content is enclosed by the supplied rectangle.</returns>
		public XmlNodeList GetEnclosureList(ISVGRect rect, ISVGElement referenceElement)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns true if the rendered content of the given element intersects the supplied rectangle, honoring the 'pointer-events' property value on each candidate graphics element.
		/// </summary>
		/// <param name="element">The element on which to perform the given test.</param>
		/// <param name="rect">The test rectangle. The values are in the initial coordinate system for the current 'svg' element.</param>
		/// <returns>True or false, depending on whether the given element intersects the supplied rectangle.</returns>
		public bool CheckIntersection(ISVGElement element, ISVGRect rect)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns true if the rendered content of the given element is entirely contained within the supplied rectangle, honoring the 'pointer-events' property value on each candidate graphics element.
		/// </summary>
		/// <param name="element">The element on which to perform the given test</param>
		/// <param name="rect">The test rectangle. The values are in the initial coordinate system for the current 'svg' element.</param>
		/// <returns>True or false, depending on whether the given element is enclosed by the supplied rectangle.</returns>
		public bool CheckEnclosure(ISVGElement element, ISVGRect rect)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Unselects any selected objects, including any selections of text strings and type-in bars.
		/// </summary>
		public void DeselectAll()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Creates an SVGNumber object outside of any document trees. The object is initialized to a value of zero.
		/// </summary>
		/// <returns>An SVGNumber object.</returns>
		public float CreateSVGNumber()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Creates an SVGLength object outside of any document trees. The object is initialized to the value of 0 user units.
		/// </summary>
		/// <returns>An SVGLength object.</returns>
		public ISVGLength CreateSVGLength()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Creates an SVGAngle object outside of any document trees. The object is initialized to the value 0 degrees (unitless).
		/// </summary>
		/// <returns>An SVGAngle object.</returns>
		public ISVGAngle CreateSVGAngle()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Creates an SVGPoint object outside of any document trees. The object is initialized to the point (0,0) in the user coordinate system.
		/// </summary>
		/// <returns>An SVGPoint object.</returns>
		public ISVGPoint CreateSVGPoint()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Creates an SVGMatrix object outside of any document trees. The object is initialized to the identity matrix.
		/// </summary>
		/// <returns>An SVGMatrix object.</returns>
		public ISVGMatrix CreateSVGMatrix()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Creates an SVGRect object outside of any document trees. The object is initialized such that all values are set to 0 user units.
		/// </summary>
		/// <returns>An SVGRect object.</returns>
		public ISVGRect CreateSVGRect()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Creates an SVGTransform object outside of any document trees. The object is initialized to an identity matrix transform (SVG_TRANSFORM_MATRIX).
		/// </summary>
		/// <returns>An SVGTransform object.</returns>
		public ISVGTransform CreateSVGTransform()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Creates an SVGTransform object outside of any document trees. The object is initialized to the given matrix transform (i.e., SVG_TRANSFORM_MATRIX).
		/// </summary>
		/// <param name="matrix">The transform matrix.</param>
		/// <returns>An SVGTransform object.</returns>
		public ISVGTransform CreateSVGTransformFromMatrix(ISVGMatrix matrix)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Searches this SVG document fragment (i.e., the search is restricted to a subset of the document tree) for an Element whose id is given by elementId. If an Element is found, that Element is returned. If no such element exists, returns null. Behavior is not defined if more than one element has this id.
		/// </summary>
		/// <param name="elementId">The unique id value for an element.</param>
		/// <returns>The matching element.</returns>
		public XmlElement GetElementById(string elementId)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region ..属性操作
		public override void SetSVGAttribute(string attributeName, string attributeValue)
		{
			switch(attributeName)
			{
				case "x":
					this.x = new SVGLength(attributeValue,this,YP.SVG.LengthDirection.Hori);
					break;
				case "y":
					this.y = new SVGLength(attributeValue,this,LengthDirection.Vect);
					break;
				case "width":
					this.width = new SVGLength(attributeValue,this,LengthDirection.Hori,"100%");
					break;
				case "height":
					this.height = new SVGLength(attributeValue,this,LengthDirection.Vect,"100%");
					break;
				case "preserveAspectRatio":
					this.per = new SVGPreserveAspectRatio(attributeValue);//,this);
					break;
				case "viewBox":
					this.viewBox = new SVGRect(attributeValue);
					break;
			}
			base.SetSVGAttribute (attributeName, attributeValue);
		}
		#endregion

        #region ..AttributeChangeTest
        public override AttributeChangedResult AttributeChangeTest(string attributeName)
        {
            if (string.Compare(attributeName,"width")==0 ||string.Compare(attributeName,"height") ==0)
                return AttributeChangedResult.GraphicsPathChanged;
            return base.AttributeChangeTest(attributeName);
        }
        #endregion

        #region ..ISVGViewport
        DataType.SVGViewport Interface.ISVGTextBlockContainer.Viewport
        {
            get
            {
                return new SVGViewport(this.Viewport, 0);
            }
        }
        #endregion
    }
}
