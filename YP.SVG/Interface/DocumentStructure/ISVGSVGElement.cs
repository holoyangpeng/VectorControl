using System;
using System.Xml;

namespace YP.SVG.Interface.DocumentStructure
{
	/// <summary>
	/// 定义SVG对象的一般行为
	/// </summary>
	public interface ISVGSVGElement:
		ISVGElement,
		ISVGTests,
		ISVGLangSpace,
		ISVGExternalResourcesRequired,
		ISVGStylable,
		ISVGLocatable,
		ISVGFitToViewBox,
		ISVGZoomAndPan
	{
		DataType.ISVGLength X{get;}
		DataType.ISVGLength Y{get;}
		DataType.ISVGLength Width{get;}
		DataType.ISVGLength Height{get;}
		 
		string ContentScriptType{get;}
		string ContentStyleType{get;}
		 
		SVG.DataType.SVGRect Viewport{get;}

		float PixelUnitToMillimeterX{get;}
		float PixelUnitToMillimeterY{get;}
		float ScreenPixelToMillimeterX{get;}
		float ScreenPixelToMillimeterY{get;}

		bool UseCurrentView{get;set;}
		ISVGViewSpec CurrentView{get;}

		float CurrentScale{get;set;}
		CTS.ISVGPoint CurrentTranslate{get;}

		int SuspendRedraw(int max_wait_milliseconds);
		void UnsuspendRedraw(int suspend_handle_id);
		void UnsuspendRedrawAll();
		void ForceRedraw();
		void PauseAnimations();
		void UnpauseAnimations();
		bool AnimationsPaused();
		float GetCurrentTime();

		XmlNodeList GetIntersectionList(DataType.ISVGRect rect, ISVGElement referenceElement);
		XmlNodeList GetEnclosureList(DataType.ISVGRect rect, ISVGElement referenceElement);
		bool CheckIntersection(ISVGElement element, DataType.ISVGRect rect);
		bool CheckEnclosure(ISVGElement element, DataType.ISVGRect rect);
		void DeselectAll();
		float CreateSVGNumber();
		DataType.ISVGLength CreateSVGLength();
		DataType.ISVGAngle CreateSVGAngle();
		CTS.ISVGPoint CreateSVGPoint();
		CTS.ISVGMatrix CreateSVGMatrix();
		DataType.ISVGRect CreateSVGRect();
		CTS.ISVGTransform CreateSVGTransform();
		CTS.ISVGTransform CreateSVGTransformFromMatrix(CTS.ISVGMatrix matrix);
		XmlElement GetElementById(string elementId);
	}
}
